using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using Cysharp.Threading.Tasks;
using System.Diagnostics;
using NovaSamples.UIControls;
using Nova;
/// <summary>
/// 截图和PDF导出打印工具类（精简版）
/// 支持页面截图、NovaUI滚动视图完整截图、PDF导出、打印功能
/// </summary>
public interface IScreenshotPDFUtility : IUtility
{
    /// <summary>
    /// 截图当前屏幕
    /// </summary>
    UniTask<string> CaptureScreenshotAsync(string fileName = null);
    
    /// <summary>
    /// 截图指定相机视图
    /// </summary>
    UniTask<string> CaptureCameraViewAsync(Camera camera, string fileName = null);
    
    /// <summary>
    /// 截图NovaUI滚动视图完整内容
    /// </summary>
    UniTask<string> CaptureScrollViewAsync(Scroller scrollView, string fileName = null);
    
    /// <summary>
    /// 截图并直接导出为PDF
    /// </summary>
    UniTask<string> CaptureAndExportToPDFAsync(string fileName = null);
    
    /// <summary>
    /// 将图片转换为可打印的HTML页面
    /// </summary>
    string ConvertImageToHTML(string imagePath, string title = "截图文档");
    
    /// <summary>
    /// 打印文件（支持图片和HTML）
    /// </summary>
    bool PrintFile(string filePath);
    
    /// <summary>
    /// 打开文件所在目录
    /// </summary>
    void OpenFileDirectory(string filePath);
    
    /// <summary>
    /// 获取导出目录
    /// </summary>
    string GetExportDirectory();
}

public class ScreenshotPDFUtility : IScreenshotPDFUtility
{
    private const string EXPORT_FOLDER = "Screenshots";
    
    public string GetExportDirectory()
    {
        return Path.Combine(Application.persistentDataPath, EXPORT_FOLDER);
    }
    
