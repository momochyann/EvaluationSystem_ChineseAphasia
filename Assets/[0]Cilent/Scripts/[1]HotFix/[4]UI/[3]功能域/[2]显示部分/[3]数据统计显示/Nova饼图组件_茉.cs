using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
using TMPro;

/// <summary>
/// Nova UI 饼图/环形图组件 - 使用UIBlock2D的RadialFill实现
/// 支持饼图和环形图两种模式
/// </summary>
[System.Serializable]
public class 饼图数据项
{
    public string 名称;
    public float 数值;
    public Color 颜色;
    [HideInInspector] public float 百分比;
    [HideInInspector] public float 起始角度;
    [HideInInspector] public float 角度范围;
    
    public 饼图数据项(string 名称, float 数值, Color 颜色)
    {
        this.名称 = 名称;
        this.数值 = 数值;
        this.颜色 = 颜色;
    }
}

public class Nova饼图组件_茉 : MonoBehaviour
{
    [Header("图表配置")]
    [SerializeField] private bool 环形图模式 = false; // false=饼图, true=环形图
    [SerializeField] private float 图表半径 = 200f; // 外圆半径
    [SerializeField] private float 内圆半径比例 = 0.4f; // 环形图内圆半径比例（0-1）
    [SerializeField] private float 图表中心X = 0f;
    [SerializeField] private float 图表中心Y = 0f;
    
    [Header("图表数据")]
    [SerializeField] private List<饼图数据项> 数据列表 = new List<饼图数据项>();
    
    [Header("样式配置")]
    [SerializeField] private bool 显示标签 = true;
    [SerializeField] private bool 显示图例 = true;
    [SerializeField] private float 标签字体大小 = 14f;
    [SerializeField] private float 图例字体大小 = 12f;
    [SerializeField] private Color 标签颜色 = Color.white;
    [SerializeField] private float 扇形间距 = 0f; // 扇形之间的间距（度数）
    
    [Header("图例配置")]
    [SerializeField] private UIBlock 图例容器; // 图例容器（可选）
    [SerializeField] private GameObject 图例项预制体; // 图例项预制体（可选）
    [SerializeField] private float 图例间距 = 30f;
    [SerializeField] private bool 图例在右侧 = true; // 图例是否显示在右侧
    [SerializeField] private float 图例偏移X = 250f; // 图例X偏移（右侧为正）
    
    [Header("动画配置")]
    [SerializeField] private bool 启用动画 = true;
    [SerializeField] private float 动画时长 = 1f;
    [SerializeField] private AnimationCurve 动画曲线 = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private List<UIBlock2D> 扇形列表 = new List<UIBlock2D>();
    private List<UIBlock2D> 内圆遮挡列表 = new List<UIBlock2D>(); // 用于环形图的内圆
    private List<TextBlock> 标签列表 = new List<TextBlock>();
    private UIBlock2D 内圆背景; // 环形图的中心圆（背景色）
    
    private float 当前动画进度 = 0f;
    private bool 正在动画中 = false;
    
    void Start()
    {
        if (数据列表.Count > 0)
        {
            刷新图表();
        }
    }
    
    /// <summary>
    /// 设置数据并刷新图表
    /// </summary>
    public void 设置数据(List<饼图数据项> 新数据)
    {
        数据列表 = 新数据;
        刷新图表();
    }
    
    /// <summary>
    /// 设置数据（使用字典）
    /// </summary>
    public void 设置数据(Dictionary<string, float> 数据字典, List<Color> 颜色列表 = null)
    {
        数据列表.Clear();
        
        int 索引 = 0;
        foreach (var kvp in 数据字典)
        {
            Color 颜色 = 颜色列表 != null && 索引 < 颜色列表.Count 
                ? 颜色列表[索引] 
                : 获取默认颜色(索引);
            
            数据列表.Add(new 饼图数据项(kvp.Key, kvp.Value, 颜色));
            索引++;
        }
        
        刷新图表();
    }
    
