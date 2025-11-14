using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;

public class 量表表格可视化_茉 : MonoBehaviour
{
    [System.Serializable]
    public class 维度数据
    {
        public string 维度名称;
        public Transform 分数点容器; // 包含0-100分点的父物体
        public int 当前分数;
        [HideInInspector] public Vector3 显示点位置; // 记录当前显示的点的位置
    }

    [Header("维度配置")]
    [SerializeField] private List<维度数据> 维度列表 = new List<维度数据>();

    [Header("曲线配置")]
    [SerializeField] private Nova曲线绘制器_茉 Nova曲线绘制器; // 手动在Inspector中赋值
    
    [Header("调试选项")]
    [SerializeField] private bool 显示详细日志 = false;
    
    void Start()
    {
        if (Nova曲线绘制器 == null)
        {
            Debug.LogError("[量表表格可视化] Nova曲线绘制器未设置！请在Inspector中手动赋值");
        }
    }

    /// <summary>
    /// 设置所有维度的分数并刷新显示
    /// </summary>
    public void 设置分数(Dictionary<string, int> 分数字典)
    {
        foreach (var 维度 in 维度列表)
        {
            if (分数字典.ContainsKey(维度.维度名称))
            {
                维度.当前分数 = 分数字典[维度.维度名称];
                Debug.Log($"[量表表格可视化] 设置 {维度.维度名称} 的分数为 {维度.当前分数}");
            }
        }

        // 延迟刷新，等待Nova UI布局完成
        StartCoroutine(延迟刷新显示());
    }
    
    /// <summary>
    /// 延迟刷新显示（等待布局完成）
    /// </summary>
    IEnumerator 延迟刷新显示()
    {
        // 等待一帧，确保UI布局完成
        yield return null;
        yield return null; // 多等一帧确保稳定
        
        刷新显示();
    }

    /// <summary>
    /// 刷新所有维度的点显示和曲线
    /// </summary>
    public void 刷新显示()
    {
        if (Nova曲线绘制器 == null)
        {
            Debug.LogError("[量表表格可视化] Nova曲线绘制器未设置！");
            return;
        }

        List<Vector3> 曲线绘制器本地坐标列表 = new List<Vector3>();

        Debug.Log($"[量表表格可视化] 开始刷新显示，维度数量: {维度列表.Count}");

        foreach (var 维度 in 维度列表)
        {
            if (维度.分数点容器 == null)
            {
                Debug.LogWarning($"[量表表格可视化] 维度 {维度.维度名称} 的分数点容器未设置");
                continue;
            }

            // 显示对应分数的点，获取相对于曲线绘制器的本地坐标
            Vector3 本地坐标 = 显示分数点(维度);
            曲线绘制器本地坐标列表.Add(本地坐标);
        }

        Debug.Log($"[量表表格可视化] 收集到 {曲线绘制器本地坐标列表.Count} 个点的本地坐标");

        // 绘制曲线
        if (曲线绘制器本地坐标列表.Count > 1)
        {
            绘制曲线(曲线绘制器本地坐标列表);
        }
        else
        {
            Debug.LogWarning($"[量表表格可视化] 点数不足，无法绘制曲线（需要至少2个点）");
        }
    }

    /// <summary>
    /// 显示指定维度的分数点，返回相对于曲线绘制器的本地坐标
    /// </summary>
    Vector3 显示分数点(维度数据 维度)
    {
        int 子物体数量 = 维度.分数点容器.childCount;
        Vector3 本地坐标 = Vector3.zero;

        // 计算应该显示哪个点：取分数的十位数
        int 十位数 = 维度.当前分数 / 10;
        // 计算索引：从后往前数
        int 目标索引 = Mathf.Max(0, 子物体数量 - 1 - 十位数);

        for (int i = 0; i < 子物体数量; i++)
        {
            Transform 子物体 = 维度.分数点容器.GetChild(i);
            
            if (i == 目标索引)
            {
                // 显示这个点
                子物体.gameObject.SetActive(true);
                
                // 转换到曲线绘制器的本地坐标
                本地坐标 = 转换到曲线绘制器坐标(子物体);
                维度.显示点位置 = 本地坐标;
                
                if (显示详细日志)
                {
                    Debug.Log($"[量表表格可视化] {维度.维度名称}: {维度.当前分数}分 -> 显示第{i}个点 ({子物体.name})");
                    Debug.Log($"  世界坐标: {子物体.position}");
                    Debug.Log($"  本地坐标: {本地坐标}");
                }
            }
            else
            {
                // 隐藏其他点
                子物体.gameObject.SetActive(false);
            }
        }

        return 本地坐标;
    }
    
