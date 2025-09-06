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
