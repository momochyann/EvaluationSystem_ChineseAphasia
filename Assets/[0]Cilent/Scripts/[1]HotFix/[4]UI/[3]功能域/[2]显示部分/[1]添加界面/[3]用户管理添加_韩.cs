using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using NovaSamples.UIControls;
using Nova;
using System;
using Cysharp.Threading.Tasks;
public class 用户管理添加_韩 : 界面基础_韩
{
    // Start is called before the first frame update

    [Header("个人信息")]
    [SerializeField] public NovaSamples.UIControls.TextField 编号;
    [SerializeField] public NovaSamples.UIControls.TextField 姓名;
    // [SerializeField] public NovaSamples.UIControls.TextField 性别;
    [SerializeField] public NovaSamples.UIControls.TextField 身份证;

    [SerializeField] public NovaSamples.UIControls.TextField 民族;
    
    [Header("监护人信息")]
    [SerializeField] public NovaSamples.UIControls.TextField 监护人;
    [SerializeField] public NovaSamples.UIControls.TextField 关系;
    [SerializeField] public NovaSamples.UIControls.TextField 联系方式;
    [SerializeField] public NovaSamples.UIControls.TextField 居住地址;
    [Header("按钮")]
    [SerializeField] public NovaSamples.UIControls.Button 确定;
    [SerializeField] public NovaSamples.UIControls.Button 取消;
    I记贞 旧记;

    [SerializeField] public Button 出生日期选择按钮;
    [SerializeField] public NovaSamples.UIControls.Dropdown 性别;
    [SerializeField] public TextBlock 出生日期文本; 
    private void Awake()
    {
        显示信息 += 显示信息详情;
        // 性别.Visuals.SelectionLabel.Color = Color.gray * 0.8f;
        性别.Visuals.SelectionLabel.Text = "请选择";
    }
    private void OnEnable()
    {
        旧记 = null;
        // 性别.Visuals.SelectionLabel.Color = Color.gray * 0.8f;
        性别.Visuals.SelectionLabel.Text = "请选择";
        编号.Text = 编号管理界面_茉.生成用户编号(); // 使用编号管理界面配置的规则生成编号
    }
    void Start()
    {
        this.确定.OnClicked.AddListener(添加账号);
        this.取消.OnClicked.AddListener(取消添加);
        出生日期选择按钮.OnClicked.AddListener(() =>
        {
            选择出生日期().Forget();
        });
    }
    async UniTaskVoid 选择出生日期()
    {
        var 择日器气 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("择日器");
        var 择日器 = Instantiate(择日器气, this.transform);
        择日器.GetComponent<择日器_茉>().On保存日期 += (年, 月, 日) =>
        {
            Debug.Log("出生日期选择");
            出生日期文本.Text = $"{年}-{月}-{日}";
        };
    }
    void 添加账号()
    {
        if (姓名.Text == "")
        {
            Debug.Log("[ [3] 评定人管理添加_韩]添加用户失败，请检查姓名");
            return;
        }

        if (出生日期文本.Text.Length < 5)
        {
            Debug.Log("[ [3] 评定人管理添加_韩]添加用户失败，请检查出生日期");
            return;
        }
        添加信息();
        清空信息();
        FindObjectOfType<界面管理器_韩>().显示界面("用户管理显示");

    }

    void 取消添加()
    {
        Debug.Log("[ [3] 用户管理添加_韩]取消添加");
        清空信息();
        FindObjectOfType<界面管理器_韩>().显示界面("用户管理显示");

    }
    void 添加信息()
    {
        用户 新用户 = new 用户();
        新用户.编号 = 编号.Text;
        编号管理界面_茉.每日档案注册计数++;
        Debug.Log("[ [3] 用户管理添加_韩]" + 编号管理界面_茉.每日档案注册计数);
        新用户.名 = 姓名.Text;
        新用户.性别 = 性别.DropdownOptions.Options[性别.DropdownOptions.SelectedIndex];
        新用户.身份证 = 身份证.Text;
        新用户.诞辰 = 出生日期文本.Text;
        新用户.民族 = 民族.Text;
        新用户.监护人 = 监护人.Text;
        新用户.关系 = 关系.Text;
        新用户.联系方式 = 联系方式.Text;
        新用户.居住地址 = 居住地址.Text;
        if (旧记 != null)
        {
            this.SendCommand(new 改记于录(旧记, 新用户, "用户录"));
        }
        else
        {
            this.SendCommand(new 增记于录(新用户, "用户录"));
        }

        Debug.Log("[ [3] 用户管理添加_韩]添加账号" + 新用户.名);
    }
    void 清空信息()
    {
        编号.Text = "";
        姓名.Text = "";
        性别.Visuals.SelectionLabel.Color = Color.gray * 0.8f;
        性别.Visuals.SelectionLabel.Text = "请选择";
        身份证.Text = "";
        出生日期文本.Text = "待选择";
        民族.Text = "";
        监护人.Text = "";
        关系.Text = "";
        联系方式.Text = "";
        居住地址.Text = "";
    }
    void 显示信息详情(I记贞 记)
    {
        Debug.Log("[ [3] 用户管理添加_韩]显示信息详情");
        用户 用户 = 记 as 用户;
        编号.Text = 用户.编号;
        姓名.Text = 用户.名;
        性别.DropdownOptions.SelectedIndex = 用户.性别 == "男" ? 0 : 1;
        身份证.Text = 用户.身份证;
        出生日期文本.Text = 用户.诞辰;
        民族.Text = 用户.民族;
        监护人.Text = 用户.监护人;
        关系.Text = 用户.关系;
        联系方式.Text = 用户.联系方式;
        居住地址.Text = 用户.居住地址;
        旧记 = 记;
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

        新用户.监护人 = "张三";
        新用户.关系 = "父子";
        新用户.联系方式 = "1234567890";
        新用户.居住地址 = "北京市海淀区";

        if (旧记 != null)
        {
            this.SendCommand(new 改记于录(旧记, 新用户, "用户录"));
        }
        else
        {
            this.SendCommand(new 增记于录(新用户, "用户录"));
        }



        Debug.Log("[ [3] 用户管理添加_韩]临时添加信息" + 新用户.名);
        FindObjectOfType<界面管理器_韩>().显示界面("用户管理显示");
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
       
    }
}
