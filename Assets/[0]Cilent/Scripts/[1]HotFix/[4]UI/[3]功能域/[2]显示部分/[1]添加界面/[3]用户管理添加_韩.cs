using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using NovaSamples.UIControls;
public class 用户管理添加_韩 : 界面基础_韩
{
    // Start is called before the first frame update

    [Header("个人信息")]
    [SerializeField] public NovaSamples.UIControls.TextField 编号;
    [SerializeField] public NovaSamples.UIControls.TextField 姓名;
    [SerializeField] public NovaSamples.UIControls.TextField 性别;
    [SerializeField] public NovaSamples.UIControls.TextField 身份证;
    [SerializeField] public NovaSamples.UIControls.TextField 出生日期;
    [SerializeField] public NovaSamples.UIControls.TextField 民族;
    [Header("出生信息")]
    [SerializeField] public NovaSamples.UIControls.TextField 怀孕时间;
    [SerializeField] public NovaSamples.UIControls.TextField 分娩方式;
    [SerializeField] public NovaSamples.UIControls.TextField 出生身长;
    [SerializeField] public NovaSamples.UIControls.TextField 出生体重;
    [SerializeField] public NovaSamples.UIControls.TextField 出生头围;

    [Header("父母信息")]
    [SerializeField] public NovaSamples.UIControls.TextField 父亲;
    [SerializeField] public NovaSamples.UIControls.TextField 父亲电话;
    [SerializeField] public NovaSamples.UIControls.TextField 母亲;
    [SerializeField] public NovaSamples.UIControls.TextField 母亲电话;
    [SerializeField] public NovaSamples.UIControls.Toggle 是否为本地户籍;
    [SerializeField] public NovaSamples.UIControls.TextField 居住地址;
    [Header("按钮")]
    [SerializeField] public NovaSamples.UIControls.Button 确定;
    [SerializeField] public NovaSamples.UIControls.Button 取消;
    [SerializeField] public NovaSamples.UIControls.Button 临时添加;


    void Start()
    {
        this.确定.OnClicked.AddListener(添加账号);
        this.取消.OnClicked.AddListener(取消添加);
        this.临时添加.OnClicked.AddListener(临时添加信息);
    }

    void 添加账号()
    {
        if (姓名.Text == "")
        {
            Debug.Log("[ [3] 评定人管理添加_韩]添加用户失败，请检查姓名");
            return;
        }
        if (性别.Text == "")
        {
            Debug.Log("[ [3] 评定人管理添加_韩]添加用户失败，请检查性别");
            return;
        }
        if (出生日期.Text == "")
        {
            Debug.Log("[ [3] 评定人管理添加_韩]添加用户失败，请检查出生日期");
            return;
        }
        添加信息();
        清空信息();
        FindObjectOfType<界面管理器_韩>().显示界面("用户管理显示_PC");

    }

    void 取消添加()
    {
        Debug.Log("[ [3] 用户管理添加_韩]取消添加");
        清空信息();
        FindObjectOfType<界面管理器_韩>().显示界面("用户管理显示_PC");

    }
    void 添加信息()
    {
        用户 新用户 = new 用户();
        新用户.编号 = 编号.Text;
        新用户.名 = 姓名.Text;
        新用户.性别 = 性别.Text;
        新用户.身份证 = 身份证.Text;
        新用户.诞辰 = 出生日期.Text;
        新用户.民族 = 民族.Text;
        新用户.怀孕时间 = 怀孕时间.Text;
        新用户.分娩方式 = 分娩方式.Text;
        新用户.出生身长 = 出生身长.Text;
        新用户.出生体重 = 出生体重.Text;
        新用户.出生头围 = 出生头围.Text;
        新用户.父亲 = 父亲.Text;
        新用户.父亲电话 = 父亲电话.Text;
        新用户.母亲 = 母亲.Text;
        新用户.母亲电话 = 母亲电话.Text;
        新用户.是否为本地户籍 = 是否为本地户籍.ToggledOn;
        新用户.居住地址 = 居住地址.Text;
        this.SendCommand(new 增记于录(新用户, "用户录"));
        Debug.Log("[ [3] 用户管理添加_韩]添加账号" + 新用户.名);
    }
    void 清空信息()
    {
        编号.Text = "";
        姓名.Text = "";
        性别.Text = "";
        身份证.Text = "";
        出生日期.Text = "";
        民族.Text = "";
        怀孕时间.Text = "";
        分娩方式.Text = "";
        出生身长.Text = "";
        出生体重.Text = "";
        出生头围.Text = "";
        父亲.Text = "";
        父亲电话.Text = "";
        母亲.Text = "";
        母亲电话.Text = "";
        是否为本地户籍.ToggledOn = false;
        居住地址.Text = "";
    }
    void 临时添加信息()
    {
        // 临时添加信息
        用户 新用户 = new 用户();
        新用户.编号 = "123456";
        新用户.名 = "张三";
        新用户.性别 = "男";
        新用户.身份证 = "1234567890";
        新用户.诞辰 = "2020-01-01";
        新用户.民族 = "汉族";
        新用户.怀孕时间 = "2020-01-01";
        新用户.分娩方式 = "顺产";
        新用户.出生身长 = "50";
        新用户.出生体重 = "3000";
        新用户.出生头围 = "30";
        新用户.父亲 = "张三";
        新用户.父亲电话 = "1234567890";
        新用户.母亲 = "李四";
        新用户.母亲电话 = "1234567890";
        新用户.是否为本地户籍 = true;
        新用户.居住地址 = "北京市海淀区";
        this.SendCommand(new 增记于录(新用户, "用户录"));
        Debug.Log("[ [3] 用户管理添加_韩]临时添加信息" + 新用户.名);
        FindObjectOfType<界面管理器_韩>().显示界面("用户管理显示_PC");
    }
    protected override void OnShow()
    {
        Debug.Log("[ [3] 用户管理添加_韩]OnShow");
    }
    protected override void OnHide()
    {
        Debug.Log("[ [3] 用户管理添加_韩]OnHide");
    }
    void OnDestroy()
    {
        this.取消.OnClicked.RemoveAllListeners();
        this.确定.OnClicked.RemoveAllListeners();
        this.临时添加.OnClicked.RemoveAllListeners();
    }
}
