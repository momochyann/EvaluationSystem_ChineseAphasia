using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
using QFramework;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using NovaSamples.UIControls;
using Cysharp.Threading.Tasks;
using System.Linq;

// 登录界面逻辑类，实现IController接口
public class 登录界面_茉 : MonoBehaviour, IController
{


    [SerializeField] TextField 用户名;  // 用户名输入框
    [SerializeField] TextField 密码;     // 密码输入框
    [SerializeField] TextBlock 密码inputDIs;     // 密码输入框
    [SerializeField] NovaSamples.UIControls.Button 登录按键;    // 登录按钮
    [SerializeField] NovaSamples.UIControls.Button 退出按键;    // 退出按钮

    // 图标相关组件
    [SerializeField] Sprite 正确图标;
    [SerializeField] Sprite 错误图标;
    [SerializeField] UIBlock2D 用户名图标显示;  // 用于显示用户名验证图标的UI组件
    [SerializeField] UIBlock2D 密码图标显示;    // 用于显示密码验证图标的UI组件

    // 占位符文本组件
    [SerializeField] TextBlock 用户名占位符;    // 用户名输入框的占位符文本
    [SerializeField] TextBlock 密码占位符;      // 密码输入框的占位符文本

    private bool _needUpdateCursor = false;  // 标记是否需要更新光标位置
    private bool _isUpdating = false;        // 防止事件递归的标记

    // 保存原始的占位符文本
    private string 用户名原始占位符文本;
    private string 密码原始占位符文本;

    // // 隐藏密码属性（管理真实密码的读写）
    string 输入数据 = "";
    string 显示数据
    {
        get
        {
            string result = "";
            for (int i = 0; i < 输入数据.Length; i++)
            {
                result += "*";
            }
            return result;
        }
    }

    // 初始化方法
    void Start()
    {
        // 保存原始的占位符文本
        用户名原始占位符文本 = 用户名占位符.Text;
        密码原始占位符文本 = 密码占位符.Text;

        // 初始化图标为隐藏状态
        用户名图标显示?.Hide();
        密码图标显示?.Hide();

        // 禁止用户名和密码输入框的换行符输入（针对安卓端）
        SetTextFieldDisallowNewlines(用户名);
        SetTextFieldDisallowNewlines(密码);

        // 订阅密码输入变化事件
        密码.OnTextChanged += On密码变化;
        // 订阅用户名输入变化事件，当用户开始输入时隐藏错误提示
        用户名.OnTextChanged += On用户名变化;
        Debug.Log("[0]登录界面_韩 Start");

        // 绑定登录按钮点击事件
        登录按键?.OnClicked.AddListener(登录);
        // 绑定退出按钮点击事件
        退出按键?.OnClicked.AddListener(退出应用程序);

        var 评人录 = this.GetModel<评人录>();
        var 管员录 = this.GetModel<管员录>();
        评人录.登陆评人 = null;
        管员录.登陆管理员 = null;
    }
    char lastChar = ' ';
    int lastIndex = -1;

    // 用户名输入变化时触发，隐藏错误提示
    private void On用户名变化()
    {
        // 过滤换行符、回车符和制表符
        string filteredText = FilterInvalidChars(用户名.Text);
        if (filteredText != 用户名.Text)
        {
            用户名.Text = filteredText;
            return; // 避免递归调用
        }

        // 当用户开始输入用户名时，隐藏所有错误提示和图标
        if (用户名.Text != "")
            用户名图标显示?.Hide();

        // 恢复原始占位符文本
        用户名占位符.Text = 用户名原始占位符文本;
        用户名占位符.Color = new Color(0.6f, 0.6f, 0.6f, 1f); // 恢复正常的占位符颜色

    }

    // 密码输入变化时触发
    private void On密码变化()
    {
        // 过滤换行符、回车符和制表符
        string filteredText = FilterInvalidChars(密码.Text);
        if (filteredText != 密码.Text)
        {
            密码.Text = filteredText;
            return; // 避免递归调用
        }

        输入数据 = 密码.Text;
        密码inputDIs.Text = 显示数据;

        // 当用户开始输入密码时，隐藏所有错误提示和图标
        if (密码.Text != "")
            密码图标显示?.Hide();

        // 恢复原始占位符文本
        密码占位符.Text = 密码原始占位符文本;
        密码占位符.Color = new Color(0.6f, 0.6f, 0.6f, 1f); // 恢复正常的占位符颜色
    }

    // 过滤无效字符的方法
    private string FilterInvalidChars(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        string result = "";
        foreach (char c in input)
        {
            // 过滤换行符(\n)、回车符(\r)、制表符(\t)
            if (c != '\n' && c != '\r' && c != '\t' && c != '\0')
            {
                result += c;
            }
        }
        return result;
    }

