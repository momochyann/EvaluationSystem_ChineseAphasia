using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;
using QFramework;
using System.Linq;
using System;
using TMPro;
/// <summary>
/// 档案管理数据环形图组件 - 使用XCharts
/// 统计档案录中的总量、现存量、删除量，显示为环形图
/// </summary>
public class 档案管理数据环形图_茉 : MonoBehaviour, IController
{
    [Header("XCharts组件")]
    [SerializeField] private PieChart 环形图组件;
    
    [Header("中心文本显示组件（可选）")]
    [SerializeField] private TextMeshProUGUI 总量文本;   // 环形图中心的大数字
    [SerializeField] private TextMeshProUGUI 同比文本;   // 中心下方：同比
    [SerializeField] private TextMeshProUGUI 环比文本;   // 中心下方：环比

    [Header("左侧图例文本（可选）")]
    [SerializeField] private TextMeshProUGUI 总量图例文本;   // 左侧 “总量 39745”
    [SerializeField] private TextMeshProUGUI 现存量图例文本; // 左侧 “现存量 12710”
    [SerializeField] private TextMeshProUGUI 删除量图例文本; // 左侧 “删除量 7175”
    
    [Header("配置")]
    [SerializeField] private bool 自动配置样式 = false; // 是否自动配置样式（false=保持外部设置）
    [SerializeField] private bool 使用预设颜色 = true;   // 是否使用脚本内置颜色
    
    private 档案录 档案录模型;
    private 删除档案录 删除档案录模型;
    
    // 数据类型定义（用于 XCharts 的数据与图例）
    private readonly string[] 数据类型名称 = new string[]
    {
        "现存量",
        "删除量",
        "总量"   // 注意：这里的“总量”是真实总量，用于图表和图例
    };
    
    // 数据类型颜色（根据图片中的颜色顺序：现存量、删除量、总量）
    private readonly Color[] 数据类型颜色 = new Color[]
    {
        new Color(0.2f, 0.8f, 0.6f),  // 绿色 - 现存量
        new Color(1f, 0.6f, 0.2f),    // 橙色 - 删除量
        new Color(0.2f, 0.4f, 0.8f)   // 深蓝色 - 总量
    };
    
    void Start()
    {
        初始化();
    }
    
    void 初始化()
    {
        // 获取模型
        档案录模型 = GetArchitecture().GetModel<档案录>();
        删除档案录模型 = GetArchitecture().GetModel<删除档案录>();
        
        // 如果没有指定环形图组件，尝试获取
        if (环形图组件 == null)
        {
            环形图组件 = GetComponent<PieChart>();
            if (环形图组件 == null)
            {
                Debug.LogError("[档案管理数据环形图] 未找到PieChart组件！请在Inspector中指定或添加PieChart组件");
                return;
            }
        }
        
        // 初始化环形图
        配置环形图();
        
        // 刷新数据
        刷新数据();
    }
    
    /// <summary>
    /// 配置环形图样式
    /// </summary>
    void 配置环形图()
    {
        if (环形图组件 == null) return;
        
        if (自动配置样式)
        {
            // 设置标题
            var title = 环形图组件.GetChartComponent<Title>();
            if (title != null)
            {
                title.text = "档案管理数据";
                title.show = true;
            }
            
            // 获取或创建Serie
            var serie = 环形图组件.GetSerie(0);
            if (serie == null)
            {
                serie = 环形图组件.AddSerie<Pie>("档案管理");
            }
            
            // 配置环形图样式
            var pieSerie = serie as Pie;
            if (pieSerie != null)
            {
                // 设置为环形图（设置内半径）
                pieSerie.radius[0] = 0.5f; // 内半径，形成环形
                pieSerie.radius[1] = 0.8f; // 外半径
                
                // 设置标签样式
                var labelStyle = pieSerie.EnsureComponent<LabelStyle>();
                labelStyle.show = true;
                labelStyle.position = LabelStyle.Position.Outside;
                labelStyle.formatter = "{b}: {c}";
                
                // 设置标签线
                var labelLine = pieSerie.EnsureComponent<LabelLine>();
                labelLine.show = true;
                labelLine.lineLength1 = 15;
                labelLine.lineLength2 = 10;
            }
        }
    }
    