    /// <summary>
    /// 刷新图表
    /// </summary>
    public void 刷新图表()
    {
        if (数据列表 == null || 数据列表.Count == 0)
        {
            Debug.LogWarning("[Nova饼图] 数据列表为空");
            return;
        }
        
        // 清除旧的图表元素
        清除图表();
        
        // 计算总数值和百分比
        float 总值 = 0f;
        foreach (var 数据 in 数据列表)
        {
            总值 += 数据.数值;
        }
        
        if (总值 <= 0)
        {
            Debug.LogWarning("[Nova饼图] 数据总和为0");
            return;
        }
        
        // 计算每个扇形的角度
        float 当前角度 = -90f; // 从顶部开始（-90度）
        float 总角度 = 360f - (扇形间距 * 数据列表.Count); // 减去所有间距
        
        Debug.Log($"[Nova饼图] 开始创建图表，数据项数={数据列表.Count}，总值={总值}，总角度={总角度}°");
        
        for (int i = 0; i < 数据列表.Count; i++)
        {
            var 数据 = 数据列表[i];
            数据.百分比 = (数据.数值 / 总值) * 100f;
            数据.角度范围 = (数据.数值 / 总值) * 总角度;
            数据.起始角度 = 当前角度;
            
            Debug.Log($"[Nova饼图] 数据项 {i}: {数据.名称}, 数值={数据.数值}, 百分比={数据.百分比:F2}%, 起始角度={数据.起始角度:F1}°, 角度范围={数据.角度范围:F1}°");
            
            // 创建扇形
            创建扇形(i, 数据);
            
            // 创建标签
            if (显示标签)
            {
                创建标签(i, 数据);
            }
            
            当前角度 += 数据.角度范围 + 扇形间距;
        }
        
        Debug.Log($"[Nova饼图] 图表创建完成，最终角度={当前角度}°");
        
        // 创建环形图的内圆（如果需要）
        if (环形图模式)
        {
            创建内圆();
        }
        
        // 创建图例
        if (显示图例 && 图例容器 != null)
        {
            创建图例();
        }
        
        // 播放动画
        if (启用动画)
        {
            StartCoroutine(播放动画());
        }
    }
    
    /// <summary>
    /// 创建扇形
    /// </summary>
    void 创建扇形(int 索引, 饼图数据项 数据)
    {
        GameObject 扇形对象 = new GameObject($"扇形_{索引}_{数据.名称}");
        扇形对象.transform.SetParent(transform, false);
        
        UIBlock2D 扇形 = 扇形对象.AddComponent<UIBlock2D>();
        
        // 设置大小（正方形，边长为直径）
        float 直径 = 图表半径 * 2f;
        扇形.Size = new Length3(
            Length.FixedValue(直径),
            Length.FixedValue(直径),
            Length.FixedValue(0)
        );
        
        // 设置位置（中心）
        扇形.Position = new Length3(
            Length.FixedValue(图表中心X),
            Length.FixedValue(图表中心Y),
            Length.FixedValue(0)
        );
        
        // 设置颜色
        扇形.Color = 数据.颜色;
        
        // 设置ZIndex，确保按顺序叠加（后面的在上层）
        扇形.ZIndex = (short)(10 + 索引);
        
        // 设置RadialFill（关键部分）
        var radialFill = new RadialFill
        {
            Enabled = true,
            Center = Length2.Percentage(0.5f, 0.5f), // 中心位置（50%, 50%）
            Rotation = 数据.起始角度,
            FillAngle = 数据.角度范围
        };
        扇形.RadialFill = radialFill;
        
        // 设置圆角（让边缘更圆滑）
        扇形.CornerRadius = Length.FixedValue(图表半径);
        
        // 初始状态（动画用）
        if (启用动画)
        {
            扇形.RadialFill = new RadialFill
            {
                Enabled = true,
                Center = Length2.Percentage(0.5f, 0.5f),
                Rotation = 数据.起始角度,
                FillAngle = 0f // 初始角度为0
            };
        }
        
        // 调试输出
        Debug.Log($"[Nova饼图] 创建扇形 {索引}: {数据.名称}, 起始角度={数据.起始角度:F1}°, 角度范围={数据.角度范围:F1}°, 百分比={数据.百分比:F1}%");
        
        扇形列表.Add(扇形);
    }
    
