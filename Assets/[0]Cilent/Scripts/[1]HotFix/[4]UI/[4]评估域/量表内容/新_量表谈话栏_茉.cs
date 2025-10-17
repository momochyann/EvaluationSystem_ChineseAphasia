using UnityEngine;

/// <summary>
/// 量表谈话栏 - 使用栏目组统一管理（选项组性）
/// </summary>
public class 新_量表谈话栏_茉 : 量表评估基类_茉
{
    protected override string 获取记录字段名称() => "谈话记录";

    protected override bool 检查是否全部完成()
    {
        if (栏目组性群 == null) return false;
        return 栏目组性群.TrueForAll(组 => 组?.是否全部已选择 ?? false);
    }

    protected override string 获取未完成提示信息() => "请完成全部选项组后保存";
}




