using UnityEngine;
using QFramework;
using NovaSamples.UIControls;
using Cysharp.Threading.Tasks;
using Nova;
using DG.Tweening;
using System;

public class 界面管理界面_茉 : 界面基础_韩
{
    public bool 是否输入框动画 = true;
    [Header("输入框")]
    [SerializeField] private TextField 单位名称输入框;
    [SerializeField] private TextField 软件名称输入框;

    [Header("图片选择器")]
    [SerializeField] private 图片选择组件 图片选择器;

    [Header("按钮")]
    [SerializeField] private NovaSamples.UIControls.Button 保存按钮;
    [SerializeField] private NovaSamples.UIControls.Button 取消按钮;
    [SerializeField] private NovaSamples.UIControls.Button 还原图片按钮;

    private 软件配置录 配置录;
    private IWindowTitleBridge 窗口标题桥;
    [SerializeField] public Color inputSetColor;
    private static readonly string[] 默认登录背景资源候选 =
    {
        "登录界面A-2000-bg",
        "登录界面A-2000-bg.jpg"
        
    };
    private const string 默认单位名称 = "北京心智超航科技有限公司";
    private const string 默认软件名称 = "汉语失语症检查表（ABC法）评估管理系统";
    private const string 默认窗口标题 = 默认软件名称;
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
        还原图片按钮?.OnClicked.AddListener(() => 还原图片到初始状态().Forget());
        if (是否输入框动画)
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
        string 单位名称 = string.IsNullOrWhiteSpace(config.单位名称) ? 默认单位名称 : config.单位名称;
        string 软件名称 = string.IsNullOrWhiteSpace(config.软件名称) ? 默认软件名称 : config.软件名称;

        // 加载文本数据
        if (单位名称输入框 != null)
            单位名称输入框.Text = 单位名称;
        if (软件名称输入框 != null)
            软件名称输入框.Text = 软件名称;

        await 应用图片缓存到选择器(config?.图片缓存名称 ?? "");
        同步窗口标题(软件名称);

        // 设置截图工具的单位名称
        this.GetUtility<IScreenshotPDFUtility>().SetCompanyName(单位名称);

        Debug.Log("配置数据已加载到界面");
    }

    private async UniTask 应用图片缓存到选择器(string 图片缓存名)
    {
        if (图片选择器 == null)
        {
            return;
        }

        var imageUtility = this.GetUtility<ImagePickerUtility>();
        if (string.IsNullOrWhiteSpace(图片缓存名))
        {
            bool 已加载默认图 = await 加载并显示默认登录背景图();
            if (!已加载默认图)
            {
                图片选择器.恢复默认显示();
            }
            imageUtility.SaveCurrentImageRecord(string.Empty);
            return;
        }

        var texture2D = await imageUtility.LoadImageFromCacheAsync(图片缓存名);
        if (texture2D != null)
        {
            图片选择器.SetDisplayImage(texture2D);
            imageUtility.SaveCurrentImageRecord(图片缓存名);
            return;
        }

        bool 已回退默认图 = await 加载并显示默认登录背景图();
        if (!已回退默认图)
        {
            图片选择器.恢复默认显示();
        }
        imageUtility.SaveCurrentImageRecord(string.Empty);
    }

    private async UniTask<bool> 加载并显示默认登录背景图()
    {
        if (图片选择器 == null)
        {
            return false;
        }

        foreach (var 资源名 in 默认登录背景资源候选)
        {
            try
            {
                if (!LoadYooAssetsTool.CheckAssetExist(资源名))
                {
                    continue;
                }

                var sprite = await LoadYooAssetsTool.LoadAsset<Sprite>(资源名);
                if (sprite != null)
                {
                    图片选择器.SetDisplayImage(sprite);
                    Debug.Log($"已从YooAssets加载默认背景图(Sprite): {资源名}");
                    return true;
                }

                var texture = await LoadYooAssetsTool.LoadAsset<Texture2D>(资源名);
                if (texture != null)
                {
                    图片选择器.SetDisplayImage(texture);
                    Debug.Log($"已从YooAssets加载默认背景图(Texture): {资源名}");
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"加载默认背景图失败: {资源名}, {e.Message}");
            }
        }

        Debug.LogError("YooAssets中未找到默认背景图: 登录界面A-2000-bg.jpg / 登录界面A-2000-bg");
        return false;
    }

    private async UniTaskVoid 还原图片到初始状态()
    {
        try
        {
            bool 确认还原 = await 显示提示("是否还原图片为默认图片");
            if (!确认还原)
            {
                return;
            }

            bool 已还原 = await 加载并显示默认登录背景图();
            if (!已还原)
            {
                图片选择器?.恢复默认显示();
            }

            this.GetUtility<ImagePickerUtility>().SaveCurrentImageRecord(string.Empty);

            var 当前配置 = 配置录.当前配置 ?? new 软件配置();
            var 更新配置 = new 软件配置
            {
                单位名称 = 当前配置.单位名称 ?? "",
                软件名称 = 当前配置.软件名称 ?? "",
                图片缓存名称 = string.Empty
            };

            // 立即保存并触发配置修改事件，确保首页其他使用该图片的界面同步更新
            配置录.保存配置(更新配置);
            Debug.Log("图片已还原并同步到其他界面");
        }
        catch (Exception e)
        {
            Debug.LogError($"还原图片失败: {e.Message}");
            await 显示提示("还原图片失败");
        }
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
                软件名称 = (软件名称输入框?.Text?.Length > 21) ? 软件名称输入框.Text.Substring(0, 21) : (软件名称输入框?.Text ?? ""),
                图片缓存名称 = 图片选择器?.GetCurrentImageCacheName() ?? ""
            };

            // 显示保存成功提示
            bool IsConfig = await 显示提示("是否保存配置");
            if (IsConfig)
            {
                配置录.保存配置(新配置);
                同步窗口标题(新配置.软件名称);
                PlayerPrefs.SetString("sofewareName", 新配置.软件名称);
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
        还原图片按钮?.OnClicked.RemoveAllListeners();

    }


}
