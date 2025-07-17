using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;

/// <summary>
/// 简单的下拉框示例 - 使用两个平行的List
/// </summary>
public class SimpleDropdownExample : MonoBehaviour
{
    [Header("下拉框配置 - 两个List必须保持相同长度")]
    [SerializeField] private List<string> 下拉框键值 = new List<string>();
    [SerializeField] private List<string> 下拉框显示值 = new List<string>();
    
    [Header("组件引用")]
    [SerializeField] private Dropdown dropdown;
    
    void Start()
    {
        InitializeDropdown();
    }
    
    /// <summary>
    /// 初始化下拉框
    /// </summary>
    private void InitializeDropdown()
    {
        if (dropdown == null) return;
        
        // 验证两个List长度是否一致
        if (下拉框键值.Count != 下拉框显示值.Count)
        {
            Debug.LogError("下拉框键值和显示值的数量不匹配！");
            return;
        }
        
        // 使用显示值初始化下拉框
        // dropdown.SetOptions(下拉框显示值);
        
        Debug.Log("下拉框已初始化，选项数量: " + 下拉框显示值.Count);
    }
    
    /// <summary>
    /// 根据显示值获取键值
    /// </summary>
    public string GetKeyByValue(string displayValue)
    {
        int index = 下拉框显示值.IndexOf(displayValue);
        return index >= 0 && index < 下拉框键值.Count ? 下拉框键值[index] : null;
    }
    
    /// <summary>
    /// 根据键值获取显示值
    /// </summary>
    public string GetValueByKey(string key)
    {
        int index = 下拉框键值.IndexOf(key);
        return index >= 0 && index < 下拉框显示值.Count ? 下拉框显示值[index] : null;
    }
    
    /// <summary>
    /// 添加新的键值对
    /// </summary>
    public void AddOption(string key, string displayValue)
    {
        下拉框键值.Add(key);
        下拉框显示值.Add(displayValue);
    }
    
    /// <summary>
    /// 添加新的键值对 - 使用值元组
    /// </summary>
    public void AddOption((string key, string value) option)
    {
        下拉框键值.Add(option.key);
        下拉框显示值.Add(option.value);
    }
    
    /// <summary>
    /// 获取所有键值对作为值元组
    /// </summary>
    public IEnumerable<(string key, string value)> GetAllOptions()
    {
        int count = Mathf.Min(下拉框键值.Count, 下拉框显示值.Count);
        for (int i = 0; i < count; i++)
        {
            yield return (下拉框键值[i], 下拉框显示值[i]);
        }
    }
} 