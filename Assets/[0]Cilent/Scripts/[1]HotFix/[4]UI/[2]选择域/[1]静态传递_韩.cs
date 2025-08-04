using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

/// <summary>
/// 面板跨场景传递参数的辅助类
/// 用于在上一个场景向下当前场景的界面管理器传递要显示的面板名称
/// </summary>
public static class 静态传递_韩
{
    // 存储要传递的面板名称
    private static string _targetPanelKey;
    
    // 场景加载完成的回调
    private static UnityAction<Scene, LoadSceneMode> _onSceneLoaded;

    /// <summary>
    /// 存储面板名称并切换场景
    /// 在上一个场景中调用此方法
    /// </summary>
    /// <param name="sceneName">目标场景名称</param>
    /// <param name="panelKey">要显示的面板唯一标识</param>
    public static void SwitchSceneWithPanel(string sceneName, string panelKey)
    {
        // 保存要传递的面板名称
        _targetPanelKey = panelKey;
        
        // 确保只注册一次事件
        if (_onSceneLoaded == null)
        {
            _onSceneLoaded = OnSceneLoaded;
            SceneManager.sceneLoaded += _onSceneLoaded;
        }
        
        // 切换场景
        LoadYooAssetsTool.LoadSceneAsync(sceneName).Forget();
    }

    /// <summary>
    /// 泛型版本：通过面板类型传递
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    /// <param name="sceneName">目标场景名称</param>
    public static void SwitchSceneWithPanel<T>(string sceneName) where T : 界面基础_韩
    {
        // 获取面板类型对应的唯一标识
        string panelKey = GetPanelKey<T>();
        if (!string.IsNullOrEmpty(panelKey))
        {
            SwitchSceneWithPanel(sceneName, panelKey);
        }
        else
        {
            Debug.LogError($"无法获取面板 {typeof(T).Name} 的唯一标识");
        }
    }

    /// <summary>
    /// 场景加载完成后调用
    /// 尝试找到界面管理器并显示指定面板
    /// </summary>
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!string.IsNullOrEmpty(_targetPanelKey))
        {
            // 查找当前场景中的界面管理器
            界面管理器_韩 manager = GameObject.FindObjectOfType<界面管理器_韩>();
            if (manager != null)
            {
                // 显示传递过来的面板
                manager.显示界面(_targetPanelKey);
            }
            else
            {
                Debug.LogError("在目标场景中未找到 界面管理器_韩 组件");
            }
        }
        
        // 清理资源
        _targetPanelKey = null;
        if (_onSceneLoaded != null)
        {
            SceneManager.sceneLoaded -= _onSceneLoaded;
            _onSceneLoaded = null;
        }
    }

    /// <summary>
    /// 获取面板类型对应的唯一标识
    /// </summary>
    private static string GetPanelKey<T>() where T : 界面基础_韩
    {
        try
        {
            // 通过反射获取面板的唯一标识
            var type = typeof(T);
            var instance = Activator.CreateInstance(type) as 界面基础_韩;
            return instance?.界面名称;
        }
        catch (Exception e)
        {
            Debug.LogError($"获取面板标识失败: {e.Message}");
            return null;
        }
    }
}
