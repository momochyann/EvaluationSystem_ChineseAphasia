using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
using NovaSamples.UIControls;

/// <summary>
/// 大数据仪表板控制器 - 整合所有图表组件
/// 支持多种图表类型：饼图、折线图、柱状图、环形图
/// </summary>
public class 大数据仪表板控制器_茉 : MonoBehaviour
{
    [Header("用户数据卡片")]
    [SerializeField] private TextBlock 用户总数文本;
    [SerializeField] private TextBlock 评估师人数文本;
    [SerializeField] private TextBlock 男性人数文本;
    [SerializeField] private TextBlock 女性人数文本;
    
    [Header("图表容器")]
    [SerializeField] private UIBlock 折线图容器;
    [SerializeField] private UIBlock 饼图容器;
    [SerializeField] private UIBlock 柱状图容器1; // 失语症类型
    [SerializeField] private UIBlock 柱状图容器2; // BDAE严重程度
    [SerializeField] private UIBlock 环形图容器; // 档案管理
    
    [Header("图表控制器")]
    [SerializeField] private Nova图表适配器_茉 折线图适配器;
    [SerializeField] private Nova图表适配器_茉 饼图适配器;
    [SerializeField] private Nova图表适配器_茉 柱状图适配器1;
    [SerializeField] private Nova图表适配器_茉 柱状图适配器2;
    [SerializeField] private Nova图表适配器_茉 环形图适配器;
    
    [Header("WebView方案（可选）")]
    [SerializeField] private WebView图表控制器_茉 WebView控制器;
    [SerializeField] private bool 使用WebView方案 = false;
    
    [Header("时间筛选")]
    [SerializeField] private Button 日按钮;
    [SerializeField] private Button 周按钮;
    [SerializeField] private Button 月按钮;
    
    private string 当前时间单位 = "日";
    
    void Start()
    {
        初始化界面();
        加载示例数据();
    }
    
    /// <summary>
    /// 初始化界面
    /// </summary>
    void 初始化界面()
    {
        // 时间筛选按钮
        if (日按钮 != null)
        {
            日按钮.OnClicked.AddListener(() => 切换时间单位("日"));
        }
        if (周按钮 != null)
        {
            周按钮.OnClicked.AddListener(() => 切换时间单位("周"));
        }
        if (月按钮 != null)
        {
            月按钮.OnClicked.AddListener(() => 切换时间单位("月"));
        }
    }
    
    /// <summary>
    /// 加载示例数据
    /// </summary>
    void 加载示例数据()
    {
        // 用户数据
        更新用户数据(219850, 150, 599, 139);
        
        // 评估数据（折线图）
        var 评估数据 = new Dictionary<string, float>
        {
            { "2016", 89.4f },
            { "2017", 46.5f },
            { "2018", 32.9f },
            { "2019", 36.7f },
            { "2020", 40.5f },
            { "2021", 10.2f },
            { "2022", 55.0f }
        };
        更新折线图(评估数据);
        
        // 用户年龄占比（饼图）
        var 年龄数据 = new Dictionary<string, float>
        {
            { "幼年(0-6岁)", 18f },
            { "少年(7-17岁)", 12f },
            { "青年(18-40岁)", 20f },
            { "中年(41-65岁)", 32f },
            { "老年(66岁以上)", 18f }
        };
        更新饼图(年龄数据);
        
        // 失语症类型排行（柱状图）
        var 失语症数据 = new Dictionary<string, float>
        {
            { "Broca(BA)", 109f },
            { "Wernicke(WA)", 85f },
            { "Conduction(CA)", 72f },
            { "Transcortical Motor(TCM)", 58f },
            { "Transcortical Sensory(TCS)", 45f },
            { "Mixed Transcortical(MT)", 38f },
            { "Anomic(AA)", 32f },
            { "Global(GA)", 28f },
            { "Thalamic(TA)", 22f },
            { "Subcortical(BAA)", 15f }
        };
        更新柱状图1(失语症数据);
        
        // BDAE严重程度排行（柱状图）
        var 严重程度数据 = new Dictionary<string, float>
        {
            { "0级", 95f },
            { "1级", 78f },
            { "2级", 65f },
            { "3级", 52f },
            { "4级", 38f },
            { "5级", 25f }
        };
        更新柱状图2(严重程度数据);
        
        // 档案管理数据（环形图）
        var 档案数据 = new Dictionary<string, float>
        {
            { "现存量", 12710f },
            { "删除量", 7175f },
            { "其他", 19860f }
        };
        更新环形图(档案数据, 39745, 1.35f, 28.2f);
    }
    
