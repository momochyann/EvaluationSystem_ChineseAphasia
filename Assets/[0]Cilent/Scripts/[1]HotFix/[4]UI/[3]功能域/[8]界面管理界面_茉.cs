using UnityEngine;
using QFramework;
using NovaSamples.UIControls;
using Cysharp.Threading.Tasks;
using Nova;
using DG.Tweening;
using System;

public class 界面管理界面_茉 : 界面基础_韩
{
    public bool 是否输入框动画= true;
    [Header("输入框")]
    [SerializeField] private TextField 单位名称输入框;
    [SerializeField] private TextField 软件名称输入框;

    [Header("图片选择器")]
    [SerializeField] private 图片选择组件 图片选择器;

    [Header("按钮")]
    [SerializeField] private NovaSamples.UIControls.Button 保存按钮;
    [SerializeField] private NovaSamples.UIControls.Button 取消按钮;

    private 软件配置录 配置录;
    private IWindowTitleBridge 窗口标题桥;
    [SerializeField] public Color inputSetColor;
    private const string 默认窗口标题 = "汉语失语症评测系统";
    bool isFirst = true;
    void Start()
    {
        InitializeUI();
        LoadConfigData();
    }
    void 输入文本框点击(TextField 输入框)
    {
        TextFieldSelector selector = 输入框.GetComponent<TextFieldSelector>();
        selector.OnFocused += () =>
        {
            选中动画(true, 输入框.transform.parent.parent);
        };
        selector.OnFocusLost += () =>
        {
            选中动画(false, 输入框.transform.parent.parent);
        };

    }
    void 选中动画(bool 是否选中, Transform 选中对象)
    {
        if (是否选中)
        {
            if (选中对象.GetComponent<UIBlock2D>().Color.a == 0)
            {
                DOTween.To((value) =>
               {
                   选中对象.GetComponent<UIBlock2D>().Color = new Color(inputSetColor.r, inputSetColor.g, inputSetColor.b, value);
               }, 0, 1, 0.3f);
            }
        }
        else
        {
            if (选中对象.GetComponent<UIBlock2D>().Color.a == 1)
            {
                DOTween.To((value) =>
                {
                    选中对象.GetComponent<UIBlock2D>().Color = new Color(inputSetColor.r, inputSetColor.g, inputSetColor.b, value);
                }, 1, 0, 0.3f);
            }
        }
    }
    private void InitializeUI()
    {
        配置录 = this.GetModel<软件配置录>();
        窗口标题桥 = new WindowTitleBridge();

        // 绑定按钮事件
        保存按钮?.OnClicked.AddListener(保存配置);
        取消按钮?.OnClicked.AddListener(取消操作);
        if(是否输入框动画)
        {
            输入文本框点击(单位名称输入框);
            输入文本框点击(软件名称输入框);
        }
        isFirst = false;
    }
    void OnEnable()
    {
        if (!isFirst)
            LoadConfigData();
    }
    /// <summary>
    /// 加载配置数据到界面
    /// </summary>
    private async void LoadConfigData()
    {
        var config = 配置录.当前配置;

        // 加载文本数据
        if (单位名称输入框 != null)
            单位名称输入框.Text = config.单位名称;
        if (软件名称输入框 != null)
            软件名称输入框.Text = config.软件名称;

        var texture2D = await this.GetUtility<ImagePickerUtility>().LoadImageFromCacheAsync(config.图片缓存名称);
        图片选择器.SetDisplayImage(texture2D);
        // 图片选择器会自动加载上次的图片（如果启用了显示默认图片）
        同步窗口标题(config.软件名称);

        // 设置截图工具的单位名称
        this.GetUtility<IScreenshotPDFUtility>().SetCompanyName(config.单位名称);

        Debug.Log("配置数据已加载到界面");
    }

    /// <summary>
    /// 保存配置
    /// </summary>
    private async void 保存配置()
    {
        try
        {
            var 新配置 = new 软件配置
            {
                单位名称 = 单位名称输入框?.Text ?? "",
                软件名称 = (软件名称输入框?.Text?.Length > 14) ? 软件名称输入框.Text.Substring(0, 14) : (软件名称输入框?.Text ?? ""),
                图片缓存名称 = 图片选择器?.GetCurrentImageCacheName() ?? ""
            };

            // 显示保存成功提示
            bool IsConfig = await 显示提示("是否保存配置");
            if (IsConfig)
            {
                配置录.保存配置(新配置);
                同步窗口标题(新配置.软件名称);

                // 更新截图工具的单位名称
                this.GetUtility<IScreenshotPDFUtility>().SetCompanyName(新配置.单位名称);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"保存配置失败: {e.Message}");
            await 显示提示("保存配置失败");
        }
    }

    /// <summary>
    /// 取消操作
    /// </summary>
    private void 取消操作()
    {
        // 重新加载配置数据，恢复到保存的状态
        LoadConfigData();
        // await 显示提示("已取消更改");
    }

    /// <summary>
    /// 显示提示信息
    /// </summary>
    private async UniTask<bool> 显示提示(string 消息)
    {
        try
        {
            var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
            var 告示实例 = Instantiate(告示, transform.parent);
            var 告示组件 = 告示实例.GetComponent<告示_茉>();

            bool IsConfig = await 告示组件.等待确认显示("提示", 消息);
            Destroy(告示实例);
            return IsConfig;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"显示提示失败: {e.Message}");
        }
        return false;
    }

    private void 同步窗口标题(string 名称)
    {
        窗口标题桥?.SetTitle(名称, 默认窗口标题);
    }

    void OnDestroy()
    {
        // 清理事件监听
        保存按钮?.OnClicked.RemoveAllListeners();
        取消按钮?.OnClicked.RemoveAllListeners();

    }

 
}
