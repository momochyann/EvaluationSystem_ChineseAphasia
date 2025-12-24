using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class 用户 : I记贞
{
    public string 编号;
    public string 名;
    public string 性别;
    public string 身份证;
    public string 诞辰;
    public string 民族;
    public string 监护人;
    public string 关系;
    public string 联系方式;
    public bool 是否为本地户籍;
    public string 居住地址;
    
    // 背景信息维度
    public string 发病日期;
    public string 文化程度;
    public string 血压;
    public string 神志状态;
    public string 家庭成员;
    public string 发病前后语言状态;
    public string 发病时状况;
    public string 既往病史;
    public string 添加时间;
    // 用于存储选项组性和选项组性多选的选择结果（格式："内容|索引"）
    public Dictionary<string, string> 背景信息选项组选择 = new Dictionary<string, string>();
}
public class 用户录 : 录元<用户>
{
    protected override string GetStorageKey()
    {
        return "用户录";
    }
    protected override void OnInit()
    {
        base.OnInit();
    }
    protected override List<int> OnSearchByName(string keyword)
    {
        List<int> indexList = new List<int>();
        for (int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i].编号.Contains(keyword))
            {
                indexList.Add(i);
            }
        }
        return indexList;
    }

}
