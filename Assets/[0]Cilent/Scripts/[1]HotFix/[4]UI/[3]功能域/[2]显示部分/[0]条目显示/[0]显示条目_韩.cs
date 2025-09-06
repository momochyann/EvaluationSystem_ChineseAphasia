using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using NovaSamples.UIControls;
using Nova;
using DG.Tweening;
using UnityEngine.UI;


public class 显示条目_韩 : MonoBehaviour, IController
{
    [Header("基础配置")]
    [SerializeField] public GameObject prefeb;
    [SerializeField] public string ModelName; // 设置为与模型类名一致
    [SerializeField] public bool 是否刷新 = false;
    [SerializeField] Scroller 滚动条;

    void Start()
    {
        if (prefeb == null)
        {
            Debug.Log("[显示条目_韩] prefeb为空");
            return;
        }
        if (ModelName == null)
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

        // 重新获取数据并显示
        var 录 = this.GetUtility<按名索录之器>().寻录<I录贞>(ModelName);
        if (录 == null)
        {
            Debug.LogError($"[显示条目_韩] 无法找到模型: {ModelName}");
            return;
        }

        var 数据 = 录.GetAllItems();
        var 索引下标 = 数据.Count;
        for (int i = 0; i < 索引下标; i++)
        {
            Debug.Log(this.transform.parent.gameObject.name);
            var 条目 = Instantiate(prefeb, this.transform);
            var mask = 条目.GetComponent<ClipMask>();
            DOTween.To((float value) =>
           {
               mask.Tint = new Color(mask.Tint.r, mask.Tint.g, mask.Tint.b, value);
           }, 0, 1, 0.5f).SetDelay(0.05f*i);

            // 现在可以使用接口了，因为组件实现了接口
            var 显示条目 = 条目.GetComponent<IentryDis>();
            if (显示条目 != null)
            {
                显示条目.显示信息(i);
                //Debug.Log("[显示条目_韩] 条目显示" + 条目.GetComponent<用户显示条目_韩>().姓名.Text);
            }
            else
            {
                Debug.LogWarning($"[显示条目_韩] 条目对象 {条目.name} 没有实现 显示条目接口");
            }
        }

        // 控制滚动条状态
        控制滚动条状态(索引下标);
    }

    /// <summary>
    /// 控制滚动条状态：>7启用，<=7禁用
    /// </summary>
    /// <param name="条目数量">当前条目数量</param>
    private void 控制滚动条状态(int 条目数量)
    {
        if (滚动条 == null) return;

        bool 应该启用滚动 = 条目数量 > 7;

        if (应该启用滚动)
        {
            滚动条.enabled = true;
            Debug.Log($"[显示条目_韩] 条目数量({条目数量}) > 7，已启用滚动");
        }
        else
        {
            滚动条.enabled = false;
            Debug.Log($"[显示条目_韩] 条目数量({条目数量}) <= 7，已禁用滚动");
        }
    }

    public void 清除现有显示()
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

        // 恢复滚动条状态
        if (滚动条 != null)
        {
            滚动条.enabled = true;
        }
    }
}

// public interface I显示条目接口
// {
//     void 显示信息(int 索引 = -1);
// }
public interface IentryDis
{
    void 显示信息(int 索引 = -1);
    int 索引下标 { get; set; }
}