using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using DG.Tweening;
using Nova;
using NovaSamples.UIControls;
using Cysharp.Threading.Tasks;

public class 告示_茉 : MonoBehaviour, IController
{
    // Start is called before the first frame update

    [SerializeField] Transform 栏;
    [SerializeField] TextBlock 标题框;
    [SerializeField] TextBlock 内容框;
    [SerializeField] Button 确定;
    [SerializeField] Button 取消;
    private UniTaskCompletionSource<bool> 等待完成;

    private void Awake()
    {
        栏.DOScale(1, 0.2f).From(0);
    }
    private void Start()
    {
        确定.OnClicked.AddListener(() => 等待完成.TrySetResult(true));
        取消.OnClicked.AddListener(() => 等待完成.TrySetResult(false));
    }
    public  async UniTask<bool> 等待确认显示(string 标题, string 内容)
    {
         if (确定 != null) 确定.gameObject.SetActive(true);
        if (取消 != null) 取消.gameObject.SetActive(true);
        标题框.Text = 标题;
        内容框.Text = 内容;
        等待完成 = new UniTaskCompletionSource<bool>();
        bool 户择 = await 等待完成.Task;
        return 户择;
    }
    
    /// <summary>
    /// 更新内容（用于动态显示进度）
    /// </summary>
    public void 更新内容(string 内容)
    {
        if (内容框 != null)
        {
            内容框.Text = 内容;
        }
    }
    
    /// <summary>
    /// 显示进度信息（不等待确认，用于显示同步进度）
    /// </summary>
    public void 显示进度(string 标题, string 内容)
    {
        if (标题框 != null) 标题框.Text = 标题;
        if (内容框 != null) 内容框.Text = 内容;
        // 隐藏确认和取消按钮（如果存在）
        if (确定 != null) 确定.gameObject.SetActive(false);
        if (取消 != null) 取消.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 关闭告示（用于进度显示完成后）
    /// </summary>
    public void 关闭()
    {
        if (栏 != null)
        {
            栏.DOScale(0, 0.2f).OnComplete(() => Destroy(gameObject));
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void 退()
    {

    }
    private void OnDestroy()
    {
        // 清理事件监听
        确定?.OnClicked.RemoveAllListeners();
        取消?.OnClicked.RemoveAllListeners();

        // 取消未完成的等待
        等待完成?.TrySetCanceled();
    }
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