    /// <summary>
    /// 更新用户数据
    /// </summary>
    public void 更新用户数据(int 总数, int 评估师, int 男性, int 女性)
    {
        if (用户总数文本 != null) 用户总数文本.Text = 总数.ToString("N0");
        if (评估师人数文本 != null) 评估师人数文本.Text = 评估师.ToString("N0");
        if (男性人数文本 != null) 男性人数文本.Text = 男性.ToString("N0");
        if (女性人数文本 != null) 女性人数文本.Text = 女性.ToString("N0");
    }
    
    /// <summary>
    /// 更新折线图
    /// </summary>
    public void 更新折线图(Dictionary<string, float> 数据)
    {
        if (使用WebView方案 && WebView控制器 != null)
        {
            WebView控制器.更新折线图(数据);
        }
        else if (折线图适配器 != null)
        {
            // 如果使用XCharts，需要在这里调用XCharts的API
            // 折线图适配器.刷新图表();
        }
    }
    
    /// <summary>
    /// 更新饼图
    /// </summary>
    public void 更新饼图(Dictionary<string, float> 数据)
    {
        if (使用WebView方案 && WebView控制器 != null)
        {
            WebView控制器.更新饼图(数据);
        }
        else if (饼图适配器 != null)
        {
            // 如果使用XCharts，需要在这里调用XCharts的API
            // 饼图适配器.刷新图表();
        }
    }
    
    /// <summary>
    /// 更新柱状图1（失语症类型）
    /// </summary>
    public void 更新柱状图1(Dictionary<string, float> 数据)
    {
        if (使用WebView方案 && WebView控制器 != null)
        {
            // WebView控制器.更新柱状图(数据);
        }
        else if (柱状图适配器1 != null)
        {
            // 柱状图适配器1.刷新图表();
        }
    }
    
    /// <summary>
    /// 更新柱状图2（BDAE严重程度）
    /// </summary>
    public void 更新柱状图2(Dictionary<string, float> 数据)
    {
        if (使用WebView方案 && WebView控制器 != null)
        {
            // WebView控制器.更新柱状图(数据);
        }
        else if (柱状图适配器2 != null)
        {
            // 柱状图适配器2.刷新图表();
        }
    }
    
    /// <summary>
    /// 更新环形图
    /// </summary>
    public void 更新环形图(Dictionary<string, float> 数据, int 总量, float 同比, float 环比)
    {
        if (使用WebView方案 && WebView控制器 != null)
        {
            // WebView控制器.更新环形图(数据, 总量, 同比, 环比);
        }
        else if (环形图适配器 != null)
        {
            // 环形图适配器.刷新图表();
        }
    }
    
    /// <summary>
    /// 切换时间单位
    /// </summary>
    void 切换时间单位(string 单位)
    {
        当前时间单位 = 单位;
        Debug.Log($"[大数据仪表板] 切换到{单位}视图");
        
        // 重新加载数据
        加载示例数据();
    }
    
    /// <summary>
    /// 从数据库加载真实数据
    /// </summary>
    public void 从数据库加载数据()
    {
        // TODO: 实现数据库查询逻辑
        Debug.Log("[大数据仪表板] 从数据库加载数据");
    }
}

