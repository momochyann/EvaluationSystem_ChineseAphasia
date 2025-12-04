using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;
using QFramework;
using System;
using System.Linq;
using Nova;

/// <summary>
/// 评估数据折线图组件 - 使用XCharts
/// 统计档案录中前7个时间单位的档案数量
/// 支持日/周/月三种时间单位切换
/// </summary>
public class 评估数据折线图_茉 : MonoBehaviour, IController
{
    [Header("XCharts组件")]
    [SerializeField] private LineChart 折线图组件;
    
    [Header("时间单位按钮")]
    [SerializeField] private NovaSamples.UIControls.Button 日按钮;
    [SerializeField] private NovaSamples.UIControls.Button 周按钮;
    [SerializeField] private NovaSamples.UIControls.Button 月按钮;
    
    [Header("按钮样式配置")]
    [SerializeField] private Color 按钮选中背景颜色 = new Color(0.2f, 0.4f, 0.8f, 1f); // 深蓝色
    [SerializeField] private Color 按钮未选中背景颜色 = new Color(0.9f, 0.9f, 0.9f, 1f); // 浅灰色
    [SerializeField] private Color 按钮选中文字颜色 = Color.white; // 白色
    [SerializeField] private Color 按钮未选中文字颜色 = new Color(0.3f, 0.3f, 0.3f, 1f); // 深灰色
    
    [Header("配置")]
    [SerializeField] private bool 自动配置样式 = false; // 是否自动配置样式（false=保持外部设置）
    
    private 档案录 档案录模型;
    private 时间单位 当前时间单位 = 时间单位.日;
    
    public enum 时间单位
    {
        日,
        周,
        月
    }
    
    void Start()
    {
        初始化();
    }
    
    void 初始化()
    {
        // 获取档案录模型
        档案录模型 = GetArchitecture().GetModel<档案录>();
        
        // 如果没有指定折线图组件，尝试获取
        if (折线图组件 == null)
        {
            折线图组件 = GetComponent<LineChart>();
            if (折线图组件 == null)
            {
                Debug.LogError("[评估数据折线图] 未找到LineChart组件！请在Inspector中指定或添加LineChart组件");
                return;
            }
        }
        
        // 初始化按钮事件
        初始化按钮();
        
        // 初始化折线图
        配置折线图();
        
        // 刷新数据（默认使用"日"）
        刷新数据();
    }
    
    /// <summary>
    /// 初始化按钮事件
    /// </summary>
    void 初始化按钮()
    {
        if (日按钮 != null)
        {
            日按钮.OnClicked.AddListener(() => 切换时间单位(时间单位.日));
        }
        
        if (周按钮 != null)
        {
            周按钮.OnClicked.AddListener(() => 切换时间单位(时间单位.周));
        }
        
        if (月按钮 != null)
        {
            月按钮.OnClicked.AddListener(() => 切换时间单位(时间单位.月));
        }
        
        // 初始化按钮状态（默认选中"日"）
        更新按钮状态();
    }
    
