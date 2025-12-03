using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
using System.IO;

/// <summary>
/// WebView图表控制器 - 使用WebView显示ECharts图表
/// 注意：需要安装UniWebView或其他WebView插件
/// </summary>
public class WebView图表控制器_茉 : MonoBehaviour
{
    [Header("WebView配置")]
    [SerializeField] private UIBlock2D 显示容器; // Nova UI显示容器（用于定位WebView）
    [SerializeField] private string HTML文件路径 = "StreamingAssets/Charts/chart.html";
    
    [Header("图表数据")]
    [SerializeField] private bool 使用示例数据 = true;
    
    // WebView实例（需要根据实际使用的WebView插件调整）
    // private UniWebView webView; // UniWebView示例
    // private Vuplex.WebView.WebView webView; // Vuplex示例
    
    void Start()
    {
        // 初始化WebView
        // 初始化WebView();
    }
    
    /// <summary>
    /// 初始化WebView（UniWebView示例）
    /// </summary>
    /*
    void 初始化WebView()
    {
        // 创建WebView GameObject
        GameObject webViewObject = new GameObject("WebView");
        webViewObject.transform.SetParent(显示容器.transform);
        
        // 添加UniWebView组件
        webView = webViewObject.AddComponent<UniWebView>();
        
        // 设置WebView大小和位置（基于Nova UIBlock2D）
        if (显示容器 != null)
        {
            Vector3 size = 显示容器.CalculatedSize.Value;
            webView.Frame = new Rect(0, 0, size.x, size.y);
        }
        
        // 加载HTML文件
        string htmlPath = Path.Combine(Application.streamingAssetsPath, "Charts/chart.html");
        if (File.Exists(htmlPath))
        {
            webView.Load(htmlPath);
        }
        else
        {
            Debug.LogError($"[WebView图表] HTML文件不存在: {htmlPath}");
        }
        
        // 设置回调
        webView.OnPageFinished += (view, statusCode, url) => {
            Debug.Log($"[WebView图表] 页面加载完成: {url}");
            // 注入数据
            if (使用示例数据)
            {
                注入示例数据();
            }
        };
        
        // 显示WebView
        webView.Show();
    }
    */
    
    /// <summary>
    /// 注入图表数据到WebView
    /// </summary>
    public void 注入数据(string jsonData)
    {
        // 根据使用的WebView插件调用JavaScript
        // webView.EvaluateJavaScript($"updateChartData({jsonData});");
    }
    
    /// <summary>
    /// 注入示例数据
    /// </summary>
    void 注入示例数据()
    {
        // 示例：用户年龄占比数据
        string 年龄数据 = @"{
            'categories': ['幼年(0-6岁)', '少年(7-17岁)', '青年(18-40岁)', '中年(41-65岁)', '老年(66岁以上)'],
            'values': [18, 12, 20, 32, 18]
        }";
        
        注入数据(年龄数据);
    }
    
    /// <summary>
    /// 更新折线图数据
    /// </summary>
    public void 更新折线图(Dictionary<string, float> 数据)
    {
        string json = 转换为JSON(数据);
        // webView.EvaluateJavaScript($"updateLineChart({json});");
    }
    
    /// <summary>
    /// 更新饼图数据
    /// </summary>
    public void 更新饼图(Dictionary<string, float> 数据)
    {
        string json = 转换为JSON(数据);
        // webView.EvaluateJavaScript($"updatePieChart({json});");
    }
    
    /// <summary>
    /// 转换为JSON（简单实现）
    /// </summary>
    string 转换为JSON(Dictionary<string, float> 数据)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append("{");
        bool first = true;
        foreach (var kvp in 数据)
        {
            if (!first) sb.Append(",");
            sb.Append($"\"{kvp.Key}\":{kvp.Value}");
            first = false;
        }
        sb.Append("}");
        return sb.ToString();
    }
    
    void OnDestroy()
    {
        // 清理WebView
        // if (webView != null)
        // {
        //     webView.Destroy();
        // }
    }
}

