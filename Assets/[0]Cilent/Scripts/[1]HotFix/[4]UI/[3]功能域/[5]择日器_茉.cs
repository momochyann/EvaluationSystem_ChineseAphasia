using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
using NovaSamples.UIControls;
using DG.Tweening;
public class 择日器_茉 : MonoBehaviour
{
    [Header("下拉框组件")]
    [SerializeField] private Dropdown 年份下拉框;
    [SerializeField] private Dropdown 月份下拉框;
    [SerializeField] private Dropdown 日期下拉框;

    [Header("按钮组件")]
    [SerializeField] private UIBlock 保存按钮;
    [SerializeField] private Button 取消按钮;

    [Header("日期范围设置")]
    [SerializeField] private int 最小年份 = 1900;
    [SerializeField] private int 最大年份 = 2024;
    [SerializeField] private int 默认年份 = 2000;
    [SerializeField] private int 默认月份 = 1;
    [SerializeField] private int 默认日期 = 1;

    // 当前选择的日期
    private int 当前年份;
    private int 当前月份;
    private int 当前日期;

    // 每月天数（非闰年）
    private readonly int[] 月份天数 = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

    /// <summary>
    /// 日期改变事件，参数为 (年, 月, 日)
    /// </summary>
    public event System.Action<int, int, int> On日期改变;

    /// <summary>
    /// 保存按钮点击事件，参数为 (年, 月, 日)
    /// </summary>
    public event System.Action<int, int, int> On保存日期;

    /// <summary>
    /// 获取当前选择的日期
    /// </summary>
    public DateTime 当前选择日期 => new DateTime(当前年份, 当前月份, 当前日期);
    private void Awake()
    {
        var 现有择日器 = FindObjectsOfType<择日器_茉>();
        if (现有择日器.Length > 1)
        {
            foreach (var 择日器 in 现有择日器)
            {
                if (择日器.gameObject != this.gameObject)
                {
                    Destroy(择日器.gameObject); 
                    return;
                }
            }
        }

        transform.DOScale(1, 0.3f).From(0);
    }
    void Start()
    {
        初始化日期选择器();
        取消按钮.OnClicked.AddListener(取消选择);
    }

    private void 取消选择()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 初始化日期选择器
    /// </summary>
    private void 初始化日期选择器()
    {
        // 验证组件引用
        if (年份下拉框 == null || 月份下拉框 == null || 日期下拉框 == null)
        {
            Debug.LogError("择日器_茉: 下拉框组件未正确赋值！");
            return;
        }

        // 初始化默认值
        当前年份 = 默认年份;
        当前月份 = 默认月份;
        当前日期 = 默认日期;

        // 填充下拉框数据
        填充年份下拉框();
        填充月份下拉框();
        填充日期下拉框();

        // 设置默认选中项
        设置默认选择();

        // 绑定事件
        绑定下拉框事件();
        绑定保存按钮事件();
    }

    /// <summary>
    /// 填充年份下拉框
    /// </summary>
    private void 填充年份下拉框()
    {
        年份下拉框.DropdownOptions.Options.Clear();

        for (int year = 最大年份; year >= 最小年份; year--)
        {
            年份下拉框.DropdownOptions.Options.Add(year.ToString());
        }
    }

    /// <summary>
    /// 填充月份下拉框
    /// </summary>
    private void 填充月份下拉框()
    {
        月份下拉框.DropdownOptions.Options.Clear();

        for (int month = 1; month <= 12; month++)
        {
            月份下拉框.DropdownOptions.Options.Add(month.ToString());
        }
    }

    /// <summary>
    /// 填充日期下拉框（根据当前年月动态生成）
    /// </summary>
    private void 填充日期下拉框()
    {
        日期下拉框.DropdownOptions.Options.Clear();
        int 最大日期 = 获取月份最大天数(当前年份, 当前月份);

        for (int day = 1; day <= 最大日期; day++)
        {
            日期下拉框.DropdownOptions.Options.Add(day.ToString());
        }




    }

    /// <summary>
    /// 刷新下拉框的ListView数据源
    /// </summary>
    private void 刷新下拉框ListView()
    {
        // 如果下拉框当前是展开状态，需要重新设置ListView数据源
        if (日期下拉框.Visuals.IsExpanded)
        {
            // 方法1：直接调用ListView的Refresh方法
            日期下拉框.Visuals.OptionsView.Refresh();
            Debug.Log("调用ListView.Refresh()方法");

            // 方法2：重新设置数据源
            日期下拉框.Visuals.OptionsView.SetDataSource(日期下拉框.DropdownOptions.Options);
            Debug.Log("重新设置ListView数据源");

            // 方法3：再次调用Refresh确保更新
            日期下拉框.Visuals.OptionsView.Refresh();
            Debug.Log("再次调用Refresh确保更新完成");
        }
    }





