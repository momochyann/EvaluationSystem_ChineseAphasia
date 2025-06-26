using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Cysharp.Threading.Tasks;
using System.Text;
using System;

/// <summary>
/// YooAsset高级缓存管理器
/// 提供LRU缓存、内存监控、自动清理等高级功能
/// </summary>
public class YooAssetCacheManager : MonoBehaviour
{
    [Header("缓存配置")]
    [SerializeField] private int maxCacheSize = 100; // 最大缓存数量
    [SerializeField] private float memoryThreshold = 0.8f; // 内存使用阈值 (80%)
    [SerializeField] private float autoCleanInterval = 60f; // 自动清理间隔（秒）
    
    // LRU缓存节点
    private class CacheNode
    {
        public string key;
        public UnityEngine.Object asset;
        public long lastAccessTime;
        public int accessCount;
        public long memorySize; // 预估内存大小
        
        public CacheNode prev;
        public CacheNode next;
    }
    
    // LRU缓存实现
    private readonly Dictionary<string, CacheNode> cacheMap = new Dictionary<string, CacheNode>();
    private readonly CacheNode head = new CacheNode();
    private readonly CacheNode tail = new CacheNode();
    
    // 统计信息
    private long totalMemoryUsage = 0;
    private int hitCount = 0;
    private int missCount = 0;
    
    private void Awake()
    {
        // 初始化双向链表
        head.next = tail;
        tail.prev = head;
        
        // 启动自动清理协程
        InvokeRepeating(nameof(AutoCleanup), autoCleanInterval, autoCleanInterval);
    }
    
    /// <summary>
    /// 获取缓存的资源
    /// </summary>
    public T GetCachedAsset<T>(string assetName) where T : UnityEngine.Object
    {
        if (cacheMap.TryGetValue(assetName, out CacheNode node))
        {
            // 更新访问信息
            node.lastAccessTime = DateTime.Now.Ticks;
            node.accessCount++;
            
            // 移动到链表头部（最近使用）
            MoveToHead(node);
            
            hitCount++;
            return node.asset as T;
        }
        
        missCount++;
        return null;
    }
    
    /// <summary>
    /// 缓存资源
    /// </summary>
    public void CacheAsset(string assetName, UnityEngine.Object asset)
    {
        if (cacheMap.TryGetValue(assetName, out CacheNode existingNode))
        {
            // 更新现有节点
            existingNode.asset = asset;
            existingNode.lastAccessTime = DateTime.Now.Ticks;
            existingNode.accessCount++;
            MoveToHead(existingNode);
            return;
        }
        
        // 创建新节点
        var newNode = new CacheNode
        {
            key = assetName,
            asset = asset,
            lastAccessTime = DateTime.Now.Ticks,
            accessCount = 1,
            memorySize = EstimateMemorySize(asset)
        };
        
        // 检查内存和缓存大小限制
        if (ShouldEvict())
        {
            EvictLeastUsed();
        }
        
        // 添加到缓存
        cacheMap[assetName] = newNode;
        AddToHead(newNode);
        totalMemoryUsage += newNode.memorySize;
    }
    
    /// <summary>
    /// 估算资源内存大小
    /// </summary>
    private long EstimateMemorySize(UnityEngine.Object asset)
    {
        if (asset == null) return 0;
        
        switch (asset)
        {
            case Texture2D texture:
                return texture.width * texture.height * 4; // 假设RGBA32格式
            case Mesh mesh:
                return mesh.vertexCount * 32 + mesh.triangles.Length * 4; // 粗略估算
            case AudioClip audio:
                return audio.samples * audio.channels * 2; // 假设16位采样
            case GameObject prefab:
                return 1024; // 预制体本身内存较小，主要是引用
            default:
                return 512; // 默认估算
        }
    }
    
    /// <summary>
    /// 判断是否需要清理缓存
    /// </summary>
    private bool ShouldEvict()
    {
        // 检查缓存数量限制
        if (cacheMap.Count >= maxCacheSize)
            return true;
            
        // 检查内存使用率
        long totalMemory = GC.GetTotalMemory(false);
        float memoryUsageRatio = (float)totalMemoryUsage / totalMemory;
        
        return memoryUsageRatio > memoryThreshold;
    }
    
