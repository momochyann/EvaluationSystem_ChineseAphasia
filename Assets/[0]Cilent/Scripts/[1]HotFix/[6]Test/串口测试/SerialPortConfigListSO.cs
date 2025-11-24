using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 串口配置列表 ScriptableObject
/// 用于管理多个串口配置，每个配置对应一个预制体
/// </summary>
[CreateAssetMenu(fileName = "SerialPortConfigList", menuName = "Config/SerialPortConfigList")]
public class SerialPortConfigListSO : ScriptableObject
{
    [Header("串口配置列表")]
    [Tooltip("多个串口配置，每个配置对应一个预制体")]
    public List<SerialPortConfigSO> configs = new List<SerialPortConfigSO>();
}

