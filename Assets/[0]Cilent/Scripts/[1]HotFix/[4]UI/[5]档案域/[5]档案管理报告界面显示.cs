using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Nova;
using System.Linq;
using Cysharp.Threading.Tasks;
using System;

public class 档案管理报告界面显示 : 界面基础_韩
{
    [Header("基本信息")]
    [SerializeField] private TextBlock 姓名;
    [SerializeField] private TextBlock 性别;
    [SerializeField] private TextBlock 出生日期;
    [SerializeField] private TextBlock 编号;
    [SerializeField] private TextBlock 年龄;
    [SerializeField] private TextBlock 评估人员;
    [SerializeField] private TextBlock 评估时间;
    
    [Header("个人信息")]
    [SerializeField] private TextBlock 身份证;

    
    [Header("家庭信息")]
    [SerializeField] private TextBlock 联系方式;

    
    [Header("背景信息")]
    [SerializeField] private TextBlock 发病日期;
    [SerializeField] private TextBlock 文化程度;
    [SerializeField] private TextBlock 血压;
    [SerializeField] private TextBlock 神志状态;
    [SerializeField] private TextBlock 家庭成员;
    [SerializeField] private TextBlock 发病前后语言状态;
    [SerializeField] private TextBlock 发病时状况;
    [SerializeField] private TextBlock 既往病史;
    
    [Header("综合信息")]
    [SerializeField] private TextBlock 综合评价;
    [SerializeField] private TextBlock 指导建议;
    
    [Header("量表评估分数")]
    [SerializeField] private TextBlock 谈话信息量总分;
    [SerializeField] private TextBlock 谈话流利性总分;
    [SerializeField] private TextBlock 系列语言总分;
    [SerializeField] private TextBlock 是否题总分;
    [SerializeField] private TextBlock 听辨认总分;
   
    [SerializeField] private TextBlock 口头指令总分;
    [SerializeField] private TextBlock 词复述总分;
    [SerializeField] private TextBlock 句复述总分;
    [SerializeField] private TextBlock 词命名总分;
    [SerializeField] private TextBlock 颜色识别总分;
    [SerializeField] private TextBlock 颜色命名总分;
    [SerializeField] private TextBlock 反应性命名总分;
    [SerializeField] private TextBlock 视读总分;
    [SerializeField] private TextBlock 阅读指令总分;
    [SerializeField] private TextBlock 字画匹配总分;
    [SerializeField] private TextBlock 执行指令总分;

    [SerializeField] private TextBlock 填空总分;
    [SerializeField] private TextBlock 运算总分;
    [SerializeField] private TextBlock 写姓名地址总分;
    [SerializeField] private TextBlock 抄写总分;
    [SerializeField] private TextBlock 系列书写总分;
    [SerializeField] private TextBlock 听写总分;
    [SerializeField] private TextBlock 看图写字总分;
    [SerializeField] private TextBlock 写病情总分;
    [SerializeField] private TextBlock 照画图总分;
    [SerializeField] private TextBlock 摆方块总分;
    [SerializeField] private TextBlock 面部动作总分;
    [SerializeField] private TextBlock 上肢动作总分;
    [SerializeField] private TextBlock 复杂动作总分;
    [SerializeField] private TextBlock 利手评定总分;
    
    [Header("操作按钮")]
    [SerializeField] private NovaSamples.UIControls.Button 保存按钮;
    [SerializeField] private NovaSamples.UIControls.Button 取消按钮;
    
    // 存储选项组性和选项组性多选
    private List<选项组性_茉> 选项组性列表 = new List<选项组性_茉>();
    private List<选项组性多选_茉> 选项组性多选列表 = new List<选项组性多选_茉>();
    
    // 存储传入的档案数据
    private 档案 当前显示档案;
    
    // 按钮委托
    public Action On保存确认;

    public Action On取消确认;
 
    
    void Awake()
    {
        // 在Awake中订阅显示信息事件
        初始化选项组();
        显示信息 += 显示档案详情;
    }
    
