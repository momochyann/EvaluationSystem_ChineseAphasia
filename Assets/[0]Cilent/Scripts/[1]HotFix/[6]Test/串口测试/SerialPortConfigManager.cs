using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// 串口配置管理器
/// 提供静态方法加载和访问配置列表
/// </summary>
public static class SerialPortConfigManager
{
    private static SerialPortConfigListSO _configList;
    private static bool _isLoaded = false;

    /// <summary>
    /// 加载配置列表
    /// </summary>
    public static async UniTask LoadConfigListAsync()
    {
        if (_isLoaded && _configList != null)
        {
            return;
        }

        // 优先尝试加载配置列表
        _configList = await LoadYooAssetsTool.LoadAsset<SerialPortConfigListSO>("SerialPortConfigList");
        
        if (_configList == null)
        {
            Debug.LogWarning("[SerialPortConfigManager] 未找到 SerialPortConfigList，尝试加载单个配置");
            // 向后兼容：尝试加载单个配置并转换为列表
            var singleConfig = await LoadYooAssetsTool.LoadAsset<SerialPortConfigSO>("SerialPortConfig");
            if (singleConfig != null)
            {
                _configList = ScriptableObject.CreateInstance<SerialPortConfigListSO>();
                _configList.configs = new List<SerialPortConfigSO> { singleConfig };
            }
        }

        _isLoaded = true;
        Debug.Log($"[SerialPortConfigManager] 配置列表加载完成，共 {(_configList?.configs?.Count ?? 0)} 个配置");
    }

    /// <summary>
    /// 根据预制体名称获取配置
    /// </summary>
    /// <param name="prefabName">预制体名称（GameObject 名称）</param>
    /// <returns>匹配的配置，如果未找到则返回 null</returns>
    public static SerialPortConfigSO GetConfigByPrefabName(string prefabName)
    {
        if (_configList == null || _configList.configs == null)
        {
            Debug.LogWarning("[SerialPortConfigManager] 配置列表未加载");
            return null;
        }

        foreach (var config in _configList.configs)
        {
            if (config != null && config.prefabName == prefabName)
            {
                return config;
            }
        }

        Debug.LogWarning($"[SerialPortConfigManager] 未找到名称匹配的配置: {prefabName}");
        return null;
    }

    /// <summary>
    /// 获取所有配置
    /// </summary>
    public static List<SerialPortConfigSO> GetAllConfigs()
    {
        if (_configList == null || _configList.configs == null)
        {
            return new List<SerialPortConfigSO>();
        }

        return new List<SerialPortConfigSO>(_configList.configs);
    }

    /// <summary>
    /// 检查配置是否已加载
    /// </summary>
    public static bool IsLoaded => _isLoaded;
}

