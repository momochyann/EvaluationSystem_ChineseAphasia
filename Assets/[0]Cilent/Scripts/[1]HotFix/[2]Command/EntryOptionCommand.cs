using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
public class EntryOptionCommand : AbstractCommand
{
    protected override void OnExecute()
    {

    }

}

public class 删记于录 : AbstractCommand
{
    public 删记于录(I记贞 deleteValue, string entryModelName)
    {
        this.deleteValue = deleteValue;
        this.entryModelName = entryModelName;
    }
    I记贞 deleteValue;
    string entryModelName;

    protected override void OnExecute()
    {
        var model = this.按名取录(entryModelName);
        if (model != null)
        {
            model.RemoveItem(deleteValue);
            Debug.Log("[删记于录]删除" + entryModelName);
        }
        else
        {
            Debug.LogError("[删记于录]找不到" + entryModelName);
        }
    }

}
public class 改记于录 : AbstractCommand
{
    public 改记于录(I记贞 oldValue, I记贞 newValue, string entryModelName)
    {
        this.oldValue = oldValue;
        this.newValue = newValue;
        this.entryModelName = entryModelName;
    }
    I记贞 oldValue;
    I记贞 newValue;
    string entryModelName;
    protected override void OnExecute()
    {
        var model = this.按名取录(entryModelName);
        if (model != null)
        {
            model.EditItem(oldValue, newValue);
            Debug.Log("[改记于录]编辑" + entryModelName);
        //    WorkSceneManager.Instance.加载提示("编辑成功");
        }
    }
}

public class 增记于录 : AbstractCommand
{
    public 增记于录(I记贞 entry, string entryModelName)
    {
        this.entry = entry;
        this.entryModelName = entryModelName;
    }
    I记贞 entry;
    string entryModelName;
    protected override void OnExecute()
    {
        var model = this.按名取录(entryModelName);
        if (model != null)
        {
            model.AddItem(entry);
            Debug.Log("[增记于录]添加" + entryModelName);
        }
        else
        {
            Debug.LogError("[增记于录]找不到" + entryModelName);
        }
    }
    // async UniTaskVoid 确认面板(string 提示文本内容, UnityAction 确认回调, string 跳转面板名称 = "")
    // {
    //     var pfb = await this.GetModel<YooAssetPfbModel>().LoadPfb("确认提示弹窗");
    //     var 确认提示弹窗 = Instantiate(pfb, FindObjectOfType<Canvas>().transform).GetComponent<P_TipPanel>();
    //     确认提示弹窗.显示面板(提示文本内容, 跳转面板名称, 确认回调);
    // }
}
