using System;
using System.Collections;
using System.Collections.Generic;
using NovaSamples.UIControls;
using UnityEngine;
using XCharts;
using Cysharp.Threading.Tasks;
using Nova;
using DG.Tweening;
using System.Text;

public class PC首页大数据页面_茉 : 界面基础_韩
{
    [Header("按钮")]
    [SerializeField] Button 返回按钮;
    
    [Header("图表组件")]
    [SerializeField] 用户年龄占比饼图_茉 用户年龄占比饼图;
    [SerializeField] 评估数据折线图_茉 评估数据折线图;
    [SerializeField] 失语症类型条形图_茉 失语症类型条形图;
    [SerializeField] BDAE严重程度条形图_茉 BDAE严重程度条形图;
    [SerializeField] 档案管理数据环形图_茉 档案管理数据环形图;

    [Header("用户数据文本")]
    [SerializeField] TextBlock 用户总数文本;
    [SerializeField] TextBlock 评定人总数文本;
    [SerializeField] TextBlock 男性人数文本;
    [SerializeField] TextBlock 男性占比文本;
    [SerializeField] TextBlock 女性人数文本;
    [SerializeField] TextBlock 女性占比文本;
    [SerializeField] float 数字动画时长 = 0.8f;

    private 用户录 用户录模型;
    private 评人录 评人录模型;
    
    void Start()
    {
        用户录模型 = GetArchitecture().GetModel<用户录>();
        评人录模型 = GetArchitecture().GetModel<评人录>();

        if (返回按钮 != null)
        {
            返回按钮.OnClicked.AddListener(() =>
            {
                 FindObjectOfType<界面管理器_韩>().显示界面("评定人管理添加");
            });
        }
        
        // 延迟刷新数据，等待组件初始化完成
        延迟刷新数据().Forget();
    }
    
    /// <summary>
    /// 刷新所有图表数据
    /// </summary>
    void 刷新所有图表数据()
    {
        Debug.Log("[PC首页大数据页面] 开始刷新图表数据");
        
        if (用户年龄占比饼图 != null)
        {
            Debug.Log("[PC首页大数据页面] 调用饼图刷新数据");
            用户年龄占比饼图.刷新数据();
        }
        else
        {
            Debug.LogWarning("[PC首页大数据页面] 用户年龄占比饼图组件为空！");
        }
        
        if (评估数据折线图 != null)
        {
            Debug.Log("[PC首页大数据页面] 调用折线图刷新数据");
            评估数据折线图.刷新数据();
        }
        else
        {
            Debug.LogWarning("[PC首页大数据页面] 评估数据折线图组件为空！");
        }
        
        if (失语症类型条形图 != null)
        {
            Debug.Log("[PC首页大数据页面] 调用条形图刷新数据");
            失语症类型条形图.刷新数据();
        }
        else
        {
            Debug.LogWarning("[PC首页大数据页面] 失语症类型条形图组件为空！");
        }
        
        if (BDAE严重程度条形图 != null)
        {
            Debug.Log("[PC首页大数据页面] 调用BDAE严重程度条形图刷新数据");
            BDAE严重程度条形图.刷新数据();
        }
        else
        {
            Debug.LogWarning("[PC首页大数据页面] BDAE严重程度条形图组件为空！");
        }
        
        if (档案管理数据环形图 != null)
        {
            Debug.Log("[PC首页大数据页面] 调用档案管理数据环形图刷新数据");
            档案管理数据环形图.刷新数据();
        }
        else
        {
            Debug.LogWarning("[PC首页大数据页面] 档案管理数据环形图组件为空！");
        }

        更新用户数据卡片();
    }
    