    /// <summary>
    /// 创建标签
    /// </summary>
    void 创建标签(int 索引, 饼图数据项 数据)
    {
        // 计算标签位置（扇形中心）
        float 标签角度 = 数据.起始角度 + 数据.角度范围 * 0.5f;
        float 标签距离 = 环形图模式 ? (图表半径 * (1f + 内圆半径比例) * 0.5f) : (图表半径 * 0.7f);
        
        float 弧度 = 标签角度 * Mathf.Deg2Rad;
        float 标签X = 图表中心X + Mathf.Cos(弧度) * 标签距离;
        float 标签Y = 图表中心Y + Mathf.Sin(弧度) * 标签距离;
        
        GameObject 标签对象 = new GameObject($"标签_{索引}");
        标签对象.transform.SetParent(transform, false);
        
        TextBlock 标签 = 标签对象.AddComponent<TextBlock>();
        标签.Text = $"{数据.名称}\n{数据.百分比:F0}%";
        标签.Color = 标签颜色;
        标签.TMP.fontSize = 标签字体大小;
        标签.TMP.alignment = TextAlignmentOptions.Center;
        
        // 设置位置
        标签.Position = new Length3(
            Length.FixedValue(标签X),
            Length.FixedValue(标签Y),
            Length.FixedValue(0)
        );
        
        // 设置大小
        标签.Size = new Length3(
            Length.FixedValue(100f),
            Length.FixedValue(40f),
            Length.FixedValue(0)
        );
        
        // 设置ZIndex，确保标签在扇形之上
        标签.ZIndex = 300;
        
        标签列表.Add(标签);
    }
    
    /// <summary>
    /// 创建环形图的内圆
    /// </summary>
    void 创建内圆()
    {
        // 创建内圆背景（遮挡中心部分）
        GameObject 内圆对象 = new GameObject("内圆背景");
        内圆对象.transform.SetParent(transform, false);
        
        内圆背景 = 内圆对象.AddComponent<UIBlock2D>();
        
        float 内圆直径 = 图表半径 * 2f * 内圆半径比例;
        内圆背景.Size = new Length3(
            Length.FixedValue(内圆直径),
            Length.FixedValue(内圆直径),
            Length.FixedValue(0)
        );
        
        内圆背景.Position = new Length3(
            Length.FixedValue(图表中心X),
            Length.FixedValue(图表中心Y),
            Length.FixedValue(0)
        );
        
        // 设置背景色（通常是白色或透明）
        内圆背景.Color = new Color(1f, 1f, 1f, 1f); // 白色背景
        内圆背景.CornerRadius = Length.FixedValue(内圆直径 * 0.5f);
        
        // 确保内圆在最上层（在所有扇形之上）
        内圆背景.ZIndex = 200;
    }
    
    /// <summary>
    /// 创建图例
    /// </summary>
    void 创建图例()
    {
        if (图例容器 == null) return;
        
        // 设置图例容器的位置（在饼图右侧或左侧）
        float 图例X = 图例在右侧 ? 图例偏移X : -图例偏移X;
        图例容器.Position = new Length3(
            Length.FixedValue(图表中心X + 图例X),
            Length.FixedValue(图表中心Y),
            Length.FixedValue(0)
        );
        
        // 注意：UIBlock 没有 ZIndex 属性，图例会通过位置偏移避免重叠
        
        float 当前Y = (数据列表.Count - 1) * 图例间距 * 0.5f; // 从中间开始，向上向下分布
        
        for (int i = 0; i < 数据列表.Count; i++)
        {
            var 数据 = 数据列表[i];
            
            GameObject 图例项对象;
            if (图例项预制体 != null)
            {
                图例项对象 = Instantiate(图例项预制体, 图例容器.transform);
            }
            else
            {
                图例项对象 = new GameObject($"图例项_{i}");
                图例项对象.transform.SetParent(图例容器.transform, false);
                
                // 创建颜色块
                GameObject 颜色块对象 = new GameObject("颜色块");
                颜色块对象.transform.SetParent(图例项对象.transform, false);
                
                UIBlock2D 颜色块 = 颜色块对象.AddComponent<UIBlock2D>();
                颜色块.Size = new Length3(
                    Length.FixedValue(20f),
                    Length.FixedValue(20f),
                    Length.FixedValue(0)
                );
                颜色块.Color = 数据.颜色;
                颜色块.Position = new Length3(
                    Length.FixedValue(-50f),
                    Length.FixedValue(0),
                    Length.FixedValue(0)
                );
                // 设置颜色块的ZIndex，确保在饼图之上
                颜色块.ZIndex = 150;
                
                // 创建文本
                GameObject 文本对象 = new GameObject("文本");
                文本对象.transform.SetParent(图例项对象.transform, false);
                
                TextBlock 文本 = 文本对象.AddComponent<TextBlock>();
                文本.Text = $"{数据.名称} ({数据.百分比:F0}%)";
                文本.Color = Color.black;
                文本.TMP.fontSize = 图例字体大小;
                文本.TMP.alignment = TextAlignmentOptions.Left;
                文本.Position = new Length3(
                    Length.FixedValue(0),
                    Length.FixedValue(0),
                    Length.FixedValue(0)
                );
                // 设置文本的ZIndex，确保在饼图之上
                文本.ZIndex = 150;
            }
            
            // 设置位置
            var 图例项UIBlock = 图例项对象.GetComponent<UIBlock>();
            if (图例项UIBlock == null)
            {
                图例项UIBlock = 图例项对象.AddComponent<UIBlock>();
            }
            
            图例项UIBlock.Position = new Length3(
                Length.FixedValue(0),
                Length.FixedValue(当前Y),
                Length.FixedValue(0)
            );
            
            当前Y -= 图例间距;
        }
    }
    