    /// <summary>
    /// 获取指定年月的最大天数
    /// </summary>
    private int 获取月份最大天数(int year, int month)
    {
        if (month == 2 && 是闰年(year))
        {
            return 29;
        }
        return 月份天数[month - 1];
    }

    /// <summary>
    /// 判断是否为闰年
    /// </summary>
    private bool 是闰年(int year)
    {
        return (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0);
    }

    /// <summary>
    /// 设置默认选择
    /// </summary>
    private void 设置默认选择()
    {
        // 设置年份
        int 年份索引 = 最大年份 - 默认年份;
        if (年份索引 >= 0 && 年份索引 < (最大年份 - 最小年份 + 1))
        {
            年份下拉框.DropdownOptions.SelectedIndex = 年份索引;
        }

        // 设置月份
        if (默认月份 >= 1 && 默认月份 <= 12)
        {
            月份下拉框.DropdownOptions.SelectedIndex = 默认月份 - 1;
        }

        // 设置日期
        int 最大日期 = 获取月份最大天数(当前年份, 当前月份);
        int 有效日期 = Mathf.Min(默认日期, 最大日期);
        日期下拉框.DropdownOptions.SelectedIndex = 有效日期 - 1;

        // 更新显示标签
        年份下拉框.载始项();
        月份下拉框.载始项();
        日期下拉框.载始项();
    }

    /// <summary>
    /// 绑定下拉框事件
    /// </summary>
    private void 绑定下拉框事件()
    {
        年份下拉框.OnValueChanged.AddListener(年份改变时);
        月份下拉框.OnValueChanged.AddListener(月份改变时);
        日期下拉框.OnValueChanged.AddListener(日期改变时);
    }

    /// <summary>
    /// 绑定保存按钮事件
    /// </summary>
    private void 绑定保存按钮事件()
    {
        if (保存按钮 != null)
        {
            保存按钮.AddGestureHandler<Gesture.OnClick>(保存按钮点击时);
        }
        else
        {
            Debug.LogWarning("择日器_茉: 保存按钮组件未赋值，跳过绑定事件");
        }
    }

    /// <summary>
    /// 年份改变事件处理
    /// </summary>
    private void 年份改变时(string selectedValue)
    {
        if (int.TryParse(selectedValue, out int year))
        {
            当前年份 = year;

            // 获取新年份该月的最大天数（主要处理闰年2月的情况）
            int 最大日期 = 获取月份最大天数(当前年份, 当前月份);

            // 保存当前日期值
            int 原日期值 = 当前日期;

            // 重新填充日期下拉框（主要为了处理闰年）
            填充日期下拉框();

            // 智能选择新的日期
            if (原日期值 <= 最大日期)
            {
                // 如果原日期值在新年份该月中有效，则保持日期值
                当前日期 = 原日期值;
                日期下拉框.DropdownOptions.SelectedIndex = 原日期值 - 1;
            }
            else
            {
                // 如果原日期值超出范围（如闰年2月29日到平年），则调整到该月最后一天
                当前日期 = 最大日期;
                日期下拉框.DropdownOptions.SelectedIndex = 最大日期 - 1;
            }

            // 刷新显示标签
            日期下拉框.载始项();

            Debug.Log($"年份选择完成: {当前年份}年{当前月份}月{当前日期}日");
            触发日期改变事件();
        }
    }

    /// <summary>
    /// 月份改变事件处理
    /// </summary>
    private void 月份改变时(string selectedValue)
    {
        if (int.TryParse(selectedValue, out int month))
        {
            当前月份 = month;

            // 获取新月份的最大天数
            int 最大日期 = 获取月份最大天数(当前年份, 当前月份);

            // 保存当前日期值
            int 原日期值 = 当前日期;

            // 重新填充日期下拉框选项
            填充日期下拉框();

            // 智能选择新的日期
            if (原日期值 <= 最大日期)
            {
                // 如果原日期值在新月份中有效，则保持日期值
                当前日期 = 原日期值;
                日期下拉框.DropdownOptions.SelectedIndex = 原日期值 - 1;
            }
            else
            {
                // 如果原日期值超出范围，则选择该月的最后一天
                当前日期 = 最大日期;
                日期下拉框.DropdownOptions.SelectedIndex = 最大日期 - 1;
            }

            // 刷新显示标签
            日期下拉框.载始项();

            Debug.Log($"月份选择完成: {当前年份}年{当前月份}月{当前日期}日");
            触发日期改变事件();
        }
    }

    /// <summary>
    /// 日期改变事件处理
    /// </summary>
    private void 日期改变时(string selectedValue)
    {
        if (int.TryParse(selectedValue, out int day))
        {
            当前日期 = day;
            Debug.Log($"日期改变为: {当前日期}");
            触发日期改变事件();
        }
    }

