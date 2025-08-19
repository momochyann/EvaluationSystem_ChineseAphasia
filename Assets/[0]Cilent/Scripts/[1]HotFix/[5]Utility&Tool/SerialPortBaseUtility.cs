using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using QFramework;
using UnityEngine;
using SerialPortUtility;
using Cysharp.Threading.Tasks;

/// <summary>
/// 串口设备基础工具类接口
/// </summary>
public interface ISerialPortBaseUtility : IUtility
{
    bool IsConnected { get; }
    string CurrentPortName { get; }
    event Action<bool> OnConnectionStatusChanged;
    event Action<string> OnDeviceFound;

    UniTask<bool> SearchAndConnectAsync();
    void StartMonitoring();
    void StopMonitoring();
    void Disconnect();
}

/// <summary>
/// 串口设备基础工具类
/// 提供通用的串口搜索、连接管理功能
/// </summary>
public abstract class SerialPortBaseUtility : ISerialPortBaseUtility
{
    protected SerialPortUtilityPro serialPort;
    protected GameObject serialPortObject;
    protected bool isConnected = false;
    protected string currentPortName = "";
    protected bool isMonitoring = false;
    protected readonly object lockObject = new object();

    // Windows API声明（用于获取串口信息）
    [DllImport("SetupAPI.dll", CharSet = CharSet.Auto)]
    private static extern int spapDeviceListAvailable();

    [DllImport("SetupAPI.dll", CharSet = CharSet.Auto)]
    private static extern void spapDeviceList(int index, StringBuilder deviceInfo, int bufferSize);

    public bool IsConnected => isConnected; 
    public string CurrentPortName => currentPortName;

    public event Action<bool> OnConnectionStatusChanged;
    public event Action<string> OnDeviceFound;

    /// <summary>
    /// 设备名称（用于日志显示）
    /// </summary>
    protected abstract string DeviceName { get; }

    /// <summary>
    /// 串口波特率配置
    /// </summary>
    protected virtual int BaudRate => 115200;

    /// <summary>
    /// 连接超时时间（毫秒）
    /// </summary>
    protected virtual int ConnectionTimeoutMs => 2000;

    /// <summary>
    /// 获取系统中所有COM端口信息
    /// </summary>
    protected async UniTask<List<(string portName, int portSkip)>> GetComPortsWithSkip()
    {
        List<(string portName, int portSkip)> comPorts = new List<(string, int)>();
        
        try
        {
            int deviceNum = spapDeviceListAvailable();
            await UniTask.Yield();
            
            for (int i = 0; i < deviceNum; i++)
            {
                StringBuilder deviceInfo = new StringBuilder(1024);
                spapDeviceList(i, deviceInfo, 1024);
                string portInfo = deviceInfo.ToString();

                if (portInfo.Contains("COM"))
                {
                    string comPort = portInfo.Substring(portInfo.IndexOf("COM"));
                    int comIndex = comPort.IndexOf(',');
                    if (comIndex > 0)
                        comPort = comPort.Substring(0, comIndex);
                    
                    if (comPort.Length >= 4 && comPort.StartsWith("COM"))
                    {
                        // 正确解析COM端口号：COM3 -> Skip = 3
                        int portSkip = int.Parse(comPort[3..]);
                        comPorts.Add((comPort, portSkip));
                        Debug.Log($"[{DeviceName}] 找到串口: {comPort} (Skip: {portSkip})");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{DeviceName}] 获取COM端口失败: {ex.Message}");
            // 如果API调用失败，使用备用方法
            return GetComPortsFallback();
        }

        return comPorts;
    }

    /// <summary>
    /// 备用获取COM端口方法
    /// </summary>
    protected virtual List<(string portName, int portSkip)> GetComPortsFallback()
    {
        List<(string portName, int portSkip)> comPorts = new List<(string, int)>();
        
        // 尝试常见的COM端口
        for (int i = 1; i <= 20; i++)
        {
            comPorts.Add(($"COM{i}", i));
        }
        
        Debug.Log($"[{DeviceName}] 使用备用COM端口列表");
        return comPorts;
    }

    /// <summary>
    /// 搜索并连接设备（可重写）
    /// </summary>
    public virtual async UniTask<bool> SearchAndConnectAsync()
    {
        Debug.Log($"[{DeviceName}] 开始搜索设备...");

        // 获取系统中所有COM端口
        var comPorts = await GetComPortsWithSkip();
        
        if (comPorts.Count == 0)
        {
            Debug.LogWarning($"[{DeviceName}] 未发现可用的COM端口");
            return false;
        }

        // 逐个尝试连接COM端口
        foreach (var (portName, portSkip) in comPorts)
        {
            if (await TryConnectPort(portName, portSkip))
            {
                OnDeviceFound?.Invoke($"在端口 {portName} 发现{DeviceName}设备");
                return true;
            }
        }

        Debug.LogWarning($"[{DeviceName}] 未发现设备");
        return false;
    }

    /// <summary>
    /// 尝试连接指定COM端口
    /// </summary>
    protected virtual async UniTask<bool> TryConnectPort(string portName, int portSkip)
    {
        try
        {
            // 清理之前的连接
            CleanupSerialPort();

            // 创建专用的GameObject来管理串口
            serialPortObject = new GameObject($"{DeviceName}Serial_{portName}");
            
            // 防止在切换场景时被销毁（可选）
            UnityEngine.Object.DontDestroyOnLoad(serialPortObject);
            
            serialPort = serialPortObject.AddComponent<SerialPortUtilityPro>();

            // 配置串口参数
            ConfigureSerialPort(portSkip);

            // 绑定数据接收事件
            serialPort.ReadCompleteEventObject.AddListener(OnDataReceived);

            // 尝试打开串口
            serialPort.Open();

            // 等待一小段时间让串口完全打开
            await UniTask.Delay(100);

            if (!serialPort.IsOpened())
            {
                Debug.Log($"[{DeviceName}] 无法打开端口: {portName} (Skip: {portSkip})");
                CleanupSerialPort();
                return false;
            }

            Debug.Log($"[{DeviceName}] 成功打开端口: {portName} (Skip: {portSkip})，正在检测设备...");

            // 执行设备识别逻辑
            bool deviceRecognized = await RecognizeDevice(portName, portSkip);
            
            if (deviceRecognized)
            {
                isConnected = true;
                currentPortName = portName;
                OnConnectionStatusChanged?.Invoke(true);
                Debug.Log($"[{DeviceName}] 连接到设备，端口: {portName} (Skip: {portSkip})");
                return true;
            }

            // 如果不是目标设备，清理资源
            Debug.Log($"[{DeviceName}] 端口 {portName} (Skip: {portSkip}) 不是目标设备");
            CleanupSerialPort();
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{DeviceName}] 连接端口 {portName} (Skip: {portSkip}) 失败: {ex.Message}");
            CleanupSerialPort();
            return false;
        }
    }

    /// <summary>
    /// 配置串口参数（可重写）
    /// </summary>
    protected virtual void ConfigureSerialPort(int portSkip)
    {
        serialPort.OpenMethod = SerialPortUtilityPro.OpenSystem.NumberOrder;
        serialPort.Skip = portSkip;
        serialPort.BaudRate = BaudRate;
        serialPort.DataBit = SerialPortUtilityPro.DataBitEnum.EightBit;
        serialPort.Parity = SerialPortUtilityPro.ParityEnum.No;
        serialPort.StopBit = SerialPortUtilityPro.StopBitEnum.OneBit;
        serialPort.IsAutoOpen = false;
    }

    /// <summary>
    /// 设备识别逻辑（子类必须实现）
    /// </summary>
    protected abstract UniTask<bool> RecognizeDevice(string portName, int portSkip);

    /// <summary>
    /// 串口数据接收处理（可重写）
    /// </summary>
    protected virtual void OnDataReceived(object data)
    {
        try
        {
            var bytes = (byte[])data;
            Debug.Log($"[{DeviceName}] 收到数据: {BitConverter.ToString(bytes)}");
            
            // 子类可以重写此方法来处理具体的数据协议
            ProcessReceivedData(bytes);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{DeviceName}] 处理接收数据时出错: {ex.Message}");
        }
    }

