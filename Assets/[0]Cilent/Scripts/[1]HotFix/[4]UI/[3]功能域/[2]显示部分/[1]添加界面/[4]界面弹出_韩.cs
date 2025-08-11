using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using NovaSamples.UIControls;
using Nova;
public class 界面弹出_韩 : MonoBehaviour,IController
{
    // Start is called before the first frame update
    public Toggle 评估量表基本信息;
    public Toggle 训练指导方案;
    public Button 确定按钮;
    public Button 取消按钮;
    void Start()
    {
        确定按钮.OnClicked.AddListener(确定);
        取消按钮.OnClicked.AddListener(取消);
    }

    // Update is called once per frame
    void 确定()
    {
        Debug.Log("确定按钮被点击");
        if (评估量表基本信息.ToggledOn)
        {
            Debug.Log("打印评估量表基本信息");
        }
        if (训练指导方案.ToggledOn)
        {
            Debug.Log("打印训练指导方案");
        }
    }
    void 取消()
    {
        Debug.Log("取消按钮被点击");
        
        var color = this.transform.parent.gameObject.GetComponent<UIBlock2D>().Color;
        this.transform.parent.gameObject.GetComponent<UIBlock2D>().Color = new Color(color.r, color.g, color.b, 1f);
        Destroy(this.gameObject);
    }
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
    void OnDestroy()
    {
        确定按钮.OnClicked.RemoveAllListeners();
        取消按钮.OnClicked.RemoveAllListeners();
    }
}
