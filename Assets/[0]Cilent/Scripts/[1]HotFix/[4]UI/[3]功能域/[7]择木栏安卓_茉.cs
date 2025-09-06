using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
using QFramework;
using Cysharp.Threading.Tasks;
using NovaSamples.UIControls;
using System;
using System.Linq;
using DG.Tweening;
using UnityEngine.Events;

public class 择木栏安卓_茉 : MonoBehaviour, IController
{
    [SerializeField] string 录名;
    [SerializeField] string 目气名;
    [SerializeField] List<下拉框帜> 下拉框帜集;
    [SerializeField] TextField 输入框;
    [SerializeField] Dropdown 下拉框;
    [SerializeField] Button 搜索确认按钮;
    [SerializeField] Button 取消按钮;
    [SerializeField] UIBlock 面板容器; // 整个搜索面板的容器
    public  UnityAction 索后行;
    private void Awake()
    {
        面板容器.transform.DOScale(1, 0.3f).From(0);
    }
    void Start()
    {
        长生();
    }

    void 长生()
    {
        下拉框.DropdownOptions.Options.Clear();
        foreach (var 下标 in 下拉框帜集)
        {
            下拉框.DropdownOptions.Options.Add(下标.示);
        }
        下拉框.载始项();

        // 绑定按钮事件
        搜索确认按钮.OnClicked.AddListener(确认搜索);
        取消按钮.OnClicked.AddListener(取消搜索);

    }


    /// <summary>
    /// 确认搜索并执行搜索逻辑
    /// </summary>
    void 确认搜索()
    {
        索记集();

    }

    /// <summary>
    /// 取消搜索，清空输入框并隐藏面板
    /// </summary>
    void 取消搜索()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 执行搜索记录的核心逻辑
    /// </summary>
    void 索记集()
    {
        var 录 = this.GetUtility<按名索录之器>().寻录<I录贞>(录名);
        var 记干名 = new List<string>();
        var fields = 录.GetAllItems()[0].GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var properties = 录.GetAllItems()[0].GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        // 下拉框对应数据[下拉框.DropdownOptions.SelectedIndex] 
        List<I记贞> 记集 = new List<I记贞>();

        foreach (var field in fields)
        {
            // 可以在这里添加字段处理逻辑
            Debug.Log(field.Name);
            记干名.Add(field.Name);
        }

        foreach (var property in properties)
        {
            Debug.Log(property.Name);
            记干名.Add(property.Name);
        }

        foreach (var item in 记干名)
        {
            if (下拉框帜集[下拉框.DropdownOptions.SelectedIndex].帜 == item)
            {
                foreach (var 记 in 录.GetAllItems())
                {
                    var matchedField = fields.FirstOrDefault(f => f.Name == item);
                    if (matchedField != null)
                    {
                        var fieldValue = matchedField.GetValue(记);
                        // Debug.Log(fieldValue.ToString() + " " + 输入框.Text);
                        if (fieldValue != null && fieldValue.ToString().Contains(输入框.Text))
                        {
                            记集.Add(记);
                        }
                    }
                }
            }
        }
        显记于目(记集).Forget();
        //显记栏.现目于栏(记集).Forget();
    }
    async UniTaskVoid 显记于目(List<I记贞> 记集)
    {
        var 目气 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb(目气名);
        按名索录之器 按名索录之器 = this.GetUtility<按名索录之器>();
        var 录 = 按名索录之器.寻录<I录贞>(录名);
        var 显示条目_韩 = FindObjectOfType<显示条目_韩>();
        显示条目_韩.清除现有显示();
        Debug.Log("记名");
        foreach (var 记 in 记集)
        {
            Debug.Log("记名" + 记);
            var 显记 = Instantiate(目气, 显示条目_韩.transform);
            显记.GetComponent<IentryDis>().显示信息(录.GetIndex(记));
        }
        索后行?.Invoke();
        Destroy(gameObject, 0.1f);
    }
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
