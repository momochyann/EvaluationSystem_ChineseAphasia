using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
public class GameStartAfterLoad : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        gameObject.GetComponent<SupplementaryMetadata>().onSupplementaryMetadata += GameLoadedInit;
    }
    async void GameLoadedInit()
    {
        Debug.Log("GameHotedInit8");
        Application.targetFrameRate = 120;
        //    await WaitLoadAnimation(this.GetCancellationTokenOnDestroy());
        Debug.Log("加载数据完成222");
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        var 电脑加密狗串口生气 = await LoadYooAssetsTool.LoadAsset<GameObject>("加密狗工具");
        var 电脑加密狗串口成气 = Instantiate(电脑加密狗串口生气, FindObjectOfType<Canvas>().transform);
        var _电脑加密狗串口 = 电脑加密狗串口成气.GetComponent<电脑端加密狗串口检测>();
        bool isLocked = await _电脑加密狗串口.启动连接加密狗();
        _电脑加密狗串口.DisLockUI(isLocked);

        if (isLocked)
        {
            _电脑加密狗串口.Disconnect();
            var 电脑蓝牙串口生气 = await LoadYooAssetsTool.LoadAsset<GameObject>("电脑蓝牙串口");
            var 电脑蓝牙串口成气 = Instantiate(电脑蓝牙串口生气);
            var _电脑蓝牙串口 = 电脑蓝牙串口成气.GetComponent<电脑端蓝牙串口检测>();
            await UniTask.Delay(10);
            await _电脑蓝牙串口.启动连接蓝牙();
            LoadYooAssetsTool.LoadSceneAsync("登录界面场景").Forget();
        }

        //await UniTask.Delay(3000);
#elif UNITY_ANDROID || UNITY_IOS
        LoadYooAssetsTool.LoadSceneAsync("登录界面场景").Forget();
#endif
        // var HotUI = await LoadYooAssetsTool.LoadAsset<GameObject>("LoadNewGameVesion");
        // Instantiate(HotUI, FindObjectOfType<Canvas>().transform);
        //    LoadYooAssetsTool.LoadSceneAsync("GameStartScene").Forget();


        //   LoadYooAssetsTool.LoadSceneAsync("登录界面场景").Forget();

    }
    async UniTaskVoid 加载蓝牙串口()
    {

    }
    async UniTask WaitLoadAnimation(CancellationToken cancellationToken)
    {
        var progress = FindObjectOfType<HotFixAssetsProgress>();
        while (progress._tweenerQueue.Count > 0)
        {
            await UniTask.Delay(100, cancellationToken: cancellationToken);
        }
    }
}
