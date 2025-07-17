using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DropdownItem
{
    [SerializeField] public string key;
    [SerializeField] public string value;
    
    public DropdownItem(string key, string value)
    {
        this.key = key;
        this.value = value;
    }
    
    // 隐式转换为值元组
    public static implicit operator (string key, string value)(DropdownItem item)
    {
        return (item.key, item.value);
    }
    
    // 从值元组隐式转换
    public static implicit operator DropdownItem((string key, string value) tuple)
    {
        return new DropdownItem(tuple.key, tuple.value);
    }
    
    public void Deconstruct(out string key, out string value)
    {
        key = this.key;
        value = this.value;
    }
}

[Serializable]
public class DropdownConfig
{
    [SerializeField] 
    [Tooltip("下拉框的键值对配置")]
    public List<DropdownItem> 下拉框选项 = new List<DropdownItem>();
    
    /// <summary>
    /// 转换为Dictionary
    /// </summary>
    public Dictionary<string, string> ToDictionary()
    {
        var dict = new Dictionary<string, string>();
        foreach (var item in 下拉框选项)
        {
            if (!string.IsNullOrEmpty(item.key) && !dict.ContainsKey(item.key))
            {
                dict[item.key] = item.value;
            }
        }
        return dict;
    }
    
    /// <summary>
    /// 从Dictionary设置值
    /// </summary>
    public void FromDictionary(Dictionary<string, string> dictionary)
    {
        下拉框选项.Clear();
        foreach (var kvp in dictionary)
        {
            下拉框选项.Add(new DropdownItem(kvp.Key, kvp.Value));
        }
    }
    
    /// <summary>
    /// 获取所有键
    /// </summary>
    public List<string> GetKeys()
    {
        var keys = new List<string>();
        foreach (var item in 下拉框选项)
        {
            if (!string.IsNullOrEmpty(item.key))
            {
                keys.Add(item.key);
            }
        }
        return keys;
    }
    
    /// <summary>
    /// 获取所有值
    /// </summary>
    public List<string> GetValues()
    {
        var values = new List<string>();
        foreach (var item in 下拉框选项)
        {
            values.Add(item.value ?? "");
        }
        return values;
    }
    
    /// <summary>
    /// 根据键获取值
    /// </summary>
    public string GetValue(string key)
    {
        foreach (var item in 下拉框选项)
        {
            if (item.key == key)
            {
                return item.value;
            }
        }
        return null;
    }
    
    /// <summary>
    /// 根据值获取键
    /// </summary>
    public string GetKey(string value)
    {
        foreach (var item in 下拉框选项)
        {
            if (item.value == value)
            {
                return item.key;
            }
        }
        return null;
    }
    
    /// <summary>
    /// 添加新的键值对
    /// </summary>
    public void AddItem(string key, string value)
    {
        下拉框选项.Add(new DropdownItem(key, value));
    }
    
    /// <summary>
    /// 使用值元组语法
    /// </summary>
    public void AddItem((string key, string value) item)
    {
        下拉框选项.Add(item);
    }
    
    /// <summary>
    /// 支持foreach遍历，返回值元组
    /// </summary>
    public IEnumerable<(string key, string value)> GetEnumerable()
    {
        foreach (var item in 下拉框选项)
        {
            yield return (item.key, item.value);
        }
    }
} 