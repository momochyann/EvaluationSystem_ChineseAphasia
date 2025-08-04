using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using NovaSamples.UIControls;
using Nova;

// public interface 显示条目接口
// {
//     void 显示信息(int 索引 = -1);
// }

public class 显示条目_韩 : MonoBehaviour,IController
{
    [Header("基础配置")]
    [SerializeField] public GameObject prefeb;
    [SerializeField] public string ModelName; // 设置为与模型类名一致
    [SerializeField] public bool 是否刷新 = false;
    void Start()
    {
        if(prefeb == null)
        {
            Debug.Log("[显示条目_韩] prefeb为空");
            return;
        }
        if(ModelName == null)
        {
            Debug.Log("[显示条目_韩] ModelName为空");
            return;
        }

        // 注册模型变化事件监听
        this.RegisterEvent<Can2ListModelChangeEvent>(On模型数据变化);
        刷新显示();
        Debug.Log("[显示条目_韩] 刷新显示transform.childCount:" + transform.childCount);
        // 初始显示
    }

    // 监听模型数据变化事件
    void On模型数据变化(Can2ListModelChangeEvent 模型变化事件)
    {
        // 检查是否是我们要监听的模型
        if (模型变化事件.ModelTypeName == ModelName)
        {
            Debug.Log($"[显示条目_韩] 检测到 {ModelName} 数据变化，刷新显示");
            刷新显示();
        }
    }

    // 刷新显示方法
    public void 刷新显示()
    {
        清除现有显示();
        // 获取数据
        //var 数据列表 = 获取数据列表();
        // if (数据列表 == null)
        // {
        //     Debug.LogWarning($"[显示条目_韩] 无法获取 {ModelName} 的数据列表");
        //     return;
        // }

        // 重新获取数据并显示
        var 录 = this.GetUtility<按名索录之器>().寻录<I录贞>(ModelName);
        if (录 == null)
        {
            Debug.LogError($"[显示条目_韩] 无法找到模型: {ModelName}");
            return;
        }
        
        var 数据 = 录.GetAllItems();
        var 索引下标 = 数据.Count;
        for(int i = 0; i < 索引下标; i++)
        {
            var 条目 = Instantiate(prefeb, this.transform);
            
            // 现在可以使用接口了，因为组件实现了接口
            var 显示条目 = 条目.GetComponent</*显示条目接口*//*评定人显示条目_韩*/用户显示条目_韩>();
            if (显示条目 != null)
            {
                显示条目.显示信息(i);
                Debug.Log("[显示条目_韩] 条目显示" + 条目.GetComponent<用户显示条目_韩>().姓名.Text);
            }
            else
            {
                Debug.LogWarning($"[显示条目_韩] 条目对象 {条目.name} 没有实现 显示条目接口");
            }
        }

    }
    void 清除现有显示()
    {
        Debug.Log("[显示条目_韩] 清除现有显示transform.childCount:" + transform.childCount);
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }
    }
        
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
    void OnDestroy()
    {
        this.UnRegisterEvent<Can2ListModelChangeEvent>(On模型数据变化);
    }
}
