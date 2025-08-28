using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using QFramework;
using NovaSamples.UIControls;
using Nova;

public class 加载动画_韩 : MonoBehaviour, IController
{
    // Start is called before the first frame update
    [SerializeField] public TextBlock 加载动画文本;
    [SerializeField] public NovaSamples.UIControls.Button 取消按钮;
    private float totalLoadingTime = 1f; // 总加载时间（3秒）
    private float elapsedTime = 0f; // 已过去的时间
    public 加载动画状态 加载动画状态 = 加载动画状态.加载中;
    void Start()
    {
        加载动画文本.Text = "正在加载中......";

        取消按钮.OnClicked.AddListener(取消);
    }
    void Update()
    {
        if (elapsedTime < totalLoadingTime)
        {
            elapsedTime += Time.deltaTime;
        }else 
        {
            加载动画状态 = 加载动画状态.完成;
        }

        // 计算进度百分比（0-100）
        float progress = Mathf.Min(100, (elapsedTime / totalLoadingTime) * 100);
        int progressPercent = Mathf.RoundToInt(progress);

        // 计算省略号数量（1-6个，随时间循环变化）
        // 每0.2秒切换一次点数（3秒内可循环多次）
        float dotCycle = 1.2f; // 1-6个点的总周期（每个点显示0.2秒）
        int currentDots = Mathf.Clamp((int)(elapsedTime/dotCycle *6 %6),1,6);
        string dots = "";
        for (int i = 0; i < 6; i++)
        {
            dots += i < currentDots ? ".":" ";
            Debug.Log(dots);
        }

        // 更新文本内容
        加载动画文本.Text = $"正在加载中{dots} {progressPercent}%";
    }
    public void 取消()
    {
        Debug.Log("取消");
        加载动画状态 = 加载动画状态.取消;
        Destroy(this.gameObject);
    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}

public enum 加载动画状态
{
    加载中 = 0,
    取消 = 1,
    完成 = 2
}