    /// <summary>
    /// 处理接收到的数据（子类可重写）
    /// </summary>
    protected virtual void ProcessReceivedData(byte[] data)
    {
        // 默认实现为空，子类根据需要重写
    }

    /// <summary>
    /// 开始设备监控（可重写）
    /// </summary>
    public virtual void StartMonitoring()
    {
        if (isMonitoring) return;

        isMonitoring = true;
        MonitorDeviceStatus().Forget();
        Debug.Log($"[{DeviceName}] 开始监控设备状态");
    }

    /// <summary>
    /// 停止设备监控
    /// </summary>
    public virtual void StopMonitoring()
    {
        isMonitoring = false;
        Debug.Log($"[{DeviceName}] 停止监控设备状态");
    }

    /// <summary>
    /// 设备状态监控循环（可重写）
    /// </summary>
    protected virtual async UniTaskVoid MonitorDeviceStatus()
    {
        while (isMonitoring && isConnected)
        {
            try
            {
                bool deviceAlive = await CheckDeviceHealth();

                if (!deviceAlive)
                {
                    HandleDisconnection();
                    break;
                }

                await UniTask.Delay(5000); // 每5秒检查一次
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{DeviceName}] 设备监控出错: {ex.Message}");
                HandleDisconnection();
                break;
            }
        }
    }

    /// <summary>
    /// 检查设备健康状态（子类可重写）
    /// </summary>
    protected virtual async UniTask<bool> CheckDeviceHealth()
    {
        // 默认实现：检查串口是否还在打开状态
        return serialPort != null && serialPort.IsOpened();
    }

    /// <summary>
    /// 处理设备断开
    /// </summary>
    protected virtual void HandleDisconnection()
    {
        if (isConnected)
        {
            isConnected = false;
            OnConnectionStatusChanged?.Invoke(false);
            Debug.LogWarning($"[{DeviceName}] 设备断开连接");
        }
    }

    /// <summary>
    /// 发送数据到串口
    /// </summary>
    protected virtual bool SendData(byte[] data)
    {
        if (serialPort == null || !serialPort.IsOpened()) return false;

        try
        {
            serialPort.Write(data);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{DeviceName}] 发送数据失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 清理串口资源
    /// </summary>
    protected virtual void CleanupSerialPort()
    {
        if (serialPort != null)
        {
            try
            {
                if (serialPort.IsOpened())
                    serialPort.Close();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{DeviceName}] 关闭串口时出错: {ex.Message}");
            }
        }

        if (serialPortObject != null)
        {
            UnityEngine.Object.Destroy(serialPortObject);
            serialPortObject = null;
        }

        serialPort = null;
    }

    /// <summary>
    /// 断开连接
    /// </summary>
    public virtual void Disconnect()
    {
        StopMonitoring();
        CleanupSerialPort();

        isConnected = false;
        currentPortName = "";
        OnConnectionStatusChanged?.Invoke(false);
        Debug.Log($"[{DeviceName}] 已断开连接");
    }
} 