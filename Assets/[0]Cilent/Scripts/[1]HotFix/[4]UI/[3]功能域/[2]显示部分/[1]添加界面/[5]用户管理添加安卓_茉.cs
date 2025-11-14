using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using NovaSamples.UIControls;
using Nova;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;
public class 用户管理添加安卓_茉 : 界面基础_韩
{
    // Start is called before the first frame update

    [Header("个人信息")]
    [SerializeField] public TextBlock 编号;
    [SerializeField] public NovaSamples.UIControls.TextField 姓名;
    [SerializeField] public NovaSamples.UIControls.Dropdown 性别;
    [SerializeField] public NovaSamples.UIControls.TextField 身份证;
    [SerializeField] public Button 出生日期选择按钮;
    [SerializeField] public TextBlock 出生日期文本;
    [SerializeField] public NovaSamples.UIControls.TextField 民族;

    [Header("家庭信息")]
    [SerializeField] public NovaSamples.UIControls.TextField 监护人;
    [SerializeField] public NovaSamples.UIControls.TextField 关系;
    [SerializeField] public NovaSamples.UIControls.TextField 联系方式;
    [SerializeField] public NovaSamples.UIControls.TextField 居住地址;

    [Header("背景信息")]
    [SerializeField] public Button 发病日期选择按钮;
    [SerializeField] public TextBlock 发病日期文本;
    [SerializeField] public NovaSamples.UIControls.TextField 文化程度;
    [SerializeField] public NovaSamples.UIControls.TextField 血压;
    [SerializeField] public NovaSamples.UIControls.TextField 家庭成员;
    [SerializeField] public NovaSamples.UIControls.TextField 发病前后语言状态;
    [SerializeField] public NovaSamples.UIControls.TextField 发病时状况;
    [SerializeField] public NovaSamples.UIControls.TextField 既往病史;
    [SerializeField] public NovaSamples.UIControls.TextField 神志状态;

    [Header("界面")]
    [SerializeField] public GameObject 个人信息界面;
    [SerializeField] public GameObject 背景信息界面;

    [Header("按钮")]
    [SerializeField] public NovaSamples.UIControls.Button 确定;
    [SerializeField] public NovaSamples.UIControls.Button 取消;
    [SerializeField] public NovaSamples.UIControls.Button 返回;
    [SerializeField] public NovaSamples.UIControls.Button 下一步;
    [SerializeField] public NovaSamples.UIControls.Button 上一步;
    [SerializeField] public Color inputSetColor;

    I记贞 旧记;
    
    // 存储选项组性和选项组性多选
    private List<选项组性_茉> 选项组性列表 = new List<选项组性_茉>();
    private List<选项组性多选_茉> 选项组性多选列表 = new List<选项组性多选_茉>();

