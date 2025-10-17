using UnityEngine;
using NovaSamples.UIControls;

/// <summary>
/// 写病情分数控制器示例
/// 展示如何为写病情创建分数选择UI
/// </summary>
public class 写病情分数控制器_示例 : MonoBehaviour
{
    [Header("引用")]
    [Tooltip("书写栏组件")]
    [SerializeField] private 量表书写栏_茉 书写栏;
    
    [Header("UI方式1：按钮组")]
    [Tooltip("分数按钮（0-5分，共6个按钮）")]
    [SerializeField] private Button[] 分数按钮组;
    
    [Header("UI方式2：下拉框")]
    [Tooltip("分数下拉选择框")]
    [SerializeField] private Dropdown 分数下拉框;
    
    void Start()
    {
        // 如果没有指定书写栏，自动查找
        if (书写栏 == null)
        {
            书写栏 = FindObjectOfType<量表书写栏_茉>();
        }
        
        // 初始化按钮组
        初始化分数按钮();
        
        // 初始化下拉框
        初始化分数下拉框();
    }
    
    /// <summary>
    /// 初始化分数按钮（0-5分）
    /// </summary>
    private void 初始化分数按钮()
    {
        if (分数按钮组 == null || 分数按钮组.Length == 0) return;
        
        for (int i = 0; i < 分数按钮组.Length && i <= 5; i++)
        {
            if (分数按钮组[i] == null) continue;
            
            int 分数 = i; // 捕获当前分数值
            分数按钮组[i].OnClicked.AddListener(() => 点击分数按钮(分数));
            
            Debug.Log($"[写病情分数控制器] 初始化按钮 {i}");
        }
    }
    
    /// <summary>
    /// 初始化分数下拉框
    /// </summary>
    private void 初始化分数下拉框()
    {
        if (分数下拉框 == null) return;
        
        // 清空现有选项
        分数下拉框.DropdownOptions.Options.Clear();
        
        // 添加0-5分选项
        for (int i = 0; i <= 5; i++)
        {
            分数下拉框.DropdownOptions.Options.Add($"{i}分");
        }
        
        // 设置默认选中第一项（0分）
        分数下拉框.DropdownOptions.SelectedIndex = 0;
        
        // 监听选择变化
        分数下拉框.OnValueChanged.AddListener(下拉框选择变化);
        
        // 刷新显示
        分数下拉框.Visuals.OptionsView.Refresh();
        
        Debug.Log("[写病情分数控制器] 下拉框初始化完成");
    }
    
    /// <summary>
    /// 按钮点击事件处理
    /// </summary>
    private void 点击分数按钮(int 分数)
    {
        if (书写栏 == null)
        {
            Debug.LogError("[写病情分数控制器] 书写栏引用为空");
            return;
        }
        
        书写栏.设置写病情分数(分数);
        Debug.Log($"[写病情分数控制器] 通过按钮设置分数: {分数}");
    }
    
    /// <summary>
    /// 下拉框选择变化事件处理
    /// </summary>
    private void 下拉框选择变化(string 选中文本)
    {
        if (书写栏 == null)
        {
            Debug.LogError("[写病情分数控制器] 书写栏引用为空");
            return;
        }
        
        // 从选中的索引获取分数（索引就是分数）
        int 分数 = 分数下拉框.DropdownOptions.SelectedIndex;
        书写栏.设置写病情分数(分数);
        
        Debug.Log($"[写病情分数控制器] 通过下拉框设置分数: {分数}");
    }
    
    // === 以下是公共方法，可以直接在Unity的Button OnClick中调用 ===
    
    /// <summary>
    /// 设置分数为0（可在Inspector中直接绑定到按钮）
    /// </summary>
    public void 设置分数0() => 点击分数按钮(0);
    
    /// <summary>
    /// 设置分数为1（可在Inspector中直接绑定到按钮）
    /// </summary>
    public void 设置分数1() => 点击分数按钮(1);
    
    /// <summary>
    /// 设置分数为2（可在Inspector中直接绑定到按钮）
    /// </summary>
    public void 设置分数2() => 点击分数按钮(2);
    
    /// <summary>
    /// 设置分数为3（可在Inspector中直接绑定到按钮）
    /// </summary>
    public void 设置分数3() => 点击分数按钮(3);
    
    /// <summary>
    /// 设置分数为4（可在Inspector中直接绑定到按钮）
    /// </summary>
    public void 设置分数4() => 点击分数按钮(4);
    
    /// <summary>
    /// 设置分数为5（可在Inspector中直接绑定到按钮）
    /// </summary>
    public void 设置分数5() => 点击分数按钮(5);
    
    void OnDestroy()
    {
        // 清理事件监听
        if (分数下拉框 != null)
        {
            分数下拉框.OnValueChanged.RemoveListener(下拉框选择变化);
        }
    }
}


