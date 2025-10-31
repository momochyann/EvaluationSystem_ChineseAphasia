using UnityEngine;

/// <summary>
/// 量表理解栏 - 使用栏目组统一管理
/// 包含4个维度：是否题、听辨认实物、听辨认图形、口头指令（选项组性）
/// </summary>
public class 量表理解栏_茉 : 量表评估基类_茉
{
    protected override string 获取记录字段名称() => "理解记录";

    protected override bool 检查是否全部完成()
    {
        if (栏目组性群 == null) return false;
        return 栏目组性群.TrueForAll(组 => 组?.是否全部已选择 ?? false);
    }

    protected override string 获取未完成提示信息() => "请完成全部理解评估后保存";

    protected override void 保存额外数据(object 记录)
    {
        var 理解记录 = 记录 as 理解记录;
        if (理解记录 == null || 栏目组性群 == null) return;

        // 保存各维度总分
        foreach (var 栏目组 in 栏目组性群)
        {
            if (栏目组 == null) continue;
            
            switch (栏目组.维度名称)
            {
                case "是否题":
                    理解记录.是否题总分 = 栏目组.当前总分;
                    Debug.Log($"[量表理解栏] 保存是否题总分: {栏目组.当前总分}");
                    break;
                case "听辨认实物":
                    理解记录.听辨认实物总分 = 栏目组.当前总分;
                    Debug.Log($"[量表理解栏] 保存听辨认实物总分: {栏目组.当前总分}");
                    break;
                case "听辨认图形":
                    理解记录.听辨认图形总分 = 栏目组.当前总分;
                    Debug.Log($"[量表理解栏] 保存听辨认图形总分: {栏目组.当前总分}");
                    break;
                case "口头指令":
                    理解记录.口头指令总分 = 栏目组.当前总分;
                    Debug.Log($"[量表理解栏] 保存口头指令总分: {栏目组.当前总分}");
                    break;
            }
        }
    }
}
