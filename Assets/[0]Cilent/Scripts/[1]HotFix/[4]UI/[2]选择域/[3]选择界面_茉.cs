using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;
using Cysharp.Threading.Tasks;
using QFramework;
using System;
using Nova;

public class 选择界面_茉 : MonoBehaviour, IController
{
    [SerializeField] public Button 用户名;
    [SerializeField] public Button 退出;
    [SerializeField] public List<Button> 功能列表;
    [SerializeField] public Button 蓝牙连接按钮;
    [SerializeField] public Button 编号管理按钮;
    [SerializeField] Button 设置按钮;
    [SerializeField] GameObject 设置详情;
    [SerializeField] TextBlock 软件名称;
    private 评人录 评人录;
    private 管员录 管员录;

    void Start()
    {
        评人录 = this.GetModel<评人录>();
        管员录 = this.GetModel<管员录>();
        // if (评人录.登陆评人 != null)
        // {
        //     用户名.transform.GetComponentInChildren<TextBlock>().Text = 评人录.登陆评人.名;
        // }
        // if (管员录.登陆管理员 != null)
        // {
        //     用户名.transform.GetComponentInChildren<TextBlock>().Text = 管员录.登陆管理员.名;
        // }
        用户名.OnClicked.AddListener(执行用户名管理);
        退出.OnClicked.AddListener(执行退出);
        蓝牙连接按钮.OnClicked.AddListener(执行蓝牙连接);
        设置按钮.OnClicked.AddListener(展示设置);
        编号管理按钮.OnClicked.AddListener(执行编号管理);
        foreach (var item in 功能列表)
        {
            item.OnClicked.AddListener(() => 执行功能(item.name));
        }
        加载软件名称();
    }

    async private void 执行编号管理()
    {
        var 编号管理界面 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("编号管理栏");
        var 编号管理界面实例 = Instantiate(编号管理界面, transform);
      //  var 编号管理界面安卓 = 编号管理界面实例.GetComponent<编号管理界面_茉>();
        //编号管理界面安卓.执行编号管理();
    }

    void 加载软件名称()
    {
        var 软件配置录 = this.GetModel<软件配置录>();
        if (软件配置录.当前配置 != null)
        {
            软件名称.Text = string.IsNullOrEmpty(软件配置录.当前配置.软件名称) ? "汉语失语症评测系统" : 软件配置录.当前配置.软件名称;
        }
        else
        {
            软件名称.Text = "汉语失语症评测系统";
        }
    }

    private void 展示设置()
    {
        if (设置详情.activeSelf)
        {
            设置详情.SetActive(false);
        }
        else
        {
            设置详情.SetActive(true);
        }
    }

    async private void 执行蓝牙连接()
    {
        var 蓝牙连接界面气 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("蓝牙连接界面");
        var 蓝牙连接界面实例 = Instantiate(蓝牙连接界面气, transform);
        //   var 蓝牙连接界面实例组件 = 蓝牙连接界面实例.GetComponent<蓝牙连接界面_茉>();
        //   蓝牙连接界面实例组件.执行蓝牙连接();
    }

    async public void 执行用户名管理()
    {

        if (管员录.登陆管理员 != null)
        {
            var 评定人管理添加 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("评定人管理添加");
            var 评定人管理添加实例 = Instantiate(评定人管理添加, transform.parent);
            var 评定人管理添加安卓 = 评定人管理添加实例.GetComponent<评定人管理添加安卓_茉>();
            评定人管理添加安卓.显示信息(管员录.登陆管理员);
        }
        else if (评人录.登陆评人 != null)
        {
            var 评定人管理添加 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("评定人管理添加");
            var 评定人管理添加实例 = Instantiate(评定人管理添加, transform.parent);
            var 评定人管理添加安卓 = 评定人管理添加实例.GetComponent<评定人管理添加安卓_茉>();
            评定人管理添加安卓.显示信息(评人录.登陆评人);
        }

        //评定人管理添加安卓.显示信息();
        // 评定人管理添加实例组件.执行评定人管理添加();
    }

    public void 执行退出()
    {
        Debug.Log("退出");
        PlayerPrefs.SetString("自动登录", "false");
        LoadYooAssetsTool.LoadSceneAsync("登录界面场景").Forget();

    }

    public void 执行功能(string 功能名称)
    {
        Debug.Log("执行功能：" + 功能名称);
        功能名称 = 功能名称 + "显示";
        静态传递_韩.SwitchSceneWithPanel("主场景", 功能名称);
    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
