using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Cysharp.Threading.Tasks;

public class 择域 : MonoBehaviour, IController
{
    // Start is called before the first frame update
    void Start()
    {
        长生().Forget();
    }
    async UniTaskVoid 长生()
    {
        var pfb = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("主页选择界面");
        Instantiate(pfb,this.transform);
    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
