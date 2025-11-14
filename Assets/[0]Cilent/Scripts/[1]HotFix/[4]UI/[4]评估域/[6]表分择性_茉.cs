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
    
    // 下拉框展开事件 - 用于通知其他表分择性收起
    public static event Action<表分择性_茉> 下拉框展开事件;
    
    // 静态列表，保存所有实例
    private static List<表分择性_茉> 所有实例 = new List<表分择性_茉>();
    
    // 标记是否已初始化
    private bool 是否已初始化 = false;
    
    // 待还原的分数值(-1表示无待还原数据)
    private int 待还原分数 = -1;
    
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
            // 订阅展开事件
            if (表分选择.Visuals != null)
            {
                表分选择.Visuals.OnExpand += 处理下拉框展开;
            }
        }

        // 初始化填充下拉框
        填充表分选择下拉框();
        
        // 如果有待还原的分数,现在还原它
        if (待还原分数 >= 0)
        {
            设置选中分数内部(待还原分数);
            待还原分数 = -1;
        }
        
        是否已初始化 = true;
        
        // 添加到所有实例列表
        if (!所有实例.Contains(this))
        {
            所有实例.Add(this);
        }
        
        // 订阅下拉框展开事件
        下拉框展开事件 += 处理其他下拉框展开;
    }
    
    void OnDestroy()
    {
        // 取消订阅事件，避免内存泄漏
        if (表分选择 != null)
        {
            表分选择.OnValueChanged.RemoveListener(处理选择变化);
            // 取消订阅展开事件
            if (表分选择.Visuals != null)
            {
                表分选择.Visuals.OnExpand -= 处理下拉框展开;
            }
        }
        
        // 从所有实例列表中移除
        所有实例.Remove(this);
        
        // 取消订阅下拉框展开事件
        下拉框展开事件 -= 处理其他下拉框展开;
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
    
    /// <summary>
    /// 处理下拉框展开
    /// </summary>
    private void 处理下拉框展开()
    {
        // 通知其他表分择性，当前这个下拉框展开了
        下拉框展开事件?.Invoke(this);
    }
    
    /// <summary>
    /// 处理其他下拉框展开的事件
    /// </summary>
    /// <param name="展开的实例">展开的表分择性实例</param>
    private void 处理其他下拉框展开(表分择性_茉 展开的实例)
    {
        // 如果不是自己，就收起自己的下拉框
        if (展开的实例 != this && 表分选择 != null)
        {
            收起下拉框();
        }
    }
    
    /// <summary>
    /// 收起下拉框
    /// </summary>
    private void 收起下拉框()
    {
        if (表分选择 != null && 表分选择.Visuals != null && 表分选择.Visuals.IsExpanded)
        {
            表分选择.Collapse();
        }
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
            表分选择.DropdownOptions.Options.Add("  ");
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
    /// 设置选中的分数值(公共接口)
    /// </summary>
    /// <param name="分数">要选中的分数值</param>
    public void 设置选中分数(int 分数)
    {
        if (!是否已初始化)
        {
            // 如果还未初始化,保存待还原的分数,等Start执行后再设置
            待还原分数 = 分数;
            Debug.Log($"[{name}] 表分择性还未初始化,保存待还原分数: {分数}");
            return;
        }
        
        设置选中分数内部(分数);
    }
    
    /// <summary>
    /// 设置选中的分数值(内部方法)
    /// </summary>
    /// <param name="分数">要选中的分数值</param>
    private void 设置选中分数内部(int 分数)
    {
        if (表分选择 == null)
        {
            Debug.LogWarning($"[{name}] 表分选择组件为空");
            return;
        }
        
        // 查找对应分数的选项
        string 分数字符串 = 分数.ToString();
        int 索引 = 表分选择.DropdownOptions.Options.IndexOf(分数字符串);
        
        if (索引 >= 0)
        {
            表分选择.DropdownOptions.SelectedIndex = 索引;
            Debug.Log($"[{name}] 表分择性设置选中分数成功: {分数}, 索引: {索引}");
        }
        else
        {
            Debug.LogWarning($"[{name}] 未找到分数 {分数} 对应的选项, 当前选项数: {表分选择.DropdownOptions.Options.Count}");
            // 打印所有可用选项用于调试
            Debug.Log($"[{name}] 可用选项: {string.Join(", ", 表分选择.DropdownOptions.Options)}");
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
