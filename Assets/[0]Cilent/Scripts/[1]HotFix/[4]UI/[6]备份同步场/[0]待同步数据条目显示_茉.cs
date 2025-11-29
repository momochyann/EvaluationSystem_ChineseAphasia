using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Nova;
using NovaSamples.UIControls;
using QFramework;
using Cysharp.Threading.Tasks;
using LitJson;

/// <summary>
/// 备份同步场 - 待同步数据条目显示
/// 仅负责显示：编号 / 姓名 / 性别 / 添加时间 / 类型 + 同步按钮
/// 数据来自 待同步数据录
/// </summary>
public class 待同步数据条目显示_茉 : MonoBehaviour, IentryDis, IController
{
    public int 索引下标 { get; set; }

    [Header("基础字段显示")]
    [SerializeField] private TextBlock 编号文本;
    [SerializeField] private TextBlock 姓名文本;
    [SerializeField] private TextBlock 性别文本;
    [SerializeField] private TextBlock 添加时间文本;
    [SerializeField] private TextBlock 类型文本;

    [Header("交互")]
    [SerializeField] private Button 同步按钮;

    private 待同步数据录 待同步录;
    private 待同步数据 当前待同步数据;

    private void Start()
    {
        if (同步按钮 != null)
        {
            同步按钮.OnClicked.AddListener(() => 执行同步占位().Forget());
        }
    }

    /// <summary>
    /// IentryDis 接口：由 显示条目_韩 调用
    /// 默认只传索引
    /// </summary>
    /// <param name="索引"></param>
    public void 显示信息(int 索引 = -1)
    {
        待同步录 = this.GetModel<待同步数据录>();
        if (待同步录 == null)
        {
            Debug.LogError("[待同步数据条目显示_茉] 未能获取 待同步数据录 模型，无法显示");
        }

        索引下标 = 索引;

        if (待同步录 == null)
        {
            Debug.LogError("[待同步数据条目显示_茉] 未获取到 待同步数据录，无法显示");
            return;
        }

        var 列表 = 待同步录.GetDataList();
        if (索引 < 0 || 索引 >= 列表.Count)
        {
            Debug.LogError($"[待同步数据条目显示_茉] 待同步数据录 索引无效: {索引}");
            return;
        }

        当前待同步数据 = 列表[索引];

        if (编号文本 != null) 编号文本.Text = 当前待同步数据?.编号 ?? string.Empty;
        if (姓名文本 != null) 姓名文本.Text = 当前待同步数据?.姓名 ?? string.Empty;
        if (性别文本 != null) 性别文本.Text = 当前待同步数据?.性别 ?? string.Empty;
        if (添加时间文本 != null) 添加时间文本.Text = 当前待同步数据?.添加时间 ?? string.Empty;
        if (类型文本 != null) 类型文本.Text = 当前待同步数据?.类型 ?? string.Empty;
    }

    /// <summary>
    /// 同步按钮点击：执行真正的同步逻辑
    /// </summary>
    private async UniTaskVoid 执行同步占位()
    {
        // 显示确认对话框
        var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 界面管理器 = FindObjectOfType<界面管理器_韩>();
        var ins = Instantiate(告示, 界面管理器.transform);
        var 户择 = await ins.GetComponent<告示_茉>().等待确认显示("提示", "即将同步数据，请确保蓝牙设备连接正常");
        Destroy(ins);
        
        if (!户择)
        {
            Debug.Log("[待同步数据条目显示_茉] 用户取消同步");
            return;
        }

        if (当前待同步数据 == null)
        {
            Debug.LogWarning("[待同步数据条目显示_茉] 当前待同步数据为空，无法执行同步");
            await 显示提示("错误", "当前待同步数据为空");
            return;
        }

        Debug.Log($"[待同步数据条目显示_茉] 开始同步，来源录: {当前待同步数据.原数据录名称}，原索引: {当前待同步数据.待同步索引}");

        // 获取蓝牙数据同步工具
        var 同步工具 = this.GetUtility<I蓝牙数据同步工具>();
        if (同步工具 == null)
        {
            Debug.LogError("[待同步数据条目显示_茉] 无法获取蓝牙数据同步工具");
            await 显示提示("错误", "无法获取蓝牙数据同步工具，请检查设备连接");
            return;
        }

        // 执行同步
        bool 同步成功 = await 同步工具.发送待同步数据(当前待同步数据);
        
        if (同步成功)
        {
            Debug.Log($"[待同步数据条目显示_茉] 同步成功: {当前待同步数据.姓名} ({当前待同步数据.类型})");
            await 显示提示("成功", $"数据同步成功\n{当前待同步数据.姓名} ({当前待同步数据.类型})");
        }
        else
        {
            Debug.LogError($"[待同步数据条目显示_茉] 同步失败: {当前待同步数据.姓名} ({当前待同步数据.类型})");
            await 显示提示("失败", $"数据同步失败\n{当前待同步数据.姓名} ({当前待同步数据.类型})");
        }
    }

    /// <summary>
    /// 显示提示信息
    /// </summary>
    private async UniTask 显示提示(string 标题, string 内容)
    {
        var utility = this.GetUtility<YooAssetPfbPoolUtility>();
        var prefab = await utility.LoadPfb("告示");
        if (prefab == null)
        {
            Debug.LogError("[待同步数据条目显示_茉] 加载告示预制体失败");
            return;
        }

        var 界面管理器 = FindObjectOfType<界面管理器_韩>();
        var tips = Instantiate(prefab, 界面管理器.transform);
        await tips.GetComponent<告示_茉>().等待确认显示(标题, 内容);
        Destroy(tips);
    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
