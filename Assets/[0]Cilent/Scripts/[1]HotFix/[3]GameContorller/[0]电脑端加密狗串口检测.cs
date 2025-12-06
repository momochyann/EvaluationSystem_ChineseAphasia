using UnityEngine;
using QFramework;
using Cysharp.Threading.Tasks;
using System;
using SerialPortUtility;
using TMPro;
/// <summary>
/// 电脑端加密狗串口检测
/// 负责加密狗的连接、认证和解锁
/// </summary>
public class 电脑端加密狗串口检测 : MonoBehaviour, IController
{

    // Start is called before the first frame update
    // GameObject 遮罩;
    [SerializeField] TextMeshProUGUI 解锁文本;
    void Start()
    {
        //启动连接加密狗().Forget();
    }
    public void DisLockUI(bool isLock)
    {
        if (isLock)
        {
           //解锁文本.text = "解锁成功";
        }
        else
        {
            解锁文本.text = "设备未连接或未解锁";
        }
    }
    public async UniTask<bool> 启动连接加密狗()
    {
        await UniTask.Delay(100);
        var dongleUtility = this.GetUtility<SerialPortDongleUtility>();

        // 注册加密狗事件
        dongleUtility.OnConnectionStatusChanged += (connected) =>
        {
            Debug.Log($"加密狗连接状态: {connected}");
        };

        dongleUtility.OnEncryptionStatusChanged += (encrypted) =>
        {
            Debug.Log($"加密状态: {encrypted}");
        };

        var serialPortUtility = this.GetComponent<SerialPortUtilityPro>();
        if (serialPortUtility == null)
        {
            Debug.LogError("[电脑端加密狗串口检测] 未找到 SerialPortUtilityPro 组件");
            return false;
        }

        Debug.Log("传入串口工具到加密狗工具: " + serialPortUtility.gameObject.name);

        // 绑定串口工具到加密狗工具类
        dongleUtility.BindSreialPortUtility(serialPortUtility);
        await UniTask.Delay(100);

        // 搜索并连接加密狗设备
        bool dongleConnected = await dongleUtility.SearchAndConnectAsync();

        if (dongleConnected)
        {
            // 请求解锁
            bool unlocked = await dongleUtility.RequestUnlockAsync();

            if (unlocked)
            {
                Debug.Log("[电脑端加密狗串口检测] 加密狗解锁成功");
                return true;
            }
            else
            {
                Debug.LogWarning("[电脑端加密狗串口检测] 加密狗解锁失败");
                return false;
            }
        }
        else
        {
            Debug.LogWarning("[电脑端加密狗串口检测] 加密狗连接失败");
            return false;
        }
    }
    public void Disconnect()
    {
        var dongleUtility = this.GetUtility<SerialPortDongleUtility>();
        dongleUtility.Disconnect();
    }
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}

