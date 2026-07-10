using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using Cysharp.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using Microsoft.Win32;
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
    /// 将多个图片合并为可打印的HTML页面（每张图片一页）
    /// </summary>
    string ConvertMultipleImagesToHTML(string[] imagePaths, string title = "多页截图文档");

    /// <summary>
    /// 将图片转换为可打印的HTML页面
    /// </summary>
    string ConvertImageToHTML(string imagePath, string title = "截图文档");
    
    /// <summary>
    /// 打印文件（支持图片和HTML）
    /// </summary>
    bool PrintFile(string filePath);

    /// <summary>
    /// 弹出系统打印机选择并打印多个文件（Windows）
    /// </summary>
    bool PrintFilesWithPrinterDialog(string[] filePaths);

    /// <summary>
    /// 使用指定打印机打印多个文件（Windows）
    /// </summary>
    bool PrintFilesWithPrinter(string[] filePaths, string printerName);

    /// <summary>
    /// 获取系统可用打印机名称列表
    /// </summary>
    List<string> GetAvailablePrinterNames();

    /// <summary>
    /// 获取系统默认打印机名称
    /// </summary>
    string GetDefaultPrinterName();
    
    /// <summary>
    /// 打开文件所在目录
    /// </summary>
    void OpenFileDirectory(string filePath);
    
    /// <summary>
    /// 获取导出目录
    /// </summary>
    string GetExportDirectory();

    /// <summary>
    /// 设置页脚显示的单位名称
    /// </summary>
    void SetCompanyName(string companyName);
}

public class ScreenshotPDFUtility : IScreenshotPDFUtility
{
    private const string EXPORT_FOLDER = "Screenshots";
    private string _companyName = "北京心智超航科技有限公司";
    private static IPrinterBridge _printerBridge;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    private static IPrinterBridge GetPrinterBridge()
    {
        if (_printerBridge == null)
        {
            try
            {
                _printerBridge = new PrinterBridge();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning($"[打印] 创建PrinterBridge失败: {e.Message}");
            }
        }
        return _printerBridge;
    }

#endif
    
    public string GetExportDirectory()
    {
        return Path.Combine(Application.persistentDataPath, EXPORT_FOLDER);
    }

