using System.Collections.Generic;
using UnityEngine;
using Nova;

/// <summary>
/// 饼图生成脚本
/// 在当前物体下生成一个圆形物体，使用NOVA的UIBlock2D组件呈现
/// </summary>
public class 饼图生成 : MonoBehaviour
{
    [System.Serializable]
    public class 饼图扇形参数
    {
        public string 名称 = "扇形";
        public Color 颜色 = Color.white;
        [Range(0f, 1f)]
        public float 占比 = 0.25f;
    }

    [Header("圆形设置")]
    [Tooltip("圆形的直径（像素）")]
    [Range(10f, 1000f)]
    public float 直径 = 200f;
    
    [Tooltip("默认情况下用于第一个扇形的颜色")]
    public Color 颜色 = Color.white;
    
    [Header("饼图设置")]
    [Tooltip("默认情况下第二个扇形所占的比例（0-1）")]
    [Range(0f, 1f)]
    public float 扇形比例 = 0.5f;
    
    [Tooltip("可选：直接在 Inspector 中配置多个扇形数据")]
    public List<饼图扇形参数> 扇形配置列表 = new List<饼图扇形参数>();
    
    [Tooltip("是否在Start时自动生成")]
    public bool 自动生成 = true;
    
    [Tooltip("生成的圆形物体名称")]
    public string 物体名称 = "圆形物体";
    
    private readonly List<UIBlock2D> 已生成扇形 = new List<UIBlock2D>();
    
    // 用于在编辑器中实时更新
    private float 上次直径值 = 200f;
    private Color 上次颜色值 = Color.white;
    private float 上次扇形比例值 = 0.5f;
    private bool 使用默认双扇形 = true;

    void Start()
    {
        if (自动生成)
        {
            if (扇形配置列表 != null && 扇形配置列表.Count > 0)
            {
                使用默认双扇形 = false;
                根据扇形数据生成(扇形配置列表);
            }
            else
            {
                生成圆形();
            }
        }
        上次直径值 = 直径;
        上次颜色值 = 颜色;
        上次扇形比例值 = 扇形比例;
    }

    /// <summary>
    /// 当Inspector中的值改变时调用（仅在编辑器中）
    /// </summary>
    private void OnValidate()
    {
        // 如果直径改变了，且已经生成，则更新大小
        if (已生成扇形.Count > 0 && 直径 != 上次直径值)
        {
            更新大小(直径);
            上次直径值 = 直径;
        }
        
        bool 有自定义配置 = 扇形配置列表 != null && 扇形配置列表.Count > 0;

        if (有自定义配置)
        {
            使用默认双扇形 = false;
            根据扇形数据生成(扇形配置列表);
        }
        else if (使用默认双扇形)
        {
            bool 颜色变化 = 颜色 != 上次颜色值;
            bool 比例变化 = Mathf.Abs(扇形比例 - 上次扇形比例值) > Mathf.Epsilon;
            if ((颜色变化 || 比例变化) && 已生成扇形.Count > 0)
            {
                生成圆形();
            }
        }
    }

    /// <summary>
    /// 生成圆形物体（默认两个扇形）
    /// </summary>
    public void 生成圆形()
    {
        if (扇形配置列表 != null && 扇形配置列表.Count > 0)
        {
            使用默认双扇形 = false;
            根据扇形数据生成(扇形配置列表);
        }
        else
        {
            float clampedRatio = Mathf.Clamp01(扇形比例);
            float backgroundRatio = Mathf.Clamp01(1f - clampedRatio);
            var 默认数据 = new List<饼图扇形参数>
            {
                new 饼图扇形参数
                {
                    名称 = 物体名称 + "_背景",
                    颜色 = 颜色,
                    占比 = backgroundRatio
                },
                new 饼图扇形参数
                {
                    名称 = 物体名称 + "_扇形",
                    颜色 = 计算反色(颜色),
                    占比 = clampedRatio
                }
            };

            使用默认双扇形 = true;
            根据扇形数据生成(默认数据);
        }

        上次直径值 = 直径;
        上次颜色值 = 颜色;
        上次扇形比例值 = 扇形比例;
    }
    
    /// <summary>
    /// 计算颜色的反色
    /// </summary>
    private Color 计算反色(Color 原色)
    {
        return new Color(1f - 原色.r, 1f - 原色.g, 1f - 原色.b, 原色.a);
    }

    /// <summary>
    /// 将 Inspector 上配置好的扇形列表生成出来
    /// </summary>
    [ContextMenu("根据配置列表生成饼图")]
    public void 根据配置列表生成饼图()
    {
        if (扇形配置列表 == null || 扇形配置列表.Count == 0)
        {
            Debug.LogWarning("扇形配置列表为空，无法生成饼图。");
            return;
        }

        使用默认双扇形 = false;
        根据扇形数据生成(扇形配置列表);
    }

