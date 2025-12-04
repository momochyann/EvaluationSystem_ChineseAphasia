using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;
using QFramework;

/// <summary>
/// 用户年龄占比饼图组件 - 使用XCharts
/// 从档案录中统计用户年龄分布
/// </summary>

public class 用户年龄占比饼图_茉 : MonoBehaviour, IController
{
    [Header("XCharts组件")]
    [SerializeField] private PieChart 饼图组件;
    
    [Header("配置")]
    [SerializeField] private bool 自动配置样式 = false; // 是否自动配置图例和样式（false=保持外部设置）
    
    private 档案录 档案录模型;
    
    // 年龄段定义
    private readonly string[] 年龄段名称 = new string[]
    {
        "幼年(0-6岁)",
        "少年(7-17岁)",
        "青年(18-40岁)",
        "中年(41-65岁)",
        "老年(66岁以上)"
    };
    
    // 年龄段颜色（根据图片中的颜色）
    private readonly Color[] 年龄段颜色 = new Color[]
    {
        new Color(0.2f, 0.4f, 0.8f),  // 深蓝 - 幼年
        new Color(0.2f, 0.8f, 0.6f),  // 青色 - 少年
        new Color(1f, 0.6f, 0.2f),    // 橙色 - 青年
        new Color(1f, 0.8f, 0.4f),    // 黄色 - 中年
        new Color(1f, 0.4f, 0.6f)     // 粉色 - 老年
    };
    
    void Start()
    {
        初始化();
    }
    
    void 初始化()
    {
        // 获取档案录模型
        档案录模型 = GetArchitecture().GetModel<档案录>();
        
        // 如果没有指定饼图组件，尝试获取
        if (饼图组件 == null)
        {
            饼图组件 = GetComponent<PieChart>();
            if (饼图组件 == null)
            {
                Debug.LogError("[用户年龄占比饼图] 未找到PieChart组件！请在Inspector中指定或添加PieChart组件");
                return;
            }
        }
        
        // 初始化饼图
        配置饼图();
        
        // 注意：数据刷新由外部调用（如界面显示时）
    }
    
    /// <summary>
    /// 配置饼图样式
    /// </summary>
    void 配置饼图()
    {
        if (饼图组件 == null) return;
        
        // 设置标题
        // var title = 饼图组件.GetChartComponent<Title>();
        // if (title != null)
        // {
        //     title.text = "用户年龄占比";
        //     title.show = true;
        // }
        
        // 只在需要时配置图例和样式
        if (自动配置样式)
        {
            // 设置图例（按照图片中的设置）
            var legend = 饼图组件.EnsureChartComponent<Legend>();
            legend.show = true;
            legend.location.align = Location.Align.CenterRight; // 图片中显示的是 Center Right
            legend.location.right = 20;
            legend.orient = Orient.Vertical; // 图片中显示的是 Vertical
            legend.itemWidth = 15;  // 图片中显示的是 15
            legend.itemHeight = 15; // 图片中显示的是 15
            legend.itemGap = 10;    // 图片中显示的是 10
            legend.itemAutoColor = true; // 图片中显示的是 checked
            legend.itemOpacity = 1f;     // 图片中显示的是 1
            
            // 获取或创建Serie
            var serie = 饼图组件.GetSerie(0);
            if (serie == null)
            {
                serie = 饼图组件.AddSerie<Pie>("年龄分布");
            }
            
            // 配置饼图样式
            var pieSerie = serie as Pie;
            if (pieSerie != null)
            {
                // 设置饼图大小和位置
                pieSerie.radius[0] = 0;      // 内半径（0表示实心饼图）
                pieSerie.radius[1] = 0.7f;   // 外半径（相对于容器大小）
                pieSerie.center[0] = 0.4f;  // 中心X位置（0.5表示居中）
                pieSerie.center[1] = 0.5f;  // 中心Y位置
                
                // 启用标签
                var labelStyle = pieSerie.EnsureComponent<LabelStyle>();
                labelStyle.show = true;
                labelStyle.position = LabelStyle.Position.Outside;
                labelStyle.formatter = "{b}: {c} ({d}%)";
                
                // 启用标签线
                var labelLine = pieSerie.EnsureComponent<LabelLine>();
                labelLine.show = true;
                labelLine.lineLength1 = 15;
                labelLine.lineLength2 = 10;
            }
        }
        else
        {
            // 只确保有Serie，不修改样式
            var serie = 饼图组件.GetSerie(0);
            if (serie == null)
            {
                饼图组件.AddSerie<Pie>("年龄分布");
            }
        }
    }
    
