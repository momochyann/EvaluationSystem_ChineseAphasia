using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
using NovaSamples.UIControls;
using QFramework;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
public class 索录栏契韩_茉 : MonoBehaviour, IController
{
    // Start is called before the first frame update

    [SerializeField] string 录名;
    [SerializeField] List<下拉框帜> 下拉框帜集;
    [SerializeField] TextField 输入框;
    [SerializeField] Dropdown 下拉框;
    [SerializeField] Button 索按钮;
    [SerializeField] string 目气名;
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
            Debug.Log("记干名1" + field.Name);
            记干名.Add(field.Name);
        }
        foreach (var property in properties)
        {
            Debug.Log("记干名" + property.Name);

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
                        //                        Debug.Log(fieldValue.ToString() + " " + 输入框.Text);
                        if (fieldValue != null && fieldValue.ToString().Contains(输入框.Text))
                        {

                            记集.Add(记);
                        }
                    }
                }
            }
        }
        显记于目(记集).Forget();
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
            var mask = 显记.GetComponent<ClipMask>();
            if (mask != null)
            {
               DOTween.To((float value) =>
               {
                   mask.Tint = new Color(mask.Tint.r, mask.Tint.g, mask.Tint.b, value);
               }, 0, 1, 0.5f);
            }

        }
    }
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
