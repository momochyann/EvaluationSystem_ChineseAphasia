using System.Collections;
using System.Collections.Generic;
using NovaSamples.UIControls;
using QFramework;
using UnityEngine;

public class 档案管理显示 : 界面基础_韩
{
 //   [SerializeField] Button 评测按钮;
  //  [SerializeField] Button 新增按钮;
    void Start()
    {
    //    评测按钮.OnClicked.AddListener(评测);
       // 新增按钮.OnClicked.AddListener(新增);
    }

    protected override void OnShow()
    {
        base.OnShow();
        // 获取当前档案
        var 档案录 = this.GetModel<档案录>();
        档案录.当前档案 = null;
    }
    void 新增()
    {
        FindObjectOfType<界面管理器_韩>().显示界面("量表待评测添加");
    }

    protected override void OnHide()
    {
        //Debug.Log("量表待评测显示_茉 OnHide");
    }
}