    /// <summary>
    /// 保存按钮点击事件处理
    /// </summary>
    private void 保存按钮点击时(Gesture.OnClick evt)
    {
        Debug.Log($"保存按钮被点击，当前日期: {当前年份}年{当前月份}月{当前日期}日");
        On保存日期?.Invoke(当前年份, 当前月份, 当前日期);
        Destroy(this.gameObject, 0.1f);
    }

    /// <summary>
    /// 触发日期改变事件
    /// </summary>
    private void 触发日期改变事件()
    {
        On日期改变?.Invoke(当前年份, 当前月份, 当前日期);
        Debug.Log($"当前选择日期: {当前年份}年{当前月份}月{当前日期}日");
    }

    /// <summary>
    /// 设置指定日期（外部调用）
    /// </summary>
    public void 设置日期(int year, int month, int day)
    {
        // 验证日期有效性
        if (year < 最小年份 || year > 最大年份)
        {
            Debug.LogWarning($"年份超出范围: {year}，有效范围: {最小年份}-{最大年份}");
            return;
        }

        if (month < 1 || month > 12)
        {
            Debug.LogWarning($"月份超出范围: {month}，有效范围: 1-12");
            return;
        }

        int 最大日期 = 获取月份最大天数(year, month);
        if (day < 1 || day > 最大日期)
        {
            Debug.LogWarning($"日期超出范围: {day}，{year}年{month}月有效范围: 1-{最大日期}");
            return;
        }

        // 更新内部状态
        当前年份 = year;
        当前月份 = month;
        当前日期 = day;

        // 更新UI显示
        年份下拉框.DropdownOptions.SelectedIndex = 最大年份 - year;
        年份下拉框.载始项();
        月份下拉框.DropdownOptions.SelectedIndex = month - 1;
        月份下拉框.载始项();
        填充日期下拉框();
        日期下拉框.DropdownOptions.SelectedIndex = day - 1;
        日期下拉框.载始项();

        触发日期改变事件();
    }

    /// <summary>
    /// 设置为今天的日期
    /// </summary>
    public void 设置为今天()
    {
        DateTime 今天 = DateTime.Now;
        设置日期(今天.Year, 今天.Month, 今天.Day);
    }

    /// <summary>
    /// 获取格式化的日期字符串
    /// </summary>
    public string 获取格式化日期(string format = "yyyy年MM月dd日")
    {
        return 当前选择日期.ToString(format);
    }

    /// <summary>
    /// 手动触发保存事件（可以通过代码调用）
    /// </summary>
    public void 手动保存()
    {
        Debug.Log($"手动保存日期: {当前年份}年{当前月份}月{当前日期}日");
        On保存日期?.Invoke(当前年份, 当前月份, 当前日期);
    }

    /// <summary>
    /// 验证当前选择的日期是否有效
    /// </summary>
    public bool 验证日期有效性()
    {
        try
        {
            DateTime testDate = new DateTime(当前年份, 当前月份, 当前日期);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 获取当前日期的详细信息
    /// </summary>
    public (int 年, int 月, int 日, string 格式化字符串, bool 是否有效) 获取当前日期信息()
    {
        return (当前年份, 当前月份, 当前日期, 获取格式化日期(), 验证日期有效性());
    }



    /// <summary>
    /// 测试月份选择功能（开发调试用）
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void 测试月份选择()
    {
        Debug.Log("=== 测试月份选择功能 ===");

        // 测试31天→28天
        设置日期(2023, 1, 31);
        Debug.Log($"1月31日 → 选项数: {日期下拉框.DropdownOptions.Options.Count}");

        // 展开下拉框，用户可以看到当前有31个选项
        日期下拉框.Expand();
        Debug.Log("日期下拉框已展开，请观察选项数量");

        // 等待2秒让用户看到31个选项
        DOTween.Sequence()
            .AppendInterval(2f)
            .AppendCallback(() =>
            {
                // 现在改变月份到2月
                月份改变时("2");
                Debug.Log($"2月 → 选项数: {日期下拉框.DropdownOptions.Options.Count}，日期: {当前日期}");
                Debug.Log("现在应该看到选项数量变为28个");
            });

        Debug.Log("=== 测试进行中，请观察UI变化 ===");
    }

    /// <summary>
    /// 清理事件绑定
    /// </summary>
    private void OnDestroy()
    {
        // 清理下拉框事件
        if (年份下拉框 != null)
            年份下拉框.OnValueChanged.RemoveListener(年份改变时);
        if (月份下拉框 != null)
            月份下拉框.OnValueChanged.RemoveListener(月份改变时);
        if (日期下拉框 != null)
            日期下拉框.OnValueChanged.RemoveListener(日期改变时);

        // 清理保存按钮事件
        if (保存按钮 != null)
            保存按钮.RemoveGestureHandler<Gesture.OnClick>(保存按钮点击时);
    }
}
