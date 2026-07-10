using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;
using Nova;
using QFramework;
using Cysharp.Threading.Tasks;

/// <summary>
/// 评估模板选择界面 - 主界面，包含综合评价和指导建议两个输入区域
/// </summary>
public class 评估模板选择界面 : 界面基础_韩
{
    [Header("综合评价区域")]
    [SerializeField] private TextField 综合评价输入框;
    [SerializeField] private Button 综合评价模板按钮;

    [Header("指导建议区域")]
    [SerializeField] private TextField 指导建议输入框;
    [SerializeField] private Button 指导建议模板按钮;

    [Header("操作按钮")]
    [SerializeField] private Button 保存按钮;
    [SerializeField] private Button 返回按钮;
    [SerializeField] private Button 返回按钮2;
     [SerializeField] private Button 返回按钮3;

    [Header("模板设置界面")]
    [SerializeField] private 评估模板设置显示 模板设置界面;

    private 档案 当前档案;

    void Start()
    {
        // 综合评价模板按钮
        if (综合评价模板按钮 != null)
        {
            综合评价模板按钮.OnClicked.AddListener(() =>
            {
                打开模板设置界面(评估模板类型.综合评价);
            });
        }

        // 指导建议模板按钮
        if (指导建议模板按钮 != null)
        {
            指导建议模板按钮.OnClicked.AddListener(() =>
            {
                打开模板设置界面(评估模板类型.指导建议);
            });
        }

        // 保存按钮
        if (保存按钮 != null)
        {
            保存按钮.OnClicked.AddListener(() =>
            {
                保存评价和建议().Forget();
            });
        }

        // 返回按钮
        if (返回按钮 != null)
        {
            返回按钮.OnClicked.AddListener(() =>
            {
                返回上一界面().Forget();
            });
        }
        if (返回按钮2 != null)
        {
            返回按钮2.OnClicked.AddListener(() =>
            {
                返回上一界面().Forget();
            });
        }
        if (返回按钮3 != null)
        {
            返回按钮3.OnClicked.AddListener(() =>
            {
                返回上一界面().Forget();
            });
        }
    }

    protected override void OnShow()
    {
        base.OnShow();

        // 获取当前档案
        var 档案录 = this.GetModel<档案录>();
        当前档案 = 档案录.当前档案;

        if (当前档案 == null)
        {
            Debug.LogWarning("当前档案为空");
            return;
        }

        // 加载已有的评价和建议
        if (综合评价输入框 != null)
            综合评价输入框.Text = 当前档案.综合评价 ?? "";

        if (指导建议输入框 != null)
            指导建议输入框.Text = 当前档案.指导建议 ?? "";
    }

    /// <summary>
    /// 打开模板设置界面
    /// </summary>
    /// <param name="模板类型">模板类型</param>
    private void 打开模板设置界面(评估模板类型 模板类型)
    {
        if (模板设置界面 == null)
        {
            Debug.LogWarning("模板设置界面未配置");
            return;
        }

        // 初始化模板设置界面，传入选择回调
        模板设置界面.初始化界面(模板类型, (选中模板) =>
        {
            应用选中的模板(模板类型, 选中模板);
        });

        // 显示模板设置界面
        模板设置界面.显示界面();
    }

    /// <summary>
    /// 应用选中的模板
    /// </summary>
    /// <param name="模板类型">模板类型</param>
    /// <param name="模板">选中的模板</param>
    private void 应用选中的模板(评估模板类型 模板类型, 评估模板 模板)
    {
        if (模板 == null)
            return;

        if (模板类型 == 评估模板类型.综合评价)
        {
            if (综合评价输入框 != null)
            {
                // 如果输入框已有内容，追加模板内容；否则直接使用模板内容
                if (string.IsNullOrEmpty(综合评价输入框.Text))
                {
                    综合评价输入框.Text = 模板.模板内容;
                }
                else
                {
                    综合评价输入框.Text = 模板.模板内容; // 直接替换
                }
            }
        }
        else
        {
            if (指导建议输入框 != null)
            {
                if (string.IsNullOrEmpty(指导建议输入框.Text))
                {
                    指导建议输入框.Text = 模板.模板内容;
                }
                else
                {
                    指导建议输入框.Text = 模板.模板内容; // 直接替换
                }
            }
        }

        Debug.Log($"已应用模板: {模板.模板名称}");
    }

