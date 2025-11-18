using UnityEngine;

/// <summary>
/// 量表运用栏 - 使用栏目组统一管理
/// 包含3个维度：面部动作、上肢动作、复杂动作（选项组性）
/// </summary>
public class 量表运用栏_茉 : 量表评估基类_茉
{
    protected override string 获取记录字段名称() => "运用记录";

    protected override bool 检查是否全部完成()
    {
        if (栏目组性群 == null) return false;
        return 栏目组性群.TrueForAll(组 => 组?.是否全部已选择 ?? false);
    }

    protected override string 获取未完成提示信息() => "请完成全部运用评估后保存";

    protected override void 保存额外数据(object 记录)
    {
        var 运用记录 = 记录 as 运用记录;
        if (运用记录 == null || 栏目组性群 == null) return;

        // 保存各维度总分
        foreach (var 栏目组 in 栏目组性群)
        {
            if (栏目组 == null) continue;
            
            switch (栏目组.维度名称)
            {
                case "面部动作":
                    运用记录.面部动作总分 = 栏目组.当前总分原始值;
                    Debug.Log($"[量表运用栏] 保存面部动作总分: {运用记录.面部动作总分}");
                    break;
                case "上肢动作":
                    运用记录.上肢动作总分 = 栏目组.当前总分原始值;
                    Debug.Log($"[量表运用栏] 保存上肢动作总分: {运用记录.上肢动作总分}");
                    break;
                case "复杂动作":
                    运用记录.复杂动作总分 = 栏目组.当前总分;
                    Debug.Log($"[量表运用栏] 保存复杂动作总分: {栏目组.当前总分}");
                    break;
            }
        }
    }
}
