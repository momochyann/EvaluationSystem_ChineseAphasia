using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
using NovaSamples.UIControls;
using QFramework;

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
            同步按钮.OnClicked.AddListener(执行同步占位);
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
    /// 同步按钮点击：暂时只做占位打印
    /// </summary>
    private void 执行同步占位()
    {
        if (当前待同步数据 == null)
        {
            Debug.LogWarning("[待同步数据条目显示_茉] 当前待同步数据为空，无法执行同步占位");
            return;
        }

        Debug.Log($"[待同步数据条目显示_茉] 同步占位触发，来源录: {当前待同步数据.原数据录名称}，原索引: {当前待同步数据.待同步索引}，当前索引: {索引下标}");
        // TODO: 后续在这里接入真正的同步逻辑
    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
