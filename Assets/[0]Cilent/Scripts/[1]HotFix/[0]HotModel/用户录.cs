using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class 用户 : I记贞
{
    public string 名;
    public string 族;
    public string 身份证;
    public string 账号;
    public string 密码;
    public string 电话;
    public string 性别;
    public string 录编;
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
            if (dataList[i].录编.Contains(keyword))
            {
                indexList.Add(i);
            }
        }
        return indexList;
    }

}
