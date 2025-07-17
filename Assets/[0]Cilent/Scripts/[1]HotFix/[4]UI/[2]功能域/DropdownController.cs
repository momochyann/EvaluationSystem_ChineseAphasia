using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;

/// <summary>
/// 下拉框控制器示例 - 使用值元组配置
/// </summary>
public class DropdownController : MonoBehaviour
{
    [Header("下拉框配置")]
    [SerializeField] private DropdownConfig dropdownConfig = new DropdownConfig();
    
    [Header("组件引用")]
    [SerializeField] private Dropdown dropdown;
    
    void Start()
    {
        InitializeDropdown();
        
        // 监听下拉框值变化
        if (dropdown != null)
        {
            dropdown.OnValueChanged.AddListener(OnDropdownValueChanged);
        }
    }
    
    /// <summary>
    /// 初始化下拉框
    /// </summary>
    private void InitializeDropdown()
    {
        if (dropdown == null || dropdownConfig == null) return;
        
        // 获取所有显示值（用于下拉框显示）
        var displayValues = dropdownConfig.GetValues();
        
        // 设置下拉框选项（这里需要根据Nova UI的具体API调整）
        // dropdown.SetOptions(displayValues);
        
        Debug.Log("下拉框已初始化，选项数量: " + displayValues.Count);
    }
    
    /// <summary>
    /// 下拉框值变化时的回调
    /// </summary>
    private void OnDropdownValueChanged(string selectedValue)
    {
        // 根据显示值获取对应的键
        string selectedKey = dropdownConfig.GetKey(selectedValue);
        
        Debug.Log($"选择了: 键={selectedKey}, 值={selectedValue}");
        
        // 处理选择逻辑
        HandleSelection(selectedKey, selectedValue);
    }
    
    /// <summary>
    /// 处理选择逻辑
    /// </summary>
    private void HandleSelection(string key, string value)
    {
        // 使用值元组解构
        foreach (var (itemKey, itemValue) in dropdownConfig.GetEnumerable())
        {
            if (itemKey == key)
            {
                Debug.Log($"找到匹配项: ({itemKey}, {itemValue})");
                break;
            }
        }
    }
    
    /// <summary>
    /// 程序化添加选项 - 使用值元组语法
    /// </summary>
    [ContextMenu("添加测试选项")]
    public void AddTestOptions()
    {
        // 使用值元组语法添加选项
        dropdownConfig.AddItem(("option1", "选项1"));
        dropdownConfig.AddItem(("option2", "选项2"));
        dropdownConfig.AddItem(("option3", "选项3"));
        
        // 重新初始化下拉框
        InitializeDropdown();
    }
    
    /// <summary>
    /// 根据键设置下拉框选择
    /// </summary>
    public void SetSelectionByKey(string key)
    {
        string value = dropdownConfig.GetValue(key);
        if (value != null)
        {
            // 设置下拉框选择（需要根据Nova UI的具体API调整）
            // dropdown.SetValue(value);
            Debug.Log($"设置选择: {key} -> {value}");
        }
    }
    
    /// <summary>
    /// 根据值设置下拉框选择
    /// </summary>
    public void SetSelectionByValue(string value)
    {
        string key = dropdownConfig.GetKey(value);
        if (key != null)
        {
            // 设置下拉框选择
            // dropdown.SetValue(value);
            Debug.Log($"设置选择: {key} -> {value}");
        }
    }
    
    /// <summary>
    /// 获取当前选择的键值对
    /// </summary>
    public (string key, string value)? GetCurrentSelection()
    {
        // 这里需要根据Nova UI的API获取当前选择的值
        // string currentValue = dropdown.GetCurrentValue();
        // string currentKey = dropdownConfig.GetKey(currentValue);
        // 
        // if (currentKey != null)
        // {
        //     return (currentKey, currentValue);
        // }
        
        return null;
    }
    
    /// <summary>
    /// 清空并重新设置选项
    /// </summary>
    public void ResetOptions(List<(string key, string value)> newOptions)
    {
        dropdownConfig.下拉框选项.Clear();
        
        foreach (var (key, value) in newOptions)
        {
            dropdownConfig.AddItem((key, value));
        }
        
        InitializeDropdown();
    }
} 