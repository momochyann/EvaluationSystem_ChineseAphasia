using UnityEngine;
using SerialPortUtility;
using Cysharp.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.Generic;

/// <summary>
/// 串口基础工具类 MonoBehaviour 版本
/// 自动从配置文件读取配置并应用
/// </summary>
[RequireComponent(typeof(SerialPortUtilityPro))]
public abstract class SerialPortBaseUtilityMono : MonoBehaviour, ISerialPortBaseUtility
{
    protected SerialPortUtilityPro serialPort;
    protected bool isConnected = false;
    protected string currentPortName = "";
    protected bool isMonitoring = false;
    protected readonly object lockObject = new object();

    // 配置参数（从配置文件读取）
    protected int baudRate = 9600;
    protected int connectionTimeoutMs = 2000;
    protected SerialPortUtilityPro.DataBitEnum dataBit = SerialPortUtilityPro.DataBitEnum.EightBit;
    protected SerialPortUtilityPro.StopBitEnum stopBit = SerialPortUtilityPro.StopBitEnum.OneBit;
    protected SerialPortUtilityPro.ParityEnum parity = SerialPortUtilityPro.ParityEnum.No;
    protected int skip = 0;
    protected bool isAutoOpen = false;
    protected SerialPortUtilityPro.OpenSystem openMethod = SerialPortUtilityPro.OpenSystem.NumberOrder;

    public bool IsConnected => isConnected;
    public string CurrentPortName => currentPortName;

    public event System.Action<bool> OnConnectionStatusChanged;
    public event System.Action<string> OnDeviceFound;

    /// <summary>
    /// 设备名称（用于日志显示）
    /// </summary>
    protected abstract string DeviceName { get; }

    protected virtual void Awake()
    {
        // 获取 SerialPortUtilityPro 组件
        serialPort = GetComponent<SerialPortUtilityPro>();
        if (serialPort == null)
        {
            Debug.LogError($"[{DeviceName}] 未找到 SerialPortUtilityPro 组件");
            return;
        }

        // 自动加载并应用配置
        LoadAndApplyConfigAsync().Forget();
    }

    /// <summary>
    /// 从配置文件加载并应用配置
    /// </summary>
    private async UniTaskVoid LoadAndApplyConfigAsync()
    {
        // 等待配置管理器加载完成
        if (!SerialPortConfigManager.IsLoaded)
        {
            await SerialPortConfigManager.LoadConfigListAsync();
        }

        // 根据 GameObject 名称查找匹配的配置
        string prefabName = gameObject.name;
        SerialPortConfigSO config = SerialPortConfigManager.GetConfigByPrefabName(prefabName);

        if (config != null)
        {
            // 应用配置参数
            ApplyConfig(config);
            Debug.Log($"[{DeviceName}] 已从配置文件加载配置: {prefabName}");
        }
        else
        {
            Debug.LogWarning($"[{DeviceName}] 未找到匹配的配置，使用默认值: {prefabName}");
        }
    }

    /// <summary>
    /// 应用配置参数
    /// </summary>
    protected virtual void ApplyConfig(SerialPortConfigSO config)
    {
        if (config == null || serialPort == null) return;

        // 保存配置参数
        baudRate = config.baudRate;
        connectionTimeoutMs = config.connectionTimeoutMs;
        dataBit = config.dataBit;
        stopBit = config.stopBit;
        parity = config.parity;
        skip = config.skip;
        isAutoOpen = config.isAutoOpen;
        openMethod = config.openMethod;

        // 应用到 SerialPortUtilityPro
        serialPort.BaudRate = baudRate;
        serialPort.DataBit = dataBit;
        serialPort.StopBit = stopBit;
        serialPort.Parity = parity;
        serialPort.Skip = skip;
        serialPort.IsAutoOpen = isAutoOpen;
        serialPort.OpenMethod = openMethod;
    }

    /// <summary>
    /// 配置串口参数（可重写）
    /// </summary>
    protected virtual void ConfigureSerialPort(int portSkip)
    {
        if (serialPort == null) return;

        serialPort.OpenMethod = openMethod;
        serialPort.Skip = portSkip;
        serialPort.BaudRate = baudRate;
        serialPort.DataBit = dataBit;
        serialPort.Parity = parity;
        serialPort.StopBit = stopBit;
        serialPort.IsAutoOpen = isAutoOpen;
    }

    // 实现 ISerialPortBaseUtility 接口的方法
    // 这些方法需要调用 SerialPortBaseUtility 的实现
    // 由于 SerialPortBaseUtility 不是 MonoBehaviour，我们需要重新实现这些方法

    public virtual async UniTask<bool> SearchAndConnectAsync()
    {
        Debug.Log($"[{DeviceName}] 开始搜索设备...");

        var comPorts = await GetComPortsWithSkip();

        if (comPorts.Count == 0)
        {
            Debug.LogWarning($"[{DeviceName}] 未发现可用的COM端口");
            return false;
        }

        foreach (var (portName, portSkip) in comPorts)
        {
            if (portName.Contains("COM12") || portName.Contains("COM27") || portName.Contains("COM11") || portName.Contains("COM17"))
            {
                continue;
            }
            if (await TryConnectPort(portName, portSkip))
            {
                OnDeviceFound?.Invoke($"在端口 {portName} 发现{DeviceName}设备");
                return true;
            }
        }

        Debug.LogWarning($"[{DeviceName}] 未发现设备");
        return false;
    }

    protected virtual async UniTask<List<(string portName, int portSkip)>> GetComPortsWithSkip()
    {
        // 使用 SerialPortBaseUtility 的静态方法
        return await SerialPortBaseUtilityHelper.GetComPortsWithSkip(DeviceName);
    }

