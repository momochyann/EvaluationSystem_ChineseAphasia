using UnityEngine;
using QFramework;
using Cysharp.Threading.Tasks;
using System;


/// <summary>
/// 加密狗控制器 - MonoBehaviour版本
/// </summary>
public class DongleController : MonoBehaviour, IController
{
    [Header("加密狗设置")]
    [SerializeField] private bool autoConnect = true;
    [SerializeField] private bool autoStartMonitoring = true;

    private IDongleUtility dongleUtility;

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }

    private void Start()
    {
        InitializeDongle().Forget();
    }

    private async UniTaskVoid InitializeDongle()
    {
        // 获取加密狗工具实例
        dongleUtility = this.GetUtility<IDongleUtility>();

        // 订阅事件
        dongleUtility.OnConnectionStatusChanged += OnConnectionStatusChanged;
        dongleUtility.OnDeviceFound += OnDeviceFound;

        if (autoConnect)
        {
            await ConnectDongle();
        }
    }

    /// <summary>
    /// 连接加密狗
    /// </summary>
    public async UniTask<bool> ConnectDongle()
    {
        Debug.Log("开始连接加密狗...");

        // 搜索并连接设备
        var connected = await dongleUtility.SearchAndConnectAsync();

        if (connected)
        {
            // 执行认证
            var authenticated = await dongleUtility.AuthenticateAsync();

            if (authenticated && autoStartMonitoring)
            {
                dongleUtility.StartMonitoring();
            }

            return authenticated;
        }

        return false;
    }

    /// <summary>
    /// 断开加密狗
    /// </summary>
    public void DisconnectDongle()
    {
        dongleUtility?.Disconnect();
    }

    /// <summary>
    /// 加密数据
    /// </summary>
    public async UniTask<byte[]> EncryptData(byte[] data)
    {
        if (!dongleUtility.IsConnected)
        {
            Debug.LogWarning("加密狗未连接，无法加密数据");
            return null;
        }

        return await dongleUtility.EncryptDataAsync(data);
    }

    /// <summary>
    /// 解密数据
    /// </summary>
    public async UniTask<byte[]> DecryptData(byte[] data)
    {
        if (!dongleUtility.IsConnected)
        {
            Debug.LogWarning("加密狗未连接，无法解密数据");
            return null;
        }

        return await dongleUtility.DecryptDataAsync(data);
    }

    /// <summary>
    /// 连接状态变化事件处理
    /// </summary>
    private void OnConnectionStatusChanged(bool isConnected)
    {
        Debug.Log($"加密狗连接状态: {(isConnected ? "已连接" : "已断开")}");

        // 这里可以触发UI更新或其他业务逻辑
        if (isConnected)
        {
            // 连接成功的处理
            NotifyConnectionSuccess();
        }
        else
        {
            // 断开连接的处理
            NotifyConnectionLost();
        }
    }

    /// <summary>
    /// 设备发现事件处理
    /// </summary>
    private void OnDeviceFound(string message)
    {
        Debug.Log($"设备发现: {message}");
    }

    /// <summary>
    /// 通知连接成功
    /// </summary>
    private void NotifyConnectionSuccess()
    {
        // 可以在这里触发UI事件或其他系统通知
        TypeEventSystem.Global.Send(new DongleConnectedEvent
        {
            DeviceId = dongleUtility.DeviceId
        });
    }

    /// <summary>
    /// 通知连接丢失
    /// </summary>
    private void NotifyConnectionLost()
    {
        // 可以在这里触发UI事件或其他系统通知
        TypeEventSystem.Global.Send(new DongleDisconnectedEvent());
    }

    private void OnDestroy()
    {
        if (dongleUtility != null)
        {
            dongleUtility.OnConnectionStatusChanged -= OnConnectionStatusChanged;
            dongleUtility.OnDeviceFound -= OnDeviceFound;
            dongleUtility.Disconnect();
        }
    }

    #region 编辑器测试方法
#if UNITY_EDITOR
    [ContextMenu("测试连接加密狗")]
    private void TestConnect()
    {
        ConnectDongle().Forget();
    }

    [ContextMenu("测试断开加密狗")]
    private void TestDisconnect()
    {
        DisconnectDongle();
    }

    [ContextMenu("测试加密数据")]
    private void TestEncrypt()
    {
        TestEncryptData().Forget();
    }

    private async UniTaskVoid TestEncryptData()
    {
        var testData = System.Text.Encoding.UTF8.GetBytes("Hello Dongle!");
        var encryptedData = await EncryptData(testData);

        if (encryptedData != null)
        {
            Debug.Log($"加密成功，长度: {encryptedData.Length}");

            // 测试解密
            var decryptedData = await DecryptData(encryptedData);
            if (decryptedData != null)
            {
                var decryptedText = System.Text.Encoding.UTF8.GetString(decryptedData);
                Debug.Log($"解密结果: {decryptedText}");
            }
        }
    }
#endif
    #endregion
}

/// <summary>
/// 加密狗连接成功事件
/// </summary>
public struct DongleConnectedEvent
{
    public string DeviceId;
}

/// <summary>
/// 加密狗断开连接事件
/// </summary>
public struct DongleDisconnectedEvent
{
}

/// <summary>
/// 业务逻辑使用示例
/// </summary>
public class DongleUsageExample : MonoBehaviour, IController
{
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }

    private void Start()
    {
        // 监听加密狗事件
        TypeEventSystem.Global.Register<DongleConnectedEvent>(OnDongleConnected);
        TypeEventSystem.Global.Register<DongleDisconnectedEvent>(OnDongleDisconnected);
    }

    /// <summary>
    /// 加密狗连接成功处理
    /// </summary>
    private void OnDongleConnected(DongleConnectedEvent e)
    {
        Debug.Log($"业务逻辑收到加密狗连接事件，设备ID: {e.DeviceId}");

        // 可以在这里执行需要加密狗的业务逻辑
        StartSecureOperations().Forget();
    }

    /// <summary>
    /// 加密狗断开连接处理
    /// </summary>
    private void OnDongleDisconnected(DongleDisconnectedEvent e)
    {
        Debug.Log("业务逻辑收到加密狗断开事件");

        // 停止需要加密狗的操作
        StopSecureOperations();
    }

    /// <summary>
    /// 开始安全操作
    /// </summary>
    private async UniTaskVoid StartSecureOperations()
    {
        var dongleUtility = this.GetUtility<IDongleUtility>();

        // 示例：加密用户数据
        var userData = "重要的用户评测数据";
        var userDataBytes = System.Text.Encoding.UTF8.GetBytes(userData);

        var encryptedData = await dongleUtility.EncryptDataAsync(userDataBytes);
        if (encryptedData != null)
        {
            Debug.Log("用户数据加密成功，可以安全存储");
            // 这里可以保存加密后的数据
        }
    }

    /// <summary>
    /// 停止安全操作
    /// </summary>
    private void StopSecureOperations()
    {
        Debug.Log("停止需要加密狗的安全操作");
        // 清理相关资源
    }

    private void OnDestroy()
    {
        TypeEventSystem.Global.UnRegister<DongleConnectedEvent>(OnDongleConnected);
        TypeEventSystem.Global.UnRegister<DongleDisconnectedEvent>(OnDongleDisconnected);
    }
}
