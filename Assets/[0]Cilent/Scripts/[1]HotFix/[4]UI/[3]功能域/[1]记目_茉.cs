using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;
using Nova;
using System.Linq;

public class 记目_茉 : MonoBehaviour, I记目贞
{
    I记贞 _记;
    public I记贞 记 { get => _记; set => 现记于目(value); }
    TextBlock[] _blockTexts;
    private void Awake()
    {
        _blockTexts = GetComponentsInChildren<TextBlock>();

    }
    // Start is called before the first frame update
    void Start()
    {

    }
    protected virtual void 现记于目(I记贞 目显记)
    {
        _记 = 目显记;
        if (目显记 != null)
        {
            var fields = 目显记.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var properties = 目显记.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var 记干名 = new List<string>();
            foreach (var field in fields)
            {
                记干名.Add(field.Name);
            }
            foreach (var property in properties)
            {
                记干名.Add(property.Name);
            }
            // 比对记干名和TextBlock的gameObject.name，如果相同则显示对应的值
            foreach (var textBlock in _blockTexts)
            {
                if (textBlock != null && 记干名.Contains(textBlock.gameObject.name))
                {
                    // 找到对应的字段
                    var matchedField = fields.FirstOrDefault(f => f.Name == textBlock.gameObject.name);
                    if (matchedField != null)
                    {
                        var fieldValue = matchedField.GetValue(目显记);
                        textBlock.Text = fieldValue?.ToString() ?? "";
                    }
                }
            }
        }
    }

}
public interface I记目贞
{
    public I记贞 记 { get; set; }
}