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
    private 评人录 评人录;
    private 管员录 管员录;

    void Start()
    {
        Debug.Log("[0] 选择界面_韩 Start");
        评人录 = this.GetModel<评人录>();
        管员录 = this.GetModel<管员录>();
        用户名.OnClicked.AddListener(执行用户名管理);
        退出.OnClicked.AddListener(执行退出);
        foreach (var item in 功能列表)
        {
            item.OnClicked.AddListener(() => 执行功能(item.name));
        }
    }

     public void 执行用户名管理()
    {
        // if (管员录.登陆管理员 != null)
        // {
        //     var 评定人管理添加 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("评定人管理添加");
        //     var 评定人管理添加实例 = Instantiate(评定人管理添加, transform.parent);
        //     var 评定人管理添加安卓 = 评定人管理添加实例.GetComponent<评定人管理添加_韩>();
        //     评定人管理添加安卓.显示信息(管员录.登陆管理员);
        // }
        // else if (评人录.登陆评人 != null)
        // {
        //     var 评定人管理添加 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("评定人管理添加");
        //     var 评定人管理添加实例 = Instantiate(评定人管理添加, transform.parent);
        //     var 评定人管理添加安卓 = 评定人管理添加实例.GetComponent<评定人管理添加_韩>();
        //     评定人管理添加安卓.显示信息(评人录.登陆评人);
        // }

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
        静态传递_韩.SwitchSceneWithPanel("主场景", 功能名称);
    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
