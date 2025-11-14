using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;

/// <summary>
/// Nova UI 专用曲线绘制器 - 使用UIBlock2D绘制曲线（完全基于Nova布局系统）
/// </summary>
public class Nova曲线绘制器_茉 : MonoBehaviour
{
    [Header("曲线配置")]
    [SerializeField] private Color 曲线颜色 = Color.blue;
    [SerializeField] private float 线段宽度 = 2f;
    [SerializeField] private bool 使用平滑曲线 = false;
    [SerializeField] private int 平滑度 = 10;
    
    private List<UIBlock2D> 线段列表 = new List<UIBlock2D>();

    /// <summary>
    /// 绘制曲线
    /// </summary>
    public void 绘制曲线(List<Vector3> 点列表)
    {
        if (点列表 == null || 点列表.Count < 2)
        {
            Debug.LogWarning("[Nova曲线] 点数不足，无法绘制曲线");
            return;
        }

        // 清除旧的线段
        清除曲线();

        // 生成要绘制的点
        List<Vector3> 绘制点列表;
        if (使用平滑曲线 && 点列表.Count >= 2)
        {
            绘制点列表 = 生成平滑曲线(点列表);
        }
        else
        {
            绘制点列表 = 点列表;
        }

        // Debug.Log($"[Nova曲线] 准备绘制 {绘制点列表.Count - 1} 条线段");
        // Debug.Log($"[Nova曲线] 第一个点: {绘制点列表[0]}, 最后一个点: {绘制点列表[绘制点列表.Count-1]}");

        // 创建线段连接各点
        for (int i = 0; i < 绘制点列表.Count - 1; i++)
        {
            创建Nova线段(绘制点列表[i], 绘制点列表[i + 1], i);
        }
    }

    /// <summary>
    /// 创建一条线段（使用Nova Position属性）
    /// </summary>
    void 创建Nova线段(Vector3 起点, Vector3 终点, int 索引)
    {
        // 创建GameObject并添加UIBlock2D
        GameObject 线段对象 = new GameObject($"线段_{索引}");
        线段对象.transform.SetParent(transform, false); // false = 保持本地坐标
        
        var uiBlock = 线段对象.AddComponent<UIBlock2D>();

        // 计算线段的位置、长度和角度
        Vector3 中点 = (起点 + 终点) / 2f;
        float 长度 = Vector3.Distance(起点, 终点);
        float 角度 = Mathf.Atan2(终点.y - 起点.y, 终点.x - 起点.x) * Mathf.Rad2Deg;

        // 使用Nova的Position属性（固定值）
        uiBlock.Position = new Length3(
            Length.FixedValue(中点.x),
            Length.FixedValue(中点.y),
            Length.FixedValue(中点.z)
        );
        
        // 使用Transform设置旋转
        线段对象.transform.localRotation = Quaternion.Euler(0, 0, 角度);
        
        // 使用Nova的Size属性
        uiBlock.Size = new Length3(
            Length.FixedValue(长度),
            Length.FixedValue(线段宽度),
            Length.FixedValue(0)
        );

        // 设置颜色
        uiBlock.Color = 曲线颜色;

        线段列表.Add(uiBlock);
        
        // if (索引 < 3) // 只打印前几个用于调试
        // {
        //     Debug.Log($"[Nova曲线] 创建线段{索引}: 起点{起点} → 终点{终点}");
        //     Debug.Log($"  中点:{中点}, 长度:{长度}, 角度:{角度}");
        //     Debug.Log($"  Position: X={中点.x}, Y={中点.y}");
        // }
    }

    /// <summary>
    /// 清除所有线段
    /// </summary>
    public void 清除曲线()
    {
        foreach (var 线段 in 线段列表)
        {
            if (线段 != null)
            {
                Destroy(线段.gameObject);
            }
        }
        线段列表.Clear();
    }

    /// <summary>
    /// 生成平滑曲线点
    /// </summary>
    List<Vector3> 生成平滑曲线(List<Vector3> 控制点)
    {
        List<Vector3> 平滑点 = new List<Vector3>();

        if (控制点.Count < 2) return 控制点;

        for (int i = 0; i < 控制点.Count - 1; i++)
        {
            Vector3 p0 = i > 0 ? 控制点[i - 1] : 控制点[i];
            Vector3 p1 = 控制点[i];
            Vector3 p2 = 控制点[i + 1];
            Vector3 p3 = i < 控制点.Count - 2 ? 控制点[i + 2] : 控制点[i + 1];

            for (int j = 0; j < 平滑度; j++)
            {
                float t = j / (float)平滑度;
                Vector3 点 = 计算CatmullRom点(p0, p1, p2, p3, t);
                平滑点.Add(点);
            }
        }

        平滑点.Add(控制点[控制点.Count - 1]);
        return 平滑点;
    }

    /// <summary>
    /// 计算Catmull-Rom样条点
    /// </summary>
    Vector3 计算CatmullRom点(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        Vector3 点 = 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );

        return 点;
    }

    void OnDestroy()
    {
        清除曲线();
    }
}

