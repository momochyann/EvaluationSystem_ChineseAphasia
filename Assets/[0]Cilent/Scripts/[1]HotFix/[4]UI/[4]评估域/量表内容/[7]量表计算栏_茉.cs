using UnityEngine;

/// <summary>
/// 量表计算栏 - 使用栏目组统一管理
/// 包含1个维度：运算（选项组性）
/// </summary>
public class 量表计算栏_茉 : 量表评估基类_茉
{
    protected override string 获取记录字段名称() => "计算记录";

    protected override bool 检查是否全部完成()
    {
        if (栏目组性群 == null) return false;
        return 栏目组性群.TrueForAll(组 => 组?.是否全部已选择 ?? false);
    }

    protected override string 获取未完成提示信息() => "请完成全部计算评估后保存";

    protected override void 保存额外数据(object 记录)
    {
        var 计算记录 = 记录 as 计算记录;
        if (计算记录 == null || 栏目组性群 == null) return;

        // 保存运算维度总分
        foreach (var 栏目组 in 栏目组性群)
        {
            if (栏目组 == null) continue;
            
            if (栏目组.维度名称 == "运算")
            {
                计算记录.运算总分 = 栏目组.当前总分;
                Debug.Log($"[量表计算栏] 保存运算总分: {栏目组.当前总分}");
            }
        }
    }
}
