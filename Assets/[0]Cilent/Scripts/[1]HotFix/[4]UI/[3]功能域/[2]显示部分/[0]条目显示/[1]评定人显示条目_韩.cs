using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;
using Nova;
using QFramework;

public class 评定人显示条目_韩 : MonoBehaviour, IController, IentryDis/*,显示条目接口*/
{
    // Start is called before the first frame update

    [SerializeField] public TextBlock 账号;
    [SerializeField] public TextBlock 姓名;
    [SerializeField] public TextBlock 性别;
    [SerializeField] public TextBlock 联系方式;
    [SerializeField] public TextBlock 出生日期;
    [SerializeField] public NovaSamples.UIControls.Button 查看;
    [SerializeField] public NovaSamples.UIControls.Button 删除;

    private int 当前索引 = -1; // 存储当前条目的索引

    public int 索引下标 { get; set; }

    void Start()
    {
        // 初始化时添加监听器
        查看.OnClicked.AddListener(() => 查看信息());
        删除.OnClicked.AddListener(() => 删除信息());
    }

    public void 显示信息(int 索引 = -1)
    {
        if (索引 == -1)
        {
            Debug.Log("[ [0] 评定人显示条目_韩] 显示信息 索引为-1");
            return;
        }

        当前索引 = 索引; // 保存索引

        var 评定人 = this.GetModel<评人录>();
        var 当前人员 = 评定人.GetDataList()[索引];
        Debug.Log($"[ [0] 评定人显示条目_韩] 显示信息 索引: {索引}, 账号: {当前人员.账号}, 姓名: {当前人员.名}");
        账号.Text = 当前人员.账号;
        姓名.Text = 当前人员.名;
        性别.Text = 当前人员.性别;
        联系方式.Text = 当前人员.电话;
        出生日期.Text = 当前人员.诞辰;

    }

    void 查看信息()
    {
        Debug.Log($"查看索引 {当前索引} 的信息");
    }

    void 删除信息()
    {
        if (当前索引 == -1)
        {
            Debug.Log("[ [0] 评定人显示条目_韩] 删除信息 索引为-1");
            return;
        }

        Debug.Log($"删除索引 {当前索引} 的信息");
        var 评定人 = this.GetModel<评人录>();
        var 当前人员 = 评定人.GetDataList()[当前索引];

        this.SendCommand(new 删记于录(当前人员, "评人录"));
    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }

    void OnDestroy()
    {
        if (查看 != null)
        {
            查看.OnClicked.RemoveAllListeners();
        }
        if (删除 != null)
        {
            删除.OnClicked.RemoveAllListeners();
        }
    }
}