    private void Awake()
    {
        显示信息 += 显示信息详情;
        性别.Visuals.SelectionLabel.Color = Color.gray * 0.8f;
        性别.Visuals.SelectionLabel.Text = "请选择";
        编号.Text = 编号管理界面_茉.生成用户编号(); // 使用编号管理界面配置的规则生成编号
        
        // 初始化显示个人信息界面
        显示个人信息界面();
    }
    private void OnEnable()
    {
        旧记 = null;
        编号.Text = 编号管理界面_茉.生成用户编号(); // 使用编号管理界面配置的规则生成编号
        
        // 重新显示个人信息界面
        显示个人信息界面();
    }
    void Start()
    {
        // 按钮事件绑定
        this.确定?.OnClicked.AddListener(添加账号);
        this.取消?.OnClicked.AddListener(取消添加);
        this.返回?.OnClicked.AddListener(返回上个界面);
        this.下一步?.OnClicked.AddListener(() =>
        {
            验证并显示背景信息界面().Forget();
        });
        this.上一步?.OnClicked.AddListener(显示个人信息界面);
        
        // 个人信息文本框
        输入文本框点击(姓名);
        输入文本框点击(身份证);
        输入文本框点击(民族);
        
        // 家庭信息文本框
        输入文本框点击(监护人);
        输入文本框点击(关系);
        输入文本框点击(联系方式);
        输入文本框点击(居住地址);
        
        // 背景信息文本框
        if (文化程度 != null) 输入文本框点击(文化程度);
        if (血压 != null) 输入文本框点击(血压);
        if (家庭成员 != null) 输入文本框点击(家庭成员);
        if (发病前后语言状态 != null) 输入文本框点击(发病前后语言状态);
        if (发病时状况 != null) 输入文本框点击(发病时状况);
        if (既往病史 != null) 输入文本框点击(既往病史);
        
        // 下拉框
        性别等下拉选择框点击(性别);
        
        // 日期选择按钮
        出生日期选择按钮?.OnClicked.AddListener(() =>
        {
            选择出生日期().Forget();
        });
        发病日期选择按钮?.OnClicked.AddListener(() =>
        {
            选择发病日期().Forget();
        });
        
        // 初始化选项组性和选项组性多选
        初始化背景信息选项组();
    }
    private void 返回上个界面()
    {
        清空信息();
        var 界面管理器_韩 = FindObjectOfType<界面管理器_韩>();
        if (界面管理器_韩 != null)
        {
            界面管理器_韩.显示界面("用户管理显示");
        }
        else
        {
            Destroy(gameObject);
        }

    }
    async UniTaskVoid 选择出生日期()
    {
        var 择日器气 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("择日器");
        var 择日器 = Instantiate(择日器气, this.transform);
        择日器.GetComponent<择日器_茉>().On保存日期 += (年, 月, 日) =>
        {
            Debug.Log("出生日期选择");
            出生日期文本.Text = $"{年}-{月}-{日}";
        };
    }
    
    async UniTaskVoid 选择发病日期()
    {
        var 择日器气 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("择日器");
        var 择日器 = Instantiate(择日器气, this.transform);
        择日器.GetComponent<择日器_茉>().On保存日期 += (年, 月, 日) =>
        {
            Debug.Log("发病日期选择");
            发病日期文本.Text = $"{年}-{月}-{日}";
        };
    }
    
    /// <summary>
    /// 初始化背景信息的选项组性和选项组性多选
    /// </summary>
    void 初始化背景信息选项组()
    {
        // 获取所有子物体中的选项组性和选项组性多选组件
        选项组性列表.Clear();
        选项组性多选列表.Clear();
        
        选项组性列表.AddRange(GetComponentsInChildren<选项组性_茉>(true));
        选项组性多选列表.AddRange(GetComponentsInChildren<选项组性多选_茉>(true));
        
        Debug.Log($"[用户管理添加] 找到 {选项组性列表.Count} 个选项组性，{选项组性多选列表.Count} 个选项组性多选");
    }
    
    /// <summary>
    /// 验证并显示背景信息界面
    /// </summary>
    async UniTaskVoid 验证并显示背景信息界面()
    {
        // 验证姓名
        if (string.IsNullOrEmpty(姓名.Text))
        {
            await 显示提示("提示", "请先填写姓名");
            return;
        }
        
        // 验证通过，显示背景信息界面
        显示背景信息界面();
    }
    
    /// <summary>
    /// 显示个人信息界面
    /// </summary>
    void 显示个人信息界面()
    {
        if (个人信息界面 != null)
        {
            个人信息界面.SetActive(true);
            Debug.Log("[用户管理添加] 显示个人信息界面");
        }
        if (背景信息界面 != null)
        {
            背景信息界面.SetActive(false);
        }
    }
    
    /// <summary>
    /// 显示背景信息界面
    /// </summary>
    void 显示背景信息界面()
    {
        if (个人信息界面 != null)
        {
            个人信息界面.SetActive(false);
        }
        if (背景信息界面 != null)
        {
            背景信息界面.SetActive(true);
            Debug.Log("[用户管理添加] 显示背景信息界面");
        }
    }

