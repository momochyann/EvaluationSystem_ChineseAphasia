using System.Collections;
using System.Collections.Generic;
using NovaSamples.UIControls;
using UnityEngine;

public class 量表待评测显示_茉 : 界面基础_韩
{
    // Start is called before the first frame update
    [SerializeField] Button 评测按钮;
    void Start()
    {
        评测按钮.OnClicked.AddListener(评测);
    }
    void 评测()
    {
        
    }
    protected override void OnShow()
    {
        //Debug.Log("量表待评测显示_茉 OnShow");
    }
    protected override void OnHide()
    {
        //Debug.Log("量表待评测显示_茉 OnHide");
    }
}
