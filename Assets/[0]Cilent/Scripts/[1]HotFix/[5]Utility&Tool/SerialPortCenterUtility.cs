using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
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
    private static readonly byte[] HANDSHAKE_COMMAND = { 0x57, 0x73, 0x3A ,0x7E}; // "ws:~"
    private static readonly byte[] HANDSHAKE_RESPONSE = { 0x57, 0x73, 0x3A };
    
    // 状态变量
    private bool bIsEncrypted = false;
    private bool bHandshakeCompleted = false;
    
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
            if (!bHandshakeCompleted && data.Length >= 3)
            {
                if (data.Take(3).SequenceEqual(HANDSHAKE_RESPONSE))
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
            // 使用SerialPortUtilityManager中的解析方法
            if (ParseDongleResponse(frame, out bool authSuccess))
            {
                bIsEncrypted = authSuccess;
                OnEncryptionStatusChanged?.Invoke(authSuccess);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{DeviceName}] 处理加密响应异常: {ex.Message}");
        }
    }

    /// <summary>
    /// 解析加密狗响应
    /// </summary>
    private bool ParseDongleResponse(byte[] data, out bool authSuccess)
    {
        authSuccess = false;

        if (data.Length < 8) return false;
        if (data[0] != FRAME_HEADER) return false;
        if (data[1] != ENCRYPT_RESPONSE_CODE) return false;
        
        Debug.Log($"[{DeviceName}] 收到加密狗响应: {data[0]}, 原始数据: {FRAME_HEADER}");
        
        // 校验和验证(除帧头帧尾外的所有字节相加取低八位)
        byte checksum = 0;
        for (int i = 1; i < data.Length - 1; i++)
        {
            checksum += data[i];
        }
        
        Debug.Log($"[{DeviceName}] 校验和: {checksum}, 数据: {data[data.Length - 1]}");
        if (checksum != data[data.Length - 1]) return false;

        // 解析状态码
        byte statusCode = data[6]; // 时间戳4字节后的状态码
        authSuccess = (statusCode == 0x01);

        Debug.Log($"[{DeviceName}] 收到加密狗响应，状态码: 0x{statusCode:X2}, 认证结果: {(authSuccess ? "成功" : "失败")}");
        return true;
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
            // 获取设备MAC地址
            string macAddress = GetMacAddress();
            if (string.IsNullOrEmpty(macAddress))
            {
                Debug.LogError($"[{DeviceName}] 无法获取设备硬件地址");
                return false;
            }
            
            // 生成时间戳
            uint timestamp = (uint)(DateTime.Now.Ticks / TimeSpan.TicksPerSecond);
            
            // 构建解锁请求帧
            byte[] unlockFrame = CreateDongleRequestFrame(macAddress, timestamp);
            
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
    /// 创建加密狗请求帧
    /// </summary>
    private byte[] CreateDongleRequestFrame(string macAddress, uint timestamp)
    {
        List<byte> frame = new List<byte>();

        // 帧头
        frame.Add(FRAME_HEADER);
        // 功能码
        frame.Add(ENCRYPT_REQUEST_CODE);
        // 时间戳(4字节)
        frame.AddRange(BitConverter.GetBytes(timestamp));
        // 加密MAC(6字节)
        frame.AddRange(EncryptMac(macAddress, timestamp));

        // 计算校验和(除帧头帧尾外的所有字节相加取低八位)
        byte checksum = 0;
        for (int i = 1; i < frame.Count; i++)
        {
            checksum += frame[i];
        }
        frame.Add(checksum);

        // 帧尾
        frame.Add(FRAME_TAIL);

        return frame.ToArray();
    }

    /// <summary>
    /// MAC地址加密
    /// </summary>
    private byte[] EncryptMac(string macAddress, uint timestamp)
    {
        // 解析MAC地址: AA:BB:CC:DD:EE:FF → 0xAA 0xBB 0xCC 0xDD 0xEE 0xFF
        string cleanMac = macAddress.Replace("-", "").Replace(":", "");
        byte[] macBytes = new byte[6];
        for (int i = 0; i < 6; i++)
        {
            macBytes[i] = Convert.ToByte(cleanMac.Substring(i * 2, 2), 16);
        }

        // 时间戳循环密钥: 0x64A1B2C3 → 取低6字节循环: 0xC3 0xB2 0xA1 0x64 0xC3 0xB2
        byte[] timestampBytes = BitConverter.GetBytes(timestamp);
        byte[] keyBytes = new byte[6];
        for (int i = 0; i < 6; i++)
        {
            keyBytes[i] = timestampBytes[3 - (i % 4)]; // 从低位开始循环取值
        }

        // 异或加密
        byte[] encryptedMac = new byte[6];
        for (int i = 0; i < 6; i++)
        {
            encryptedMac[i] = (byte)(macBytes[i] ^ keyBytes[i]);
        }

        Debug.Log($"[{DeviceName}] 原始MAC: {macAddress}, 加密后: {BitConverter.ToString(encryptedMac)}");
        return encryptedMac;
    }

    /// <summary>
    /// 获取设备硬件地址
    /// </summary>
    private string GetMacAddress()
    {
        try
        {
            // 获取Unity设备唯一标识符
            string deviceId = SystemInfo.deviceUniqueIdentifier;
            
            if (string.IsNullOrEmpty(deviceId))
            {
                Debug.LogError($"[{DeviceName}] 无法获取设备唯一标识符");
                return "";
            }
            
            // 将设备ID转换为MAC地址格式 (取前12个字符，每2个字符一组)
            string macAddress = "";
            for (int i = 0; i < Math.Min(12, deviceId.Length); i += 2)
            {
                if (i + 1 < deviceId.Length)
                {
                    macAddress += deviceId.Substring(i, 2);
                }
                else
                {
                    macAddress += deviceId.Substring(i, 1) + "0"; // 如果字符数不够，补0
                }
            }
            
            // 如果长度不够12位，用设备ID的哈希值补充
            if (macAddress.Length < 12)
            {
                string hashSuffix = deviceId.GetHashCode().ToString("X8");
                macAddress = (macAddress + hashSuffix).Substring(0, 12);
            }
            
            Debug.Log($"[{DeviceName}] 获取到设备硬件地址: {macAddress} (来源设备ID: {deviceId})");
            return macAddress.ToUpper();
        }
        catch (Exception e)
        {
            Debug.LogError($"[{DeviceName}] 获取设备硬件地址失败: {e.Message}");
            return "";
        }
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
            
            // 计算校验和(除帧头帧尾外的所有字节相加取低八位)
            byte checksum = 0;
            for (int i = 1; i < frame.Length - 1; i++)
            {
                checksum += frame[i];
            }
            frame[frame.Length - 2] = checksum;
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
    /// 计算校验和(除帧头帧尾外的所有字节相加取低八位)
    /// </summary>
    private byte CalculateChecksum(byte[] data, int startIndex, int length)
    {
        byte checksum = 0;
        for (int i = startIndex; i < startIndex + length; i++)
        {
            checksum += data[i];
        }
        return checksum;
    }
    
    /// <summary>
    /// 验证校验和
    /// </summary>
    private bool VerifyChecksum(byte[] frame)
    {
        if (frame.Length < 3) return false;
        
        // 计算除帧头帧尾外的所有字节相加
        byte expectedChecksum = 0;
        for (int i = 1; i < frame.Length - 1; i++)
        {
            expectedChecksum += frame[i];
        }
        
        byte actualChecksum = frame[frame.Length - 2];
        
        return expectedChecksum == actualChecksum;
    }
    
    /// <summary>
    /// 断开连接时重置状态
    /// </summary>
    public override void Disconnect()
    {
        bIsEncrypted = false;
        bHandshakeCompleted = false;
        
        OnEncryptionStatusChanged?.Invoke(false);
        
        base.Disconnect();
    }
} 