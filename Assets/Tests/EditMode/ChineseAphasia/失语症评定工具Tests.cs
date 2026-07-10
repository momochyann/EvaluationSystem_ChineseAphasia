using NUnit.Framework;

public class 失语症评定工具Tests
{
    [TestCase(40, 18, 45, 45, 35, "AA", TestName = "流畅_听辨认好_复述好_评定为AA")]
    [TestCase(40, 18, 35, 35, 35, "CA", TestName = "流畅_听辨认好_复述差_评定为CA")]
    [TestCase(40, 10, 40, 40, 35, "TCA", TestName = "流畅_听辨认差_复述好_评定为TCA")]
    [TestCase(40, 10, 25, 25, 35, "WA", TestName = "流畅_听辨认差_复述差_评定为WA")]
    [TestCase(20, 12, 40, 40, 35, "TCM", TestName = "不流畅_听辨认好_复述好_命名好_评定为TCM")]
    [TestCase(20, 12, 40, 40, 20, "TA或BaA", TestName = "不流畅_听辨认好_复述好_命名差_评定为TA或BaA")]
    [TestCase(20, 12, 25, 25, 35, "BA", TestName = "不流畅_听辨认好_复述差_评定为BA")]
    [TestCase(20, 8, 30, 30, 35, "MT", TestName = "不流畅_听辨认差_复述好_评定为MT")]
    [TestCase(20, 8, 15, 15, 35, "GA", TestName = "不流畅_听辨认差_复述差_评定为GA")]
    public void 计算失语症类型_覆盖决策树主要分支(
        float 谈话总分,
        float 听辨认总分,
        float 词复述,
        float 句复述,
        float 词命名,
        string 期望类型)
    {
        var 档案 = 创建档案(谈话总分, 听辨认总分, 词复述, 句复述, 词命名);

        var 实际类型 = 失语症评定工具.计算失语症类型(档案);

        Assert.AreEqual(期望类型, 实际类型);
    }

    [Test]
    public void 计算失语症类型_档案为空_返回未评定()
    {
        Assert.AreEqual("未评定", 失语症评定工具.计算失语症类型(null));
    }

    [Test]
    public void 计算失语症类型_缺少关键记录_返回数据不完整()
    {
        var 档案 = new 档案
        {
            谈话记录 = new 谈话记录(),
            理解记录 = new 理解记录(),
            复述记录 = null,
            命名记录 = new 命名记录()
        };

        Assert.AreEqual("评估数据不完整", 失语症评定工具.计算失语症类型(档案));
    }

    [TestCase("BA", "Broca型(BA)")]
    [TestCase("WA", "Wernicke(WA)")]
    [TestCase("TCA", "经皮质感觉型(TCS)")]
    [TestCase("TA或BaA", "丘脑型(TA)")]
    public void 获取失语症类型全称_返回报告显示文本(string 类型, string 期望全称)
    {
        Assert.AreEqual(期望全称, 失语症评定工具.获取失语症类型全称(类型));
    }

    private static 档案 创建档案(float 谈话总分, float 听辨认总分, float 词复述, float 句复述, float 词命名)
    {
        return new 档案
        {
            谈话记录 = new 谈话记录
            {
                谈话信息量总分 = 谈话总分,
                谈话流利性总分 = 0,
                系列语言总分 = 0
            },
            理解记录 = new 理解记录
            {
                听辨认图形总分 = 听辨认总分,
                听辨认实物总分 = 0
            },
            复述记录 = new 复述记录
            {
                词复述总分 = 词复述,
                句复述总分 = 句复述
            },
            命名记录 = new 命名记录
            {
                词命名总分 = 词命名
            }
        };
    }
}