    /// <summary>
    /// 保存评价和建议
    /// </summary>
    private async UniTaskVoid 保存评价和建议()
    {
        if (当前档案 == null)
        {
            await 显示提示("提示", "当前档案为空，无法保存");
            return;
        }

        // 显示确认对话框
        bool 确认保存 = await 显示确认("保存确认", "确定要保存综合评价和指导建议吗？");
        if (!确认保存)
        {
            return; // 用户取消保存
        }

        // 获取输入框内容
        string 综合评价内容 = 综合评价输入框 != null ? 综合评价输入框.Text : "";
        string 指导建议内容 = 指导建议输入框 != null ? 指导建议输入框.Text : "";

        // 创建新的档案对象用于更新
        var 更新档案 = 当前档案;
        更新档案.综合评价 = 综合评价内容;
        更新档案.指导建议 = 指导建议内容;
        更新档案.评估状态 = 评估状态.评估完成;
        更新档案.评估时间 = System.DateTime.Now.ToString("yyyy-MM-dd");

        await 显示提示("提示", "综合评价和指导建议已保存，进入报告预显示");
        // 保存到档案录
        this.SendCommand(new 改记于录(当前档案, 更新档案, "档案录"));
        var 界面管理器 = FindObjectOfType<界面管理器_韩>();
        界面管理器.显示界面("档案管理报告界面");
        await UniTask.WaitUntil(() => 界面管理器._currentPanel != null && 界面管理器._currentPanel.name.Contains("档案管理报告界面"));
        界面管理器._currentPanel.显示信息?.Invoke(更新档案);

        // 如果是电脑端，切换左目录到档案管理
        if (Is电脑端())
        {
            切换左目录到档案管理();
        }

        // 界面管理器._currentPanel.GetComponent<档案管理报告界面显示>().On取消确认 += () =>
        // {
        //     // Destroy(界面管理器._currentPanel.gameObject);
        //     FindObjectOfType<界面管理器_韩>().显示界面("量表评估模板选择");
        // };

    }

    /// <summary>
    /// 返回上一界面
    /// </summary>
    private async UniTaskVoid 返回上一界面()
    {
        // 检查是否有未保存的内容
        if (当前档案 != null)
        {
            string 当前综合评价 = 综合评价输入框 != null ? 综合评价输入框.Text : "";
            string 当前指导建议 = 指导建议输入框 != null ? 指导建议输入框.Text : "";

            // 检查是否有修改
            bool 有修改 = (当前综合评价 != (当前档案.综合评价 ?? "")) ||
                        (当前指导建议 != (当前档案.指导建议 ?? ""));

            if (有修改)
            {
                // 显示确认对话框
                bool 确认退出 = await 显示确认("退出确认", "有未保存的内容，确定要退出吗？");
                if (!确认退出)
                {
                    return; // 用户取消退出
                }
            }
        }
        FindObjectOfType<界面管理器_韩>().显示界面("量表评估显示");
    }

    /// <summary>
    /// 显示提示对话框
    /// </summary>
    private async UniTask 显示提示(string 标题, string 内容)
    {
        var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 告示实例 = Instantiate(告示, FindObjectOfType<界面管理器_韩>().transform);
        await 告示实例.GetComponent<告示_茉>().等待确认显示(标题, 内容);
        Destroy(告示实例.gameObject, 0.1f);
    }

    /// <summary>
    /// 显示确认对话框
    /// </summary>
    private async UniTask<bool> 显示确认(string 标题, string 内容)
    {
        var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 告示实例 = Instantiate(告示, FindObjectOfType<界面管理器_韩>().transform);
        var 确认 = await 告示实例.GetComponent<告示_茉>().等待确认显示(标题, 内容);
        Destroy(告示实例.gameObject, 0.1f);
        return 确认;
    }

    /// <summary>
    /// 检测是否为电脑端
    /// </summary>
    private bool Is电脑端()
    {
        RuntimePlatform platform = Application.platform;
        return platform == RuntimePlatform.WindowsPlayer ||
               platform == RuntimePlatform.WindowsEditor ||
               platform == RuntimePlatform.OSXPlayer ||
               platform == RuntimePlatform.OSXEditor ||
               platform == RuntimePlatform.LinuxPlayer ||
               platform == RuntimePlatform.LinuxEditor;
    }

    /// <summary>
    /// 切换左目录到档案管理
    /// </summary>
    private void 切换左目录到档案管理()
    {
        var 左帜象 = FindObjectOfType<左帜象_茉>();
        if (左帜象 == null)
        {
            Debug.LogWarning("[评估模板选择] 未找到左帜象组件");
            return;
        }

        var 左目录选择 = 左帜象.GetComponent<左目录选择_韩>();
        if (左目录选择 == null || 左目录选择.目录列表 == null)
        {
            Debug.LogWarning("[评估模板选择] 未找到左目录选择组件或目录列表为空");
            return;
        }

        // 查找名为"档案管理"的按钮
        foreach (var 按钮 in 左目录选择.目录列表)
        {
            if (按钮 != null && 按钮.name == "档案管理")
            {
                // 调用显切象方法切换选中状态
                左帜象.显切象(按钮);
                Debug.Log("[评估模板选择] 已切换左目录到档案管理");
                return;
            }
        }

        Debug.LogWarning("[评估模板选择] 未找到名为'档案管理'的目录按钮");
    }
}
