using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using QFramework;
using System;

/// <summary>
/// 滚轮万年历管理器 - 使用滚轮选择器实现日期选择，滚动即确认
/// </summary>
public class 滚轮万年历管理器 : MonoBehaviour, IController
{
    [Header("滚轮选择器")]
    [SerializeField] 滚轮选择器 年份滚轮;
    [SerializeField] 滚轮选择器 月份滚轮;
    [SerializeField] 滚轮选择器 日期滚轮;
    
    [Header("显示组件")]
    [SerializeField] TMP_Text 当前日期显示;
    
    [Header("UI按钮")]
    [Tooltip("将您在预制体中创建的保存按钮拖拽到这里")]
    [SerializeField] Button 保存按钮;
    [Tooltip("可选，将取消按钮拖拽到这里")]
    [SerializeField] Button 取消按钮;

    [Header("数据范围")]
    [SerializeField] int 最小年份 = 1900;
    [SerializeField] int 最大年份 = 2100;
    
    // 私有变量
    private int 当前年份;
    private int 当前月份;
    private int 当前日期;
    private Coroutine 自动确认协程;
    
    // 事件
    public Action<DateTime> On日期确认;
    public Action On日期取消;
    
    void Start()
    {
        Debug.Log("[管理器探针] Start 方法已调用。");
        初始化万年历();
        绑定事件();
    }
    
    /// <summary>
    /// 初始化万年历
    /// </summary>
    void 初始化万年历()
    {
        Debug.Log("[管理器探针] 1. 进入 初始化万年历");
        
        // 检查滚轮引用
        if (年份滚轮 == null || 月份滚轮 == null || 日期滚轮 == null)
        {
            Debug.LogError("[管理器探针] 错误：一个或多个滚轮选择器引用为空！");
            return;
        }
        Debug.Log("[管理器探针] 2. 滚轮引用检查通过");
        
        // 设置当前日期
        DateTime 现在 = DateTime.Now;
        当前年份 = 现在.Year;
        当前月份 = 现在.Month;
        当前日期 = 现在.Day;
        Debug.Log("[管理器探针] 3. 当前日期设置完毕");
        
        try
        {
            Debug.Log("[管理器探针] 4a. 准备初始化年份滚轮...");
            初始化年份滚轮();
            Debug.Log("[管理器探针] 4b. 年份滚轮初始化完成");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[管理器探针] 4c. 年份滚轮初始化失败: {e.Message}\n{e.StackTrace}");
            return; // 发生错误时中止
        }
        
        try
        {
            Debug.Log("[管理器探针] 5a. 准备初始化月份滚轮...");
            初始化月份滚轮();
            Debug.Log("[管理器探针] 5b. 月份滚轮初始化完成");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[管理器探针] 5c. 月份滚轮初始化失败: {e.Message}\n{e.StackTrace}");
            return;
        }
        
        try
        {
            Debug.Log("[管理器探针] 6a. 准备初始化日期滚轮...");
            初始化日期滚轮();
            Debug.Log("[管理器探针] 6b. 日期滚轮初始化完成");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[管理器探针] 6c. 日期滚轮初始化失败: {e.Message}\n{e.StackTrace}");
            return;
        }
        
        // 自动选择当前日期
        Debug.Log("[管理器探针] 7a. 准备自动选择当前日期...");
        自动选择当前日期();
        Debug.Log("[管理器探针] 7b. 自动选择当前日期完成");
        
        // 更新显示
        Debug.Log("[管理器探针] 8a. 准备更新日期显示...");
        更新当前日期显示();
        Debug.Log("[管理器探针] 8b. 更新日期显示完成");
        Debug.Log("[管理器探针] 9. 万年历初始化流程全部完成！");
    }
    
    /// <summary>
    /// 初始化年份滚轮
    /// </summary>
    void 初始化年份滚轮()
    {
        List<string> 年份列表 = new List<string>();
        for (int i = 最小年份; i <= 最大年份; i++)
        {
            年份列表.Add(i.ToString());
        }
        
        Debug.Log($"[滚轮万年历管理器] 年份列表已生成，共 {年份列表.Count} 项。");
        年份滚轮.设置选项列表(年份列表);
        年份滚轮.设置选中索引(当前年份 - 最小年份);
        年份滚轮.设置自动确认(false); // 禁用自动确认
    }
    
