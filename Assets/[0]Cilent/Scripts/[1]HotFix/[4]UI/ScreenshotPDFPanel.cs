using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using Cysharp.Threading.Tasks;
using Nova; // NovaUI

/// <summary>
/// 截图和PDF功能UI面板（精简版）
/// 支持NovaUI Scroller
/// </summary>
public class ScreenshotPDFPanel : MonoBehaviour, IController
{
    [Header("UI控件")]
    [SerializeField] private Button btnCapture;           // 截图按钮
    [SerializeField] private Button btnCapturePDF;        // 截图并导出按钮
    [SerializeField] private Button btnPrint;             // 打印按钮
    [SerializeField] private Button btnOpenFolder;        // 打开文件夹按钮
    
    [Header("状态显示")]
    [SerializeField] private Text txtStatus;              // 状态文本
    [SerializeField] private Text txtLastFile;            // 最后生成的文件路径
    
    [Header("截图选项")]
    [SerializeField] private Toggle toggleUseCamera;      // 是否使用相机截图
    [SerializeField] private Toggle toggleCaptureScroll;  // 是否截图NovaUI滚动视图
    [SerializeField] private Camera targetCamera;         // 目标相机
    [SerializeField] private Scroller targetNovaScroller; // 目标NovaUI滚动视图
    
    private IScreenshotPDFUtility screenshotUtility;
    private string lastGeneratedFile;

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }

    private void Start()
    {
        screenshotUtility = this.GetUtility<IScreenshotPDFUtility>();
        InitializeUI();
        BindEvents();
        UpdateStatus("就绪 - 选择截图方式");
    }

    private void InitializeUI()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        if (btnPrint != null) btnPrint.interactable = false;
        
        if (toggleUseCamera != null)
        {
            toggleUseCamera.isOn = false;
            toggleUseCamera.onValueChanged.AddListener(OnToggleChanged);
        }
        
        if (toggleCaptureScroll != null)
        {
            toggleCaptureScroll.isOn = false;
            toggleCaptureScroll.onValueChanged.AddListener(OnToggleChanged);
        }
    }

    private void BindEvents()
    {
        btnCapture?.onClick.AddListener(() => CaptureScreenshotAsync().Forget());
        btnCapturePDF?.onClick.AddListener(() => CaptureAndExportAsync().Forget());
        btnPrint?.onClick.AddListener(PrintLastFile);
        btnOpenFolder?.onClick.AddListener(OpenFileFolder);
    }

    /// <summary>
    /// 截图功能
    /// </summary>
    private async UniTask CaptureScreenshotAsync()
    {
        try
        {
            SetButtonsEnabled(false);
            UpdateStatus("正在截图...");
            
            string filePath = null;
            
            // 根据设置选择截图方式
            if (toggleCaptureScroll != null && toggleCaptureScroll.isOn && targetNovaScroller != null)
            {
                filePath = await screenshotUtility.CaptureScrollViewAsync(targetNovaScroller);
            }
            else if (toggleUseCamera != null && toggleUseCamera.isOn && targetCamera != null)
            {
                filePath = await screenshotUtility.CaptureCameraViewAsync(targetCamera);
            }
            else
            {
                filePath = await screenshotUtility.CaptureScreenshotAsync();
            }
            
            HandleCaptureResult(filePath, "截图");
        }
        catch (Exception e)
        {
            UpdateStatus($"截图错误: {e.Message}");
        }
        finally
        {
            SetButtonsEnabled(true);
        }
    }

    /// <summary>
    /// 截图并导出
    /// </summary>
    private async UniTask CaptureAndExportAsync()
    {
        try
        {
            SetButtonsEnabled(false);
            UpdateStatus("正在截图并生成文档...");
            
            string htmlPath = await screenshotUtility.CaptureAndExportToPDFAsync();
            HandleCaptureResult(htmlPath, "文档生成");
        }
        catch (Exception e)
        {
            UpdateStatus($"文档生成错误: {e.Message}");
        }
        finally
        {
            SetButtonsEnabled(true);
        }
    }

    /// <summary>
    /// 处理截图结果
    /// </summary>
    private void HandleCaptureResult(string filePath, string operation)
    {
        if (!string.IsNullOrEmpty(filePath))
        {
            lastGeneratedFile = filePath;
            UpdateStatus($"{operation}成功！");
            UpdateLastFile(filePath);
            if (btnPrint != null) btnPrint.interactable = true;
        }
        else
        {
            UpdateStatus($"{operation}失败，请重试");
        }
    }

    /// <summary>
    /// 打印最后生成的文件
    /// </summary>
    private void PrintLastFile()
    {
        if (string.IsNullOrEmpty(lastGeneratedFile))
        {
            UpdateStatus("没有可打印的文件");
            return;
        }
        
        UpdateStatus("正在发送到打印机...");
        bool success = screenshotUtility.PrintFile(lastGeneratedFile);
        UpdateStatus(success ? "文件已发送到打印机" : "打印失败");
    }

    /// <summary>
    /// 打开文件夹
    /// </summary>
    private void OpenFileFolder()
    {
        screenshotUtility.OpenFileDirectory(lastGeneratedFile);
        UpdateStatus("已打开文件夹");
    }

    /// <summary>
    /// 切换选项
    /// </summary>
    private void OnToggleChanged(bool value)
    {
        string mode = "全屏截图模式";
        
        if (toggleCaptureScroll != null && toggleCaptureScroll.isOn)
        {
            mode = "NovaUI滚动视图完整截图模式";
            if (targetNovaScroller == null)
                UpdateStatus("警告：未设置目标NovaUI滚动视图");
        }
        else if (toggleUseCamera != null && toggleUseCamera.isOn)
        {
            mode = "相机截图模式";
            if (targetCamera == null)
                targetCamera = Camera.main;
        }
        
        UpdateStatus($"当前模式: {mode}");
    }

    private void SetButtonsEnabled(bool enabled)
    {
        if (btnCapture != null) btnCapture.interactable = enabled;
        if (btnCapturePDF != null) btnCapturePDF.interactable = enabled;
        if (btnOpenFolder != null) btnOpenFolder.interactable = enabled;
    }

    private void UpdateStatus(string status)
    {
        if (txtStatus != null)
            txtStatus.text = $"[{DateTime.Now:HH:mm:ss}] {status}";
        Debug.Log($"[截图工具] {status}");
    }

    private void UpdateLastFile(string filePath)
    {
        if (txtLastFile != null)
        {
            string fileName = System.IO.Path.GetFileName(filePath);
            txtLastFile.text = $"最新文件: {fileName}";
        }
    }

    /// <summary>
    /// 快捷键支持
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
            CaptureScreenshotAsync().Forget();
        
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.P))
            CaptureAndExportAsync().Forget();
        
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.P))
            if (!string.IsNullOrEmpty(lastGeneratedFile)) PrintLastFile();
    }

    private void OnDestroy()
    {
        btnCapture?.onClick.RemoveAllListeners();
        btnCapturePDF?.onClick.RemoveAllListeners();
        btnPrint?.onClick.RemoveAllListeners();
        btnOpenFolder?.onClick.RemoveAllListeners();
        toggleUseCamera?.onValueChanged.RemoveAllListeners();
        toggleCaptureScroll?.onValueChanged.RemoveAllListeners();
    }
}

