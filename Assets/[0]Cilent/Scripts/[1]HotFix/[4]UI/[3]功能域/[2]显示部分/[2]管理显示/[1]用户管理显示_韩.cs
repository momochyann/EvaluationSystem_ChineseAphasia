using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
public class 用户管理显示_韩 : 界面基础_韩
{
    // Start is called before the first frame update
    [SerializeField] public NovaSamples.UIControls.Button 添加用户;
    void Start()
    {
        if (添加用户 != null)
        添加用户.OnClicked.AddListener(添加新数据);
    }

    void 添加新数据()
    {
        Debug.Log("[ [1] 用户管理显示_韩] 添加新数据");
        FindObjectOfType<界面管理器_韩>().显示界面("用户管理添加");
    }
    protected override void OnShow()
    {
        Debug.Log("[ [1] 用户管理显示_韩]OnShow");
    }
    protected override void OnHide()
    {
        Debug.Log("[ [1] 用户管理显示_韩]OnHide");
    }
    void OnDestroy()
    {
        添加用户.OnClicked.RemoveAllListeners();
    }
}
