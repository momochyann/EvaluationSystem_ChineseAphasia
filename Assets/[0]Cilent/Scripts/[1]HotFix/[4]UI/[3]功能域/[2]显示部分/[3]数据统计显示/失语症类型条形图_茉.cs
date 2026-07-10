using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;
using QFramework;
using System.Linq;

/// <summary>
/// 失语症类型条形图组件 - 使用XCharts
/// 统计档案录中失语症类型的数量，显示为水平条形图
/// </summary>
public class 失语症类型条形图_茉 : MonoBehaviour, IController
{
    [Header("XCharts组件")]
    [SerializeField] private BarChart 条形图组件;
    
    [Header("配置")]
    [SerializeField] private bool 自动配置样式 = false; // 是否自动配置样式（false=保持外部设置）
    
    private 档案录 档案录模型;
    
    // 失语症类型定义（按照图片中的顺序）
    private readonly string[] 失语症类型列表 = new string[]
    {
        "BA",  // Broca型
        "WA",  // Wernicke
        "CA",  // 传导型
        "TCM", // 经皮质运动型
        "TCS", // 经皮质感觉型（也可能是TCA）
        "MT",  // 经皮质混合型
        "AA",  // 命名型
        "GA",  // 完全型
        "TA",  // 丘脑型（实际可能是"TA或BaA"）
        "BaA"  // 基底型（实际可能是"TA或BaA"）
    };
    
    // 失语症类型显示名称（用于Y轴标签）
    private readonly string[] 失语症类型名称 = new string[]
    {
        "Broca型(BA)",
        "Wernicke(WA)",
        "传导型(CA)",
        "经皮质运动型(TCM)",
        "经皮质感觉型(TCS)",
        "经皮质混合型(MT)",
        "命名型(AA)",
        "完全型(GA)",
        "丘脑型(TA)",
        "基底型(BaA)"
    };
    
    // 条形图颜色
    private readonly Color 条形颜色 = new Color(1f, 0.2f, 0.2f, 1f); // 红色
    private readonly Color 背景颜色 = new Color(0.8f, 0.8f, 0.8f, 1f); // 浅灰色
    
    void Start()
    {
        档案录模型 = GetArchitecture().GetModel<档案录>();
        
        if (条形图组件 == null)
        {
            条形图组件 = GetComponent<BarChart>();
            if (条形图组件 == null)
            {
                Debug.LogError("[失语症类型条形图] 未找到BarChart组件！请在Inspector中指定或添加BarChart组件");
                return;
            }
        }
        
        配置条形图();
        刷新数据();
    }
    
    /// <summary>
    /// 配置条形图样式
    /// </summary>
    void 配置条形图()
    {
        if (条形图组件 == null) return;
        
        if (自动配置样式)
        {
            // 设置标题
            var title = 条形图组件.GetChartComponent<Title>();
            if (title != null)
            {
                title.text = "失语症类型数据排行";
                title.show = true;
            }
            
            // 配置X轴（数值轴，水平条形图时X轴是数值）
            var xAxis = 条形图组件.GetChartComponent<XAxis>();
            if (xAxis != null)
            {
                xAxis.type = Axis.AxisType.Value;
                xAxis.minMaxType = Axis.AxisMinMaxType.Default;
                xAxis.min = 0;
                // 隐藏X轴的网格线（竖线）
                xAxis.splitLine.show = false;
            }
            
            // 配置Y轴（类目轴，水平条形图时Y轴是类目）
            var yAxis = 条形图组件.GetChartComponent<YAxis>();
            if (yAxis != null)
            {
                yAxis.type = Axis.AxisType.Category;
                yAxis.boundaryGap = true;
            }
            
            // 获取或创建两个Serie（用于堆叠：实际值+背景）
            var serie1 = 条形图组件.GetSerie(0);
            if (serie1 == null)
            {
                serie1 = 条形图组件.AddSerie<Bar>("实际值");
            }
            
            var serie2 = 条形图组件.GetSerie(1);
            if (serie2 == null)
            {
                serie2 = 条形图组件.AddSerie<Bar>("差值");
            }
            
            // 配置条形样式
            var barSerie1 = serie1 as Bar;
            var barSerie2 = serie2 as Bar;
            if (barSerie1 != null && barSerie2 != null)
            {
                // 设置为水平条形图
                barSerie1.barType = BarType.Normal;
                barSerie2.barType = BarType.Normal;
                
                // 设置堆叠（使用相同的stack名称）
                barSerie1.stack = "失语症类型";
                barSerie2.stack = "失语症类型";
                
                // 设置条形宽度
                barSerie1.barWidth = 0.6f;
                barSerie2.barWidth = 0.6f;
            }
            
            // 配置Grid（确保有足够的空间显示Y轴标签）
            var grid = 条形图组件.GetChartComponent<GridCoord>();
            if (grid != null)
            {
                // 为Y轴标签留出空间（左侧）
                grid.left = 120f; // 左侧留出120像素给Y轴标签
            }
        }
    }
    
