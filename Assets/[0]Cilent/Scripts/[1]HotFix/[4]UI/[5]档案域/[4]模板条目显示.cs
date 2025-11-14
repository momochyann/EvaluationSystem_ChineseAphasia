using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;
using Nova;
using Cysharp.Threading.Tasks;
using QFramework;
using System;
/// <summary>
/// 模板条目显示组件 - 用于显示单个模板项
/// </summary>
public class 模板条目显示 : MonoBehaviour, IController
{
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
    [Header("UI组件")]
    [SerializeField] private TextField 模板名称文本;
    [SerializeField] private TextField 模板内容文本;
    [SerializeField] private Button 保存按钮;
    [SerializeField] private Button 选择按钮;
    [SerializeField] private Button 删除按钮;
    private 评估模板 当前模板;
    private 评估模板设置显示 父界面;
    private 评估模板类型 模板类型;
    private System.Action<评估模板> 选择回调;

    /// <summary>
    /// 初始化模板条目
    /// </summary>
    public void 初始化(评估模板 模板, 评估模板设置显示 父界面, 评估模板类型 模板类型, System.Action<评估模板> 选择回调)
    {
        当前模板 = 模板;
        this.父界面 = 父界面;
        this.模板类型 = 模板类型;
        if (模板类型 == 评估模板类型.综合评价)
        {
            选择按钮.GetComponentInChildren<TextBlock>().Text = "保存评价";
        }
        else if (模板类型 == 评估模板类型.指导建议)
        {
            选择按钮.GetComponentInChildren<TextBlock>().Text = "保存建议";
        }
        this.选择回调 = 选择回调;

        // 显示模板信息
        if (模板名称文本 != null)
            模板名称文本.Text = 模板.模板名称;

        if (模板内容文本 != null)
            模板内容文本.Text = 模板.模板内容;

        // 绑定按钮事件
        if (选择按钮 != null)
        {
            选择按钮.OnClicked.RemoveAllListeners();
            选择按钮.OnClicked.AddListener(() =>
            {
                // 选择前先保存当前编辑
                保存当前编辑();
                选择回调?.Invoke(当前模板);
            });
        }

        if (保存按钮 != null)
        {
            保存按钮.OnClicked.RemoveAllListeners();
            保存按钮.OnClicked.AddListener(() =>
            {
                保存当前编辑();
            });
        }
        if (删除按钮 != null)
        {
            删除按钮.OnClicked.RemoveAllListeners();
            删除按钮.OnClicked.AddListener(() =>
            {
                删除模板();
            });
        }
    }

    async private void 删除模板()
    {
        var 确认 = await 保存通知("提示", "确定要删除该模板吗？");
        if (确认)
        {
            // 删除模板
            父界面.删除模板修改(当前模板);
            //Destroy(gameObject);
        }
    }

    /// <summary>
    /// 保存当前编辑的内容
    /// </summary>
    private void 保存当前编辑()
    {
        if (父界面 == null || 当前模板 == null)
            return;

        // 获取当前输入框的内容
        string 新名称 = 模板名称文本 != null ? 模板名称文本.Text.Trim() : 当前模板.模板名称;
        string 新内容 = 模板内容文本 != null ? 模板内容文本.Text.Trim() : 当前模板.模板内容;

        // 检查是否有修改
        if (新名称 == 当前模板.模板名称 && 新内容 == 当前模板.模板内容)
        {
            return; // 没有修改，不需要保存
        }

        // 创建新的模板对象
        var 新模板 = new 评估模板
        {
            模板名称 = string.IsNullOrEmpty(新名称) ? "未命名模板" : 新名称,
            模板内容 = 新内容,
            模板类型 = 当前模板.模板类型,
            创建时间 = 当前模板.创建时间,
            最后修改时间 = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        // 通知父界面保存
        父界面.保存模板修改(当前模板, 新模板);

        // 更新当前模板引用
        当前模板 = 新模板;
        保存通知("提示", "保存成功").Forget();
    }
    async UniTask<bool> 保存通知(string 标题, string 内容)
    {
        var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 界面管理器 = FindObjectOfType<界面管理器_韩>();
        var ins = Instantiate(告示, 界面管理器.transform);
        var 用户选择 = await ins.GetComponent<告示_茉>().等待确认显示(标题, 内容);
        Destroy(ins.gameObject, 0.1f);
        return 用户选择;
    }
    /// <summary>
    /// 更新模板内容显示
    /// </summary>
    public void 更新显示(评估模板 模板)
    {
        当前模板 = 模板;

        if (模板名称文本 != null)
            模板名称文本.Text = 模板.模板名称;

        if (模板内容文本 != null)
            模板内容文本.Text = 模板.模板内容;
    }
}

