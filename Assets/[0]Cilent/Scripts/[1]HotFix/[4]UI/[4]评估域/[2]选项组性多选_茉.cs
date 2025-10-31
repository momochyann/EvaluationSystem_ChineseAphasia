using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;
using System.Linq;
using System;

public class 选项组性多选_茉 : MonoBehaviour
{
    [SerializeField] public string 选项组名称;
    // 选项组列表
    List<Toggle> 选项组;
    
    // 当前选中的选项列表（公开属性）
    [HideInInspector] public List<Toggle> 当前选中选项列表 = new List<Toggle>();
    
    // 获取当前选中选项的索引列表（公开属性）
    public List<int> 当前选中索引列表
    {
        get
        {
            List<int> 索引列表 = new List<int>();
            foreach (var 选项 in 当前选中选项列表)
            {
                索引列表.Add(选项组.IndexOf(选项));
            }
            return 索引列表;
        }
    }
    
    // 获取当前选中选项的名称列表（公开属性）
    public List<string> 当前选中名称列表
    {
        get
        {
            List<string> 名称列表 = new List<string>();
            foreach (var 选项 in 当前选中选项列表)
            {
                名称列表.Add(选项.name);
            }
            return 名称列表;
        }
    }
    
    // 获取选中选项的数量（公开属性）
    public int 选中数量 => 当前选中选项列表.Count;
    
    // 选择变化事件
    public static event Action 选择变化事件;
    
    void Start()
    {
        选项组 = GetComponentsInChildren<Toggle>().ToList();
        
        // 为每个选项添加事件监听
        foreach (var 选项 in 选项组)
        {
            选项.OnToggled.AddListener((value) => 处理选项切换(选项, value));
        }
    }
    
    /// <summary>
    /// 处理选项切换逻辑（多选模式）
    /// </summary>
    /// <param name="当前选项">被点击的选项</param>
    /// <param name="是否选中">是否被选中</param>
    private void 处理选项切换(Toggle 当前选项, bool 是否选中)
    {
        if (是否选中)
        {
            // 如果选中了这个选项，添加到选中列表
            if (!当前选中选项列表.Contains(当前选项))
            {
                当前选中选项列表.Add(当前选项);
                Debug.Log("选项 " + 当前选项.name + " 被选中");
            }
        }
        else
        {
            // 如果取消选中，从选中列表移除
            if (当前选中选项列表.Contains(当前选项))
            {
                当前选中选项列表.Remove(当前选项);
                Debug.Log("选项 " + 当前选项.name + " 被取消选中");
            }
        }
        
        // 触发选择变化事件
        选择变化事件?.Invoke();
        
        // 打印当前选中的所有选项
        Debug.Log($"当前选中数量: {选中数量}, 选项: {string.Join(", ", 当前选中名称列表)}");
    }
    
    /// <summary>
    /// 程序化设置选中的选项（单个，通过索引）
    /// </summary>
    /// <param name="索引">要选中的选项索引</param>
    /// <param name="是否选中">true为选中，false为取消选中</param>
    public void 设置选中选项(int 索引, bool 是否选中 = true)
    {
        if (索引 >= 0 && 索引 < 选项组.Count)
        {
            选项组[索引].ToggledOn = 是否选中;
        }
    }
    
    /// <summary>
    /// 程序化设置选中的选项（单个，通过名称）
    /// </summary>
    /// <param name="选项名称">要选中的选项名称</param>
    /// <param name="是否选中">true为选中，false为取消选中</param>
    public void 设置选中选项(string 选项名称, bool 是否选中 = true)
    {
        var 目标选项 = 选项组.FirstOrDefault(x => x.name == 选项名称);
        if (目标选项 != null)
        {
            目标选项.ToggledOn = 是否选中;
        }
    }
    
    /// <summary>
    /// 程序化设置选中的选项（批量，通过索引列表）
    /// </summary>
    /// <param name="索引列表">要选中的选项索引列表</param>
    public void 批量设置选中选项(List<int> 索引列表)
    {
        清除所有选择();
        foreach (var 索引 in 索引列表)
        {
            设置选中选项(索引, true);
        }
    }
    
    /// <summary>
    /// 程序化设置选中的选项（批量，通过名称列表）
    /// </summary>
    /// <param name="名称列表">要选中的选项名称列表</param>
    public void 批量设置选中选项(List<string> 名称列表)
    {
        清除所有选择();
        foreach (var 名称 in 名称列表)
        {
            设置选中选项(名称, true);
        }
    }
    
    /// <summary>
    /// 清除所有选项的选中状态
    /// </summary>
    public void 清除所有选择()
    {
        if (选项组 == null) return;
        foreach (var 选项 in 选项组)
        {
            选项.ToggledOn = false;
        }
        当前选中选项列表.Clear();
    }
    
    /// <summary>
    /// 全选所有选项
    /// </summary>
    public void 全选()
    {
        foreach (var 选项 in 选项组)
        {
            选项.ToggledOn = true;
        }
    }
    
    /// <summary>
    /// 反选（已选变未选，未选变已选）
    /// </summary>
    public void 反选()
    {
        foreach (var 选项 in 选项组)
        {
            选项.ToggledOn = !选项.ToggledOn;
        }
    }
    
    /// <summary>
    /// 判断某个选项是否被选中（通过索引）
    /// </summary>
    /// <param name="索引">选项索引</param>
    /// <returns>是否被选中</returns>
    public bool 是否选中(int 索引)
    {
        if (索引 >= 0 && 索引 < 选项组.Count)
        {
            return 当前选中选项列表.Contains(选项组[索引]);
        }
        return false;
    }
    
    /// <summary>
    /// 判断某个选项是否被选中（通过名称）
    /// </summary>
    /// <param name="选项名称">选项名称</param>
    /// <returns>是否被选中</returns>
    public bool 是否选中(string 选项名称)
    {
        var 目标选项 = 选项组.FirstOrDefault(x => x.name == 选项名称);
        if (目标选项 != null)
        {
            return 当前选中选项列表.Contains(目标选项);
        }
        return false;
    }
}