    void Start()
    {
        // 绑定按钮事件
        保存按钮?.OnClicked.AddListener(() => 处理保存按钮点击().Forget());
        取消按钮?.OnClicked.AddListener(() => 处理取消按钮点击().Forget());
    }
    
    /// <summary>
    /// 初始化所有子物体中的选项组性和选项组性多选
    /// </summary>
    void 初始化选项组()
    {
        选项组性列表.Clear();
        选项组性多选列表.Clear();
        
        选项组性列表.AddRange(GetComponentsInChildren<选项组性_茉>(true));
        选项组性多选列表.AddRange(GetComponentsInChildren<选项组性多选_茉>(true));
        
        Debug.Log($"[档案报告] 找到 {选项组性列表.Count} 个选项组性，{选项组性多选列表.Count} 个选项组性多选");
    }
    
    /// <summary>
    /// 显示档案详情（通过界面基础_韩的显示信息事件调用）
    /// </summary>
    void 显示档案详情(I记贞 记)
    {
        当前显示档案 = 记 as 档案;
        
        if (当前显示档案 == null)
        {
            Debug.LogWarning("[档案报告] 传入的档案数据为空");
            return;
        }
        
        Debug.Log($"[档案报告] 显示档案: {当前显示档案.姓名}(编号:{当前显示档案.编号})");
        
        // 显示基本信息
        显示基本信息(当前显示档案);
        
        // 显示个人信息
        显示个人信息(当前显示档案);
        
        // 显示家庭信息
        显示家庭信息(当前显示档案);
        
        // 显示背景信息
        显示背景信息(当前显示档案);
        
        // 显示综合信息
        显示综合信息(当前显示档案);
        
        // 显示量表评估分数
        显示量表评估分数(当前显示档案);
        
        // 还原选项组选择
        还原背景信息选项组(当前显示档案);
    }
    
    /// <summary>
    /// 显示基本信息
    /// </summary>
    void 显示基本信息(档案 档案)
    {
        if (姓名 != null) 姓名.Text = 档案.姓名 ?? "";
        if (性别 != null) 性别.Text = 档案.性别 ?? "";
        if (出生日期 != null) 出生日期.Text = 档案.出生日期 ?? "";
        if (编号 != null) 编号.Text = 档案.编号 ?? "";
        if (年龄 != null) 年龄.Text = 档案.年龄 ?? "";
        if (评估人员 != null) 评估人员.Text = 档案.评估人员 ?? "";
        if (评估时间 != null) 评估时间.Text = 档案.评估时间 ?? "";
    }
    
    /// <summary>
    /// 显示个人信息
    /// </summary>
    void 显示个人信息(档案 档案)
    {
        if (身份证 != null) 身份证.Text = 档案.身份证 ?? "";
   
    }
    
    /// <summary>
    /// 显示家庭信息
    /// </summary>
    void 显示家庭信息(档案 档案)
    {

        if (联系方式 != null) 联系方式.Text = 档案.联系方式 ?? "";
     
    }
    
    /// <summary>
    /// 显示背景信息
    /// </summary>
    void 显示背景信息(档案 档案)
    {
        if (发病日期 != null) 发病日期.Text = 档案.发病日期 ?? "";
        if (文化程度 != null) 文化程度.Text = 档案.文化程度 ?? "";
        if (血压 != null) 血压.Text = 档案.血压 ?? "";
        if (神志状态 != null) 神志状态.Text = 档案.神志状态 ?? "";
        if (家庭成员 != null) 家庭成员.Text = 档案.家庭成员 ?? "";
        if (发病前后语言状态 != null) 发病前后语言状态.Text = 档案.发病前后语言状态 ?? "";
        if (发病时状况 != null) 发病时状况.Text = 档案.发病时状况 ?? "";
        if (既往病史 != null) 既往病史.Text = 档案.既往病史 ?? "";
    }
    
    /// <summary>
    /// 显示综合信息
    /// </summary>
    void 显示综合信息(档案 档案)
    {
        if (综合评价 != null) 综合评价.Text = 档案.综合评价 ?? "";
        if (指导建议 != null) 指导建议.Text = 档案.指导建议 ?? "";
    }
    
