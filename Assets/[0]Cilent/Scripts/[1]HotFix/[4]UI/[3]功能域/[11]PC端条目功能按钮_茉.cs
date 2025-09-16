using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using NovaSamples.UIControls;
using System.Linq;
using Cysharp.Threading.Tasks;
using System;
public class PC端条目功能按钮_茉 : MonoBehaviour, IController
{


    // Start is called before the first frame update
    [SerializeField] string 录名;
    [SerializeField] GameObject 条目显示面板;

    [SerializeField] Button 删除按钮;
    void Start()
    {
  
        // foreach (var 勾 in 勾组)
        // {
        //     勾.OnToggled.AddListener((bool 是否选中) => 择(勾, 是否选中));
        // }
        删除按钮.OnClicked.AddListener(删除);
        // first = false;
    }

    private void 删除()
    {
        Debug.Log("删除");
        var 勾组 = 条目显示面板.GetComponentsInChildren<Toggle>(true).ToList();
        Debug.Log("勾组" + 勾组.Count);
        List<Toggle> 勾组2 = new List<Toggle>();
        if (勾组.Count > 0)
        {
            foreach (var 勾 in 勾组)
            {
                if (勾.ToggledOn)
                {
                    勾组2.Add(勾);
                }
            }
        }
        if (勾组2.Count > 0)
        {
            删除(勾组2);
        }
      
    }

    async private void 删除(List<Toggle> 勾组)
    {
        
        var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 界面管理器 = FindObjectOfType<界面管理器_韩>();

        var ins = Instantiate(告示, 界面管理器.transform);
        var 户择 = await ins.GetComponent<告示_茉>().等待确认显示("删除确认", "确定要删除数据吗？");
        if (户择)
        {
            foreach (var 勾 in 勾组)
            {
                var 条目 = 勾.transform.parent.GetComponent<IentryDis>();
                var 条目下标 = 条目.索引下标;
                this.SendCommand(new 删记于录(条目下标, 录名));
            }
            // foreach (var 条目 in 条组.CurrentSelectedEntries)
            // {
            //     var 条目下标 = 条目.GetComponent<IentryDis>().索引下标;
            //     this.SendCommand(new 删记于录(条目下标, 录名));
            // }
            // 条组.CurrentSelectedEntries.Clear();
            // await UniTask.Delay(100);
            // 条组.沐浴();
        }
        Destroy(ins.gameObject, 0.1f);
    }
    // Update is called once per frame
    void Update()
    {

    }
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }

}
