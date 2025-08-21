using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class 评定人管理显示_韩 : 界面基础_韩
{
    // Start is called before the first frame update
    [SerializeField] public NovaSamples.UIControls.Button 添加评定人;

    void Start()
    {
        添加评定人.OnClicked.AddListener(添加新数据);
        
    }

    void 添加新数据()
    {
        Debug.Log("[ [0] 评定人管理显示_韩] 添加新数据");
        FindObjectOfType<界面管理器_韩>().显示界面("评定人管理添加");

    }
    protected override void OnShow()
    {
        Debug.Log("[ [0] 评定人管理显示_韩]OnShow");
    }
    protected override void OnHide()
    {
        Debug.Log("[ [0] 评定人管理显示_韩]OnHide");
    }
    void OnDestroy()
    {
        添加评定人.OnClicked.RemoveAllListeners();
    }
}
