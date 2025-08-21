using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System;
using NovaSamples.UIControls;
using Nova;
using DG.Tweening;
public class 评定人管理添加安卓_茉 : 界面基础_韩
{
    // Start is called before the first frame update
    [SerializeField] public Color inputSetColor;
    [SerializeField] public NovaSamples.UIControls.TextField 登录账号;
    [SerializeField] public NovaSamples.UIControls.TextField 登录密码;
    [SerializeField] public NovaSamples.UIControls.TextField 姓名;
    [SerializeField] public NovaSamples.UIControls.TextField 性别;
    [SerializeField] public NovaSamples.UIControls.TextField 联系方式;
    [SerializeField] public NovaSamples.UIControls.TextField 身份证;
    [SerializeField] public NovaSamples.UIControls.TextField 出生日期;
    [SerializeField] public NovaSamples.UIControls.Button 确定;
    [SerializeField] public NovaSamples.UIControls.Button 取消;
    [SerializeField] public NovaSamples.UIControls.Button 临时添加;
    private void Awake()
    {
        登录账号.Text = "";
        登录密码.Text = "";
        姓名.Text = "";
        性别.Text = "";
        联系方式.Text = "";
        身份证.Text = "";
        出生日期.Text = "";
        显示信息 += 显示信息详情;
    }
    void Start()
    {

        确定.OnClicked.AddListener(() => 添加账号());
        取消.OnClicked.AddListener(() => 取消添加());
        输入文本框点击(登录账号);
        输入文本框点击(登录密码);
        输入文本框点击(姓名);
        输入文本框点击(联系方式);
        输入文本框点击(身份证);
      
        //  临时添加.OnClicked.AddListener(临时添加信息);
        // 登录账号.OnFocusGained += () =>
        // {
        //     Debug.Log("点击登录账号");
        // };
        // 登录账号.OnFocusLost += () =>
        // {
        //     Debug.Log("点击登录账号");
        // };
        // TextFieldSelector selector = 登录账号.GetComponent<TextFieldSelector>();
        // selector.OnFocused += () =>
        // 登录账号.OnTextSelectionChanged += () =>
        // {
        //     // 登录账号.isFocused
        //     // Debug.Log(登录账号.transform.parent.parent.GetComponent<UIBlock2D>().Color.a + " " + transform.parent.parent.gameObject.name);
        //     if (登录账号.transform.parent.parent.GetComponent<UIBlock2D>().Color.a == 0)
        //     {
        //         Debug.Log("点击登录账号1");
        //         DOTween.To((value) =>
        //         {
        //             登录账号.transform.parent.parent.GetComponent<UIBlock2D>().Color = new Color(inputSetColor.r, inputSetColor.g, inputSetColor.b, value);

        //         }, 0, 1, 0.5f);
        //     }
        //     else
        //     {
        //         Debug.Log("点击登录账号2");
        //         DOTween.To((value) =>
        //        {
        //            登录账号.transform.parent.parent.GetComponent<UIBlock2D>().Color = new Color(inputSetColor.r, inputSetColor.g, inputSetColor.b, value);

        //        }, 1, 0, 0.5f);
        //     }
        // };
    }
    void 输入文本框点击(TextField 输入框)
    {
        TextFieldSelector selector = 输入框.GetComponent<TextFieldSelector>();
        selector.OnFocused += () =>
        {
            if (输入框.transform.parent.parent.GetComponent<UIBlock2D>().Color.a == 0)
            {
                Debug.Log("点击登录账号1");
                DOTween.To((value) =>
                {
                    输入框.transform.parent.parent.GetComponent<UIBlock2D>().Color = new Color(inputSetColor.r, inputSetColor.g, inputSetColor.b, value);

                }, 0, 1, 0.3f);
            }
        };
        selector.OnFocusLost += () =>
        {
            if (登录账号.transform.parent.parent.GetComponent<UIBlock2D>().Color.a == 1)
            {
                DOTween.To((value) =>
                            {
                                输入框.transform.parent.parent.GetComponent<UIBlock2D>().Color = new Color(inputSetColor.r, inputSetColor.g, inputSetColor.b, value);

                            }, 1, 0, 0.3f);
            }

        };

    }
    private void 显示信息详情(I记贞 记)
    {
        Debug.Log("显示详情");
        评人 评人 = 记 as 评人;
        登录账号.Text = 评人.账号;
        登录密码.Text = 评人.密码;
        姓名.Text = 评人.名;
        性别.Text = 评人.性别;
        联系方式.Text = 评人.电话;
        身份证.Text = 评人.身份证;
        出生日期.Text = 评人.诞辰;

    }

    void 添加账号()
    {
        if (登录账号.Text == "")
        {
            Debug.Log("[ [2] 评定人管理添加_韩]添加账号失败，请检查登录账号");
            return;
        }
        if (登录密码.Text == "")
        {
            Debug.Log("[ [2] 评定人管理添加_韩]添加账号失败，请检查登录密码");
            return;
        }
        if (姓名.Text == "")
        {
            Debug.Log("[ [2] 评定人管理添加_韩]添加账号失败，请检查姓名");
            return;
        }

        评人 新评人 = new 评人();
        新评人.账号 = 登录账号.Text;
        新评人.密码 = 登录密码.Text;
        新评人.名 = 姓名.Text;
        新评人.性别 = 性别.Text;
        新评人.电话 = 联系方式.Text;
        新评人.身份证 = 身份证.Text;
        新评人.诞辰 = 出生日期.Text;
        this.SendCommand(new 增记于录(新评人, "评人录"));
        Debug.Log("[ [2] 评定人管理添加_韩]添加账号" + 新评人.名);
        清空文本框内容();
        FindObjectOfType<界面管理器_韩>().显示界面("评定人管理显示_PC");

    }

    void 取消添加()
    {
        Debug.Log("[ [2] 评定人管理添加_韩]取消添加");
        清空文本框内容();
        FindObjectOfType<界面管理器_韩>().显示界面("评定人管理显示_PC");

    }

    protected override void OnShow()
    {
        Debug.Log("[ [2] 评定人管理添加_韩]OnShow");
    }
    protected override void OnHide()
    {
        Debug.Log("[ [2] 评定人管理添加_韩]OnHide");
    }
    void 临时添加信息()
    {
        评人 新评人 = new 评人();
        新评人.账号 = "admin";
        新评人.密码 = "123456";
        新评人.名 = "张三";
        新评人.性别 = "男";
        新评人.电话 = "1234567890";
        新评人.身份证 = "1234567890";
        新评人.诞辰 = "2020-01-01";
        this.SendCommand(new 增记于录(新评人, "评人录"));
        Debug.Log("[ [2] 评定人管理添加_韩]临时添加信息" + 新评人.名);
        FindObjectOfType<界面管理器_韩>().显示界面("评定人管理显示_PC");
    }
    public void 清空文本框内容()
    {
        登录账号.Text = "";
        登录密码.Text = "";
        姓名.Text = "";
        性别.Text = "";
        联系方式.Text = "";
        身份证.Text = "";
        出生日期.Text = "";
    }
    void OnDestroy()
    {
        this.取消.OnClicked.RemoveAllListeners();
        this.确定.OnClicked.RemoveAllListeners();
    }

}

public struct 安卓输入结构体
{
    public TextField 输入框;
    public Button 按钮;
}