    // 设置TextField禁止换行符输入（安卓端解决方案）
    private void SetTextFieldDisallowNewlines(TextField textField)
    {
        if (textField == null) return;

        // 查找TextField游戏对象上的所有TextFieldInputProvider组件
        var inputProviders = textField.GetComponents<NovaSamples.UIControls.TextFieldInputProvider>();

        foreach (var provider in inputProviders)
        {
            // 使用反射设置allowNewlines字段为false
            var field = provider.GetType().GetField("allowNewlines",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (field != null)
            {
                field.SetValue(provider, false);
                Debug.Log($"[0]登录界面_韩 已禁用{textField.name}的换行符输入");
            }
        }
    }


    // 登录逻辑
    void 登录()
    {

        用户名图标显示?.Hide();
        密码图标显示?.Hide();

        // 第一步：验证输入框是否为空
        bool 用户名为空 = string.IsNullOrEmpty(用户名.Text);
        bool 密码为空 = string.IsNullOrEmpty(输入数据);

        if (用户名为空)
        {
            Debug.Log("[0]登录界面_韩 用户名不能为空");
            // 显示用户名错误状态

            if (用户名图标显示 != null && 错误图标 != null)
            {
                Debug.Log("用户名图标显示");
                用户名图标显示.SetImage(错误图标);
                用户名图标显示.Show();
            }
            // 清空输入框并在占位符显示错误信息
            用户名.Text = "";
            用户名占位符.Text = "用户名不能为空";
            用户名占位符.Color = Color.red;
            return;
        }

        if (密码为空)
        {
            Debug.Log("[0]登录界面_韩 密码不能为空");
            // 显示密码错误状态

            if (密码图标显示 != null && 错误图标 != null)
            {
                Debug.Log("密码图标显示");
                密码图标显示.SetImage(错误图标);
                密码图标显示.Show();
            }
            // 清空输入框并在占位符显示错误信息
            密码.Text = "";
            输入数据 = "";
            密码inputDIs.Text = "";
            密码占位符.Text = "密码不能为空";
            密码占位符.Color = Color.red;
            return;
        }

        // 第二步：验证账号和密码
        var 评人录 = this.GetModel<评人录>();
        var 管员录 = this.GetModel<管员录>();
        var 评人 = 评人录.GetDataList().Find(x => x.账号 == 用户名.Text);
        var 管理员 = 管员录.GetDataList().Find(x => x.账号 == 用户名.Text);

        bool 用户存在 = 评人 != null || 管理员 != null;
        bool 密码正确 = false;

        if (管理员 != null)
        {
            // 验证管理员密码
            密码正确 = 管理员.密码 == 输入数据;
            if (密码正确)
            {
                管员录.登陆管理员 = 管理员;
                Debug.Log($"[0]登录界面_韩 管理员登录成功：{管理员.名}");
            }
        }
        else if (评人 != null)
        {
            // 验证评定人密码
            密码正确 = 评人.密码 == 输入数据;
            if (密码正确)
            {
                评人录.登陆评人 = 评人;
                Debug.Log($"[0]登录界面_韩 评定人登录成功：{评人.名}");
            }
        }

        // 根据验证结果显示对应图标
        if (!用户存在)
        {
            // 用户不存在
            Debug.Log("[0]登录界面_韩 用户不存在");

            if (用户名图标显示 != null && 错误图标 != null)
            {

                用户名图标显示.SetImage(错误图标);
                用户名图标显示.Show();
            }
            // 清空用户名框并显示错误信息
            用户名.Text = "";
            用户名占位符.Text = "用户不存在";
            用户名占位符.Color = Color.red;
        }
        else if (!密码正确)
        {
            // 用户存在但密码错误 - 用户名显示正确图标，密码显示错误图标
            Debug.Log("[0]登录界面_韩 密码错误");


            // 用户名正确，显示正确图标
            if (用户名图标显示 != null && 正确图标 != null)
            {
                用户名图标显示.Show();
                用户名图标显示.SetImage(正确图标);

            }

            // 密码错误，显示错误图标
            if (密码图标显示 != null && 错误图标 != null)
            {
                密码图标显示.SetImage(错误图标);
                密码图标显示.Show();
            }

            // 清空密码框并显示错误信息
            密码.Text = "";
            输入数据 = "";
            密码inputDIs.Text = "";
            密码占位符.Text = "密码错误";
            密码占位符.Color = Color.red;
        }
        else
        {
            // 登录成功 - 不显示图标，直接跳转
            Debug.Log("[0]登录界面_韩 登录成功");
            跳转界面("选择场景");
        }
    }

    // 退出应用程序
    async void 退出应用程序()
    {
        Debug.Log("[0]登录界面_韩 用户点击退出按钮");
        var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 告示实例 = Instantiate(告示, transform.parent);
        var 告示组件 = 告示实例.GetComponent<告示_茉>();
        var 确认 = await 告示组件.等待确认显示("提示", "确定要退出应用程序吗？");
        if (确认)
        {

            #if UNITY_EDITOR
                        // 在Unity编辑器中停止播放
                        UnityEditor.EditorApplication.isPlaying = false;
                        Debug.Log("[0]登录界面_韩 在编辑器中停止播放");
            #elif UNITY_IOS
                    // iOS不允许程序自己退出，这是苹果的政策
                    Debug.Log("[0]登录界面_韩 iOS平台不支持程序退出");
            #else
                    // 安卓、PC等其他平台
                    Application.Quit();
                    Debug.Log("[0]登录界面_韩 应用程序已退出");
            #endif
        }
        else
        {
            Destroy(告示实例);
        }

    }

    // 界面跳转逻辑
    async void 跳转界面(string 界面名称)
    {
        Debug.Log("[0]登录界面_韩 跳转界面:" + 界面名称);
        await UniTask.Yield();
        LoadYooAssetsTool.LoadSceneAsync(界面名称).Forget();

    }



    // 组件销毁时的清理工作
    void OnDestroy()
    {
        Debug.Log("[0]登录界面_茉 OnDestroy - 清理键盘状态");
        
        // 强制关闭TouchScreenKeyboard
#if UNITY_ANDROID && !UNITY_EDITOR
        if (TouchScreenKeyboard.visible)
        {
            TouchScreenKeyboard.hideInput = true;
        }
#endif
        
        // 移除事件监听
        if (密码 != null)
            密码.OnTextChanged -= On密码变化;
        if (用户名 != null)
            用户名.OnTextChanged -= On用户名变化;
        
        // 移除按钮监听
        登录按键?.OnClicked.RemoveAllListeners();
        退出按键?.OnClicked.RemoveAllListeners();
    }

    // 实现IController接口，返回架构实例
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
