using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;
using Nova;
using QFramework;
public class 量表待添加条目_茉 : MonoBehaviour, IentryDis, IController
{
    public int 索引下标 { get; set; }
    [SerializeField] TextBlock 用户编号;
    [SerializeField] TextBlock 姓名;
    [SerializeField] TextBlock 性别;
    [SerializeField] TextBlock 出生日期;
    [SerializeField] TextBlock 年龄;
    [SerializeField] TextBlock 添加时间;
    [SerializeField] TextBlock 评估状态文本;
    private int 当前索引 = -1;
    void Start()
    {

    }


    public void 显示信息(int 索引 = -1)
    {
        当前索引 = 索引;
        var 档案录 = this.GetModel<档案录>();
        var 当前档案 = 档案录.GetDataList()[索引];
        if (当前档案.评估状态 != 评估状态.评估完成)
        {
            用户编号.Text = 当前档案.编号;
            姓名.Text = 当前档案.姓名;
            性别.Text = 当前档案.性别;
            出生日期.Text = 当前档案.出生日期;
            年龄.Text = 当前档案.年龄;
            添加时间.Text = 当前档案.添加时间;
            评估状态文本.Text = 当前档案.评估状态.ToString();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {

    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
