using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
using UnityEngine.UI;

/// <summary>
/// Nova UI 图表适配器 - 将基于UGUI的图表（如XCharts）适配到Nova UI中
/// 原理：使用RenderTexture将图表渲染到纹理，然后在Nova UIBlock2D上显示
/// </summary>
public class Nova图表适配器_茉 : MonoBehaviour
{
    [Header("图表配置")]
    [SerializeField] private Canvas 图表Canvas; // 包含图表的Canvas（UGUI）
    [SerializeField] private UIBlock2D 显示容器; // Nova UI显示容器
    [SerializeField] private Camera 渲染相机; // 用于渲染图表的相机（可选，会自动创建）
    
    [Header("渲染设置")]
    [SerializeField] private int 纹理宽度 = 1920;
    [SerializeField] private int 纹理高度 = 1080;
    [SerializeField] private bool 自动更新 = true;
    [SerializeField] private float 更新间隔 = 0.1f;
    
    private RenderTexture 渲染纹理;
    private Camera 临时相机;
    
    void Start()
    {
        初始化适配器();
    }
    
    /// <summary>
    /// 初始化适配器
    /// </summary>
    void 初始化适配器()
    {
        if (图表Canvas == null)
        {
            Debug.LogError("[Nova图表适配器] 图表Canvas未设置！");
            return;
        }
        
        if (显示容器 == null)
        {
            Debug.LogError("[Nova图表适配器] 显示容器未设置！");
            return;
        }
        
        // 创建或使用渲染相机
        if (渲染相机 == null)
        {
            创建渲染相机();
        }
        
        // 创建RenderTexture
        创建渲染纹理();
        
        // 设置Canvas的渲染相机
        图表Canvas.worldCamera = 渲染相机;
        图表Canvas.renderMode = RenderMode.ScreenSpaceCamera;
        
        // 将纹理应用到Nova UIBlock2D
        应用纹理到Nova();
        
        if (自动更新)
        {
            StartCoroutine(自动更新协程());
        }
    }
    
    /// <summary>
    /// 创建渲染相机
    /// </summary>
    void 创建渲染相机()
    {
        GameObject 相机对象 = new GameObject("图表渲染相机");
        相机对象.transform.SetParent(transform);
        临时相机 = 相机对象.AddComponent<Camera>();
        
        临时相机.clearFlags = CameraClearFlags.SolidColor;
        临时相机.backgroundColor = Color.clear;
        临时相机.orthographic = true;
        临时相机.orthographicSize = 5f;
        临时相机.cullingMask = 1 << 图表Canvas.gameObject.layer;
        临时相机.depth = 100; // 确保在其他相机之上
    }
    
    /// <summary>
    /// 创建渲染纹理
    /// </summary>
    void 创建渲染纹理()
    {
        渲染纹理 = new RenderTexture(纹理宽度, 纹理高度, 24);
        渲染纹理.name = "图表渲染纹理";
        
        if (渲染相机 != null)
        {
            渲染相机.targetTexture = 渲染纹理;
        }
    }
    
    /// <summary>
    /// 将纹理应用到Nova UIBlock2D
    /// </summary>
    void 应用纹理到Nova()
    {
        if (显示容器 != null && 渲染纹理 != null)
        {
            // 将RenderTexture转换为Texture2D
            Texture2D 纹理 = new Texture2D(纹理宽度, 纹理高度, TextureFormat.RGB24, false);
            RenderTexture.active = 渲染纹理;
            纹理.ReadPixels(new Rect(0, 0, 纹理宽度, 纹理高度), 0, 0);
            纹理.Apply();
            RenderTexture.active = null;
            
            // 应用到Nova UIBlock2D
            显示容器.SetImage(纹理);
            Debug.Log("[Nova图表适配器] 纹理已应用到Nova UI");
        }
    }
    
    /// <summary>
    /// 手动刷新图表显示
    /// </summary>
    public void 刷新图表()
    {
        if (渲染纹理 != null && 显示容器 != null)
        {
            应用纹理到Nova();
        }
    }
    
    /// <summary>
    /// 自动更新协程
    /// </summary>
    IEnumerator 自动更新协程()
    {
        while (true)
        {
            yield return new WaitForSeconds(更新间隔);
            刷新图表();
        }
    }
    
    void OnDestroy()
    {
        if (渲染纹理 != null)
        {
            渲染纹理.Release();
            Destroy(渲染纹理);
        }
        
        if (临时相机 != null)
        {
            Destroy(临时相机.gameObject);
        }
    }
}

