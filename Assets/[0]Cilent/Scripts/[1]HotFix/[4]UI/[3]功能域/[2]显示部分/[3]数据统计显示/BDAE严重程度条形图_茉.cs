using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;
using QFramework;
using System.Linq;

/// <summary>
/// BDAE严重程度条形图组件 - 使用XCharts
/// 统计档案录中BDAE严重程度等级（0-5级）的数量，显示为水平条形图
/// </summary>
public class BDAE严重程度条形图_茉 : MonoBehaviour, IController
{
    [Header("XCharts组件")]
    [SerializeField] private BarChart 条形图组件;
    
    [Header("配置")]
    [SerializeField] private bool 自动配置样式 = false; // 是否自动配置样式（false=保持外部设置）
    
    private 档案录 档案录模型;
    
    // BDAE严重程度等级（0-5级）
    private readonly string[] 等级名称 = new string[]
    {
        "0级",
        "1级",
        "2级",
        "3级",
        "4级",
        "5级"
    };
    
    // 条形图颜色（青色/蓝绿色，类似图片中的颜色）
    private readonly Color 条形颜色 = new Color(0.3f, 0.82f, 0.88f, 1f); // 青色 #4DD0E1
    private readonly Color 背景颜色 = new Color(0.88f, 0.88f, 0.88f, 1f); // 浅灰色 #E0E0E0
    
    void Start()
    {
        档案录模型 = GetArchitecture().GetModel<档案录>();
        
        if (条形图组件 == null)
        {
            条形图组件 = GetComponent<BarChart>();
            if (条形图组件 == null)
            {
                Debug.LogError("[BDAE严重程度条形图] 未找到BarChart组件！请在Inspector中指定或添加BarChart组件");
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
                title.text = "BDAE严重程度数据排行";
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
                serie2 = 条形图组件.AddSerie<Bar>("背景");
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
                barSerie1.stack = "BDAE严重程度";
                barSerie2.stack = "BDAE严重程度";
                
                // 设置条形宽度
                barSerie1.barWidth = 0.6f;
                barSerie2.barWidth = 0.6f;
            }
            
            // 配置Grid（确保有足够的空间显示Y轴标签）
            var grid = 条形图组件.GetChartComponent<GridCoord>();
            if (grid != null)
            {
                // 为Y轴标签留出空间（左侧）
                grid.left = 80f; // 左侧留出80像素给Y轴标签
            }
        }
    }
    
    /// <summary>
    /// 刷新数据
    /// </summary>
    public void 刷新数据()
    {
        Debug.Log($"[BDAE严重程度条形图] 刷新数据被调用，条形图组件={条形图组件 != null}, 档案录模型={档案录模型 != null}");
        
        if (条形图组件 == null)
        {
            Debug.LogError("[BDAE严重程度条形图] 条形图组件为空！");
            return;
        }
        
        if (档案录模型 == null)
        {
            Debug.LogError("[BDAE严重程度条形图] 档案录模型为空！尝试重新获取...");
            档案录模型 = GetArchitecture().GetModel<档案录>();
            if (档案录模型 == null)
            {
                Debug.LogError("[BDAE严重程度条形图] 无法获取档案录模型！");
                return;
            }
        }
        
        // 统计数据
        var 统计数据 = 统计BDAE严重程度();
        
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
            serie2 = 条形图组件.AddSerie<Bar>("背景");
        }
        
        // 设置堆叠
        var barSerie1 = serie1 as Bar;
        var barSerie2 = serie2 as Bar;
        if (barSerie1 != null && barSerie2 != null)
        {
            barSerie1.stack = "BDAE严重程度";
            barSerie2.stack = "BDAE严重程度";
        }
        
        // 按照数量从高到低排序（从0级到5级）
        var 排序数据 = 统计数据
            .Select((数量, 索引) => new { 索引, 数量, 名称 = 等级名称[索引] })
            .OrderByDescending(x => x.数量)
            .ToList();
        
        // 计算最大值（用于背景条的长度）
        int 最大值 = 排序数据.Count > 0 ? 排序数据.Max(x => x.数量) : 1;
        if (最大值 == 0) 最大值 = 1; // 避免除零
        
        // 添加Y轴数据（类目，从下往上，按0-5级的顺序）
        // 注意：这里按原始顺序（0-5级）显示，而不是按数量排序
        for (int i = 等级名称.Length - 1; i >= 0; i--)
        {
            条形图组件.AddYAxisData(等级名称[i]);
        }
        
        // 先添加背景数据（灰色，从下往上，按0-5级的顺序）
        for (int i = 等级名称.Length - 1; i >= 0; i--)
        {
            int 数量 = 统计数据[i];
            // 背景条的长度 = 最大值 - 实际值
            int 背景值 = 最大值 - 数量;
            条形图组件.AddData(serie2.index, 背景值);
            
            // 设置背景颜色（灰色）
            var serieData = serie2.GetSerieData(等级名称.Length - 1 - i);
            if (serieData != null)
            {
                var itemStyle = serieData.EnsureComponent<ItemStyle>();
                itemStyle.color = 背景颜色;
            }
        }
        
        // 再添加实际数据（青色，从下往上，按0-5级的顺序）
        for (int i = 等级名称.Length - 1; i >= 0; i--)
        {
            int 数量 = 统计数据[i];
            条形图组件.AddData(serie1.index, 数量);
            
            // 设置实际值颜色（青色）
            var serieData = serie1.GetSerieData(等级名称.Length - 1 - i);
            if (serieData != null)
            {
                var itemStyle = serieData.EnsureComponent<ItemStyle>();
                itemStyle.color = 条形颜色;
            }
        }
        
        Debug.Log($"[BDAE严重程度条形图] 数据刷新完成，共{等级名称.Length}个等级，最大值={最大值}");
    }
    
    /// <summary>
    /// 统计BDAE严重程度等级数量
    /// </summary>
    int[] 统计BDAE严重程度()
    {
        var 统计结果 = new int[6]; // 0-5级，共6个等级
        
        // 初始化所有等级为0
        for (int i = 0; i < 6; i++)
        {
            统计结果[i] = 0;
        }
        
        if (档案录模型 == null)
        {
            Debug.LogWarning("[BDAE严重程度条形图] 档案录模型为空");
            return 统计结果;
        }
        
        var 档案列表 = 档案录模型.GetDataList();
        if (档案列表 == null || 档案列表.Count == 0)
        {
            Debug.LogWarning("[BDAE严重程度条形图] 档案列表为空");
            return 统计结果;
        }
        
        // 统计每个等级的数量
        foreach (var 档案 in 档案列表)
        {
            if (档案 == null)
                continue;
            
            // 获取BDAE严重程度等级（0-5级）
            int 等级 = 档案.BDAE严重程度等级;
            
            // 确保等级在有效范围内
            if (等级 >= 0 && 等级 <= 5)
            {
                统计结果[等级]++;
            }
            else
            {
                Debug.LogWarning($"[BDAE严重程度条形图] 无效的BDAE严重程度等级: {等级}，应在0-5之间");
            }
        }
        
        return 统计结果;
    }
    
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}

