using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 全局键盘管理器 - 处理场景切换和应用生命周期时的键盘清理
/// </summary>
public class GlobalKeyboardManager : MonoBehaviour
{
    private static GlobalKeyboardManager instance;
    
    void Awake()
    {
        // 单例模式，确保只有一个实例
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 监听场景切换事件
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            Debug.Log("[GlobalKeyboardManager] 全局键盘管理器已初始化");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneUnloaded(Scene scene)
    {
        Debug.Log($"[GlobalKeyboardManager] 场景卸载：{scene.name} - 清理键盘状态");
        ForceCloseKeyboard();
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GlobalKeyboardManager] 场景加载：{scene.name}");
        // 确保键盘状态清理
        ForceCloseKeyboard();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Debug.Log("[GlobalKeyboardManager] 应用失去焦点 - 清理键盘状态");
            ForceCloseKeyboard();
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Debug.Log("[GlobalKeyboardManager] 应用暂停 - 清理键盘状态");
            ForceCloseKeyboard();
        }
    }

    /// <summary>
    /// 强制关闭键盘
    /// </summary>
    public static void ForceCloseKeyboard()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            if (TouchScreenKeyboard.visible)
            {
                TouchScreenKeyboard.hideInput = true;
                Debug.Log("[GlobalKeyboardManager] 强制隐藏TouchScreenKeyboard");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[GlobalKeyboardManager] 隐藏键盘时出错: {e.Message}");
        }
#endif
    }

    void OnDestroy()
    {
        // 清理事件监听
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
        Debug.Log("[GlobalKeyboardManager] 全局键盘管理器已销毁");
    }

    /// <summary>
    /// 在游戏启动时调用此方法来初始化全局键盘管理器
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        if (instance == null)
        {
            GameObject keyboardManager = new GameObject("GlobalKeyboardManager");
            keyboardManager.AddComponent<GlobalKeyboardManager>();
        }
    }
}