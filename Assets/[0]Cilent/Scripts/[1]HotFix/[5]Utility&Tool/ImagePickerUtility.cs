using UnityEngine;
using QFramework;
using Cysharp.Threading.Tasks;
using System;
using System.IO;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif


public interface IImagePickerUtility : IUtility
{
    /// <summary>
    /// 从相册选择图片
    /// </summary>
    /// <param name="maxSize">图片最大尺寸，-1表示不限制</param>
    UniTask<Texture2D> PickImageFromGalleryAsync(int maxSize = 1024);
    
    /// <summary>
    /// 保存图片到缓存目录
    /// </summary>
    /// <param name="texture">要保存的纹理</param>
    /// <param name="fileName">文件名（不含扩展名）</param>
    /// <returns>保存的文件路径</returns>
    UniTask<string> SaveImageToCacheAsync(Texture2D texture, string fileName);
    
    /// <summary>
    /// 从缓存目录加载图片
    /// </summary>
    /// <param name="fileName">文件名（不含扩展名）</param>
    UniTask<Texture2D> LoadImageFromCacheAsync(string fileName);
    
    /// <summary>
    /// 检查缓存中是否存在指定图片
    /// </summary>
    /// <param name="fileName">文件名（不含扩展名）</param>
    bool IsImageCached(string fileName);
    
    /// <summary>
    /// 删除缓存中的图片
    /// </summary>
    /// <param name="fileName">文件名（不含扩展名）</param>
    bool DeleteCachedImage(string fileName);
    
    /// <summary>
    /// 清空图片缓存
    /// </summary>
    void ClearImageCache();
    
    /// <summary>
    /// 保存当前显示图片的记录
    /// </summary>
    /// <param name="fileName">当前显示的图片文件名</param>
    void SaveCurrentImageRecord(string fileName);
    
    /// <summary>
    /// 获取上次显示的图片记录
    /// </summary>
    /// <returns>图片文件名，如果没有记录返回null</returns>
    string GetLastImageRecord();
    
    /// <summary>
    /// 获取缓存图片的完整路径
    /// </summary>
    /// <param name="fileName">文件名（不含扩展名）</param>
    /// <returns>完整的文件路径</returns>
    string GetCachedImagePath(string fileName);
}

public class ImagePickerUtility : IImagePickerUtility
{
    private string ImageCacheDirectory => Path.Combine(Application.persistentDataPath, "ImageCache");
    private const string LAST_IMAGE_RECORD_KEY = "LastDisplayedImage";
    
    // 自制文件选择器的配置
    private static readonly string[] ImageExtensions = { "*.png", "*.jpg", "*.jpeg", "*.bmp", "*.tga", "*.gif" };
    private static readonly string[] SearchDirectories = {
        "MyPictures",
        "Desktop", 
        "Downloads",
        "Documents"
    };
    
