using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Unity.VisualScripting;
public class BleScaner : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject deviceDisPrefab;
    string ConnectBLEServiceUUID;
    string ConnectdBLEAddress;
    [Header("蓝牙Notify特征值UUID")]
    [SerializeField] string characteristicNotifyUUID;
    Transform deviceDisBack;
    Button disConnectBtn;
    //开始扫描按钮
    Button startScanningBtn;
    bool isScanning = false;

    void Start()
    {
        deviceDisBack = transform.Find("DeviceDisBack");
        startScanningBtn = transform.Find("StartScanningBtn").GetComponent<Button>();
        disConnectBtn = transform.Find("DisConnectBtn").GetComponent<Button>();

        Input.location.Start();     //启用手机位置定位服务
        BleManager.InitBluethood(); //初始化蓝牙
        BleManager.discoveredPeripheralAction += FindingBLE2Dis; //发现并显示可连接的蓝牙设备
        BleManager.onCharacteristicNotificationAction += OnCharacteristicNotification;
        BleManager.onConnectSuccessAction += OnConnectSuccess; //连接成功 等待订阅特征值
        BleManager.onNotificationSuccessAction += OnSubscribeSuccess; //订阅成功
        startScanningBtn.onClick.AddListener(() =>
        {
            StartScanButtonListener();
        });
        disConnectBtn.onClick.AddListener(() =>
        {
            if (ConnectdBLEAddress != null)
            {
                BleManager.DisConnect(ConnectdBLEAddress);
                Debug.Log($"断开连接{ConnectdBLEAddress}");
                ConnectBLEServiceUUID = null;
                ConnectdBLEAddress = null;
            }
            Debug.Log("并未连接设备");
        });
    }

    private void OnSubscribeSuccess(string serviceUUID, string characteristicUUID)
    {
        Debug.Log($"订阅成功{serviceUUID}{characteristicUUID}");
        // if(ConnectdBLEAddress != null)
        PlayerPrefs.SetString("BLEAddress", ConnectdBLEAddress);
        PlayerPrefs.SetString("BLEServiceUUID", ConnectBLEServiceUUID);
        PlayerPrefs.SetString("BLECharacteristicUUID", characteristicNotifyUUID);
    }

    private void OnCharacteristicNotification(string arg0, string arg1, byte[] arg2)
    {
        string arg2Str = System.Text.Encoding.UTF8.GetString(arg2);
        Debug.Log(arg2Str);
    }

    void StartScanButtonListener()
    {
        if (!isScanning)
        {
            isScanning = true;
            StartScan().Forget();
            startScanningBtn.GetComponentInChildren<Text>().text = "停止扫描";
        }
        else
        {
            BleManager.StopScan();
            isScanning = false;
            startScanningBtn.GetComponentInChildren<Text>().text = "开始扫描";
        }
    }
    async UniTaskVoid StartScan()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        BleManager.StartScan();
        Debug.Log("开始扫描");
    }


    /// <summary>
    /// 将扫描到的蓝牙设备显示在UI界面上
    /// </summary>
    /// <param name="address">蓝牙设备地址</param>
    /// <param name="name">蓝牙设备名称</param>
    public void FindingBLE2Dis(string address, string name)
    {
        var _deviceDis = Instantiate(deviceDisPrefab, deviceDisBack);
        _deviceDis.GetComponentInChildren<Text>().text = name;
        _deviceDis.GetComponent<Button>().onClick.AddListener(() =>
        {
            BleManager.StopScan();
            Debug.Log($"连接{address}");
            BleManager.ConnectBLE(address);
        });
    }
    void OnConnectSuccess(string address, string serviceUUID)
    {
        if (ConnectdBLEAddress != null || ConnectBLEServiceUUID != null) return;
        ConnectdBLEAddress = address;
        ConnectBLEServiceUUID = serviceUUID;
        SubscribeCharacteristic();
    }
    void SubscribeCharacteristic()
    {
        if (transform.Find("NoityUUIDInput").TryGetComponent<InputField>(out var inputField))
        {
            if (!string.IsNullOrEmpty(inputField.text))
            {
                characteristicNotifyUUID = inputField.text;
            }
            Debug.Log(inputField.text + "--" + characteristicNotifyUUID);
        }
        BleManager.SubscribeCharacteristicWithDeviceAddress(ConnectdBLEAddress, ConnectBLEServiceUUID, characteristicNotifyUUID);
        Debug.Log($"订阅特征值{characteristicNotifyUUID}");
    }
 
}
