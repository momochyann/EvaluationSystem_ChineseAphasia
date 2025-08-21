using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NovaSamples.UIControls;
using Cysharp.Threading.Tasks;
using QFramework;

public class 选择界面_韩 : MonoBehaviour, IController
{
    [SerializeField] public NovaSamples.UIControls.Button 用户名;
    [SerializeField] public NovaSamples.UIControls.Button 退出;
    [SerializeField] public List<NovaSamples.UIControls.Button> 功能列表;

    void Start()
    {
        Debug.Log("[0] 选择界面_韩 Start");
        用户名.OnClicked.AddListener(执行用户名管理);
        退出.OnClicked.AddListener(执行退出);
        foreach (var item in 功能列表)
        {
            item.OnClicked.AddListener(() => 执行功能(item.name));
        }
    }

    public void 执行用户名管理()
    {
        Debug.Log("用户名管理");
    }

    public void 执行退出()
    {
        Debug.Log("退出");
        PlayerPrefs.SetString("自动登录", "false");
        LoadYooAssetsTool.LoadSceneAsync("登录界面场景").Forget();

    }

    public void 执行功能(string 功能名称)
    {
        Debug.Log("执行功能：" + 功能名称);
        功能名称 = 功能名称 + "显示";
        静态传递_韩.SwitchSceneWithPanel("主场景",功能名称);
    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
