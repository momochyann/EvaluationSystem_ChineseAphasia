using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class BleManager
{
	public static bool isInit;
	public static bool isScanning;
	public static HashSet<string> deviceHashSet = new HashSet<string>();
	public static UnityAction<string, string> discoveredPeripheralAction;
	public static UnityAction<string, string, byte[]> onCharacteristicNotificationAction;
	public static UnityAction<string, string> onConnectSuccessAction;
	public static UnityAction<string, string> onNotificationSuccessAction;
	/// <summary>
	/// 初始化蓝牙设备
	/// </summary>
	public static void InitBluethood()
	{
		if (!isInit)
		{
			BluetoothLEHardwareInterface.Initialize(true, false, () =>
			{
				Show("蓝牙初始化成功");//初始化成功执行回调
				isInit = true;
			},
		   (error) =>
		   {
			   Show(error.ToString());//初始化失败的回调响应
		   });
		}
	}
	public static void SendData(string address,string serviceUUID,string characteristicUUID,sbyte[] data)
	{
		
		BluetoothLEHardwareInterface.WriteCharacteristic(address, serviceUUID, characteristicUUID, data,data.Length,false,(_)=>{

		});
	}
	public static bool ConnectBLE(string adress)
	{
		Show("开始连接设备");
		try
		{
			BluetoothLEHardwareInterface.ConnectToPeripheral(adress, null, null, (address, _serviceUUID, _characteristicUUID) =>
			{
				Debug.Log($"连接成功{address}{_serviceUUID}{_characteristicUUID}等待订阅");
				onConnectSuccessAction?.Invoke(address,_serviceUUID);
			});
			return true;
		}
		catch (Exception e)
		{
			Show("连接失败");
			return false;
		}
	}
	public static void SubscribeCharacteristicWithDeviceAddress(string address, string serviceUUID, string characteristicUUID)
	{
		BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(address, serviceUUID, characteristicUUID, OnNotificationSuccess, OnCharacteristicNotification);
	}
	static void OnNotificationSuccess(string serviceUUID, string characteristicUUID)
	{
		onNotificationSuccessAction?.Invoke(serviceUUID, characteristicUUID);
	}
	public static void StartScan()
	{
		Input.location.Start();
		BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, (address, name) =>
		{
			if (deviceHashSet.Contains(name)) return;
			deviceHashSet.Add(name);
			discoveredPeripheralAction?.Invoke(address, name);
		}, (address, name, rssi, advertisingInfo) =>
		{
			if (deviceHashSet.Contains(name)) return;
			deviceHashSet.Add(name);
			discoveredPeripheralAction?.Invoke(address, name);
		});
	}
	public static void StopScan()
	{
		BluetoothLEHardwareInterface.StopScan();
	}
	/// <summary>
	/// 收到数据
	/// </summary>
	/// <param name="arg1"></param>
	/// <param name="arg2"></param>
	/// <param name="arg3"></param>
	static void OnCharacteristicNotification(string arg1, string arg2, byte[] arg3)
	{
		onCharacteristicNotificationAction?.Invoke(arg1, arg2, arg3);
	}
	public static void DisConnect(string address)
	{
		BluetoothLEHardwareInterface.DisconnectPeripheral(address, (bleAddress) =>
		{
			Show(bleAddress + "已经断开");
			isScanning = false;
		});
	}

	static void Show(string info)
	{
		Debug.Log(info);
	}
}