    /// <summary>
    /// 截图当前屏幕
    /// </summary>
    public async UniTask<string> CaptureScreenshotAsync(string fileName = null)
    {
        try
        {
            fileName = fileName ?? $"Screenshot_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";
            string exportDir = GetExportDirectory();
            if (!Directory.Exists(exportDir))
                Directory.CreateDirectory(exportDir);
            
            string filePath = Path.Combine(exportDir, fileName + ".png");
            
            await UniTask.WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot(filePath);
            await WaitForFileCreated(filePath);
            
            UnityEngine.Debug.Log($"截图成功: {filePath}");
            return filePath;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"截图失败: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 截图指定相机视图
    /// </summary>
    public async UniTask<string> CaptureCameraViewAsync(Camera camera, string fileName = null)
    {
        try
        {
            if (camera == null)
            {
                UnityEngine.Debug.LogError("相机为空");
                return null;
            }
            
            fileName = fileName ?? $"CameraShot_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";
            string exportDir = GetExportDirectory();
            if (!Directory.Exists(exportDir))
                Directory.CreateDirectory(exportDir);
            
            string filePath = Path.Combine(exportDir, fileName + ".png");
            
            await UniTask.WaitForEndOfFrame();
            
            // 使用QFramework的相机截图功能
            Rect captureRect = new Rect(0, 0, Screen.width, Screen.height);
            Texture2D screenshot = camera.CaptureCamera(captureRect);
            
            if (screenshot != null)
            {
                byte[] data = screenshot.EncodeToPNG();
                await File.WriteAllBytesAsync(filePath, data);
                UnityEngine.Object.DestroyImmediate(screenshot);
                
                UnityEngine.Debug.Log($"相机截图成功: {filePath}");
                return filePath;
            }
            
            return null;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"相机截图失败: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 截图NovaUI滚动视图完整内容
    /// </summary>
    public async UniTask<string> CaptureScrollViewAsync(Scroller scrollView, string fileName = null)
    {
        try
        {
            if (scrollView == null)
            {
                UnityEngine.Debug.LogError("NovaUI滚动视图为空");
                return null;
            }
            
            fileName = fileName ?? $"NovaScroller_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";
            string exportDir = GetExportDirectory();
            if (!Directory.Exists(exportDir))
                Directory.CreateDirectory(exportDir);
            
            string filePath = Path.Combine(exportDir, fileName + ".png");
            
            // 获取Scroller关联的UIBlock
            UIBlock uiBlock = scrollView.UIBlock;
            if (uiBlock == null)
            {
                UnityEngine.Debug.LogError("Scroller没有关联的UIBlock");
                return null;
            }
            
            // 获取AutoLayout并保存原始状态
            AutoLayout autoLayout = uiBlock.AutoLayout;
            float originalOffset = autoLayout.Offset;
            
            // 获取AutoLayout轴向
            if (!autoLayout.Axis.TryGetIndex(out int axis))
            {
                UnityEngine.Debug.LogError("AutoLayout轴向未设置");
                return null;
            }
            
            // 获取内容和视口尺寸
            Vector3 paddedSize = uiBlock.PaddedSize;
            Vector3 calculatedSize = uiBlock.CalculatedSize.Value;
            
            // 尝试获取子元素的总尺寸
            Bounds childBounds = uiBlock.ChildBounds;
            Vector3 contentSize = childBounds.size;
            
            UnityEngine.Debug.Log($"PaddedSize: {paddedSize}, CalculatedSize: {calculatedSize}, ChildBounds: {childBounds.size}");
            
            // 计算需要的RenderTexture尺寸
            int textureWidth = Mathf.CeilToInt(paddedSize.x);
            int textureHeight = Mathf.CeilToInt(contentSize[axis] > paddedSize[axis] ? contentSize[axis] : paddedSize.y);
            
            // 限制最大尺寸防止内存溢出
            textureWidth = Mathf.Min(textureWidth, 4096);
            textureHeight = Mathf.Min(textureHeight, 8192);
            
            UnityEngine.Debug.Log($"RenderTexture尺寸: {textureWidth}x{textureHeight}");
            
            // 获取渲染相机
            Canvas canvas = uiBlock.GetComponentInParent<Canvas>();
            Camera renderCamera = canvas?.worldCamera ?? Camera.main;
            
            // 创建RenderTexture
            RenderTexture renderTexture = new RenderTexture(textureWidth, textureHeight, 24);
            RenderTexture.active = renderTexture;
            
            // 临时设置相机
            RenderTexture originalTargetTexture = renderCamera.targetTexture;
            CameraClearFlags originalClearFlags = renderCamera.clearFlags;
            Color originalBackgroundColor = renderCamera.backgroundColor;
            
            renderCamera.targetTexture = renderTexture;
            renderCamera.clearFlags = CameraClearFlags.SolidColor;
            renderCamera.backgroundColor = Color.white;
            
            // 移动到内容开始位置（显示完整内容）
            float contentOverflow = contentSize[axis] - paddedSize[axis];
            if (contentOverflow > 0)
            {
                // 将内容起点对齐到顶部，避免截图偏下
                autoLayout.Offset = 0;
                uiBlock.AutoLayout = autoLayout; // 应用修改
                
                // 等待布局更新
                uiBlock.CalculateLayout();
                await UniTask.WaitForEndOfFrame();
            }
            
            // 渲染
            renderCamera.Render();
            
            // 等待渲染完成（等待多帧确保GPU渲染完成）
            await UniTask.WaitForEndOfFrame();
            await UniTask.Yield();
            await UniTask.Yield();
            
            // 确保在正确的时机读取像素
            // 必须在主线程中，且RenderTexture.active必须正确设置
            RenderTexture previousActive = RenderTexture.active;
            RenderTexture.active = renderTexture;
            
            try
            {
                Texture2D screenshot = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB24, false);
                screenshot.ReadPixels(new Rect(0, 0, textureWidth, textureHeight), 0, 0);
                screenshot.Apply();
                
                // 保存文件
                byte[] data = screenshot.EncodeToPNG();
                await File.WriteAllBytesAsync(filePath, data);
                
                // 清理screenshot
                UnityEngine.Object.DestroyImmediate(screenshot);
            }
            finally
            {
                // 恢复之前的RenderTexture.active状态
                RenderTexture.active = previousActive;
            }
            
            // 恢复原始状态
            autoLayout.Offset = originalOffset;
            uiBlock.AutoLayout = autoLayout; // 恢复原始值
            uiBlock.CalculateLayout();
            
            renderCamera.targetTexture = originalTargetTexture;
            renderCamera.clearFlags = originalClearFlags;
            renderCamera.backgroundColor = originalBackgroundColor;
            
            // 清理资源
            UnityEngine.Object.DestroyImmediate(renderTexture);
            
            UnityEngine.Debug.Log($"NovaUI滚动视图截图成功: {filePath}");
            return filePath;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"NovaUI滚动视图截图失败: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 截图并导出为可打印格式
    /// </summary>
    public async UniTask<string> CaptureAndExportToPDFAsync(string fileName = null)
    {
        try
        {
            fileName = fileName ?? $"Document_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";
            
            // 先截图
            string imagePath = await CaptureScreenshotAsync(fileName);
            if (string.IsNullOrEmpty(imagePath))
                return null;
            
            // 转换为HTML格式（更兼容打印）
            string htmlPath = ConvertImageToHTML(imagePath, "屏幕截图文档");
            
            UnityEngine.Debug.Log($"文档生成成功: {htmlPath}");
            return htmlPath;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"文档生成失败: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 将图片转换为HTML格式（便于打印）
    /// </summary>
    public string ConvertImageToHTML(string imagePath, string title = "截图文档")
    {
        try
        {
            if (!File.Exists(imagePath))
                return null;
            
            string htmlContent = $@"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>{title}</title>
    <style>
        @page {{ margin: 0; size: A4; }}
        body {{ margin: 0; padding: 0; font-family: Arial, sans-serif; }}
        .content {{ text-align: center; }}
        .image {{ max-width: 100%; height: auto; }}
        @media print {{
            .no-print {{ display: none; }}
            .image {{ max-height: 100vh; }}
        }}
    </style>
</head>
<body>
    <div class='content'>
        <img src='file:///{imagePath.Replace("\\", "/")}' alt='截图' class='image'>
    </div>
    <div class='no-print' style='position: fixed; top: 10px; right: 10px; z-index: 1000;'>
        <button onclick='window.print()' style='padding: 10px 20px; font-size: 14px;'>打印文档</button>
    </div>
</body>
</html>";
            
            string htmlPath = Path.ChangeExtension(imagePath, ".html");
            File.WriteAllText(htmlPath, htmlContent, System.Text.Encoding.UTF8);
            
            return htmlPath;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"生成HTML失败: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 打印文件（适用于HybridCLR热更新环境）
    /// </summary>
    public bool PrintFile(string filePath)
    {
        try
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                UnityEngine.Debug.LogError("文件不存在: " + filePath);
                return false;
            }
            
            // 确保路径是绝对路径且格式正确
            filePath = Path.GetFullPath(filePath);
            UnityEngine.Debug.Log($"[打印] 尝试打开文件: {filePath}");
            
            string extension = Path.GetExtension(filePath).ToLower();
            
            // 对于HTML文件，使用Application.OpenURL打开（HybridCLR热更新代码中可用）
            if (extension == ".html" || extension == ".htm")
            {
                // 将文件路径转换为file://协议格式
                string fileUrl = "file:///" + filePath.Replace("\\", "/");
                
                try
                {
                    // 方法1：使用Application.OpenURL（HybridCLR热更新代码中可用）
                    Application.OpenURL(fileUrl);
                    UnityEngine.Debug.Log($"已用浏览器打开HTML文件(Application.OpenURL): {fileUrl}");
                    return true;
                }
                catch (Exception e1)
                {
                    UnityEngine.Debug.LogWarning($"Application.OpenURL失败: {e1.Message}，尝试备用方法");
                    
                    // 方法2：尝试使用Process.Start（备用）
                    try
                    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            FileName = filePath,
                            UseShellExecute = true
                        };
                        Process.Start(startInfo);
                        UnityEngine.Debug.Log($"已用浏览器打开HTML文件(Process.Start): {filePath}");
                        return true;
#else
                        Application.OpenURL(fileUrl);
                        return true;
#endif
                    }
                    catch (Exception e2)
                    {
                        UnityEngine.Debug.LogError($"所有方法都失败了。最后错误: {e2.Message}");
                        return false;
                    }
                }
            }
            else
            {
                // 其他文件类型尝试直接打印
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true,
                        Verb = "print"
                    });
                    UnityEngine.Debug.Log($"文件已发送到打印机: {filePath}");
                    return true;
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogWarning($"直接打印失败: {e.Message}，尝试打开文件");
                    // 打印失败时打开文件
                    Application.OpenURL("file:///" + filePath.Replace("\\", "/"));
                    return true;
                }
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"打印失败: {e.Message}");
            UnityEngine.Debug.LogError($"异常堆栈: {e.StackTrace}");
            // 打印失败时打开文件让用户手动打印
            try
            {
                string fileUrl = "file:///" + filePath.Replace("\\", "/");
                Application.OpenURL(fileUrl);
                UnityEngine.Debug.Log("已打开文件，请手动打印");
                return true;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"打开文件失败: {ex.Message}");
                UnityEngine.Debug.LogError($"异常堆栈: {ex.StackTrace}");
                return false;
            }
        }
    }
    
    /// <summary>
    /// 打开文件所在目录
    /// </summary>
    public void OpenFileDirectory(string filePath)
    {
        try
        {
            if (string.IsNullOrEmpty(filePath))
                filePath = GetExportDirectory();
            
            string directory = File.Exists(filePath) ? Path.GetDirectoryName(filePath) : filePath;
            
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            if (File.Exists(filePath))
                Process.Start("explorer.exe", $"/select,\"{filePath}\"");
            else
                Process.Start("explorer.exe", $"\"{directory}\"");
#else
            Process.Start(directory);
#endif
            
            UnityEngine.Debug.Log($"已打开目录: {directory}");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"打开目录失败: {e.Message}");
        }
    }
    
    /// <summary>
    /// 等待文件创建完成
    /// </summary>
    private async UniTask WaitForFileCreated(string filePath, int timeoutSeconds = 10)
    {
        var startTime = DateTime.Now;
        while (!File.Exists(filePath))
        {
            if ((DateTime.Now - startTime).TotalSeconds > timeoutSeconds)
                throw new TimeoutException($"等待文件创建超时: {filePath}");
            await UniTask.Delay(100);
        }
        await UniTask.Delay(500); // 等待文件写入完成
    }
} 