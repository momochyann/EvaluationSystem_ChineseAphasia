using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nova;
using NovaSamples.UIControls;
using UnityEngine;
using Cysharp.Threading.Tasks;
using QFramework;

public class 量表复述栏_茉 : 界面基础_韩
{
    // Start is called before the first frame update

    List<选项组性_茉> 选项组性群;
    [SerializeField] TextBlock 词复述总分;
    [SerializeField] TextBlock 表分择性总分;
    List<表分择性_茉> 表分择性群;
    [SerializeField] Button 提交按钮;
    [SerializeField] Button 取消按钮;
    [SerializeField] Button 返回按钮;
    
    // 标记Start是否已执行完成
    private bool 是否已初始化 = false;
    
    void Start()
    {
        选项组性群 = FindObjectsOfType<选项组性_茉>().ToList();
        表分择性群 = FindObjectsOfType<表分择性_茉>().ToList();
        
        // 订阅选择变化事件
        选项组性_茉.选择变化事件 += 更新词复述总分;
        表分择性_茉.表分选择变化事件 += 更新表分择性总分;
        
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
        
        // 初始化显示
        更新词复述总分();
        更新表分择性总分();
        
        // 标记初始化完成
        是否已初始化 = true;
    }
    
    void OnDestroy()
    {
        // 取消订阅事件，避免内存泄漏
        选项组性_茉.选择变化事件 -= 更新词复述总分;
        表分择性_茉.表分选择变化事件 -= 更新表分择性总分;
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
    /// 从当前档案还原选项组和表分择性的选择
    /// </summary>
    private void 还原选项组选择()
    {
        // 此时Start已执行完成
        if ((选项组性群 == null || 选项组性群.Count == 0) && 
            (表分择性群 == null || 表分择性群.Count == 0))
        {
            Debug.LogWarning("选项组性群和表分择性群都未找到");
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

        // 检查是否有复述记录
        if (当前档案.复述记录 == null)
        {
            Debug.Log("当前档案没有复述记录，显示空白选项");
            清空所有选择();
            return;
        }

        // 还原选项组选择
        if (当前档案.复述记录.选项组选择 != null && 当前档案.复述记录.选项组选择.Count > 0)
        {
            foreach (var 选项组 in 选项组性群)
            {
                if (选项组 == null) continue;

                string 组名 = string.IsNullOrEmpty(选项组.选项组名称) ? 选项组.name : 选项组.选项组名称;

                if (当前档案.复述记录.选项组选择.ContainsKey(组名))
                {
                    int 保存的索引 = 当前档案.复述记录.选项组选择[组名];
                    选项组.设置选中选项(保存的索引);
                    Debug.Log($"还原选项组 [{组名}] 的选择，索引: {保存的索引}");
                }
                else
                {
                    选项组.清除所有选择();
                }
            }
        }
        else
        {
            // 清空选项组
            foreach (var 选项组 in 选项组性群)
            {
                if (选项组 != null)
                {
                    选项组.清除所有选择();
                }
            }
        }

        // 还原表分择性选择
        if (当前档案.复述记录.表分择性选择 != null && 当前档案.复述记录.表分择性选择.Count > 0)
        {
            foreach (var 表分择性 in 表分择性群)
            {
                if (表分择性 == null) continue;

                string 组名 = 表分择性.name;

                if (当前档案.复述记录.表分择性选择.ContainsKey(组名))
                {
                    int 保存的分数 = 当前档案.复述记录.表分择性选择[组名];
                    表分择性.设置选中分数(保存的分数);
                    Debug.Log($"还原表分择性 [{组名}] 的选择，分数: {保存的分数}");
                }
                else
                {
                    表分择性.清除选择();
                }
            }
        }
        else
        {
            // 清空表分择性
            foreach (var 表分择性 in 表分择性群)
            {
                if (表分择性 != null)
                {
                    表分择性.清除选择();
                }
            }
        }

        // 更新总分显示
        更新词复述总分();
        更新表分择性总分();

        Debug.Log("复述记录已还原");
    }

    /// <summary>
    /// 清空所有选择
    /// </summary>
    private void 清空所有选择()
    {
        foreach (var 选项组 in 选项组性群)
        {
            if (选项组 != null)
            {
                选项组.清除所有选择();
            }
        }

        foreach (var 表分择性 in 表分择性群)
        {
            if (表分择性 != null)
            {
                表分择性.清除选择();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 移除每帧检查，改为事件驱动
    }
    
    async UniTaskVoid 提交按钮点击()
    {
        // 检查词复述是否完成
        bool 词复述完成 = !词复述总分.Text.Equals("待选择完") && !词复述总分.Text.Equals("无选项组");
        
        // 检查表分择性是否完成
        bool 表分择性完成 = !表分择性总分.Text.Equals("待选择完") && !表分择性总分.Text.Equals("无表分择性组件");
        
        // 如果有未完成的部分，显示提示
        if (!词复述完成 || !表分择性完成)
        {
            var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
            var 告示实例 = Instantiate(告示, FindObjectOfType<界面管理器_韩>().transform);
            var 告示组件 = 告示实例.GetComponent<告示_茉>();
            
            string 提示信息 = "请完成全部复述评估后保存";
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
            保存复述结果();
            
            // 标记复述记录为完成
            var 量表评估显示 = FindObjectOfType<量表评估显示>();
            if (量表评估显示 != null)
            {
                量表评估显示.标记记录完成("复述记录");
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
        Debug.Log("复述记录已临时保存（未完成状态）");
    }

    /// <summary>
    /// 保存复述评估结果（完整提交，评估状态为完成）
    /// </summary>
    private void 保存复述结果()
    {
        保存记录(评估状态.评估完成);
        Debug.Log("复述记录已完整保存（完成状态）");
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
            Debug.LogError("当前档案为空，无法保存复述记录");
            return;
        }
        
        // 初始化复述记录
        if (当前档案.复述记录 == null)
        {
            当前档案.复述记录 = new 复述记录();
        }
        
        // 清空之前的选择记录
        当前档案.复述记录.选项组选择.Clear();
        当前档案.复述记录.表分择性选择.Clear();
        
        // 保存选项组选择
        foreach (var 选项组 in 选项组性群)
        {
            if (选项组.当前选中选项 != null)
            {
                string 组名 = string.IsNullOrEmpty(选项组.选项组名称) ? 选项组.name : 选项组.选项组名称;
                int 选中索引 = 选项组.当前选中索引;
                当前档案.复述记录.选项组选择[组名] = 选中索引;
            }
        }
        
        // 保存表分择性选择
        foreach (var 表分择性 in 表分择性群)
        {
            if (表分择性 != null)
            {
                int 选中分数 = 表分择性.获取选择的分数();
                if (选中分数 != -1)
                {
                    string 组名 = 表分择性.name;
                    当前档案.复述记录.表分择性选择[组名] = 选中分数;
                }
            }
        }
        
        // 获取两个总分
        string 词复述分数文本 = 词复述总分.Text;
        string 表分择性分数文本 = 表分择性总分.Text;
        
        // 尝试解析为整数
        int.TryParse(词复述分数文本, out int 词复述分数);
        int.TryParse(表分择性分数文本, out int 表分择性分数);
        
        当前档案.复述记录.词复述总分 = 词复述分数;
        当前档案.复述记录.句复述总分 = 表分择性分数; // 使用句复述总分字段存储表分择性分数
        当前档案.复述记录.评估时间 = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        当前档案.复述记录.是否完成 = 状态; // 使用传入的评估状态
        
        // 保存档案更改
        this.SendCommand(new 改记于录(当前档案, 当前档案, "档案录"));
        
        Debug.Log($"复述记录已保存 - 词复述总分: {词复述分数}, 表分择性总分: {表分择性分数}，状态: {状态}");
    }
    
    /// <summary>
    /// 更新词复述总分
    /// </summary>
    private void 更新词复述总分()
    {
        if (选项组性群 == null || 选项组性群.Count == 0)
        {
            词复述总分.Text = "无选项组";
            return;
        }
        
        // 检查是否所有选项组都已选择
        bool 全部已选择 = true;
        int 总分 = 0;
        
        foreach (var 选项组 in 选项组性群)
        {
            if (选项组.当前选中索引 == -1)
            {
                全部已选择 = false;
                break;
            }
            else
            {
                总分 += 选项组.当前选中索引;
            }
        }
        
        // 更新显示文本
        if (全部已选择)
        {
            词复述总分.Text = 总分.ToString();
        }
        else
        {
            词复述总分.Text = "待选择完";
        }
    }
    
    /// <summary>
    /// 更新表分择性总分
    /// </summary>
    private void 更新表分择性总分()
    {
        if (表分择性群 == null || 表分择性群.Count == 0)
        {
            表分择性总分.Text = "无表分择性组件";
            return;
        }
        
        // 检查是否所有表分择性都已选择
        bool 全部已选择 = true;
        int 总分 = 0;
        
        foreach (var 表分择性 in 表分择性群)
        {
            if (表分择性 == null) continue;
            
            int 当前分数 = 表分择性.获取选择的分数();
            if (当前分数 == -1)
            {
                全部已选择 = false;
                break;
            }
            else
            {
                总分 += 当前分数;
            }
        }
        
        // 更新显示文本
        if (全部已选择)
        {
            表分择性总分.Text = 总分.ToString();
        }
        else
        {
            表分择性总分.Text = "待选择完";
        }
    }
}
