using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NovaSamples.UIControls;
using Nova;
using QFramework;
using Cysharp.Threading.Tasks;
public class 用户显示条目_韩 : MonoBehaviour, IController, IentryDis
{
    // Start is called before the first frame update
    [SerializeField] public TextBlock 账号;
    [SerializeField] public TextBlock 姓名;
    [SerializeField] public TextBlock 性别;
    [SerializeField] public TextBlock 身份信息;
    [SerializeField] public TextBlock 出生日期;
    [SerializeField] public NovaSamples.UIControls.Button 查看;
    [SerializeField] public NovaSamples.UIControls.Button 删除;
    private int 当前索引 = -1; // 存储当前条目的索引

    public int 索引下标 { get; set; }

    void Start()
    {
        // 初始化时添加监听器
        if (删除 != null) 删除.OnClicked.AddListener(() => 删除信息());
        if (查看 != null) 查看.OnClicked.AddListener(() => 查看信息());
    }

    public void 显示信息(int 索引 = -1)
    {
        if (索引 == -1)
        {
            Debug.Log("[ [0] 用户显示条目_韩] 显示信息 索引为-1");
            return;
        }

        当前索引 = 索引; // 保存索引
        索引下标 = 索引;
        var 用户录 = this.GetModel<用户录>();
        var 当前人员 = 用户录.GetDataList()[索引];
        账号.Text = 当前人员.编号;
        姓名.Text = 当前人员.名;
        性别.Text = 当前人员.性别;
        身份信息.Text = 当前人员.身份证;
        出生日期.Text = 当前人员.诞辰;

        // 注意：这里不需要重复添加监听器，因为已经在Start()中添加过了
        // 如果需要在显示信息时重新绑定，应该先清除再添加
        // 查看.OnClicked.RemoveAllListeners();
        // 删除.OnClicked.RemoveAllListeners();
        // 查看.OnClicked.AddListener(() => 查看信息());
        // 删除.OnClicked.AddListener(() => 删除信息());
    }

    void 删除信息()
    {
        if (当前索引 == -1)
        {
            Debug.Log("[ [0] 用户显示条目_韩] 删除信息 索引为-1");
            return;
        }

        Debug.Log($"删除索引 {当前索引} 的信息");
        var 用户录 = this.GetModel<用户录>();
        var 当前人员 = 用户录.GetDataList()[当前索引];

        this.SendCommand(new 删记于录(当前人员, "用户录"));
    }

    async void 查看信息()
    {
        var 界面 = FindObjectOfType<界面管理器_韩>();
        界面.显示界面("用户管理添加");
        await UniTask.Delay(100);
        界面._currentPanel.GetComponent<界面基础_韩>().显示信息?.Invoke(this.GetModel<用户录>().GetDataList()[当前索引]);
        //界面._currentPanel.GetComponent<用户管理添加安卓_茉>().显示信息(this.GetModel<用户录>().GetDataList()[当前索引]);
        Debug.Log($"查看索引 {当前索引} 的信息");
    }

    void OnDestroy()
    {
        if (查看 != null) 查看.OnClicked.RemoveAllListeners();
        if (删除 != null) 删除.OnClicked.RemoveAllListeners();
    }
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
