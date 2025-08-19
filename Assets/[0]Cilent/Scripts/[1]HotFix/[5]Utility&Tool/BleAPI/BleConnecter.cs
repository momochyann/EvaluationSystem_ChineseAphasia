using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using NovaSamples.UIControls;
using QFramework;
using Nova;
using DG.Tweening;
public class BleConnecter : MonoBehaviour
{
    // Start is called before the first frame update
    string bleAddress;
    string bleServiceUUID;
    string bleCharacteristicUUID;
    string bleName;
    bool isConnected;
    Button connectBtn;
    private void Awake()
    {

    }
    void Start()
    {
        长生();
    }
    void 长生()
    {
        connectBtn = this.GetComponent<Button>();
        this.GetComponent<UIBlock2D>().Color = Color.gray;
        this.GetComponentInChildren<TextBlock>().Text = "蓝牙未连接";
        // UIBlock2D = this.GetComponent<UIBlock2D>();
        connectBtn.OnClicked.AddListener(() =>
        {
            ConnectBLE().Forget();
        });
        bleAddress = PlayerPrefs.GetString("BLEAddress");
        bleServiceUUID = PlayerPrefs.GetString("BLEServiceUUID");
        bleCharacteristicUUID = PlayerPrefs.GetString("BLECharacteristicUUID");
        if (string.IsNullOrEmpty(bleAddress))
        {
            Debug.Log("设备地址为空 自动连接失败");
            return;
        }
        Input.location.Start();     //启用手机位置定位服务
        BleManager.InitBluethood(); //初始化蓝牙
        BleManager.onConnectSuccessAction += OnConnectSuccess;
        BleManager.onNotificationSuccessAction += OnNotificationSuccess;
        BleManager.discoveredPeripheralAction += OnDiscoveredPeripheral;
        BleManager.discoveredPeripheralAction += OnDiscoveredPeripheral;
        ConnectBLE().Forget();
    }
    async UniTaskVoid SendDataLoop()
    {
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: this.GetCancellationTokenOnDestroy());
            BleManager.SendData(bleAddress, bleServiceUUID, bleCharacteristicUUID, new sbyte[2] { 0x01, 0x02 });
        }
    }

    async UniTaskVoid ConnectBLE()
    {
        Debug.Log("开始连接蓝牙");
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        BleManager.StartScan();
    }
    void OnDiscoveredPeripheral(string address, string name)
    {

        if (address != bleAddress)
        {
            // Debug.Log($"设备ID不匹配{address}:{bleAddress}");
            return;
        }
        bleName = name;
        BleManager.StopScan();
        BleManager.ConnectBLE(address);
    }
    void OnConnectSuccess(string address, string serviceUUID)
    {
        Debug.Log($"开始订阅{address}");
        if (isConnected) return;
        isConnected = true;
        if (serviceUUID != bleServiceUUID)
        {
            Debug.Log($"服务ID不匹配{serviceUUID}:{bleServiceUUID}");
            return;
        }
        BleManager.SubscribeCharacteristicWithDeviceAddress(bleAddress, bleServiceUUID, bleCharacteristicUUID);
    }
    void OnNotificationSuccess(string serviceUUID, string characteristicUUID)
    {
        // SendDataLoop().Forget();
        BleManager.isConnected = true;
        this.GetComponent<UIBlock2D>().Color = Color.blue;
        Debug.Log($"订阅Noity成功{serviceUUID}{characteristicUUID}");
    }
}
