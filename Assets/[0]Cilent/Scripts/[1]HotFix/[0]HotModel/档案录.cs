using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
public class 谈话记录
{
    public string 谈话时间;
    public string 谈话人员;
    public string 谈话内容;
}

public class 理解记录
{

}
public class 复述记录
{

}

public class 命名记录
{

}

public class 阅读记录
{

}

public class 计算记录
{

}

public class 书写记录
{

}
public class 结构与空间记录
{

}
public class 视觉记录
{

}
public class 运用记录
{

}
public class 利手记录
{

}
public enum 评估状态
{
    未评估,
    评估中,
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
}



public class 档案录 : 录元<档案>
{
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
