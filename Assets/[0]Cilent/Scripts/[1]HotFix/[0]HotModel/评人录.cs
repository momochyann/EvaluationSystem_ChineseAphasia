using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class 评人 : I记贞
{
    public string 名;
    public string 诞辰;
    public string 账号;
    public string 密码;
    public string 电话;
    public string 身份证;
    public string 性别;

}
public class 评人录 : 录元<评人>
{
    public 评人 登陆评人;
    protected override string GetStorageKey()
    {
        return "评人录";
    }
    protected override void OnInit()
    {
        base.OnInit();
    }
    /// <summary>
    /// 根据身份证搜索
    /// </summary>
    /// <param name="keyword">身份证</param>
    /// <returns>搜索到的项的索引列表</returns>
    protected override List<int> OnSearchByName(string keyword)
    {
        List<int> indexList = new List<int>();
        for (int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i].身份证.Contains(keyword))
            {
                indexList.Add(i);
            }
        }
        return indexList;
    }
}

public class 管理员 : I记贞
{
    public string 名;
    public string 账号;
    public string 密码;

}

public class 管员录 : 录元<评人>
{
    public 评人 登陆管理员;
    protected override void OnInit()
    {
        base.OnInit();
        // 检查是否需要添加默认管理员账号
        CreateDefaultAdminIfNeeded();
    }
    
    /// <summary>
    /// 如果管理员列表为空，创建默认的admin账号
    /// </summary>
    private void CreateDefaultAdminIfNeeded()
    {
        // 检查是否已存在admin账号
        bool hasAdmin = dataList.Exists(admin => admin.账号 == "admin");
        
        if (!hasAdmin)
        {
            // 创建默认管理员账号
            评人 defaultAdmin = new 评人
            {
                名 = "初始管理员",
                账号 = "admin",
                密码 = "123456"
            };
            
            // 添加到列表（不插入到开头，因为这是系统默认的）
            AddItem(defaultAdmin, false);
            Debug.Log("[管员录] 已创建默认管理员账号：admin/123456");
        }
        else
        {
            Debug.Log("[管员录] 默认管理员账号已存在");
        }
    }
    
    protected override string GetStorageKey()
    {
        return "管理员录";
    }
}