    /// <summary>
    /// 刷新数据
    /// </summary>
    public void 刷新数据()
    {
        Debug.Log($"[用户年龄占比饼图] 刷新数据被调用，饼图组件={饼图组件 != null}, 档案录模型={档案录模型 != null}");
        
        if (饼图组件 == null)
        {
            Debug.LogError("[用户年龄占比饼图] 饼图组件为空！");
            return;
        }
        
        if (档案录模型 == null)
        {
            Debug.LogError("[用户年龄占比饼图] 档案录模型为空！尝试重新获取...");
            // 尝试重新获取模型
            档案录模型 = GetArchitecture().GetModel<档案录>();
            if (档案录模型 == null)
            {
                Debug.LogError("[用户年龄占比饼图] 无法获取档案录模型！");
                return;
            }
        }
        
        // 统计年龄分布
        var 年龄统计 = 统计年龄分布();
        
        // 清除旧数据
        饼图组件.RemoveData();
        
        // 获取Serie
        var serie = 饼图组件.GetSerie(0);
        if (serie == null)
        {
            serie = 饼图组件.AddSerie<Pie>("年龄分布");
        }
        
        // 添加数据并设置颜色
        int 数据索引 = 0;
        for (int i = 0; i < 年龄段名称.Length; i++)
        {
            int 数量 = 年龄统计[i];
            if (数量 > 0)
            {
                饼图组件.AddData(serie.index, 数量, 年龄段名称[i]);
                
                // 设置每个数据项的颜色（通过索引获取）
                var serieData = serie.GetSerieData(数据索引);
                if (serieData != null)
                {
                    var itemStyle = serieData.EnsureComponent<ItemStyle>();
                    itemStyle.color = 年龄段颜色[i];
                    itemStyle.show = true;
                }
                
                数据索引++;
            }
        }
        
        Debug.Log($"[用户年龄占比饼图] 数据刷新完成，总计 {年龄统计[0] + 年龄统计[1] + 年龄统计[2] + 年龄统计[3] + 年龄统计[4]} 人");
    }
    
    /// <summary>
    /// 统计年龄分布
    /// </summary>
    int[] 统计年龄分布()
    {
        var 统计结果 = new int[5] { 0, 0, 0, 0, 0 }; // 幼年、少年、青年、中年、老年
        
        if (档案录模型 == null)
        {
            Debug.LogWarning("[用户年龄占比饼图] 档案录模型为空");
            return 统计结果;
        }
        
        var 档案列表 = 档案录模型.GetDataList();
        if (档案列表 == null || 档案列表.Count == 0)
        {
            Debug.LogWarning("[用户年龄占比饼图] 档案列表为空");
            return 统计结果;
        }
        
        foreach (var 档案 in 档案列表)
        {
            if (档案 == null) continue;
            
            // 优先从出生日期计算年龄，如果失败则使用年龄字段
            int 年龄 = -1;
            
            // 首先尝试从出生日期计算
            if (!string.IsNullOrEmpty(档案.出生日期))
            {
                年龄 = 从出生日期计算年龄(档案.出生日期);
            }
            
            // 如果出生日期计算失败，尝试解析年龄字段
            if (年龄 < 0 && !string.IsNullOrEmpty(档案.年龄))
            {
                年龄 = 解析年龄(档案.年龄);
            }
            
            // 分类统计
            if (年龄 >= 0 && 年龄 <= 6)
            {
                统计结果[0]++; // 幼年
            }
            else if (年龄 >= 7 && 年龄 <= 17)
            {
                统计结果[1]++; // 少年
            }
            else if (年龄 >= 18 && 年龄 <= 40)
            {
                统计结果[2]++; // 青年
            }
            else if (年龄 >= 41 && 年龄 <= 65)
            {
                统计结果[3]++; // 中年
            }
            else if (年龄 >= 66)
            {
                统计结果[4]++; // 老年
            }
            // 如果年龄解析失败（返回-1），不统计
        }
        
        return 统计结果;
    }
    
    /// <summary>
    /// 从出生日期计算年龄
    /// 支持格式：yyyy-MM-dd 或 yyyy/MM/dd
    /// </summary>
    int 从出生日期计算年龄(string 出生日期字符串)
    {
        if (string.IsNullOrEmpty(出生日期字符串))
        {
            return -1;
        }
        
        try
        {
            // 尝试解析日期
            System.DateTime 出生日期;
            if (System.DateTime.TryParse(出生日期字符串, out 出生日期))
            {
                var 今天 = System.DateTime.Today;
                int 年龄 = 今天.Year - 出生日期.Year;
                
                // 如果还没过生日，年龄减1
                if (今天.Month < 出生日期.Month || 
                    (今天.Month == 出生日期.Month && 今天.Day < 出生日期.Day))
                {
                    年龄--;
                }
                
                return 年龄 >= 0 ? 年龄 : -1;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[用户年龄占比饼图] 解析出生日期失败: {出生日期字符串}, 错误: {e.Message}");
        }
        
        return -1;
    }
    
    /// <summary>
    /// 解析年龄字符串
    /// 支持格式："X岁"、"X岁Y月"、"X岁Y月Z天"、"X"（纯数字）
    /// </summary>
    int 解析年龄(string 年龄字符串)
    {
        if (string.IsNullOrEmpty(年龄字符串))
        {
            return -1; // 无效年龄
        }
        
        // 尝试直接解析为数字
        if (int.TryParse(年龄字符串, out int 年龄))
        {
            return 年龄 >= 0 ? 年龄 : -1;
        }
        
        // 解析"X岁"格式
        if (年龄字符串.Contains("岁"))
        {
            int 岁索引 = 年龄字符串.IndexOf("岁");
            if (岁索引 > 0)
            {
                string 岁数部分 = 年龄字符串.Substring(0, 岁索引).Trim();
                if (int.TryParse(岁数部分, out 年龄))
                {
                    return 年龄 >= 0 ? 年龄 : -1;
                }
            }
        }
        
        Debug.LogWarning($"[用户年龄占比饼图] 无法解析年龄: {年龄字符串}");
        return -1;
    }
    
    /// <summary>
    /// 手动刷新（可在Inspector中调用）
    /// </summary>
    [ContextMenu("刷新数据")]
    public void 手动刷新()
    {
        刷新数据();
    }
    
    /// <summary>
    /// 实现IController接口
    /// </summary>
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}

