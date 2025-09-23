using System.Collections;
using UnityEngine;
using SerialPortUtility;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading;
using System;

public class SerialPortUtilityManager : MonoBehaviour
{
    [DllImport("spap", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int spapDeviceListAvailable();

    [DllImport("spap", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int spapDeviceList(int deviceNum, [MarshalAs(UnmanagedType.LPStr)] StringBuilder deviceInfo, int buffer_size);

    [SerializeField] int ComPortName;
    public UnityAction<byte[]> OnSerialDataReceived;
    public UnityAction<byte[]> OnBluetoothDataReceived; // 蓝牙数据接收事件
    public UnityAction<bool> OnDongleAuthResult; // 加密狗认证结果事件

    SerialPortUtilityPro serialPortUtilityPro;
    [HideInInspector] public bool serialPortSetPanelIsActive = false;
    bool isHandshake = false;
    bool isDongleAuthenticated = false;
    int currentComPort;

    // 基础配置
    public int BaudRate = 115200;
    public int HANDSHAKE_TIMEOUT = 2;
    public int PORT_OPEN_TIMEOUT = 1;

    // 协议定义
    public byte[] HANDSHAKE_DATA = new byte[] { 0x57, 0x73, 0x3A, 0x7E }; // "ws:~"
    private const byte FRAME_HEAD = 0xAA;
    private const byte FRAME_TAIL = 0x7E;
    private const byte FUNC_DONGLE_REQUEST = 0x01;
    private const byte FUNC_DONGLE_RESPONSE = 0x02;
    private const byte FUNC_BLUETOOTH = 0x03;

    // 加密狗状态码
    private const byte STATUS_AUTH_FAILED = 0x00;
    private const byte STATUS_AUTH_SUCCESS = 0x01;

    void Start()
    {
        serialPortUtilityPro = GetComponent<SerialPortUtilityPro>();
        ComPortInit().Forget();
        OnBluetoothDataReceived += (data) =>
        {
            Debug.Log($"收到蓝牙数据: {BitConverter.ToString(data)}");
            // 处理蓝牙数据
        };
    }

    async UniTaskVoid ComPortInit()
    {
        serialPortUtilityPro.OpenMethod = SerialPortUtilityPro.OpenSystem.NumberOrder;
        currentComPort = PlayerPrefs.GetInt("ComPortName", 0);
        if (currentComPort != 0 && await TryConnectPort($"COM{currentComPort}"))
        {
            Debug.Log("串口初始化成功");
            // 初始化完成后，尝试加密狗认证
            await AuthenticateDongle();
            await UniTask.Delay(5000);
            SendBluetoothData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 });

        }
        else
        {
            PlayerPrefs.SetInt("ComPortName", 0);
            Debug.Log(currentComPort == 0 ? "无保存串口" : "串口初始化失败");
            await ScanComPort();
        }
    }

    async UniTask ScanComPort()
    {
        try
        {
            var comPorts = await GetComPorts();
            if (comPorts.Count == 0)
            {
                Debug.Log("未找到任何串口");
                return;
            }

            foreach (string comPort in comPorts)
            {
                Debug.Log($"正在测试串口: {comPort}");
                if (await TryConnectPort(comPort))
                {
                    // 连接成功后尝试加密狗认证
                    await AuthenticateDongle();
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"扫描串口时发生错误: {e}");
        }
    }

    async UniTask<List<string>> GetComPorts()
    {
        List<string> comPorts = new List<string>();
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
                if (comPort.Length < 6)
                {
                    comPorts.Add(comPort);
                    Debug.Log($"找到串口: {comPort}");
                }
            }
        }
        return comPorts;
    }

    async UniTask<bool> TryConnectPort(string comPort)
    {
        try
        {
            serialPortUtilityPro.ReadCompleteEventObject.AddListener(HandshakeDataReceived);
            Debug.Log($"尝试打开串口: {comPort}");
            if (!await TryOpenPort(comPort))
            {
                return false;
            }

            if (await TryHandshake())
            {
                PlayerPrefs.SetInt("ComPortName", int.Parse(comPort[3..]));
                OnConnectSuccess(comPort);
                return true;
            }
            Debug.Log($"串口 {comPort} 达到最大重试次数");
            return false;
        }
        finally
        {
            serialPortUtilityPro.ReadCompleteEventObject.RemoveListener(HandshakeDataReceived);
        }
    }

    async UniTask<bool> TryOpenPort(string comPort)
    {
        try
        {
            serialPortUtilityPro.Skip = int.Parse(comPort[3..]);
            serialPortUtilityPro.BaudRate = BaudRate;
            serialPortUtilityPro.Open();

            var cts = new CancellationTokenSource();
            cts.CancelAfterSlim(TimeSpan.FromSeconds(PORT_OPEN_TIMEOUT));
            try
            {
                await UniTask.WaitUntil(
                    () => serialPortUtilityPro.IsOpened(),
                    cancellationToken: cts.Token
                );
                return true;
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"串口 {comPort} 打开超时");
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"串口 {comPort} 打开失败: {e.Message}");
            return false;
        }
    }

