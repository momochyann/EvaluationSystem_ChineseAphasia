using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Nova;
using NovaSamples.UIControls;
using System;
using Cysharp.Threading.Tasks;
using System.Linq;
public class 条目组性_茉 : MonoBehaviour, IController
{
    // Start is called before the first frame update
    [HideInInspector] public bool 管锁 = false;
    [HideInInspector] public List<GameObject> CurrentSelectedEntries;
    [SerializeField] Color 择色;
    Button[] 钮组;
    public List<Toggle> 勾组;
    bool first = true;
    void Start()
    {
        长生();
    }
    async void 长生()
    {
        await UniTask.Delay(100);
        钮组 = GetComponentsInChildren<Button>();
        foreach (var 钮 in 钮组)
        {
            钮.OnClicked.AddListener(() => 择(钮));
        }
        勾组 = GetComponentsInChildren<Toggle>(true).ToList();
        foreach (var 勾 in 勾组)
        {
            勾.OnToggled.AddListener((bool 是否选中) => 择(勾, 是否选中));
        }
        first = false;
    }
    private async void OnEnable() {
        if(first)
        return;
        await UniTask.Delay(100);
        沐浴();
    }
    public void 沐浴()
    {
        foreach (var 钮 in 钮组)
        {
            钮.OnClicked.RemoveAllListeners();
        }
        foreach (var 勾 in 勾组)
        {
            勾.OnToggled.RemoveAllListeners();
        }
        勾组.Clear();
        Array.Clear(钮组, 0, 钮组.Length);
        长生();
    }
    async private void 择(Toggle 勾, bool 是否选中)
    {
        await UniTask.Delay(100);
        if (CurrentSelectedEntries.Contains(勾.transform.parent.gameObject))
        {
            if (!是否选中)
            {
                CurrentSelectedEntries.Remove(勾.transform.parent.gameObject);
                勾.transform.parent.GetComponent<UIBlock2D>().Color = Color.white;
            }
            return;
        }

        if (是否选中)
        {
            CurrentSelectedEntries.Add(勾.transform.parent.gameObject);
            勾.transform.parent.GetComponent<UIBlock2D>().Color = 择色;
        }
        else
        {
            CurrentSelectedEntries.Remove(勾.transform.parent.gameObject);
            勾.transform.parent.GetComponent<UIBlock2D>().Color = Color.white;

        }
    }

    async void 择(Button 钮)
    {
        if (管锁)
            return;
        if (CurrentSelectedEntries.Contains(钮.gameObject))
            return;
        CurrentSelectedEntries.Add(钮.gameObject);
        钮.GetComponent<UIBlock2D>().Color = 择色;

        foreach (var _钮 in 钮组)
        {
            if (钮 != _钮)
            {
                if (_钮.gameObject.GetComponentInChildren<Toggle>().ToggledOn == true)
                    continue;

                if (CurrentSelectedEntries.Contains(_钮.gameObject))
                    CurrentSelectedEntries.Remove(_钮.gameObject);
                _钮.GetComponent<UIBlock2D>().Color = Color.white;
            }
        }
    }
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }

}
