using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
using NovaSamples.UIControls;
using QFramework;
using System;

public class 蓝牙标识提示 : MonoBehaviour, IController
{
    [Header("UI组件")]
    [SerializeField] private UIBlock2D 蓝牙图标;
    [SerializeField] private TextBlock 状态文本;


    private Sprite 蓝牙连接图片;
    private Sprite 蓝牙未连接图片;

    private 软件配置录 软件配置录;

    void Start()
    {
        初始化蓝牙状态监听();
    }

    async void 初始化蓝牙状态监听()
    {
        蓝牙连接图片 = await LoadYooAssetsTool.LoadAsset<Sprite>("蓝牙已连接");
        蓝牙未连接图片 = await LoadYooAssetsTool.LoadAsset<Sprite>("蓝牙未连接");
        // 获取软件配置录模型
        软件配置录 = this.GetModel<软件配置录>();

        // 绑定到软件配置录的蓝牙状态
        软件配置录._蓝牙状态.Register(更新蓝牙状态显示).UnRegisterWhenGameObjectDestroyed(gameObject);

        // 初始化时更新一次显示
        更新蓝牙状态显示();
    }

    private void 更新蓝牙状态显示(bool 当前连接状态)
    {
        if (蓝牙图标 != null)
        {
            // 更新图标
            蓝牙图标.SetImage(当前连接状态 ? 蓝牙连接图片 : 蓝牙未连接图片);
        }

        if (状态文本 != null)
        {
            // 更新状态文字
            if (当前连接状态)
            {
                状态文本.Text = "蓝牙已连接";
                状态文本.Color = Color.green;
            }
            else
            {
                状态文本.Text = "蓝牙未连接";
                状态文本.Color = Color.red;
            }
        }
    }

    void 更新蓝牙状态显示()
    {
        bool 当前连接状态 = 软件配置录._蓝牙状态.Value;

        if (蓝牙图标 != null)
        {
            // 更新图标
            蓝牙图标.SetImage(当前连接状态 ? 蓝牙连接图片 : 蓝牙未连接图片);
        }

        if (状态文本 != null)
        {
            // 更新状态文字
            if (当前连接状态)
            {
                状态文本.Text = "蓝牙已连接";
                状态文本.Color = Color.green;
            }
            else
            {
                状态文本.Text = "蓝牙未连接";
                状态文本.Color = Color.red;
            }
        }
    }

    // 公共方法：手动更新连接状态
    public void 设置连接状态(bool 连接状态)
    {
        软件配置录._蓝牙状态.Value = 连接状态;
    }

    // 公共方法：获取当前连接状态
    public bool 获取连接状态()
    {
        return 软件配置录._蓝牙状态.Value;
    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
