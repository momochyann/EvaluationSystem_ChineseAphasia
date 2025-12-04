using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

/// <summary>
/// 删除档案录 - 存储已删除的档案
/// </summary>
public class 删除档案录 : 录元<档案>
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
        return "删除档案录";
    }
}

