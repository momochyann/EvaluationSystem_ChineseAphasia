using System.Collections;
using System.Collections.Generic;
using NovaSamples.UIControls;
using UnityEngine;
using System;
using QFramework;
using DG.Tweening;
using Nova;



public class 编号管理界面_茉 : MonoBehaviour, IController
{
    [SerializeField] TextField 输入框;
    [SerializeField] Dropdown 下拉框;
    [SerializeField] Button 确认按钮;
    [SerializeField] Button 取消按钮;
    [SerializeField] UIBlock 面板容器;
    [SerializeField] TextBlock 预览文本; // 可选：用于显示生成规则预览
    
    private const string 编号规则配置键 = "编号规则配置";
    private 编号规则配置 当前配置;
    
    void Start()
    {
        初始化界面();
        加载配置();
        面板容器?.transform.DOScale(1, 0.3f).From(0);
    }

    void 初始化界面()
    {
        // 设置下拉框选项
        下拉框.DropdownOptions.Options.Clear();
        下拉框.DropdownOptions.Options.Add("前缀");
        下拉框.DropdownOptions.Options.Add("后缀");
        
        // 绑定按钮事件
        确认按钮.OnClicked.AddListener(确认设置);
        取消按钮.OnClicked.AddListener(取消设置);
        
        // 监听下拉框变化以更新预览
        下拉框.OnValueChanged.AddListener(更新预览);
    }

    void 加载配置()
    {
        // 从本地存储加载配置
        if (ES3.KeyExists(编号规则配置键))
        {
            当前配置 = ES3.Load<编号规则配置>(编号规则配置键);
            输入框.Text = 当前配置.自定义字符串;
            下拉框.DropdownOptions.SelectedIndex = 当前配置.规则类型;
        }
        else
        {
            // 默认配置
            当前配置 = new 编号规则配置();
            下拉框.DropdownOptions.SelectedIndex = 0;
        }
        
        更新预览(string.Empty);
    }

    void 更新预览(string newValue)
    {
        if (预览文本 != null)
        {
            string 示例编号 = 生成示例编号();
            预览文本.Text = $"示例: {示例编号}";
        }
    }

    string 生成示例编号()
    {
        string 日期部分 = DateTime.Now.ToString("yyyyMMdd");
        string 序号部分 = "001";
        string 自定义部分 = 输入框.Text;
        
        if (下拉框.DropdownOptions.SelectedIndex == 0) // 前缀
        {
            return $"{自定义部分}{日期部分}{序号部分}";
        }
        else // 后缀
        {
            return $"{日期部分}{序号部分}{自定义部分}";
        }
    }

    void 确认设置()
    {
        // 限制输入框字符数为1-5
        if (输入框.Text.Length > 5)
        {
            输入框.Text = 输入框.Text.Substring(0, 5);
        }
        
        // 验证输入
        if (string.IsNullOrEmpty(输入框.Text))
        {
            Debug.LogWarning("请输入前缀或后缀字符（1-5个字符）");
            return;
        }
        
        if (输入框.Text.Length < 1 || 输入框.Text.Length > 5)
        {
            Debug.LogWarning("前缀或后缀字符长度必须在1-5个字符之间");
            return;
        }
        
        // 保存配置
        当前配置 = new 编号规则配置
        {
            规则类型 = 下拉框.DropdownOptions.SelectedIndex,
            自定义字符串 = 输入框.Text
        };
        
        ES3.Save(编号规则配置键, 当前配置);
        Debug.Log($"编号规则已保存: {(当前配置.规则类型 == 0 ? "前缀" : "后缀")} - {当前配置.自定义字符串}");
        
        // 关闭界面
        Destroy(gameObject);
    }

    void 取消设置()
    {
        Debug.Log("取消编号规则设置");
        Destroy(gameObject);
    }

    /// <summary>
    /// 静态方法：根据当前配置生成编号
    /// </summary>
    public static string 生成用户编号()
    {
        编号规则配置 配置;
        
        // 加载配置
        if (ES3.KeyExists("编号规则配置"))
        {
            配置 = ES3.Load<编号规则配置>("编号规则配置");
        }
        else
        {
            // 使用默认配置
            配置 = new 编号规则配置
            {
                规则类型 = 0,
                自定义字符串 = "U"
            };
        }
        
        // 获取当前日期
        string 日期部分 = DateTime.Now.ToString("yyyyMMdd");
        
        // 获取今天已有的编号数量，计算序号
        int 今日序号 = 获取今日编号序号(日期部分);
        string 序号部分 = 今日序号.ToString("D3"); // 格式化为3位数字，如001, 002
        
        // 根据规则类型生成编号
        string 最终编号;
        if (配置.规则类型 == 0) // 前缀
        {
            最终编号 = $"{配置.自定义字符串}{日期部分}{序号部分}";
        }
        else // 后缀
        {
            最终编号 = $"{日期部分}{序号部分}{配置.自定义字符串}";
        }
        
        return 最终编号;
    }

    /// <summary>
    /// 获取今天已添加的编号序号（从用户录中统计）
    /// </summary>
    private static int 获取今日编号序号(string 日期部分)
    {
        try
        {
            var 架构 = HotFixTemplateArchitecture.Interface;
            var 按名索录之器 = 架构.GetUtility<按名索录之器>();
            var 用户录接口 = 按名索录之器.寻录<I录贞>("用户录");
            
            if (用户录接口 != null)
            {
                var 所有用户 = 用户录接口.GetAllItems();
                int 今日数量 = 0;
                
                foreach (var 用户 in 所有用户)
                {
                    if (用户 is 用户 u && !string.IsNullOrEmpty(u.编号))
                    {
                        // 检查编号中是否包含今天的日期
                        if (u.编号.Contains(日期部分))
                        {
                            今日数量++;
                        }
                    }
                }
                
                return 今日数量 + 1;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"获取今日编号序号时出错: {e.Message}");
        }
        
        return 1;
    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
[System.Serializable]
public class 编号规则配置
{
    public int 规则类型; // 0=前缀, 1=后缀
    public string 自定义字符串 = "";
}