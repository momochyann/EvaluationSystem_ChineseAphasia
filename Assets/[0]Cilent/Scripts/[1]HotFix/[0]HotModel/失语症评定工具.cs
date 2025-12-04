using UnityEngine;

/// <summary>
/// 失语症评定工具类
/// 根据BDAE评估标准，通过决策树判断失语症类型
/// </summary>
public static class 失语症评定工具
{
    /// <summary>
    /// 根据档案数据计算失语症类型
    /// </summary>
    /// <param name="档案">包含各项评估数据的档案</param>
    /// <returns>失语症类型字符串</returns>
    /// static int 谈话流利性满分 = 54;
    static int 听辨认满分 = 20;
    static int 复述满分 = 100;
    static int 词命名满分 = 40;
    static int 谈话流利性满分 = 54;
    public static string 计算失语症类型(档案 档案)
    {

        if (档案 == null)
        {
            Debug.LogWarning("[失语症评定] 档案为空");
            return "未评定";
        }

        // 检查必要的评估记录是否存在
        if (档案.谈话记录 == null || 档案.理解记录 == null ||
            档案.复述记录 == null || 档案.命名记录 == null)
        {
            Debug.LogWarning("[失语症评定] 缺少必要的评估记录");
            return "评估数据不完整";
        }

        // 提取关键分数
        int 谈话流利性 = 档案.谈话记录.谈话流利性总分;
        int 听辨认 = 档案.理解记录.听辨认图形总分; // 使用听辨认图形总分
        int 词复述 = 档案.复述记录.词复述总分;
        int 句复述 = 档案.复述记录.句复述总分;
        int 词命名 = 档案.命名记录.词命名总分;

        Debug.Log($"[失语症评定] 谈话流利性:{谈话流利性}, 听辨认:{听辨认}, 词复述:{词复述}, 句复述:{句复述}, 词命名:{词命名}");

        // 按照决策树进行判断
        return 执行决策树判断(谈话流利性, 听辨认, 词复述, 句复述, 词命名);
    }

    /// <summary>
    /// 执行决策树判断
    /// </summary>
    private static string 执行决策树判断(int 谈话流利性, int 听辨认, int 词复述, int 句复述, int 词命名)
    {
        // 第一级：谈话流利性判断
        if (谈话流利性 / 谈话流利性满分 * 100f >= 67)
        {
            // 流畅组（≥67）：WA、TCS、AA、CA
            return 判断流畅组(听辨认, 词复述, 句复述);
        }
        else
        {
            // 不流畅组（<67）：BA、TCM、MT、GA、TA、BaA
            return 判断不流畅组(听辨认, 词复述, 句复述, 词命名);
        }
    }

    /// <summary>
    /// 判断流畅组失语症类型
    /// </summary>
    private static string 判断流畅组(int 听辨认, int 词复述, int 句复述)
    {
        // 第二级：听辨认判断
        if (听辨认 / 听辨认满分 * 100f >= 80)
        {
            // 听辨认较好（≥80）：AA、CA
            return 判断流畅组_听辨认好(词复述, 句复述);
        }
        else
        {
            // 听辨认差（<80）：WA、TCS
            return 判断流畅组_听辨认差(词复述, 句复述);
        }
    }

    /// <summary>
    /// 判断流畅组-听辨认好的类型
    /// </summary>
    private static string 判断流畅组_听辨认好(int 词复述, int 句复述)
    {
        // 第三级：句复述判断
        if ((句复述+词复述) / 复述满分 * 100f >= 86)
        {
            return "AA"; // 异常性失语(Anomic Aphasia)
        }
        else
        {
            return "CA"; // 传导性失语(Conduction Aphasia)
        }
    }

    /// <summary>
    /// 判断流畅组-听辨认差的类型
    /// </summary>
    private static string 判断流畅组_听辨认差(int 词复述, int 句复述)
    {
        // 第三级：句复述判断
        if ((句复述+词复述) / 复述满分 * 100f >= 75)
        {
            return "TCA"; // 经皮质感觉性失语(Transcortical Sensory Aphasia)
        }
        else
        {
            return "WA"; // 韦尼克失语(Wernicke's Aphasia)
        }
    }

    /// <summary>
    /// 判断不流畅组失语症类型
    /// </summary>
    private static string 判断不流畅组(int 听辨认, int 词复述, int 句复述, int 词命名)
    {
        // 第二级：听辨认判断
        if (听辨认 / 听辨认满分 * 100f >= 55)
        {
            // 听辨认好的（≥55）
            return 判断不流畅组_听辨认好(词复述, 句复述, 词命名);
        }
        else
        {
            // 听辨认差的（<55）
            return 判断不流畅组_听辨认差(词复述, 句复述);
        }
    }

    /// <summary>
    /// 判断不流畅组-听辨认好的类型
    /// </summary>
    private static string 判断不流畅组_听辨认好(int 词复述, int 句复述, int 词命名)
    {
        // 第三级：句复述判断
        if ((句复述+词复述) / 复述满分 * 100f >= 72)
        {
            // 句复述好（≥72）：TCM、TA、BaA
            return 判断不流畅组_复述好(词命名);
        }
        else
        {
            // 句复述差的（<72）
            return "BA"; // 布洛卡失语(Broca's Aphasia)
        }
    }

    /// <summary>
    /// 判断不流畅组-听辨认好-复述好的类型
    /// </summary>
    private static string 判断不流畅组_复述好(int 词命名)
    {
        // 第四级：词命名判断
        if (词命名 / 词命名满分 * 100f >= 80)
        {
            return "TCM"; // 经皮质运动性失语(Transcortical Motor Aphasia)
        }
        else
        {
            return "TA或BaA"; // 完全性失语(Total Aphasia)或混合性经皮质失语(Mixed Transcortical Aphasia)
        }
    }

    /// <summary>
    /// 判断不流畅组-听辨认差的类型
    /// </summary>
    private static string 判断不流畅组_听辨认差(int 词复述, int 句复述)
    {
        // 第三级：句复述判断
        if ((句复述+词复述) / 复述满分 * 100f >= 58)
        {
            // 句复述好的（≥58）
            return "MT"; // 混合性经皮质失语(Mixed Transcortical Aphasia)
        }
        else
        {
            // 句复述差（<58）
            return "GA"; // 全面性失语(Global Aphasia)
        }
    }

    /// <summary>
    /// 获取失语症类型的中文全称（与条形图显示格式一致）
    /// </summary>
    public static string 获取失语症类型全称(string 类型缩写)
    {
        switch (类型缩写)
        {
            case "BA":
                return "Broca型(BA)";
            case "WA":
                return "Wernicke(WA)";
            case "CA":
                return "传导型(CA)";
            case "TCM":
                return "经皮质运动型(TCM)";
            case "TCS":
            case "TCA": // TCA和TCS是同一类型
                return "经皮质感觉型(TCS)";
            case "MT":
                return "经皮质混合型(MT)";
            case "AA":
                return "命名型(AA)";
            case "GA":
                return "完全型(GA)";
            case "TA":
                return "丘脑型(TA)";
            case "BaA":
                return "基底型(BaA)";
            case "TA或BaA":
                // 兼容旧数据，默认返回TA
                return "丘脑型(TA)";
            default:
                return 类型缩写;
        }
    }
}

