using NUnit.Framework;

public class 档案数据Tests
{
    [Test]
    public void 新建档案_可保存用户基本信息和背景信息()
    {
        var 档案 = new 档案
        {
            姓名 = "测试患者A",
            性别 = "男",
            编号 = "U20260515001",
            出生日期 = "1990-01-01",
            年龄 = "36岁4月14天",
            评估人员 = "测试评定人",
            身份证 = "110101199001010011",
            民族 = "汉族",
            联系方式 = "13800000000",
            居住地址 = "测试地址",
            发病日期 = "2026-01-01",
            文化程度 = "本科",
            既往病史 = "无"
        };

        Assert.AreEqual("测试患者A", 档案.姓名);
        Assert.AreEqual("U20260515001", 档案.编号);
        Assert.AreEqual("测试评定人", 档案.评估人员);
        Assert.AreEqual("本科", 档案.文化程度);
    }

    [Test]
    public void 新建档案_可初始化全部评估记录()
    {
        var 档案 = 创建完整空档案();

        Assert.NotNull(档案.谈话记录);
        Assert.NotNull(档案.理解记录);
        Assert.NotNull(档案.复述记录);
        Assert.NotNull(档案.命名记录);
        Assert.NotNull(档案.阅读记录);
        Assert.NotNull(档案.计算记录);
        Assert.NotNull(档案.书写记录);
        Assert.NotNull(档案.结构与空间记录);
        Assert.NotNull(档案.视觉记录);
        Assert.NotNull(档案.运用记录);
        Assert.NotNull(档案.利手记录);
    }

    [Test]
    public void 评估记录_默认状态可以标记为未完成和完成()
    {
        var 档案 = 创建完整空档案();

        档案.谈话记录.是否完成 = 评估状态.未完成;
        Assert.AreEqual(评估状态.未完成, 档案.谈话记录.是否完成);

        档案.谈话记录.是否完成 = 评估状态.评估完成;
        档案.谈话记录.评估时间 = "2026-05-15 11:00";

        Assert.AreEqual(评估状态.评估完成, 档案.谈话记录.是否完成);
        Assert.AreEqual("2026-05-15 11:00", 档案.谈话记录.评估时间);
    }

    [Test]
    public void 量表记录_可保存并读取选项选择()
    {
        var 档案 = 创建完整空档案();

        档案.理解记录.选项组选择["是否题1"] = 1;
        档案.理解记录.选项组选择["口头指令1"] = 0;

        Assert.AreEqual(1, 档案.理解记录.选项组选择["是否题1"]);
        Assert.AreEqual(0, 档案.理解记录.选项组选择["口头指令1"]);
    }

    [Test]
    public void 报告字段_可保存综合评价指导建议和BDAE选择()
    {
        var 档案 = 创建完整空档案();

        档案.综合评价 = "综合评价测试内容";
        档案.指导建议 = "指导建议测试内容";
        档案.BDAE严重程度等级 = 3;
        档案.是否选择BDAE等级 = true;

        Assert.AreEqual("综合评价测试内容", 档案.综合评价);
        Assert.AreEqual("指导建议测试内容", 档案.指导建议);
        Assert.AreEqual(3, 档案.BDAE严重程度等级);
        Assert.IsTrue(档案.是否选择BDAE等级);
    }

    private static 档案 创建完整空档案()
    {
        return new 档案
        {
            评估状态 = 评估状态.未评估,
            谈话记录 = new 谈话记录(),
            理解记录 = new 理解记录(),
            复述记录 = new 复述记录(),
            命名记录 = new 命名记录(),
            阅读记录 = new 阅读记录(),
            计算记录 = new 计算记录(),
            书写记录 = new 书写记录(),
            结构与空间记录 = new 结构与空间记录(),
            视觉记录 = new 视觉记录(),
            运用记录 = new 运用记录(),
            利手记录 = new 利手记录()
        };
    }
}