/// <summary>
/// 快捷截图工具类（精简版，支持NovaUI）
/// </summary>
public static class QuickScreenshot
{
    /// <summary>
    /// 快速截图
    /// </summary>
    public static void Capture() => CaptureAsync().Forget();

    /// <summary>
    /// 快速截图并生成文档
    /// </summary>
    public static void CaptureAndPrint() => CaptureAndPrintAsync().Forget();

    /// <summary>
    /// 快速截图NovaUI滚动视图
    /// </summary>
    public static void CaptureNovaScrollView(Scroller novaScroller) => CaptureNovaScrollViewAsync(novaScroller).Forget();

    private static async UniTask CaptureAsync()
    {
        try
        {
            var utility = HotFixTemplateArchitecture.Interface.GetUtility<IScreenshotPDFUtility>();
            string filePath = await utility.CaptureScreenshotAsync();
            if (!string.IsNullOrEmpty(filePath))
            {
                Debug.Log($"快速截图完成: {filePath}");
                utility.OpenFileDirectory(filePath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"快速截图失败: {e.Message}");
        }
    }

    private static async UniTask CaptureAndPrintAsync()
    {
        try
        {
            var utility = HotFixTemplateArchitecture.Interface.GetUtility<IScreenshotPDFUtility>();
            string htmlPath = await utility.CaptureAndExportToPDFAsync();
            if (!string.IsNullOrEmpty(htmlPath))
            {
                Debug.Log($"文档生成完成: {htmlPath}");
                utility.PrintFile(htmlPath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"快速打印失败: {e.Message}");
        }
    }

    private static async UniTask CaptureNovaScrollViewAsync(Scroller novaScroller)
    {
        try
        {
            var utility = HotFixTemplateArchitecture.Interface.GetUtility<IScreenshotPDFUtility>();
            string filePath = await utility.CaptureScrollViewAsync(novaScroller);
            if (!string.IsNullOrEmpty(filePath))
            {
                Debug.Log($"NovaUI滚动视图截图完成: {filePath}");
                utility.OpenFileDirectory(filePath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"NovaUI滚动视图截图失败: {e.Message}");
        }
    }
} 