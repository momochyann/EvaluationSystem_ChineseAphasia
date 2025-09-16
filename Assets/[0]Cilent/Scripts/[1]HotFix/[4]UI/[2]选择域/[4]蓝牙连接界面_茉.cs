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
        Destroy(tip);
        Destroy(gameObject);
    }

    private void 接收数据回调(string 服务UUID, string 特征值UUID, byte[] 数据)
    {
        string 数据文本 = System.Text.Encoding.UTF8.GetString(数据);
        Debug.Log($"接收到数据: {数据文本}");
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
