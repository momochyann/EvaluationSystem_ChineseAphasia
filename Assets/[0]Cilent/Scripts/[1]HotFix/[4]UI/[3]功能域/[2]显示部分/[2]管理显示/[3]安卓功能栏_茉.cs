using System;
using System.Collections;
using System.Collections.Generic;
using Nova;
using NovaSamples.UIControls;
using UnityEngine;
using QFramework;
using Cysharp.Threading.Tasks;
public class 安卓功能栏 : MonoBehaviour, IController
{
    // Start is called before the first frame update
    bool _持管;
    public bool 持管
    {
        get => _持管;
        set
        {
            _持管 = value;
            条组.管锁 = value;
            删钮.gameObject.SetActive(value);
            Debug.Log(条组.勾组.Count);
            foreach (var 勾 in 条组.勾组)
            {
                勾.enabled = !value;
            }
        }
    }
    [SerializeField] string 录名;
    [SerializeField] Button 管钮;
    [SerializeField] Button 删钮;
    [SerializeField] Button 寻钮;
    [SerializeField] 条目组性_茉 条组;
    void Start()
    {
        管钮.OnClicked.AddListener(管);
        删钮.OnClicked.AddListener(() => 删().Forget());
        寻钮.OnClicked.AddListener(寻);
    }

    private void 寻()
    {

    }

    async UniTaskVoid 删()
    {
        var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 界面管理器 = FindObjectOfType<界面管理器_韩>();
        var ins = Instantiate(告示, 界面管理器.transform);
        var 户择 = await ins.GetComponent<告示_茉>().等待确认显示("删除确认", "确定要删除这个项目吗？");
        if (户择)
        {
            foreach (var 条目 in 条组.CurrentSelectedEntries)
            {
                var 条目下标 = 条目.GetComponent<IentryDis>().索引下标;
                this.SendCommand(new 删记于录(条目下标, 录名));
            }
            条组.CurrentSelectedEntries.Clear();
            await UniTask.Delay(100);
            条组.沐浴();
           
        }
        Destroy(ins.gameObject, 0.1f);
    }

    private void 管()
    {
        if (持管)
        {
            持管 = false;
            管钮.GetComponentInChildren<TextBlock>().Text = "管理";
        }
        else
        {
            持管 = true;
            管钮.GetComponentInChildren<TextBlock>().Text = "退出管理";
        }
    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }


}