    /// <summary>
    /// 配置折线图样式
    /// </summary>
    void 配置折线图()
    {
        if (折线图组件 == null) return;
        
        if (自动配置样式)
        {
            // 设置标题
            var title = 折线图组件.GetChartComponent<Title>();
            if (title != null)
            {
                title.text = "评估数据";
                title.show = true;
            }
            
            // 配置X轴（类目轴）
            var xAxis = 折线图组件.GetChartComponent<XAxis>();
            if (xAxis != null)
            {
                xAxis.type = XCharts.Runtime.Axis.AxisType.Category;
                // 设置标签显示间隔为0，确保所有标签都显示
                xAxis.axisLabel.interval = 0;
                xAxis.axisLabel.show = true;
                // 设置标签旋转角度，让标签倾斜显示，节省水平空间
                xAxis.axisLabel.textStyle.rotate = -45f; // 逆时针旋转45度
                // 设置较小的字体大小，节省空间
                if (xAxis.axisLabel.textStyle.fontSize == 0)
                {
                    xAxis.axisLabel.textStyle.fontSize = 16; // 设置较小的字体
                }
                // 设置标签宽度限制，防止标签过宽
                xAxis.axisLabel.width = 60; // 限制标签宽度为60像素
                // 关闭边界间隙，让标签更紧凑
                xAxis.boundaryGap = false;
                // 确保显示首尾标签
                xAxis.axisLabel.showStartLabel = true;
                xAxis.axisLabel.showEndLabel = true;
            }
            
            // 配置Y轴（数值轴）
            var yAxis = 折线图组件.GetChartComponent<YAxis>();
            if (yAxis != null)
            {
                yAxis.type = XCharts.Runtime.Axis.AxisType.Value;
                yAxis.minMaxType = XCharts.Runtime.Axis.AxisMinMaxType.Default;
                yAxis.min = 0;
            }
            
            // 获取或创建Serie
            var serie = 折线图组件.GetSerie(0);
            if (serie == null)
            {
                serie = 折线图组件.AddSerie<Line>("评估数据");
            }
            
            // 配置折线样式
            var lineSerie = serie as Line;
            if (lineSerie != null)
            {
                lineSerie.lineType = LineType.Smooth; // 平滑曲线
                lineSerie.symbol.show = true;
                lineSerie.symbol.type = SymbolType.Circle;
                
                // 启用面积样式（可选）
                var areaStyle = lineSerie.EnsureComponent<AreaStyle>();
                areaStyle.show = true;
                areaStyle.opacity = 0.3f;
            }
        }
        else
        {
            // 只确保有Serie，不修改样式
            var serie = 折线图组件.GetSerie(0);
            if (serie == null)
            {
                折线图组件.AddSerie<Line>("评估数据");
            }
        }
    }
    
    /// <summary>
    /// 切换时间单位
    /// </summary>
    void 切换时间单位(时间单位 新单位)
    {
        if (当前时间单位 == 新单位) return;
        
        当前时间单位 = 新单位;
        Debug.Log($"[评估数据折线图] 切换到{新单位}视图");
        
        // 更新按钮状态（可选，如果需要视觉反馈）
        更新按钮状态();
        
        // 刷新数据
        刷新数据();
    }
    
    /// <summary>
    /// 更新按钮状态（改变颜色显示选中效果）
    /// </summary>
    void 更新按钮状态()
    {
        // 更新日按钮
        设置按钮状态(日按钮, 当前时间单位 == 时间单位.日);
        
        // 更新周按钮
        设置按钮状态(周按钮, 当前时间单位 == 时间单位.周);
        
        // 更新月按钮
        设置按钮状态(月按钮, 当前时间单位 == 时间单位.月);
    }
    
    /// <summary>
    /// 设置按钮的选中/未选中状态
    /// </summary>
    void 设置按钮状态(NovaSamples.UIControls.Button 按钮, bool 是否选中)
    {
        if (按钮 == null) return;
        
        // 从按钮的GameObject上查找UIBlock2D（背景）和TextBlock（文字）
        // 先尝试从当前对象查找
        UIBlock2D 背景 = 按钮.GetComponent<UIBlock2D>();
        TextBlock 文字 = 按钮.GetComponent<TextBlock>();
        
        // 如果当前对象没有，从子对象查找
        if (背景 == null)
        {
            背景 = 按钮.GetComponentInChildren<UIBlock2D>();
        }
        
        if (文字 == null)
        {
            文字 = 按钮.GetComponentInChildren<TextBlock>();
        }
        
        // 设置背景颜色
        if (背景 != null)
        {
            背景.Color = 是否选中 ? 按钮选中背景颜色 : 按钮未选中背景颜色;
        }
        else
        {
            Debug.LogWarning($"[评估数据折线图] 按钮 {按钮.name} 未找到UIBlock2D背景组件");
        }
        
        // 设置文字颜色
        if (文字 != null)
        {
            文字.Color = 是否选中 ? 按钮选中文字颜色 : 按钮未选中文字颜色;
        }
        else
        {
            Debug.LogWarning($"[评估数据折线图] 按钮 {按钮.name} 未找到TextBlock文字组件");
        }
    }
    
