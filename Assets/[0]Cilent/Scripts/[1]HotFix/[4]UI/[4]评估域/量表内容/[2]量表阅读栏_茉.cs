using UnityEngine;

/// <summary>
/// 量表阅读栏 - 使用栏目组统一管理
/// 包含7个维度：视读、阅读指令、字画匹配-朗读、字画匹配-配画、执行指令-朗读、执行指令-配画、填空
/// </summary>
public class 量表阅读栏_茉 : 量表评估基类_茉
{
    protected override string 获取记录字段名称() => "阅读记录";

    protected override bool 检查是否全部完成()
    {
        if (栏目组性群 == null) return false;
        return 栏目组性群.TrueForAll(组 => 组?.是否全部已选择 ?? false);
    }

    protected override string 获取未完成提示信息() => "请完成全部阅读评估后保存";

    protected override void 保存额外数据(object 记录)
    {
        var 阅读记录 = 记录 as 阅读记录;
        if (阅读记录 == null || 栏目组性群 == null) return;

        // 保存各维度总分
        foreach (var 栏目组 in 栏目组性群)
        {
            if (栏目组 == null) continue;
            
            switch (栏目组.维度名称)
            {
                case "视读":
                    阅读记录.视读总分 = 栏目组.当前总分原始值;
                    Debug.Log($"[量表阅读栏] 保存视读总分: {栏目组.当前总分}");
                    break;
                case "阅读指令":
                    阅读记录.阅读指令总分 = 栏目组.当前总分原始值;
                    Debug.Log($"[量表阅读栏] 保存阅读指令总分: {栏目组.当前总分}");
                    break;
                case "字画匹配-朗读":
                case "字画匹配朗读":
                    阅读记录.字画匹配朗读总分 = 栏目组.当前总分原始值;
                    Debug.Log($"[量表阅读栏] 保存字画匹配-朗读总分: {栏目组.当前总分}");
                    break;
                case "字画匹配-配画":
                case "字画匹配配画":
                    阅读记录.字画匹配配画总分 = 栏目组.当前总分原始值;
                    Debug.Log($"[量表阅读栏] 保存字画匹配-配画总分: {栏目组.当前总分}");
                    break;
                case "执行指令-朗读":
                case "执行指令朗读":
                    阅读记录.执行指令朗读总分 = 栏目组.当前总分原始值;
                    Debug.Log($"[量表阅读栏] 保存执行指令-朗读总分: {栏目组.当前总分}");
                    break;
                case "执行指令-配画":
                case "执行指令配画":
                    阅读记录.执行指令配画总分 = 栏目组.当前总分原始值;
                    Debug.Log($"[量表阅读栏] 保存执行指令-配画总分: {栏目组.当前总分}");
                    break;
                case "填空":
                    阅读记录.填空总分 = 栏目组.当前总分原始值;
                    Debug.Log($"[量表阅读栏] 保存填空总分: {栏目组.当前总分}");
                    break;
            }
        }
    }
}
