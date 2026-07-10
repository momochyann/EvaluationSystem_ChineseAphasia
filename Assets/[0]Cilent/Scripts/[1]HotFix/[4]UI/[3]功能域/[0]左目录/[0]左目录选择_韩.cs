using System.Collections;
using System.Collections.Generic;
using Nova;
using NovaSamples.UIControls;
using UnityEngine;
using QFramework;
using Cysharp.Threading.Tasks;

public class 左目录选择_韩 : MonoBehaviour, IController
{
    // Start is called before the first frame update

    [SerializeField] public List<NovaSamples.UIControls.Button> 目录列表;
    private 评人录 评人录;
    private 管员录 管员录;

    void Start()
    {
        评人录 = this.GetModel<评人录>();
        管员录 = this.GetModel<管员录>();

        foreach (var item in 目录列表)
        {
            if (是否需要管理员权限(item.name) && 管员录.登陆管理员 == null)
            {
                item.gameObject.SetActive(false);
                continue;
            }

            item.OnClicked.AddListener(() => 执行功能(item.name));
        }
    }

    async void 执行功能(string 功能名称)
    {
        Debug.Log("执行功能：" + 功能名称);

        // 检查是否为管理员权限功能
        if (是否需要管理员权限(功能名称))
        {
            if (管员录.登陆管理员 == null)
            {
                // 评定人登录时，弹出权限提示
                await 显示权限提示();
                return;
            }
        }

        功能名称 = 功能名称 + "显示_PC";
        FindObjectOfType<界面管理器_韩>().显示界面(功能名称);
    }

    /// <summary>
    /// 检查功能是否需要管理员权限
    /// </summary>
    private bool 是否需要管理员权限(string 功能名称)
    {
        // 需要管理员权限的功能列表
        string[] 管理员功能 = new string[]
        {
            "评定人管理",
            "界面管理",
            "备份还原"
        };

        foreach (var 功能 in 管理员功能)
        {
            if (功能名称.Contains(功能))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 显示权限不足的提示
    /// </summary>
    private async UniTask 显示权限提示()
    {
        var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 告示实例 = Instantiate(告示, FindObjectOfType<界面管理器_韩>().transform);
        await 告示实例.GetComponent<告示_茉>().等待确认显示("权限不足", "此仅为管理员权限，请登录管理员账号");
        Destroy(告示实例.gameObject, 0.1f);
    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

