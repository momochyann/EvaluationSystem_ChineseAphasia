using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NovaSamples.UIControls;
using QFramework;
using UnityEngine;
using Nova;

public class 量表命名栏_茉 : 界面基础_韩
{
    // 栏目组
    List<栏目组性_茉> 栏目组性群;
    
    // 列名输入
    [SerializeField] TextField 列名输入;
    
    // 按钮
    [SerializeField] NovaSamples.UIControls.Button 提交按钮;
    [SerializeField] NovaSamples.UIControls.Button 取消按钮;
    [SerializeField] NovaSamples.UIControls.Button 返回按钮;
    
    // 标记Start是否已执行完成
    private bool 是否已初始化 = false;
    
    void Start()
    {
        栏目组性群 = GetComponentsInChildren<栏目组性_茉>().ToList();
        
        // 订阅栏目组总分变化事件
        栏目组性_茉.总分变化事件 += 处理栏目组总分变化;
        
        // 订阅按钮点击事件
        if (提交按钮 != null)
        {
            提交按钮.OnClicked.AddListener(() =>
            {
                提交按钮点击().Forget();
            });
        }
        
        if (取消按钮 != null)
        {
            取消按钮.OnClicked.AddListener(() =>
            {
                取消按钮点击().Forget();
            });
        }
        
        if (返回按钮 != null)
        {
            返回按钮.OnClicked.AddListener(() =>
            {
                返回按钮点击().Forget();
            });
        }
        
        // 标记初始化完成
        是否已初始化 = true;
    }
    
    void OnDestroy()
    {
        // 取消订阅事件，避免内存泄漏
        栏目组性_茉.总分变化事件 -= 处理栏目组总分变化;
    }

    protected override void OnShow()
    {
        base.OnShow();

        // 等待初始化完成后再还原栏目组
        等待初始化后还原().Forget();
    }

    /// <summary>
    /// 等待Start初始化完成后还原栏目组选择
    /// </summary>
    private async UniTaskVoid 等待初始化后还原()
    {
        // 使用WaitUntil等待初始化完成
        await UniTask.WaitUntil(() => 是否已初始化);

        // 初始化完成后，还原栏目组选择
        还原栏目组选择();
    }

    /// <summary>
    /// 还原栏目组选择
    /// </summary>
    private void 还原栏目组选择()
    {
        var 当前档案 = this.GetModel<档案录>().当前档案;
        if (当前档案?.命名记录?.选项组选择 != null)
        {
            Debug.Log($"[量表命名栏] 开始还原栏目组选择，共 {栏目组性群.Count} 个栏目组");
            
            foreach (var 栏目组 in 栏目组性群)
            {
                if (栏目组 == null) continue;
                
                栏目组.设置选项组选择(当前档案.命名记录.选项组选择);
                Debug.Log($"[量表命名栏] 还原栏目组 [{栏目组.维度名称}] 的选择");
            }
            
            // 还原列名输入
            if (!string.IsNullOrEmpty(当前档案.命名记录.列名))
            {
                列名输入.Text = 当前档案.命名记录.列名;
                Debug.Log($"[量表命名栏] 还原列名: {当前档案.命名记录.列名}");
            }
        }
        else
        {
            Debug.Log("[量表命名栏] 没有找到保存的选项组选择记录，清除所有选择");
            
            // 清除所有选择
            foreach (var 栏目组 in 栏目组性群)
            {
                if (栏目组 != null)
                {
                    栏目组.清除所有选择();
                }
            }
            
            // 清空列名输入
            列名输入.Text = "";
        }
    }

    /// <summary>
    /// 处理栏目组总分变化
    /// </summary>
    /// <param name="栏目组">发生变化的栏目组</param>
    private void 处理栏目组总分变化(栏目组性_茉 栏目组)
    {
        // 栏目组会自动处理总分计算和显示，这里可以添加其他逻辑
        Debug.Log($"[量表命名栏] 栏目组 [{栏目组.维度名称}] 总分变化: {栏目组.当前总分}");
    }

    /// <summary>
    /// 检查是否所有维度都已完成
    /// </summary>
    /// <returns>是否全部完成</returns>
    private bool 检查是否全部完成()
    {
        // 检查列名是否已输入
        if (string.IsNullOrEmpty(列名输入.Text))
        {
            Debug.Log("[量表命名栏] 列名未输入");
            return false;
        }
        
        // 检查所有栏目组是否都已完成选择
        foreach (var 栏目组 in 栏目组性群)
        {
            if (栏目组 == null) continue;
            
            if (!栏目组.是否全部已选择)
            {
                Debug.Log($"[量表命名栏] 栏目组 [{栏目组.维度名称}] 未完成选择");
                return false;
            }
        }
        
        return true;
    }

