using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Cysharp.Threading.Tasks;

public class 界面管理器_韩 : MonoBehaviour, IController
{

    void Start()
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorPrefs.GetInt("TestPlatform", 0) == 0)
        {
            var pc背景 = GameObject.Find("PC背景");
            var mobile背景 = GameObject.Find("安卓背景");
            if(pc背景 != null) pc背景.SetActive(true);
            if(mobile背景 != null) mobile背景.SetActive(false);
        }
        else{

            Debug.Log("安卓背景");
            var pc背景 = GameObject.Find("PC背景");
            var mobile背景 = GameObject.Find("安卓背景");
            if(pc背景 != null) pc背景.SetActive(false);
            if(mobile背景 != null) mobile背景.SetActive(true);
        }
#elif UNITY_ANDROID
        var pc背景 = GameObject.Find("PC背景");
        var mobile背景 = GameObject.Find("安卓背景");
        if(pc背景 != null) pc背景.SetActive(false);
        if(mobile背景 != null) mobile背景.SetActive(true);
#else
        var pc背景 = GameObject.Find("PC背景");
        var mobile背景 = GameObject.Find("安卓背景");
        if(pc背景 != null) pc背景.SetActive(true);
        if(mobile背景 != null) mobile背景.SetActive(false);
#endif

    }


    // 当前显示的面板
    public 界面基础_韩 _currentPanel;

    // 已加载的面板缓存（键：面板唯一标识，值：面板实例）
    // 缓存避免重复加载，提高性能
    private Dictionary<string, 界面基础_韩> _loadedPanels = new Dictionary<string, 界面基础_韩>();

    /// <summary>
    /// 显示指定类型的面板（泛型版本，使用更方便）
    /// 会自动隐藏当前显示的面板
    /// </summary>
    /// <typeparam name="T">要显示的面板类型（必须继承自界面基础_韩）</typeparam>
    public void 显示界面<T>() where T : 界面基础_韩
    {
        // 获取面板的唯一标识（资源路径）
        string panelKey = GetPanelKey<T>();
        Debug.Log("显示界面" + panelKey);
        // 调用字符串版本的ShowPanel方法
        显示界面(panelKey);
    }

    /// <summary>
    /// 显示指定标识的面板（字符串版本，更灵活）
    /// 会自动隐藏当前显示的面板
    /// </summary>
    /// <param name="panelKey">面板唯一标识（对应YooAsset中的资源路径）</param>
    public void 显示界面(string panelKey)
    {
        // 如果要显示的面板就是当前面板，直接返回，避免重复操作
        if (_currentPanel != null && _currentPanel.界面名称 == panelKey)
            return;

        // 隐藏当前正在显示的面板
        隐藏界面();

        // 检查面板是否已加载（在缓存中）
        if (_loadedPanels.TryGetValue(panelKey, out var panel))
        {
            // 如果已加载，直接显示
            panel.显示界面();
            _currentPanel = panel;
        }
        else
        {
            // 如果未加载，通过YooAsset异步加载面板
            加载界面(panelKey);
        }
    }

    /// <summary>
    /// 隐藏当前显示的面板
    /// </summary>
    public void 隐藏界面()
    {
        if (_currentPanel != null)
        {
            _currentPanel.隐藏界面();
            _currentPanel = null;
        }
    }

    /// <summary>
    /// 卸载指定面板（从缓存中移除并销毁）
    /// </summary>
    /// <param name="panelKey">要卸载的面板标识</param>
    public void 卸载界面(string panelKey)
    {
        // 检查面板是否在缓存中
        if (_loadedPanels.TryGetValue(panelKey, out var panel))
        {
            // 如果卸载的是当前显示的面板，需要先清除当前面板引用
            if (_currentPanel == panel)
            {
                _currentPanel = null;
            }

            // 销毁面板游戏对象
            Destroy(panel.gameObject);
            // 从缓存中移除
            _loadedPanels.Remove(panelKey);
        }
    }

    /// <summary>
    /// 异步加载面板（通过YooAsset）
    /// </summary>
    /// <param name="panelKey">面板的资源路径</param>
    private async void 加载界面(string panelKey)
    {
        try
        {
            // 使用项目中已有的LoadYooAssetsTool来加载资源
            Debug.Log("加载界面" + panelKey);
            GameObject panelPrefab = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb(panelKey);
            // GameObject panelPrefab = await LoadYooAssetsTool.LoadAsset<GameObject>(panelKey);

            if (panelPrefab != null)
            {
                // 加载成功，实例化面板预制体
                GameObject panelObj = Instantiate(panelPrefab);
                // 将面板设置为管理器的子对象，方便场景管理
                panelObj.transform.SetParent(transform, false);

                // 获取面板上的界面基础_韩组件
                界面基础_韩 panel = panelObj.GetComponent<界面基础_韩>();
                if (panel == null)
                {
                    // 如果面板预制体上没有挂载界面基础_韩或其子类，报错并销毁
                    Debug.LogError($"面板 {panelKey} 缺少 界面基础_韩 组件，请检查预制体");
                    Destroy(panelObj);
                    return;
                }

                // 将面板添加到缓存
                _loadedPanels[panelKey] = panel;
                // 显示面板
                panel.显示界面();
                // 设置为当前面板
                _currentPanel = panel;
                Debug.Log("当前面板" + _currentPanel.name);
            }
            else
            {
                Debug.LogError($"加载面板 {panelKey} 失败: 预制体为空");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"加载面板 {panelKey} 失败: {e.Message}");
        }
    }

    /// <summary>
    /// 预加载面板到缓存但不显示
    /// </summary>
    public async UniTask 预加载界面(string panelKey)
    {
        if (string.IsNullOrEmpty(panelKey)) return;
        if (_loadedPanels.ContainsKey(panelKey)) return;
        try
        {
            GameObject panelPrefab = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb(panelKey);
            if (panelPrefab == null)
            {
                Debug.LogError($"预加载面板 {panelKey} 失败: 预制体为空");
                return;
            }

            GameObject panelObj = Instantiate(panelPrefab);
            panelObj.transform.SetParent(transform, false);

            界面基础_韩 panel = panelObj.GetComponent<界面基础_韩>();
            if (panel == null)
            {
                Debug.LogError($"面板 {panelKey} 缺少 界面基础_韩 组件，请检查预制体");
                Destroy(panelObj);
                return;
            }

            panel.隐藏界面(); // 确保不显示
            _loadedPanels[panelKey] = panel;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"预加载面板 {panelKey} 失败: {e.Message}");
        }
    }

    /// <summary>
    /// 获取面板类型对应的唯一标识（资源路径）
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    /// <returns>面板的资源路径</returns>
    private string GetPanelKey<T>() where T : 界面基础_韩
    {
        // 通过反射创建面板类型的临时实例，获取其界面名称
        // 注意：这里不会执行Awake/Start等生命周期方法，仅用于获取界面名称
        var type = typeof(T);
        var instance = System.Activator.CreateInstance(type) as 界面基础_韩;
        return instance?.界面名称;
    }

    /// <summary>
    /// 管理器销毁时，清理所有面板
    /// </summary>
    private void OnDestroy()
    {
        // 销毁所有已加载的面板
        foreach (var panel in _loadedPanels.Values)
        {
            if (panel != null)
            {
                Destroy(panel.gameObject);
            }
        }
        // 清空缓存
        _loadedPanels.Clear();
        _currentPanel = null;
    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}