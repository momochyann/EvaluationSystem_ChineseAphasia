using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;


public interface 档案记录接口
{
    public string 评估时间{get;set;}
    public 评估状态 是否完成{get;set;}
}
/// <summary>
/// 谈话记录 - 包含3个维度的评估数据
/// </summary>
public class 谈话记录 : 档案记录接口
{
    public string 谈话时间;
    public string 谈话人员;
    public string 谈话内容;
    public string 评估时间 { get; set; }
    public 评估状态 是否完成 { get; set; }
    
    // 三个维度的分数
    public int 谈话信息量总分;
    public int 谈话流利性总分;
    public int 系列语言总分;
    
    // 存储选项组名称和选中索引的映射
    public Dictionary<string, int> 选项组选择 = new Dictionary<string, int>();
}


/// <summary>
/// 理解记录 - 包含4个维度的评估数据
/// </summary>
public class 理解记录 : 档案记录接口
{
    public string 评估时间 { get; set; }
    public 评估状态 是否完成 { get; set; }
    
    // 四个维度的分数
    public int 是否题总分;
    public int 听辨认实物总分;
    public int 听辨认图形总分;
    public int 口头指令总分;
    
    // 存储选项组名称和选中索引的映射
    public Dictionary<string, int> 选项组选择 = new Dictionary<string, int>();
}
public class 复述记录 : 档案记录接口
{
    public int 词复述总分;
    public int 句复述总分;
    public string 评估时间 { get; set; }
    public 评估状态 是否完成 { get; set; }
    
    // 存储选项组名称和选中索引的映射
    public Dictionary<string, int> 选项组选择 = new Dictionary<string, int>();
    // 存储表分择性名称和选中索引的映射
    public Dictionary<string, int> 表分择性选择 = new Dictionary<string, int>();
}

public class 命名记录 : 档案记录接口
{
    public string 评估时间 { get; set; }
    public 评估状态 是否完成 { get; set; }
    
    // 五个维度的分数
    public int 词命名总分;
    public string 列名; // 输入方式
    public int 颜色识别总分;
    public int 颜色命名总分;
    public int 反应性命名总分;
    
    // 存储选项组名称和选中索引的映射
    public Dictionary<string, int> 选项组选择 = new Dictionary<string, int>();
}

/// <summary>
/// 阅读记录 - 包含7个维度的评估数据
/// </summary>
public class 阅读记录 : 档案记录接口
{
    public string 评估时间 { get; set; }
    public 评估状态 是否完成 { get; set; }
    
    // 七个维度的分数
    public int 视读总分;
    public int 阅读指令总分;
    public int 字画匹配朗读总分;
    public int 字画匹配配画总分;
    public int 执行指令朗读总分;
    public int 执行指令配画总分;
    public int 填空总分;
    
    // 存储选项组名称和选中索引的映射
    public Dictionary<string, int> 选项组选择 = new Dictionary<string, int>();
}

/// <summary>
/// 计算记录 - 包含1个维度的评估数据
/// </summary>
public class 计算记录 : 档案记录接口
{
    public string 评估时间 { get; set; }
    public 评估状态 是否完成 { get; set; }
    
    // 一个维度的分数
    public int 运算总分;
    
    // 存储选项组名称和选中索引的映射
    public Dictionary<string, int> 选项组选择 = new Dictionary<string, int>();
}

/// <summary>
/// 书写记录 - 包含6个维度的评估数据
/// </summary>
public class 书写记录 : 档案记录接口
{
    public string 评估时间 { get; set; }
    public 评估状态 是否完成 { get; set; }
    
    // 六个维度的分数
    public int 写姓名地址总分;
    public int 抄写总分;
    public int 系列书写总分;
    public int 听写总分;
    public int 看图写字总分;
    public int 写病情分数; // 直接输入0-5分
    
    // 存储选项组名称和选中索引的映射（表分择性使用）
    public Dictionary<string, int> 选项组选择 = new Dictionary<string, int>();
}
/// <summary>
/// 结构与空间记录 - 包含2个维度的评估数据
/// </summary>
public class 结构与空间记录 : 档案记录接口
{
    public string 评估时间 { get; set; }
    public 评估状态 是否完成 { get; set; }
    
    // 两个维度的分数
    public int 照画图总分;
    public int 摆方块总分;
    
    // 存储选项组名称和选中索引的映射
    public Dictionary<string, int> 选项组选择 = new Dictionary<string, int>();
}
public class 视觉记录 : 档案记录接口
{
    public string 评估时间 { get; set; }
    public 评估状态 是否完成 { get; set; }
}
/// <summary>
/// 运用记录 - 包含3个维度的评估数据
/// </summary>
public class 运用记录 : 档案记录接口
{
    public string 评估时间 { get; set; }
    public 评估状态 是否完成 { get; set; }
    
    // 三个维度的分数
    public int 面部动作总分;
    public int 上肢动作总分;
    public int 复杂动作总分;
    
    // 存储选项组名称和选中索引的映射
    public Dictionary<string, int> 选项组选择 = new Dictionary<string, int>();
}
/// <summary>
/// 利手记录 - 包含1个维度的评估数据
/// </summary>
public class 利手记录 : 档案记录接口
{
    public string 评估时间 { get; set; }
    public 评估状态 是否完成 { get; set; }
    
    // 一个维度的分数
    public int 利手评定总分;
    
    // 存储选项组名称和选中索引的映射
    public Dictionary<string, int> 选项组选择 = new Dictionary<string, int>();
}
public enum 评估状态
{
    未评估,
    未完成,
    评估完成
}
public class 档案 : I记贞
{
    public 评估状态 评估状态;
    public string 姓名;
    public string 性别;
    public string 出生日期;
    public string 编号;
    public string 添加时间;
    public string 年龄;
    public string 评估人员;
    public string 评估时间;
    public 谈话记录 谈话记录;
    public 理解记录 理解记录;
    public 复述记录 复述记录;
    public 命名记录 命名记录;
    public 阅读记录 阅读记录;
    public 计算记录 计算记录;
    public 书写记录 书写记录;
    public 结构与空间记录 结构与空间记录;
    public 视觉记录 视觉记录;
    public 运用记录 运用记录;
    public 利手记录 利手记录;
    
    // 背景信息（从用户录复制过来）
    public string 身份证;
    public string 民族;
    public string 监护人;
    public string 关系;
    public string 联系方式;
    public string 居住地址;
    public string 发病日期;
    public string 文化程度;
    public string 血压;
    public string 神志状态;
    public string 家庭成员;
    public string 发病前后语言状态;
    public string 发病时状况;
    public string 既往病史;
    // 格式："内容|索引"或"内容1,内容2|索引1,索引2"（多选）
    public Dictionary<string, string> 背景信息选项组选择 = new Dictionary<string, string>();
    
    // 综合评价和指导建议
    public string 综合评价 = "";
    public string 指导建议 = "";
}



public class 档案录 : 录元<档案>
{
    public 档案 当前档案;
    protected override void OnInit()
    {
        base.OnInit();
    }
    /// <summary>
    /// 根据姓名搜索
    /// </summary>
    /// <param name="keyword">姓名</param>
    /// <returns>搜索到的项的索引列表</returns>
    protected override List<int> OnSearchByName(string keyword)
    {
        List<int> indexList = new List<int>();
        for (int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i].姓名.Contains(keyword))
            {
                indexList.Add(i);
            }
        }
        return indexList;
    }
    protected override string GetStorageKey()
    {
        return "档案录";
    }
}