    void 输入文本框点击(TextField 输入框)
    {
        TextFieldSelector selector = 输入框.GetComponent<TextFieldSelector>();
        selector.OnFocused += () =>
        {
            选中动画(true, 输入框.transform.parent.parent);
        };
        selector.OnFocusLost += () =>
        {
            选中动画(false, 输入框.transform.parent.parent);
        };

    }
    void 性别等下拉选择框点击(Dropdown 选择框)
    {
        选择框.Visuals.OnExpand += () =>
        {
            选中动画(true, 选择框.transform.parent);
        };
        选择框.Visuals.OnCollapse += () =>
        {
            选中动画(false, 选择框.transform.parent);
        };
        选择框.OnValueChanged.AddListener((_) =>
        {
            选择框.Visuals.SelectionLabel.Color = Color.black * 0.95f;
        });
    }

    void 选中动画(bool 是否选中, Transform 选中对象)
    {
        if (是否选中)
        {
            if (选中对象.GetComponent<UIBlock2D>().Color.a == 0)
            {
                DOTween.To((value) =>
               {
                   选中对象.GetComponent<UIBlock2D>().Color = new Color(inputSetColor.r, inputSetColor.g, inputSetColor.b, value);
               }, 0, 1, 0.3f);
            }
        }
        else
        {
            if (选中对象.GetComponent<UIBlock2D>().Color.a == 1)
            {
                DOTween.To((value) =>
                {
                    选中对象.GetComponent<UIBlock2D>().Color = new Color(inputSetColor.r, inputSetColor.g, inputSetColor.b, value);
                }, 1, 0, 0.3f);
            }
        }
    }
    void 添加账号()
    {
        验证并添加账号().Forget();
    }
    
    async UniTaskVoid 验证并添加账号()
    {
        // 验证选项组性是否都已选择
        List<string> 未填写的选项组 = new List<string>();
        foreach (var 选项组 in 选项组性列表)
        {
            if (选项组 != null && !string.IsNullOrEmpty(选项组.选项组名称))
            {
                if (选项组.当前选中选项 == null)
                {
                    未填写的选项组.Add(选项组.选项组名称);
                }
            }
        }
        
        if (未填写的选项组.Count > 0)
        {
            string 提示内容 = "背景资料不完整，是否确认保存";
            var 确认 = await 显示确认("提示", 提示内容);
            if (!确认)
            return;
        }

        添加信息();
        清空信息();
        FindObjectOfType<界面管理器_韩>().显示界面("用户管理显示");
    }

    /// <summary> 
    /// 显示提示告示
    /// </summary>
    private async UniTask 显示提示(string 标题, string 内容)
    {
        var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 告示实例 = Instantiate(告示, FindObjectOfType<界面管理器_韩>().transform);
        await 告示实例.GetComponent<告示_茉>().等待确认显示(标题, 内容);
        Destroy(告示实例.gameObject, 0.1f);
    }

