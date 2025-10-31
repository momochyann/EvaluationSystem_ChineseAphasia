using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Nova;
using System;

/// <summary>
/// 栏目组类型枚举
/// </summary>
public enum 栏目组类型
{
    选项组性,      // 使用选项组性_茉计算总分
    表分择性       // 使用表分择性_茉计算总分
}

/// <summary>
/// 组件获取模式枚举
/// </summary>
public enum 组件获取模式
{
    拖动模式,      // 在Inspector中手动拖入组件
    自动寻找模式   // 使用GetComponentsInChildren自动获取
}


public class 栏目组性_茉 : MonoBehaviour
{
    [Header("基础配置")]
    [SerializeField] public string 维度名称; // 维度名称，如"词命名"、"词复述"等
    [SerializeField] public 栏目组类型 组类型 = 栏目组类型.选项组性; // 栏目组类型
    [SerializeField] TextBlock 总分显示; // 可选的总分显示组件
    
    [Header("自定义分数映射（可选）")]
    [Tooltip("统一分数映射：所有选项组共用此分数列表，留空则使用索引作为分数")]
    [SerializeField] public List<float> 统一分数映射 = new List<float>();
    
    [Tooltip("单独分数映射：为特定选项组单独配置分数，优先级高于统一映射")]
    [SerializeField] public List<选项组分数映射> 单独分数映射列表 = new List<选项组分数映射>();
    
    [Header("组件获取方式")]
    [Tooltip("选择如何获取子组件：拖动模式-手动在Inspector中拖入；自动寻找模式-自动查找子物体")]
    [SerializeField] public 组件获取模式 获取模式 = 组件获取模式.自动寻找模式;
    
    [Tooltip("拖动模式下使用：手动拖入的选项组")]
    [SerializeField] List<选项组性_茉> 手动选项组群 = new List<选项组性_茉>();
    
    [Tooltip("拖动模式下使用：手动拖入的表分择性")]
    [SerializeField] List<表分择性_茉> 手动表分择性群 = new List<表分择性_茉>();
    
    List<选项组性_茉> 子选项组群; // 实际使用的所有选项组（组类型为选项组性时使用）
    List<表分择性_茉> 子表分择性群; // 实际使用的所有表分择性（组类型为表分择性时使用）
    
    // 当前维度的总分
    public int 当前总分 { get; private set; }
    
    // 是否所有组件都已选择
    public bool 是否全部已选择 { get; private set; }
    
    // 总分变化事件
    public static event Action<栏目组性_茉> 总分变化事件;
    
    void Start()
    {
        // 根据获取模式初始化子组件
        初始化子组件();
        
        // 根据组类型订阅事件
        if (组类型 == 栏目组类型.选项组性)
        {
            选项组性_茉.选择变化事件 += 处理选择变化;
        }
        else if (组类型 == 栏目组类型.表分择性)
        {
            表分择性_茉.表分选择变化事件 += 处理选择变化;
        }
        
        // 初始化计算总分
        计算总分();
    }
    
    void OnDestroy()
    {
        // 根据组类型取消订阅事件
        if (组类型 == 栏目组类型.选项组性)
        {
            选项组性_茉.选择变化事件 -= 处理选择变化;
        }
        else if (组类型 == 栏目组类型.表分择性)
        {
            表分择性_茉.表分选择变化事件 -= 处理选择变化;
        }
    }
    
    /// <summary>
    /// 初始化子组件
    /// </summary>
    private void 初始化子组件()
    {
        if (获取模式 == 组件获取模式.拖动模式)
        {
            // 拖动模式：使用手动拖入的组件
            if (组类型 == 栏目组类型.选项组性)
            {
                子选项组群 = 手动选项组群;
                Debug.Log($"[{维度名称}] 使用拖动模式，加载了 {子选项组群.Count} 个选项组");
            }
            else if (组类型 == 栏目组类型.表分择性)
            {
                子表分择性群 = 手动表分择性群;
                Debug.Log($"[{维度名称}] 使用拖动模式，加载了 {子表分择性群.Count} 个表分择性");
            }
        }
        else if (获取模式 == 组件获取模式.自动寻找模式)
        {
            // 自动寻找模式：使用GetComponentsInChildren自动获取
            if (组类型 == 栏目组类型.选项组性)
            {
                子选项组群 = GetComponentsInChildren<选项组性_茉>().ToList();
                Debug.Log($"[{维度名称}] 使用自动寻找模式，找到了 {子选项组群.Count} 个选项组");
            }
            else if (组类型 == 栏目组类型.表分择性)
            {
                子表分择性群 = GetComponentsInChildren<表分择性_茉>().ToList();
                Debug.Log($"[{维度名称}] 使用自动寻找模式，找到了 {子表分择性群.Count} 个表分择性");
            }
        }
    }
    
    /// <summary>
    /// 处理选择变化
    /// </summary>
    private void 处理选择变化()
    {
        计算总分();
    }
    
