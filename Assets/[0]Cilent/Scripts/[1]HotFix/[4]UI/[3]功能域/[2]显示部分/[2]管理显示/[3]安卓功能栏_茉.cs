using System;
using System.Collections;
using System.Collections.Generic;
using Nova;
using NovaSamples.UIControls;
using UnityEngine;
using QFramework;
using Cysharp.Threading.Tasks;
using System.Linq;
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
    [SerializeField] string 搜索栏名;
    [SerializeField] string 录名;
    [SerializeField] Button 管钮;
    [SerializeField] Button 删钮;
    [SerializeField] Button 寻钮;
    [SerializeField] Button 消寻钮;
    [SerializeField] 条目组性_茉 条组;
    [SerializeField] Toggle 全选勾选;
   
    bool 全选监听;
    void Start()
    {
        if (管钮 != null) 管钮.OnClicked.AddListener(管);
        if (删钮 != null) 删钮.OnClicked.AddListener(() => 删().Forget());
        if (寻钮 != null) 寻钮.OnClicked.AddListener(寻);
        if (消寻钮 != null)
        {
            消寻钮.OnClicked.AddListener(消寻);
            消寻钮.gameObject.SetActive(false);
        }
        if (全选勾选 != null) 全选勾选.OnToggled.AddListener(全选);
    }

    private void 全选(bool 是否选中)
    {
        if (是否选中)
        {
            条组.勾组.ForEach(勾 =>
                  {
                      勾.ToggledOn = 是否选中;
                  });
            全选监听 = true;
        }
        else
        {
            if (全选监听)
            {
                全选监听 = false;
                条组.勾组.ForEach(勾 =>
                  {
                      勾.ToggledOn = 是否选中;
                  });
            }
        }
    }

    private void FixedUpdate()
    {
        if (全选监听)
        {
            Debug.Log("全选监听");
            if (条组.勾组.Any(勾 => !勾.ToggledOn))
            {
                全选监听 = false;
                全选勾选.ToggledOn = false;
            }
        }
    }
    async private void 消寻()
    {
        消寻钮.gameObject.SetActive(false);
        var 显示条目_韩 = FindObjectOfType<显示条目_韩>();
        显示条目_韩.刷新显示();
        条组.CurrentSelectedEntries.Clear();
        await UniTask.Delay(100);
        条组.沐浴();

    }

    async private void 寻()
    {
        var 搜索栏new = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb(搜索栏名);
        var 搜索栏 = Instantiate(搜索栏new, transform.parent.parent);
        搜索栏.GetComponent<择木栏安卓_茉>().索后行 = async () =>
        {
            消寻钮.gameObject.SetActive(true);
            条组.CurrentSelectedEntries.Clear();
            await UniTask.Delay(100);
            条组.沐浴();
        };
        //var 户择 = await 搜索栏.GetComponent<择木栏安卓_茉>().等待确认显示("搜索", "请输入搜索内容");
        //if (户择)
        //{
        //    Debug.Log("搜索");
        //}
        // Destroy(搜索栏.gameObject, 0.1f);
    }

    async UniTaskVoid 删()
    {
        if (条组.CurrentSelectedEntries.Count == 0)
        {
            return;
        }
        var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 界面管理器 = FindObjectOfType<界面管理器_韩>();

        var ins = Instantiate(告示, 界面管理器.transform);
        var 户择 = await ins.GetComponent<告示_茉>().等待确认显示("删除确认", "确定要删除这个项目吗？");
        if (户择)
        {
            List<int> 条目下标集 = new List<int>();
            foreach (var 条目 in 条组.CurrentSelectedEntries)
            {
                var 条目下标 = 条目.GetComponent<IentryDis>().索引下标;
                条目下标集.Add(条目下标);
            }
            条目下标集 = 条目下标集.OrderByDescending(x => x).ToList();
            foreach (var 条目下标 in 条目下标集)
            {
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

    public void 设置全选(bool 是否选中)
    {
        if (全选勾选 == null)
        {
            return;
        }

        全选(是否选中);
        全选勾选.ToggledOn = 是否选中;
    }

    public IReadOnlyList<GameObject> 获取当前选中条目()
    {
        return 条组?.CurrentSelectedEntries;
    }
}
