using System;
using System.Collections.Generic;

/// <summary>
/// 待同步的数据条目
/// </summary>
[Serializable]
public class 待同步数据 : I记贞
{
    /// <summary>
    /// 来源的模型名称，例如 "档案录" / "用户录"
    /// </summary>
    public string 原数据录名称;

    /// <summary>
    /// 在来源模型中的索引
    /// </summary>
    public int 待同步索引;

    /// <summary>
    /// 需要在 UI 中显示的字段（保持与 待同步数据条目显示_茉 对齐）
    /// </summary>
    public string 编号;
    public string 姓名;
    public string 性别;
    public string 添加时间;
    public string 类型;
}

/// <summary>
/// 待同步数据录：用于存放待同步的数据条目列表
/// </summary>
public class 待同步数据录 : 录元<待同步数据>
{
    /// <summary>
    /// 允许按姓名模糊搜索
    /// </summary>
    protected override List<int> OnSearchByName(string keyword)
    {
        List<int> indexList = new List<int>();
        if (string.IsNullOrEmpty(keyword))
        {
            return indexList;
        }

        for (int i = 0; i < dataList.Count; i++)
        {
            if (!string.IsNullOrEmpty(dataList[i].姓名) && dataList[i].姓名.Contains(keyword))
            {
                indexList.Add(i);
            }
        }

        return indexList;
    }

    protected override string GetStorageKey()
    {
        return "待同步数据录";
    }
}

