using UnityEngine;
using QFramework;
using Cysharp.Threading.Tasks;
using System;
using SerialPortUtility;

/// <summary>
/// 电脑端蓝牙串口检测
/// 负责蓝牙透传的连接和数据接收转发
/// </summary>
public class 电脑端蓝牙串口检测 : MonoBehaviour, IController
{
    private I蓝牙数据同步工具 _同步工具;

    // Start is called before the first frame update
    void Start()
    {
        启动连接蓝牙().Forget();
    }

    public async UniTask 启动连接蓝牙()
    {
        await UniTask.Delay(100);
        var bluetoothUtility = this.GetUtility<SerialPortBluetoothUtility>();
        
        // 注册蓝牙事件
        bluetoothUtility.OnConnectionStatusChanged += (connected) =>
        {
            Debug.Log($"蓝牙连接状态: {connected}");
        };

        bluetoothUtility.OnBluetoothDataReceived += (data) =>
        {
            Debug.Log($"[电脑端蓝牙串口检测] 收到蓝牙数据: {BitConverter.ToString(data)}");
            转发蓝牙数据(data).Forget();
        };

        var serialPortUtility = this.GetComponent<SerialPortUtilityPro>();
        if (serialPortUtility == null)
        {
            Debug.LogError("[电脑端蓝牙串口检测] 未找到 SerialPortUtilityPro 组件");
            return;
        }
        
        Debug.Log("传入串口工具到蓝牙工具: " + serialPortUtility.gameObject.name);
        
        // 绑定串口工具到蓝牙工具类
        bluetoothUtility.BindSreialPortUtility(serialPortUtility);
        await UniTask.Delay(100);
        
        // 连接蓝牙透传设备
        bool bluetoothConnected = await bluetoothUtility.SearchAndConnectAsync();
        if (bluetoothConnected)
        {
            Debug.Log("[电脑端蓝牙串口检测] 蓝牙透传连接成功，等待接收数据");
        }
        else
        {
            Debug.LogWarning("[电脑端蓝牙串口检测] 蓝牙透传连接失败");
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
