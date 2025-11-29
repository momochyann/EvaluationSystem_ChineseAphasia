using UnityEngine;
using QFramework;
using Cysharp.Threading.Tasks;
using System;
using SerialPortUtility;

public class 电脑端蓝牙串口检测 : MonoBehaviour, IController
{
    private I蓝牙数据同步工具 _同步工具;

    // Start is called before the first frame update
    void Start()
    {
        启动连接加密狗().Forget();
    }

    async UniTask 启动连接加密狗()
    {
        await UniTask.Delay(100);
        var centerUtility = this.GetUtility<SerialPortCenterUtility>();
        
        // 注册事件
        centerUtility.OnConnectionStatusChanged += (connected) =>
        {
            Debug.Log($"连接状态: {connected}");
        };

        centerUtility.OnEncryptionStatusChanged += (encrypted) =>
        {
            Debug.Log($"加密状态: {encrypted}");
        };

        centerUtility.OnBluetoothDataReceived += (data) =>
        {
            Debug.Log($"[电脑端蓝牙串口检测] 收到蓝牙数据: {BitConverter.ToString(data)}");
            转发蓝牙数据(data).Forget();
        };

        var serialPortUtility = this.GetComponent<SerialPortUtilityPro>();
        Debug.Log("传入串口工具" + serialPortUtility.gameObject.name);
        centerUtility.BindSreialPortUtility(serialPortUtility);
        await UniTask.Delay(100);
        
        // 搜索并连接设备
        bool connected = await centerUtility.SearchAndConnectAsync();

        if (connected)
        {
            // 请求解锁
            bool unlocked = await centerUtility.RequestUnlockAsync();

            if (unlocked)
            {
                Debug.Log("[电脑端蓝牙串口检测] 加密狗解锁成功，等待接收数据");
            }
        }
    }

    private async UniTaskVoid 转发蓝牙数据(byte[] data)
    {
        if (data == null || data.Length == 0)
        {
            Debug.LogWarning("[电脑端蓝牙串口检测] 接收到空数据");
            return;
        }

        var 同步工具 = 获取同步工具();
        if (同步工具 == null)
        {
            Debug.LogError("[电脑端蓝牙串口检测] 无法获取蓝牙数据同步工具");
            return;
        }
        // data
        await 同步工具.处理接收到的蓝牙原始数据(data);
    }

    private I蓝牙数据同步工具 获取同步工具()
    {
        if (_同步工具 == null)
        {
            _同步工具 = this.GetUtility<I蓝牙数据同步工具>();
        }

        return _同步工具;
    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
