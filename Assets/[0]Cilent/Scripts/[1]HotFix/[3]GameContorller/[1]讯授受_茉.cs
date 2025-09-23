using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class 讯授受_茉 : MonoBehaviour, IController
{
    void Start()
    {
        BleManager.onCharacteristicNotificationAction += 受讯令;
    }
    
    void 受讯令(string arg0, string arg1, byte[] arg2)
    {
        // 验证数据帧格式：0xAA | 功能码 | 长度 | Data区 | 校验和 | 0x7E
        if (!验证数据帧(arg2))
        {
            Debug.LogWarning("数据帧格式验证失败");
            return;
        }
        
        // 提取功能码
        byte 功能码 = arg2[1];
        if (功能码 != 0x03)
        {
            Debug.LogWarning($"未知功能码: 0x{功能码:X2}");
            return;
        }
        
        // 提取数据长度
        byte 数据长度 = arg2[2];
        
        // 提取Data区（从索引3开始，到倒数第2个字节结束，排除校验和和结束符）
        byte[] data区 = new byte[数据长度];
        System.Array.Copy(arg2, 3, data区, 0, 数据长度);
        
        // 处理Data区
        处理Data区(data区);
    }
    
    /// <summary>
    /// 验证数据帧格式
    /// </summary>
    /// <param name="数据">接收到的数据</param>
    /// <returns>验证是否通过</returns>
    bool 验证数据帧(byte[] 数据)
    {
        // 最小长度检查：起始符(1) + 功能码(1) + 长度(1) + 至少1字节数据(1) + 校验和(1) + 结束符(1) = 6字节
        if (数据.Length < 6)
        {
            Debug.LogError("数据帧长度不足");
            return false;
        }
        
        // 检查起始符
        if (数据[0] != 0xAA)
        {
            Debug.LogError($"起始符错误，期望0xAA，实际0x{数据[0]:X2}");
            return false;
        }
        
        // 检查结束符
        if (数据[数据.Length - 1] != 0x7E)
        {
            Debug.LogError($"结束符错误，期望0x7E，实际0x{数据[数据.Length - 1]:X2}");
            return false;
        }
        
        // 检查长度字段
        byte 声明长度 = 数据[2];
        int 实际数据长度 = 数据.Length - 5; // 总长度 - 起始符 - 功能码 - 长度字段 - 校验和 - 结束符
        if (声明长度 != 实际数据长度)
        {
            Debug.LogError($"长度字段不匹配，声明{声明长度}，实际{实际数据长度}");
            return false;
        }
        
        // 校验和验证
        byte 计算出的校验和 = 计算校验和(数据,1, 数据.Length - 1); // 不包括校验和和结束符
        byte 接收校验和 = 数据[数据.Length - 2];
        if (计算出的校验和 != 接收校验和)
        {
            Debug.LogError($"校验和不匹配，计算0x{计算出的校验和:X2}，接收0x{接收校验和:X2}");
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// 计算校验和
    /// </summary>
    /// <param name="数据">要计算的数据</param>
    /// <param name="起始索引">起始索引</param>
    /// <param name="结束索引">结束索引（不包含）</param>
    /// <returns>校验和</returns>
    byte 计算校验和(byte[] 数据, int 起始索引, int 结束索引)
    {
        byte sum = 0;
        for (int i = 起始索引; i < 结束索引; i++)
        {
            sum += 数据[i];
        }
        return sum;
    }
    
    /// <summary>
    /// 处理Data区数据
    /// </summary>
    /// <param name="data区">Data区字节数组</param>
    void 处理Data区(byte[] data区)
    {
        if (data区.Length < 1)
        {
            Debug.LogError("Data区长度不足");
            return;
        }
        
        byte 命令类型 = data区[0];
        
        // 处理连接命令
        if (命令类型 == 0x01)
        {
            处理连接命令();
            return;
        }
        
        // 处理数据同步命令
        if (data区.Length < 2)
        {
            Debug.LogError("数据同步命令格式错误，缺少录名标识");
            return;
        }
        
        byte 录名标识 = data区[1];
        string 录名 = 获取录名(录名标识);
        
        if (string.IsNullOrEmpty(录名))
        {
            Debug.LogWarning($"未知录名标识: 0x{录名标识:X2}");
            return;
        }
        
        // 提取JSON数据（从索引2开始）
        // Data区结构：[命令类型][录名标识][JSON数据...]
        // 索引0：命令类型, 索引1：录名标识, 索引2开始：JSON数据
        if (data区.Length <= 2)
        {
            Debug.LogWarning("没有JSON数据");
            return;
        }
        
        byte[] json数据 = new byte[data区.Length - 2];
        System.Array.Copy(data区, 2, json数据, 0, json数据.Length);
        
        string json字符串 = System.Text.Encoding.UTF8.GetString(json数据);
        
        Debug.Log($"接收到蓝牙数据同步命令 - 录名: {录名}, JSON长度: {json字符串.Length}");
        
        // 同步数据到对应的录
        var 录 = this.GetUtility<按名索录之器>().寻录<录元<I记贞>>(录名);
        if (录 != null)
        {
            bool 成功 = 录.纳记于信(json字符串);
            if (成功)
            {
                Debug.Log($"数据同步成功 - 录名: {录名}");
            }
            else
            {
                Debug.LogError($"数据同步失败 - 录名: {录名}");
            }
        }
        else
        {
            Debug.LogError($"未找到录: {录名}");
        }
    }
    
    /// <summary>
    /// 处理连接命令
    /// </summary>
    void 处理连接命令()
    {
        Debug.Log("收到蓝牙连接命令，蓝牙设备已连接");
        
        // 这里可以添加连接成功后的处理逻辑
        // 例如：更新UI状态、发送确认消息等
        
        // 发送连接确认或状态更新
        // 可以在这里触发事件或调用其他系统
    }
    
    /// <summary>
    /// 根据录名标识获取录名
    /// </summary>
    /// <param name="录名标识">录名标识字节</param>
    /// <returns>对应的录名</returns>
    string 获取录名(byte 录名标识)
    {
        switch (录名标识)
        {
            case 0x00:
                return "评人录";
            case 0x01:
                return "用户录";
            case 0x02:
                return "档案录";
            default:
                return null;
        }
    }
    
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
