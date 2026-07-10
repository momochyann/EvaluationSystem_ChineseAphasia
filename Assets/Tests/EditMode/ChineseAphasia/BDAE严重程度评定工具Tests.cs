using NUnit.Framework;

public class BDAE严重程度评定工具Tests
{
    [TestCase(0, "0级")]
    [TestCase(1, "1级")]
    [TestCase(2, "2级")]
    [TestCase(3, "3级")]
    [TestCase(4, "4级")]
    [TestCase(5, "5级")]
    public void 获取严重程度描述_合法等级_返回对应等级文本(int 等级, string 期望前缀)
    {
        var 描述 = BDAE严重程度评定工具.获取严重程度描述(等级);

        StringAssert.StartsWith(期望前缀, 描述);
        Assert.IsNotEmpty(描述);
    }

    [Test]
    public void 获取严重程度描述_非法等级_返回未评定()
    {
        Assert.AreEqual("未评定", BDAE严重程度评定工具.获取严重程度描述(99));
    }

    [Test]
    public void 获取等级选项列表_返回0到5级()
    {
        CollectionAssert.AreEqual(
            new[] { "0级", "1级", "2级", "3级", "4级", "5级" },
            BDAE严重程度评定工具.获取等级选项列表());
    }

    [Test]
    public void 推荐严重程度等级_全面性失语且表达理解很低_推荐0级()
    {
        var 档案 = 创建档案(5, 5, 5, 5, 0);

        var 等级 = BDAE严重程度评定工具.推荐严重程度等级(档案, "GA");

        Assert.AreEqual(0, 等级);
    }

    [Test]
    public void 推荐严重程度等级_异常性失语且表达理解高_推荐5级()
    {
        var 档案 = 创建档案(90, 90, 90, 95, 0);

        var 等级 = BDAE严重程度评定工具.推荐严重程度等级(档案, "AA");

        Assert.AreEqual(5, 等级);
    }

    [Test]
    public void 推荐严重程度等级_档案为空_返回0级()
    {
        Assert.AreEqual(0, BDAE严重程度评定工具.推荐严重程度等级(null, "AA"));
    }

    [Test]
    public void 推荐严重程度等级_缺少必要记录_返回0级()
    {
        var 档案 = new 档案
        {
            谈话记录 = new 谈话记录(),
            理解记录 = null
        };

        Assert.AreEqual(0, BDAE严重程度评定工具.推荐严重程度等级(档案, "AA"));
    }

    private static 档案 创建档案(float 谈话流利性, float 谈话信息量, float 系列语言, float 听辨认图形, float 口头指令)
    {
        return new 档案
        {
            谈话记录 = new 谈话记录
            {
                谈话流利性总分 = 谈话流利性,
                谈话信息量总分 = 谈话信息量,
                系列语言总分 = 系列语言
            },
            理解记录 = new 理解记录
            {
                听辨认图形总分 = 听辨认图形,
                口头指令总分 = 口头指令
            }
        };
    }
}
