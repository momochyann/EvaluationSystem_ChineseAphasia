using System;
using System.Collections.Generic;
using System.Linq;
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
    protected bool isConnected = false;
    protected string currentPortName = "";
    protected bool isMonitoring = false;
    protected readonly object lockObject = new object();

    // 串口桥接接口（从Main程序集调用P/Invoke）
    private static ISerialPortBridge _serialPortBridge;

    /// <summary>
    /// 获取串口桥接实例
    /// </summary>
    protected static ISerialPortBridge GetSerialPortBridge()
    {
        if (_serialPortBridge == null)
        {
            try
            {
                // 直接创建桥接实例（HotFix程序集已引用Main程序集）
                _serialPortBridge = new SerialPortBridge();
                Debug.Log($"[SerialPortBaseUtility] 成功创建串口桥接实例");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SerialPortBaseUtility] 创建串口桥接失败: {ex.Message}");
                // 如果创建失败，返回null，后续会使用备用方法
            }
        }
        return _serialPortBridge;
    }

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
    /// （适当调小，加快每个串口握手失败时的退出速度）
    /// </summary>
    protected virtual int ConnectionTimeoutMs => 100;

    /// <summary>
    /// 获取保存端口名称的PlayerPrefs键名
    /// </summary>
    protected virtual string SavedPortNameKey => $"SerialPort_{DeviceName}_LastPortName";

    /// <summary>
    /// 获取保存端口序号的PlayerPrefs键名（保留用于兼容性）
    /// </summary>
    protected virtual string SavedPortSkipKey => $"SerialPort_{DeviceName}_LastPortSkip";

    /// <summary>
    /// 保存端口名称（而不是Skip值，因为Skip值可能因USB Hub顺序变化）
    /// </summary>
    protected virtual void SavePortSkip(int portSkip)
    {
        // 保存Skip值（用于兼容性）
        PlayerPrefs.SetInt(SavedPortSkipKey, portSkip);
        // 同时保存端口名称，因为Skip值可能因USB Hub顺序变化
        if (!string.IsNullOrEmpty(currentPortName))
        {
            PlayerPrefs.SetString(SavedPortNameKey, currentPortName);
        }
        PlayerPrefs.Save();
        Debug.Log($"[{DeviceName}] 已保存端口: {currentPortName} (Skip: {portSkip})");
    }

    /// <summary>
    /// 加载保存的端口名称，并找到对应的Skip值
    /// </summary>
    protected virtual async UniTask<(string portName, int portSkip)?> LoadSavedPortInfo()
    {
        // 先尝试加载端口名称
        if (PlayerPrefs.HasKey(SavedPortNameKey))
        {
            string savedPortName = PlayerPrefs.GetString(SavedPortNameKey);
            Debug.Log($"[{DeviceName}] 加载保存的端口名称: {savedPortName}");

            // 获取当前系统中的所有COM端口
            var comPorts = await GetComPortsWithSkip();

            // 查找保存的端口名称对应的Skip值
            var portInfo = comPorts.FirstOrDefault(port => port.portName == savedPortName);
            if (portInfo.portName == savedPortName)
            {
                Debug.Log($"[{DeviceName}] 找到保存的端口: {savedPortName} (Skip: {portInfo.portSkip})");
                return (savedPortName, portInfo.portSkip);
            }
            else
            {
                Debug.LogWarning($"[{DeviceName}] 保存的端口 {savedPortName} 不在当前系统端口列表中");
            }
        }

        // 如果没有保存的端口名称，尝试使用旧的Skip值（兼容性）
        if (PlayerPrefs.HasKey(SavedPortSkipKey))
        {
            int savedPortSkip = PlayerPrefs.GetInt(SavedPortSkipKey);
            Debug.Log($"[{DeviceName}] 加载保存的端口序号（兼容模式）: {savedPortSkip}");

            // 获取当前系统中的所有COM端口
            var comPorts = await GetComPortsWithSkip();

            // 查找对应的端口
            if (savedPortSkip >= 0 && savedPortSkip < comPorts.Count)
            {
                var portInfo = comPorts[savedPortSkip];
                Debug.Log($"[{DeviceName}] 找到端口（兼容模式）: {portInfo.portName} (Skip: {portInfo.portSkip})");
                return (portInfo.portName, portInfo.portSkip);
            }
        }

        return null;
    }

    /// <summary>
    /// 加载保存的端口序号（保留用于兼容性，但建议使用LoadSavedPortInfo）
    /// </summary>
    protected virtual int? LoadSavedPortSkip()
    {
        if (PlayerPrefs.HasKey(SavedPortSkipKey))
        {
            int savedPortSkip = PlayerPrefs.GetInt(SavedPortSkipKey);
            Debug.Log($"[{DeviceName}] 加载保存的端口序号: {savedPortSkip}");
            return savedPortSkip;
        }
        return null;
    }

    /// <summary>
    /// 清除保存的端口信息
    /// </summary>
    protected virtual void ClearSavedPortSkip()
    {
        bool cleared = false;
        if (PlayerPrefs.HasKey(SavedPortSkipKey))
        {
            PlayerPrefs.DeleteKey(SavedPortSkipKey);
            cleared = true;
        }
        if (PlayerPrefs.HasKey(SavedPortNameKey))
        {
            PlayerPrefs.DeleteKey(SavedPortNameKey);
            cleared = true;
        }
        if (cleared)
        {
            PlayerPrefs.Save();
            Debug.Log($"[{DeviceName}] 已清除保存的端口信息");
        }
    }

    /// <summary>
    /// 获取系统中所有COM端口信息
    /// 关键修复：在NumberOrder模式下，Skip值应该是spapDeviceList的索引，而不是COM端口号
    /// 因为USB Hub可能导致端口枚举顺序与COM端口号不一致
    /// </summary>
    protected async UniTask<List<(string portName, int portSkip)>> GetComPortsWithSkip()
    {
        List<(string portName, int portSkip)> comPorts = new List<(string, int)>();

        try
        {
            await UniTask.Yield();

            // 使用桥接接口获取COM端口列表（从Main程序集调用P/Invoke）
            var bridge = GetSerialPortBridge();
            if (bridge != null)
            {
                try
                {
                    // 直接调用GetComPortsWithSkip方法
                    comPorts = bridge.GetComPortsWithSkip();
                    if (comPorts.Count > 0)
                    {
                        Debug.Log($"[{DeviceName}] 通过桥接接口获取到 {comPorts.Count} 个COM端口");
                        return comPorts;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[{DeviceName}] 调用桥接接口失败: {ex.Message}");
                }
            }

            // 未获取到真实串口时不再尝试遍历所有端口，直接返回空列表
            Debug.LogWarning($"[{DeviceName}] 未获取到可用COM端口");
            return comPorts;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{DeviceName}] 获取COM端口失败: {ex.Message}");
            // 如果API调用失败，返回空列表，不再遍历所有端口
            return comPorts;
        }
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
    public void BindSreialPortUtility(SerialPortUtilityPro serialPortUtility)
    {
        serialPort = serialPortUtility;
        Debug.Log("传入串口工具" + serialPortUtility.gameObject.name);
    }
    /// <summary>
    /// 搜索并连接设备（可重写）
    /// </summary>
    public virtual async UniTask<bool> SearchAndConnectAsync()
    {
        Debug.Log($"[{DeviceName}] 开始搜索设备...");

        // 获取系统中所有COM端口
        var comPorts = await GetComPortsWithSkip();

        // 先尝试使用保存的端口信息
        var savedPortInfo = await LoadSavedPortInfo();
        if (savedPortInfo.HasValue)
        {
            string savedPortName = savedPortInfo.Value.portName;
            int savedPortSkip = savedPortInfo.Value.portSkip;
            Debug.Log($"[{DeviceName}] 优先尝试保存的端口: {savedPortName} (Skip: {savedPortSkip})");

            // 检查保存的端口是否在系统COM端口列表中
            bool savedPortExists = comPorts.Any(port => port.portName == savedPortName);

            if (!savedPortExists)
            {
                Debug.Log($"[{DeviceName}] 保存的端口 {savedPortName} 不在系统COM端口列表中，清除缓存后开始全量搜索...");
                ClearSavedPortSkip();
            }
            else
            {

                // 保存的端口跳过握手，直接打开并验证加密狗
                if (await TryConnectPort(savedPortName, savedPortSkip, skipHandshake: true))
                {
                    OnDeviceFound?.Invoke($"在端口 {savedPortName} 发现{DeviceName}设备");
                    return true;
                }
                else
                {
                    Debug.Log($"[{DeviceName}] 保存的端口连接失败，清除缓存后开始全量搜索...");
                    ClearSavedPortSkip();
                }

            }
        }

        if (comPorts.Count == 0)
        {
            Debug.LogWarning($"[{DeviceName}] 未发现可用的COM端口");
            return false;
        }

        // 逐个尝试连接COM端口（包含之前保存的端口，避免因直接验证失败而被跳过）
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
    /// <param name="portName">端口名称</param>
    /// <param name="portSkip">端口序号</param>
    /// <param name="skipHandshake">是否跳过握手，直接验证设备（用于保存的端口）</param>
    protected virtual async UniTask<bool> TryConnectPort(string portName, int portSkip, bool skipHandshake = false)
    {
        try
        {
            // 清理之前的连接
            // CleanupSerialPort();

            // 创建专用的GameObject来管理串口
            // serialPortObject = new GameObject($"{DeviceName}Serial_{portName}");

            // // 防止在切换场景时被销毁（可选）
            // UnityEngine.Object.DontDestroyOnLoad(serialPortObject);

            // serialPort = serialPortObject.AddComponent<SerialPortUtilityPro>();
            // Debug.Log("serialPortObject" + serialPortObject.name);
            // 配置串口参数
            // ConfigureSerialPort(portSkip);
            if (serialPort == null)
            {
                Debug.LogError($"[{DeviceName}] 没有对应的串口连接工具");
                return false;
            }
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

            bool deviceRecognized = false;

            if (skipHandshake)
            {
                // 跳过握手，直接验证设备（用于保存的端口）
                Debug.Log($"[{DeviceName}] 跳过握手，直接验证设备...");
                deviceRecognized = await VerifyDeviceDirectly(portName, portSkip);
            }
            else
            {
                // 执行设备识别逻辑（握手）
                deviceRecognized = await RecognizeDevice(portName, portSkip);
            }

            if (deviceRecognized)
            {
                isConnected = true;
                currentPortName = portName;
                // 保存成功连接的端口序号
                SavePortSkip(portSkip);
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
    /// 直接验证设备（跳过握手，用于保存的端口）
    /// 子类可以重写此方法来实现直接验证逻辑（如直接验证加密狗）
    /// 默认实现返回false，子类应该重写
    /// </summary>
    protected virtual async UniTask<bool> VerifyDeviceDirectly(string portName, int portSkip)
    {
        // 默认实现：如果子类没有重写，返回false
        await UniTask.Yield();
        Debug.LogWarning($"[{DeviceName}] VerifyDeviceDirectly 未实现，子类应该重写此方法");
        return false;
    }

    /// <summary>
    /// 串口数据接收处理（可重写）
    /// </summary>
    protected virtual void OnDataReceived(object data)
    {
        try
        {
            var bytes = (byte[])data;
            Debug.Log($"[{DeviceName}] [{DateTime.Now:HH:mm:ss.fff}] 收到数据: {BitConverter.ToString(bytes)}");

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


        // serialPort = null;
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
        // 注意：不断除保存的端口序号，以便下次优先尝试
    }
}
