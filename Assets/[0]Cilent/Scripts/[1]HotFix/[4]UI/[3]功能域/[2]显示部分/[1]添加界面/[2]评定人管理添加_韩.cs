using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System;
using Nova;
using NovaSamples.UIControls;
using Cysharp.Threading.Tasks;
public class 评定人管理添加_韩 : 界面基础_韩
{
    // Start is called before the first frame update
    [SerializeField] public NovaSamples.UIControls.TextField 登录账号;
    [SerializeField] public NovaSamples.UIControls.TextField 登录密码;
    [SerializeField] public NovaSamples.UIControls.TextField 姓名;
    [SerializeField] public NovaSamples.UIControls.Dropdown 性别;
    //  [SerializeField] public NovaSamples.UIControls.TextField 性别;
    [SerializeField] public NovaSamples.UIControls.TextField 联系方式;
    [SerializeField] public NovaSamples.UIControls.TextField 身份证;
    [SerializeField] public Button 出生日期选择按钮;
    [SerializeField] public TextBlock 出生日期文本;
    [SerializeField] public NovaSamples.UIControls.Button 确定;
    [SerializeField] public NovaSamples.UIControls.Button 取消;
 
    private bool 择日器显示;
    private GameObject 当前择日器;
    private I记贞 旧记;

    private void Awake()
    {
        登录账号.Text = "";
        登录密码.Text = "";
        姓名.Text = "";
        性别.Visuals.SelectionLabel.Color = Color.gray * 0.8f;
        性别.Visuals.SelectionLabel.Text = "请选择";
        联系方式.Text = "";
        身份证.Text = "";
        出生日期文本.Text = "";
        显示信息 += 显示信息详情;
    }
    void Start()
    {

        确定.OnClicked.AddListener(() => 添加账号());
        取消.OnClicked.AddListener(() => 取消添加());
  
        出生日期选择按钮.OnClicked.AddListener(() =>
     {
         选择出生日期().Forget();
     });
    }
    async UniTaskVoid 选择出生日期()
    {
        if (择日器显示)
        {
            return;
        }
        择日器显示 = true;
        var 择日器气 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("择日器");
        var 择日器 = Instantiate(择日器气, this.transform);
        当前择日器 = 择日器;
        择日器.GetComponent<择日器_茉>().On保存日期 += (年, 月, 日) =>
        {
            Debug.Log("出生日期选择");
            出生日期文本.Text = $"{年}-{月}-{日}";
            择日器显示 = false;
            当前择日器 = null;
        };
        // 监控择日器是否被销毁（用于处理取消关闭的情况）
        监控择日器销毁(择日器).Forget();
    }

    async UniTaskVoid 监控择日器销毁(GameObject 择日器)
    {
        // 等待择日器被销毁
        while (择日器 != null)
        {
            await UniTask.Yield();
        }
        // 择日器已被销毁，重置标志
        if (择日器显示)
        {
            择日器显示 = false;
            当前择日器 = null;
        }
    }

    private void 显示信息详情(I记贞 记)
    {
        Debug.Log("显示详情");
        评人 评人 = 记 as 评人;
        登录账号.Text = 评人.账号;
        登录密码.Text = 评人.密码;
        姓名.Text = 评人.名;
        性别.DropdownOptions.SelectedIndex = 评人.性别 == "男" ? 0 : 1;
        性别.载始项();
        联系方式.Text = 评人.电话;
        身份证.Text = 评人.身份证;
        出生日期文本.Text = 评人.诞辰;
        旧记 = 记;
    }

    void 添加账号()
    {
        if (登录账号.Text == "")
        {
            Debug.Log("[ [2] 评定人管理添加_韩]添加账号失败，请检查登录账号");
            return;
        }
        if (登录密码.Text == "")
        {
            Debug.Log("[ [2] 评定人管理添加_韩]添加账号失败，请检查登录密码");
            return;
        }
        if (姓名.Text == "")
        {
            Debug.Log("[ [2] 评定人管理添加_韩]添加账号失败，请检查姓名");
            return;
        }

        评人 新评人 = new 评人();
        新评人.账号 = 登录账号.Text;
        新评人.密码 = 登录密码.Text;
        新评人.名 = 姓名.Text;
        新评人.性别 = 性别.DropdownOptions.Options[性别.DropdownOptions.SelectedIndex];
        新评人.电话 = 联系方式.Text;
        新评人.身份证 = 身份证.Text;
        新评人.诞辰 = 出生日期文本.Text;

        var 管员录 = this.GetModel<管员录>();
        var 旧评人 = 旧记 as 评人;
        if (旧记 != null)
        {
            if (管员录.GetDataList().Find(x => x.账号 == 旧评人.账号) != null)
            {
                this.SendCommand(new 改记于录(旧记, 新评人, "管员录"));
                this.GetModel<管员录>().登陆管理员 = 新评人;
            }
            else
            {
                this.SendCommand(new 改记于录(旧记, 新评人, "评人录"));
            }
        }
        else
        {
            this.SendCommand(new 增记于录(新评人, "评人录"));
        }


      
        Debug.Log("[ [2] 评定人管理添加_韩]添加账号" + 新评人.名);
        清空文本框内容();
        FindObjectOfType<界面管理器_韩>().显示界面("评定人管理显示");
    }

    void 取消添加()
    {
        Debug.Log("[ [2] 评定人管理添加_韩]取消添加");
        清空文本框内容();
        FindObjectOfType<界面管理器_韩>().显示界面("评定人管理显示_PC");

    }

    protected override void OnShow()
    {
        Debug.Log("[ [2] 评定人管理添加_韩]OnShow");
    }
    protected override void OnHide()
    {
        Debug.Log("[ [2] 评定人管理添加_韩]OnHide");
    }

    public void 清空文本框内容()
    {
        登录账号.Text = "";
        登录密码.Text = "";
        姓名.Text = "";
        性别.Visuals.SelectionLabel.Color = Color.gray * 0.8f;
        性别.Visuals.SelectionLabel.Text = "请选择";
        联系方式.Text = "";
        身份证.Text = "";
        出生日期文本.Text = "";
    }
    void OnDestroy()
    {
        this.取消.OnClicked.RemoveAllListeners();
        this.确定.OnClicked.RemoveAllListeners();
    
    }

}