    async UniTask<bool> TryHandshake()
    {
        isHandshake = false;
        serialPortUtilityPro.Write(HANDSHAKE_DATA);

        var _cts = new CancellationTokenSource();
        _cts.CancelAfterSlim(TimeSpan.FromSeconds(HANDSHAKE_TIMEOUT));
        var _linkCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, this.GetCancellationTokenOnDestroy());
        try
        {
            await UniTask.WaitUntil(
                () => isHandshake,
                cancellationToken: _linkCancellationToken.Token
            );
            return true;
        }
        catch (OperationCanceledException)
        {
            Debug.Log("握手超时");
            return false;
        }
    }

    void HandshakeDataReceived(object data)
    {
        byte[] bytes = (byte[])data;
        Debug.Log("握手数据接收" + bytes[0] + "原始数据" + HANDSHAKE_DATA[0]);
        if (bytes.Length >= 3 &&
            bytes[0] == HANDSHAKE_DATA[0] &&
            bytes[1] == HANDSHAKE_DATA[1] &&
            bytes[2] == HANDSHAKE_DATA[2])
        {
            isHandshake = true;
        }
    }

    // ==================== 加密狗功能 ====================

    /// <summary>
    /// 加密狗认证
    /// </summary>
    public async UniTask<bool> AuthenticateDongle()
    {
        try
        {
            string macAddress = GetMacAddress();
            if (string.IsNullOrEmpty(macAddress))
            {
                Debug.LogError("无法获取设备硬件地址");
                return false;
            }

            uint timestamp = (uint)(DateTime.Now.Ticks / TimeSpan.TicksPerSecond);
            byte[] requestFrame = CreateDongleRequestFrame(macAddress, timestamp);

            // 重置认证状态
            isDongleAuthenticated = false;

            // 发送认证请求
            serialPortUtilityPro.Write(requestFrame);
            Debug.Log($"发送加密狗认证请求: {BitConverter.ToString(requestFrame)}");

            // 等待响应
            var cts = new CancellationTokenSource();
            cts.CancelAfterSlim(TimeSpan.FromSeconds(3));

            try
            {
                await UniTask.WaitUntil(() => isDongleAuthenticated, cancellationToken: cts.Token);
                OnDongleAuthResult?.Invoke(true);
                Debug.Log("加密狗认证成功");
                return true;
            }
            catch (OperationCanceledException)
            {
                OnDongleAuthResult?.Invoke(false);
                Debug.LogWarning("加密狗认证超时");
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"加密狗认证失败: {e.Message}");
            OnDongleAuthResult?.Invoke(false);
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
        frame.Add(FRAME_HEAD);
        // 功能码
        frame.Add(FUNC_DONGLE_REQUEST);
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

        Debug.Log($"原始MAC: {macAddress}, 加密后: {BitConverter.ToString(encryptedMac)}");
        return encryptedMac;
    }

    /// <summary>
    /// 解析加密狗响应
    /// </summary>
    private bool ParseDongleResponse(byte[] data, out bool authSuccess)
    {
        authSuccess = false;

        if (data.Length < 8) return false;
        if (data[0] != FRAME_HEAD) return false;
        if (data[1] != FUNC_DONGLE_RESPONSE) return false;
        Debug.Log("收到加密狗响应" + data[0] + "原始数据" + FRAME_HEAD);
        // 校验和验证
        byte checksum = 0;
        for (int i = 1; i < data.Length - 1; i++)
        {
            checksum += data[i];
        }
        Debug.Log("校验和" + checksum + "数据" + data[data.Length - 1]);
        if (checksum != data[data.Length - 1]) return false;

        // 解析状态码
        byte statusCode = data[6]; // 时间戳4字节后的状态码
        authSuccess = (statusCode == STATUS_AUTH_SUCCESS);

        Debug.Log($"收到加密狗响应，状态码: 0x{statusCode:X2}, 认证结果: {(authSuccess ? "成功" : "失败")}");
        return true;
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
                Debug.LogError("无法获取设备唯一标识符");
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
            
            Debug.Log($"获取到设备硬件地址: {macAddress} (来源设备ID: {deviceId})");
            return macAddress.ToUpper();
        }
        catch (Exception e)
        {
            Debug.LogError($"获取设备硬件地址失败: {e.Message}");
            return "";
        }
    }

    // ==================== 蓝牙透传功能 ====================

    /// <summary>
    /// 发送蓝牙数据
    /// </summary>
    public void SendBluetoothData(byte[] data)
    {
        if (data == null || data.Length == 0) return;

        byte[] bluetoothFrame = CreateBluetoothFrame(data);
        serialPortUtilityPro.Write(bluetoothFrame);
        Debug.Log($"发送蓝牙数据: {BitConverter.ToString(bluetoothFrame)}");
    }

    /// <summary>
    /// 创建蓝牙帧
    /// </summary>
    private byte[] CreateBluetoothFrame(byte[] data)
    {
        List<byte> frame = new List<byte>();

        // 帧头
        frame.Add(FRAME_HEAD);
        // 功能码
        frame.Add(FUNC_BLUETOOTH);
        // 长度
        frame.Add((byte)data.Length);
        // 数据区
        frame.AddRange(data);

        // 计算校验和
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
    /// 解析蓝牙帧
    /// </summary>
    private bool ParseBluetoothFrame(byte[] data, out byte[] bluetoothData)
    {
        bluetoothData = null;

        if (data.Length < 6) return false; // 最小帧长度
        if (data[0] != FRAME_HEAD || data[data.Length - 1] != FRAME_TAIL) return false;
        if (data[1] != FUNC_BLUETOOTH) return false;

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
        bluetoothData = new byte[dataLength];
        Array.Copy(data, 3, bluetoothData, 0, dataLength);

        return true;
    }

    // ==================== 数据处理 ====================

    void OnDataReceived(object data)
    {
        byte[] bytes = (byte[])data;
        string hexString = BitConverter.ToString(bytes).Replace("-", " ");
        Debug.Log("数据接收(HEX): " + hexString);

        // 尝试解析加密狗响应
        if (ParseDongleResponse(bytes, out bool authResult))
        {
            if (authResult)
            {
                isDongleAuthenticated = true;
                Debug.Log("收到加密狗认证成功响应");
            }
            else
            {
                Debug.LogWarning("收到加密狗认证失败响应");
            }
            return;
        }

        // 尝试解析蓝牙帧
        if (ParseBluetoothFrame(bytes, out byte[] bluetoothData))
        {
            Debug.Log($"收到蓝牙透传数据: {BitConverter.ToString(bluetoothData)}");
            OnBluetoothDataReceived?.Invoke(bluetoothData);
            return;
        }

        // 普通串口数据
        OnSerialDataReceived?.Invoke(bytes);
    }

    void OnConnectSuccess(string comPort)
    {
        serialPortUtilityPro.ReadCompleteEventObject.AddListener(OnDataReceived);
        Debug.Log($"串口 {comPort} 初始化成功");
    }

    // ==================== 配置方法 ====================

    public void SetBaudRate(int baudRate)
    {
        if (serialPortSetPanelIsActive == false) return;
        serialPortUtilityPro.Close();
        serialPortUtilityPro.BaudRate = baudRate;
        PlayerPrefs.SetInt("BaudRate", baudRate);
        serialPortUtilityPro.Open();
    }

    public void SetComPortName(int comPortName)
    {
        if (serialPortSetPanelIsActive == false) return;
        serialPortUtilityPro.Close();
        serialPortUtilityPro.Skip = comPortName;
        PlayerPrefs.SetInt("ComPortName", comPortName);
        serialPortUtilityPro.Open();
    }

    public void RegisterSerialPortSetPanel(bool isActive)
    {
        serialPortSetPanelIsActive = isActive;
    }
}
