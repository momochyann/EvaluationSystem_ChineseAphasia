using UnityEngine;
using QFramework;
using Cysharp.Threading.Tasks;
using Nova;
using NovaSamples.UIControls;

public class 图片选择组件 : MonoBehaviour, IController
{
    [SerializeField] private UIBlock2D imageDisplay;          // 图片显示框
    [SerializeField] private NovaSamples.UIControls.Button imageButton; // NovaUI按钮
    [SerializeField] bool 是否显示默认图片 = false;
    private Texture2D currentTexture;
    private const string IMAGE_NAME_PREFIX = "user_image_";

    void Start()
    {
        InitializeUI();
        if (是否显示默认图片)
        {
            LoadLastImage();
        }
    }

    private void InitializeUI()
    {
        // 设置NovaUI按钮点击事件
        if (imageButton != null)
        {
            imageButton.OnClicked.AddListener(() => OnImageClicked().Forget());
        }
        else if (imageDisplay != null)
        {
            // 如果没有单独的按钮，尝试从imageDisplay获取按钮组件
            var buttonComponent = imageDisplay.GetComponent<NovaSamples.UIControls.Button>();
            if (buttonComponent != null)
            {
                buttonComponent.OnClicked.AddListener(() => OnImageClicked().Forget());
            }
            else
            {
                Debug.LogWarning("图片选择组件: 没有找到NovaUI按钮组件");
            }
        }
    }

    /// <summary>
    /// 图片被点击时的处理 - 直接打开相册
    /// </summary>
    private async UniTaskVoid OnImageClicked()
    {
        try
        {
            Debug.Log("打开相册选择图片");
#if UNITY_STANDALONE_WIN
            bool IsConfig = await 显示提示("自动选择系统最新添加的图片，请注意查看");
            if (!IsConfig)
            {
                return;
            }
#endif
            var imageUtility = this.GetUtility<ImagePickerUtility>();

            var newTexture = await imageUtility.PickImageFromGalleryAsync(1024);

            if (newTexture != null)
            {
                // 生成基于时间戳的文件名
                string fileName = GenerateImageFileName();

                // 自动保存到缓存
                string savedPath = await imageUtility.SaveImageToCacheAsync(newTexture, fileName);

                if (!string.IsNullOrEmpty(savedPath))
                {
                    // 显示新图片
                    SetDisplayImage(newTexture);
                    Debug.Log($"图片已保存并显示: {fileName}");
                    Debug.Log($"缓存路径: {savedPath}");
                }
                else
                {
                    // 保存失败，清理纹理
                    UnityEngine.Object.Destroy(newTexture);
                    Debug.LogWarning("图片保存失败");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"图片选择失败: {e.Message}");
        }
    }
    private async UniTask<bool> 显示提示(string 消息)
    {
        try
        {
            var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
            var 告示实例 = Instantiate(告示, transform.parent.parent);
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

    /// <summary>
    /// 启动时加载上次显示的图片
    /// </summary>
    private async void LoadLastImage()
    {
        try
        {
            var imageUtility = this.GetUtility<ImagePickerUtility>();
            string lastImageName = imageUtility.GetLastImageRecord();

            if (!string.IsNullOrEmpty(lastImageName) && imageUtility.IsImageCached(lastImageName))
            {
                var texture = await imageUtility.LoadImageFromCacheAsync(lastImageName);
                if (texture != null)
                {
                    SetDisplayImage(texture);
                    Debug.Log($"自动加载上次图片: {lastImageName}");
                    Debug.Log($"图片路径: {imageUtility.GetCachedImagePath(lastImageName)}");
                }
            }
            else
            {
                Debug.Log("没有找到上次的图片，显示默认状态");
                SetDefaultImage();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"加载上次图片失败: {e.Message}");
            SetDefaultImage();
        }
    }

    /// <summary>
    /// 设置显示的图片
    /// </summary>
    public void SetDisplayImage(Texture2D texture)
    {
        if (imageDisplay == null || texture == null) return;

        // 清理之前的纹理
        if (currentTexture != null)
        {
            UnityEngine.Object.Destroy(currentTexture);
        }

        currentTexture = texture;

        // 使用NovaUI的UIBlock2D设置图片
        imageDisplay.SetImage(texture);
        imageDisplay.Color = Color.white; // 确保显示正常颜色

        Debug.Log($"图片显示成功: {texture.width}x{texture.height}");
    }

    /// <summary>
    /// 设置默认图片显示
    /// </summary>
    private void SetDefaultImage()
    {
        if (imageDisplay != null)
        {
            imageDisplay.SetImage((Texture2D)null); // 清空图片
            imageDisplay.Color = new Color(0.8f, 0.8f, 0.8f, 1f); // 浅灰色背景
        }
    }

    /// <summary>
    /// 生成基于时间戳的图片文件名
    /// </summary>
    private string GenerateImageFileName()
    {
        return $"{IMAGE_NAME_PREFIX}{System.DateTime.Now:yyyyMMdd_HHmmss}";
    }

    /// <summary>
    /// 获取当前显示图片的缓存路径（供其他地方使用）
    /// </summary>
    public string GetCurrentImageCachePath()
    {
        var imageUtility = this.GetUtility<ImagePickerUtility>();
        string lastImageName = imageUtility.GetLastImageRecord();

        if (!string.IsNullOrEmpty(lastImageName))
        {
            return imageUtility.GetCachedImagePath(lastImageName);
        }

        return null;
    }
    public string GetCurrentImageCacheName()
    {
        var imageUtility = this.GetUtility<ImagePickerUtility>();
        string lastImageName = imageUtility.GetLastImageRecord();
        return lastImageName;
    }
    //  public Texture2D 
    private void OnDestroy()
    {
        // 清理事件监听
        if (imageButton != null)
        {
            imageButton.OnClicked.RemoveAllListeners();
        }

        // 清理纹理资源
        if (currentTexture != null)
        {
            UnityEngine.Object.Destroy(currentTexture);
            currentTexture = null;
        }
    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}