using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
using NovaSamples.UIControls;
using QFramework;
using Cysharp.Threading.Tasks;
public class 截图钮 : MonoBehaviour, IController
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().OnClicked.AddListener(() =>
        {
            this.截图().Forget();
        });
    }
    async UniTask 截图()
    {
        await UniTask.Delay(1000);
        var utility = this.GetUtility<IScreenshotPDFUtility>();
        Scroller resultList = FindObjectOfType<Scroller>(); // 您的结果列表
        Debug.Log(resultList.gameObject.name);
        string filePath = await utility.CaptureScrollViewAsync(resultList);
        utility.PrintFile(filePath); // 直接打印
    }
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
