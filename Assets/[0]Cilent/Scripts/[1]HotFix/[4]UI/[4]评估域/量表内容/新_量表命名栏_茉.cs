using NovaSamples.UIControls;
using UnityEngine;

/// <summary>
/// 量表命名栏 - 使用栏目组统一管理（选项组性 + 输入框）
/// </summary>
public class 新_量表命名栏_茉 : 量表评估基类_茉
{
    [SerializeField] TextField 列名输入;

    protected override string 获取记录字段名称() => "命名记录";

    protected override bool 检查是否全部完成()
    {
        if (string.IsNullOrEmpty(列名输入?.Text)) return false;
        if (栏目组性群 == null) return false;
        return 栏目组性群.TrueForAll(组 => 组?.是否全部已选择 ?? false);
    }

    protected override string 获取未完成提示信息() => "请完成全部命名评估后保存";

    protected override void 还原其他数据(object 记录)
    {
        var 命名记录 = 记录 as 命名记录;
        if (命名记录 != null && 列名输入 != null)
        {
            列名输入.Text = 命名记录.列名 ?? "";
        }
    }

    protected override void 清除所有选择()
    {
        base.清除所有选择();
        if (列名输入 != null) 列名输入.Text = "";
    }

    protected override void 保存额外数据(object 记录)
    {
        var 命名记录 = 记录 as 命名记录;
        if (命名记录 == null) return;

        // 保存列名
        命名记录.列名 = 列名输入?.Text ?? "";

        // 保存各维度总分
        if (栏目组性群 != null)
        {
            foreach (var 栏目组 in 栏目组性群)
            {
                if (栏目组 == null) continue;
                
                switch (栏目组.维度名称)
                {
                    case "词命名": 命名记录.词命名总分 = 栏目组.当前总分; break;
                    case "颜色识别": 命名记录.颜色识别总分 = 栏目组.当前总分; break;
                    case "颜色命名": 命名记录.颜色命名总分 = 栏目组.当前总分; break;
                    case "反应性命名": 命名记录.反应性命名总分 = 栏目组.当前总分; break;
                }
            }
        }
    }
}




