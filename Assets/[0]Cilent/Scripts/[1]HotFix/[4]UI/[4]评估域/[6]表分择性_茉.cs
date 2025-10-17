using System.Collections;
using System.Collections.Generic;
using Nova;
using NovaSamples.UIControls;
using UnityEngine;
using System;

public class 表分择性_茉 : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] TextBlock 分数文本;
    [SerializeField] Dropdown 表分选择;
    
    // 选择变化事件
    public static event Action 表分选择变化事件;
    
    void Start()
    {
        if (分数文本 == null)
        {
            分数文本 = transform.Find("分数").GetComponent<TextBlock>();
        }
        if (表分选择 == null)
        {
            表分选择 = transform.Find("选择下拉框").GetComponent<Dropdown>();
        }
        
        // 订阅下拉框选择变化事件
        if (表分选择 != null)
        {
            表分选择.OnValueChanged.AddListener(处理选择变化);
        }

        // 初始化填充下拉框
        填充表分选择下拉框();
    }
    
    void OnDestroy()
    {
        // 取消订阅事件，避免内存泄漏
        if (表分选择 != null)
        {
            表分选择.OnValueChanged.RemoveListener(处理选择变化);
        }
    }
    
    /// <summary>
    /// 处理下拉框选择变化
    /// </summary>
    /// <param name="选中值">用户选择的值</param>
    private void 处理选择变化(string 选中值)
    {
        // 触发静态事件，通知其他组件
        表分选择变化事件?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    /// <summary>
    /// 根据分数文本填充表分选择下拉框
    /// </summary>
    private void 填充表分选择下拉框()
    {
        if (分数文本 == null || 表分选择 == null)
        {
            Debug.LogWarning("分数文本或表分选择组件为空");
            return;
        }
        
        // 获取分数文本的数字值
        string 分数文本内容 = 分数文本.Text;
        int 最大分数 = 0;

        // 尝试解析分数文本为数字
        if (int.TryParse(分数文本内容, out 最大分数))
        {
            // 清空现有选项
            表分选择.DropdownOptions.Options.Clear();
            表分选择.DropdownOptions.Options.Add("选择");
            // 从0到最大分数填充选项
            for (int i = 0; i <= 最大分数; i++)
            {
                表分选择.DropdownOptions.Options.Add(i.ToString());
            }

            // 重置选中索引
            表分选择.DropdownOptions.SelectedIndex = 0;
        }
        else
        {
            // 如果无法解析为数字，添加默认选项
            表分选择.DropdownOptions.Options.Clear();
            表分选择.DropdownOptions.Options.Add("无效分数");
            表分选择.DropdownOptions.SelectedIndex = 0;

            Debug.LogWarning($"无法解析分数文本为数字：{分数文本内容}");
        }
        表分选择.Visuals.OptionsView.Refresh();
    }
    
    /// <summary>
    /// 获取当前选择的分数值
    /// </summary>
    /// <returns>选择的分数值，如果未选择有效数字则返回-1</returns>
    public int 获取选择的分数()
    {
        if (表分选择 == null)
        {
            return -1;
        }
        
        string 当前选择 = 表分选择.DropdownOptions.CurrentSelection;
        
        if (当前选择 == "选择" || 当前选择 == "无效分数")
        {
            return -1;
        }
        
        if (int.TryParse(当前选择, out int 分数值))
        {
            return 分数值;
        }
        
        return -1;
    }
    
    /// <summary>
    /// 检查是否已选择有效选项
    /// </summary>
    /// <returns>是否已选择有效选项</returns>
    public bool 是否已选择()
    {
        return 获取选择的分数() != -1;
    }

    /// <summary>
    /// 手动刷新下拉框（可以在外部调用）
    /// </summary>
    public void 刷新下拉框()
    {
        填充表分选择下拉框();
    }
    
    /// <summary>
    /// 设置选中的分数值
    /// </summary>
    /// <param name="分数">要选中的分数值</param>
    public void 设置选中分数(int 分数)
    {
        if (表分选择 == null)
        {
            Debug.LogWarning("表分选择组件为空");
            return;
        }
        
        // 查找对应分数的选项
        string 分数字符串 = 分数.ToString();
        int 索引 = 表分选择.DropdownOptions.Options.IndexOf(分数字符串);
        
        if (索引 >= 0)
        {
            表分选择.DropdownOptions.SelectedIndex = 索引;
            Debug.Log($"表分择性设置选中分数: {分数}");
        }
        else
        {
            Debug.LogWarning($"未找到分数 {分数} 对应的选项");
        }
    }
    
    /// <summary>
    /// 清除选择，重置为"选择"选项
    /// </summary>
    public void 清除选择()
    {
        if (表分选择 == null)
        {
            return;
        }
        
        // 设置回"选择"选项（索引0）
        表分选择.DropdownOptions.SelectedIndex = 0;
    }
}
