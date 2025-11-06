using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Nova;

public class 打印报告界面_茉 : 界面基础_韩
{
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
  
     [Header("评估结果")]
     [SerializeField] private TextBlock 利手评定结果;
     [SerializeField] private TextBlock 失语症评定;
     [SerializeField] private TextBlock BDAE严重程度;
    
    private 档案 当前显示档案;
    public bool 是否为打印;
    void Awake()
    {
        显示信息 += 显示档案详情;
    }
    
    void 显示档案详情(I记贞 记)
    {
        当前显示档案 = 记 as 档案;
        
        if (当前显示档案 == null)
        {
            Debug.LogWarning("[打印报告] 传入的档案数据为空");
            return;
        }
        
        Debug.Log($"[打印报告] 显示档案: {当前显示档案.姓名}(编号:{当前显示档案.编号})");
        
        显示量表评估分数(当前显示档案);
    }
    
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
            if (利手评定结果 != null) 利手评定结果.Text = 档案.利手记录.利手评定结果 ?? "";
        }
        
        // 失语症评定
        显示失语症评定结果(档案);
        
        // BDAE严重程度
        显示BDAE严重程度(档案);
    }
    
    void 显示失语症评定结果(档案 档案)
    {
        if (失语症评定 == null) return;
        
        string 失语症类型 = 失语症评定工具.计算失语症类型(档案);
        string 完整名称 = 失语症评定工具.获取失语症类型全称(失语症类型);
        失语症评定.Text = 完整名称;
        
        Debug.Log($"[打印报告] 失语症评定结果: {完整名称}");
    }
    
    void 显示BDAE严重程度(档案 档案)
    {
        if (BDAE严重程度 == null) return;
        
        int 等级 = 档案.BDAE严重程度等级;
        string 描述 = BDAE严重程度评定工具.获取严重程度描述(等级);
        BDAE严重程度.Text = 描述;
        
        Debug.Log($"[打印报告] BDAE严重程度: {等级}级 - {描述}");
    }
    
    void OnDestroy()
    {
        显示信息 -= 显示档案详情;
    }
}