    protected virtual async UniTask<bool> TryConnectPort(string portName, int portSkip)
    {
        try
        {
            if (serialPort == null)
            {
                Debug.LogError($"[{DeviceName}] 没有对应的串口连接工具");
                return false;
            }

            ConfigureSerialPort(portSkip);
            serialPort.ReadCompleteEventObject.AddListener(OnDataReceived);
            serialPort.Open();

            await UniTask.Delay(100);

            if (!serialPort.IsOpened())
            {
                Debug.Log($"[{DeviceName}] 无法打开端口: {portName} (Skip: {portSkip})");
                CleanupSerialPort();
                return false;
            }

            Debug.Log($"[{DeviceName}] 成功打开端口: {portName} (Skip: {portSkip})，正在检测设备...");

            bool deviceRecognized = await RecognizeDevice(portName, portSkip);

            if (deviceRecognized)
            {
                isConnected = true;
                currentPortName = portName;
                OnConnectionStatusChanged?.Invoke(true);
                Debug.Log($"[{DeviceName}] 连接到设备，端口: {portName} (Skip: {portSkip})");
                return true;
            }

            Debug.Log($"[{DeviceName}] 端口 {portName} (Skip: {portSkip}) 不是目标设备");
            CleanupSerialPort();
            return false;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[{DeviceName}] 连接端口 {portName} (Skip: {portSkip}) 失败: {ex.Message}");
            CleanupSerialPort();
            return false;
        }
    }

    /// <summary>
    /// 设备识别逻辑（子类必须实现）
    /// </summary>
    protected abstract UniTask<bool> RecognizeDevice(string portName, int portSkip);

    protected virtual void OnDataReceived(object data)
    {
        try
        {
            var bytes = (byte[])data;
            Debug.Log($"[{DeviceName}] 收到数据: {System.BitConverter.ToString(bytes)}");
            ProcessReceivedData(bytes);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[{DeviceName}] 处理接收数据时出错: {ex.Message}");
        }
    }

    protected virtual void ProcessReceivedData(byte[] data)
    {
        // 默认实现为空，子类根据需要重写
    }

    public virtual void StartMonitoring()
    {
        if (isMonitoring) return;
        isMonitoring = true;
        MonitorDeviceStatus().Forget();
        Debug.Log($"[{DeviceName}] 开始监控设备状态");
    }

    public virtual void StopMonitoring()
    {
        isMonitoring = false;
        Debug.Log($"[{DeviceName}] 停止监控设备状态");
    }

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
                await UniTask.Delay(5000);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{DeviceName}] 设备监控出错: {ex.Message}");
                HandleDisconnection();
                break;
            }
        }
    }

    protected virtual async UniTask<bool> CheckDeviceHealth()
    {
        return serialPort != null && serialPort.IsOpened();
    }

    protected virtual void HandleDisconnection()
    {
        if (isConnected)
        {
            isConnected = false;
            OnConnectionStatusChanged?.Invoke(false);
            Debug.LogWarning($"[{DeviceName}] 设备断开连接");
        }
    }

    protected virtual bool SendData(byte[] data)
    {
        if (serialPort == null || !serialPort.IsOpened()) return false;

        try
        {
            serialPort.Write(data);
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[{DeviceName}] 发送数据失败: {ex.Message}");
            return false;
        }
    }

    protected virtual void CleanupSerialPort()
    {
        if (serialPort != null)
        {
            try
            {
                if (serialPort.IsOpened())
                    serialPort.Close();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{DeviceName}] 关闭串口时出错: {ex.Message}");
            }
        }
    }

    public virtual void Disconnect()
    {
        StopMonitoring();
        CleanupSerialPort();
        isConnected = false;
        currentPortName = "";
        OnConnectionStatusChanged?.Invoke(false);
        Debug.Log($"[{DeviceName}] 已断开连接");
    }

    protected virtual void OnDestroy()
    {
        Disconnect();
    }
}

/// <summary>
/// SerialPortBaseUtility 的辅助类，提供静态方法
/// </summary>
public static class SerialPortBaseUtilityHelper
{
    [DllImport("spap", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int spapDeviceListAvailable();

    [DllImport("spap", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int spapDeviceList(int deviceNum, [MarshalAs(UnmanagedType.LPStr)] System.Text.StringBuilder deviceInfo, int buffer_size);

    public static async UniTask<List<(string portName, int portSkip)>> GetComPortsWithSkip(string deviceName)
    {
        List<(string portName, int portSkip)> comPorts = new List<(string, int)>();

        try
        {
            int deviceNum = spapDeviceListAvailable();
            await UniTask.Yield();

            for (int i = 0; i < deviceNum; i++)
            {
                System.Text.StringBuilder deviceInfo = new System.Text.StringBuilder(1024);
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
                        int portSkip = int.Parse(comPort[3..]);
                        comPorts.Add((comPort, portSkip));
                        Debug.Log($"[{deviceName}] 找到串口: {comPort} (Skip: {portSkip})");
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[{deviceName}] 获取COM端口失败: {ex.Message}");
            return GetComPortsFallback(deviceName);
        }

        return comPorts;
    }

    private static List<(string portName, int portSkip)> GetComPortsFallback(string deviceName)
    {
        List<(string portName, int portSkip)> comPorts = new List<(string, int)>();

        for (int i = 1; i <= 20; i++)
        {
            comPorts.Add(($"COM{i}", i));
        }

        Debug.Log($"[{deviceName}] 使用备用COM端口列表");
        return comPorts;
    }
}

