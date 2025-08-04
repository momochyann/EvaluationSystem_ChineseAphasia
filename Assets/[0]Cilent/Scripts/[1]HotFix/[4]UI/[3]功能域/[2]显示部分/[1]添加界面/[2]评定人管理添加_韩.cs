using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
public class 评定人管理添加_韩 : 界面基础_韩
{
    // Start is called before the first frame update
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
    void Start()
    {
        登录账号.Text = "";
        登录密码.Text = "";
        姓名.Text = "";
        性别.Text = "";
        联系方式.Text = "";
        身份证.Text = "";
        出生日期.Text = "";
        确定.OnClicked.AddListener(() => 添加账号());
        取消.OnClicked.AddListener(() => 取消添加());
        临时添加.OnClicked.AddListener(临时添加信息);
    }
    void 添加账号()
    {
        if(登录账号.Text == "")
        {
            Debug.Log("[ [2] 评定人管理添加_韩]添加账号失败，请检查登录账号");
            return;
        }
        if(登录密码.Text == "")
        {
            Debug.Log("[ [2] 评定人管理添加_韩]添加账号失败，请检查登录密码");
            return;
        }
        if(姓名.Text == "")
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
        FindObjectOfType<界面管理器_韩>().显示界面("评定人管理显示_PC");
    }
    void 取消添加()
    {
        Debug.Log("[ [2] 评定人管理添加_韩]取消添加");
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
        新评人.账号 = "123456";
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
    void OnDestroy()
    {
        this.取消.OnClicked.RemoveAllListeners();
        this.确定.OnClicked.RemoveAllListeners();
        this.临时添加.OnClicked.RemoveAllListeners();
    }

}
