using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;
using System.Linq;
using System;

public class 选项组性_茉 : MonoBehaviour
{
    [SerializeField] public string 选项组名称;
    // 选项组列表
    List<Toggle> 选项组;
    
    // 当前选中的选项（公开属性）
    [HideInInspector] public Toggle 当前选中选项;
    
    // 当前选中选项的索引（公开属性）
    public int 当前选中索引 => 当前选中选项 != null ? 选项组.IndexOf(当前选中选项) : -1;
    
    // 当前选中选项的名称（公开属性）
    public string 当前选中名称 => 当前选中选项 != null ? 当前选中选项.name : "";
    
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
    /// 处理选项切换逻辑（单选模式）
    /// </summary>
    /// <param name="当前选项">被点击的选项</param>
    /// <param name="是否选中">是否被选中</param>
    private void 处理选项切换(Toggle 当前选项, bool 是否选中)
    {
        if (是否选中)
        {
            // 如果选中了这个选项，取消其他所有选项的选中状态
            foreach (var 其他选项 in 选项组)
            {
                if (其他选项 != 当前选项 && 其他选项.ToggledOn)
                {
                    其他选项.ToggledOn = false;
                }
            }
            
            // 设置当前选中的选项
            当前选中选项 = 当前选项;
            Debug.Log("选项 " + 当前选项.name + " 被选中");
        }
        else
        {
            // 如果取消选中，清空当前选中选项
            if (当前选中选项 == 当前选项)
            {
                当前选中选项 = null;
            }
            Debug.Log("选项 " + 当前选项.name + " 被取消选中");
        }
        
        // 触发选择变化事件
        选择变化事件?.Invoke();
    }
    
    /// <summary>
    /// 程序化设置选中的选项
    /// </summary>
    /// <param name="索引">要选中的选项索引</param>
    public void 设置选中选项(int 索引)
    {
        if (索引 >= 0 && 索引 < 选项组.Count)
        {
            选项组[索引].ToggledOn = true;
        }
    }
    
    /// <summary>
    /// 程序化设置选中的选项
    /// </summary>
    /// <param name="选项名称">要选中的选项名称</param>
    public void 设置选中选项(string 选项名称)
    {
        var 目标选项 = 选项组.FirstOrDefault(x => x.name == 选项名称);
        if (目标选项 != null)
        {
            目标选项.ToggledOn = true;
        }
    }
    
    /// <summary>
    /// 清除所有选项的选中状态
    /// </summary>
    public void 清除所有选择()
    {
        foreach (var 选项 in 选项组)
        {
            选项.ToggledOn = false;
        }
        当前选中选项 = null;
    }
}
