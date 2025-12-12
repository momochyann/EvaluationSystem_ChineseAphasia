using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NovaSamples.UIControls;
using QFramework;
using UnityEngine;

/// <summary>
/// 量表评估界面的基类 - 统一使用栏目组方式
/// </summary>
public abstract class 量表评估基类_茉 : 界面基础_韩
{
    [SerializeField] protected Button 提交按钮;
    [SerializeField] protected Button 取消按钮;
    [SerializeField] protected Button 返回按钮;

    protected List<栏目组性_茉> 栏目组性群;
    private bool 是否已初始化 = false;

    void Start()
    {
        栏目组性群 = GetComponentsInChildren<栏目组性_茉>().ToList();
        栏目组性_茉.总分变化事件 += 处理栏目组变化;
        订阅按钮事件();
        OnStartExtra();
        是否已初始化 = true;
    }

    void OnDestroy()
    {
        栏目组性_茉.总分变化事件 -= 处理栏目组变化;
        OnDestroyExtra();
    }

    protected override void OnShow()
    {
        base.OnShow();
        等待初始化后还原().Forget();
    }

    private async UniTaskVoid 等待初始化后还原()
    {
        await UniTask.WaitUntil(() => 是否已初始化);
        还原选择();
    }

    #region 抽象方法

    protected abstract string 获取记录字段名称();
    protected abstract bool 检查是否全部完成();
    protected abstract string 获取未完成提示信息();

    #endregion

    #region 虚方法

    protected virtual void OnStartExtra() { }
    protected virtual void OnDestroyExtra() { }
    protected virtual void 还原其他数据(object 记录) { }
    protected virtual void 保存额外数据(object 记录) { }
    protected virtual void 处理栏目组变化(栏目组性_茉 栏目组) { }

    #endregion

    #region 还原选择

    protected virtual void 还原选择()
    {
        var 当前档案 = this.GetModel<档案录>().当前档案;
        if (当前档案 == null) return;

        // 每次进入先清空一次，避免上一次用户或上一次记录的残留
        清除所有选择();

        var 记录 = 获取档案记录(当前档案);
        if (记录 == null) return;

        var 选项组选择字段 = 记录.GetType().GetField("选项组选择");
        var 选项组选择 = 选项组选择字段?.GetValue(记录) as Dictionary<string, int>;

        // 尝试获取表分择性选择(如果存在)
        var 表分择性选择字段 = 记录.GetType().GetField("表分择性选择");
        var 表分择性选择 = 表分择性选择字段?.GetValue(记录) as Dictionary<string, int>;
        if (表分择性选择 != null)
        {
            foreach (var item in 表分择性选择)
            {
                Debug.Log($"[量表评估基类] 还原选择: 表分择性选择: {item.Key} = {item.Value}");
            }
        }
        bool 有数据 = (选项组选择 != null && 选项组选择.Count > 0) ||
                     (表分择性选择 != null && 表分择性选择.Count > 0);
        Debug.Log($"[量表评估基类] 还原选择: 有数据: {有数据}");
        还原其他数据(记录);
        if (!有数据) return;

        foreach (var 栏目组 in 栏目组性群)
        {
            if (栏目组 == null) continue;

            // 根据栏目组类型设置对应的选择
            if (栏目组.组类型 == 栏目组类型.选项组性 && 选项组选择 != null)
            {
                栏目组.设置选项组选择(选项组选择);
            }
            else if (栏目组.组类型 == 栏目组类型.表分择性 && 表分择性选择 != null)
            {
                栏目组.设置选项组选择(表分择性选择); // 内部会调用设置表分择性选择
            }
        }

    }

    protected virtual void 清除所有选择()
    {
        if (栏目组性群 != null)
        {
            foreach (var 栏目组 in 栏目组性群)
            {
                栏目组?.清除所有选择();
            }
        }
    }

    #endregion

    #region 按钮事件

    private void 订阅按钮事件()
    {
        提交按钮?.OnClicked.AddListener(() => 提交按钮点击().Forget());
        取消按钮?.OnClicked.AddListener(() => 取消按钮点击().Forget());
        返回按钮?.OnClicked.AddListener(() => 返回按钮点击().Forget());
    }

    protected virtual async UniTaskVoid 提交按钮点击()
    {
        if (!检查是否全部完成())
        {
            await 显示提示("提示", 获取未完成提示信息());
            return;
        }

        if (await 显示确认("提示", "确认保存并返回选择界面"))
        {
            保存记录(评估状态.评估完成);
            FindObjectOfType<界面管理器_韩>().显示界面("量表评估显示");
        }
    }