    /// <summary>
    /// 根据二维列表生成饼图
    /// </summary>
    /// <param name="参数列表">二维列表，每个一维列表包含：名称（string）、颜色（Color）、占比（float）</param>
    public void 根据参数列表生成饼图(List<List<object>> 参数列表)
    {
        if (参数列表 == null || 参数列表.Count == 0)
        {
            Debug.LogWarning("参数列表为空，无法生成饼图。");
            return;
        }

        var 临时配置 = new List<饼图扇形参数>();

        for (int i = 0; i < 参数列表.Count; i++)
        {
            List<object> 行 = 参数列表[i];
            if (行 == null || 行.Count < 3)
            {
                Debug.LogWarning($"第 {i} 行数据不足 3 个元素，已跳过。");
                continue;
            }

            string 名称 = 行[0] as string ?? $"扇形_{i}";

            Color 颜色值 = Color.white;
            if (行[1] is Color c)
            {
                颜色值 = c;
            }
            else
            {
                Debug.LogWarning($"第 {i} 行的颜色无法识别，使用白色。");
            }

            float 占比值 = 0f;
            if (行[2] is float f)
            {
                占比值 = f;
            }
            else if (行[2] is double d)
            {
                占比值 = (float)d;
            }
            else if (行[2] is int intVal)
            {
                占比值 = intVal;
            }
            else
            {
                Debug.LogWarning($"第 {i} 行的占比无法识别，使用 0。");
            }

            临时配置.Add(new 饼图扇形参数
            {
                名称 = 名称,
                颜色 = 颜色值,
                占比 = Mathf.Clamp01(占比值)
            });
        }

        if (临时配置.Count == 0)
        {
            Debug.LogWarning("没有有效的扇形配置，无法生成饼图。");
            return;
        }

        使用默认双扇形 = false;
        根据扇形数据生成(临时配置);
    }

    /// <summary>
    /// 根据扇形数据生成饼图
    /// </summary>
    private void 根据扇形数据生成(IList<饼图扇形参数> 数据列表)
    {
        销毁圆形();

        if (数据列表 == null || 数据列表.Count == 0)
        {
            Debug.LogWarning("扇形数据为空。");
            return;
        }

        float 当前起始角 = -90f;
        float 总占比 = 0f;

        foreach (var 数据 in 数据列表)
        {
            if (数据 == null)
            {
                continue;
            }

            float clampedRatio = Mathf.Clamp01(数据.占比);
            if (clampedRatio <= 0f)
            {
                continue;
            }

            float 扇形角度 = clampedRatio * 360f;
            UIBlock2D 扇形 = 创建扇形片(数据.名称, 数据.颜色, 当前起始角, 扇形角度);
            已生成扇形.Add(扇形);

            当前起始角 += 扇形角度;
            总占比 += clampedRatio;
        }

        if (已生成扇形.Count == 0)
        {
            Debug.LogWarning("所有扇形占比都为0，未生成任何图形。");
        }
        else if (Mathf.Abs(总占比 - 1f) > 0.01f)
        {
            Debug.LogWarning($"扇形占比之和为 {总占比:0.##}，未完全覆盖整圆。");
        }
    }

    private UIBlock2D 创建扇形片(string 名称, Color 颜色值, float 起始角, float 扇形角度)
    {
        GameObject 扇形GameObject = new GameObject(string.IsNullOrWhiteSpace(名称) ? $"扇形_{已生成扇形.Count}" : 名称);
        扇形GameObject.transform.SetParent(transform, false);

        UIBlock2D 扇形 = 扇形GameObject.AddComponent<UIBlock2D>();
        扇形.Size = new Length3(直径, 直径, 0f);
        扇形.CornerRadius = new Length
        {
            Raw = 1.0f,
            Type = LengthType.Percent
        };
        扇形.Color = 颜色值;
        扇形.RadialFill = new RadialFill
        {
            Enabled = true,
            Center = Length2.Percentage(0, 0),
            Rotation = 起始角,
            FillAngle = 扇形角度
        };

        return 扇形;
    }

    /// <summary>
    /// 更新所有扇形的大小
    /// </summary>
    public void 更新大小(float 新直径)
    {
        foreach (var 扇形 in 已生成扇形)
        {
            if (扇形 == null)
            {
                continue;
            }

            扇形.Size = new Length3(新直径, 新直径, 0f);
        }

        直径 = 新直径;
    }

    /// <summary>
    /// 销毁已生成的所有扇形
    /// </summary>
    public void 销毁圆形()
    {
        for (int i = 0; i < 已生成扇形.Count; i++)
        {
            UIBlock2D 扇形 = 已生成扇形[i];
            if (扇形 == null)
            {
                continue;
            }

            if (Application.isPlaying)
            {
                Destroy(扇形.gameObject);
            }
            else
            {
                DestroyImmediate(扇形.gameObject);
            }
        }

        已生成扇形.Clear();
    }

    // 在编辑器中可以右键调用
    [ContextMenu("生成圆形")]
    private void 编辑器生成圆形()
    {
        生成圆形();
    }

    [ContextMenu("销毁圆形")]
    private void 编辑器销毁圆形()
    {
        销毁圆形();
    }
}

