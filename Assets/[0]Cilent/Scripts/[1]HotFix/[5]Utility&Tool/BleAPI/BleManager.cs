using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class BleManager
{
	private const int DefaultMtu = 23;
	private const int DesiredMtu = 247;

	public static bool isInit;
	public static bool isScanning;
	public static bool isConnected;
	public static HashSet<string> deviceHashSet = new HashSet<string>();
	public static UnityAction<string, string> discoveredPeripheralAction;
	public static UnityAction<string, string, byte[]> onCharacteristicNotificationAction;
	public static UnityAction<string, string> onConnectSuccessAction;
	public static UnityAction<string, string> onNotificationSuccessAction;

// #if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
	private static bool s_MtuRequestPending;
	private static bool s_MtuNegotiated;
	private static int s_CurrentMtu = DefaultMtu;
	private static string s_MtuAddress;
// #endif
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
	public static void SendData(string address, string serviceUUID, string characteristicUUID, byte[] data)
	{
		if (data == null || data.Length == 0)
		{
			Debug.LogWarning("[BleManager] 发送数据为空，已忽略");
			return;
		}

// #if UNITY_ANDROID || UNITY_IOS
// 				EnsureMtu(address);
// #endif

		Debug.Log($"发送数据: {BitConverter.ToString(data)}+{address}{serviceUUID}{characteristicUUID}");
		var signedData = Array.ConvertAll(data, b => unchecked((sbyte)b));
		BluetoothLEHardwareInterface.WriteCharacteristic(address, serviceUUID, characteristicUUID, signedData, signedData.Length, false, (_) =>
		{
			Debug.Log($"实际发送数据: {signedData.Length}");
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
				onConnectSuccessAction?.Invoke(address, _serviceUUID);

			});
			return true;
		}
		catch (Exception)
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

// #if UNITY_ANDROID || UNITY_IOS
		s_MtuNegotiated = false;
		s_MtuRequestPending = false;
		s_MtuAddress = null;
		s_CurrentMtu = DefaultMtu;
// #endif
	}

	static void Show(string info)
	{
		Debug.Log(info);
	}

// #if UNITY_ANDROID || UNITY_IOS
	public static void EnsureMtu(string address)
	{
		if (s_MtuNegotiated || s_MtuRequestPending)
		{
			return;
		}

		if (string.IsNullOrEmpty(address))
		{
			Debug.LogWarning("[BleManager] 无法申请MTU，地址为空");
			return;
		}

		s_MtuRequestPending = true;
		s_MtuAddress = address.ToUpperInvariant();

		BluetoothLEHardwareInterface.RequestMtu(address, DesiredMtu, (dev, mtu) =>
		{
			s_CurrentMtu = mtu;
			s_MtuNegotiated = mtu >= DesiredMtu;
			s_MtuRequestPending = false;

			if (!string.IsNullOrEmpty(dev))
			{
				Debug.Log($"[BleManager] 设备 {dev} MTU 升级结果: {mtu}");
			}
			else
			{
				Debug.Log($"[BleManager] MTU 升级结果: {mtu}");
			}
		});
	}
// #endif
}