    protected virtual async UniTaskVoid 返回按钮点击()
    {
        if (await 显示确认("提示", "是否保存当前填写的记录？"))
        {
            // 根据是否全部完成来决定保存状态
            var 保存状态 = 检查是否全部完成() ? 评估状态.评估完成 : 评估状态.未完成;
            保存记录(保存状态);
        }
        FindObjectOfType<界面管理器_韩>().显示界面("量表评估显示");
    }

    protected virtual async UniTaskVoid 取消按钮点击()
    {
        if (await 显示确认("提示", "确认取消并返回？"))
        {
            FindObjectOfType<界面管理器_韩>().显示界面("量表评估显示");
        }
    }

    // 辅助方法：显示提示
    private async UniTask 显示提示(string 标题, string 内容)
    {
        var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 告示实例 = Instantiate(告示, FindObjectOfType<界面管理器_韩>().transform);
        await 告示实例.GetComponent<告示_茉>().等待确认显示(标题, 内容);
        Destroy(告示实例.gameObject, 0.1f);
    }

    // 辅助方法：显示确认
    private async UniTask<bool> 显示确认(string 标题, string 内容)
    {
        var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 告示实例 = Instantiate(告示, FindObjectOfType<界面管理器_韩>().transform);
        var 确认 = await 告示实例.GetComponent<告示_茉>().等待确认显示(标题, 内容);
        Destroy(告示实例.gameObject, 0.1f);
        return 确认;
    }

    #endregion

    #region 保存记录

    protected virtual void 保存记录(评估状态 状态)
    {
        var 当前档案 = this.GetModel<档案录>().当前档案;
        if (当前档案 == null)
        {
            Debug.LogError($"[{获取记录字段名称()}] 当前档案为空");
            return;
        }

        var 记录 = 获取或创建档案记录(当前档案);
        if (记录 == null) return;

        // 保存选项组选择
        var 选项组选择字段 = 记录.GetType().GetField("选项组选择");
        if (选项组选择字段 != null)
        {
            var 选项组选择 = 选项组选择字段.GetValue(记录) as Dictionary<string, int>
                           ?? new Dictionary<string, int>();
            选项组选择.Clear();

            foreach (var 栏目组 in 栏目组性群)
            {
                if (栏目组 == null || 栏目组.组类型 != 栏目组类型.选项组性) continue;
                foreach (var 项 in 栏目组.获取选项组选择())
                {
                    选项组选择[项.Key] = 项.Value;
                }
            }

            选项组选择字段.SetValue(记录, 选项组选择);
        }

        // 保存表分择性选择(如果存在)
        var 表分择性选择字段 = 记录.GetType().GetField("表分择性选择");
        if (表分择性选择字段 != null)
        {
            var 表分择性选择 = 表分择性选择字段.GetValue(记录) as Dictionary<string, int>
                           ?? new Dictionary<string, int>();
            表分择性选择.Clear();

            foreach (var 栏目组 in 栏目组性群)
            {
                if (栏目组 == null || 栏目组.组类型 != 栏目组类型.表分择性) continue;
                foreach (var 项 in 栏目组.获取选项组选择())
                {
                    表分择性选择[项.Key] = 项.Value;
                }
            }

            表分择性选择字段.SetValue(记录, 表分择性选择);
        }

        // 保存额外数据
        保存额外数据(记录);

        // 设置状态和时间
        记录.GetType().GetProperty("是否完成")?.SetValue(记录, 状态);
        记录.GetType().GetProperty("评估时间")?.SetValue(记录, DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

        // 保存到数据库
        this.SendCommand(new 改记于录(当前档案, 当前档案, "档案录"));
        Debug.Log($"[{获取记录字段名称()}] 保存完成，状态: {状态}");
    }

    #endregion

    #region 辅助方法

    protected object 获取档案记录(档案 当前档案)
    {
        return typeof(档案).GetField(获取记录字段名称())?.GetValue(当前档案);
    }

    protected object 获取或创建档案记录(档案 当前档案)
    {
        var 记录字段 = typeof(档案).GetField(获取记录字段名称());
        if (记录字段 == null) return null;

        var 记录 = 记录字段.GetValue(当前档案);
        if (记录 == null)
        {
            记录 = Activator.CreateInstance(记录字段.FieldType);
            记录字段.SetValue(当前档案, 记录);
        }

        return 记录;
    }

    #endregion
}
