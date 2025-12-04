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
    [SerializeField] Button 删除按钮;
    private int 当前索引 = -1;
    
    void Start()
    {
        if (查看按钮 != null)
        {
            查看按钮.OnClicked.AddListener(查看档案);
        }
        if (删除按钮 != null)
        {
            删除按钮.OnClicked.AddListener(删除本条目);
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
            if (档案编号 != null) 档案编号.Text = 当前档案.编号;
            if (姓名 != null) 姓名.Text = 当前档案.姓名;
            if (性别 != null) 性别.Text = 当前档案.性别;
            if (出生日期 != null) 出生日期.Text = 当前档案.出生日期;
            if (身份信息 != null) 身份信息.Text = 当前档案.年龄; // 或者可以根据需要显示其他身份信息
            if (评估时间 != null) 评估时间.Text = 当前档案.评估时间;
            if (评估人员 != null) 评估人员.Text = 当前档案.评估人员;
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

    /// <summary>
    /// 删除当前档案条目入口：先弹出确认框，再真正删除
    /// </summary>
    void 删除本条目()
    {
        删除本条目带确认().Forget();
    }

    /// <summary>
    /// 异步：显示告示预制体确认，确认后再删除数据并记录到删除档案录
    /// </summary>
    private async UniTaskVoid 删除本条目带确认()
    {
        if (当前索引 == -1)
        {
            Debug.LogError("[档案条目显示] 删除本条目失败：当前索引无效(-1)");
            return;
        }

        // 加载告示预制体并显示确认框
        var utility = this.GetUtility<YooAssetPfbPoolUtility>();
        var prefab = await utility.LoadPfb("告示");
        if (prefab == null)
        {
            Debug.LogError("[档案条目显示] 加载告示预制体失败");
            return;
        }

        var 界面管理器 = FindObjectOfType<界面管理器_韩>();
        var tips = Instantiate(prefab, 界面管理器.transform);
        bool 确认删除 = await tips.GetComponent<告示_茉>().等待确认显示("删除档案", "是否确认删除该档案？\n该档案将移动到删除档案录。");
        Destroy(tips);

        if (!确认删除)
        {
            Debug.Log("[档案条目显示] 用户取消删除档案");
            return;
        }

        // 取当前档案
        var 档案录 = this.GetModel<档案录>();
        var 当前档案列表 = 档案录.GetDataList();
        if (当前档案列表 == null || 当前索引 < 0 || 当前索引 >= 当前档案列表.Count)
        {
            Debug.LogError($"[档案条目显示] 删除本条目失败：索引越界 索引={当前索引}, 列表长度={当前档案列表?.Count ?? -1}");
            return;
        }

        var 当前档案 = 当前档案列表[当前索引];

        // 1. 从档案录中删除
        this.SendCommand(new 删记于录(当前档案, "档案录"));

        // 2. 记录到 删除档案录 中
        this.SendCommand(new 增记于录(当前档案, "删除档案录"));

        // 3. 删除当前UI条目
        Destroy(gameObject);
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
