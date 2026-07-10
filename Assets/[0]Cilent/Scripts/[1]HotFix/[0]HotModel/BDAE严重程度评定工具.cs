using UnityEngine;

/// <summary>
/// BDAE严重程度评定工具类
/// 根据失语症类型和评估分数判断严重程度等级（0-5级）
/// </summary>
public static class BDAE严重程度评定工具
{
    /// <summary>
    /// 获取BDAE严重程度等级描述
    /// </summary>
    /// <param name="等级">0-5级</param>
    /// <returns>等级描述文本</returns>
    public static string 获取严重程度描述(int 等级)
    {
        switch (等级)
        {
            case 0:
                return "0级：无有意义的言语或听觉理解能力。";
            
            case 1:
                return "1级：言语交流中有不连续的言语表达，但大部分需要听者去推测、询问和猜测；可交流的信息范围有限。听者在言语交流中感到困难。";
            
            case 2:
                return "2级：在听者的帮助下，可能进行熟悉话题的交谈。但对陌生话题常常不能表达出自己的思想，使患者与检查者都感到进行言语交流有困难。";
            
            case 3:
                return "3级：在仅需要少量帮助或无帮助下，患者可以讨论几乎所有的日常问题。但由于言语和（或）理解能力的减弱，使某些谈话出现困难或不大可能。";
            
            case 4:
                return "4级：言语流利，但可观察到有理解障碍，但思想和言语表达尚不明显限制。";
            
            case 5:
                return "5级：有极少少的可分辨得出的言语障碍，患者主观上感到有点儿困难，但听者不一定能明显觉察到。";
            
            default:
                return "未评定";
        }
    }

    /// <summary>
    /// 根据失语症类型和评估分数推荐BDAE严重程度等级
    /// </summary>
    /// <param name="档案">档案数据</param>
    /// <param name="失语症类型">失语症类型缩写</param>
    /// <returns>推荐的严重程度等级（0-5）</returns>
    public static int 推荐严重程度等级(档案 档案, string 失语症类型)
    {
        if (档案 == null || string.IsNullOrEmpty(失语症类型))
        {
            Debug.LogWarning("[BDAE严重程度] 档案或失语症类型为空");
            return 0;
        }

        // 检查必要的评估记录
        if (档案.谈话记录 == null || 档案.理解记录 == null)
        {
            Debug.LogWarning("[BDAE严重程度] 缺少必要的评估记录");
            return 0;
        }

        // 计算综合评估分数（用于辅助判断）
        float 谈话流利性 = 档案.谈话记录.谈话流利性总分;
        float 谈话信息量 = 档案.谈话记录.谈话信息量总分;
        float 系列语言 = 档案.谈话记录.系列语言总分;
        float 听辨认 = 档案.理解记录.听辨认图形总分;
        float 口头指令 = 档案.理解记录.口头指令总分;
        
        // 计算理解能力平均分
        float 理解能力平均 = (听辨认 + 口头指令) / 2f;
        
        // 计算表达能力平均分（流利性和信息量）
        float 表达能力平均 = (谈话流利性 + 谈话信息量 + 系列语言) / 3f;

        Debug.Log($"[BDAE严重程度] 失语症类型:{失语症类型}, 流利性:{谈话流利性}, 理解:{理解能力平均:F1}, 表达:{表达能力平均:F1}");

        // 根据失语症类型推荐严重程度
        switch (失语症类型)
        {
            case "GA": // 全面性失语 - 最严重
                return 推荐等级_全面性失语(表达能力平均, 理解能力平均);
            
            case "BA": // 布洛卡失语 - 严重到中度
                return 推荐等级_布洛卡失语(谈话流利性, 理解能力平均);
            
            case "WA": // 韦尼克失语 - 严重到中度
                return 推荐等级_韦尼克失语(谈话流利性, 理解能力平均);
            
            case "TCM": // 经皮质运动性失语 - 中度到轻度
                return 推荐等级_经皮质运动性失语(谈话流利性, 理解能力平均);
            
            case "TCA": // 经皮质感觉性失语 - 中度
            case "TCS": // 经皮质感觉性失语
                return 推荐等级_经皮质感觉性失语(谈话流利性, 理解能力平均);
            
            case "CA": // 传导性失语 - 中度到轻度
                return 推荐等级_传导性失语(谈话流利性, 理解能力平均);
            
            case "AA": // 异常性失语 - 轻度到极轻度
                return 推荐等级_异常性失语(表达能力平均, 理解能力平均);
            
            case "MT": // 混合性经皮质失语 - 严重
                return 推荐等级_混合性经皮质失语(表达能力平均, 理解能力平均);
            
            case "TA或BaA": // 完全性失语/混合性经皮质失语 - 严重
                return 推荐等级_完全性失语(表达能力平均, 理解能力平均);
            
            default:
                // 未知类型，根据综合分数判断
                return 推荐等级_综合评估(表达能力平均, 理解能力平均);
        }
    }

