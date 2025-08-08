using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
using QFramework;
using Cysharp.Threading.Tasks;
using NovaSamples.UIControls;
using System;
using System.Linq;


public class 索录栏_茉 : MonoBehaviour, IController
{
    // Start is called before the first frame update
    [SerializeField] string 录名;
    [SerializeField] List<下拉框帜> 下拉框帜集;
    [SerializeField] TextField 输入框;
    [SerializeField] Dropdown 下拉框;
    [SerializeField] Button 索按钮;
    显记栏_茉 显记栏;

    void Start()
    {
        长生();
    }

    void 长生()
    {
        var 显记栏集 = FindObjectsOfType<显记栏_茉>();
        foreach (var _显记栏 in 显记栏集)
        {
            if (_显记栏.录帜 == 录名)
            {
                显记栏 = _显记栏;
            }
        }
        下拉框.DropdownOptions.Options.Clear();
        foreach (var 下标 in 下拉框帜集)
        {
            下拉框.DropdownOptions.Options.Add(下标.示);
        }
        下拉框.载始项();
        索按钮.OnClicked.AddListener(索记集);
    }

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
        }
        foreach (var property in properties)
        {
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
                        //   Debug.Log(fieldValue.ToString() + " " + 输入框.Text);
                        if (fieldValue != null && fieldValue.ToString().Contains(输入框.Text))
                        {
                            记集.Add(记);
                        }
                    }
                }
            }
        }
        显记栏.现目于栏(记集).Forget();
    }
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
[Serializable]
public struct 下拉框帜
{
    public string 示;
    public string 帜;
}