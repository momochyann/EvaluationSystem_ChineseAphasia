using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

/// <summary>
/// 串口设备列表桥接类实现（主程序集）
/// 在AOT代码中调用P/Invoke，供热更新代码使用
/// </summary>
public class SerialPortBridge : ISerialPortBridge
{
    // P/Invoke声明（必须在AOT代码中）
    [DllImport("spap", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern int spapDeviceListAvailable();

    [DllImport("spap", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern int spapDeviceList(int deviceNum, [MarshalAs(UnmanagedType.LPStr)] StringBuilder deviceInfo, int buffer_size);

    /// <summary>
    /// 获取可用的串口设备数量
    /// </summary>
    public int GetDeviceListAvailable()
    {
        try
        {
            return spapDeviceListAvailable();
        }
        catch (Exception ex)
        {
            Debug.LogError($"获取串口设备数量失败: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// 获取指定索引的设备信息
    /// </summary>
    public string GetDeviceList(int deviceIndex)
    {
        try
        {
            StringBuilder deviceInfo = new StringBuilder(1024);
            int result = spapDeviceList(deviceIndex, deviceInfo, 1024);
            if (result >= 0)
            {
                return deviceInfo.ToString();
            }
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"获取串口设备信息失败 (索引: {deviceIndex}): {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 获取所有COM端口信息
    /// </summary>
    public List<(string portName, int portSkip)> GetComPortsWithSkip()
    {
        List<(string portName, int portSkip)> comPorts = new List<(string, int)>();

        try
        {
            int deviceNum = GetDeviceListAvailable();
            if (deviceNum <= 0)
            {
                return comPorts;
            }

            int comPortIndex = 0; // COM端口的索引（用于NumberOrder模式的Skip值）
            for (int i = 0; i < deviceNum; i++)
            {
                string portInfo = GetDeviceList(i);
                if (string.IsNullOrEmpty(portInfo))
                {
                    continue;
                }

                if (portInfo.Contains("COM"))
                {
                    string comPort = portInfo.Substring(portInfo.IndexOf("COM"));
                    int comIndex = comPort.IndexOf(',');
                    if (comIndex > 0)
                    {
                        comPort = comPort.Substring(0, comIndex);
                    }

                    if (comPort.Length >= 4 && comPort.StartsWith("COM"))
                    {
                        // 在NumberOrder模式下，Skip值应该是COM端口的索引（从0开始）
                        // 而不是COM端口号，因为USB Hub可能导致端口枚举顺序与COM端口号不一致
                        int portNumber = int.Parse(comPort.Substring(3)); // COM端口号（用于显示和保存）
                        comPorts.Add((comPort, comPortIndex)); // portSkip存储的是索引，不是COM端口号
                        Debug.Log($"找到串口: {comPort} (端口号: {portNumber}, Skip索引: {comPortIndex}, 枚举索引: {i})");
                        comPortIndex++;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"获取COM端口失败: {ex.Message}");
        }

        return comPorts;
    }
}