    /// <summary>
    /// 初始化月份滚轮
    /// </summary>
    void 初始化月份滚轮()
    {
        List<string> 月份列表 = new List<string>();
        for (int i = 1; i <= 12; i++)
        {
            月份列表.Add(i.ToString("00"));
        }
        
        月份滚轮.设置选项列表(月份列表);
        月份滚轮.设置选中索引(当前月份 - 1);
        月份滚轮.设置自动确认(false); // 禁用自动确认
    }
    
    /// <summary>
    /// 初始化日期滚轮
    /// </summary>
    void 初始化日期滚轮()
    {
        更新日期滚轮();
        日期滚轮.设置自动确认(false); // 禁用自动确认
    }
    
    /// <summary>
    /// 自动选择当前日期
    /// </summary>
    void 自动选择当前日期()
    {
        try
        {
            // 计算当前日期在滚轮中的索引
            int 年份索引 = 当前年份 - 最小年份;
            int 月份索引 = 当前月份 - 1;
            int 日期索引 = 当前日期 - 1;
            
            Debug.Log($"[滚轮万年历管理器] 自动选择日期: 年份索引={年份索引}, 月份索引={月份索引}, 日期索引={日期索引}");
            
            // 设置各滚轮的选中索引
            年份滚轮.设置选中索引(年份索引);
            月份滚轮.设置选中索引(月份索引);
            日期滚轮.设置选中索引(日期索引);
            
            Debug.Log($"[滚轮万年历管理器] 自动选择完成: {当前年份}年{当前月份}月{当前日期}日");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[滚轮万年历管理器] 自动选择当前日期失败: {e.Message}");
        }
    }
    
    /// <summary>
    /// 更新日期滚轮
    /// </summary>
    void 更新日期滚轮()
    {
        Debug.Log($"[滚轮万年历管理器] 更新日期滚轮: {当前年份}年{当前月份}月");
        
        int 该月天数 = DateTime.DaysInMonth(当前年份, 当前月份);
        List<string> 日期列表 = new List<string>();
        
        for (int i = 1; i <= 该月天数; i++)
        {
            日期列表.Add(i.ToString("00"));
        }
        
        Debug.Log($"[滚轮万年历管理器] 生成日期列表，共 {日期列表.Count} 项");
        
        // 直接设置选项列表和索引，不使用延迟
        日期滚轮.设置选项列表(日期列表);
        
        // 确保当前日期在有效范围内
        int 新日期索引 = Mathf.Clamp(当前日期 - 1, 0, 该月天数 - 1);
        当前日期 = 新日期索引 + 1;
        
        // 立即设置选中索引，保持滚轮响应性
        日期滚轮.设置选中索引(新日期索引);
        
        Debug.Log($"[滚轮万年历管理器] 日期滚轮更新完成，当前日期: {当前日期}");
    }
    
    /// <summary>
    /// 年份确认回调
    /// </summary>
    void On年份改变(int 索引, string 值)
    {
        if(int.TryParse(值, out int newYear))
        {
            当前年份 = newYear;
            ValidateAndUpdateDate();
        }
    }
    
    /// <summary>
    /// 月份确认回调
    /// </summary>
    void On月份改变(int 索引, string 值)
    {
        if (int.TryParse(值, out int newMonth))
        {
            当前月份 = newMonth;
            ValidateAndUpdateDate();
        }
    }
    
    /// <summary>
    /// 日期确认回调
    /// </summary>
    void On日期改变(int 索引, string 值)
    {
        if (int.TryParse(值, out int newDay))
        {
            当前日期 = newDay;
            // 直接调用ValidateAndUpdateDate，不再使用防抖机制
            ValidateAndUpdateDate();
        }
    }
    