    /// <summary>
    /// 刷新数据
    /// </summary>
    public void 刷新数据()
    {
        Debug.Log($"[评估数据折线图] 刷新数据被调用，时间单位={当前时间单位}, 饼图组件={折线图组件 != null}, 档案录模型={档案录模型 != null}");
        
        if (折线图组件 == null)
        {
            Debug.LogError("[评估数据折线图] 折线图组件为空！");
            return;
        }
        
        if (档案录模型 == null)
        {
            Debug.LogError("[评估数据折线图] 档案录模型为空！尝试重新获取...");
            档案录模型 = GetArchitecture().GetModel<档案录>();
            if (档案录模型 == null)
            {
                Debug.LogError("[评估数据折线图] 无法获取档案录模型！");
                return;
            }
        }
        
        // 统计数据
        var 统计数据 = 统计评估数据();
        
        // 清除旧数据
        折线图组件.RemoveData();
        
        // 清除X轴数据（通过XAxis组件）
        var xAxis = 折线图组件.GetChartComponent<XAxis>();
        if (xAxis != null)
        {
            xAxis.ClearData();
            // 确保所有标签都显示（不跳过）
            xAxis.axisLabel.interval = 0;
            // 设置标签旋转角度，让标签倾斜显示，节省水平空间
            xAxis.axisLabel.textStyle.rotate = -45f; // 逆时针旋转45度
            // 设置较小的字体大小，节省空间
            if (xAxis.axisLabel.textStyle.fontSize == 0)
            {
                xAxis.axisLabel.textStyle.fontSize = 10; // 设置较小的字体
            }
            // 设置标签宽度限制，防止标签过宽
            xAxis.axisLabel.width = 60; // 限制标签宽度为60像素
            // 关闭边界间隙，让标签更紧凑
            xAxis.boundaryGap = false;
            // 确保显示首尾标签
            xAxis.axisLabel.showStartLabel = true;
            xAxis.axisLabel.showEndLabel = true;
        }
        
        // 获取Serie
        var serie = 折线图组件.GetSerie(0);
        if (serie == null)
        {
            serie = 折线图组件.AddSerie<Line>("评估数据");
        }
        
        // 添加X轴数据（时间标签）- 确保连续显示
        foreach (var 时间标签 in 统计数据.时间标签列表)
        {
            折线图组件.AddXAxisData(时间标签);
        }
        
        // 添加Y轴数据（数量）
        foreach (var 数量 in 统计数据.数量列表)
        {
            折线图组件.AddData(serie.index, 数量);
        }
        
        // 刷新图表以确保X轴标签正确显示
        折线图组件.RefreshChart();
        
        Debug.Log($"[评估数据折线图] 数据刷新完成，共{统计数据.数量列表.Count}个数据点");
    }
    
    /// <summary>
    /// 统计数据结果
    /// </summary>
    [System.Serializable]
    public class 统计数据结果
    {
        public List<string> 时间标签列表 = new List<string>();
        public List<double> 数量列表 = new List<double>();
    }
    
