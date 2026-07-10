using UnityEngine;

/// <summary>
/// 量表结构栏 - 使用栏目组统一管理
/// 包含2个维度：照画图（选项组性）、摆方块（选项组性）
/// </summary>
public class 量表结构栏_茉 : 量表评估基类_茉
{
    protected override string 获取记录字段名称() => "结构与空间记录";

    protected override bool 检查是否全部完成()
    {
        if (栏目组性群 == null) return false;
        return 栏目组性群.TrueForAll(组 => 组?.是否全部已选择 ?? false);
    }

    protected override string 获取未完成提示信息() => "请完成全部结构与空间评估后保存";

    protected override void 保存额外数据(object 记录)
    {
        var 结构与空间记录 = 记录 as 结构与空间记录;
        if (结构与空间记录 == null || 栏目组性群 == null) return;

        // 保存各维度总分
        foreach (var 栏目组 in 栏目组性群)
        {
            if (栏目组 == null) continue;
            
            switch (栏目组.维度名称)
            {
                case "照画图":
                    结构与空间记录.照画图总分 = 栏目组.当前总分原始值;
                    Debug.Log($"[量表结构栏] 保存照画图总分: {栏目组.当前总分}");
                    break;
                case "摆方块":
                    结构与空间记录.摆方块总分 = 栏目组.当前总分原始值;
                    Debug.Log($"[量表结构栏] 保存摆方块总分: {栏目组.当前总分}");
                    break;
            }
        }
    }
}