    /// <summary>
    /// 计算当前维度的总分
    /// </summary>
    private void 计算总分()
    {
        if (组类型 == 栏目组类型.选项组性)
        {
            计算选项组总分();
        }
        else if (组类型 == 栏目组类型.表分择性)
        {
            计算表分择性总分();
        }
        
        // 触发总分变化事件
        总分变化事件?.Invoke(this);
    }
    
    /// <summary>
    /// 获取选项组的分数（支持单独映射、统一映射、默认索引）
    /// 优先级：单独映射 > 统一映射 > 索引
    /// </summary>
    private float 获取选项组分数(int 选项组索引, int 选中索引)
    {
        // 1. 优先查找单独分数映射
        var 单独映射 = 单独分数映射列表?.Find(m => m.选项组索引 == 选项组索引);
        if (单独映射 != null && 单独映射.分数列表 != null && 选中索引 < 单独映射.分数列表.Count)
            return 单独映射.分数列表[选中索引];
        
        // 2. 其次使用统一分数映射
        if (统一分数映射 != null && 统一分数映射.Count > 0 && 选中索引 < 统一分数映射.Count)
            return 统一分数映射[选中索引];
        
        // 3. 最后使用索引作为分数
        return 选中索引;
    }
    
    /// <summary>
    /// 计算选项组总分
    /// </summary>
    private void 计算选项组总分()
    {
        if (子选项组群 == null || 子选项组群.Count == 0)
        {
            当前总分 = 0;
            是否全部已选择 = false;
            更新总分显示("无选项组");
            return;
        }
        
        是否全部已选择 = true;
        float 总分 = 0;
        
        for (int i = 0; i < 子选项组群.Count; i++)
        {
            var 选项组 = 子选项组群[i];
            if (选项组 == null) continue;
            
            if (选项组.当前选中索引 == -1)
            {
                是否全部已选择 = false;
                break;
            }
            else
            {
                总分 += 获取选项组分数(i, 选项组.当前选中索引);
            }
        }
        
        当前总分 = Mathf.RoundToInt(总分);
        更新总分显示(是否全部已选择 ? 总分.ToString() : "待选择完");
    }
    
    /// <summary>
    /// 计算表分择性总分
    /// </summary>
    private void 计算表分择性总分()
    {
        if (子表分择性群 == null || 子表分择性群.Count == 0)
        {
            当前总分 = 0;
            是否全部已选择 = false;
            更新总分显示("无表分择性");
            return;
        }
        
        是否全部已选择 = true;
        int 总分 = 0;
        
        foreach (var 表分择性 in 子表分择性群)
        {
            if (表分择性 == null) continue;
            
            int 当前分数 = 表分择性.获取选择的分数();
            if (当前分数 == -1)
            {
                是否全部已选择 = false;
                break;
            }
            else
            {
                总分 += 当前分数;
            }
        }
        
        当前总分 = 总分;
        更新总分显示(是否全部已选择 ? 总分.ToString() : "待选择完");
    }
    
    /// <summary>
    /// 更新总分显示
    /// </summary>
    /// <param name="文本">要显示的文本</param>
    private void 更新总分显示(string 文本)
    {
        if (总分显示 != null)
        {
            总分显示.Text = 文本;
        }
    }
    
    /// <summary>
    /// 获取该维度下的所有选项组
    /// </summary>
    public List<选项组性_茉> 获取选项组列表()
    {
        return 子选项组群 ?? new List<选项组性_茉>();
    }
    
    /// <summary>
    /// 获取该维度下的所有表分择性
    /// </summary>
    public List<表分择性_茉> 获取表分择性列表()
    {
        return 子表分择性群 ?? new List<表分择性_茉>();
    }
    
    /// <summary>
    /// 设置该维度下所有组件的选择
    /// </summary>
    /// <param name="选择字典">组件名称和选中值的映射</param>
    public void 设置选项组选择(Dictionary<string, int> 选择字典)
    {
        if (选择字典 == null) return;
        
        if (组类型 == 栏目组类型.选项组性)
        {
            设置选项组选择内部(选择字典);
        }
        else if (组类型 == 栏目组类型.表分择性)
        {
            设置表分择性选择内部(选择字典);
        }
    }
    
    /// <summary>
    /// 设置选项组选择（内部方法）
    /// </summary>
    private void 设置选项组选择内部(Dictionary<string, int> 选择字典)
    {
        if (子选项组群 == null) return;
        
        foreach (var 选项组 in 子选项组群)
        {
            if (选项组 == null) continue;
            
            string 组名 = string.IsNullOrEmpty(选项组.选项组名称) ? 选项组.name : 选项组.选项组名称;
            
            if (选择字典.ContainsKey(组名))
            {
                int 保存的索引 = 选择字典[组名];
                选项组.设置选中选项(保存的索引);
                Debug.Log($"[{维度名称}] 还原选项组 [{组名}]，索引: {保存的索引}");
            }
            else
            {
                选项组.清除所有选择();
            }
        }
    }
    
