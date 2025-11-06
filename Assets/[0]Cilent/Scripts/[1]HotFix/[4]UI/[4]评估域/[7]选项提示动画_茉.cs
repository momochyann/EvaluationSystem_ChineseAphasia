using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;
using Nova;
using DG.Tweening;
using QFramework;
using System.Linq;

/// <summary>
/// 选项提示动画组件 - 纯动画效果，不改变选项状态
/// 监听一组Toggle的选中事件，当某个被选中时显示对应背景块，其他背景块隐藏
/// 只负责动画，不管理Toggle的选中状态
/// </summary>
public class 选项提示动画_茉 : MonoBehaviour, IController
{
    [System.Serializable]
    public class 选项项
    {
        [Tooltip("Toggle组件")]
        public Toggle toggle;
        
        [Tooltip("对应的背景块")]
        public UIBlock2D 背景块;
        
        [HideInInspector]
        public Tweener 当前动画;
    }

    [Header("选项组设置")]
    [SerializeField] public string 组名称 = "选项提示动画组";
    
    [Tooltip("所有的选项（Toggle和对应的背景块）")]
    [SerializeField] private List<选项项> 选项列表 = new List<选项项>();

    [Header("动画设置")]
    [SerializeField] private float 渐显时长 = 0.3f;
    [SerializeField] private float 渐隐时长 = 0.2f;
    [SerializeField] private float 选中透明度 = 1f;
    [SerializeField] private float 未选中透明度 = 0f;
    [SerializeField] private Ease 渐显缓动 = Ease.OutSine;
    [SerializeField] private Ease 渐隐缓动 = Ease.InSine;

    // 当前高亮的选项索引
    private int 当前高亮索引 = -1;

    void OnDisable()
    {
        // 清理所有动画
        foreach (var 选项 in 选项列表)
        {
            选项.当前动画?.Kill();
        }
    }

    void Start()
    {
        // 初始化所有选项
        for (int i = 0; i < 选项列表.Count; i++)
        {
            var 选项 = 选项列表[i];
            
            if (选项.toggle == null)
            {
                Debug.LogWarning($"[{组名称}] 选项 {i} 的Toggle为空，跳过", this);
                continue;
            }

            if (选项.背景块 == null)
            {
                Debug.LogWarning($"[{组名称}] 选项 {i} 的背景块为空，跳过", this);
                continue;
            }

            // 初始化背景块透明度
            设置背景透明度(选项, 未选中透明度, 0);

            // 监听Toggle事件（使用闭包捕获索引）
            int 索引 = i;
            选项.toggle.OnToggled.AddListener((是否选中) => 处理Toggle切换(索引, 是否选中));
        }

        Debug.Log($"[{组名称}] 初始化完成，共 {选项列表.Count} 个选项");
    }

    /// <summary>
    /// 处理Toggle切换事件 - 只播放动画，不改变Toggle状态
    /// </summary>
    /// <param name="索引">被切换的选项索引</param>
    /// <param name="是否选中">Toggle的选中状态</param>
    private void 处理Toggle切换(int 索引, bool 是否选中)
    {
        if (索引 < 0 || 索引 >= 选项列表.Count) return;

        var 当前选项 = 选项列表[索引];

        if (是否选中)
        {
            Debug.Log($"[{组名称}] 选项 {索引} ({当前选项.toggle.name}) 被选中，播放高亮动画");

            // 隐藏其他所有选项的背景（只播放动画，不改变Toggle状态）
            for (int i = 0; i < 选项列表.Count; i++)
            {
                if (i != 索引)
                {
                    播放取消动画(选项列表[i]);
                }
            }

            // 播放当前选项的选中动画
            播放选中动画(当前选项);
            当前高亮索引 = 索引;
        }
        else
        {
            Debug.Log($"[{组名称}] 选项 {索引} ({当前选项.toggle.name}) 被取消选中，播放隐藏动画");
            
            // 播放取消动画
            播放取消动画(当前选项);
            
            if (当前高亮索引 == 索引)
            {
                当前高亮索引 = -1;
            }
        }
    }

    /// <summary>
    /// 播放选中动画 - 背景渐显
    /// </summary>
    private void 播放选中动画(选项项 选项)
    {
        if (选项.背景块 == null) return;

        // 停止当前动画，避免冲突
        选项.当前动画?.Kill();

        // 渐显动画
        设置背景透明度(选项, 选中透明度, 渐显时长, 渐显缓动);
    }

    /// <summary>
    /// 播放取消动画 - 背景渐隐
    /// </summary>
    private void 播放取消动画(选项项 选项)
    {
        if (选项.背景块 == null) return;

        // 停止当前动画，避免冲突
        选项.当前动画?.Kill();

        // 渐隐动画
        设置背景透明度(选项, 未选中透明度, 渐隐时长, 渐隐缓动);
    }

    /// <summary>
    /// 设置背景块透明度（使用 DOTween 动画）
    /// </summary>
    /// <param name="选项">目标选项</param>
    /// <param name="目标透明度">目标透明度值（0-1）</param>
    /// <param name="时长">动画时长（秒）</param>
    /// <param name="缓动">缓动类型</param>
    private void 设置背景透明度(选项项 选项, float 目标透明度, float 时长, Ease 缓动 = Ease.Linear)
    {
        if (选项.背景块 == null) return;

        Color 当前颜色 = 选项.背景块.Color;
        Color 目标颜色 = new Color(当前颜色.r, 当前颜色.g, 当前颜色.b, 目标透明度);

        if (时长 <= 0)
        {
            // 直接设置，无动画
            选项.背景块.Color = 目标颜色;
        }
        else
        {
            // 使用 DOTween 动画
            选项.当前动画 = DOTween.To(
                () => 选项.背景块.Color,
                x => 选项.背景块.Color = x,
                目标颜色,
                时长
            ).SetEase(缓动);
        }
    }

    /// <summary>
    /// 隐藏所有背景（只播放动画）
    /// </summary>
    public void 隐藏所有背景()
    {
        Debug.Log($"[{组名称}] 隐藏所有背景");
        
        // 播放所有背景的渐隐动画
        foreach (var 选项 in 选项列表)
        {
            if (选项.背景块 != null)
            {
                播放取消动画(选项);
            }
        }
        
        当前高亮索引 = -1;
    }

    /// <summary>
    /// 显示指定索引的背景（只播放动画）
    /// </summary>
    /// <param name="索引">要显示背景的选项索引</param>
    public void 显示背景(int 索引)
    {
        if (索引 >= 0 && 索引 < 选项列表.Count)
        {
            // 先隐藏其他所有背景
            for (int i = 0; i < 选项列表.Count; i++)
            {
                if (i != 索引)
                {
                    播放取消动画(选项列表[i]);
                }
            }
            
            // 显示指定背景
            var 选项 = 选项列表[索引];
            if (选项.背景块 != null)
            {
                播放选中动画(选项);
                当前高亮索引 = 索引;
            }
        }
    }

    /// <summary>
    /// 获取当前高亮的选项索引
    /// </summary>
    public int 获取当前高亮索引()
    {
        return 当前高亮索引;
    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