    /// <summary>
    /// 播放动画
    /// </summary>
    IEnumerator 播放动画()
    {
        正在动画中 = true;
        float 已用时间 = 0f;
        
        while (已用时间 < 动画时长)
        {
            已用时间 += Time.deltaTime;
            当前动画进度 = 动画曲线.Evaluate(已用时间 / 动画时长);
            
            // 更新每个扇形的角度
            for (int i = 0; i < 扇形列表.Count && i < 数据列表.Count; i++)
            {
                var 扇形 = 扇形列表[i];
                var 数据 = 数据列表[i];
                
                float 当前角度 = 数据.角度范围 * 当前动画进度;
                
                扇形.RadialFill = new RadialFill
                {
                    Enabled = true,
                    Center = Length2.Percentage(0.5f, 0.5f),
                    Rotation = 数据.起始角度,
                    FillAngle = 当前角度
                };
            }
            
            yield return null;
        }
        
        正在动画中 = false;
    }
    
    /// <summary>
    /// 清除图表
    /// </summary>
    void 清除图表()
    {
        foreach (var 扇形 in 扇形列表)
        {
            if (扇形 != null)
            {
                Destroy(扇形.gameObject);
            }
        }
        扇形列表.Clear();
        
        foreach (var 标签 in 标签列表)
        {
            if (标签 != null)
            {
                Destroy(标签.gameObject);
            }
        }
        标签列表.Clear();
        
        if (内圆背景 != null)
        {
            Destroy(内圆背景.gameObject);
            内圆背景 = null;
        }
        
        // 清除图例
        if (图例容器 != null)
        {
            for (int i = 图例容器.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(图例容器.transform.GetChild(i).gameObject);
            }
        }
    }
    
    /// <summary>
    /// 获取默认颜色
    /// </summary>
    Color 获取默认颜色(int 索引)
    {
        Color[] 默认颜色 = new Color[]
        {
            new Color(0.2f, 0.4f, 0.8f), // 深蓝
            new Color(0.2f, 0.8f, 0.6f), // 青色
            new Color(1f, 0.6f, 0.2f),   // 橙色
            new Color(1f, 0.8f, 0.4f),   // 黄色
            new Color(1f, 0.4f, 0.6f),   // 粉色
            new Color(0.6f, 0.4f, 0.8f), // 紫色
            new Color(0.4f, 0.8f, 0.4f), // 绿色
            new Color(0.8f, 0.4f, 0.4f), // 红色
        };
        
        return 默认颜色[索引 % 默认颜色.Length];
    }
    
    void OnDestroy()
    {
        清除图表();
    }
    
    #if UNITY_EDITOR
    /// <summary>
    /// 编辑器模式下预览
    /// </summary>
    [ContextMenu("刷新图表")]
    void 编辑器刷新图表()
    {
        if (Application.isPlaying)
        {
            刷新图表();
        }
    }
    #endif
}

