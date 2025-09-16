using System;
using System.Collections;
using System.Collections.Generic;
using NovaSamples.UIControls;
using UnityEngine;
using QFramework;
using Cysharp.Threading.Tasks;

public class 右上角功能端PC_茉 : MonoBehaviour, IController
{
    // Start is called before the first frame update
    [SerializeField] public Button 用户名;
    [SerializeField] public Button 退出;
    [SerializeField] GameObject 设置详情;
    [SerializeField] Button 大数据按钮;
    [SerializeField] public Button 蓝牙连接按钮;
    [SerializeField] Button 设置按钮;
    [SerializeField] Button 全屏切换按钮;
    private 评人录 评人录;
    private 管员录 管员录;
    private bool bIsFullScreen = true; // 记录当前是否为全屏状态
    private int uwOriginalWidth = 1920; // 原始屏幕宽度
    private int uwOriginalHeight = 1080; // 原始屏幕高度

    void Start()
    {
        用户名.OnClicked.AddListener(执行用户名管理);
        退出.OnClicked.AddListener(执行退出);
        大数据按钮.OnClicked.AddListener(显示大数据界面);
        设置按钮.OnClicked.AddListener(展示设置);
        全屏切换按钮.OnClicked.AddListener(切换全屏状态);
        评人录 = this.GetModel<评人录>();
        管员录 = this.GetModel<管员录>();
        
        // 初始化全屏状态和原始分辨率
        bIsFullScreen = Screen.fullScreen;
        if (bIsFullScreen)
        {
            // 保存当前全屏状态下的分辨率
            uwOriginalWidth = Screen.width;
            uwOriginalHeight = Screen.height;
        }
        
        Debug.Log($"[右上角功能端] 初始化 - 全屏状态: {bIsFullScreen}, 分辨率: {uwOriginalWidth}x{uwOriginalHeight}");
    }

    private void 显示大数据界面()
    {
        FindObjectOfType<界面管理器_韩>().显示界面("首页界面显示");
    }

    private void 执行退出()
    {

    }
    private void 展示设置()
    {
        if (设置详情.activeSelf)
        {
            设置详情.SetActive(false);
        }
        else
        {
            设置详情.SetActive(true);
        }
    }
    /// <summary>
    /// 切换全屏和窗口化状态
    /// </summary>
    private void 切换全屏状态()
    {
        try
        {
            bIsFullScreen = !bIsFullScreen;
            
            if (bIsFullScreen)
            {
                // 切换到全屏 - 使用保存的原始分辨率
                Screen.SetResolution(uwOriginalWidth, uwOriginalHeight, FullScreenMode.FullScreenWindow);
                Debug.Log($"[右上角功能端] 切换到全屏模式: {uwOriginalWidth}x{uwOriginalHeight}");
            }
            else
            {
                // 切换到窗口化前，先保存当前全屏分辨率
                if (Screen.fullScreen)
                {
                    uwOriginalWidth = Screen.width;
                    uwOriginalHeight = Screen.height;
                    Debug.Log($"[右上角功能端] 保存全屏分辨率: {uwOriginalWidth}x{uwOriginalHeight}");
                }
                
                // 切换到窗口化，设置一个合适的窗口大小
                int uwWindowWidth = 1280;
                int uwWindowHeight = 720;
                Screen.SetResolution(uwWindowWidth, uwWindowHeight, FullScreenMode.Windowed);
                Debug.Log($"[右上角功能端] 切换到窗口模式: {uwWindowWidth}x{uwWindowHeight}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[右上角功能端] 切换屏幕模式失败: {ex.Message}");
        }
    }

    async private void 执行用户名管理()
    {
        var 界面管理器 = FindObjectOfType<界面管理器_韩>();
        if (管员录.登陆管理员 != null)
        {
            界面管理器.显示界面("评定人管理添加");
            await UniTask.WaitUntil(() => 界面管理器._currentPanel != null && 界面管理器._currentPanel.name.Contains("评定人管理添加"));
            界面管理器._currentPanel.GetComponent<评定人管理添加_韩>().显示信息(管员录.登陆管理员);

        }
        else if (评人录.登陆评人 != null)
        {
            界面管理器.显示界面("评定人管理添加");
            await UniTask.WaitUntil(() => 界面管理器._currentPanel != null && 界面管理器._currentPanel.name.Contains("评定人管理添加"));
            界面管理器._currentPanel.GetComponent<评定人管理添加_韩>().显示信息(评人录.登陆评人);
        }
    }


    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
