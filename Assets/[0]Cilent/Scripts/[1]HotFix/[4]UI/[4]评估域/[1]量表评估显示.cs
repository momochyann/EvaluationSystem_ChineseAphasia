using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;
public class 量表评估显示 : 界面基础_韩
{
    // Start is called before the first frame update
    [SerializeField] List<Button> 评估按钮;
    void Start()
    {
        foreach (var 评估按钮 in 评估按钮)
        {
            评估按钮.OnClicked.AddListener(() =>
            {
                FindObjectOfType<界面管理器_韩>().显示界面(评估按钮.gameObject.name);
            });         
        }
    }
}
