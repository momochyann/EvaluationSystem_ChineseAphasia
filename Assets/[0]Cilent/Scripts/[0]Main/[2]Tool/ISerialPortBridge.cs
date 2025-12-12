using System.Collections.Generic;

/// <summary>
/// 串口设备列表桥接接口（主程序集）
/// 用于在HybridCLR热更新代码中调用串口设备列表功能
/// </summary>
public interface ISerialPortBridge
{
    /// <summary>
    /// 获取可用的串口设备数量
    /// </summary>
    /// <returns>设备数量</returns>
    int GetDeviceListAvailable();
    
    /// <summary>
    /// 获取指定索引的设备信息
    /// </summary>
    /// <param name="deviceIndex">设备索引（从0开始）</param>
    /// <returns>设备信息字符串，如果失败返回null</returns>
    string GetDeviceList(int deviceIndex);
    
    /// <summary>
    /// 获取所有COM端口信息
    /// </summary>
    /// <returns>COM端口列表，每个元素包含(端口名称, 端口序号)</returns>
    List<(string portName, int portSkip)> GetComPortsWithSkip();
}







