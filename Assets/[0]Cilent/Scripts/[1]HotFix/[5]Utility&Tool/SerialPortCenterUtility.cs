using System;
using System.Text;
using System.Linq;
using QFramework;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// 蓝牙加密狗串口工具类接口
/// </summary>
public interface ISerialPortCenterUtility : ISerialPortBaseUtility
{
    event Action<bool> OnEncryptionStatusChanged;
    event Action<byte[]> OnBluetoothDataReceived;
    
    UniTask<bool> RequestUnlockAsync();
    bool SendBluetoothData(byte[] data);
}

/// <summary>
/// 蓝牙加密狗串口工具类
/// 实现握手、加密狗和蓝牙透传三大功能
/// </summary>
public class SerialPortCenterUtility : SerialPortBaseUtility, ISerialPortCenterUtility
{
    // 协议常量
    private const byte FRAME_HEADER = 0xAA;
    private const byte FRAME_TAIL = 0x7E;
    private const byte HANDSHAKE_FUNC_CODE = 0x01;
    private const byte ENCRYPT_REQUEST_CODE = 0x01;
    private const byte ENCRYPT_RESPONSE_CODE = 0x02;
    private const byte BLUETOOTH_FUNC_CODE = 0x03;
    
    // 握手指令
    private static readonly byte[] HANDSHAKE_COMMAND = { 0x57, 0x73, 0x3A, 0x7E }; // "ws:~"
    private static readonly byte[] HANDSHAKE_RESPONSE = { 0x57, 0x73, 0x3A, 0x7E };
    
    // 状态变量
    private bool bIsEncrypted = false;
    private bool bHandshakeCompleted = false;
    private string sDeviceId = "";
    
    public event Action<bool> OnEncryptionStatusChanged;
    public event Action<byte[]> OnBluetoothDataReceived;
    
    protected override string DeviceName => "蓝牙加密狗";
    
