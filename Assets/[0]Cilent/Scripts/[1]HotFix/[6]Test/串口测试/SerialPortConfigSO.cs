using UnityEngine;
using SerialPortUtility;

/// <summary>
/// 串口配置 ScriptableObject
/// 用于配置串口预制体的资源路径和串口参数
/// </summary>
[CreateAssetMenu(fileName = "SerialPortConfig", menuName = "Config/SerialPortConfig")]
public class SerialPortConfigSO : ScriptableObject
{
    [Header("预制体配置")]
    [Tooltip("预制体名称（用于匹配，必须与预制体 GameObject 名称一致）")]
    public string prefabName = "";
    
    [Tooltip("包含 SerialPortUtilityPro 和 SerialPortBaseUtility 的预制体在 YooAsset 中的路径")]
    public string serialPortPrefabPath = "SerialPortPrefab";
    
    [Tooltip("是否从本地包加载预制体")]
    public bool isLocalPackage = false;
    
    [Header("串口参数配置")]
    [Tooltip("波特率")]
    public int baudRate = 9600;
    
    [Tooltip("连接超时时间（毫秒）")]
    public int connectionTimeoutMs = 2000;
    
    [Tooltip("数据位")]
    public SerialPortUtilityPro.DataBitEnum dataBit = SerialPortUtilityPro.DataBitEnum.EightBit;
    
    [Tooltip("停止位")]
    public SerialPortUtilityPro.StopBitEnum stopBit = SerialPortUtilityPro.StopBitEnum.OneBit;
    
    [Tooltip("校验位")]
    public SerialPortUtilityPro.ParityEnum parity = SerialPortUtilityPro.ParityEnum.No;
    
    [Tooltip("端口跳过数量（Skip）")]
    public int skip = 0;
    
    [Tooltip("是否自动打开")]
    public bool isAutoOpen = false;
    
    [Tooltip("打开方式")]
    public SerialPortUtilityPro.OpenSystem openMethod = SerialPortUtilityPro.OpenSystem.NumberOrder;
}

