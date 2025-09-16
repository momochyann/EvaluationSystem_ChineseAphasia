using System.Collections;
using System.Collections.Generic;
using NovaSamples.UIControls;
using UnityEngine;

public class PC首页大数据页面_茉 : 界面基础_韩
{
    // Start is called before the first frame update
    [SerializeField] Button 返回按钮;
    void Start()
    {
        返回按钮.OnClicked.AddListener(() =>
        {
             FindObjectOfType<界面管理器_韩>().显示界面("评定人管理添加");
        });
    }

  
}
