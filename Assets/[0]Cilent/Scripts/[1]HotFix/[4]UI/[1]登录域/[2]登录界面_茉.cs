using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
using QFramework;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using NovaSamples.UIControls;
using Cysharp.Threading.Tasks;
using System.Linq;
// 登录界面逻辑类，实现IController接口
public class 登录界面_茉 : MonoBehaviour, IController
{

    // Start在第一帧更新前调用
    [SerializeField] NovaSamples.UIControls.TextField 用户名;  // 用户名输入框
    [SerializeField] NovaSamples.UIControls.TextField 密码;     // 密码输入框
    [SerializeField] TextBlock 密码inputDIs;     // 密码输入框
    [SerializeField] NovaSamples.UIControls.Toggle 记住密码;   // 记住密码复选框
    [SerializeField] NovaSamples.UIControls.Toggle 自动登录;   // 自动登录复选框
    [SerializeField] NovaSamples.UIControls.Button 登录按键;    // 登录按钮
    private bool _needUpdateCursor = false;  // 标记是否需要更新光标位置
    private bool _isUpdating = false;        // 防止事件递归的标记

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
        // 订阅密码输入变化事件
        密码.OnTextChanged += On密码变化;
        Debug.Log("[0]登录界面_韩r Start");

        // 绑定登录按钮点击事件
        登录按键?.OnClicked.AddListener(登录);
        // 绑定自动登录复选框事件
        自动登录?.OnToggled.AddListener(是否自动登录);
        // 绑定记住密码复选框事件
        记住密码?.OnToggled.AddListener(是否记住密码);

        // 读取本地存储的登录状态
        if (PlayerPrefs.GetString("自动登录") == "true")
        {
            自动登录.ToggledOn = true;
            用户名.Text = PlayerPrefs.GetString("用户名");
            // 从本地读取的密码需要转换为星号显示
            var realPwd = PlayerPrefs.GetString("密码");
            输入数据 = realPwd;
            密码inputDIs.Text = 显示数据;
            登录();  // 自动登录
        }
        else if (PlayerPrefs.GetString("记住密码") == "true")
        {
            记住密码.ToggledOn = true;
            用户名.Text = PlayerPrefs.GetString("用户名");
            // 从本地读取的密码需要转换为星号显示
            var realPwd = PlayerPrefs.GetString("密码");
            输入数据 = realPwd;
            密码inputDIs.Text = 显示数据;
        }
        else
        {
            // 清除本地存储的用户名和密码
            PlayerPrefs.SetString("用户名", "");
            PlayerPrefs.SetString("密码", "");
            输入数据 = "";
        }
    }
    char lastChar = ' ';
    int lastIndex = -1;
    // 密码输入变化时触发
    private void On密码变化()
    {
        输入数据 = 密码.Text;
        密码inputDIs.Text = 显示数据;
    }



    // 自动登录状态变化处理（保留原有逻辑）
    void 是否自动登录(bool 是否)
    {
        int index = 密码.CursorPosition.Index;
        bool isRightSide = 密码.CursorPosition.IsRightSide;
        Debug.Log("[0]登录界面_韩 index:" + index);
        Debug.Log("[0]登录界面_韩 isRightSide:" + (isRightSide == true ? "右边" : "左边"));
        if (是否)
        {
            Debug.Log("[0]登录界面_韩 设置自动登录为true");
            PlayerPrefs.SetString("自动登录", "true");

            Debug.Log("[0]登录界面_韩 设置记住密码为true");
            PlayerPrefs.SetString("记住密码", "true");
            记住密码.ToggledOn = true;

        }
        else
        {
            Debug.Log("[0]登录界面_韩 设置自动登录为false");
            PlayerPrefs.SetString("自动登录", "false");
        }
    }

    // 记住密码状态变化处理（保留原有逻辑）
    void 是否记住密码(bool 是否)
    {
        if (是否)
        {
            Debug.Log("[0]登录界面_韩 设置记住密码为true");
            PlayerPrefs.SetString("记住密码", "true");
        }
        else
        {
            Debug.Log("[0]登录界面_韩 设置记住密码为false");
            PlayerPrefs.SetString("记住密码", "false");
        }
    }

    // 登录逻辑
    void 登录()
    {
        if (用户名.Text == "")
        {
            Debug.Log("[0]登录界面_韩 用户名不能为空");
            return;
        }
        // 验证密码时使用隐藏的真实密码
        else if (string.IsNullOrEmpty(输入数据))
        {
            Debug.Log("[0]登录界面_韩 密码不能为空");
            return;
        }
        else
        {
            var 评人录 = this.GetModel<评人录>();
            var 评人 = 评人录.GetDataList().Find(x => x.账号 == 用户名.Text);

            if (评人 != null)   // 判断用户是否存在
            {
                // 使用真实密码进行验证
                if (评人.密码 == 输入数据)
                {
                    Debug.Log("[0]登录界面_韩 登录成功");
                    PlayerPrefs.SetString("用户名", 用户名.Text);
                    // 存储真实密码到本地
                    PlayerPrefs.SetString("密码", 输入数据);
                    跳转界面("选择场景");
                }
                else
                {
                    Debug.Log("[0]登录界面_韩 密码错误");
                }
            }
            else
            {
                // 管理员账号验证
                if (用户名.Text == "admin" && 输入数据 == "123456")
                {
                    Debug.Log("[0]登录界面_韩 管理员登录成功");
                    跳转界面("选择场景");
                }
                else
                {
                    Debug.Log("[0]登录界面_韩 用户不存在");
                }
            }
        }
    }

    // 界面跳转逻辑
    async void 跳转界面(string 界面名称)
    {
        Debug.Log("[0]登录界面_韩 跳转界面:" + 界面名称);
        var prefab = await LoadYooAssetsTool.LoadAsset<GameObject>("界面加载动画");
        var 加载动画 = Instantiate(prefab, this.transform.parent);

        // 等待加载动画完成
        while (加载动画.GetComponent<加载动画_韩>().加载动画状态 == 加载动画状态.加载中)
        {
            await UniTask.Delay(100);
        }

        if (加载动画.GetComponent<加载动画_韩>().加载动画状态 == 加载动画状态.取消)
        {
            Debug.Log("[0]登录界面_韩 取消加载");
            return;
        }
        else if (加载动画.GetComponent<加载动画_韩>().加载动画状态 == 加载动画状态.完成)
        {
            Destroy(加载动画);
            await UniTask.Delay(100);
            LoadYooAssetsTool.LoadSceneAsync(界面名称).Forget();
        }
    }



    // 实现IController接口，返回架构实例
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
