using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEngine.Events;
public class 界面基础_韩 : MonoBehaviour,IController
{
    // Start is called before the first frame update
    public string 界面名称 { get; }
    public bool 是否显示 { get; private set; }
    public UnityAction<I记贞> 显示信息;
    public void 显示界面()
    {
        this.gameObject.SetActive(true);
        this.是否显示 = true;
        OnShow();
    }
    public void 隐藏界面()
    {
        this.gameObject.SetActive(false);
        this.是否显示 = false;
        Debug.Log("[ [0] 界面基础_韩]隐藏界面");
        OnHide();
    }

    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
