using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;
using Nova;
using DG.Tweening;
using QFramework;
public class 左帜象_茉 : MonoBehaviour, IController
{
    [SerializeField] Color 选中颜色;
    [SerializeField] Color 未选中颜色;
    TextField textField;
    左目录选择_韩 目录形;
    private 评人录 评人录;
    private 管员录 管员录;

    void Start()
    {
        目录形 = GetComponent<左目录选择_韩>();
        评人录 = this.GetModel<评人录>();
        管员录 = this.GetModel<管员录>();

        foreach (var item in 目录形.目录列表)
        {
            item.OnClicked.AddListener(() => 尝试显切象(item));
        }
    }

    /// <summary>
    /// 尝试显示视觉切换，只有在有权限时才执行
    /// </summary>
    private void 尝试显切象(Button button)
    {
        // 检查是否为管理员权限功能且当前不是管理员登录
        if (是否需要管理员权限(button.name) && 管员录.登陆管理员 == null)
        {
            // 权限不足，不切换视觉状态
            return;
        }

        // 有权限，执行视觉切换
        显切象(button);
    }

    public void 显切象(Button button)
    {
        var uiBlock = button.GetComponent<UIBlock2D>();
       // 更新Block2D颜色(button.transform.Find("图标").GetComponent<UIBlock2D>(), Color.white);
        更新Block2D颜色(uiBlock, 选中颜色);
        更新Block2D颜色(button.transform.Find("标题").GetComponent<TextBlock>(), Color.white);
        foreach (var item in 目录形.目录列表)
        {
            if (item != button)
            {
                更新Block2D颜色(item.GetComponent<UIBlock2D>(), 未选中颜色);
               // 更新Block2D颜色(item.transform.Find("图标").GetComponent<UIBlock2D>(), 未选中颜色);
                更新Block2D颜色(item.transform.Find("标题").GetComponent<TextBlock>(), Color.black);
            }
        }
    }

    /// <summary>
    /// 检查功能是否需要管理员权限
    /// </summary>
    private bool 是否需要管理员权限(string 功能名称)
    {
        // 需要管理员权限的功能列表
        string[] 管理员功能 = new string[]
        {
            "评定人管理",
            "界面管理",
            "备份还原"
        };

        foreach (var 功能 in 管理员功能)
        {
            if (功能名称.Contains(功能))
            {
                return true;
            }
        }
        return false;
    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
    void 更新Block2D颜色(UIBlock block, Color color, float duration = 0.4f)
    {
        Color startColor = block.Color;
        DOTween.To(() => startColor, x =>
        {
            startColor = x;
            block.Color = x;
        }, color, duration);
    }

    // button.();
    //button.GetComponent<UIBlock2D>().Color = Color.white;
}
