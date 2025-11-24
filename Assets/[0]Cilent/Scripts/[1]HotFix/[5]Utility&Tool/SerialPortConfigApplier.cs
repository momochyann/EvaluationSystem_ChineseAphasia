using UnityEngine;
using SerialPortUtility;
using System.Reflection;

/// <summary>
/// 串口配置应用器
/// 用于将配置参数应用到预制体实例
/// </summary>
public static class SerialPortConfigApplier
{
    /// <summary>
    /// 应用配置到预制体实例
    /// </summary>
    /// <param name="instance">预制体实例</param>
    /// <param name="config">串口配置</param>
    public static void ApplyConfig(GameObject instance, SerialPortConfigSO config)
    {
        if (instance == null || config == null)
        {
            Debug.LogError("[SerialPortConfigApplier] 实例或配置为空");
            return;
        }

        // 应用 SerialPortUtilityPro 配置
        var serialPortUtility = instance.GetComponent<SerialPortUtilityPro>();
        if (serialPortUtility != null)
        {
            serialPortUtility.BaudRate = config.baudRate;
            serialPortUtility.DataBit = config.dataBit;
            serialPortUtility.StopBit = config.stopBit;
            serialPortUtility.Parity = config.parity;
            serialPortUtility.Skip = config.skip;
            serialPortUtility.IsAutoOpen = config.isAutoOpen;
            serialPortUtility.OpenMethod = config.openMethod;
            
            Debug.Log($"[SerialPortConfigApplier] 已应用 SerialPortUtilityPro 配置到 {instance.name}: 波特率={config.baudRate}, 数据位={config.dataBit}, 停止位={config.stopBit}, 校验位={config.parity}, 超时={config.connectionTimeoutMs}ms");
        }
        else
        {
            Debug.LogWarning($"[SerialPortConfigApplier] 预制体 {instance.name} 未找到 SerialPortUtilityPro 组件");
        }

        // 注意：SerialPortBaseUtility 是抽象类，不是 MonoBehaviour，不能通过 GetComponent 获取
        // 如果子类需要配置，应该在子类中重写 BaudRate 和 ConnectionTimeoutMs 属性
        // 或者通过其他方式（如 QFramework 的 Utility 系统）来访问和配置
    }
}

