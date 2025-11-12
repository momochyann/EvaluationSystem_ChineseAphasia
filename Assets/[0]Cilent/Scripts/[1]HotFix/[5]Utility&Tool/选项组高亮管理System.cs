using System.Collections.Generic;
using UnityEngine;
using QFramework;

/// <summary>
/// 选项组高亮管理系统
/// 负责管理所有选项组的高亮状态，实现跨组通信
/// </summary>
public class 选项组高亮管理System : AbstractSystem
{
    /// <summary>
    /// 所有已注册的选项组列表
    /// </summary>
    private List<选项组性_茉> 已注册选项组 = new List<选项组性_茉>();

    /// <summary>
    /// 当前高亮的选项组
    /// </summary>
    private 选项组性_茉 当前高亮组;

    /// <summary>
    /// 注册选项组
    /// </summary>
    /// <param name="选项组">要注册的选项组实例</param>
    public void 注册选项组(选项组性_茉 选项组)
    {
        if (选项组 == null) return;
        
        if (!已注册选项组.Contains(选项组))
        {
            已注册选项组.Add(选项组);
          //  Debug.Log($"[选项组高亮管理] 注册选项组: {选项组.选项组名称}，当前总数: {已注册选项组.Count}");
        }
    }

    /// <summary>
    /// 注销选项组
    /// </summary>
    /// <param name="选项组">要注销的选项组实例</param>
    public void 注销选项组(选项组性_茉 选项组)
    {
        if (选项组 == null) return;
        
        if (已注册选项组.Remove(选项组))
        {
           // Debug.Log($"[选项组高亮管理] 注销选项组: {选项组.选项组名称}，当前总数: {已注册选项组.Count}");
            
            // 如果注销的是当前高亮组，清空当前高亮组引用
            if (当前高亮组 == 选项组)
            {
                当前高亮组 = null;
            }
        }
    }

    /// <summary>
    /// 设置高亮选项组
    /// 会自动取消其他所有选项组的高亮
    /// </summary>
    /// <param name="目标选项组">要高亮的选项组</param>
    public void 设置高亮组(选项组性_茉 目标选项组)
    {
        if (目标选项组 == null) return;
        
        // 如果已经是当前高亮组，不需要重复处理
        if (当前高亮组 == 目标选项组) return;

    //   Debug.Log($"[选项组高亮管理] 设置高亮组: {目标选项组.选项组名称}");

        // 取消之前的高亮组
        if (当前高亮组 != null && 当前高亮组 != 目标选项组)
        {
            当前高亮组.取消高亮();
        }

        // 取消其他所有选项组的高亮
        foreach (var 其他选项组 in 已注册选项组)
        {
            if (其他选项组 != 目标选项组 && 其他选项组 != 当前高亮组)
            {
                其他选项组.取消高亮();
            }
        }

        // 更新当前高亮组
        当前高亮组 = 目标选项组;
    }

    /// <summary>
    /// 获取当前高亮的选项组
    /// </summary>
    public 选项组性_茉 获取当前高亮组()
    {
        return 当前高亮组;
    }

    /// <summary>
    /// 取消所有高亮
    /// </summary>
    public void 取消所有高亮()
    {
      //  Debug.Log("[选项组高亮管理] 取消所有高亮");
        
        foreach (var 选项组 in 已注册选项组)
        {
            选项组.取消高亮();
        }
        
        当前高亮组 = null;
    }

    /// <summary>
    /// 获取已注册选项组数量
    /// </summary>
    public int 获取注册数量()
    {
        return 已注册选项组.Count;
    }

    protected override void OnInit()
    {
        // System 初始化逻辑（如果需要）
    }
}

