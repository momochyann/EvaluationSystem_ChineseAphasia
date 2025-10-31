using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

/// <summary>
/// 评估模板类型枚举
/// </summary>
public enum 评估模板类型
{
    综合评价,
    指导建议
}

/// <summary>
/// 评估模板数据类
/// </summary>
public class 评估模板 : I记贞
{
    public string 模板名称;
    public string 模板内容;
    public 评估模板类型 模板类型;
    public string 创建时间;
    public string 最后修改时间;
}

/// <summary>
/// 综合评价模板录
/// </summary>
public class 综合评价模板录 : 录元<评估模板>
{
    protected override string GetStorageKey()
    {
        return "综合评价模板录";
    }

    protected override void OnInit()
    {
        base.OnInit();
        
        // 如果是第一次初始化，添加默认模板
        if (dataList.Count == 0)
        {
            添加默认模板();
        }
    }

    protected override List<int> OnSearchByName(string keyword)
    {
        List<int> indexList = new List<int>();
        for (int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i].模板名称.Contains(keyword))
            {
                indexList.Add(i);
            }
        }
        return indexList;
    }

    /// <summary>
    /// 添加默认模板
    /// </summary>
    private void 添加默认模板()
    {
        var 默认模板 = new 评估模板
        {
            模板名称 = "默认综合评价模板",
            模板内容 = "患者表现良好，各项评估指标正常。",
            模板类型 = 评估模板类型.综合评价,
            创建时间 = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            最后修改时间 = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };
        
        dataList.Add(默认模板);
        SaveData();
    }
}

/// <summary>
/// 指导建议模板录
/// </summary>
public class 指导建议模板录 : 录元<评估模板>
{
    protected override string GetStorageKey()
    {
        return "指导建议模板录";
    }

    protected override void OnInit()
    {
        base.OnInit();
        
        // 如果是第一次初始化，添加默认模板
        if (dataList.Count == 0)
        {
            添加默认模板();
        }
    }

    protected override List<int> OnSearchByName(string keyword)
    {
        List<int> indexList = new List<int>();
        for (int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i].模板名称.Contains(keyword))
            {
                indexList.Add(i);
            }
        }
        return indexList;
    }

    /// <summary>
    /// 添加默认模板
    /// </summary>
    private void 添加默认模板()
    {
        var 默认模板 = new 评估模板
        {
            模板名称 = "默认指导建议模板",
            模板内容 = "建议患者继续保持良好的康复训练习惯，定期复查。",
            模板类型 = 评估模板类型.指导建议,
            创建时间 = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            最后修改时间 = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };
        
        dataList.Add(默认模板);
        SaveData();
    }
}