    /// <summary>
    /// 刷新数据
    /// </summary>
    public void 刷新数据()
    {
        Debug.Log($"[失语症类型条形图] 刷新数据被调用，条形图组件={条形图组件 != null}, 档案录模型={档案录模型 != null}");
        
        if (条形图组件 == null)
        {
            Debug.LogError("[失语症类型条形图] 条形图组件为空！");
            return;
        }
        
        if (档案录模型 == null)
        {
            Debug.LogError("[失语症类型条形图] 档案录模型为空！尝试重新获取...");
            档案录模型 = GetArchitecture().GetModel<档案录>();
            if (档案录模型 == null)
            {
                Debug.LogError("[失语症类型条形图] 无法获取档案录模型！");
                return;
            }
        }
        
        // 统计数据
        var 统计数据 = 统计失语症类型();
        
        // 清除旧数据
        条形图组件.RemoveData();
        
        // 清除Y轴数据（类目轴）
        var yAxis = 条形图组件.GetChartComponent<YAxis>();
        if (yAxis != null)
        {
            yAxis.ClearData();
        }
        
        // 获取两个Serie（实际值和背景）
        var serie1 = 条形图组件.GetSerie(0);
        if (serie1 == null)
        {
            serie1 = 条形图组件.AddSerie<Bar>("实际值");
        }
        
        var serie2 = 条形图组件.GetSerie(1);
        if (serie2 == null)
        {
            serie2 = 条形图组件.AddSerie<Bar>("差值");
        }
        
        // 设置堆叠
        var barSerie1 = serie1 as Bar;
        var barSerie2 = serie2 as Bar;
        if (barSerie1 != null && barSerie2 != null)
        {
            barSerie1.stack = "失语症类型";
            barSerie2.stack = "失语症类型";
        }
        
        // 按照数量从高到低排序
        var 排序数据 = 统计数据
            .Select((数量, 索引) => new { 索引, 数量, 名称 = 失语症类型名称[索引] })
            .OrderByDescending(x => x.数量)
            .ToList();
        
        // 计算最大值（用于背景条的长度）
        int 最大值 = 排序数据.Count > 0 ? 排序数据.Max(x => x.数量) : 1;
        if (最大值 == 0) 最大值 = 1; // 避免除零
        
        // 添加Y轴数据（类目，从下往上）
        for (int i = 排序数据.Count - 1; i >= 0; i--)
        {
            var 数据项 = 排序数据[i];
            if (数据项.数量 > 0 || true) // 显示所有类型，即使数量为0
            {
                条形图组件.AddYAxisData(数据项.名称);
            }
        }
        
        // 先添加背景数据（灰色，从下往上）
        for (int i = 排序数据.Count - 1; i >= 0; i--)
        {
            var 数据项 = 排序数据[i];
            if (数据项.数量 > 0 || true) // 显示所有类型，即使数量为0
            {
                // 背景条的长度 = 最大值 - 实际值
                int 背景值 = 最大值 - 数据项.数量;
                条形图组件.AddData(serie2.index, 背景值);
                
                // 设置背景颜色（灰色）
                var serieData = serie2.GetSerieData(排序数据.Count - 1 - i);
                if (serieData != null)
                {
                    var itemStyle = serieData.EnsureComponent<ItemStyle>();
                    itemStyle.color = 背景颜色;
                }
            }
        }
        
        // 再添加实际数据（红色，从下往上）
        for (int i = 排序数据.Count - 1; i >= 0; i--)
        {
            var 数据项 = 排序数据[i];
            if (数据项.数量 > 0 || true) // 显示所有类型，即使数量为0
            {
                条形图组件.AddData(serie1.index, 数据项.数量);
                
                // 设置实际值颜色（红色）
                var serieData = serie1.GetSerieData(排序数据.Count - 1 - i);
                if (serieData != null)
                {
                    var itemStyle = serieData.EnsureComponent<ItemStyle>();
                    itemStyle.color = 条形颜色;
                }
            }
        }
        
        Debug.Log($"[失语症类型条形图] 数据刷新完成，共{排序数据.Count}个数据点，最大值={最大值}");
    }
    
    /// <summary>
    /// 统计失语症类型数量
    /// </summary>
    int[] 统计失语症类型()
    {
        var 统计结果 = new int[失语症类型列表.Length];
        
        // 初始化所有类型为0
        for (int i = 0; i < 失语症类型列表.Length; i++)
        {
            统计结果[i] = 0;
        }
        
        if (档案录模型 == null)
        {
            Debug.LogWarning("[失语症类型条形图] 档案录模型为空");
            return 统计结果;
        }
        
        var 档案列表 = 档案录模型.GetDataList();
        if (档案列表 == null || 档案列表.Count == 0)
        {
            Debug.LogWarning("[失语症类型条形图] 档案列表为空");
            return 统计结果;
        }
        
        // 统计每种类型的数量
        foreach (var 档案 in 档案列表)
        {
            if (档案 == null || string.IsNullOrEmpty(档案.失语症类型))
                continue;
            
            string 类型 = 档案.失语症类型.Trim();
            
            // 查找匹配的类型索引
            int 类型索引 = -1;
            
            // 特殊处理"TA或BaA"这种情况
            if (类型 == "TA或BaA")
            {
                // 可以根据需要分配给TA或BaA，这里先分配给TA
                类型索引 = 8; // TA的索引
            }
            else
            {
                // 查找匹配的类型
                for (int i = 0; i < 失语症类型列表.Length; i++)
                {
                    if (类型 == 失语症类型列表[i] || 
                        (类型 == "TCA" && 失语症类型列表[i] == "TCS")) // TCA和TCS是同一个类型
                    {
                        类型索引 = i;
                        break;
                    }
                }
            }
            
            if (类型索引 >= 0 && 类型索引 < 失语症类型列表.Length)
            {
                统计结果[类型索引]++;
            }
            else
            {
                Debug.LogWarning($"[失语症类型条形图] 未识别的失语症类型: {类型}");
            }
        }
        
        return 统计结果;
    }
    
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}

