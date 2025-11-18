using UnityEngine;
using QFramework;
using Cysharp.Threading.Tasks;
using Nova;

/// <summary>
/// 将界面管理界面_茉中图片选择器设置的图片同步到挂载的UIBlock2D
/// </summary>
public class UIBlock2D图片同步器_茉 : MonoBehaviour, IController
{
    [SerializeField] private UIBlock2D targetUIBlock2D;
    private 软件配置录 配置录;
    private Texture2D currentTexture;

    void Start()
    {
        // 如果没有手动指定，自动获取挂载的UIBlock2D
        if (targetUIBlock2D == null)
        {
            targetUIBlock2D = GetComponent<UIBlock2D>();
        }

        if (targetUIBlock2D == null)
        {
            Debug.LogWarning($"[UIBlock2D图片同步器_茉] {gameObject.name} 未找到UIBlock2D组件");
            return;
        }

        配置录 = this.GetModel<软件配置录>();
        
        // 加载图片
        加载配置图片().Forget();
        
        // 监听配置修改事件
        配置录.软件修改事件 += 配置修改时更新图片;
    }

    /// <summary>
    /// 加载配置中的图片
    /// </summary>
    private async UniTaskVoid 加载配置图片()
    {
        try
        {
            var config = 配置录.当前配置;
            if (config == null || string.IsNullOrEmpty(config.图片缓存名称))
            {
                Debug.Log("[UIBlock2D图片同步器_茉] 配置中没有图片缓存名称");
                return;
            }

            var imageUtility = this.GetUtility<ImagePickerUtility>();
            var texture = await imageUtility.LoadImageFromCacheAsync(config.图片缓存名称);
            
            if (texture != null)
            {
                设置图片(texture);
            }
            else
            {
                Debug.LogWarning($"[UIBlock2D图片同步器_茉] 加载图片失败: {config.图片缓存名称}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[UIBlock2D图片同步器_茉] 加载图片异常: {e.Message}");
        }
    }

    /// <summary>
    /// 配置修改时更新图片
    /// </summary>
    private void 配置修改时更新图片(软件配置 配置)
    {
        加载配置图片().Forget();
    }

    /// <summary>
    /// 设置图片到UIBlock2D
    /// </summary>
    private void 设置图片(Texture2D texture)
    {
        if (targetUIBlock2D == null || texture == null) return;

        // 清理之前的纹理
        if (currentTexture != null)
        {
            UnityEngine.Object.Destroy(currentTexture);
        }

        currentTexture = texture;
        targetUIBlock2D.SetImage(texture);
        targetUIBlock2D.Color = Color.white;
    }

    void OnDestroy()
    {
        // 取消事件监听
        if (配置录 != null)
        {
            配置录.软件修改事件 -= 配置修改时更新图片;
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

