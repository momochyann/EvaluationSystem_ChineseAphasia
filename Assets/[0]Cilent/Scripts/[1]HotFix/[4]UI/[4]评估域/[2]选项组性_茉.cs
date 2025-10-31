using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;
using System.Linq;
using System;
using DG.Tweening;
using Nova;
using QFramework;

public class 选项组性_茉 : MonoBehaviour, IController
{
    [SerializeField] public string 选项组名称;
    public bool IsAnimationOnSelect = false;

    [Header("选中动画设置")]
    [SerializeField] private UIBlock2D 背景块;  // 用于显示选中提示的背景
    [SerializeField] private bool GetForm父物体 = false;  // 是否从第一个子物体获取背景块
    [SerializeField] private float 渐显时长 = 0.15f;
    [SerializeField] private float 渐隐时长 = 0.1f;
    [SerializeField] private float 选中透明度 = 1f;
    [SerializeField] private float 未选中透明度 = 0f;

    //[SerializeField] public  TextBlock
    // 选项组列表；
    List<Toggle> 选项组;

    // 用于控制透明度的 Tweener，避免重复动画
    private Tweener 当前动画;

    // 当前选中的选项（公开属性）
    [HideInInspector] public Toggle 当前选中选项;

    // 当前选中选项的索引（公开属性）
    public int 当前选中索引 => 当前选中选项 != null ? 选项组.IndexOf(当前选中选项) : -1;

    // 当前选中选项的名称（公开属性）
    public string 当前选中名称 => 当前选中选项 != null ? 当前选中选项.name : "";

    // 选择变化事件
    public static event Action 选择变化事件;

    void OnEnable()
    {
        // 向 System 注册
        this.GetSystem<选项组高亮管理System>().注册选项组(this);
    }

    void OnDisable()
    {
        // 从 System 注销
        this.GetSystem<选项组高亮管理System>().注销选项组(this);
    }

    void Start()
    {
        选项组 = GetComponentsInChildren<Toggle>().ToList();

        // 为每个选项添加事件监听
        foreach (var 选项 in 选项组)
        {
            选项.OnToggled.AddListener((value) => 处理选项切换(选项, value));
        }

        // 根据设置获取背景块
        if (IsAnimationOnSelect && 背景块 == null)
        {
            if (GetForm父物体)
            {
                // 从第一个子物体获取背景块
                背景块 = transform.parent.GetComponent<UIBlock2D>();
            }
            else
            {
                // 从当前物体获取背景块
                背景块 = GetComponent<UIBlock2D>();
            }
        }

        // 初始化背景块透明度
        if (IsAnimationOnSelect && 背景块 != null)
        {
            设置背景透明度(未选中透明度, 0);
        }
    }

    /// <summary>
    /// 处理选项切换逻辑（单选模式）
    /// </summary>
    /// <param name="当前选项">被点击的选项</param>
    /// <param name="是否选中">是否被选中</param>
    private void 处理选项切换(Toggle 当前选项, bool 是否选中)
    {
        if (是否选中)
        {
            // 如果选中了这个选项，取消其他所有选项的选中状态
            foreach (var 其他选项 in 选项组)
            {
                if (其他选项 != 当前选项 && 其他选项.ToggledOn)
                {
                    其他选项.ToggledOn = false;
                }
            }

            // 设置当前选中的选项
            当前选中选项 = 当前选项;
            Debug.Log("选项 " + 当前选项.name + " 被选中");

            // 播放选中动画 - 显示这个选项组正在被操作
            if (IsAnimationOnSelect)
            {
                if (背景块.Color.a == 选中透明度) return;
                播放选中动画();
                // 通过 System 通知其他选项组取消高亮
                this.GetSystem<选项组高亮管理System>().设置高亮组(this);
            }
        }
        else
        {
            // 如果取消选中，清空当前选中选项
            if (当前选中选项 == 当前选项)
            {
                当前选中选项 = null;
            }
            Debug.Log("选项 " + 当前选项.name + " 被取消选中");

            // 注意：这里不播放取消选中动画
            // 因为取消选中是由外部（其他选项组被选中）触发的
            // 动画会通过 取消高亮() 方法来播放
        }

        // 触发选择变化事件
        选择变化事件?.Invoke();
    }

    /// <summary>
    /// 程序化设置选中的选项
    /// </summary>
    /// <param name="索引">要选中的选项索引</param>
    public void 设置选中选项(int 索引)
    {
        if (索引 >= 0 && 索引 < 选项组.Count)
        {
            选项组[索引].ToggledOn = true;
        }
    }

    /// <summary>
    /// 程序化设置选中的选项
    /// </summary>
    /// <param name="选项名称">要选中的选项名称</param>
    public void 设置选中选项(string 选项名称)
    {
        var 目标选项 = 选项组.FirstOrDefault(x => x.name == 选项名称);
        if (目标选项 != null)
        {
            目标选项.ToggledOn = true;
        }
    }

    /// <summary>
    /// 清除所有选项的选中状态
    /// </summary>
    public void 清除所有选择()
    {
        if (选项组 == null) return;
        foreach (var 选项 in 选项组)
        {
            选项.ToggledOn = false;
        }
        当前选中选项 = null;
    }

    /// <summary>
    /// 播放选中动画 - 背景渐显
    /// </summary>
    private void 播放选中动画()
    {
        if (背景块 == null) return;

        // 停止当前动画，避免冲突
        当前动画?.Kill();

        // 渐显动画
        设置背景透明度(选中透明度, 渐显时长);
    }

    /// <summary>
    /// 取消高亮 - 背景渐隐（由其他选项组调用）
    /// </summary>
    public void 取消高亮()
    {
        if (背景块 == null) return;

        // 停止当前动画，避免冲突
        当前动画?.Kill();

        // 渐隐动画
        设置背景透明度(未选中透明度, 渐隐时长);
    }

    /// <summary>
    /// 设置背景块透明度（使用 DOTween 动画）
    /// </summary>
    /// <param name="目标透明度">目标透明度值（0-1）</param>
    /// <param name="时长">动画时长（秒）</param>
    private void 设置背景透明度(float 目标透明度, float 时长)
    {
        if (背景块 == null) return;

        Color 当前颜色 = 背景块.Color;
        Color 目标颜色 = new Color(当前颜色.r, 当前颜色.g, 当前颜色.b, 目标透明度);

        if (时长 <= 0)
        {
            // 直接设置，无动画
            背景块.Color = 目标颜色;
        }
        else
        {
            // 使用 DOTween 动画
            当前动画 = DOTween.To(
                () => 背景块.Color,
                x => 背景块.Color = x,
                目标颜色,
                时长
            ).SetEase(Ease.OutSine);  // 使用缓动效果，让动画更自然
        }
    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
