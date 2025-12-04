using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;
using Nova;
using QFramework;
using Cysharp.Threading.Tasks;

/// <summary>
/// 评估模板设置显示界面 - 用于添加和选择模板
/// </summary>
public class 评估模板设置显示 : 界面基础_韩
{
    [Header("UI组件")]
    [SerializeField] private TextField 新模板内容输入;
    [SerializeField] private Button 添加模板按钮;
    [SerializeField] private Button 关闭按钮;
    [SerializeField] private Transform 模板列表容器;
    [SerializeField] private GameObject 模板条目预制体;

    [Header("当前设置")]
    private 评估模板类型 当前模板类型;
    private System.Action<评估模板> 选择回调;
    private List<GameObject> 已生成的条目 = new List<GameObject>();

    void Start()
    {
      
        // 添加模板按钮事件
        if (添加模板按钮 != null)
        {
            添加模板按钮.OnClicked.AddListener(() =>
            {
                添加新模板();
            });
        }

        // 关闭按钮事件
        if (关闭按钮 != null)
        {
            关闭按钮.OnClicked.AddListener(() =>
            {
                隐藏界面();
            });
        }
       加载模板条目预制体().Forget();
    }
async UniTaskVoid 加载模板条目预制体()
{
    var 模板条目预制体 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("评估评价模板");
    if (模板条目预制体 != null)
    {
        this.模板条目预制体 = 模板条目预制体;
    }
}
    /// <summary>
    /// 初始化界面
    /// </summary>
    /// <param name="模板类型">模板类型（综合评价/指导建议）</param>
    /// <param name="选择回调">选择模板后的回调</param>
    public void 初始化界面(评估模板类型 模板类型, System.Action<评估模板> 选择回调)
    {
        当前模板类型 = 模板类型;
        this.选择回调 = 选择回调;
        
        // 清空输入框
        if (新模板内容输入 != null)
            新模板内容输入.Text = "";

        // 刷新模板列表
        刷新模板列表();
    }

    /// <summary>
    /// 添加新空模板
    /// </summary>
    private void 添加新模板()
    {
        // 创建空模板
        var 新模板 = new 评估模板
        {
            模板名称 = "新模板",
            模板内容 = 新模板内容输入 != null ? 新模板内容输入.Text.Trim() : "",
            模板类型 = 当前模板类型,
            创建时间 = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            最后修改时间 = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        // 根据类型添加到对应的录
        if (当前模板类型 == 评估模板类型.综合评价)
        {
            this.SendCommand(new 增记于录(新模板, "综合评价模板录"));
        }
        else
        {
            this.SendCommand(new 增记于录(新模板, "指导建议模板录"));
        }

        // 清空输入框
        if (新模板内容输入 != null)
            新模板内容输入.Text = "";

        // 刷新列表
        刷新模板列表();

        Debug.Log("成功添加新模板");
    }

    /// <summary>
    /// 保存模板修改（从模板条目调用）
    /// </summary>
    public void 保存模板修改(评估模板 旧模板, 评估模板 新模板)
    {
        // 更新到录中
        if (当前模板类型 == 评估模板类型.综合评价)
        {
            this.SendCommand(new 改记于录(旧模板, 新模板, "综合评价模板录"));
        }
        else
        {
            this.SendCommand(new 改记于录(旧模板, 新模板, "指导建议模板录"));
        }

        Debug.Log($"模板修改成功: {新模板.模板名称}");
    }

    /// <summary>
    /// 刷新模板列表
    /// </summary>
    private void 刷新模板列表()
    {
        // 清空现有列表
        foreach (var 条目 in 已生成的条目)
        {
            if (条目 != null)
                Destroy(条目);
        }
        已生成的条目.Clear();

        if (模板列表容器 == null || 模板条目预制体 == null)
        {
            Debug.LogWarning("模板列表容器或预制体未配置");
            return;
        }

        // 获取对应类型的模板列表
        List<评估模板> 模板列表 = null;
        if (当前模板类型 == 评估模板类型.综合评价)
        {
            var 综合评价录 = this.GetModel<综合评价模板录>();
            模板列表 = 综合评价录.GetDataList() as List<评估模板>;
        }
        else
        {
            var 指导建议录 = this.GetModel<指导建议模板录>();
            模板列表 = 指导建议录.GetDataList() as List<评估模板>;
        }

        if (模板列表 == null || 模板列表.Count == 0)
        {
            Debug.Log("暂无模板");
            return;
        }

        // 生成模板条目
        foreach (var 模板 in 模板列表)
        {
            var 条目对象 = Instantiate(模板条目预制体, 模板列表容器);
            var 条目显示 = 条目对象.GetComponent<模板条目显示>();

            if (条目显示 != null)
            {
                条目显示.初始化(模板, this, 当前模板类型,
                    (选中模板) => 
                    {
                        // 选择模板回调
                        选择回调?.Invoke(选中模板);
                        隐藏界面();
                    });
            }

            已生成的条目.Add(条目对象);
        }
    }
    public void 删除模板修改(评估模板 模板)
    {
        if (当前模板类型 == 评估模板类型.综合评价)
        {
            this.SendCommand(new 删记于录(模板, "综合评价模板录"));
        }
        else
        {
            this.SendCommand(new 删记于录(模板, "指导建议模板录"));
        }
        // 刷新模板列表
        刷新模板列表();
    }
    protected override void OnShow()
    {
        base.OnShow();
        // 每次显示时刷新列表
        if (模板列表容器 != null)
        {
            刷新模板列表();
        }
    }
}
