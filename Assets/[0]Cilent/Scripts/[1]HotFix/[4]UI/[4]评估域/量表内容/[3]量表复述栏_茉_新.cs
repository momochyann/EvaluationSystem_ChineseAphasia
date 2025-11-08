using System.Collections.Generic;
using Nova;
using UnityEngine;

/// <summary>
/// 量表复述栏 - 使用栏目组统一管理
/// 包含2个部分：词复述(选项组性)、句复述(表分择性)
/// </summary>
public class 量表复述栏_茉_新 : 量表评估基类_茉
{
    [SerializeField] private TextBlock 词复述总分显示;
    [SerializeField] private TextBlock 句复述总分显示;
    
    private 栏目组性_茉 词复述栏目组;
    private 栏目组性_茉 句复述栏目组;

    protected override string 获取记录字段名称() => "复述记录";

    protected override bool 检查是否全部完成()
    {
        if (栏目组性群 == null) return false;
        return 栏目组性群.TrueForAll(组 => 组?.是否全部已选择 ?? false);
    }

    protected override string 获取未完成提示信息() => "请完成全部复述评估后保存";

    protected override void OnStartExtra()
    {
        // 查找两个栏目组
        if (栏目组性群 != null)
        {
            词复述栏目组 = 栏目组性群.Find(组 => 组.维度名称 == "词复述");
            句复述栏目组 = 栏目组性群.Find(组 => 组.维度名称 == "句复述");
        }
    }

    protected override void 处理栏目组变化(栏目组性_茉 栏目组)
    {
        // 更新对应的总分显示
        if (栏目组 == 词复述栏目组 && 词复述总分显示 != null)
        {
            词复述总分显示.Text = 栏目组.是否全部已选择 ? 栏目组.当前总分.ToString() : "待选择完";
        }
        else if (栏目组 == 句复述栏目组 && 句复述总分显示 != null)
        {
            句复述总分显示.Text = 栏目组.是否全部已选择 ? 栏目组.当前总分.ToString() : "待选择完";
        }
    }

    protected override void 保存额外数据(object 记录)
    {
        var 复述记录 = 记录 as 复述记录;
        if (复述记录 == null || 栏目组性群 == null) return;

        // 保存两个维度的总分
        foreach (var 栏目组 in 栏目组性群)
        {
            if (栏目组 == null) continue;
            
            switch (栏目组.维度名称)
            {
                case "词复述":
                    复述记录.词复述总分 = 栏目组.当前总分;
                    Debug.Log($"[量表复述栏] 保存词复述总分: {栏目组.当前总分}");
                    break;
                case "句复述":
                    复述记录.句复述总分 = 栏目组.当前总分;
                    Debug.Log($"[量表复述栏] 保存句复述总分: {栏目组.当前总分}");
                    break;
            }
        }
    }

    protected override void 还原其他数据(object 记录)
    {
        var 复述记录 = 记录 as 复述记录;
        if (复述记录 == null) return;

        // 还原总分显示
        if (词复述栏目组 != null && 词复述总分显示 != null)
        {
            词复述总分显示.Text = 词复述栏目组.是否全部已选择 ? 词复述栏目组.当前总分.ToString() : "待选择完";
        }
        
        if (句复述栏目组 != null && 句复述总分显示 != null)
        {
            句复述总分显示.Text = 句复述栏目组.是否全部已选择 ? 句复述栏目组.当前总分.ToString() : "待选择完";
        }
    }
}

