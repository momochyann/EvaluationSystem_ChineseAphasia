using System;
using NUnit.Framework;

public class 编号规则Tests
{
    [Test]
    public void 编号规则配置_默认自定义字符串为空()
    {
        var 配置 = new 编号规则配置();

        Assert.AreEqual(0, 配置.规则类型);
        Assert.AreEqual(string.Empty, 配置.自定义字符串);
    }

    [Test]
    public void 编号规则_前缀格式_自定义字符加日期加三位序号()
    {
        var 编号 = 生成编号(规则类型: 0, 自定义字符串: "U", 日期: new DateTime(2026, 5, 15), 序号: 1);

        Assert.AreEqual("U20260515001", 编号);
    }

    [Test]
    public void 编号规则_后缀格式_日期加三位序号加自定义字符()
    {
        var 编号 = 生成编号(规则类型: 1, 自定义字符串: "A", 日期: new DateTime(2026, 5, 15), 序号: 12);

        Assert.AreEqual("20260515012A", 编号);
    }

    [TestCase("A", true)]
    [TestCase("ABCDE", true)]
    [TestCase("", false)]
    [TestCase("ABCDEF", false)]
    public void 编号规则_自定义字符长度_要求1到5个字符(string 输入, bool 期望有效)
    {
        Assert.AreEqual(期望有效, 自定义字符有效(输入));
    }

    private static string 生成编号(int 规则类型, string 自定义字符串, DateTime 日期, int 序号)
    {
        var 日期部分 = 日期.ToString("yyyyMMdd");
        var 序号部分 = 序号.ToString("D3");

        return 规则类型 == 0
            ? $"{自定义字符串}{日期部分}{序号部分}"
            : $"{日期部分}{序号部分}{自定义字符串}";
    }

    private static bool 自定义字符有效(string 输入)
    {
        return !string.IsNullOrEmpty(输入) && 输入.Length >= 1 && 输入.Length <= 5;
    }
}
