using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 档案管理显示_韩 : 界面基础_韩
{
    // Start is called before the first frame update
    [SerializeField] public NovaSamples.UIControls.Button 打印;
    [SerializeField] public NovaSamples.UIControls.Button 添加;
    
    void Start()
    {
        添加.OnClicked.AddListener(添加新数据);
        打印.OnClicked.AddListener(打印文件);
    }
    void 添加新数据()
    {
        Debug.Log("[ [2] 档案管理显示_韩] 添加新数据");
        FindObjectOfType<界面管理器_韩>().显示界面("档案管理添加_PC");
    }
    void 打印文件()
    {
        Debug.Log("[ [2] 档案管理显示_韩] 打印文件");
    }
    protected override void OnShow()
    {
        Debug.Log("[ [2] 档案管理显示_韩]OnShow");
    }
    protected override void OnHide()
    {
        Debug.Log("[ [2] 档案管理显示_韩]OnHide");
    }
    void OnDestroy()
    {
        添加.OnClicked.RemoveAllListeners();
        打印.OnClicked.RemoveAllListeners();
    }
}
