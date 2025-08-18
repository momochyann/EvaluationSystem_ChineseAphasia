using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System;
using Nova;

public class 评定人管理添加_韩 : 界面基础_韩
{
    // Start is called before the first frame update
    [SerializeField] public NovaSamples.UIControls.TextField 登录账号;
    [SerializeField] public NovaSamples.UIControls.TextField 登录密码;
    [SerializeField] public NovaSamples.UIControls.TextField 姓名;
    [SerializeField] public NovaSamples.UIControls.TextField 性别;
    [SerializeField] public NovaSamples.UIControls.TextField 联系方式;
    [SerializeField] public NovaSamples.UIControls.TextField 身份证;
    [SerializeField] public TextBlock 出生日期;
    [SerializeField] public NovaSamples.UIControls.Button 出生日期选择;
    [SerializeField] public NovaSamples.UIControls.Button 确定;
    [SerializeField] public NovaSamples.UIControls.Button 取消;
    [SerializeField] public NovaSamples.UIControls.Button 临时添加;
    
    [Header("万年历组件")]
    [SerializeField] public 滚轮万年历管理器 万年历管理器;
    
    // 万年历实例管理
    private GameObject 当前万年历实例;
    private 滚轮万年历管理器 当前万年历管理器;
    
    void Start()
    {
        登录账号.Text = "";
        登录密码.Text = "";
        姓名.Text = "";
        性别.Text = "";
        联系方式.Text = "";
        身份证.Text = "";
        出生日期.Text = "";
        出生日期选择.OnClicked.AddListener(万年历);
        确定.OnClicked.AddListener(() => 添加账号());
        取消.OnClicked.AddListener(() => 取消添加());
        临时添加.OnClicked.AddListener(临时添加信息);
        
        // 绑定万年历事件
        绑定万年历事件();
    }
    
    /// <summary>
    /// 绑定万年历事件
    /// </summary>
    void 绑定万年历事件()
    {
        // 注释掉原来的绑定，因为我们现在动态创建实例
        // if (万年历管理器 != null)
        // {
        //     万年历管理器.On日期确认 += On万年历日期确认;
        //     万年历管理器.On日期取消 += On万年历日期取消;
        // }
        // else
        // {
        //     Debug.LogWarning("[评定人管理添加_韩] 万年历管理器引用为空，请在Inspector中设置！");
        // }
    }
    
