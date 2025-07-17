using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Bluethooh : MonoBehaviour
{
	//连接的蓝牙mac地址
	private string bleAddress;
	//保存的uuid
	private string serviceUUID = "";//此处是设备的serviceUUID
									//保存的特征值
	private string characteristicUUID = "";//此处是设备的characteristicUUID 
										   //表示是否开始扫描
	private bool isScanning;
	//表示是否已经蓝牙初始化
	private bool isInit;
	//设备信息预制体 当扫描到一个设备时就会创建一个实例并放到滚动视图中去
	public GameObject deviceInfoPrefab;
	//滚动视图的容器结点
	public Transform container;
	//提示的文本信息框  连接提示。。。
	public Text otherText;
	//断开连接按钮
	public Button disConnectBtn;
	//开始扫描按钮
	public Button startScanningBtn;
	//以设备名字做个存储，相同名字的就忽视不在扫描
	private HashSet<string> deviceHashSet = new HashSet<string>();
	//设备名、设备地址字典
	private Dictionary<string, string> deviceDictionary = new Dictionary<string, string>();

	void Start()
	{
		Input.location.Start(); 	//启用手机位置定位服务
		InitBluethood();
		startScanningBtn.onClick.AddListener(() =>
		{
			StartScnnning();
		});
		disConnectBtn.onClick.AddListener(() =>
		{
			DisConnect();
		});
	}

	/// <summary>
	/// 初始化蓝牙设备
	/// </summary>
	private void InitBluethood()
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
	/// <summary>
	/// 扫描设备  
	/// </summary>
	private void StartScnnning()
	{
		if (!isScanning && isInit)
		{
			Show("扫描设备中");
			Input.location.Start(); //定位
			Invoke("FindDevice", 1f);
			isScanning = true;
		}
	}
	public void FindDevice()
	{
		BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, (address, name) =>
		{
			if (deviceHashSet.Contains(name)) return;
			CreationTextPrefab(address, name);
			//扫描处理，加入设备列表              
			AddPeripheral(name, address);
			deviceHashSet.Add(name);

		}, (address, name, rssi, advertisingInfo) =>
		{
			if (deviceHashSet.Contains(name)) return;
			CreationTextPrefab(address, name);
			AddPeripheral(name, address);
			deviceHashSet.Add(name);
		});
	}

	public void Show(string info)
	{
		// text.text = info;
		Debug.Log(info);
	}
	/// <summary>
	/// 创建提示信息预制件
	/// </summary>
	/// <param name="address"></param>
	/// <param name="name"></param>
	private void CreationTextPrefab(string address, string name)
	{
		GameObject game = Instantiate<GameObject>(deviceInfoPrefab);
		game.transform.Find("Text").GetComponent<Text>().text = "设备名字:" + name + "   " + "地址:" + address + "\n";
		game.transform.localScale = Vector3.one;
		game.transform.SetParent(container);
		Button button = game.transform.Find("Button").GetComponent<Button>();
		button.onClick.AddListener(() =>
		{
			if (deviceHashSet.Contains(name)) //找到名字一样的设备
			{
				BluetoothLEHardwareInterface.StopScan(); //停止扫描
				ConnectBluetooth(deviceDictionary[name]);  //开始连接
			}
		});
	}

	/// <summary>
	/// 更新存储要连接设备的名字，是否扫描到
	/// </summary>
	/// <param name="name"></param>
	/// <param name="address"></param>
	public void AddPeripheral(string name, string address)
	{
		deviceDictionary.Add(name, address);
	}
	/// <summary>
	/// 开始连接设备
	/// </summary>
	public void ConnectBluetooth(string adress)
	{
		Show("开始连接设备");
		try
		{
			BluetoothLEHardwareInterface.ConnectToPeripheral(adress, null, null, (address, serviceUUID, characteristicUUID) =>
			{
				SubscribeCharacteristicWithDeviceAddress(address);
			});
		}
		catch (Exception e)
		{
			Show("连接失败");
		}
	}

	/// <summary>
	/// 连接成功的回调
	/// </summary>
	public void SubscribeCharacteristicWithDeviceAddress(string address)
	{
		otherText.text += "\n" + "尝试接受数据";
		otherText.text = serviceUUID + "\n" + characteristicUUID;
		
		bleAddress = address;
		BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(address, serviceUUID, characteristicUUID, delegate { }, OnCharacteristicNotification);
	}
	/// <summary>
	/// 收到数据
	/// </summary>
	/// <param name="arg1"></param>
	/// <param name="arg2"></param>
	/// <param name="arg3"></param>
	private void OnCharacteristicNotification(string arg1, string arg2, byte[] arg3)
	{
		//Show("接收到数据");
		otherText.text = "连接成功" + "\narg1=" + arg1 + "\narg2=" + arg2 + "\n";
		Show(ByteArrayToString(arg3));
	}
	/// <summary>
	/// 断开连接
	/// </summary>
	public void DisConnect()
	{
		BluetoothLEHardwareInterface.DisconnectPeripheral(bleAddress, (bleAddress) =>
		{
			Show(bleAddress + "已经断开");
			otherText.text = "";
			isScanning = false;
		});
	}
	/// <summary>
	/// 字节组转换为字符串
	/// </summary>
	/// <param name="ba"></param>
	/// <returns></returns>
	public static string ByteArrayToString(byte[] ba)
	{
		string hex = BitConverter.ToString(ba);
		return hex.Replace("-", "");
	}
}