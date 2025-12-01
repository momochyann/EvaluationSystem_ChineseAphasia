using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using Cysharp.Threading.Tasks;


/// <summary>
/// 蓝牙透传串口工具类接口
/// </summary>
public interface ISerialPortBluetoothUtility : ISerialPortBaseUtility
{
    event Action<byte[]> OnBluetoothDataReceived;
    
    bool SendBluetoothData(byte[] data);
}

/// <summary>
/// 蓝牙透传串口工具类
/// 实现握手和蓝牙透传功能
/// </summary>
public class SerialPortBluetoothUtility : SerialPortBaseUtility, ISerialPortBluetoothUtility
{
    
    // 协议常量
    private const byte FRAME_HEADER = 0xAA;
    private const byte FRAME_TAIL = 0x7E;
    private const byte BLUETOOTH_FUNC_CODE = 0x03;
    
    // 握手指令
    private static readonly byte[] HANDSHAKE_COMMAND = { 0x41, 0x54, 0x0D, 0x0A }; // "AT\r\nOK\r\n"
    private static readonly byte[] HANDSHAKE_RESPONSE = { 0x41, 0x54, 0x0D, 0x0A, 0x4F, 0x4B, 0x0D, 0x0A }; // "AT\r\n"
    
    // 状态变量
    private bool bHandshakeCompleted = false;
    
    public event Action<byte[]> OnBluetoothDataReceived;
    
    protected override string DeviceName => "蓝牙透传";
    protected override string SavedPortSkipKey => "SerialPort_Bluetooth_LastPortSkip";
    
    /// <summary>
    /// 直接验证设备（跳过握手，只要串口打开就认为成功）
    /// </summary>
    protected override async UniTask<bool> VerifyDeviceDirectly(string portName, int portSkip)
    {
        try
        {
            Debug.Log($"[{DeviceName}] 跳过握手，直接验证串口连接...");
            
            // 等待一小段时间确保串口完全打开
            await UniTask.Delay(200);
            
            // 检查串口是否打开
            if (serialPort != null && serialPort.IsOpened())
            {
                Debug.Log($"[{DeviceName}] 串口已打开，设备识别完成");
                return true;
            }
            
            Debug.LogWarning($"[{DeviceName}] 串口未打开");
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{DeviceName}] 直接验证设备异常: {ex.Message}");
            return false;
        }
    }
    
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
            // 打印原始数据
            string hexString = BitConverter.ToString(data).Replace("-", " ");
            Debug.Log($"[{DeviceName}] 数据接收(HEX): {hexString}");
            
            // 检查握手响应
            if (!bHandshakeCompleted && data.Length >= HANDSHAKE_RESPONSE.Length)
            {
                if (data.Take(HANDSHAKE_RESPONSE.Length).SequenceEqual(HANDSHAKE_RESPONSE))
                {
                    bHandshakeCompleted = true;
                    Debug.Log($"[{DeviceName}] 收到握手响应");
                    return;
                }
            }

            // 解析蓝牙帧
            if (ParseBluetoothFrame(data, out byte[] bluetoothData))
            {
                OnBluetoothDataReceived?.Invoke(bluetoothData);
                Debug.Log($"[{DeviceName}] 收到蓝牙数据: {BitConverter.ToString(bluetoothData)}");
                return;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{DeviceName}] 处理数据异常: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 解析蓝牙帧
    /// </summary>
    private bool ParseBluetoothFrame(byte[] data, out byte[] bluetoothData)
    {
        bluetoothData = null;
        // 如果最后一字节不是0x7E，则补上
        if (data.Length == 0 || data[data.Length - 1] != 0x7E)
        {
            var newData = new byte[data.Length + 1];
            Array.Copy(data, 0, newData, 0, data.Length);
            newData[newData.Length - 1] = 0x7E;
            data = newData;
        }
        if (data.Length < 6) return false; // 最小帧长度
        if (data[0] != FRAME_HEADER || data[data.Length - 1] != FRAME_TAIL) return false;
        if (data[1] != BLUETOOTH_FUNC_CODE) return false;

        byte dataLength = data[2];
        if (data.Length != dataLength + 5) return false; // 帧头+功能码+长度+数据+校验+帧尾

        // 校验和验证
        byte checksum = 0;
        for (int i = 1; i < data.Length - 2; i++)
        {
            checksum += data[i];
        }
        if (checksum != data[data.Length - 2]) return false;

        // 提取蓝牙数据
        bluetoothData = data;

        return true;
    }
    
    /// <summary>
    /// 发送蓝牙数据（通过串口发送到加密狗，加密狗会通过BLE发送）
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
            byte[] frame = CreateBluetoothFrame(data);
            bool bSuccess = SendData(frame);
            if (bSuccess)
            {
                Debug.Log($"[{DeviceName}] 发送蓝牙数据成功: {BitConverter.ToString(frame).Replace("-", " ")}");
            }
            return bSuccess;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{DeviceName}] 发送蓝牙数据异常: {ex.Message}");
            return false;
        }
    }
    
    private byte[] CreateBluetoothFrame(byte[] data)
    {
        List<byte> frame = new List<byte>();

        frame.Add(FRAME_HEADER);
        frame.Add(BLUETOOTH_FUNC_CODE);
        frame.Add((byte)data.Length);
        frame.AddRange(data);

        byte checksum = 0;
        for (int i = 1; i < frame.Count; i++)
        {
            checksum += frame[i];
        }
        frame.Add(checksum);
        frame.Add(FRAME_TAIL);

        return frame.ToArray();
    }
    
    /// <summary>
    /// 断开连接时重置状态
    /// </summary>
    public override void Disconnect()
    {
        bHandshakeCompleted = false;
        
        base.Disconnect();
    }
}