    /// <summary>
    /// 清理最少使用的资源
    /// </summary>
    private void EvictLeastUsed()
    {
        if (tail.prev == head) return; // 缓存为空
        
        var lruNode = tail.prev;
        RemoveNode(lruNode);
        cacheMap.Remove(lruNode.key);
        totalMemoryUsage -= lruNode.memorySize;
        
        Debug.Log($"[YooAssetCache] 清理LRU资源: {lruNode.key}");
    }
    
    /// <summary>
    /// 自动清理过期缓存
    /// </summary>
    private void AutoCleanup()
    {
        long currentTime = DateTime.Now.Ticks;
        long expireTime = TimeSpan.FromMinutes(10).Ticks; // 10分钟过期
        
        var nodesToRemove = new List<CacheNode>();
        
        // 查找过期节点
        var current = tail.prev;
        while (current != head)
        {
            if (current.accessCount == 1 && (currentTime - current.lastAccessTime) > expireTime)
            {
                nodesToRemove.Add(current);
            }
            current = current.prev;
        }
        
        // 移除过期节点
        foreach (var node in nodesToRemove)
        {
            RemoveNode(node);
            cacheMap.Remove(node.key);
            totalMemoryUsage -= node.memorySize;
        }
        
        if (nodesToRemove.Count > 0)
        {
            Debug.Log($"[YooAssetCache] 自动清理过期缓存: {nodesToRemove.Count} 个");
        }
    }
    
    /// <summary>
    /// 预热常用资源
    /// </summary>
    public async UniTask WarmupAssets(string[] assetNames)
    {
        var loadTasks = new List<UniTask>();
        
        foreach (string assetName in assetNames)
        {
            if (!cacheMap.ContainsKey(assetName))
            {
                loadTasks.Add(LoadAndCacheAsset(assetName));
            }
        }
        
        if (loadTasks.Count > 0)
        {
            await UniTask.WhenAll(loadTasks);
            Debug.Log($"[YooAssetCache] 预热完成: {loadTasks.Count} 个资源");
        }
    }
    
    private async UniTask LoadAndCacheAsset(string assetName)
    {
        try
        {
            var asset = await LoadYooAssetsTool.LoadAsset<UnityEngine.Object>(assetName);
            if (asset != null)
            {
                CacheAsset(assetName, asset);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[YooAssetCache] 预热资源失败: {assetName}, 错误: {e.Message}");
        }
    }
    
    /// <summary>
    /// 获取缓存统计信息
    /// </summary>
    public CacheStats GetCacheStats()
    {
        float hitRate = (hitCount + missCount) > 0 ? (float)hitCount / (hitCount + missCount) : 0f;
        
        return new CacheStats
        {
            CacheCount = cacheMap.Count,
            MaxCacheSize = maxCacheSize,
            TotalMemoryUsage = totalMemoryUsage,
            HitCount = hitCount,
            MissCount = missCount,
            HitRate = hitRate
        };
    }
    
    /// <summary>
    /// 清理所有缓存
    /// </summary>
    public void ClearAllCache()
    {
        cacheMap.Clear();
        head.next = tail;
        tail.prev = head;
        totalMemoryUsage = 0;
        hitCount = 0;
        missCount = 0;
        
        Debug.Log("[YooAssetCache] 清理所有缓存");
    }
    
    #region 双向链表操作
    
    private void AddToHead(CacheNode node)
    {
        node.prev = head;
        node.next = head.next;
        head.next.prev = node;
        head.next = node;
    }
    
    private void RemoveNode(CacheNode node)
    {
        node.prev.next = node.next;
        node.next.prev = node.prev;
    }
    
    private void MoveToHead(CacheNode node)
    {
        RemoveNode(node);
        AddToHead(node);
    }
    
    #endregion
    
    private void OnDestroy()
    {
        CancelInvoke(nameof(AutoCleanup));
    }
}

/// <summary>
/// 缓存统计信息
/// </summary>
[System.Serializable]
public struct CacheStats
{
    public int CacheCount;
    public int MaxCacheSize;
    public long TotalMemoryUsage;
    public int HitCount;
    public int MissCount;
    public float HitRate;
    