    // 各类型失语症的等级推荐方法

    private static int 推荐等级_全面性失语(float 表达, float 理解)
    {
        // 全面性失语通常是0-2级
        if (表达 < 20 && 理解 < 20) return 0;
        if (表达 < 40 && 理解 < 40) return 1;
        return 2;
    }

    private static int 推荐等级_布洛卡失语(float 流利性, float 理解)
    {
        // 布洛卡失语：表达严重受损，理解相对较好
        // 通常是1-3级
        if (流利性 < 30 && 理解 < 50) return 1;
        if (流利性 < 50 && 理解 >= 60) return 2;
        if (流利性 < 67 && 理解 >= 70) return 3;
        return 2;
    }

    private static int 推荐等级_韦尼克失语(float 流利性, float 理解)
    {
        // 韦尼克失语：理解严重受损，流利但无意义
        // 通常是1-3级
        if (理解 < 40) return 1;
        if (理解 < 60) return 2;
        if (理解 < 80) return 3;
        return 3;
    }

    private static int 推荐等级_经皮质运动性失语(float 流利性, float 理解)
    {
        // 经皮质运动性失语：理解好，表达受损但复述好
        // 通常是2-4级
        if (流利性 < 40) return 2;
        if (流利性 < 60 && 理解 >= 70) return 3;
        if (理解 >= 80) return 4;
        return 3;
    }

    private static int 推荐等级_经皮质感觉性失语(float 流利性, float 理解)
    {
        // 经皮质感觉性失语：流利但理解差，复述相对好
        // 通常是2-3级
        if (理解 < 50) return 2;
        if (理解 < 70) return 3;
        return 3;
    }

    private static int 推荐等级_传导性失语(float 流利性, float 理解)
    {
        // 传导性失语：流利且理解好，但复述差
        // 通常是3-4级
        if (流利性 >= 80 && 理解 >= 90) return 4;
        if (流利性 >= 70 && 理解 >= 80) return 3;
        return 3;
    }

    private static int 推荐等级_异常性失语(float 表达, float 理解)
    {
        // 异常性失语：最轻的失语症，主要是找词困难
        // 通常是4-5级
        if (表达 >= 85 && 理解 >= 90) return 5;
        if (表达 >= 75 && 理解 >= 85) return 4;
        return 4;
    }

    private static int 推荐等级_混合性经皮质失语(float 表达, float 理解)
    {
        // 混合性经皮质失语：表达和理解都差，但复述相对好
        // 通常是1-2级
        if (表达 < 30 && 理解 < 40) return 1;
        return 2;
    }

    private static int 推荐等级_完全性失语(float 表达, float 理解)
    {
        // 完全性失语：严重损害
        // 通常是0-2级
        if (表达 < 25 && 理解 < 35) return 1;
        return 2;
    }

    private static int 推荐等级_综合评估(float 表达, float 理解)
    {
        // 综合评估：根据表达和理解能力的平均分判断
        float 综合分 = (表达 + 理解) / 2f;
        
        if (综合分 < 20) return 0;
        if (综合分 < 40) return 1;
        if (综合分 < 60) return 2;
        if (综合分 < 75) return 3;
        if (综合分 < 85) return 4;
        return 5;
    }

    /// <summary>
    /// 获取等级选项列表（用于下拉框）
    /// </summary>
    /// <returns>等级选项数组</returns>
    public static string[] 获取等级选项列表()
    {
        return new string[]
        {
            "0级",
            "1级",
            "2级",
            "3级",
            "4级",
            "5级"
        };
    }
}