    /// <summary>
    /// 转换世界坐标到曲线绘制器的本地坐标
    /// </summary>
    Vector3 转换到曲线绘制器坐标(Transform 目标)
    {
        if (Nova曲线绘制器 == null)
        {
            Debug.LogWarning("[量表表格可视化] Nova曲线绘制器为空，返回世界坐标");
            return 目标.position;
        }
        
        // 使用InverseTransformPoint将世界坐标转换为曲线绘制器的本地坐标
        Vector3 本地坐标 = Nova曲线绘制器.transform.InverseTransformPoint(目标.position);
        
        if (显示详细日志)
        {
            Debug.Log($"[坐标转换] {目标.name}");
            Debug.Log($"  世界坐标: {目标.position}");
            Debug.Log($"  曲线绘制器本地坐标: {本地坐标}");
        }
        
        return 本地坐标;
    }

    /// <summary>
    /// 绘制连接所有点的曲线
    /// </summary>
    void 绘制曲线(List<Vector3> 点列表)
    {
        if (点列表 == null || 点列表.Count < 2)
        {
            Debug.LogWarning($"[量表表格可视化] 点列表为空或点数不足，点数: {点列表?.Count ?? 0}");
            return;
        }

        if (Nova曲线绘制器 == null)
        {
            Debug.LogError("[量表表格可视化] Nova曲线绘制器未设置！请在Inspector中赋值");
            return;
        }

        // 打印前几个点用于调试
        Debug.Log($"[量表表格可视化] 准备绘制曲线，点数: {点列表.Count}");
        for (int i = 0; i < Mathf.Min(点列表.Count, 3); i++)
        {
            Debug.Log($"  点[{i}]: {点列表[i]}");
        }
        
        // 检查所有点是否都相同
        bool 所有点相同 = true;
        Vector3 第一个点 = 点列表[0];
        foreach (var 点 in 点列表)
        {
            if (Vector3.Distance(点, 第一个点) > 0.01f)
            {
                所有点相同 = false;
                break;
            }
        }
        
        if (所有点相同)
        {
            Debug.LogError($"[量表表格可视化] ⚠️ 所有点的坐标都相同！({第一个点}) 曲线将无法显示！");
            return;
        }

        // 使用Nova曲线绘制器绘制
        Nova曲线绘制器.绘制曲线(点列表);
    }

    /// <summary>
    /// 添加维度（运行时调用）
    /// </summary>
    public void 添加维度(string 名称, Transform 容器, int 分数 = 0)
    {
        维度数据 新维度 = new 维度数据
        {
            维度名称 = 名称,
            分数点容器 = 容器,
            当前分数 = 分数
        };
        维度列表.Add(新维度);
    }

    /// <summary>
    /// 清空所有维度
    /// </summary>
    public void 清空维度()
    {
        维度列表.Clear();
    }

    /// <summary>
    /// 设置单个维度的分数
    /// </summary>
    public void 设置单个维度分数(string 维度名称, int 分数)
    {
        var 维度 = 维度列表.Find(v => v.维度名称 == 维度名称);
        if (维度 != null)
        {
            维度.当前分数 = 分数;
            刷新显示();
        }
        else
        {
            Debug.LogWarning($"[量表表格可视化] 未找到维度: {维度名称}");
        }
    }

}