    public override string ToString()
    {
        return $"Cache: {CacheCount}/{MaxCacheSize}, Memory: {TotalMemoryUsage / 1024}KB, " +
               $"Hit Rate: {HitRate:P2} ({HitCount}/{HitCount + MissCount})";
    }
}

public class YooAssetPfbPoolUtility : IUtility
{
    // 预制体缓存字典
    public Dictionary<string, UnityEngine.Object> pfbDict;
    
    // UI资源名称集合
    private HashSet<string> uiAssetNames = new HashSet<string>();
    
    // 预计算的平台后缀，避免运行时重复计算
    private readonly string platformSuffix;
    private readonly string alternatePlatformSuffix;
    
    // 字符串缓存，减少GC分配
    private readonly Dictionary<string, string> stringCache = new Dictionary<string, string>();
    
    // StringBuilder复用，减少字符串拼接的GC
    private readonly StringBuilder stringBuilder = new StringBuilder(64);
    
    // 常用后缀字符串，避免重复分配
    private const string PC_SUFFIX = "_PC";
    private const string MOBILE_SUFFIX = "_Mobile";

    public YooAssetPfbPoolUtility()
    {
        // 在构造函数中预计算平台后缀，避免运行时重复计算
        platformSuffix = GetPlatformSuffix();
        alternatePlatformSuffix = GetPlatformSuffix(true);
    }

    public void Init()
    {
        pfbDict = new Dictionary<string, UnityEngine.Object>();
        uiAssetNames = LoadYooAssetsTool.GetAssetInfosByTag("UI");
        
        // 预分配字典容量，减少扩容时的GC
        if (uiAssetNames.Count > 0)
        {
            pfbDict = new Dictionary<string, UnityEngine.Object>(uiAssetNames.Count * 2);
        }
    }

    public bool CheckPfbNameInUIAssets(string pfbName)
    {
        // 直接使用HashSet的Contains方法，性能更好
        if (uiAssetNames.Contains(pfbName))
            return true;
            
        // 如果直接匹配失败，再进行子字符串匹配
        foreach (var assetName in uiAssetNames)
        {
            if (assetName.Contains(pfbName))
            {
                return true;
            }
        }
        return false;
    }

    private string GetPlatformSuffix(bool 取反 = false)
    {
#if UNITY_EDITOR
        // 在编辑器中根据选择的平台返回后缀
        if (UnityEditor.EditorPrefs.GetInt("TestPlatform", 0) == 0)
        {
            return 取反 ? MOBILE_SUFFIX : PC_SUFFIX;
        }
        else
        {
            return 取反 ? PC_SUFFIX : MOBILE_SUFFIX;
        }
#elif UNITY_ANDROID || UNITY_IOS
        return 取反 ? PC_SUFFIX : MOBILE_SUFFIX;
#else
        return 取反 ? MOBILE_SUFFIX : PC_SUFFIX;
#endif
    }

    // 优化的字符串处理方法，减少GC分配
    private string GetOptimizedPath(string pfbName)
    {
        // 先检查缓存
        if (stringCache.TryGetValue(pfbName, out string cachedPath))
        {
            return cachedPath;
        }

        string finalPath = pfbName;

        if (CheckPfbNameInUIAssets(pfbName))
        {
            // 使用StringBuilder避免多次字符串分配
            stringBuilder.Clear();
            
            // 移除已存在的平台后缀
            string cleanName = pfbName;
            if (pfbName.EndsWith(PC_SUFFIX))
            {
                cleanName = pfbName.Substring(0, pfbName.Length - PC_SUFFIX.Length);
            }
            else if (pfbName.EndsWith(MOBILE_SUFFIX))
            {
                cleanName = pfbName.Substring(0, pfbName.Length - MOBILE_SUFFIX.Length);
            }

            // 先尝试当前平台后缀
            stringBuilder.Append(cleanName).Append(platformSuffix);
            string primaryPath = stringBuilder.ToString();
            
            if (LoadYooAssetsTool.CheckAssetExist(primaryPath))
            {
                finalPath = primaryPath;
            }
            else
            {
                // 尝试备用平台后缀
                stringBuilder.Clear();
                stringBuilder.Append(cleanName).Append(alternatePlatformSuffix);
                finalPath = stringBuilder.ToString();
            }
        }

        // 缓存结果，避免重复计算
        if (stringCache.Count < 1000) // 限制缓存大小，防止内存泄漏
        {
            stringCache[pfbName] = finalPath;
        }

        return finalPath;
    }