    /// <summary>
    /// 设备识别逻辑 - 发送握手指令
    /// </summary>
    protected override async UniTask<bool> RecognizeDevice(string portName, int portSkip)
    {
        try
        {
            // 重置状态
            bHandshakeCompleted = false;
            
            // 发送握手指令
            if (!SendData(HANDSHAKE_COMMAND))
            {
                Debug.LogError($"[{DeviceName}] 发送握手指令失败");
                return false;
            }
            
            Debug.Log($"[{DeviceName}] 已发送握手指令: {BitConverter.ToString(HANDSHAKE_COMMAND)}");
            
            // 等待握手响应
            var timeoutTask = UniTask.Delay(ConnectionTimeoutMs);
            var handshakeTask = WaitForHandshake();
            
            var result = await UniTask.WhenAny(handshakeTask, timeoutTask);
            
            if (result == 0 && bHandshakeCompleted)
            {
                Debug.Log($"[{DeviceName}] 握手成功，设备识别完成");
                return true;
            }
            
            Debug.LogWarning($"[{DeviceName}] 握手超时或失败");
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{DeviceName}] 设备识别异常: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 等待握手完成
    /// </summary>
    private async UniTask WaitForHandshake()
    {
        while (!bHandshakeCompleted)
        {
            await UniTask.Delay(50);
        }
    }
    
    /// <summary>
    /// 处理接收到的数据
    /// </summary>
    protected override void ProcessReceivedData(byte[] data)
    {
        try
        {
            // 检查握手响应
            if (!bHandshakeCompleted && data.Length >= 4)
            {
                if (data.Take(4).SequenceEqual(HANDSHAKE_RESPONSE))
                {
                    bHandshakeCompleted = true;
                    Debug.Log($"[{DeviceName}] 收到握手响应");
                    return;
                }
            }
            
            // 解析协议帧
            if (data.Length >= 5 && data[0] == FRAME_HEADER && data[data.Length - 1] == FRAME_TAIL)
            {
                ParseProtocolFrame(data);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{DeviceName}] 处理数据异常: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 解析协议帧
    /// </summary>
    private void ParseProtocolFrame(byte[] frame)
    {
        if (frame.Length < 5) return;
        
        byte ucFunctionCode = frame[1];
        
        switch (ucFunctionCode)
        {
            case ENCRYPT_RESPONSE_CODE:
                HandleEncryptResponse(frame);
                break;
                
            case BLUETOOTH_FUNC_CODE:
                HandleBluetoothData(frame);
                break;
                
            default:
                Debug.LogWarning($"[{DeviceName}] 未知功能码: 0x{ucFunctionCode:X2}");
                break;
        }
    }
    
    /// <summary>
    /// 处理加密响应
    /// </summary>
    private void HandleEncryptResponse(byte[] frame)
    {
        try
        {
            // 帧格式: 0xAA | 0x02 | 时间戳(4字节) | 状态码(1字节) | 校验和 | 0x7E
            if (frame.Length != 8)
            {
                Debug.LogError($"[{DeviceName}] 加密响应帧长度错误: {frame.Length}");
                return;
            }
            
            // 验证校验和
            if (!VerifyChecksum(frame))
            {
                Debug.LogError($"[{DeviceName}] 加密响应校验和错误");
                return;
            }
            
            byte ucStatusCode = frame[6]; // 状态码位置
            bool bSuccess = (ucStatusCode == 0x01);
            
            bIsEncrypted = bSuccess;
            OnEncryptionStatusChanged?.Invoke(bSuccess);
            
            Debug.Log($"[{DeviceName}] 加密验证{(bSuccess ? "成功" : "失败")}，状态码: 0x{ucStatusCode:X2}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{DeviceName}] 处理加密响应异常: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 处理蓝牙数据
    /// </summary>
    private void HandleBluetoothData(byte[] frame)
    {
        try
        {
            // 帧格式: 0xAA | 0x03 | 0x16(长度) | Data区 | 校验和 | 0x7E
            if (frame.Length < 5)
            {
                Debug.LogError($"[{DeviceName}] 蓝牙数据帧长度错误: {frame.Length}");
                return;
            }
            
            byte ucLength = frame[2];
            if (frame.Length != ucLength + 5) // 头+功能码+长度+数据+校验+尾
            {
                Debug.LogError($"[{DeviceName}] 蓝牙数据帧长度不匹配: 期望{ucLength + 5}, 实际{frame.Length}");
                return;
            }
            
            // 验证校验和
            if (!VerifyChecksum(frame))
            {
                Debug.LogError($"[{DeviceName}] 蓝牙数据校验和错误");
                return;
            }
            
            // 提取数据区
            byte[] data = new byte[ucLength];
            Array.Copy(frame, 3, data, 0, ucLength);
            
            OnBluetoothDataReceived?.Invoke(data);
            Debug.Log($"[{DeviceName}] 收到蓝牙数据: {BitConverter.ToString(data)}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{DeviceName}] 处理蓝牙数据异常: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 请求解锁（加密狗功能）
    /// </summary>
    public async UniTask<bool> RequestUnlockAsync()
    {
        if (!isConnected)
        {
            Debug.LogError($"[{DeviceName}] 设备未连接，无法请求解锁");
            return false;
        }
        
        try
        {
            // 获取Unity设备ID
            sDeviceId = SystemInfo.deviceUniqueIdentifier;
            if (string.IsNullOrEmpty(sDeviceId))
            {
                Debug.LogError($"[{DeviceName}] 无法获取设备ID");
                return false;
            }
            
            // 构建解锁请求帧
            byte[] unlockFrame = BuildUnlockFrame();
            
            // 重置加密状态
            bIsEncrypted = false;
            
            // 发送解锁请求
            if (!SendData(unlockFrame))
            {
                Debug.LogError($"[{DeviceName}] 发送解锁请求失败");
                return false;
            }
            
            Debug.Log($"[{DeviceName}] 已发送解锁请求: {BitConverter.ToString(unlockFrame)}");
            
            // 等待响应
            int uwTimeoutMs = 5000; // 5秒超时
            int uwElapsedMs = 0;
            
            while (!bIsEncrypted && uwElapsedMs < uwTimeoutMs)
            {
                await UniTask.Delay(100);
                uwElapsedMs += 100;
            }
            
            return bIsEncrypted;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{DeviceName}] 请求解锁异常: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 构建解锁请求帧
    /// </summary>
    private byte[] BuildUnlockFrame()
    {
        // 帧格式: 0xAA | 0x01 | 时间戳(4字节) | 加密设备ID(6字节) | 校验和 | 0x7E
        byte[] frame = new byte[13];
        
        frame[0] = FRAME_HEADER;
        frame[1] = ENCRYPT_REQUEST_CODE;
        
        // 生成时间戳（当前时间的Unix时间戳）
        uint uwTimestamp = (uint)((DateTimeOffset.UtcNow.ToUnixTimeSeconds()) & 0xFFFFFFFF);
        byte[] timestampBytes = BitConverter.GetBytes(uwTimestamp);
        if (BitConverter.IsLittleEndian)            Array.Reverse(timestampBytes); // 转为大端序
        
        Array.Copy(timestampBytes, 0, frame, 2, 4);
        
        // 加密设备ID
        byte[] encryptedDeviceId = EncryptDeviceId(sDeviceId, uwTimestamp);
        Array.Copy(encryptedDeviceId, 0, frame, 6, 6);
        
        // 计算校验和
        frame[12] = CalculateChecksum(frame, 12);
        frame[13] = FRAME_TAIL;
        
        return frame;
    }
    
    /// <summary>
    /// 加密设备ID
    /// </summary>
    private byte[] EncryptDeviceId(string deviceId, uint uwTimestamp)
    {
        // 将设备ID转换为6字节
        byte[] deviceBytes = new byte[6];
        byte[] hash = System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(deviceId));
        Array.Copy(hash, 0, deviceBytes, 0, 6); // 取MD5的前6字节
        
        // 时间戳循环加密：取低6字节循环
        byte[] timestampBytes = BitConverter.GetBytes(uwTimestamp);
        byte[] encryptKey = new byte[6];
        for (int i = 0; i < 6; i++)
        {
            encryptKey[i] = timestampBytes[i % 4];
        }
        
        // 异或加密
        byte[] encrypted = new byte[6];
        for (int i = 0; i < 6; i++)
        {
            encrypted[i] = (byte)(deviceBytes[i] ^ encryptKey[i]);
        }
        
        Debug.Log($"[{DeviceName}] 设备ID: {deviceId}");
        Debug.Log($"[{DeviceName}] 设备字节: {BitConverter.ToString(deviceBytes)}");
        Debug.Log($"[{DeviceName}] 时间戳: 0x{uwTimestamp:X8}");
        Debug.Log($"[{DeviceName}] 加密密钥: {BitConverter.ToString(encryptKey)}");
        Debug.Log($"[{DeviceName}] 加密结果: {BitConverter.ToString(encrypted)}");
        
        return encrypted;
    }
    
    /// <summary>
    /// 发送蓝牙数据
    /// </summary>
    public bool SendBluetoothData(byte[] data)
    {
        if (!isConnected)
        {
            Debug.LogError($"[{DeviceName}] 设备未连接，无法发送蓝牙数据");
            return false;
        }
        
        if (data == null || data.Length == 0 || data.Length > 255)
        {
            Debug.LogError($"[{DeviceName}] 蓝牙数据长度无效: {data?.Length ?? 0}");
            return false;
        }
        
        try
        {
            // 构建蓝牙数据帧: 0xAA | 0x03 | 长度 | Data区 | 校验和 | 0x7E
            byte[] frame = new byte[data.Length + 5];
            
            frame[0] = FRAME_HEADER;
            frame[1] = BLUETOOTH_FUNC_CODE;
            frame[2] = (byte)data.Length;
            
            Array.Copy(data, 0, frame, 3, data.Length);
            
            frame[frame.Length - 2] = CalculateChecksum(frame, frame.Length - 2);
            frame[frame.Length - 1] = FRAME_TAIL;
            
            bool bSuccess = SendData(frame);
            if (bSuccess)
            {
                Debug.Log($"[{DeviceName}] 发送蓝牙数据成功: {BitConverter.ToString(data)}");
            }
            
            return bSuccess;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{DeviceName}] 发送蓝牙数据异常: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 计算校验和
    /// </summary>
    private byte CalculateChecksum(byte[] data, int length)
    {
        byte ucChecksum = 0;
        for (int i = 0; i < length; i++)
        {
            ucChecksum ^= data[i];
        }
        return ucChecksum;
    }
    
    /// <summary>
    /// 验证校验和
    /// </summary>
    private bool VerifyChecksum(byte[] frame)
    {
        if (frame.Length < 3) return false;
        
        byte ucExpectedChecksum = CalculateChecksum(frame, frame.Length - 2);
        byte ucActualChecksum = frame[frame.Length - 2];
        
        return ucExpectedChecksum == ucActualChecksum;
    }
    
    /// <summary>
    /// 断开连接时重置状态
    /// </summary>
    public override void Disconnect()
    {
        bIsEncrypted = false;
        bHandshakeCompleted = false;
        sDeviceId = "";
        
        OnEncryptionStatusChanged?.Invoke(false);
        
        base.Disconnect();
    }
} 