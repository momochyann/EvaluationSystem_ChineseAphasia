using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using Nova;
using NovaSamples.UIControls;
using Cysharp.Threading.Tasks;

public class 条目查看按钮_茉 : MonoBehaviour, IController
{
    // Start is called before the first frame update
    [SerializeField] string 录帜;
    [SerializeField] string 添加面板名;
    void Start()
    {
        this.GetComponent<Button>().OnClicked.AddListener(() =>
        {
            查看信息();
        });
    }
    void 查看信息()
    {
        var 索引下标 = transform.parent.GetComponent<IentryDis>().索引下标;
        var 按名索录之器 = this.GetUtility<按名索录之器>();
        var I记贞 = 按名索录之器.寻录<I录贞>(录帜).GetAllItems()[索引下标];
        显录于增板(I记贞).Forget();
    }
    async UniTaskVoid 显录于增板(I记贞 记)
    {
        var 界面管理器 = FindObjectOfType<界面管理器_韩>();
        界面管理器.显示界面(添加面板名+"_PC");
        await UniTask.Delay(100);
        界面管理器._currentPanel.显示信息?.Invoke(记);
        // Instantiate(await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb(添加面板名), 界面管理器.transform)
        // .GetComponent<界面基础_韩>().显示信息?.Invoke(记);

    }
    // Update is called once per frame
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
