using System.Collections;
using System.Collections.Generic;
using NovaSamples.UIControls;
using UnityEngine;

public class PC首页大数据页面_茉 : 界面基础_韩
{
    [Header("按钮")]
    [SerializeField] Button 返回按钮;
    
    [Header("饼图组件")]
    [SerializeField] Nova饼图组件_茉 用户年龄占比饼图;
    
    void Start()
    {
        if (返回按钮 != null)
        {
            返回按钮.OnClicked.AddListener(() =>
            {
                 FindObjectOfType<界面管理器_韩>().显示界面("评定人管理添加");
            });
        }
        
        // 初始化饼图数据
        初始化饼图数据();
    }
    
    /// <summary>
    /// 初始化饼图数据（用户年龄占比）
    /// </summary>
    void 初始化饼图数据()
    {
        if (用户年龄占比饼图 == null) return;
        
        // 根据图片中的数据设置
        var 年龄数据 = new Dictionary<string, float>
        {
            { "幼年(0-6岁)", 20f },      // 20%
            { "少年(7-17岁)", 12f },     // 12%
            { "青年(18-40岁)", 18f },     // 18%
            { "中年(41-65岁)", 32f },     // 32%
            { "老年(66岁以上)", 18f }      // 18%
        };
        
        // 设置颜色（根据图片中的颜色）
        var 颜色列表 = new List<Color>
        {
            new Color(0.2f, 0.4f, 0.8f),  // 深蓝 - 幼年
            new Color(0.2f, 0.8f, 0.6f),  // 青色 - 少年
            new Color(1f, 0.6f, 0.2f),    // 橙色 - 青年
            new Color(1f, 0.8f, 0.4f),    // 黄色 - 中年
            new Color(1f, 0.4f, 0.6f)     // 粉色 - 老年
        };
        
        用户年龄占比饼图.设置数据(年龄数据, 颜色列表);
    }
}