    /// <summary>
    /// 显示量表评估分数
    /// </summary>
    void 显示量表评估分数(档案 档案)
    {
        // 谈话记录
        if (档案.谈话记录 != null)
        {
            if (谈话信息量总分 != null) 谈话信息量总分.Text = 档案.谈话记录.谈话信息量总分.ToString();
            if (谈话流利性总分 != null) 谈话流利性总分.Text = 档案.谈话记录.谈话流利性总分.ToString();
            if (系列语言总分 != null) 系列语言总分.Text = 档案.谈话记录.系列语言总分.ToString();
        }
        
        // 理解记录
        if (档案.理解记录 != null)
        {
            if (是否题总分 != null) 是否题总分.Text = 档案.理解记录.是否题总分.ToString();
            if (听辨认总分 != null) 听辨认总分.Text = 档案.理解记录.听辨认图形总分.ToString();
            if (口头指令总分 != null) 口头指令总分.Text = 档案.理解记录.口头指令总分.ToString();
        }
        
        // 复述记录
        if (档案.复述记录 != null)
        {
            if (词复述总分 != null) 词复述总分.Text = 档案.复述记录.词复述总分.ToString();
            if (句复述总分 != null) 句复述总分.Text = 档案.复述记录.句复述总分.ToString();
        }
        
        // 命名记录
        if (档案.命名记录 != null)
        {
            if (词命名总分 != null) 词命名总分.Text = 档案.命名记录.词命名总分.ToString();
            if (颜色识别总分 != null) 颜色识别总分.Text = 档案.命名记录.颜色识别总分.ToString();
            if (颜色命名总分 != null) 颜色命名总分.Text = 档案.命名记录.颜色命名总分.ToString();
            if (反应性命名总分 != null) 反应性命名总分.Text = 档案.命名记录.反应性命名总分.ToString();
        }
        
        // 阅读记录
        if (档案.阅读记录 != null)
        {
            if (视读总分 != null) 视读总分.Text = 档案.阅读记录.视读总分.ToString();
            if (阅读指令总分 != null) 阅读指令总分.Text = 档案.阅读记录.阅读指令总分.ToString();
            if (字画匹配总分 != null) 字画匹配总分.Text = (档案.阅读记录.字画匹配朗读总分+档案.阅读记录.字画匹配配画总分).ToString();
            if (执行指令总分 != null) 执行指令总分.Text = (档案.阅读记录.执行指令朗读总分+档案.阅读记录.执行指令配画总分).ToString();
            if (填空总分 != null) 填空总分.Text = 档案.阅读记录.填空总分.ToString();
        }
        
        // 计算记录
        if (档案.计算记录 != null)
        {
            if (运算总分 != null) 运算总分.Text = 档案.计算记录.运算总分.ToString();
        }
        
        // 书写记录
        if (档案.书写记录 != null)
        {
            if (写姓名地址总分 != null) 写姓名地址总分.Text = 档案.书写记录.写姓名地址总分.ToString();
            if (抄写总分 != null) 抄写总分.Text = 档案.书写记录.抄写总分.ToString();
            if (系列书写总分 != null) 系列书写总分.Text = 档案.书写记录.系列书写总分.ToString();
            if (听写总分 != null) 听写总分.Text = 档案.书写记录.听写总分.ToString();
            if (看图写字总分 != null) 看图写字总分.Text = 档案.书写记录.看图写字总分.ToString();
            if (写病情总分 != null) 写病情总分.Text = 档案.书写记录.写病情分数.ToString();
        }
        
        // 结构与空间记录
        if (档案.结构与空间记录 != null)
        {
            if (照画图总分 != null) 照画图总分.Text = 档案.结构与空间记录.照画图总分.ToString();
            if (摆方块总分 != null) 摆方块总分.Text = 档案.结构与空间记录.摆方块总分.ToString();
        }
        
        // 运用记录
        if (档案.运用记录 != null)
        {
            if (面部动作总分 != null) 面部动作总分.Text = 档案.运用记录.面部动作总分.ToString();
            if (上肢动作总分 != null) 上肢动作总分.Text = 档案.运用记录.上肢动作总分.ToString();
            if (复杂动作总分 != null) 复杂动作总分.Text = 档案.运用记录.复杂动作总分.ToString();
        }
        
        // 利手记录
        if (档案.利手记录 != null)
        {
            if (利手评定总分 != null) 利手评定总分.Text = 档案.利手记录.利手评定总分.ToString();
        }
    }
    
