using NovaSamples.UIControls;
using UnityEngine;

/// <summary>
/// 量表谈话栏 - 使用栏目组统一管理
/// 包含3个维度：谈话信息量、谈话流利性、系列语言（选项组性）
/// </summary>
public class 新_量表谈话栏_茉 : 量表评估基类_茉
{
    [SerializeField] TextField 系列语言;
    protected override string 获取记录字段名称() => "谈话记录";

    protected override bool 检查是否全部完成()
    {
        // if (string.IsNullOrEmpty(系列语言?.Text)) return false;
        if (栏目组性群 == null) return false;
        return 栏目组性群.TrueForAll(组 => 组?.是否全部已选择 ?? false);
    }

    protected override string 获取未完成提示信息() => "请完成全部谈话评估后保存";
    protected override void 还原其他数据(object 记录)
    {
        Debug.Log($"[量表谈话栏] 还原其他数据: {记录}");
        var 谈话记录 = 记录 as 谈话记录;
        if (谈话记录 != null && 系列语言 != null)
        {
            if(谈话记录.系列语言总分 <= 0)
            {
                系列语言.Text = "";
            }
            else
            系列语言.Text = 谈话记录.系列语言总分.ToString();

        }
    }
    protected override void 保存额外数据(object 记录)
    {
        var 谈话记录 = 记录 as 谈话记录;
        if (谈话记录 == null || 栏目组性群 == null) return;
        if (系列语言 != null)
        {
            系列语言.Text = float.TryParse(系列语言?.Text ?? "0", out float 系列语言总分) ? 系列语言总分.ToString() : "0";
            谈话记录.系列语言总分 = Mathf.Clamp(系列语言总分, 0, 21);
            Debug.Log($"[量表谈话栏] 保存系列语言总分: {谈话记录.系列语言总分}");
        }
        // 保存各维度总分
        foreach (var 栏目组 in 栏目组性群)
        {
            if (栏目组 == null) continue;

            switch (栏目组.维度名称)
            {
                case "谈话信息量":
                    谈话记录.谈话信息量总分 = 栏目组.当前总分原始值;
                    Debug.Log($"[量表谈话栏] 保存谈话信息量总分: {栏目组.当前总分}");
                    break;
                case "谈话流利性":
                    谈话记录.谈话流利性总分 = 栏目组.当前总分原始值;
                    Debug.Log($"[量表谈话栏] 保存谈话流利性总分: {栏目组.当前总分}");
                    break;
                case "系列语言":
                    谈话记录.系列语言总分 = 栏目组.当前总分原始值;
                    Debug.Log($"[量表谈话栏] 保存系列语言总分: {栏目组.当前总分}");
                    break;
            }
        }
    }
}




