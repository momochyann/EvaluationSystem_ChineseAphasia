using System.Collections;
using System.Collections.Generic;
using Nova;
using NovaSamples.UIControls;
using UnityEngine;
using QFramework;
using Cysharp.Threading.Tasks;

public class 档案条目显示 : MonoBehaviour, IentryDis, IController
{
    public int 索引下标 { get; set; }
    [SerializeField] TextBlock 档案编号;
    [SerializeField] TextBlock 姓名;
    [SerializeField] TextBlock 性别;
    [SerializeField] TextBlock 出生日期;
    [SerializeField] TextBlock 身份信息;
    [SerializeField] TextBlock 评估时间;
    [SerializeField] TextBlock 评估人员;
    [SerializeField] Button 查看按钮;
    private int 当前索引 = -1;
    
    void Start()
    {
        if (查看按钮 != null)
        {
            查看按钮.OnClicked.AddListener(查看档案);
        }
    }

    public void 显示信息(int 索引 = -1)
    {
        当前索引 = 索引;
        索引下标 = 索引;
        var 档案录 = this.GetModel<档案录>();
        var 当前档案 = 档案录.GetDataList()[索引];
        
        // 只显示评估状态为完成的档案
        if (当前档案.评估状态 == 评估状态.评估完成)
        {
            档案编号.Text = 当前档案.编号;
            姓名.Text = 当前档案.姓名;
            性别.Text = 当前档案.性别;
            出生日期.Text = 当前档案.出生日期;
            身份信息.Text = 当前档案.年龄; // 或者可以根据需要显示其他身份信息
            评估时间.Text = 当前档案.评估时间;
            评估人员.Text = 当前档案.评估人员;
        }
        else
        {
            // 如果不是评估完成状态，销毁该显示条目
            Destroy(gameObject);
        }
    }

    void 查看档案()
    {
        if (当前索引 == -1)
        {
            Debug.LogError("[查看档案] 当前索引无效");
            return;
        }

        // 获取当前档案
        var 档案录 = this.GetModel<档案录>();
        var 当前档案 = 档案录.GetDataList()[当前索引];
        
        Debug.Log($"[查看档案] 查看档案: {当前档案.姓名}(编号:{当前档案.编号})");
        显示档案管理报告界面(当前档案).Forget();
        // 跳转到档案管理报告界面并传入档案数据
        // var 界面管理器 = FindObjectOfType<界面管理器_韩>();
        // if (界面管理器 != null)
        // {
        //     // 先显示界面
        //     // 界面管理器.显示界面("档案管理报告界面");
            
        //     // 界面管理器._currentPanel.显示信息?.Invoke(当前档案);
        //     // 获取档案管理报告界面并传入数据
        //     // var 报告界面 = FindObjectOfType<档案管理报告界面显示>();
        //     // if (报告界面 != null)
        //     // {
        //     //     报告界面.显示信息.Invoke(当前档案);
        //     // }
        //     // else
        //     // {
        //     //     Debug.LogError("[查看档案] 未找到档案管理报告界面显示组件");
        //     // }
        // }
        // else
        // {
        //     Debug.LogError("[查看档案] 未找到界面管理器");
        // }
    }
    async UniTaskVoid 显示档案管理报告界面(档案 当前档案)
    {
        var 界面管理器 = FindObjectOfType<界面管理器_韩>();
        界面管理器.显示界面("档案管理报告界面");
        await UniTask.WaitUntil(() => 界面管理器._currentPanel != null && 界面管理器._currentPanel.name.Contains("档案管理报告界面"));
        界面管理器._currentPanel.显示信息?.Invoke(当前档案);
    }
    // Update is called once per frame
    void Update()
    {

    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