    /// <summary>
    /// 还原背景信息的选项组选择
    /// </summary>
    async void 还原背景信息选项组(档案 档案)
    {
        if (档案.背景信息选项组选择 == null || 档案.背景信息选项组选择.Count == 0) return;
        await UniTask.Delay(100);
        // 还原单选选项组
        foreach (var 选项组 in 选项组性列表)
        {
            if (选项组 != null && !string.IsNullOrEmpty(选项组.选项组名称) && 
                档案.背景信息选项组选择.ContainsKey(选项组.选项组名称))
            {
                var 数据 = 档案.背景信息选项组选择[选项组.选项组名称].Split('|');
                foreach (var 数据字符串 in 数据)
                {
                    Debug.Log(数据字符串);
                }
                if (数据.Length == 2 && int.TryParse(数据[1], out int 索引))
                    选项组.设置选中选项(索引);
            }
        }
        
        // 还原多选选项组
        foreach (var 选项组 in 选项组性多选列表)
        {
            if (选项组 != null && !string.IsNullOrEmpty(选项组.选项组名称) && 
                档案.背景信息选项组选择.ContainsKey(选项组.选项组名称))
            {
                var 数据 = 档案.背景信息选项组选择[选项组.选项组名称].Split('|');
                if (数据.Length == 2)
                {
                    var 索引列表 = new List<int>();
                    foreach (var 索引字符串 in 数据[1].Split(','))
                    {
                        if (int.TryParse(索引字符串, out int 索引))
                            索引列表.Add(索引);
                    }
                    选项组.批量设置选中选项(索引列表);
                }
            }
        }
    }
    
    protected override void OnShow()
    {
        base.OnShow();
        Debug.Log("[档案报告] 界面显示");
    }
    
    protected override void OnHide()
    {
        base.OnHide();
        Debug.Log("[档案报告] 界面隐藏");
        
        // 清空当前显示的档案
        当前显示档案 = null;
    }
    
    /// <summary>
    /// 处理保存按钮点击
    /// </summary>
    async UniTaskVoid 处理保存按钮点击()
    {
        var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 界面管理器 = FindObjectOfType<界面管理器_韩>();
        var ins = Instantiate(告示, 界面管理器.transform);
        var 用户选择 = await ins.GetComponent<告示_茉>().等待确认显示("保存确认", "确定要保存当前报告吗？");
        
        if (用户选择)
        {
            Debug.Log("[档案报告] 用户确认保存");
            界面管理器.显示界面("档案管理显示");
            On保存确认?.Invoke();
        }
        else
        {
            Debug.Log("[档案报告] 用户取消保存");
        }
        
        Destroy(ins);
    }
    
    /// <summary>
    /// 处理取消按钮点击
    /// </summary>
    async UniTaskVoid 处理取消按钮点击()
    {
        var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 界面管理器 = FindObjectOfType<界面管理器_韩>();
        var ins = Instantiate(告示, 界面管理器.transform);
        var 用户选择 = await ins.GetComponent<告示_茉>().等待确认显示("取消确认", "确定要取消并退出吗？");
        
        if (用户选择)
        {
            Debug.Log("[档案报告] 用户确认取消");
            On取消确认?.Invoke();
        }
        else
        {
            Debug.Log("[档案报告] 用户取消取消操作");
           
        }
        
        Destroy(ins);
    }
    
    void OnDestroy()
    {
        // 清理按钮事件
        保存按钮?.OnClicked.RemoveAllListeners();
        取消按钮?.OnClicked.RemoveAllListeners();
        
        // 取消订阅显示信息事件
        显示信息 -= 显示档案详情;
    }
}