    async UniTaskVoid 提交按钮点击()
    {
        // 检查是否全部完成
        if (!检查是否全部完成())
        {
            var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
            var 告示实例 = Instantiate(告示, FindObjectOfType<界面管理器_韩>().transform);
            var 告示组件 = 告示实例.GetComponent<告示_茉>();
            
            string 提示信息 = "请完成全部命名评估后保存";
            await 告示组件.等待确认显示("提示", 提示信息);
            
            Destroy(告示实例.gameObject, 0.1f);
            return;
        }

        // 所有评估都已完成，继续提交流程
        var 确认告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 确认告示实例 = Instantiate(确认告示, FindObjectOfType<界面管理器_韩>().transform);
        var 确认告示组件 = 确认告示实例.GetComponent<告示_茉>();
        var 确认 = await 确认告示组件.等待确认显示("提示", "确认保存并返回选择界面");
        
        if (确认)
        {
            // 保存逻辑
            保存命名结果();
            
            // 标记命名记录为完成
            var 量表评估显示 = FindObjectOfType<量表评估显示>();
            if (量表评估显示 != null)
            {
                量表评估显示.标记记录完成("命名记录");
            }
            
            FindObjectOfType<界面管理器_韩>().显示界面("量表评估显示");
        }
        
        Destroy(确认告示实例.gameObject, 0.1f);
    }
    
    async UniTaskVoid 返回按钮点击()
    {
        var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 告示实例 = Instantiate(告示, FindObjectOfType<界面管理器_韩>().transform);
        var 告示组件 = 告示实例.GetComponent<告示_茉>();
        var 确认 = await 告示组件.等待确认显示("提示", "是否保存当前填写的记录？");
        
        if (确认)
        {
            // 保存未完成的记录
            if (检查是否全部完成())
            {
                保存命名结果();
            }
            else
            {
                保存未完成记录();
            }
        }
        
        // 无论是否保存，都返回量表评估显示界面
        FindObjectOfType<界面管理器_韩>().显示界面("量表评估显示");
        Destroy(告示实例.gameObject, 0.1f);
    }

    async UniTaskVoid 取消按钮点击()
    {
        var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 告示实例 = Instantiate(告示, FindObjectOfType<界面管理器_韩>().transform);
        var 告示组件 = 告示实例.GetComponent<告示_茉>();
        var 确认 = await 告示组件.等待确认显示("提示", "确认取消并返回？");
        
        if (确认)
        {
            FindObjectOfType<界面管理器_韩>().显示界面("量表评估显示");
        }
        
        Destroy(告示实例.gameObject, 0.1f);
    }

    /// <summary>
    /// 保存未完成的记录（临时保存，评估状态为未完成）
    /// </summary>
    private void 保存未完成记录()
    {
        保存记录(评估状态.未完成);
        Debug.Log("命名记录已临时保存（未完成状态）");
    }

    /// <summary>
    /// 保存命名评估结果（完整提交，评估状态为完成）
    /// </summary>
    private void 保存命名结果()
    {
        保存记录(评估状态.评估完成);
        Debug.Log("命名记录已完整保存（完成状态）");
    }

    /// <summary>
    /// 保存记录
    /// </summary>
    /// <param name="状态">评估状态</param>
    private void 保存记录(评估状态 状态)
    {
        var 当前档案 = this.GetModel<档案录>().当前档案;
        if (当前档案 == null)
        {
            Debug.LogError("[量表命名栏] 当前档案为空，无法保存");
            return;
        }
        
        // 保存所有栏目组的选项组选择
        当前档案.命名记录.选项组选择.Clear();
        foreach (var 栏目组 in 栏目组性群)
        {
            if (栏目组 == null) continue;
            
            var 栏目组选择 = 栏目组.获取选项组选择();
            foreach (var 选择项 in 栏目组选择)
            {
                当前档案.命名记录.选项组选择[选择项.Key] = 选择项.Value;
                Debug.Log($"[量表命名栏] 保存选项组 [{选择项.Key}] 选择: {选择项.Value}");
            }
        }
        
        // 保存列名
        当前档案.命名记录.列名 = 列名输入.Text;
        
        // 保存各维度总分
        foreach (var 栏目组 in 栏目组性群)
        {
            if (栏目组 == null) continue;
            
            switch (栏目组.维度名称)
            {
                case "词命名":
                    当前档案.命名记录.词命名总分 = 栏目组.当前总分;
                    break;
                case "颜色识别":
                    当前档案.命名记录.颜色识别总分 = 栏目组.当前总分;
                    break;
                case "颜色命名":
                    当前档案.命名记录.颜色命名总分 = 栏目组.当前总分;
                    break;
                case "反应性命名":
                    当前档案.命名记录.反应性命名总分 = 栏目组.当前总分;
                    break;
            }
            
            Debug.Log($"[量表命名栏] 保存 {栏目组.维度名称} 总分: {栏目组.当前总分}");
        }
        
        // 设置评估状态
        当前档案.命名记录.是否完成 = 状态;
        当前档案.命名记录.评估时间 = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        
        // 保存到数据库
        this.SendCommand(new 改记于录(当前档案, 当前档案, "档案录"));
        
        Debug.Log($"[量表命名栏] 保存命名记录完成，状态: {状态}");
    }
}