    /// <summary>
    /// 设置页脚显示的单位名称
    /// </summary>
    public void SetCompanyName(string companyName)
    {
        _companyName = string.IsNullOrEmpty(companyName) ? "北京心智超航科技有限公司" : companyName;
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
            textureHeight = Mathf.Min(textureHeight, 10000);
            
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
                // 恢复之前的 RenderTexture.active 状态
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
    /// 将多个图片合并为HTML格式（每张图片一页，便于打印）
    /// </summary>
    public string ConvertMultipleImagesToHTML(string[] imagePaths, string title = "多页截图文档")
    {
        try
        {
            if (imagePaths == null || imagePaths.Length == 0)
                return null;

            // A4纸尺寸（96 DPI）：794 x 1123 像素（8.27 x 11.69 英寸）
            const int A4_WIDTH = 794;
            const int A4_HEIGHT = 1123;

            // 方案：内容区接近 A4，但保留页脚空间（避免重叠）
            const int FOOTER_RESERVE_PX = 60; // 为页脚保留60px
            const int CONTENT_PADDING_TOP = 50; // 每页顶部留白（px）
            const int CONTENT_WIDTH = A4_WIDTH;
            const int CONTENT_HEIGHT = A4_HEIGHT - FOOTER_RESERVE_PX - CONTENT_PADDING_TOP;

            string pageContent = "";

            int totalPages = 0;
            for (int imageIndex = 0; imageIndex < imagePaths.Length; imageIndex++)
            {
                string imagePath = imagePaths[imageIndex];
                if (!File.Exists(imagePath))
                {
                    UnityEngine.Debug.LogWarning($"图片文件不存在: {imagePath}");
                    continue;
                }

                // 获取图片尺寸
                Vector2Int imageSize = GetImageSize(imagePath);
                if (imageSize.x <= 0 || imageSize.y <= 0)
                {
                    UnityEngine.Debug.LogWarning($"无法获取图片尺寸: {imagePath}");
                    continue;
                }

                // 计算图片缩放比例（确保宽度适应内容区域）
                float scaleX = (float)CONTENT_WIDTH / imageSize.x;
                float scaledHeight = imageSize.y * scaleX;

                // 将图片转换为base64，避免文件路径问题
                string base64Image = "";
                try
                {
                    byte[] imageBytes = File.ReadAllBytes(imagePath);
                    string extension = Path.GetExtension(imagePath).ToLower();
                    string mimeType = extension == ".png" ? "image/png" : (extension == ".jpg" || extension == ".jpeg" ? "image/jpeg" : "image/png");
                    base64Image = "data:" + mimeType + ";base64," + Convert.ToBase64String(imageBytes);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError($"读取图片文件失败: {e.Message}");
                    // 如果 base64 失败，回退到文件路径方式
                    string imageUrl = "file:///" + imagePath.Replace("\\", "/");
                    base64Image = imageUrl;
                }
                // 计算这张图片需要多少页
                // 智能切分：先尝试加载图片并在接近切分点回溯寻找空白行
                List<float> sliceStartsScaled = null;
                Texture2D tex = null;
                try
                {
                    byte[] imageBytesForScan = File.ReadAllBytes(imagePath);
                    tex = new Texture2D(2, 2, TextureFormat.RGB24, false);
                    if (tex.LoadImage(imageBytesForScan, false))
                    {
                        sliceStartsScaled = ComputeSafeSliceStarts(tex, imageSize, CONTENT_WIDTH, CONTENT_HEIGHT, scaleX);
                    }
                }
                catch (Exception)
                {
                    sliceStartsScaled = null;
                }
                finally
                {
                    if (tex != null)
                    {
                        UnityEngine.Object.DestroyImmediate(tex);
                        tex = null;
                    }
                }

                if (sliceStartsScaled == null || sliceStartsScaled.Count == 0)
                {
                    int imagePages = Mathf.CeilToInt(scaledHeight / CONTENT_HEIGHT);
                    if (imagePages <= 0) imagePages = 1;
                    sliceStartsScaled = new List<float>();
                    for (int i = 0; i < imagePages; i++) sliceStartsScaled.Add(i * CONTENT_HEIGHT);
                }

                UnityEngine.Debug.Log($"图片 {imageIndex + 1}: 原始尺寸 {imageSize.x}x{imageSize.y}, 缩放后高度 {scaledHeight:F0}, 切片数 {sliceStartsScaled.Count}");

                // 为这张图片生成所有需要的页面（使用 sliceStartsScaled）
                for (int idx = 0; idx < sliceStartsScaled.Count; idx++)
                {
                    float sliceStart = sliceStartsScaled[idx];
                    float sliceHeight;
                    if (idx < sliceStartsScaled.Count - 1)
                    {
                        sliceHeight = sliceStartsScaled[idx + 1] - sliceStart;
                    }
                    else
                    {
                        sliceHeight = Mathf.Min(CONTENT_HEIGHT, scaledHeight - sliceStart);
                    }

                    // 当为第一页时，在内容区底部添加一条边框线；当为第二页时，在内容区顶部添加相同的边框线
                    string bottomBorderHtml = totalPages == 0
                        ? "<div class='bottom-line' style='position:absolute;bottom:0;left:67px;right:65px;height:1.5px;background:#2278D7;'></div>"
                        : "";
                    string topBorderHtml = totalPages == 1
                        ? "<div class='bottom-line' style='position:absolute;top:0;left:67px;right:65px;height:1.5px;background:#2278D7;'></div>"
                        : "";

                    // 生成页面内容
                    pageContent += $@"
    <div class='page'>
        <div class='content' style='width: {CONTENT_WIDTH}px; height: {sliceHeight:F0}px; position: relative; overflow: hidden; margin: {CONTENT_PADDING_TOP}px 0 0 0; box-sizing: border-box;'>
            {topBorderHtml}
            <div class='image-wrapper' style='position: absolute; top: {-sliceStart:F0}px; left: 0; width: {CONTENT_WIDTH}px; height: {scaledHeight:F0}px;'>
                <img src='{base64Image}' alt='第{totalPages + 1}页' style='width: {CONTENT_WIDTH}px; height: {scaledHeight:F0}px; object-fit: contain; display: block;' />
            </div>
            {bottomBorderHtml}
        </div>
        <div class='footer'>{_companyName}</div>
    </div>";

                    totalPages++;
                }
            }

            if (string.IsNullOrEmpty(pageContent))
            {
                UnityEngine.Debug.LogError("没有有效的页面内容生成");
                return null;
            }

            string htmlContent = $@"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>{title}</title>
    <style>
        /* 强制打印使用无页边距并与 A4 等比（减少浏览器打印预览自动缩放）*/
        @page {{
            margin: 0;
            size: A4;
        }}
        html, body {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
            -webkit-print-color-adjust: exact;
            print-color-adjust: exact;
            width: {A4_WIDTH}px;
            height: {A4_HEIGHT}px;
        }}
        body {{
            font-family: Arial, sans-serif;
            background: white;
        }}
        .page {{
            width: {A4_WIDTH}px;
            height: {A4_HEIGHT}px;
            margin: 0 auto;
            background: white;
            position: relative;
            overflow: hidden;
            page-break-inside: avoid;
        }}
        .content {{
            width: {CONTENT_WIDTH}px;
            height: {CONTENT_HEIGHT}px;
            position: relative;
            overflow: hidden;
            margin: 0 auto;
        }}
        .image-wrapper {{
            position: absolute;
            z-index: 1;
        }}
        .bottom-line {{
            position: absolute;
            height: 2.5px;
            background: #000;
            display: block;
            z-index: 10;
        }}
        .footer {{
            position: absolute;
            bottom: 20px; /* 固定在页面底部20px位置，与图片内容分离 */
            left: 0;
            right: 0;
            text-align: center;
            font-size: 12px;
            color: #666;
        }}
        @media print {{
            /* 移除浏览器在打印预览中添加的默认边距 */
            html, body {{
                width: {A4_WIDTH}px;
                height: {A4_HEIGHT}px;
                margin: 0;
                padding: 0;
            }}
            .no-print {{
                display: none;
            }}
            .page {{
                page-break-after: always;
                page-break-inside: avoid;
                margin: 0;
                box-shadow: none;
            }}
        }}
        @media screen {{
            .page {{
                margin-bottom: 20px;
                box-shadow: 0 0 10px rgba(0,0,0,0.1);
            }}
        }}
    </style>
</head>
<body>
{pageContent}
    <div class='no-print' style='position: fixed; top: 10px; right: 10px; z-index: 1000; background: white; padding: 10px; border-radius: 5px; box-shadow: 0 2px 10px rgba(0,0,0,0.2);'>
        <button onclick='window.print()' style='padding: 10px 20px; font-size: 14px; cursor: pointer; background: #007bff; color: white; border: none; border-radius: 3px;'>打印文档 ({imagePaths.Length} 页)</button>
    </div>
</body>
</html>";

            // 使用第一个图片的路径作为基础来生成HTML文件路径
            string htmlPath = Path.Combine(Path.GetDirectoryName(imagePaths[0]), Path.GetFileNameWithoutExtension(imagePaths[0]) + "_multi.html");
            File.WriteAllText(htmlPath, htmlContent, System.Text.Encoding.UTF8);

            UnityEngine.Debug.Log($"多页HTML生成成功: {htmlPath} ({totalPages} 页)");
            return htmlPath;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"生成多页HTML失败: {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// 将图片转换为HTML格式（便于打印，支持自动分页）
    /// </summary>
    public string ConvertImageToHTML(string imagePath, string title = "截图文档")
    {
        try
        {
            if (!File.Exists(imagePath))
                return null;
            
            // 获取图片尺寸
            Vector2Int imageSize = GetImageSize(imagePath);
            if (imageSize.x <= 0 || imageSize.y <= 0)
            {
                UnityEngine.Debug.LogWarning("无法获取图片尺寸，使用单页模式");
                return ConvertImageToHTMLSinglePage(imagePath, title);
            }
            
            // A4纸尺寸（96 DPI）：794 x 1123 像素（8.27 x 11.69 英寸）
            const int A4_WIDTH = 794;
            const int A4_HEIGHT = 1123;

            // 方案B：内容区接近 A4，但保留页脚空间（避免重叠）
            // 将内容宽度设为 A4 宽度，内容高度为 A4 高度减去页脚保留高度和顶部留白
            const int FOOTER_RESERVE_PX = 60; // 为页脚保留80px（可按需调整）
            const int CONTENT_PADDING_TOP = 50; // 每页顶部留白（px）
            const int CONTENT_WIDTH = A4_WIDTH;
            // 内容高度为 A4 高度减去页脚保留和顶部留白（用于每页切片步长）
            const int CONTENT_HEIGHT = A4_HEIGHT - FOOTER_RESERVE_PX - CONTENT_PADDING_TOP;

            // 如果图片高度小于等于内容区域高度，使用单页模式
            if (imageSize.y <= CONTENT_HEIGHT)
            {
                return ConvertImageToHTMLSinglePage(imagePath, title);
            }

            // 计算图片缩放比例（确保宽度适应内容区域）
            float scaleX = (float)CONTENT_WIDTH / imageSize.x;
            float scaledHeight = imageSize.y * scaleX;

            // 计算需要的页数（基于缩放后的高度），并尝试智能切分
            List<float> sliceStartsScaledSingle = null;
            Texture2D texSingle = null;
            try
            {
                byte[] imageBytesForScan = File.ReadAllBytes(imagePath);
                texSingle = new Texture2D(2, 2, TextureFormat.RGB24, false);
                if (texSingle.LoadImage(imageBytesForScan, false))
                {
                    sliceStartsScaledSingle = ComputeSafeSliceStarts(texSingle, imageSize, CONTENT_WIDTH, CONTENT_HEIGHT, scaleX);
                }
            }
            catch (Exception)
            {
                sliceStartsScaledSingle = null;
            }
            finally
            {
                if (texSingle != null)
                {
                    UnityEngine.Object.DestroyImmediate(texSingle);
                    texSingle = null;
                }
            }

            if (sliceStartsScaledSingle == null || sliceStartsScaledSingle.Count == 0)
            {
                int calcPages = Mathf.CeilToInt(scaledHeight / CONTENT_HEIGHT);
                if (calcPages > 4)
                {
                    UnityEngine.Debug.Log($"图片尺寸: {imageSize.x}x{imageSize.y}，缩放后: {CONTENT_WIDTH}x{scaledHeight:F0}，原需 {calcPages} 页，限制为最多 4 页");
                }
                else
                {
                    UnityEngine.Debug.Log($"图片尺寸: {imageSize.x}x{imageSize.y}，缩放后: {CONTENT_WIDTH}x{scaledHeight:F0}，将分割为 {calcPages} 页");
                }
                sliceStartsScaledSingle = new List<float>();
                int pages = calcPages;
                for (int i = 0; i < pages; i++) sliceStartsScaledSingle.Add(i * CONTENT_HEIGHT);
            }
            else
            {
                UnityEngine.Debug.Log($"图片尺寸: {imageSize.x}x{imageSize.y}，缩放后: {CONTENT_WIDTH}x{scaledHeight:F0}，智能切分为 {sliceStartsScaledSingle.Count} 段");
            }
            
            // 将图片转换为base64，避免文件路径问题
            string base64Image = "";
            try
            {
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                string extension = Path.GetExtension(imagePath).ToLower();
                string mimeType = extension == ".png" ? "image/png" : (extension == ".jpg" || extension == ".jpeg" ? "image/jpeg" : "image/png");
                base64Image = "data:" + mimeType + ";base64," + Convert.ToBase64String(imageBytes);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"读取图片文件失败: {e.Message}");
                // 如果 base64 失败，回退到文件路径方式
                string imageUrl = "file:///" + imagePath.Replace("\\", "/");
                base64Image = imageUrl;
            }
            string pageContent = "";
            
            int printedPageIndex = 0;
            for (int i = 0; i < sliceStartsScaledSingle.Count; i++)
            {
                float sliceStart = sliceStartsScaledSingle[i];
                float sliceHeight;
                if (i < sliceStartsScaledSingle.Count - 1)
                {
                    sliceHeight = sliceStartsScaledSingle[i + 1] - sliceStart;
                }
                else
                {
                    sliceHeight = Mathf.Min(CONTENT_HEIGHT, scaledHeight - sliceStart);
                }
                float topOffset = -sliceStart;

                string bottomBorderHtml = printedPageIndex == 0
                    ? "<div class='bottom-line' style='position:absolute;bottom:0;left:67px;right:65px;height:1.5px;background:#2278D7;'></div>"
                    : "";
                string topBorderHtml = printedPageIndex == 1
                    ? "<div class='bottom-line' style='position:absolute;top:0;left:67px;right:65px;height:1.5px;background:#2278D7;'></div>"
                    : "";

                pageContent += $@"
    <div class='page'>
        <div class='content' style='width: {CONTENT_WIDTH}px; height: {sliceHeight:F0}px; position: relative; overflow: hidden; margin: {CONTENT_PADDING_TOP}px 0 0 0; box-sizing: border-box;'>
            {topBorderHtml}
            <div class='image-wrapper' style='position: absolute; top: {topOffset:F0}px; left: 0; width: {CONTENT_WIDTH}px; height: {scaledHeight:F0}px;'>
                <img src='{base64Image}' alt='截图第{printedPageIndex + 1}页' style='width: {CONTENT_WIDTH}px; height: {scaledHeight:F0}px; object-fit: contain; display: block;' />
            </div>
            {bottomBorderHtml}
        </div>
        <div class='footer'>{_companyName}</div>
    </div>";
                printedPageIndex++;
            }

            int totalPages = printedPageIndex;
            
            string htmlContent = $@"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>{title}</title>
    <style>
        /* 强制打印使用无页边距并与 A4 等比（减少浏览器打印预览自动缩放）*/
        @page {{
            margin: 0;
            size: A4;
        }}
        html, body {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
            -webkit-print-color-adjust: exact;
            print-color-adjust: exact;
            width: {A4_WIDTH}px;
            height: {A4_HEIGHT}px;
        }}
        body {{
            font-family: Arial, sans-serif;
            background: white;
        }}
        .page {{
            width: {A4_WIDTH}px;
            height: {A4_HEIGHT}px;
            margin: 0 auto;
            background: white;
            position: relative;
            overflow: hidden;
            page-break-inside: avoid;
        }}
        .content {{
            width: {CONTENT_WIDTH}px;
            height: {CONTENT_HEIGHT}px;
            position: relative;
            overflow: hidden;
            margin: 0 auto;
        }}
        .image-wrapper {{
            position: absolute;
            z-index: 1;
        }}
        .bottom-line {{
            position: absolute;
            height: 2.5px;
            background: #000;
            display: block;
            z-index: 10;
        }}
        .footer {{
            position: absolute;
            bottom: 20px; /* 固定在页面底部20px位置，与图片内容分离 */
            left: 0;
            right: 0;
            text-align: center;
            font-size: 12px;
            color: #666;
        }}
        @media print {{
            /* 移除浏览器在打印预览中添加的默认边距 */
            html, body {{
                width: {A4_WIDTH}px;
                height: {A4_HEIGHT}px;
                margin: 0;
                padding: 0;
            }}
            .no-print {{
                display: none;
            }}
            .page {{
                page-break-after: always;
                page-break-inside: avoid;
                margin: 0;
                box-shadow: none;
            }}
        }}
        @media screen {{
            .page {{
                margin-bottom: 20px;
                box-shadow: 0 0 10px rgba(0,0,0,0.1);
            }}
        }}
    </style>
</head>
<body>
{pageContent}
    <div class='no-print' style='position: fixed; top: 10px; right: 10px; z-index: 1000; background: white; padding: 10px; border-radius: 5px; box-shadow: 0 2px 10px rgba(0,0,0,0.2);'>
        <button onclick='window.print()' style='padding: 10px 20px; font-size: 14px; cursor: pointer; background: #007bff; color: white; border: none; border-radius: 3px;'>打印文档 ({totalPages} 页)</button>
    </div>
</body>
</html>";
            
            string htmlPath = Path.ChangeExtension(imagePath, ".html");
            File.WriteAllText(htmlPath, htmlContent, System.Text.Encoding.UTF8);
            
            UnityEngine.Debug.Log($"多页HTML生成成功: {htmlPath} ({totalPages} 页)");
            return htmlPath;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"生成HTML失败: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 单页HTML生成（用于小图片）
    /// </summary>
    private string ConvertImageToHTMLSinglePage(string imagePath, string title)
    {
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
        .footer {{ margin-top: 12px; text-align: center; font-size: 12px; color: #666; }}
        @media print {{
            .no-print {{ display: none; }}
            .image {{ max-height: 100vh; }}
        }}
    </style>
</head>
<body>
    <div class='content'>
        <img src='file:///{imagePath.Replace("\\", "/")}' alt='截图' class='image'>
        <div class='footer'>{_companyName}</div>
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
    
    /// <summary>
    /// 获取图片尺寸
    /// </summary>
    private Vector2Int GetImageSize(string imagePath)
    {
        try
        {
            if (!File.Exists(imagePath))
                return Vector2Int.zero;
            
            // 读取文件头信息来获取尺寸（避免加载整张图片到内存）
            byte[] fileData = File.ReadAllBytes(imagePath);
            
            // 使用 Texture2D.LoadImage 来获取尺寸（仅加载元数据）
            Texture2D tempTexture = new Texture2D(2, 2);
            if (tempTexture.LoadImage(fileData, false)) // markNonReadable=false 可以更快
            {
                Vector2Int size = new Vector2Int(tempTexture.width, tempTexture.height);
                UnityEngine.Object.DestroyImmediate(tempTexture);
                return size;
            }
            
            UnityEngine.Object.DestroyImmediate(tempTexture);
            return Vector2Int.zero;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"获取图片尺寸失败: {e.Message}");
            return Vector2Int.zero;
        }
    }
    
    /// <summary>
    /// 基于像素的智能回溯切分：尝试在每页边界附近回溯查找“空白行/区域”（以白色为主）
    /// 返回缩放后坐标系（scaled）下的切片起始位置列表（单位：像素，top-origin）
    /// </summary>
    private List<float> ComputeSafeSliceStarts(Texture2D tex, Vector2Int imageSize, int contentWidth, int contentHeight, float scaleX, int maxSearchUpScaledPx = 160, int sampleHeightOrig = 3, int sampleStepX = 3, float darkPixelThreshold = 0.01f, int consecutiveWhiteLines = 3, float reserveScaledPx = 6f)
    {
        var result = new List<float>();
        if (tex == null || imageSize.x <= 0 || imageSize.y <= 0) return result;

        float scaledHeight = imageSize.y * scaleX;
        int approxPages = Mathf.CeilToInt(scaledHeight / contentHeight);
        if (approxPages <= 0) approxPages = 1;

        // helper: 返回指定原始图片行附近的“暗色像素占比”（0~1）
        float GetDarkRatioAtOrigY(int origY)
        {
            origY = Mathf.Clamp(origY, 0, imageSize.y - 1);
            int halfH = Mathf.Max(1, sampleHeightOrig / 2);
            int yMin = Mathf.Clamp(origY - halfH, 0, imageSize.y - 1);
            int yMax = Mathf.Clamp(origY + halfH, 0, imageSize.y - 1);

            long darkCount = 0;
            long sampleCount = 0;

            // 只在图片中间区域采样，避免页眉/页边线影响（排除左右各10%区域）
            int xStart = Mathf.RoundToInt(imageSize.x * 0.10f);
            int xEnd = Mathf.RoundToInt(imageSize.x * 0.90f);
            xStart = Mathf.Clamp(xStart, 0, imageSize.x - 1);
            xEnd = Mathf.Clamp(xEnd, 0, imageSize.x - 1);
            for (int y = yMin; y <= yMax; y++)
            {
                for (int x = xStart; x <= xEnd; x += Mathf.Max(1, sampleStepX))
                {
                    Color c = tex.GetPixel(x, y);
                    // 计算亮度
                    float lum = 0.299f * c.r + 0.587f * c.g + 0.114f * c.b;
                    if (lum < 0.95f) darkCount++;
                    sampleCount++;
                }
            }

            if (sampleCount == 0) return 0f;
            return (float)darkCount / (float)sampleCount;
        }

        float prevEndScaled = 0f;
        for (int p = 0; p < approxPages; p++)
        {
            float desiredStartScaled = p * contentHeight;
            if (desiredStartScaled <= 0f)
            {
                result.Add(0f);
                prevEndScaled = 0f;
                continue;
            }

            // 如果已经接近最后一页，直接使用剩余
            if (desiredStartScaled >= scaledHeight - 1f)
            {
                result.Add(Mathf.Max(0f, scaledHeight - contentHeight));
                break;
            }

            // 在 scaled 坐标系向上回溯寻找空白（单位：scaled px），并映射到原始像素采样
            float foundScaled = desiredStartScaled;
            bool found = false;
            int maxSearch = Mathf.Max(1, maxSearchUpScaledPx);
            for (int offset = 0; offset <= maxSearch; offset++)
            {
                float tryScaled = desiredStartScaled - offset;
                if (tryScaled <= prevEndScaled + 1f) break; // 防止和上一页重叠或倒退太多
                // map to original image Y (top-origin)
                int origYTop = Mathf.Clamp(Mathf.RoundToInt(tryScaled / scaleX), 0, imageSize.y - 1);
                // Convert top-origin to texture bottom-origin coordinate for GetPixel:
                int origYForGetPixel = Mathf.Clamp(imageSize.y - 1 - origYTop, 0, imageSize.y - 1);
                // 检测连续 N 行是否为空白（暗色占比低于阈值）
                int consec = 0;
                float avgDark = 0f;
                for (int k = 0; k < consecutiveWhiteLines; k++)
                {
                    int checkY = origYForGetPixel - k; // 向下（原图top-origin向下对应GetPixel y减少）
                    if (checkY < 0) break;
                    float dr = GetDarkRatioAtOrigY(checkY);
                    avgDark += dr;
                    if (dr <= darkPixelThreshold)
                    {
                        consec++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (consec > 0) avgDark /= consec;

                if (consec >= consecutiveWhiteLines)
                {
                    foundScaled = tryScaled;
                    // 向上回退：将切分点上移 reserve 像素给下一页留白（但不早于 prevEndScaled + 1）
                    float reserved = reserveScaledPx;
                    float adjusted = Mathf.Max(prevEndScaled + 1f, foundScaled - reserved);
                    adjusted = Mathf.Min(adjusted, desiredStartScaled - 1f); // 保守限制，避免超过默认分页
                    if (adjusted != foundScaled)
                    {
                        UnityEngine.Debug.Log($"[ScreenshotPDFUtility] 智能切分 - page {p}: foundScaled={foundScaled:F0} -> adjustedUp={adjusted:F0} (reserved {reserved:F0}px), consecWhite={consec}, avgDark={avgDark:F3}");
                        foundScaled = adjusted;
                    }
                    found = true;
                    UnityEngine.Debug.Log($"[ScreenshotPDFUtility] 智能切分 - page {p}: desiredScaled={desiredStartScaled:F0}, finalFoundScaled={foundScaled:F0}, origYTop={origYTop}, consecWhite={consec}, avgDark={avgDark:F3}");
                    break;
                }
            }

            if (!found)
            {
                // 未找到理想空白时，尽量上移几个像素，避免切到文字中间（保守 -6px）
                foundScaled = Mathf.Max(prevEndScaled + 1f, desiredStartScaled - 6f);
                UnityEngine.Debug.Log($"[ScreenshotPDFUtility] 智能切分未找到空白区 - page {p}: desiredScaled={desiredStartScaled:F0}, fallbackScaled={foundScaled:F0}");
            }

            // 限制范围：允许上一页裁剪到检测到的空白位置（不强制上一页最小高度）
            foundScaled = Mathf.Clamp(foundScaled, prevEndScaled + 1f, Mathf.Max(prevEndScaled + 1f, scaledHeight));
            result.Add(foundScaled);
            prevEndScaled = foundScaled;
        }

        // 给上方页面额外取一点内容（向下移动后续页面起点），默认 4 scaled px
        float upperExtraScaledPx = 4f;
        for (int i = 1; i < result.Count; i++)
        {
            // 直接向下移动后续页起点，不强制与前一页保持单调
            result[i] = Mathf.Min(result[i] + upperExtraScaledPx, scaledHeight);
        }

        return result;
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
            
            // 对于HTML文件，使用Application.OpenURL打开（HybridCLR热更新代码可用）
            if (extension == ".html" || extension == ".htm")
            {
                // 将文件路径转换为file://协议格式
                string fileUrl = "file:///" + filePath.Replace("\\", "/");
                
                try
                {
                    // 方法1：使用Application.OpenURL（HybridCLR热更新代码可用）
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
    /// 获取系统可用打印机名称列表
    /// </summary>
    public List<string> GetAvailablePrinterNames()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        try
        {
            var bridge = GetPrinterBridge();
            if (bridge != null)
            {
                return bridge.GetInstalledPrinters() ?? new List<string>();
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogWarning($"[打印] 获取打印机列表失败: {e.Message}");
        }
#endif
        return new List<string>();
    }

    /// <summary>
    /// 获取系统默认打印机名称
    /// </summary>
    public string GetDefaultPrinterName()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        return 获取默认打印机名称_Windows();
#else
        return null;
#endif
    }

    /// <summary>
    /// 使用指定打印机打印多个文件（Windows）
    /// </summary>
    public bool PrintFilesWithPrinter(string[] filePaths, string printerName)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        List<string> validFiles = 收集有效打印文件(filePaths);
        if (validFiles.Count == 0)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(printerName))
        {
            UnityEngine.Debug.LogError("[打印] 指定打印机为空，无法执行打印");
            return false;
        }

        bool allSuccess = true;
        foreach (var file in validFiles)
        {
            UnityEngine.Debug.Log("[打印] 开始处理文件: " + file);
            if (!发送到指定打印机(file, printerName))
            {
                allSuccess = false;
            }

            Thread.Sleep(200);
        }

        return allSuccess;
#else
        bool allSuccess = true;
        if (filePaths != null)
        {
            foreach (var file in filePaths)
            {
                if (!PrintFile(file))
                {
                    allSuccess = false;
                }
            }
        }
        return allSuccess;
#endif
    }

    /// <summary>
    /// 弹出系统打印机选择并打印多个文件（Windows）
    /// </summary>
    public bool PrintFilesWithPrinterDialog(string[] filePaths)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        List<string> validFiles = 收集有效打印文件(filePaths);
        if (validFiles.Count == 0)
        {
            return false;
        }

        string printerName = 选择打印机_Windows();
        if (string.IsNullOrEmpty(printerName))
        {
            UnityEngine.Debug.Log("[打印] 用户取消选择打印机");
            return false;
        }

        return PrintFilesWithPrinter(validFiles.ToArray(), printerName);
#else
        UnityEngine.Debug.LogWarning("[打印] 当前平台不支持系统打印机选择，将使用默认打印流程");

        bool allSuccess = true;
        if (filePaths != null)
        {
            foreach (var file in filePaths)
            {
                if (!PrintFile(file))
                {
                    allSuccess = false;
                }
            }
        }
        return allSuccess;
#endif
    }

    private List<string> 收集有效打印文件(string[] filePaths)
    {
        if (filePaths == null || filePaths.Length == 0)
        {
            UnityEngine.Debug.LogError("[打印] 没有可打印的文件");
            return new List<string>();
        }

        List<string> validFiles = new List<string>();
        foreach (var path in filePaths)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                validFiles.Add(path);
            }
        }

        if (validFiles.Count == 0)
        {
            UnityEngine.Debug.LogError("[打印] 所有待打印文件均无效");
        }

        return validFiles;
    }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    private string 选择打印机_Windows()
    {
        try
        {
            var bridge = GetPrinterBridge();
            if (bridge == null)
            {
                UnityEngine.Debug.LogWarning("[打印] 自定义打印机选择窗口失败: PrinterBridge 不可用");
                return null;
            }

            string selectedPrinter = bridge.SelectPrinterWithCustomDialog();
            if (string.IsNullOrEmpty(selectedPrinter))
            {
                UnityEngine.Debug.Log("[打印] 用户取消选择打印机或自定义窗口未返回打印机");
            }
            return selectedPrinter;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogWarning($"[打印] 自定义打印机选择窗口失败: {e.Message}\n{e.StackTrace}");
            return null;
        }
    }
    private bool 发送到指定打印机(string filePath, string printerName)
    {
        try
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                UnityEngine.Debug.LogError($"[打印] 文件不存在: {filePath}");
                return false;
            }

            string ext = Path.GetExtension(filePath)?.ToLowerInvariant();
            if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".bmp")
            {
                return 直接打印图片到指定打印机(filePath, printerName);
            }

            if (ext == ".html" || ext == ".htm")
            {
                return 直接打印Html到指定打印机(filePath, printerName);
            }

            if (ext == ".docx" || ext == ".doc")
            {
                return 直接打印Word到指定打印机(filePath, printerName);
            }

            UnityEngine.Debug.LogWarning($"[打印] 文件类型 {ext} 走 printto: {filePath}");
            return 通过Shell发送到指定打印机(filePath, printerName);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"[打印] 发送到指定打印机失败: {filePath}, 错误: {e.Message}\n{e.StackTrace}");
            return false;
        }
    }

    private bool 直接打印图片到指定打印机(string filePath, string printerName)
    {
        try
        {
            string fullFilePath = Path.GetFullPath(filePath);
            string paintExe = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "mspaint.exe");
            if (!File.Exists(paintExe))
            {
                paintExe = "mspaint.exe";
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = paintExe,
                Arguments = $"/pt \"{fullFilePath}\" \"{printerName}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            using (Process p = Process.Start(startInfo))
            {
                if (p == null)
                {
                    UnityEngine.Debug.LogError($"[打印] 启动 mspaint 失败: {filePath}");
                    return false;
                }

                // 等待一小段时间确保命令投递给打印子系统
                p.WaitForExit(60000);
            }

            UnityEngine.Debug.Log($"[打印] 图片已通过 mspaint /pt 发送到打印机[{printerName}]: {filePath}");
            return true;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogWarning($"[打印] mspaint /pt 失败，尝试 printto。文件: {filePath}, 错误: {e.Message}");
            return 通过Shell发送到指定打印机(filePath, printerName);
        }
    }

    private bool 直接打印Html到指定打印机(string filePath, string printerName)
    {
        string oldDefaultPrinter = null;
        string oldHeader = null;
        string oldFooter = null;
        bool hadHeader = false;
        bool hadFooter = false;
        bool changedIEPageSetup = false;

        try
        {
            oldDefaultPrinter = 获取默认打印机名称_Windows();
            if (!设置默认打印机_Windows(printerName))
            {
                UnityEngine.Debug.LogWarning($"[打印] 设置默认打印机失败，回退 printto。目标打印机: {printerName}");
                return 通过Shell发送到指定打印机(filePath, printerName);
            }

            // PrintHTML 会读取 IE 的页眉页脚配置，先临时清空，避免“位置/页码”等附加信息
            changedIEPageSetup = 临时清空IE页眉页脚_打印(filePath, out oldHeader, out oldFooter, out hadHeader, out hadFooter);

            var bridge = GetPrinterBridge();
            if (bridge == null)
            {
                UnityEngine.Debug.LogError("[打印] PrinterBridge 不可用，无法执行HTML系统打印");
                return false;
            }

            if (!bridge.PrintHtmlFile(filePath))
            {
                UnityEngine.Debug.LogError($"[打印] HTML直接打印失败(桥接): {filePath}");
                return false;
            }

            UnityEngine.Debug.Log($"[打印] HTML已发送到打印机[{printerName}]：{filePath}");
            return true;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"[打印] HTML直接打印失败: {filePath}, 错误: {e.Message}\n{e.StackTrace}");
            return false;
        }
        finally
        {
            if (changedIEPageSetup)
            {
                恢复IE页眉页脚_打印(oldHeader, oldFooter, hadHeader, hadFooter);
            }

            if (!string.IsNullOrEmpty(oldDefaultPrinter) && !string.Equals(oldDefaultPrinter, printerName, StringComparison.OrdinalIgnoreCase))
            {
                if (!设置默认打印机_Windows(oldDefaultPrinter))
                {
                    UnityEngine.Debug.LogWarning($"[打印] 恢复默认打印机失败: {oldDefaultPrinter}");
                }
            }
        }
    }

    private bool 直接打印Word到指定打印机(string filePath, string printerName)
    {
        string oldDefaultPrinter = null;
        bool needRestoreOldDefault = false;
        bool useDelayedRestore = false;
        try
        {
            oldDefaultPrinter = 获取默认打印机名称_Windows();
            needRestoreOldDefault = !string.IsNullOrEmpty(oldDefaultPrinter) &&
                                    !string.Equals(oldDefaultPrinter, printerName, StringComparison.OrdinalIgnoreCase);

            if (!设置默认打印机_Windows(printerName))
            {
                UnityEngine.Debug.LogWarning($"[打印] Word打印设置默认打印机失败，回退 printto。目标打印机: {printerName}");
                return 通过Shell发送到指定打印机(filePath, printerName);
            }

            // 1) 先尝试系统 print 动词（依赖文件关联，通常比命令行 WPS 更稳）
            if (通过ShellPrint动词打印(filePath))
            {
                useDelayedRestore = true;
                UnityEngine.Debug.Log($"[打印] Word已通过Shell print发送到打印机[{printerName}]：{filePath}");
                return true;
            }

            // 2) 不走WPS命令行，直接回退系统 printto
            UnityEngine.Debug.LogWarning($"[打印] Word的Shell print失败，回退系统printto: {filePath}");
            return 通过Shell发送到指定打印机(filePath, printerName);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"[打印] Word直接打印失败: {filePath}, 错误: {e.Message}\n{e.StackTrace}");
            return false;
        }
        finally
        {
            if (needRestoreOldDefault)
            {
                if (useDelayedRestore)
                {
                    延迟恢复默认打印机_Windows(oldDefaultPrinter, 12000);
                }
                else if (!设置默认打印机_Windows(oldDefaultPrinter))
                {
                    UnityEngine.Debug.LogWarning($"[打印] 恢复默认打印机失败: {oldDefaultPrinter}");
                }
            }
        }
    }

    private void 延迟恢复默认打印机_Windows(string oldDefaultPrinter, int delayMs)
    {
        if (string.IsNullOrEmpty(oldDefaultPrinter))
        {
            return;
        }

        ThreadPool.QueueUserWorkItem(_ =>
        {
            try
            {
                Thread.Sleep(Math.Max(1000, delayMs));
                if (!设置默认打印机_Windows(oldDefaultPrinter))
                {
                    UnityEngine.Debug.LogWarning($"[打印] 延迟恢复默认打印机失败: {oldDefaultPrinter}");
                }
                else
                {
                    UnityEngine.Debug.Log($"[打印] 已延迟恢复默认打印机: {oldDefaultPrinter}");
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning($"[打印] 延迟恢复默认打印机异常: {e.Message}");
            }
        });
    }

    private bool 尝试通过Wps打印Word(string filePath, string printerName)
    {
        try
        {
            string wpsPath = 获取Wps可执行路径_Windows();
            if (string.IsNullOrEmpty(wpsPath) || !File.Exists(wpsPath))
            {
                UnityEngine.Debug.LogWarning("[打印] 未找到WPS可执行文件，跳过WPS命令行打印");
                return false;
            }

            string fullPath = Path.GetFullPath(filePath);
            bool isKsoLauncher = Path.GetFileName(wpsPath).Equals("ksolaunch.exe", StringComparison.OrdinalIgnoreCase);

            List<string> argsList = new List<string>();
            if (isKsoLauncher)
            {
                if (!string.IsNullOrEmpty(printerName))
                {
                    argsList.Add($"/wps /pt \"{fullPath}\" \"{printerName}\"");
                }
                argsList.Add($"/wps /p \"{fullPath}\"");
            }
            else
            {
                if (!string.IsNullOrEmpty(printerName))
                {
                    argsList.Add($"/pt \"{fullPath}\" \"{printerName}\"");
                }
                argsList.Add($"/p \"{fullPath}\"");
            }

            foreach (string args in argsList)
            {
                if (尝试启动Wps进程(wpsPath, args, false, out string failReason))
                {
                    return true;
                }

                // 某些机器在 UseShellExecute=true 下才能正确处理命令行参数。
                if (尝试启动Wps进程(wpsPath, args, true, out failReason))
                {
                    return true;
                }

                UnityEngine.Debug.LogWarning($"[打印] WPS命令行尝试失败，Path={wpsPath}, Args={args}, Reason={failReason}");
            }

            return false;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogWarning($"[打印] WPS命令行打印失败: {e.Message}");
            return false;
        }
    }

    private bool 尝试启动Wps进程(string wpsPath, string args, bool useShellExecute, out string failReason)
    {
        failReason = string.Empty;
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = wpsPath,
                Arguments = args,
                UseShellExecute = useShellExecute,
                CreateNoWindow = !useShellExecute,
                WindowStyle = useShellExecute ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden,
                WorkingDirectory = Path.GetDirectoryName(wpsPath) ?? string.Empty
            };

            using (Process p = Process.Start(startInfo))
            {
                if (p == null)
                {
                    failReason = "Process.Start returned null";
                    return false;
                }
            }

            // 对于 WPS 的 /p /pt 打印命令，不以进程退出码判定成功。
            // WPS 常为单实例/异步投递，WaitForExit 可能产生“Success”伪异常并误判失败。
            Thread.Sleep(1200);
            return true;
        }
        catch (Exception e)
        {
            failReason = $"{e.GetType().Name}: {e.Message}";
            return false;
        }
    }

    private string 获取Wps可执行路径_Windows()
    {
        // 1) 注册表 App Paths（最可靠）
        string[] appKeys =
        {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\wps.exe",
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\ksolaunch.exe",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths\wps.exe",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths\ksolaunch.exe",
        };

        foreach (var key in appKeys)
        {
            string path = 从注册表读取可执行路径(Registry.CurrentUser, key);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                return path;
            }

            path = 从注册表读取可执行路径(Registry.LocalMachine, key);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                return path;
            }
        }

        // 2) 常见安装目录
        string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        string[] candidates =
        {
            Path.Combine(programFiles, "Kingsoft", "WPS Office", "office6", "wps.exe"),
            Path.Combine(programFilesX86, "Kingsoft", "WPS Office", "office6", "wps.exe"),
            Path.Combine(programFiles, "WPS Office", "office6", "wps.exe"),
            Path.Combine(programFilesX86, "WPS Office", "office6", "wps.exe"),
            Path.Combine(programFiles, "Kingsoft", "WPS Office", "ksolaunch.exe"),
            Path.Combine(programFilesX86, "Kingsoft", "WPS Office", "ksolaunch.exe"),
            Path.Combine(programFiles, "WPS Office", "ksolaunch.exe"),
            Path.Combine(programFilesX86, "WPS Office", "ksolaunch.exe"),
        };

        foreach (var path in candidates)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                return path;
            }
        }

        return null;
    }

    private string 从注册表读取可执行路径(RegistryKey root, string subKey)
    {
        try
        {
            using (RegistryKey key = root.OpenSubKey(subKey))
            {
                if (key == null)
                {
                    return null;
                }

                string rawPath = key.GetValue(string.Empty, null)?.ToString();
                if (string.IsNullOrWhiteSpace(rawPath))
                {
                    return null;
                }

                rawPath = Environment.ExpandEnvironmentVariables(rawPath.Trim());

                // App Paths 可能带参数，例如: "C:\\xxx\\wps.exe" "%1"
                if (rawPath.StartsWith("\"", StringComparison.Ordinal))
                {
                    int endQuote = rawPath.IndexOf('"', 1);
                    if (endQuote > 1)
                    {
                        rawPath = rawPath.Substring(1, endQuote - 1);
                    }
                }
                else
                {
                    int exeIndex = rawPath.IndexOf(".exe", StringComparison.OrdinalIgnoreCase);
                    if (exeIndex > 0)
                    {
                        rawPath = rawPath.Substring(0, exeIndex + 4);
                    }
                }

                return rawPath.Trim();
            }
        }
        catch
        {
            return null;
        }
    }

    private bool 通过ShellPrint动词打印(string filePath)
    {
        try
        {
            var bridge = GetPrinterBridge();
            if (bridge == null)
            {
                UnityEngine.Debug.LogWarning("[打印] PrinterBridge 不可用，Shell print无法执行");
                return false;
            }

            if (!bridge.ShellPrintFile(filePath))
            {
                return false;
            }

            return true;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogWarning($"[打印] Shell print 动词失败: {e.Message}");
            return false;
        }
    }

    private bool 临时清空IE页眉页脚_打印(string filePath, out string oldHeader, out string oldFooter, out bool hadHeader, out bool hadFooter)
    {
        oldHeader = null;
        oldFooter = null;
        hadHeader = false;
        hadFooter = false;

        try
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\\Microsoft\\Internet Explorer\\PageSetup"))
            {
                if (key == null)
                {
                    UnityEngine.Debug.LogWarning("[打印] 无法访问IE PageSetup配置，继续打印");
                    return false;
                }

                object headerValue = key.GetValue("header", null);
                object footerValue = key.GetValue("footer", null);

                if (headerValue != null)
                {
                    hadHeader = true;
                    oldHeader = headerValue.ToString();
                }

                if (footerValue != null)
                {
                    hadFooter = true;
                    oldFooter = footerValue.ToString();
                }

                key.SetValue("header", string.Empty, RegistryValueKind.String);
                key.SetValue("footer", string.Empty, RegistryValueKind.String);
                return true;
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogWarning($"[打印] 清空IE页眉页脚失败，文件: {filePath}, 错误: {e.Message}");
            return false;
        }
    }

    private void 恢复IE页眉页脚_打印(string oldHeader, string oldFooter, bool hadHeader, bool hadFooter)
    {
        try
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\\Microsoft\\Internet Explorer\\PageSetup"))
            {
                if (key == null)
                {
                    return;
                }

                if (hadHeader)
                {
                    key.SetValue("header", oldHeader ?? string.Empty, RegistryValueKind.String);
                }
                else
                {
                    key.DeleteValue("header", false);
                }

                if (hadFooter)
                {
                    key.SetValue("footer", oldFooter ?? string.Empty, RegistryValueKind.String);
                }
                else
                {
                    key.DeleteValue("footer", false);
                }
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogWarning($"[打印] 恢复IE页眉页脚失败: {e.Message}");
        }
    }
    private string 获取默认打印机名称_Windows()
    {
        try
        {
            var bridge = GetPrinterBridge();
            if (bridge != null)
            {
                return bridge.GetDefaultPrinterName();
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogWarning($"[打印] 获取默认打印机失败: {e.Message}");
        }

        return null;
    }

    private bool 设置默认打印机_Windows(string printerName)
    {
        if (string.IsNullOrEmpty(printerName))
        {
            return false;
        }
        try
        {
            var bridge = GetPrinterBridge();
            bool ok = bridge != null && bridge.SetDefaultPrinterName(printerName);
            if (!ok)
            {
                UnityEngine.Debug.LogWarning($"[打印] SetDefaultPrinter失败: {printerName}");
            }
            return ok;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogWarning($"[打印] SetDefaultPrinter异常: {e.Message}");
            return false;
        }
    }
    private bool 通过Shell发送到指定打印机(string filePath, string printerName)
    {
        string fullPath = Path.GetFullPath(filePath);
        try
        {
            var bridge = GetPrinterBridge();
            if (bridge == null)
            {
                UnityEngine.Debug.LogWarning("[打印] PrinterBridge 不可用，printto无法执行");
            }
            else if (bridge.ShellPrintToFile(fullPath, printerName))
            {
                UnityEngine.Debug.Log($"[打印] 已通过printto发送到打印机[{printerName}]: {filePath}");
                return true;
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogWarning($"[打印] printto 发送失败，回退默认打印机+print。文件: {filePath}, 错误: {e.Message}");
        }

        try
        {
            if (设置默认打印机_Windows(printerName) && 通过ShellPrint动词打印(fullPath))
            {
                UnityEngine.Debug.Log($"[打印] 已通过设置默认打印机+Shell print 发送到打印机[{printerName}]: {filePath}");
                return true;
            }
        }
        catch (Exception e2)
        {
            UnityEngine.Debug.LogWarning($"[打印] 默认打印机+print 回退失败: {e2.Message}");
        }

        UnityEngine.Debug.LogError($"[打印] printto 与默认打印机+print 都失败: {filePath}");
        return false;
    }
#endif
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


