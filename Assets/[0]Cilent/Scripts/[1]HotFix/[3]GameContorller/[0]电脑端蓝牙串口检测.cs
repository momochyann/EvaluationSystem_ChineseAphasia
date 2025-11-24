using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Cysharp.Threading.Tasks;
using System;
using SerialPortUtility;
public class 电脑端蓝牙串口检测 : MonoBehaviour, IController
{
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
            Debug.Log($"收到蓝牙数据: {BitConverter.ToString(data)}");
        };
        var serialPortUtility = this.GetComponent<SerialPortUtilityPro>();
        Debug.Log("传入串口工具"+serialPortUtility.gameObject.name);
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
                // 发送蓝牙数据
                byte[] testData = { 0x01, 0x02, 0x03 };
                centerUtility.SendBluetoothData(testData);
            }
        }
    }
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
