using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Cysharp.Threading.Tasks;
using Nova;

public class 主场景背景管理_茉 : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject PC背景;
    [SerializeField] GameObject 手机背景;
    [SerializeField] UIBlock2D 加载遮罩;
    
    private void Awake()
    {
#if UNITY_EDITOR
        // 在编辑器中根据选择的平台返回后缀
        if (UnityEditor.EditorPrefs.GetInt("TestPlatform", 0) == 0)
        {
            PC背景.SetActive(true);
            手机背景.SetActive(false);
        }
        else
        {
            PC背景.SetActive(false);
            手机背景.SetActive(true);
        }
        // ShowMask(true);
        // 预加载首页().Forget();
#elif UNITY_ANDROID || UNITY_IOS
        PC背景.SetActive(false);
        手机背景.SetActive(true);
#else
        PC背景.SetActive(true);
        手机背景.SetActive(false);
        // ShowMask(true);
        // 预加载首页().Forget();
#endif
    }
    private async UniTask 预加载首页()
    {
        var manager = FindObjectOfType<界面管理器_韩>();
        if (manager != null)
        {
            await manager.预加载界面("首页界面显示");
        }
        else
        {
            Debug.LogWarning("未找到界面管理器，无法预加载首页界面");
        }
        ShowMask(false);
        return;
    }

    private void ShowMask(bool show)
    {
        if (加载遮罩 == null) return;
        加载遮罩.gameObject.SetActive(show);
    }
}