    /// <summary>
    /// 统计评估数据
    /// </summary>
    统计数据结果 统计评估数据()
    {
        var 结果 = new 统计数据结果();
        
        if (档案录模型 == null)
        {
            Debug.LogWarning("[评估数据折线图] 档案录模型为空");
            return 结果;
        }
        
        var 档案列表 = 档案录模型.GetDataList();
        if (档案列表 == null || 档案列表.Count == 0)
        {
            Debug.LogWarning("[评估数据折线图] 档案列表为空");
            // 即使没有数据，也返回7个空数据点
            for (int i = 0; i < 7; i++)
            {
                结果.时间标签列表.Add(获取时间标签(i));
                结果.数量列表.Add(0);
            }
            return 结果;
        }
        
        // 获取当前时间
        var 当前时间 = DateTime.Now;
        
        // 统计前7个时间单位的数据（从最早到最新，确保连续）
        for (int i = 6; i >= 0; i--) // 从6到0，表示从7个时间单位前到现在
        {
            DateTime 时间单位开始;
            DateTime 时间单位结束;
            string 时间标签 = "";
            
            switch (当前时间单位)
            {
                case 时间单位.日:
                    // 从6天前到今天，共7天，确保连续
                    时间单位开始 = 当前时间.Date.AddDays(-i);
                    时间单位结束 = 时间单位开始.AddDays(1);
                    时间标签 = 时间单位开始.ToString("MM-dd");
                    break;
                    
                case 时间单位.周:
                    // 从6周前到当前周，共7周
                    // 计算当前周的开始日期
                    var 当前周开始 = 获取周开始日期(当前时间);
                    // 往前推i周
                    时间单位开始 = 当前周开始.AddDays(-i * 7);
                    时间单位结束 = 时间单位开始.AddDays(7);
                    var 周开始 = 时间单位开始;
                    var 周结束 = 时间单位结束.AddDays(-1);
                    时间标签 = $"{周开始:MM-dd}\n{周结束:MM-dd}";
                    break;
                    
                case 时间单位.月:
                    // 从6个月前到当前月，共7个月
                    var 当前月第一天 = new DateTime(当前时间.Year, 当前时间.Month, 1);
                    时间单位开始 = 当前月第一天.AddMonths(-i);
                    时间单位结束 = 时间单位开始.AddMonths(1);
                    时间标签 = 时间单位开始.ToString("yyyy-MM");
                    break;
                    
                default:
                    时间单位开始 = 当前时间.Date.AddDays(-i);
                    时间单位结束 = 时间单位开始.AddDays(1);
                    时间标签 = 时间单位开始.ToString("MM-dd");
                    break;
            }
            
            结果.时间标签列表.Add(时间标签);
            
            // 统计该时间单位内的档案数量
            int 数量 = 0;
            foreach (var 档案 in 档案列表)
            {
                if (档案 == null) continue;
                
                // 使用添加时间字段
                if (string.IsNullOrEmpty(档案.添加时间)) continue;
                
                DateTime 添加时间;
                if (DateTime.TryParse(档案.添加时间, out 添加时间))
                {
                    if (添加时间 >= 时间单位开始 && 添加时间 < 时间单位结束)
                    {
                        数量++;
                    }
                }
            }
            
            结果.数量列表.Add(数量);
        }
        
        return 结果;
    }
    
    /// <summary>
    /// 获取周开始日期（周一）
    /// </summary>
    DateTime 获取周开始日期(DateTime 日期)
    {
        int 星期几 = (int)日期.DayOfWeek;
        // C#中DayOfWeek: Sunday=0, Monday=1, ..., Saturday=6
        // 转换为：Monday=0, Tuesday=1, ..., Sunday=6
        int 偏移 = (星期几 == 0) ? 6 : 星期几 - 1;
        return 日期.Date.AddDays(-偏移);
    }
    
    /// <summary>
    /// 获取时间标签（用于空数据时）
    /// </summary>
    string 获取时间标签(int 索引)
    {
        var 当前时间 = DateTime.Now;
        
        switch (当前时间单位)
        {
            case 时间单位.日:
                return 当前时间.Date.AddDays(-(6 - 索引)).ToString("MM-dd");
            case 时间单位.周:
                var 周开始 = 获取周开始日期(当前时间.AddDays(-(6 - 索引) * 7));
                var 周结束 = 周开始.AddDays(6);
                return $"{周开始:MM-dd}\n{周结束:MM-dd}";
            case 时间单位.月:
                return new DateTime(当前时间.Year, 当前时间.Month, 1).AddMonths(-(6 - 索引)).ToString("yyyy-MM");
            default:
                return "";
        }
    }
    
    /// <summary>
    /// 实现IController接口
    /// </summary>
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
    
    /// <summary>
    /// 手动刷新（可在Inspector中调用）
    /// </summary>
    [ContextMenu("刷新数据")]
    public void 手动刷新()
    {
        刷新数据();
    }
}

