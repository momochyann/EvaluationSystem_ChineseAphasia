using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;
using QFramework;

public class 备份管理界面_茉 : 界面基础_韩
{
    [Header("UI组件")]
    [SerializeField] private Button 确认备份按钮;
    [SerializeField] private Button 确认还原按钮;
    [SerializeField] private Button 蓝牙同步按钮;
    

    评人录 评人录;
    void Start()
    {
        评人录 = this.GetModel<评人录>();
        设置按钮监听();

    }

    void 设置按钮监听()
    {
        确认备份按钮.OnClicked.AddListener(执行备份);
        确认还原按钮.OnClicked.AddListener(执行还原);
        蓝牙同步按钮.OnClicked.AddListener(执行蓝牙同步);
    }
    private void 执行蓝牙同步()
    {
        Debug.Log("执行蓝牙同步");
        // TODO: 实现蓝牙同步逻辑
    }
    private void ImportTemplate()
    {
        var jsonDataUtility = this.GetUtility<JsonDataUtility>();
        评人录.ImportFromJson(false);
    }

    private void ExportTemplate()
    {
        评人录.ExportToJson();
    }

    async void 执行备份()
    {
        var tip = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var tips = Instantiate(tip, this.transform);
        await tips.GetComponent<告示_茉>().等待确认显示("提示", "执行备份操作");
        Destroy(tips);
        // Debug.Log("执行备份操作");
        // TODO: 实现备份逻辑
    }

    async void 执行还原()
    {
        var tip = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var tips = Instantiate(tip, this.transform);
        await tips.GetComponent<告示_茉>().等待确认显示("确认一键还原吗", "一旦还原将不能恢复哦");
        Destroy(tips);
     
        // Debug.Log("执行还原操作");
        // TODO: 实现还原逻辑
    }

    void 返回上个界面()
    {
        Debug.Log("返回上个界面");
        var 界面管理器 = FindObjectOfType<界面管理器_韩>();
        if (界面管理器 != null)
        {
            界面管理器.隐藏界面();
        }
    }

    protected override void OnShow()
    {
        Debug.Log("[备份管理界面_茉] OnShow");
    }

    protected override void OnHide()
    {
        Debug.Log("[备份管理界面_茉] OnHide");
    }

    void OnDestroy()
    {
        确认备份按钮.OnClicked.RemoveAllListeners();
        确认还原按钮.OnClicked.RemoveAllListeners();
       
    }
}