    /// <summary>
    /// 核心方法：验证日期并以正确的顺序更新UI
    /// </summary>
    void ValidateAndUpdateDate()
    {
        Debug.Log($"[滚轮万年历管理器] ValidateAndUpdateDate 开始，当前日期: {当前年份}-{当前月份}-{当前日期}");
        
        // 步骤 1: 重新生成日期滚轮的选项，以应对月份天数或闰年的变化
        更新日期滚轮();

        // 步骤 2: 检查当前日期对于新的月份/年份是否有效
        int 该月最大天数 = DateTime.DaysInMonth(当前年份, 当前月份);
        if (当前日期 > 该月最大天数)
        {
            Debug.Log($"[滚轮万年历管理器] 日期无效 ({当前日期} > {该月最大天数})，修正为 {该月最大天数}");
            // 日期无效 (例如，从31日的月份切换到了30日的月份)
            当前日期 = 该月最大天数;

            // 步骤 3 (A): 立即设置日期滚轮索引并更新文本
            日期滚轮.设置选中索引(当前日期 - 1);
            UpdateBottomText();
        }
        else
        {
            Debug.Log("[滚轮万年历管理器] 日期有效，直接更新底部文本。");
            // 步骤 3 (B): 日期有效，直接更新底部文本
            UpdateBottomText();
        }
    }

    /// <summary>
    /// 核心方法：只负责更新底部的日期文本并触发最终事件
    /// </summary>
    void UpdateBottomText()
    {
        更新当前日期显示(); // 只更新 "YYYY年MM月DD日", 不再触发外部事件
    }
    
    /// <summary>
    /// 更新当前日期显示
    /// </summary>
    void 更新当前日期显示()
    {
        if (当前日期显示 != null)
        {
            DateTime 选择日期 = new DateTime(当前年份, 当前月份, 当前日期);
            当前日期显示.text = 选择日期.ToString("yyyy年MM月dd日");
        }
    }
    
    /// <summary>
    /// 绑定事件
    /// </summary>
    void 绑定事件()
    {
        // --- 核心修改：监听实时变化事件，而不是确认事件 ---
        年份滚轮.On选择改变 += On年份改变;
        月份滚轮.On选择改变 += On月份改变;
        日期滚轮.On选择改变 += On日期改变;

        if (保存按钮 != null)
        {
            保存按钮.onClick.AddListener(On保存按钮点击);
        }
        else
        {
            Debug.LogError("万年历的'保存按钮'未在Inspector中指定！");
        }

        if (取消按钮 != null)
        {
            取消按钮.onClick.AddListener(On取消按钮点击);
        }
    }

    void On保存按钮点击()
    {
        DateTime 选择日期 = new DateTime(当前年份, 当前月份, 当前日期);
        On日期确认?.Invoke(选择日期);
        隐藏万年历();
    }

    void On取消按钮点击()
    {
        On日期取消?.Invoke();
        隐藏万年历();
    }
    
    /// <summary>
    /// 设置选择日期
    /// </summary>
    public void 设置选择日期(DateTime 日期)
    {
        当前年份 = 日期.Year;
        当前月份 = 日期.Month;
        当前日期 = 日期.Day;
        
        Debug.Log($"[滚轮万年历管理器] 设置选择日期: {当前年份}年{当前月份}月{当前日期}日");
        
        // 分别命令年月滚轮滚动到指定位置（无回调）
        年份滚轮.设置选中索引(当前年份 - 最小年份);
        月份滚轮.设置选中索引(当前月份 - 1);
        
        // 对于日期滚轮，使用核心验证流程来设置，以确保一致性
        ValidateAndUpdateDate();
        
        // 更新显示
        更新当前日期显示();
    }
    
    /// <summary>
    /// 获取当前选择日期
    /// </summary>
    public DateTime 获取当前选择日期()
    {
        return new DateTime(当前年份, 当前月份, 当前日期);
    }
    
    /// <summary>
    /// 重置为当前日期
    /// </summary>
    public void 重置为当前日期()
    {
        DateTime 现在 = DateTime.Now;
        当前年份 = 现在.Year;
        当前月份 = 现在.Month;
        当前日期 = 现在.Day;
        
        Debug.Log($"[滚轮万年历管理器] 重置为当前日期: {当前年份}年{当前月份}月{当前日期}日");
        
        // 重新初始化各滚轮
        初始化年份滚轮();
        初始化月份滚轮();
        初始化日期滚轮();
        
        // 自动选择当前日期
        自动选择当前日期();
        
        // 更新显示
        更新当前日期显示();
    }
    
    /// <summary>
    /// 显示万年历
    /// </summary>
    public void 显示万年历()
    {
        gameObject.SetActive(true);
    }
    
    /// <summary>
    /// 隐藏万年历
    /// </summary>
    public void 隐藏万年历()
    {
        gameObject.SetActive(false);
    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
} 