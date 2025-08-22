using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using NovaSamples.UIControls;
using Nova;
using DG.Tweening;
using Cysharp.Threading.Tasks;
public class 用户管理添加安卓_茉 : 界面基础_韩
{
    // Start is called before the first frame update

    [Header("个人信息")]
    [SerializeField] public TextBlock 编号;
    [SerializeField] public NovaSamples.UIControls.TextField 姓名;
    [SerializeField] public NovaSamples.UIControls.Dropdown 性别;
    [SerializeField] public NovaSamples.UIControls.TextField 身份证;
    [SerializeField] public Button 出生日期选择按钮;
    [SerializeField] public TextBlock 出生日期文本;
    [SerializeField] public NovaSamples.UIControls.TextField 民族;

    [Header("家庭信息")]
    [SerializeField] public NovaSamples.UIControls.TextField 监护人;
    [SerializeField] public NovaSamples.UIControls.TextField 关系;
    [SerializeField] public NovaSamples.UIControls.TextField 联系方式;
    [SerializeField] public NovaSamples.UIControls.TextField 居住地址;

    [Header("按钮")]
    [SerializeField] public NovaSamples.UIControls.Button 确定;
    [SerializeField] public NovaSamples.UIControls.Button 取消;
    [SerializeField] public NovaSamples.UIControls.Button 返回;
    [SerializeField] public Color inputSetColor;

    private void Awake()
    {
        显示信息 += 显示信息详情;
        性别.Visuals.SelectionLabel.Color = Color.gray * 0.8f;
        性别.Visuals.SelectionLabel.Text = "请选择";
    }
    void Start()
    {
        this.确定.OnClicked.AddListener(添加账号);
        this.取消.OnClicked.AddListener(取消添加);
        this.返回.OnClicked.AddListener(返回上个界面);
        输入文本框点击(姓名);
        输入文本框点击(身份证);
        输入文本框点击(民族);
        输入文本框点击(监护人);
        输入文本框点击(关系);
        输入文本框点击(联系方式);
        输入文本框点击(居住地址);
        性别等下拉选择框点击(性别);
        出生日期选择按钮.OnClicked.AddListener(() =>
        {
            选择出生日期().Forget();
        });
    }
    private void 返回上个界面()
    {
        清空信息();
        FindObjectOfType<界面管理器_韩>().显示界面("用户管理显示");
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

    void 输入文本框点击(TextField 输入框)
    {
        TextFieldSelector selector = 输入框.GetComponent<TextFieldSelector>();
        selector.OnFocused += () =>
        {
            选中动画(true, 输入框.transform.parent.parent);
        };
        selector.OnFocusLost += () =>
        {
            选中动画(false, 输入框.transform.parent.parent);
        };

    }
    void 性别等下拉选择框点击(Dropdown 选择框)
    {
        选择框.Visuals.OnExpand += () =>
        {
            选中动画(true, 选择框.transform.parent);
        };
        选择框.Visuals.OnCollapse += () =>
        {
            选中动画(false, 选择框.transform.parent);
        };
        选择框.OnValueChanged.AddListener((_) =>
        {
            选择框.Visuals.SelectionLabel.Color = Color.black * 0.95f;
        });
    }

    void 选中动画(bool 是否选中, Transform 选中对象)
    {
        if (是否选中)
        {
            if (选中对象.GetComponent<UIBlock2D>().Color.a == 0)
            {
                DOTween.To((value) =>
               {
                   选中对象.GetComponent<UIBlock2D>().Color = new Color(inputSetColor.r, inputSetColor.g, inputSetColor.b, value);
               }, 0, 1, 0.3f);
            }
        }
        else
        {
            if (选中对象.GetComponent<UIBlock2D>().Color.a == 1)
            {
                DOTween.To((value) =>
                {
                    选中对象.GetComponent<UIBlock2D>().Color = new Color(inputSetColor.r, inputSetColor.g, inputSetColor.b, value);
                }, 1, 0, 0.3f);
            }
        }
    }
    void 添加账号()
    {
        if (姓名.Text == "")
        {
            Debug.Log("[ [3] 评定人管理添加_韩]添加用户失败，请检查姓名");
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
        新用户.名 = 姓名.Text;
        新用户.性别 = 性别.DropdownOptions.Options[性别.DropdownOptions.SelectedIndex]; ;
        新用户.身份证 = 身份证.Text;
        新用户.诞辰 = 出生日期文本.Text;
        新用户.民族 = 民族.Text;

        新用户.居住地址 = 居住地址.Text;
        this.SendCommand(new 增记于录(新用户, "用户录"));
        Debug.Log("[ [3] 用户管理添加_韩]添加账号" + 新用户.名);
    }
    void 清空信息()
    {
        编号.Text = "";
        姓名.Text = "";
        性别.Visuals.SelectionLabel.Color = Color.gray * 0.8f;
        性别.Visuals.SelectionLabel.Text = "请选择";
        身份证.Text = "";
        出生日期文本.Text = "";
        民族.Text = "";
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
        居住地址.Text = 用户.居住地址;
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