    public async UniTask<GameObject> LoadPfb(string pfbName)
    {
        string finalPath = GetOptimizedPath(pfbName);

        // 先检查缓存
        if (pfbDict.TryGetValue(finalPath, out var cachedPfb))
        {
            return cachedPfb as GameObject;
        }

        // 加载资源
        var pfb = await LoadYooAssetsTool.LoadAsset<GameObject>(finalPath);
        
        // 缓存资源
        pfbDict[finalPath] = pfb;
        
        return pfb;
    }

    public async UniTask<T> LoadConfig<T>(string configName, bool islocal = false) where T : UnityEngine.Object
    {
        // 先检查缓存
        if (pfbDict.TryGetValue(configName, out var cachedConfig))
        {
            return cachedConfig as T;
        }
        
        var config = await LoadYooAssetsTool.LoadAsset<T>(configName, islocal);
        pfbDict[configName] = config;
        return config;
    }

    // 新增：预加载常用资源的方法
    public async UniTask PreloadCommonAssets(string[] assetNames)
    {
        var tasks = new List<UniTask>();
        
        foreach (string assetName in assetNames)
        {
            if (!pfbDict.ContainsKey(assetName))
            {
                tasks.Add(LoadPfb(assetName));
            }
        }
        
        if (tasks.Count > 0)
        {
            await UniTask.WhenAll(tasks);
            Debug.Log($"[YooAssetPfbPoolUtility] 预加载完成: {tasks.Count} 个资源");
        }
    }

    // 新增：清理缓存方法，释放不再使用的资源
    public void ClearCache()
    {
        pfbDict.Clear();
        stringCache.Clear();
        Debug.Log("[YooAssetPfbPoolUtility] 缓存已清理");
    }

    // 新增：获取缓存信息
    public (int prefabCount, int stringCacheCount, long estimatedMemoryUsage) GetCacheInfo()
    {
        long estimatedMemory = 0;
        foreach (var asset in pfbDict.Values)
        {
            estimatedMemory += EstimateAssetMemorySize(asset);
        }
        
        return (pfbDict.Count, stringCache.Count, estimatedMemory);
    }
    
    // 估算资源内存大小的辅助方法
    private long EstimateAssetMemorySize(UnityEngine.Object asset)
    {
        if (asset == null) return 0;
        
        switch (asset)
        {
            case Texture2D texture:
                return texture.width * texture.height * 4; // 假设RGBA32格式
            case Mesh mesh:
                return mesh.vertexCount * 32 + mesh.triangles.Length * 4; // 粗略估算
            case AudioClip audio:
                return audio.samples * audio.channels * 2; // 假设16位采样
            case GameObject prefab:
                return 1024; // 预制体本身内存较小，主要是引用
            default:
                return 512; // 默认估算
        }
    }
    
    // 新增：按使用频率清理缓存
    public void OptimizeCache(int maxCacheSize = 50)
    {
        if (pfbDict.Count <= maxCacheSize) return;
        
        // 简单的LRU实现：保留最近使用的资源
        var keysToRemove = new List<string>();
        int removeCount = pfbDict.Count - maxCacheSize;
        
        foreach (var kvp in pfbDict)
        {
            if (removeCount <= 0) break;
            
            // 简单策略：移除GameObject类型的资源（通常是预制体）
            if (kvp.Value is GameObject)
            {
                keysToRemove.Add(kvp.Key);
                removeCount--;
            }
        }
        
        foreach (var key in keysToRemove)
        {
            pfbDict.Remove(key);
        }
        
        Debug.Log($"[YooAssetPfbPoolUtility] 缓存优化完成，移除了 {keysToRemove.Count} 个资源");
    }
}
