using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using QFramework;
public class GameStartAfterLoad : MonoBehaviour, IController
{
    // Start is called before the first frame update
    private IWindowTitleBridge 窗口标题桥;
    private const string 默认窗口标题 = "汉语失语症评测系统";
    private static readonly string[] 评估界面列表 =
    {
        "首页界面显示",
        "量表评估书写记录",
        "量表评估利手记录",
        "量表评估命名记录",
        "量表评估复述记录",
        "量表评估理解记录",
        "量表评估结构与空间记录",
        "量表评估计算记录",
        "量表评估谈话记录",
        "量表评估运用记录",
        "量表评估阅读记录"
    };

    void Start()
    {
        gameObject.GetComponent<SupplementaryMetadata>().onSupplementaryMetadata += GameLoadedInit;
        窗口标题桥 = new WindowTitleBridge();
    }
    async void GameLoadedInit()
    {
        Debug.Log("GameHotedInit8");
        Application.targetFrameRate = 120;
        //await WaitLoadAnimation(this.GetCancellationTokenOnDestroy());
        Debug.Log("加载数据完成222");
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        // var 电脑加密狗串口生气 = await LoadYooAssetsTool.LoadAsset<GameObject>("加密狗工具");
        // var 电脑加密狗串口成气 = Instantiate(电脑加密狗串口生气, FindObjectOfType<Canvas>().transform);
        // var _电脑加密狗串口 = 电脑加密狗串口成气.GetComponent<电脑端加密狗串口检测>();
        // bool isLocked = await _电脑加密狗串口.启动连接加密狗();
        // _电脑加密狗串口.DisLockUI(isLocked);

        // if (isLocked)
        // {
            // _电脑加密狗串口.Disconnect();
            var 电脑蓝牙串口生气 = await LoadYooAssetsTool.LoadAsset<GameObject>("电脑蓝牙串口");
            var 电脑蓝牙串口成气 = Instantiate(电脑蓝牙串口生气);
            var _电脑蓝牙串口 = 电脑蓝牙串口成气.GetComponent<电脑端蓝牙串口检测>();
            await UniTask.Delay(10);
            await _电脑蓝牙串口.启动连接蓝牙();
            更新窗口标题();
            预加载界面();

            LoadYooAssetsTool.LoadSceneAsync("登录界面场景").Forget();
        // }

        //await UniTask.Delay(3000);
#elif UNITY_ANDROID || UNITY_IOS
        LoadYooAssetsTool.LoadSceneAsync("登录界面场景").Forget();
#endif
        // var HotUI = await LoadYooAssetsTool.LoadAsset<GameObject>("LoadNewGameVesion");
        // Instantiate(HotUI, FindObjectOfType<Canvas>().transform);
        //    LoadYooAssetsTool.LoadSceneAsync("GameStartScene").Forget();


        //   LoadYooAssetsTool.LoadSceneAsync("登录界面场景").Forget();

    }
    UniTaskVoid 加载蓝牙串口()
    {
        return default;
    }
    async UniTask WaitLoadAnimation(CancellationToken cancellationToken)
    {
        var progress = FindObjectOfType<HotFixAssetsProgress>();
        while (progress._tweenerQueue.Count > 0)
        {
            await UniTask.Delay(100, cancellationToken: cancellationToken);
        }
    }

    private void 更新窗口标题()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        try
        {
            var 配置 = this.GetModel<软件配置录>()?.当前配置;
            var 标题 = 配置 == null || string.IsNullOrWhiteSpace(配置.软件名称)
                ? 默认窗口标题
                : 配置.软件名称;
            窗口标题桥?.SetTitle(标题, 默认窗口标题);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"启动时更新窗口标题失败: {e.Message}");
        }
#endif
    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }

    private void 预加载界面()
    {
        var loader = this.GetUtility<YooAssetPfbPoolUtility>();
        foreach (var key in 评估界面列表)
        {
            loader.LoadPfb(key).Forget();
        }
    }
}