    /// <summary>
    /// 延迟刷新数据（等待组件初始化完成）
    /// </summary>
    async UniTaskVoid 延迟刷新数据()
    {
        // 等待一帧，确保所有组件都已初始化
        await UniTask.DelayFrame(1);
        
        // 再次检查组件是否准备好
        if (用户年龄占比饼图 == null && 评估数据折线图 == null && 失语症类型条形图 == null && BDAE严重程度条形图 == null && 档案管理数据环形图 == null)
        {
            Debug.LogWarning("[PC首页大数据页面] 延迟刷新：所有图表组件仍为空");
            return;
        }
        
        Debug.Log("[PC首页大数据页面] 延迟刷新：开始刷新数据");
        刷新所有图表数据();
    }
    
    /// <summary>
    /// 界面显示时刷新数据
    /// </summary>
    protected override void OnShow()
    {
        base.OnShow();
        
        Debug.Log("[PC首页大数据页面] OnShow被调用，准备刷新数据");
        
        // 延迟刷新数据，确保组件已准备好
        延迟刷新数据().Forget();
    }

    void 更新用户数据卡片()
    {
        var 用户列表 = 用户录模型?.GetDataList();
        var 评人列表 = 评人录模型?.GetDataList();

        int 用户总数 = 用户列表?.Count ?? 0;
        int 评定人总数 = 评人列表?.Count ?? 0;

        int 男性总数 = 0;
        int 女性总数 = 0;

        累加性别统计(用户列表, u => u?.性别, ref 男性总数, ref 女性总数);
       // 累加性别统计(评人列表, p => p?.性别, ref 男性总数, ref 女性总数);

        int 总人数 = 男性总数 + 女性总数;
        float 男性占比 = 总人数 > 0 ? (float)男性总数 / 总人数 : 0f;
        float 女性占比 = 总人数 > 0 ? (float)女性总数 / 总人数 : 0f;

        动画更新数值(用户总数文本, 用户总数);
        动画更新数值(评定人总数文本, 评定人总数);
        动画更新数值(男性人数文本, 男性总数);
        动画更新数值(女性人数文本, 女性总数);

        if (男性占比文本 != null)
        {
            男性占比文本.Text = $"占比{格式化百分比(男性占比)}";
        }

        if (女性占比文本 != null)
        {
            女性占比文本.Text = $"占比{格式化百分比(女性占比)}";
        }
    }

    void 累加性别统计<T>(IList<T> 列表, Func<T, string> 获取性别, ref int 男性, ref int 女性)
    {
        if (列表 == null || 获取性别 == null) return;

        foreach (var 项 in 列表)
        {
            var 性别 = 获取性别(项);
            if (是男性(性别))
            {
                男性++;
            }
            else if (是女性(性别))
            {
                女性++;
            }
        }
    }

    bool 是男性(string 性别)
    {
        if (string.IsNullOrWhiteSpace(性别)) return false;
        性别 = 性别.Trim();
        return 性别.Contains("男") && !性别.Contains("女");
    }

    bool 是女性(string 性别)
    {
        if (string.IsNullOrWhiteSpace(性别)) return false;
        性别 = 性别.Trim();
        return 性别.Contains("女");
    }

    void 动画更新数值(TextBlock 文本, int 目标值)
    {
        if (文本 == null) return;

        DOTween.Kill(文本);
        int 初始值 = 解析文本中的数字(文本.Text);
        float 当前值 = 初始值;
        DOTween.To(() => 当前值, v =>
        {
            当前值 = v;
            文本.Text = Mathf.RoundToInt(当前值).ToString();
        }, 目标值, 数字动画时长)
        .SetEase(Ease.OutQuad)
        .SetTarget(文本);
    }

    int 解析文本中的数字(string 原文本)
    {
        if (string.IsNullOrEmpty(原文本)) return 0;
        var 数字字符 = new StringBuilder();
        foreach (var ch in 原文本)
        {
            if (char.IsDigit(ch))
            {
                数字字符.Append(ch);
            }
        }
        if (数字字符.Length == 0) return 0;
        if (int.TryParse(数字字符.ToString(), out var 数值))
        {
            return 数值;
        }
        return 0;
    }

    string 格式化百分比(float 值)
    {
        return $"{Mathf.RoundToInt(值 * 100f)}%";
    }
}
