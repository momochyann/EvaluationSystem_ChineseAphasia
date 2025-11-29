using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;
using Cysharp.Threading.Tasks;
using System;
using Nova;
using QFramework;

public class 蓝牙连接界面_茉 : MonoBehaviour, IController
{

    [SerializeField] public Button 退出按钮;
    [SerializeField] public Button 扫描按钮;
    [SerializeField] public Button 断开连接按钮;
    [SerializeField] GameObject 设备显示预制体;
    [SerializeField] Transform 设备列表父级;
    [SerializeField] string 特征值UUID = "";

    string 连接的设备地址;
    string 连接的服务UUID;
    bool 正在扫描 = false;

    void Start()
    {
        退出按钮.OnClicked.AddListener(执行退出);
        扫描按钮.OnClicked.AddListener(切换扫描状态);
        断开连接按钮.OnClicked.AddListener(断开蓝牙连接);

        初始化蓝牙();
    }

    private void 初始化蓝牙()
    {
        Input.location.Start();
        BleManager.InitBluethood();
        BleManager.discoveredPeripheralAction += 显示发现的设备;
        BleManager.onConnectSuccessAction += 连接成功回调;
        BleManager.onNotificationSuccessAction += 订阅成功回调;
        BleManager.onCharacteristicNotificationAction += 接收数据回调;
    }

    private void 切换扫描状态()
    {
        if (!正在扫描)
        {
            正在扫描 = true;
            开始扫描().Forget();
            扫描按钮.GetComponentInChildren<TextBlock>().Text = "停止扫描";
        }
        else
        {
            BleManager.StopScan();
            正在扫描 = false;
            扫描按钮.GetComponentInChildren<TextBlock>().Text = "开始扫描";
        }
    }

    private async UniTaskVoid 开始扫描()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        BleManager.StartScan();
    }

    public void 显示发现的设备(string 设备地址, string 设备名称)
    {
        var 设备显示对象 = Instantiate(设备显示预制体, 设备列表父级);
        设备显示对象.GetComponentInChildren<TextBlock>().Text = 设备名称;
        设备显示对象.GetComponent<Button>().OnClicked.AddListener(() =>
        {
            BleManager.StopScan();
            Debug.Log($"连接设备{设备地址} {设备名称}");
            BleManager.ConnectBLE(设备地址);
        });
    }

    void 连接成功回调(string 设备地址, string 服务UUID)
    {
        if (连接的设备地址 != null) return;
        连接的设备地址 = 设备地址;
        连接的服务UUID = 服务UUID;
        订阅特征值();
    }

    void 订阅特征值()
    {
        if (!string.IsNullOrEmpty(特征值UUID))
        {
            BleManager.SubscribeCharacteristicWithDeviceAddress(连接的设备地址, 连接的服务UUID, 特征值UUID);
        }
    }

    private async void 订阅成功回调(string 服务UUID, string 特征值UUID)
    {
        PlayerPrefs.SetString("BLEAddress", 连接的设备地址);
        PlayerPrefs.SetString("BLEServiceUUID", 连接的服务UUID);
        PlayerPrefs.SetString("BLECharacteristicUUID", 特征值UUID);
        this.GetModel<软件配置录>()._蓝牙状态.Value = true;
        var tip = Instantiate(await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示"), this.transform);
        await tip.GetComponent<告示_茉>().等待确认显示("提示", "蓝牙连接成功");
        var 蓝牙接受讯令气预制体 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("讯授受");
        var 蓝牙接受讯令气 = Instantiate(蓝牙接受讯令气预制体);
        BleManager.EnsureMtu(连接的设备地址);
        // BleManager.SendData(
        //     连接的设备地址,
        //     连接的服务UUID,
        //     特征值UUID,
        //     new byte[]{0xAA,0x03,0x4A,0x01,0x02,0x03,0x04,0x05,0x06,0x07,0x08,0x09,0x0A,0x0B,0x0C,0x0D,0x0E,0x0F,0x10,0x11,0x12,0x13,0x14,0x15,0x16,0x17,0x18,0x19,0x1A,0x1B,0x1C,0x1D,0x1E,0x1F,0x20,0x21,0x22,0x23,0x24,0x25,0x26,0x27,0x28,0x29,0x2A,0x2B,0x2C,0x2D,0x2E,0x2F,0x30,0x31,0x32,0x33,0x34,0x35,0x36,0x37,0x38,0x39,0x3A,0x3B,0x3C,0x3D,0x3E,0x3F,0x40,0x41,0x42,0x43,0x44,0x45,0x46,0x47,0x48,0x49,0x4A,0x24,0x7E}
        // );
        Destroy(tip);
        Destroy(gameObject);
    }

    private void 接收数据回调(string 服务UUID, string 特征值UUID, byte[] 数据)
    {
        // string 数据文本 = BitConverter.ToString(数据).Replace("-", " ");
        // Debug.Log($"接收到数据: {数据文本}");
    }

    private void 断开蓝牙连接()
    {
        if (连接的设备地址 != null)
        {
            BleManager.DisConnect(连接的设备地址);
            连接的设备地址 = null;
            连接的服务UUID = null;
        }
    }

    private void 执行退出()
    {
        Destroy(gameObject);
    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