    /// <summary>
    /// 从相册选择图片
    /// </summary>
     public async UniTask<Texture2D> PickImageFromGalleryAsync(int maxSize = 1024)
    {
        try
        {
            Debug.Log("开始选择图片...");
            Debug.Log($"当前平台: {Application.platform}");
            Debug.Log($"是否为编辑器: {Application.isEditor}");
            
            // 检查是否为PC平台
            bool isPCPlatform = IsPCPlatform();
            Debug.Log($"是否为PC平台: {isPCPlatform}");
            
            if (isPCPlatform)
            {
                Debug.Log("使用PC端文件对话框选择图片");
                return await PickImageFromFileDialogAsync(maxSize);
            }
            
            // 移动平台使用相册
            Debug.Log("使用移动端相册选择图片");
            return await PickImageFromMobileGalleryAsync(maxSize);
        }
        catch (Exception e)
        {
            Debug.LogError($"选择图片失败: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// PC平台使用文件选择器选择图片
    /// </summary>
    private async UniTask<Texture2D> PickImageFromFileDialogAsync(int maxSize = 1024)
    {
        try
        {
            Debug.Log("PC端开始选择图片...");
            
#if UNITY_EDITOR
            // 编辑器模式使用EditorUtility
            string path = EditorUtility.OpenFilePanel("选择图片", "", "png,jpg,jpeg,bmp,tga");
            if (string.IsNullOrEmpty(path))
            {
                Debug.Log("用户取消选择图片");
                return null;
            }
            
            return await LoadTextureFromFileAsync(path, maxSize);
#else
            // PC运行时模式使用自制文件选择器
            Debug.Log("使用自制文件选择器");
            string path = await ShowCustomFilePickerAsync();
            
            Debug.Log($"文件选择器返回路径: {path}");
            
            if (string.IsNullOrEmpty(path))
            {
                Debug.Log("用户取消选择或未找到图片");
                return null;
            }
            
            Debug.Log($"准备加载图片文件: {path}");
            return await LoadTextureFromFileAsync(path, maxSize);
#endif
        }
        catch (Exception e)
        {
            Debug.LogError($"PC端选择图片失败: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 移动平台使用相册选择图片
    /// </summary>
    private async UniTask<Texture2D> PickImageFromMobileGalleryAsync(int maxSize = 1024)
    {
        try
        {
            Debug.Log("移动端开始从相册选择图片...");
            
            // 双重检查确保是移动平台
            bool isMobilePlatform = Application.platform == RuntimePlatform.Android ||
                                   Application.platform == RuntimePlatform.IPhonePlayer;
            
            if (!isMobilePlatform)
            {
                Debug.LogError("当前不是移动平台，无法使用相册功能");
                return CreateTestTexture(); // 返回测试纹理作为备用
            }
            
            // 检查权限
            if (!await RequestGalleryPermissionAsync())
            {
                Debug.LogError("没有相册访问权限");
                return null;
            }

            // 使用NativeGallery选择图片
            var tcs = new UniTaskCompletionSource<Texture2D>();
            
            Debug.Log("调用NativeGallery.GetImageFromGallery");
            NativeGallery.GetImageFromGallery((path) =>
            {
                if (string.IsNullOrEmpty(path))
                {
                    Debug.Log("用户取消选择图片");
                    tcs.TrySetResult(null);
                    return;
                }
                
                // 加载选择的图片
                try
                {
                    Debug.Log($"加载图片: {path}");
                    Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
                    
                    if (texture != null)
                    {
                        Debug.Log($"图片加载成功: {texture.width}x{texture.height}");
                        tcs.TrySetResult(texture);
                    }
                    else
                    {
                        Debug.LogError("图片加载失败");
                        tcs.TrySetResult(null);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"加载图片异常: {e.Message}");
                    tcs.TrySetResult(null);
                }
            }, "选择图片", "image/*");
            
            return await tcs.Task;
        }
        catch (Exception e)
        {
            Debug.LogError($"移动端选择图片失败: {e.Message}");
            return CreateTestTexture(); // 出错时返回测试纹理
        }
    }
    
    /// <summary>
    /// 从文件路径加载纹理
    /// </summary>
    private async UniTask<Texture2D> LoadTextureFromFileAsync(string filePath, int maxSize = 1024)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"文件不存在: {filePath}");
                return null;
            }
            
            // 异步读取文件
            byte[] fileData = await File.ReadAllBytesAsync(filePath);
            
            // 创建纹理
            Texture2D texture = new Texture2D(2, 2);
            
            if (texture.LoadImage(fileData))
            {
                // 如果需要缩放
                if (maxSize > 0 && (texture.width > maxSize || texture.height > maxSize))
                {
                    texture = ResizeTexture(texture, maxSize);
                }
                
                Debug.Log($"图片加载成功: {filePath} ({texture.width}x{texture.height})");
                return texture;
            }
            else
            {
                Debug.LogError($"无法加载图片: {filePath}");
                UnityEngine.Object.Destroy(texture);
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"加载图片文件异常: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 调整纹理大小
    /// </summary>
    private Texture2D ResizeTexture(Texture2D source, int maxSize)
    {
        if (source == null) return null;
        
        int newWidth = source.width;
        int newHeight = source.height;
        
        // 计算缩放比例
        float scale = 1f;
        if (newWidth > newHeight)
        {
            if (newWidth > maxSize)
            {
                scale = (float)maxSize / newWidth;
            }
        }
        else
        {
            if (newHeight > maxSize)
            {
                scale = (float)maxSize / newHeight;
            }
        }
        
        newWidth = Mathf.RoundToInt(newWidth * scale);
        newHeight = Mathf.RoundToInt(newHeight * scale);
        
        // 创建RenderTexture进行缩放
        RenderTexture renderTexture = RenderTexture.GetTemporary(newWidth, newHeight);
        Graphics.Blit(source, renderTexture);
        
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTexture;
        
        Texture2D resizedTexture = new Texture2D(newWidth, newHeight);
        resizedTexture.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        resizedTexture.Apply();
        
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTexture);
        
        // 销毁原纹理
        UnityEngine.Object.Destroy(source);
        
        return resizedTexture;
    }
    


    /// <summary>
    /// 显示自制文件选择器
    /// </summary>
    private async UniTask<string> ShowCustomFilePickerAsync()
    {
        Debug.Log("=== 自制文件选择器开始执行 ===");
        
        try
        {
            // 搜索图片文件
            var foundImages = await ScanImageFilesAsync();
            
            if (foundImages.Count == 0)
            {
                Debug.LogWarning("未找到任何图片文件");
                return null;
            }
            
            // 选择最新的图片文件
            string selectedPath = SelectLatestImage(foundImages);
            Debug.Log($"自动选择最新图片: {selectedPath}");
            
            return selectedPath;
        }
        catch (Exception e)
        {
            Debug.LogError($"自制文件选择器异常: {e.Message}");
            return null;
        }
        finally
        {
            Debug.Log("=== 自制文件选择器执行结束 ===");
        }
    }
    
    /// <summary>
    /// 扫描图片文件
    /// </summary>
    private async UniTask<System.Collections.Generic.List<FileInfo>> ScanImageFilesAsync()
    {
        var imageFiles = new System.Collections.Generic.List<FileInfo>();
        
        await UniTask.Run(() =>
        {
            Debug.Log("开始扫描图片文件...");
            
            // 获取搜索目录
            string[] searchPaths = GetSearchDirectories();
            
            foreach (string dirPath in searchPaths)
            {
                if (!Directory.Exists(dirPath))
                {
                    Debug.Log($"目录不存在，跳过: {dirPath}");
                    continue;
                }
                
                Debug.Log($"扫描目录: {dirPath}");
                
                try
                {
                    foreach (string extension in ImageExtensions)
                    {
                        string[] files = Directory.GetFiles(dirPath, extension, SearchOption.TopDirectoryOnly);
                        foreach (string file in files)
                        {
                            var fileInfo = new FileInfo(file);
                            imageFiles.Add(fileInfo);
                            Debug.Log($"找到图片: {fileInfo.Name}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"扫描目录失败 {dirPath}: {e.Message}");
                }
            }
            
            Debug.Log($"总共找到 {imageFiles.Count} 个图片文件");
        });
        
        return imageFiles;
    }
    
    /// <summary>
    /// 获取搜索目录路径
    /// </summary>
    private string[] GetSearchDirectories()
    {
        var directories = new System.Collections.Generic.List<string>();
        
        foreach (string dirName in SearchDirectories)
        {
            try
            {
                string path = null;
                switch (dirName)
                {
                    case "MyPictures":
                        path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                        break;
                    case "Desktop":
                        path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        break;
                    case "Downloads":
                        path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                        break;
                    case "Documents":
                        path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        break;
                }
                
                if (!string.IsNullOrEmpty(path))
                {
                    directories.Add(path);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"获取目录路径失败 {dirName}: {e.Message}");
            }
        }
        
        // 添加应用数据目录
        directories.Add(Application.persistentDataPath);
        
        return directories.ToArray();
    }
    
    /// <summary>
    /// 选择最新的图片文件
    /// </summary>
    private string SelectLatestImage(System.Collections.Generic.List<FileInfo> imageFiles)
    {
        if (imageFiles == null || imageFiles.Count == 0)
            return null;
        
        // 按修改时间排序，选择最新的
        var sortedFiles = imageFiles.OrderByDescending(f => f.LastWriteTime).ToArray();
        
        var latestFile = sortedFiles[0];
        Debug.Log($"选择最新图片: {latestFile.FullName} (修改时间: {latestFile.LastWriteTime})");
        
        return latestFile.FullName;
    }
    
    /// <summary>
    /// 检查是否为PC平台
    /// </summary>
    private bool IsPCPlatform()
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
    /// 创建可读的纹理副本
    /// </summary>
    private Texture2D MakeTextureReadable(Texture2D source)
    {
        if (source == null) return null;

        // 创建RenderTexture
        RenderTexture renderTex = RenderTexture.GetTemporary(
            source.width, 
            source.height, 
            0, 
            RenderTextureFormat.Default, 
            RenderTextureReadWrite.Linear);

        // 将源纹理绘制到RenderTexture
        Graphics.Blit(source, renderTex);
        
        // 保存当前活动的RenderTexture
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        
        // 创建新的可读Texture2D
        Texture2D readableTexture = new Texture2D(source.width, source.height);
        readableTexture.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableTexture.Apply();
        
        // 恢复RenderTexture状态
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        
        return readableTexture;
    }

    /// <summary>
    /// 保存图片到缓存目录
    /// </summary>
    public async UniTask<string> SaveImageToCacheAsync(Texture2D texture, string fileName)
    {
        if (texture == null)
        {
            Debug.LogError("纹理为空，无法保存");
            return null;
        }
        
        try
        {
            // 确保缓存目录存在
            EnsureCacheDirectoryExists();
            
            // 构建完整路径
            string filePath = Path.Combine(ImageCacheDirectory, fileName + ".png");
            
            // 创建可读的纹理副本
            Texture2D readableTexture = MakeTextureReadable(texture);
            if (readableTexture == null)
            {
                Debug.LogError("无法创建可读纹理副本");
                return null;
            }
            
            // 将纹理转换为PNG字节数组
            byte[] imageData = readableTexture.EncodeToPNG();
            
            // 清理临时纹理
            UnityEngine.Object.DestroyImmediate(readableTexture);
            
            // 异步写入文件
            await File.WriteAllBytesAsync(filePath, imageData);
            
            // 保存当前图片记录
            SaveCurrentImageRecord(fileName);
            
            Debug.Log($"图片已保存到缓存: {filePath}");
            return filePath;
        }
        catch (Exception e)
        {
            Debug.LogError($"保存图片到缓存失败: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 从缓存目录加载图片
    /// </summary>
    public async UniTask<Texture2D> LoadImageFromCacheAsync(string fileName)
    {
        try
        {
            string filePath = Path.Combine(ImageCacheDirectory, fileName + ".png");
            
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"缓存中不存在图片: {fileName}");
                return null;
            }
            
            // 异步读取文件
            byte[] imageData = await File.ReadAllBytesAsync(filePath);
            
            // 创建纹理并加载图片数据
            Texture2D texture = new Texture2D(2, 2);
            
            if (texture.LoadImage(imageData))
            {
                Debug.Log($"从缓存加载图片成功: {fileName} ({texture.width}x{texture.height})");
                return texture;
            }
            else
            {
                Debug.LogError($"加载缓存图片失败: {fileName}");
                UnityEngine.Object.Destroy(texture);
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"从缓存加载图片异常: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 检查缓存中是否存在指定图片
    /// </summary>
    public bool IsImageCached(string fileName)
    {
        string filePath = Path.Combine(ImageCacheDirectory, fileName + ".png");
        return File.Exists(filePath);
    }
    
    /// <summary>
    /// 删除缓存中的图片
    /// </summary>
    public bool DeleteCachedImage(string fileName)
    {
        try
        {
            string filePath = Path.Combine(ImageCacheDirectory, fileName + ".png");
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"已删除缓存图片: {fileName}");
                return true;
            }
            else
            {
                Debug.LogWarning($"要删除的缓存图片不存在: {fileName}");
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"删除缓存图片失败: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 清空图片缓存
    /// </summary>
    public void ClearImageCache()
    {
        try
        {
            if (Directory.Exists(ImageCacheDirectory))
            {
                Directory.Delete(ImageCacheDirectory, true);
                Debug.Log("图片缓存已清空");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"清空图片缓存失败: {e.Message}");
        }
    }
    
    /// <summary>
    /// 保存当前显示图片的记录
    /// </summary>
    public void SaveCurrentImageRecord(string fileName)
    {
        PlayerPrefs.SetString(LAST_IMAGE_RECORD_KEY, fileName);
        PlayerPrefs.Save();
        Debug.Log($"保存图片记录: {fileName}");
    }
    
    /// <summary>
    /// 获取上次显示的图片记录
    /// </summary>
    public string GetLastImageRecord()
    {
        string lastImage = PlayerPrefs.GetString(LAST_IMAGE_RECORD_KEY, "");
        if (string.IsNullOrEmpty(lastImage))
        {
            Debug.Log("没有找到上次显示的图片记录");
            return null;
        }
        
        Debug.Log($"获取上次图片记录: {lastImage}");
        return lastImage;
    }
    
    /// <summary>
    /// 获取缓存图片的完整路径
    /// </summary>
    public string GetCachedImagePath(string fileName)
    {
        return Path.Combine(ImageCacheDirectory, fileName + ".png");
    }
    
    /// <summary>
    /// 确保缓存目录存在
    /// </summary>
    private void EnsureCacheDirectoryExists()
    {
        if (!Directory.Exists(ImageCacheDirectory))
        {
            Directory.CreateDirectory(ImageCacheDirectory);
            Debug.Log($"创建图片缓存目录: {ImageCacheDirectory}");
        }
    }
    
    /// <summary>
    /// 请求相册访问权限
    /// </summary>
    private async UniTask<bool> RequestGalleryPermissionAsync()
    {
        // 编辑器模式直接返回true
        if (Application.isEditor)
        {
            return true;
        }
        
        try
        {
            // 检查当前权限状态
            if (NativeGallery.CheckPermission(NativeGallery.PermissionType.Read, NativeGallery.MediaType.Image))
            {
                Debug.Log("相册权限已获得");
                return true;
            }
            
            // 请求权限 - 使用Task版本的API
            var permission = await NativeGallery.RequestPermissionAsync(NativeGallery.PermissionType.Read, NativeGallery.MediaType.Image);
            
            bool granted = permission == NativeGallery.Permission.Granted;
            Debug.Log($"相册权限请求结果: {permission} ({granted})");
            
            return granted;
        }
        catch (Exception e)
        {
            Debug.LogError($"检查相册权限失败: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 创建测试纹理（编辑器模式使用）
    /// </summary>
    private Texture2D CreateTestTexture()
    {
        Texture2D testTexture = new Texture2D(256, 256);
        
        for (int x = 0; x < 256; x++)
        {
            for (int y = 0; y < 256; y++)
            {
                float r = (float)x / 256f;
                float g = (float)y / 256f;
                float b = 0.5f;
                testTexture.SetPixel(x, y, new Color(r, g, b, 1f));
            }
        }
        
        testTexture.Apply();
        Debug.Log("创建测试纹理成功");
        return testTexture;
    }
} 