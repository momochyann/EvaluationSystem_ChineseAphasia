using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;
using Nova;
using QFramework;

public class 档案显示条目_韩 : MonoBehaviour,IController,IentryDis
{
    // Start is called before the first frame update
    public TextBlock 姓名;
    public TextBlock 性别;
    public TextBlock 出生日期;
    public TextBlock 身份证;
    public TextBlock 评估时间;
    public TextBlock 评估人员;
    public Button 查看按钮;
    public Button 删除按钮;
    private int 当前索引 = -1; // 存储当前条目的索引
    void Start()
    {
        查看按钮.OnClicked.AddListener(() => 查看信息());
        删除按钮.OnClicked.AddListener(() => 删除信息());
    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }

    public void 显示信息(int 索引 = -1)
    {
        当前索引 = 索引;
        var 档案录 = this.GetModel<档案录>();
        var 当前档案 = 档案录.GetDataList()[索引];
        姓名.Text = 当前档案.姓名;
        性别.Text = 当前档案.性别;
        出生日期.Text = 当前档案.出生日期;
        身份证.Text = 当前档案.身份证;
        评估时间.Text = 当前档案.评估时间;
        评估人员.Text = 当前档案.评估人员;
    }

    void 查看信息()
    {
        Debug.Log("[档案显示条目_韩] 查看信息");
    }

    void 删除信息()
    {
        if (当前索引 == -1)
        {
            Debug.Log("[ [0] 档案显示条目_韩] 删除信息 索引为-1");
            return;
        }
        
        Debug.Log($"删除索引 {当前索引} 的信息");
        var 档案录 = this.GetModel<档案录>();
        var 当前档案 = 档案录.GetDataList()[当前索引];
        
        this.SendCommand(new 删记于录(当前档案, "档案录"));
    }
    void OnDestroy()
    {
        查看按钮.OnClicked.RemoveAllListeners();
        删除按钮.OnClicked.RemoveAllListeners();
    }
}
