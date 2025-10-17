using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NovaSamples.UIControls;
using QFramework;
using UnityEngine;

public class 量表谈话栏_茉 : 界面基础_韩
{
    // Start is called before the first frame update
    [SerializeField] Button 返回按钮;
    List<选项组性_茉> 选项组性群;
    [SerializeField] Button 提交按钮;
    [SerializeField] Button 取消按钮;
    
    // 标记Start是否已执行完成
    private bool 是否已初始化 = false;
    
    void Start()
    {
        // 获取所有选项组
        选项组性群 = FindObjectsOfType<选项组性_茉>().ToList();

        返回按钮.OnClicked.AddListener(() =>
        {
            返回按钮点击().Forget();
        });
        提交按钮.OnClicked.AddListener(() =>
        {
            提交按钮点击().Forget();
        });
        取消按钮.OnClicked.AddListener(() =>
        {
            取消按钮点击().Forget();
        });
        
        // 标记初始化完成
        是否已初始化 = true;
    }

    protected override void OnShow()
    {
        base.OnShow();

        // 等待初始化完成后再还原选项组
        等待初始化后还原().Forget();
    }

    /// <summary>
    /// 等待Start初始化完成后还原选项组选择
    /// </summary>
    private async UniTaskVoid 等待初始化后还原()
    {
        // 使用WaitUntil等待初始化完成
        await UniTask.WaitUntil(() => 是否已初始化);

        // 初始化完成后，还原选项组选择
        还原选项组选择();
    }

    /// <summary>
    /// 从当前档案还原选项组的选择
    /// </summary>
    private void 还原选项组选择()
    {
        // 此时Start已执行完成，选项组性群应该已初始化
        if (选项组性群 == null || 选项组性群.Count == 0)
        {
            Debug.LogWarning("选项组性群未找到");
            return;
        }

        // 获取当前档案
        var 档案录 = this.GetModel<档案录>();
        var 当前档案 = 档案录?.当前档案;

        if (当前档案 == null)
        {
            Debug.Log("当前档案为空，无法还原选项组选择");
            return;
        }

        // 检查是否有谈话记录
        if (当前档案.谈话记录 == null || 当前档案.谈话记录.选项组选择 == null || 当前档案.谈话记录.选项组选择.Count == 0)
        {
            Debug.Log("当前档案没有保存的谈话记录，显示空白选项");
            // 清空所有选项组的选择
            foreach (var 选项组 in 选项组性群)
            {
                if (选项组 != null)
                {
                    选项组.清除所有选择();
                }
            }
            return;
        }

        // 还原每个选项组的选择
        foreach (var 选项组 in 选项组性群)
        {
            if (选项组 == null) continue;

            string 组名 = string.IsNullOrEmpty(选项组.选项组名称) ? 选项组.name : 选项组.选项组名称;

            // 检查是否有保存的选择
            if (当前档案.谈话记录.选项组选择.ContainsKey(组名))
            {
                int 保存的索引 = 当前档案.谈话记录.选项组选择[组名];
                选项组.设置选中选项(保存的索引);
                Debug.Log($"还原选项组 [{组名}] 的选择，索引: {保存的索引}");
            }
            else
            {
                // 如果没有保存的选择，清空该选项组
                选项组.清除所有选择();
            }
        }

        Debug.Log("选项组选择已还原");
    }

    async UniTaskVoid 提交按钮点击()
    {
        // 检查所有选项组是否都已选择
        var 未选择的选项组 = new List<string>();

        foreach (var 选项组 in 选项组性群)
        {
            if (选项组.当前选中选项 == null)
            {
                string 组名 = string.IsNullOrEmpty(选项组.选项组名称) ? 选项组.name : 选项组.选项组名称;
                未选择的选项组.Add(组名);
            }
        }

        // 如果有未选择的选项组，显示提示
        if (未选择的选项组.Count > 0)
        {
            var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
            var 告示实例 = Instantiate(告示, FindObjectOfType<界面管理器_韩>().transform);
            var 告示组件 = 告示实例.GetComponent<告示_茉>();

            string 提示信息 = "请完成全部选项组后保存";
            await 告示组件.等待确认显示("提示", 提示信息);

            Destroy(告示实例.gameObject, 0.1f);
            return;
        }

        // 所有选项组都已选择，继续提交流程
        var 确认告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 确认告示实例 = Instantiate(确认告示, FindObjectOfType<界面管理器_韩>().transform);
        var 确认告示组件 = 确认告示实例.GetComponent<告示_茉>();
        var 确认 = await 确认告示组件.等待确认显示("提示", "确认保存并返回选择界面");
        if (确认)
        {
            // 保存逻辑
            保存选择结果();

            // // 标记谈话记录为完成
            // var 量表评估显示 = FindObjectOfType<量表评估显示>();
            // if (量表评估显示 != null)
            // {
            //     量表评估显示.标记记录完成("谈话记录");
            // }

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
            保存未完成记录();
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
        Debug.Log("谈话记录已临时保存（未完成状态）");
    }

    /// <summary>
    /// 保存所有选项组的选择结果（完整提交，评估状态为完成）
    /// </summary>
    private void 保存选择结果()
    {
        保存记录(评估状态.评估完成);
        Debug.Log("谈话记录已完整保存（完成状态）");
    }

    /// <summary>
    /// 保存记录的通用方法
    /// </summary>
    /// <param name="状态">评估状态</param>
    private void 保存记录(评估状态 状态)
    {
        // 获取当前档案
        var 档案录 = this.GetModel<档案录>();
        var 当前档案 = 档案录.当前档案;

        if (当前档案 == null)
        {
            Debug.LogError("当前档案为空，无法保存谈话记录");
            return;
        }

        // 初始化谈话记录
        if (当前档案.谈话记录 == null)
        {
            当前档案.谈话记录 = new 谈话记录();
        }

        // 清空之前的选择记录
        当前档案.谈话记录.选项组选择.Clear();

        // 构建谈话内容并保存选项组选择
        string 谈话内容 = "";
        foreach (var 选项组 in 选项组性群)
        {
            if (选项组.当前选中选项 != null)
            {
                string 组名 = string.IsNullOrEmpty(选项组.选项组名称) ? 选项组.name : 选项组.选项组名称;
                string 选中项名称 = 选项组.当前选中名称;
                int 选中索引 = 选项组.当前选中索引;

                // 保存到字典中
                当前档案.谈话记录.选项组选择[组名] = 选中索引;

                谈话内容 += $"{组名}: {选中项名称}; ";
                Debug.Log($"选项组 [{组名}] 选择了: {选中项名称} (索引: {选中索引})");
            }
        }

        当前档案.谈话记录.谈话时间 = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        当前档案.谈话记录.谈话人员 = 当前档案.评估人员; // 使用档案中的评估人员
        当前档案.谈话记录.谈话内容 = 谈话内容;
        当前档案.谈话记录.评估时间 = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        当前档案.谈话记录.是否完成 = 状态; // 使用传入的评估状态

        // 保存档案更改
        this.SendCommand(new 改记于录(当前档案, 当前档案, "档案录"));

        Debug.Log($"谈话记录已保存: {谈话内容}，状态: {状态}");
    }
}

