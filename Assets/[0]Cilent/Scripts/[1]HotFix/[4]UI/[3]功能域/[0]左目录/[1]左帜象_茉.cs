using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;
using Nova;
using DG.Tweening;
public class 左帜象_茉 : MonoBehaviour
{
    [SerializeField] Color 选中颜色;
    TextField textField;
    左目录选择_韩 目录形;
    void Start()
    {
        目录形 = GetComponent<左目录选择_韩>();
        foreach (var item in 目录形.目录列表)
        {
            item.OnClicked.AddListener(() => 显切象(item));
        }
    }

    private void 显切象(Button button)
    {
        var uiBlock = button.GetComponent<UIBlock2D>();
        更新Block2D颜色(button.transform.Find("图标").GetComponent<UIBlock2D>(), Color.white);
        更新Block2D颜色(uiBlock, 选中颜色);
        更新Block2D颜色(button.transform.Find("标题").GetComponent<TextBlock>(), Color.white);
        foreach (var item in 目录形.目录列表)
        {
            if (item != button)
            {
                更新Block2D颜色(item.GetComponent<UIBlock2D>(), Color.white);
                更新Block2D颜色(item.transform.Find("图标").GetComponent<UIBlock2D>(), Color.black);
                更新Block2D颜色(item.transform.Find("标题").GetComponent<TextBlock>(), Color.black);
            }
        }
    }
    void 更新Block2D颜色(UIBlock block, Color color, float duration = 0.4f)
    {
        Color startColor = block.Color;
        DOTween.To(() => startColor, x =>
        {
            startColor = x;
            block.Color = x;
        }, color, duration);
    }

    // button.();
    //button.GetComponent<UIBlock2D>().Color = Color.white;
}
