using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;
using Nova;
using QFramework;
using Cysharp.Threading.Tasks;

public class 牖元_茉 : MonoBehaviour, IController
{
    [HideInInspector] public GameObject UIRoot;
    private void Awake() {
        UIRoot = GameObject.Find("UIRoot");
        
    }
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}

public static class 遂牖_茉
{
    async public static UniTaskVoid 显示提示(this 牖元_茉 界面,  string 提示,GameObject 乾 =null)
    {
        var 提示框生 = await 界面.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("提示框");
        var _乾 = 乾 ?? 界面.UIRoot;
        var 提示框 = GameObject.Instantiate(提示框生,_乾.transform);
        //界面.显示提示(提示);
    }
}