    /// <summary>
    /// 显示确认
    /// </summary>
     private async UniTask<bool> 显示确认(string 标题, string 内容)
    {
        var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 告示实例 = Instantiate(告示, FindObjectOfType<界面管理器_韩>().transform);
        var 确认 = await 告示实例.GetComponent<告示_茉>().等待确认显示(标题, 内容);
        Destroy(告示实例.gameObject, 0.1f);
        return 确认;
    }
    void 取消添加()
    {
        Debug.Log("[ [3] 用户管理添加_韩]取消添加");
        清空信息();
        FindObjectOfType<界面管理器_韩>().显示界面("用户管理显示");

    }
    void 添加信息()
    {
        用户 新用户 = new 用户();
        
        // 个人信息
        新用户.编号 = 编号.Text;
        新用户.名 = 姓名.Text;
        新用户.性别 = 性别.DropdownOptions.Options[性别.DropdownOptions.SelectedIndex];
        新用户.身份证 = 身份证.Text;
        新用户.诞辰 = 出生日期文本.Text;
        新用户.民族 = 民族.Text;
        
        // 家庭信息
        新用户.监护人 = 监护人.Text;
        新用户.关系 = 关系.Text;
        新用户.联系方式 = 联系方式.Text;
        新用户.居住地址 = 居住地址.Text;
        
        // 背景信息 - TextField 直接输入的字段
        新用户.发病日期 = 发病日期文本 != null ? 发病日期文本.Text : "";
        新用户.文化程度 = 文化程度 != null ? 文化程度.Text : "";
        新用户.血压 = 血压 != null ? 血压.Text : "";
        新用户.家庭成员 = 家庭成员 != null ? 家庭成员.Text : "";
        新用户.发病前后语言状态 = 发病前后语言状态 != null ? 发病前后语言状态.Text : "";
        新用户.发病时状况 = 发病时状况 != null ? 发病时状况.Text : "";
        新用户.既往病史 = 既往病史 != null ? 既往病史.Text : "";
        新用户.神志状态 = 神志状态 != null ? 神志状态.Text : "";
        // 背景信息 - 从子物体中的选项组性获取
        新用户.背景信息选项组选择.Clear();
        
        // 保存所有选项组性的选择（格式："内容|索引"）
        foreach (var 选项组 in 选项组性列表)
        {
            if (选项组 != null && !string.IsNullOrEmpty(选项组.选项组名称) && 选项组.当前选中选项 != null)
            {
                新用户.背景信息选项组选择[选项组.选项组名称] = $"{选项组.当前选中名称}|{选项组.当前选中索引}";
                Debug.Log(选项组.选项组名称 + "：" + 新用户.背景信息选项组选择[选项组.选项组名称]);
                if (选项组.选项组名称 == "神志状态")
                    新用户.神志状态 = 选项组.当前选中名称;
            }
        }
        
        // 保存所有选项组性多选的选择（格式："内容1,内容2|索引1,索引2"）
        foreach (var 选项组 in 选项组性多选列表)
        {
            if (选项组 != null && !string.IsNullOrEmpty(选项组.选项组名称) && 选项组.选中数量 > 0)
            {
                新用户.背景信息选项组选择[选项组.选项组名称] = 
                    $"{string.Join(",", 选项组.当前选中名称列表)}|{string.Join(",", 选项组.当前选中索引列表)}";
            }
        }

        if (旧记 != null)
        {
            this.SendCommand(new 改记于录(旧记, 新用户, "用户录"));
        }
        else
        {
            this.SendCommand(new 增记于录(新用户, "用户录"));
        }
    }
    void 清空信息()
    {
        // 个人信息
        编号.Text = "";
        姓名.Text = "";
        性别.Visuals.SelectionLabel.Color = Color.gray * 0.8f;
        性别.Visuals.SelectionLabel.Text = "请选择";
        身份证.Text = "";
        出生日期文本.Text = "";
        民族.Text = "";
        
        // 家庭信息
        居住地址.Text = "";
        监护人.Text = "";
        关系.Text = "";
        联系方式.Text = "";
        
        // 背景信息
        if (发病日期文本 != null) 发病日期文本.Text = "";
        if (文化程度 != null) 文化程度.Text = "";
        if (血压 != null) 血压.Text = "";
        if (家庭成员 != null) 家庭成员.Text = "";
        if (发病前后语言状态 != null) 发病前后语言状态.Text = "";
        if (发病时状况 != null) 发病时状况.Text = "";
        if (既往病史 != null) 既往病史.Text = "";
        
        // 清空所有选项组性和选项组性多选的选择
        foreach (var 选项组 in 选项组性列表)
        {
            选项组?.清除所有选择();
        }
        foreach (var 选项组 in 选项组性多选列表)
        {
            选项组?.清除所有选择();
        }
    }
    void 显示信息详情(I记贞 记)
    {
        Debug.Log("[ [3] 用户管理添加_韩]显示信息详情");
        用户 用户 = 记 as 用户;
        
        // 个人信息
        编号.Text = 用户.编号;
        姓名.Text = 用户.名;
        性别.DropdownOptions.SelectedIndex = 用户.性别 == "男" ? 0 : 1;
        身份证.Text = 用户.身份证;
        出生日期文本.Text = 用户.诞辰;
        民族.Text = 用户.民族;
        
        // 家庭信息
        居住地址.Text = 用户.居住地址;
        监护人.Text = 用户.监护人;
        关系.Text = 用户.关系;
        联系方式.Text = 用户.联系方式;
        神志状态.Text = 用户.神志状态;
        // 背景信息 - TextField 直接输入的字段
        if (发病日期文本 != null) 发病日期文本.Text = 用户.发病日期 ?? "";
        if (文化程度 != null) 文化程度.Text = 用户.文化程度 ?? "";
        if (血压 != null) 血压.Text = 用户.血压 ?? "";
        if (家庭成员 != null) 家庭成员.Text = 用户.家庭成员 ?? "";
        if (发病前后语言状态 != null) 发病前后语言状态.Text = 用户.发病前后语言状态 ?? "";
        if (发病时状况 != null) 发病时状况.Text = 用户.发病时状况 ?? "";
        if (既往病史 != null) 既往病史.Text = 用户.既往病史 ?? "";
        
        // 背景信息 - 还原子物体中选项组性的选择
        if (用户.背景信息选项组选择 != null && 用户.背景信息选项组选择.Count > 0)
        {
            // 还原单选选项组
            foreach (var 选项组 in 选项组性列表)
            {
                if (选项组 != null && !string.IsNullOrEmpty(选项组.选项组名称) && 
                    用户.背景信息选项组选择.ContainsKey(选项组.选项组名称))
                {
                    var 数据 = 用户.背景信息选项组选择[选项组.选项组名称].Split('|');
                    if (数据.Length == 2 && int.TryParse(数据[1], out int 索引))
                        选项组.设置选中选项(索引);
                }
            }
            
            // 还原多选选项组
            foreach (var 选项组 in 选项组性多选列表)
            {
                if (选项组 != null && !string.IsNullOrEmpty(选项组.选项组名称) && 
                    用户.背景信息选项组选择.ContainsKey(选项组.选项组名称))
                {
                    var 数据 = 用户.背景信息选项组选择[选项组.选项组名称].Split('|');
                    if (数据.Length == 2)
                    {
                        var 索引列表 = new List<int>();
                        foreach (var 索引字符串 in 数据[1].Split(','))
                        {
                            if (int.TryParse(索引字符串, out int 索引))
                                索引列表.Add(索引);
                        }
                        选项组.批量设置选中选项(索引列表);
                    }
                }
            }
        }
        
        旧记 = 记;
    }

    protected override void OnShow()
    {
        Debug.Log("[ [3] 用户管理添加_韩]OnShow");
    }
    protected override void OnHide()
    {
        Debug.Log("[ [3] 用户管理添加_韩]OnHide");
    }
    void OnDestroy()
    {
        Debug.Log("[3]用户管理添加安卓_茉 OnDestroy - 清理键盘状态");
        
        // 强制关闭TouchScreenKeyboard
#if UNITY_ANDROID && !UNITY_EDITOR
        if (TouchScreenKeyboard.visible)
        {
            TouchScreenKeyboard.hideInput = true;
        }
#endif
        
        // 清理按钮事件
        this.取消?.OnClicked.RemoveAllListeners();
        this.确定?.OnClicked.RemoveAllListeners();
        this.返回?.OnClicked.RemoveAllListeners();
        this.下一步?.OnClicked.RemoveAllListeners();
        this.上一步?.OnClicked.RemoveAllListeners();
    }
}
