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
    private bool 正在跳转 = false;

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
        Debug.Log("执行编号管理功能");
        Debug.Log($"当前管理员状态: {管员录.登陆管理员 != null}");
        Debug.Log($"当前评定人状态: {评人录.登陆评人 != null}");

        // 检查管理员权限 - 编号管理需要管理员权限
        if (管员录.登陆管理员 == null)
        {
            Debug.Log("编号管理需要管理员权限，当前为评定人登录，弹出权限提示");
            // 评定人登录时，弹出权限提示
            await 显示权限提示();
            return;
        }

        Debug.Log("编号管理权限检查通过，开始加载界面");
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

    public async void 执行功能(string 功能名称)
    {
        Debug.Log("执行功能：" + 功能名称);
        Debug.Log($"当前管理员状态: {管员录.登陆管理员 != null}");
        Debug.Log($"当前评定人状态: {评人录.登陆评人 != null}");

        // 检查是否为管理员权限功能
        if (是否需要管理员权限(功能名称))
        {
            Debug.Log("此功能需要管理员权限");
            if (管员录.登陆管理员 == null)
            {
                Debug.Log("当前为评定人登录，弹出权限提示");
                // 评定人登录时，弹出权限提示
                await 显示权限提示();
                return;
            }
            else
            {
                Debug.Log("当前为管理员登录，允许执行");
            }
        }
        else
        {
            Debug.Log("此功能不需要管理员权限");
        }

        // 防止多次点击导致重复跳转
        if (正在跳转)
        {
            Debug.Log("正在跳转中,忽略重复点击");
            return;
        }

        正在跳转 = true;
        功能名称 = 功能名称 + "显示";
        Debug.Log("执行跳转: " + 功能名称);
        静态传递_韩.SwitchSceneWithPanel("主场景online", 功能名称);
    }

    /// <summary>
    /// 检查功能是否需要管理员权限
    /// </summary>
    private bool 是否需要管理员权限(string 功能名称)
    {
        Debug.Log($"检查权限 - 功能名称: '{功能名称}'");

        // 需要管理员权限的功能列表
        string[] 管理员功能 = new string[]
        {
            "评定人管理",
            "界面管理",
            "备份还原"
        };

        foreach (var 功能 in 管理员功能)
        {
            Debug.Log($"检查是否包含 '{功能}'");
            if (功能名称.Contains(功能))
            {
                Debug.Log($"匹配到管理员功能: {功能}");
                return true;
            }
        }

        Debug.Log("不需要管理员权限");
        return false;
    }

    /// <summary>
    /// 显示权限不足的提示
    /// </summary>
    private async UniTask 显示权限提示()
    {
        var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 告示实例 = Instantiate(告示, transform);
        await 告示实例.GetComponent<告示_茉>().等待确认显示("权限不足", "此仅为管理员权限，请登录管理员账号");
        Destroy(告示实例.gameObject, 0.1f);
    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
