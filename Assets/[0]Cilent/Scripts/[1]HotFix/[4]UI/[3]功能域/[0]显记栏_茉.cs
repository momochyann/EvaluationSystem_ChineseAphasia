using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;
using Nova;
using QFramework;
using Cysharp.Threading.Tasks;

public class 显记栏_茉 : 牖元_茉
{
    // Start is called before the first frame update
    public string 录帜;
    [SerializeField] string 目名;
    [Header("组件")]
    [SerializeField] Scroller 记卷;
    I录贞 录;
    按名索录之器 按名索录之器;
    void Start()
    {
        长生();
    }
    void 长生()
    {
        this.RegisterEvent<Can2ListModelChangeEvent>(授录变令);
        按名索录之器 = this.GetUtility<按名索录之器>();
        录 = 按名索录之器.寻录<I录贞>(录帜);
        现目于栏(录.GetAllItems()).Forget();
    }
    void 授录变令(Can2ListModelChangeEvent 录变令)
    {
        if (录变令.ModelTypeName == 录帜)
        {
            现目于栏(录.GetAllItems()).Forget();
           // this.显示提示("录变令");
        }
    }
    public async UniTaskVoid 现目于栏(List<I记贞> 待显记集)
    {
        for (int i = 记卷.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(记卷.transform.GetChild(i).gameObject);
        }

        foreach (var 待显目之记 in 待显记集)
        {
            var 目气 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb(目名);
            var 目 = Instantiate(目气, 记卷.transform);
            目.GetComponent<记目_茉>().记 = 待显目之记;
        }
    }
}