    /// <summary>
    /// 显示万年历选择出生日期
    /// </summary>
    async void 万年历()
    {
        // 如果已经有万年历实例，先销毁它
        if (当前万年历实例 != null)
        {
            Destroy(当前万年历实例);
            当前万年历实例 = null;
            当前万年历管理器 = null;
        }
        
        try
        {
            // 加载万年历预制体
            var prefab = await LoadYooAssetsTool.LoadAsset<GameObject>("万年历_PC");
            
            // 创建实例并设置到Canvas下
            var canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                当前万年历实例 = Instantiate(prefab, canvas.transform);
            }
            else
            {
                当前万年历实例 = Instantiate(prefab, this.transform.parent);
            }
            
            // 获取万年历管理器组件
            当前万年历管理器 = 当前万年历实例.GetComponent<滚轮万年历管理器>();
            
            if (当前万年历管理器 != null)
            {
                // 绑定事件
                当前万年历管理器.On日期确认 += On万年历日期确认;
                当前万年历管理器.On日期取消 += On万年历日期取消;
                
                // 如果出生日期字段已有内容，尝试解析并设置为当前选择
                if (!string.IsNullOrEmpty(出生日期.Text))
                {
                    try
                    {
                        // 尝试解析现有日期格式（假设格式为 yyyy-MM-dd 或 yyyy年MM月dd日）
                        DateTime 现有日期;
                        if (DateTime.TryParse(出生日期.Text, out 现有日期))
                        {
                            当前万年历管理器.设置选择日期(现有日期);
                        }
                        else
                        {
                            // 尝试解析中文格式
                            string 日期文本 = 出生日期.Text.Replace("年", "-").Replace("月", "-").Replace("日", "");
                            if (DateTime.TryParse(日期文本, out 现有日期))
                            {
                                当前万年历管理器.设置选择日期(现有日期);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"[评定人管理添加_韩] 解析现有日期失败: {e.Message}");
                    }
                }
                
                // 显示万年历
                当前万年历管理器.显示万年历();
                Debug.Log("[评定人管理添加_韩] 万年历已显示");
            }
            else
            {
                Debug.LogError("[评定人管理添加_韩] 万年历预制体上没有找到滚轮万年历管理器组件！");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[评定人管理添加_韩] 加载万年历失败: {e.Message}");
        }

        // if (万年历管理器 != null)
        // {
        //     // 如果出生日期字段已有内容，尝试解析并设置为当前选择
        //     if (!string.IsNullOrEmpty(出生日期.Text))
        //     {
        //         try
        //         {
        //             // 尝试解析现有日期格式（假设格式为 yyyy-MM-dd 或 yyyy年MM月dd日）
        //             DateTime 现有日期;
        //             if (DateTime.TryParse(出生日期.Text, out 现有日期))
        //             {
        //                 万年历管理器.设置选择日期(现有日期);
        //             }
        //             else
        //             {
        //                 // 尝试解析中文格式
        //                 string 日期文本 = 出生日期.Text.Replace("年", "-").Replace("月", "-").Replace("日", "");
        //                 if (DateTime.TryParse(日期文本, out 现有日期))
        //                 {
        //                     万年历管理器.设置选择日期(现有日期);
        //                 }
        //             }
        //         }
        //         catch (Exception e)
        //         {
        //             Debug.LogWarning($"[评定人管理添加_韩] 解析现有日期失败: {e.Message}");
        //         }
        //     }
            
        //     // 显示万年历
        //     万年历管理器.显示万年历();
        //     Debug.Log("[评定人管理添加_韩] 万年历已显示");
        // }
        // else
        // {
        //     Debug.LogError("[评定人管理添加_韩] 万年历管理器引用为空，无法显示万年历！");
        // }
    }
    
    /// <summary>
    /// 万年历日期确认回调
    /// </summary>
    void On万年历日期确认(DateTime 选择日期)
    {
        // 更新出生日期显示
        出生日期.Text = 选择日期.ToString("yyyy-MM-dd");
        
        // 隐藏万年历并清理实例
        if (当前万年历管理器 != null)
        {
            当前万年历管理器.隐藏万年历();
        }
        
        // 销毁实例
        if (当前万年历实例 != null)
        {
            Destroy(当前万年历实例);
            当前万年历实例 = null;
            当前万年历管理器 = null;
        }
        
        Debug.Log($"[评定人管理添加_韩] 出生日期已选择: {选择日期:yyyy年MM月dd日}");
    }
    
    /// <summary>
    /// 万年历日期取消回调
    /// </summary>
    void On万年历日期取消()
    {
        // 隐藏万年历并清理实例
        if (当前万年历管理器 != null)
        {
            当前万年历管理器.隐藏万年历();
        }
        
        // 销毁实例
        if (当前万年历实例 != null)
        {
            Destroy(当前万年历实例);
            当前万年历实例 = null;
            当前万年历管理器 = null;
        }
        
        Debug.Log("[评定人管理添加_韩] 万年历日期选择已取消");
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
        新评人.性别 = 性别.Text;
        新评人.电话 = 联系方式.Text;
        新评人.身份证 = 身份证.Text;
        新评人.诞辰 = 出生日期.Text;
        this.SendCommand(new 增记于录(新评人, "评人录"));
        Debug.Log("[ [2] 评定人管理添加_韩]添加账号" + 新评人.名);
        清空文本框内容();
        FindObjectOfType<界面管理器_韩>().显示界面("评定人管理显示_PC");
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
    void 临时添加信息()
    {
        评人 新评人 = new 评人();
        新评人.账号 = "admin";
        新评人.密码 = "123456";
        新评人.名 = "张三";
        新评人.性别 = "男";
        新评人.电话 = "1234567890";
        新评人.身份证 = "1234567890";
        新评人.诞辰 = "2020-01-01";
        this.SendCommand(new 增记于录(新评人, "评人录"));
        Debug.Log("[ [2] 评定人管理添加_韩]临时添加信息" + 新评人.名);
        FindObjectOfType<界面管理器_韩>().显示界面("评定人管理显示_PC");
    }
    public void 清空文本框内容()
    {
        登录账号.Text = "";
        登录密码.Text = "";
        姓名.Text = "";
        性别.Text = "";
        联系方式.Text = "";
        身份证.Text = "";
        出生日期.Text = "";
    }
    void OnDestroy()
    {
        this.取消.OnClicked.RemoveAllListeners();
        this.确定.OnClicked.RemoveAllListeners();
        this.临时添加.OnClicked.RemoveAllListeners();
        
        // 清理万年历事件绑定
        if (当前万年历管理器 != null)
        {
            当前万年历管理器.On日期确认 -= On万年历日期确认;
            当前万年历管理器.On日期取消 -= On万年历日期取消;
        }
    }

}