    /// <summary>
    /// 刷新数据
    /// </summary>
    public void 刷新数据()
    {
        Debug.Log($"[档案管理数据环形图] 刷新数据被调用，环形图组件={环形图组件 != null}, 档案录模型={档案录模型 != null}, 删除档案录模型={删除档案录模型 != null}");
        
        if (环形图组件 == null)
        {
            Debug.LogError("[档案管理数据环形图] 环形图组件为空！");
            return;
        }
        
        if (档案录模型 == null)
        {
            Debug.LogError("[档案管理数据环形图] 档案录模型为空！尝试重新获取...");
            档案录模型 = GetArchitecture().GetModel<档案录>();
            if (档案录模型 == null)
            {
                Debug.LogError("[档案管理数据环形图] 无法获取档案录模型！");
                return;
            }
        }
        
        if (删除档案录模型 == null)
        {
            Debug.LogError("[档案管理数据环形图] 删除档案录模型为空！尝试重新获取...");
            删除档案录模型 = GetArchitecture().GetModel<删除档案录>();
            if (删除档案录模型 == null)
            {
                Debug.LogError("[档案管理数据环形图] 无法获取删除档案录模型！");
                return;
            }
        }
        
        // 统计数据
        var 统计数据 = 统计档案管理数据();
        
        // 获取Serie
        var serie = 环形图组件.GetSerie(0);
        if (serie == null)
        {
            serie = 环形图组件.AddSerie<Pie>("档案管理");
        }
        else
        {
            // 仅清空数据，避免重置Inspector配置
            serie.ClearData();
        }
        
        // 添加数据
        int 数据索引 = 0;
        for (int i = 0; i < 数据类型名称.Length; i++)
        {
            int 数量 = 统计数据[i];
            if (数量 > 0)
            {
                环形图组件.AddData(serie.index, 数量, 数据类型名称[i]);
                
                // 根据配置决定是否覆盖颜色
                if (使用预设颜色)
                {
                    var serieData = serie.GetSerieData(数据索引);
                    if (serieData != null)
                    {
                        var itemStyle = serieData.EnsureComponent<ItemStyle>();
                        itemStyle.color = 数据类型颜色[i];
                        itemStyle.show = true;
                    }
                }
                数据索引++;
            }
        }
        
        // 更新文本显示
        更新文本显示(统计数据);
        
        Debug.Log($"[档案管理数据环形图] 数据刷新完成，总量={统计数据[3]}, 现存量={统计数据[0]}, 删除量={统计数据[1]}, 其他量={统计数据[2]}");
    }
    
    /// <summary>
    /// 统计档案管理数据
    /// 返回数组：[现存量, 删除量, 总量(用于图表), 总量(用于文本显示)]
    /// </summary>
    int[] 统计档案管理数据()
    {
        int 现存量 = 0;
        int 删除量 = 0;
        int 总量 = 0;
        
        // 统计现存量（当前档案录中的数量）
        if (档案录模型 != null)
        {
            var 档案列表 = 档案录模型.GetDataList();
            if (档案列表 != null)
            {
                现存量 = 档案列表.Count(a => a != null && a.评估状态 == 评估状态.评估完成);
            }
        }
        
        // 统计删除量（删除档案录中的数量）
        if (删除档案录模型 != null)
        {
            var 删除档案列表 = 删除档案录模型.GetDataList();
            if (删除档案列表 != null)
            {
                删除量 = 删除档案列表.Count(a => a != null && a.评估状态 == 评估状态.评估完成);
            }
        }
        
        // 计算总量（现存量 + 删除量）
        总量 = 现存量 + 删除量;
        
        // 这里我们直接把“总量”作为第三个数据返回给 XCharts 使用
        // 这样环形图的数据就包含：现存量、删除量、总量
        return new int[] { 现存量, 删除量, 总量, 总量 };
    }
    
    /// <summary>
    /// 更新文本显示
    /// </summary>
    void 更新文本显示(int[] 统计数据)
    {
        int 现存量 = 统计数据[0];
        int 删除量 = 统计数据[1];
        int 总量 = 统计数据[3];
        
        // 更新总量文本
        if (总量文本 != null)
        {
            总量文本.text = 总量.ToString();
        }

        // 更新左侧图例上的数字显示（如图二所示）
        if (总量图例文本 != null)
        {
            // 形如：总量 39745
            总量图例文本.text = $"总量 {总量}";
        }

        if (现存量图例文本 != null)
        {
            // 形如：现存量 12710
            现存量图例文本.text = $"现存量 {现存量}";
        }

        if (删除量图例文本 != null)
        {
            // 形如：删除量 7175
            删除量图例文本.text = $"删除量 {删除量}";
        }
        
        // 计算同比和环比（这里需要历史数据，暂时显示占位符）
        // 同比：与去年同期相比
        // 环比：与上个月相比
        // 注意：实际项目中需要存储历史数据来计算这些值
        
        if (同比文本 != null)
        {
            // 这里需要根据实际的历史数据计算，暂时显示占位符
            同比文本.text = "同比: +0.00%";
        }
        
        if (环比文本 != null)
        {
            // 这里需要根据实际的历史数据计算，暂时显示占位符
            环比文本.text = "环比: +0.00%";
        }
    }
    
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}

