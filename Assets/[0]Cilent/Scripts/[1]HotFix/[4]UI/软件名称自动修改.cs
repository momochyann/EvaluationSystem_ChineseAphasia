using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Nova;
using System;

public class 软件名称自动修改 : MonoBehaviour, IController
{
    // Start is called before the first frame update
    TextBlock 软件名称;
    软件配置录 配置录;
    void Start()
    {
        软件名称 = GetComponent<TextBlock>();
        加载软件名称();
        配置录 = this.GetModel<软件配置录>();
        配置录.软件修改事件 += 自动加载名称;
    }

    private void 自动加载名称(软件配置 arg0)
    {
        Debug.Log("自动加载名称");
        if (arg0 != null)
        {
            软件名称.Text = string.IsNullOrEmpty(arg0.软件名称) ? "汉语失语症评测系统" : arg0.软件名称;
        }
        else
        {
            软件名称.Text = "汉语失语症评测系统";
        }
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
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }

    void OnDestroy()
    {
        if (配置录 != null)
        {
            配置录.软件修改事件 -= 自动加载名称;
        }
    }
}
