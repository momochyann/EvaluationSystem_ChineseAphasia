using UnityEngine;

/// <summary>
/// 量表复述栏 - 使用栏目组统一管理（选项组性 + 表分择性）
/// </summary>
public class 新_量表复述栏_茉 : 量表评估基类_茉
{
    protected override string 获取记录字段名称() => "复述记录";

    protected override bool 检查是否全部完成()
    {
        if (栏目组性群 == null) return false;
        return 栏目组性群.TrueForAll(组 => 组?.是否全部已选择 ?? false);
    }

    protected override string 获取未完成提示信息() => "请完成全部复述评估后保存";

    protected override void 保存额外数据(object 记录)
    {
        var 复述记录 = 记录 as 复述记录;
        if (复述记录 == null || 栏目组性群 == null) return;

        // 保存各维度总分
        foreach (var 栏目组 in 栏目组性群)
        {
            if (栏目组 == null) continue;
            
            switch (栏目组.维度名称)
            {
                case "词复述":
                    复述记录.词复述总分 = 栏目组.当前总分原始值;
                    break;
                case "句复述":
                    复述记录.句复述总分 = 栏目组.当前总分原始值;
                    break;
            }
        }
    }
}