    /// <summary>
    /// 设置表分择性选择（内部方法）
    /// </summary>
    private void 设置表分择性选择内部(Dictionary<string, int> 选择字典)
    {
        if (子表分择性群 == null) return;
        
        foreach (var 表分择性 in 子表分择性群)
        {
            if (表分择性 == null) continue;
            
            string 组名 = 表分择性.name;
            
            if (选择字典.ContainsKey(组名))
            {
                int 保存的分数 = 选择字典[组名];
                表分择性.设置选中分数(保存的分数);
                Debug.Log($"[{维度名称}] 还原表分择性 [{组名}]，分数: {保存的分数}");
            }
            else
            {
                表分择性.清除选择();
            }
        }
    }
    
    /// <summary>
    /// 获取该维度下所有组件的选择
    /// </summary>
    /// <returns>组件名称和选中值的映射</returns>
    public Dictionary<string, int> 获取选项组选择()
    {
        if (组类型 == 栏目组类型.选项组性)
        {
            return 获取选项组选择内部();
        }
        else if (组类型 == 栏目组类型.表分择性)
        {
            return 获取表分择性选择内部();
        }
        
        return new Dictionary<string, int>();
    }
    
    /// <summary>
    /// 获取选项组选择（内部方法）
    /// </summary>
    private Dictionary<string, int> 获取选项组选择内部()
    {
        var 选择字典 = new Dictionary<string, int>();
        if (子选项组群 == null) return 选择字典;
        
        foreach (var 选项组 in 子选项组群)
        {
            if (选项组?.当前选中选项 != null)
            {
                string 组名 = string.IsNullOrEmpty(选项组.选项组名称) ? 选项组.name : 选项组.选项组名称;
                选择字典[组名] = 选项组.当前选中索引;
            }
        }
        
        return 选择字典;
    }
    
    /// <summary>
    /// 获取表分择性选择（内部方法）
    /// </summary>
    private Dictionary<string, int> 获取表分择性选择内部()
    {
        var 选择字典 = new Dictionary<string, int>();
        if (子表分择性群 == null) return 选择字典;
        
        foreach (var 表分择性 in 子表分择性群)
        {
            if (表分择性 == null) continue;
            
            int 选中分数 = 表分择性.获取选择的分数();
            if (选中分数 != -1)
            {
                选择字典[表分择性.name] = 选中分数;
            }
        }
        
        return 选择字典;
    }
    
    /// <summary>
    /// 清除该维度下所有组件的选择
    /// </summary>
    public void 清除所有选择()
    {
        if (组类型 == 栏目组类型.选项组性)
        {
            清除选项组选择();
        }
        else if (组类型 == 栏目组类型.表分择性)
        {
            清除表分择性选择();
        }
    }
    
    /// <summary>
    /// 清除选项组选择
    /// </summary>
    private void 清除选项组选择()
    {
        if (子选项组群 == null) return;
        
        foreach (var 选项组 in 子选项组群)
        {
            if (选项组 != null)
            {
                选项组.清除所有选择();
            }
        }
    }
    
    /// <summary>
    /// 清除表分择性选择
    /// </summary>
    private void 清除表分择性选择()
    {
        if (子表分择性群 == null) return;
        
        foreach (var 表分择性 in 子表分择性群)
        {
            if (表分择性 != null)
            {
                表分择性.清除选择();
            }
        }
    }
    
    /// <summary>
    /// 切换组件获取模式
    /// </summary>
    /// <param name="新模式">要切换到的新模式</param>
    public void 切换获取模式(组件获取模式 新模式)
    {
        if (获取模式 == 新模式)
        {
            Debug.Log($"[{维度名称}] 已经处于 {新模式}，无需切换");
            return;
        }
        
        获取模式 = 新模式;
        
        // 重新初始化子组件
        初始化子组件();
        
        // 重新计算总分
        计算总分();
        
        Debug.Log($"[{维度名称}] 组件获取模式已切换到: {新模式}");
    }
    
    /// <summary>
    /// 在拖动模式和自动寻找模式之间切换
    /// </summary>
    public void 切换获取模式()
    {
        组件获取模式 新模式 = 获取模式 == 组件获取模式.拖动模式 ? 组件获取模式.自动寻找模式 : 组件获取模式.拖动模式;
        切换获取模式(新模式);
    }
    
    /// <summary>
    /// 运行时刷新子组件（当运行时修改了获取模式或手动组件列表时调用）
    /// </summary>
    public void 刷新子组件()
    {
        初始化子组件();
        计算总分();
        Debug.Log($"[{维度名称}] 子组件已刷新");
    }
}
/// <summary>
/// 单独选项组分数映射配置
/// </summary>
[System.Serializable]
public class 选项组分数映射
{
    [Tooltip("选项组在列表中的索引（从0开始）")]
    public int 选项组索引;
    
    [Tooltip("该选项组每个选项的自定义分数")]
    public List<float> 分数列表 = new List<float>();
}
