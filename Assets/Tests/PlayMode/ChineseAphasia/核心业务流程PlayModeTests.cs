using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class 核心业务流程PlayModeTests
{
    [UnityTest]
    public IEnumerator 用户加入待测名单_会生成初始化完整的档案()
    {
        var 用户 = 创建测试用户();
        var 评定人 = new 评人 { 名 = "测试评定人" };

        yield return null;

        var 档案 = 从用户创建待测档案(用户, 评定人.名);

        Assert.AreEqual(评估状态.未评估, 档案.评估状态);
        Assert.AreEqual("测试患者A", 档案.姓名);
        Assert.AreEqual("U20260515001", 档案.编号);
        Assert.AreEqual("测试评定人", 档案.评估人员);
        Assert.AreEqual("110101199001010011", 档案.身份证);
        Assert.NotNull(档案.谈话记录);
        Assert.NotNull(档案.理解记录);
        Assert.NotNull(档案.复述记录);
        Assert.NotNull(档案.命名记录);
        Assert.NotNull(档案.阅读记录);
        Assert.NotNull(档案.计算记录);
        Assert.NotNull(档案.书写记录);
        Assert.NotNull(档案.结构与空间记录);
        Assert.NotNull(档案.运用记录);
        Assert.NotNull(档案.利手记录);
    }

    [UnityTest]
    public IEnumerator 量表保存流程_理解记录分数和选项可保存并读取()
    {
        var 档案 = 从用户创建待测档案(创建测试用户(), "测试评定人");

        yield return null;

        档案.理解记录.是否题总分 = 55;
        档案.理解记录.听辨认图形总分 = 9;
        档案.理解记录.听辨认实物总分 = 9;
        档案.理解记录.口头指令总分 = 70;
        档案.理解记录.选项组选择["是否题1"] = 1;
        档案.理解记录.选项组选择["口头指令1"] = 0;
        档案.理解记录.是否完成 = 评估状态.评估完成;
        档案.理解记录.评估时间 = "2026-05-15 12:00";

        yield return null;

        Assert.AreEqual(55, 档案.理解记录.是否题总分);
        Assert.AreEqual(18, 档案.理解记录.听辨认图形总分 + 档案.理解记录.听辨认实物总分);
        Assert.AreEqual(1, 档案.理解记录.选项组选择["是否题1"]);
        Assert.AreEqual(0, 档案.理解记录.选项组选择["口头指令1"]);
        Assert.AreEqual(评估状态.评估完成, 档案.理解记录.是否完成);
        Assert.AreEqual("2026-05-15 12:00", 档案.理解记录.评估时间);
    }

    [UnityTest]
    public IEnumerator 报告计算流程_关键量表完成后可生成失语症类型和BDAE描述()
    {
        var 档案 = 从用户创建待测档案(创建测试用户(), "测试评定人");
        填充AA评定数据(档案);

        yield return null;

        var 失语症类型 = 失语症评定工具.计算失语症类型(档案);
        档案.失语症类型 = 失语症类型;
        档案.BDAE严重程度等级 = BDAE严重程度评定工具.推荐严重程度等级(档案, 失语症类型);
        档案.是否选择BDAE等级 = true;
        var BDAE描述 = BDAE严重程度评定工具.获取严重程度描述(档案.BDAE严重程度等级);

        yield return null;

        Assert.AreEqual("AA", 档案.失语症类型);
        Assert.AreEqual(5, 档案.BDAE严重程度等级);
        Assert.IsTrue(档案.是否选择BDAE等级);
        StringAssert.StartsWith("5级", BDAE描述);
    }

    [UnityTest]
    public IEnumerator Unity生命周期冒烟_测试对象可在PlayMode创建和销毁()
    {
        var go = new GameObject("ChineseAphasia_PlayModeSmoke");
        go.AddComponent<测试生命周期组件>();

        yield return null;

        var 组件 = go.GetComponent<测试生命周期组件>();
        Assert.IsTrue(组件.已启动);

        Object.Destroy(go);
        yield return null;

        Assert.IsTrue(go == null);
    }

    private static 用户 创建测试用户()
    {
        return new 用户
        {
            编号 = "U20260515001",
            名 = "测试患者A",
            性别 = "男",
            身份证 = "110101199001010011",
            诞辰 = "1990-01-01",
            民族 = "汉族",
            监护人 = "测试监护人",
            关系 = "父子",
            联系方式 = "13800000000",
            居住地址 = "测试地址",
            发病日期 = "2026-01-01",
            文化程度 = "本科",
            血压 = "120/80",
            神志状态 = "清醒",
            家庭成员 = "父母",
            烟龄 = "无",
            酒龄 = "无",
            发病前后语言状态 = "测试语言状态",
            发病时状况 = "测试发病状况",
            既往病史 = "无"
        };
    }

    private static 档案 从用户创建待测档案(用户 用户, string 评估人员)
    {
        return new 档案
        {
            评估状态 = 评估状态.未评估,
            姓名 = 用户.名,
            性别 = 用户.性别,
            出生日期 = 用户.诞辰,
            编号 = 用户.编号,
            添加时间 = "2026-05-15",
            年龄 = "36岁4月14天",
            评估人员 = 评估人员,
            评估时间 = "",
            身份证 = 用户.身份证,
            民族 = 用户.民族,
            监护人 = 用户.监护人,
            关系 = 用户.关系,
            联系方式 = 用户.联系方式,
            居住地址 = 用户.居住地址,
            发病日期 = 用户.发病日期,
            文化程度 = 用户.文化程度,
            血压 = 用户.血压,
            神志状态 = 用户.神志状态,
            家庭成员 = 用户.家庭成员,
            烟龄 = 用户.烟龄,
            酒龄 = 用户.酒龄,
            发病前后语言状态 = 用户.发病前后语言状态,
            发病时状况 = 用户.发病时状况,
            既往病史 = 用户.既往病史,
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
            利手记录 = new 利手记录(),
            是否选择BDAE等级 = false
        };
    }

    private static void 填充AA评定数据(档案 档案)
    {
        档案.谈话记录.谈话信息量总分 = 35;
        档案.谈话记录.谈话流利性总分 = 10;
        档案.谈话记录.系列语言总分 = 10;

        档案.理解记录.听辨认图形总分 = 9;
        档案.理解记录.听辨认实物总分 = 9;
        档案.理解记录.口头指令总分 = 95;

        档案.复述记录.词复述总分 = 45;
        档案.复述记录.句复述总分 = 45;

        档案.命名记录.词命名总分 = 38;
    }

    private class 测试生命周期组件 : MonoBehaviour
    {
        public bool 已启动 { get; private set; }

        private void Start()
        {
            已启动 = true;
        }
    }
}
