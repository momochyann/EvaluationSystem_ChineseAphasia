using System;
using Cysharp.Threading.Tasks;
using QFramework;
using UnityEngine;

public class 讯授受_茉 : MonoBehaviour, IController
{
    private I蓝牙数据同步工具 _同步工具;

    private void Start()
    {
        BleManager.onCharacteristicNotificationAction += 受讯令;
        Debug.Log("讯授受_茉 启动");
    }

    private void OnDestroy()
    {
        BleManager.onCharacteristicNotificationAction -= 受讯令;
    }

    private void 受讯令(string arg0, string arg1, byte[] arg2)
    {
        var 同步工具 = 获取同步工具();
        Debug.Log("受讯令: " + BitConverter.ToString(arg2));
        if (同步工具 == null)
        {
            Debug.LogError("[讯授受_茉] 无法获取蓝牙数据同步工具");
            return;
        }
        
        同步工具.处理接收到的蓝牙原始数据(arg2).Forget();
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