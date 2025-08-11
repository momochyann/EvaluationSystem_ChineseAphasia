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

// 登录界面逻辑类，实现IController接口
public class 登录界面_韩 : MonoBehaviour, IController
{
    
    // Start在第一帧更新前调用
    [SerializeField] NovaSamples.UIControls.TextField 用户名;  // 用户名输入框
    [SerializeField] NovaSamples.UIControls.TextField 密码;     // 密码输入框
    [SerializeField] NovaSamples.UIControls.Toggle 记住密码;   // 记住密码复选框
    [SerializeField] NovaSamples.UIControls.Toggle 自动登录;   // 自动登录复选框
    [SerializeField] NovaSamples.UIControls.Button 登录按键;    // 登录按钮
    private bool _needUpdateCursor = false;  // 标记是否需要更新光标位置
    private string _隐藏密码文本;            // 存储真实密码的私有字段
    private string _显示密码文本;            // 存储显示用星号的私有字段
    private bool _isUpdating = false;        // 防止事件递归的标记

    // 隐藏密码属性（管理真实密码的读写）
    private string 隐藏密码文本
    {
        get => _隐藏密码文本;  // 获取真实密码
        set
        {
            // 只有当值发生变化时才更新
            if (_隐藏密码文本 != value)
            {
                _隐藏密码文本 = value;
                // 同步更新显示文本为对应长度的星号
                更新显示文本();
            }
        }
    }

    // 显示密码属性（管理UI显示的星号文本）
    private string 显示密码文本
    {
        get => _显示密码文本;  // 获取显示用的星号文本
        set 
        {
            // 避免重复设置相同值
            if (_显示密码文本 != value)
            {
                _显示密码文本 = value;
                // 防止自身更新触发事件循环
                if (!_isUpdating)
                {
                    _isUpdating = true;
                    密码.Text = value;  // 更新输入框显示
                    _isUpdating = false;
                }
            }
        }
    }

    // 初始化方法
    void Start()
    {
        // 订阅密码输入变化事件
        密码.OnTextChanged += On密码变化;
        Debug.Log("[0]登录界面_韩 Start");
        
        // 绑定登录按钮点击事件
        登录按键?.OnClicked.AddListener(登录);
        // 绑定自动登录复选框事件
        自动登录?.OnToggled.AddListener(是否自动登录);
        // 绑定记住密码复选框事件
        记住密码?.OnToggled.AddListener(是否记住密码);
        
        // 将光标移动到密码框末尾
        密码.MoveCursor(密码.CursorPosition.MoveToEnd(), false);
        
        // 读取本地存储的登录状态
        if (PlayerPrefs.GetString("自动登录") == "true")
        {
            自动登录.ToggledOn = true;
            用户名.Text = PlayerPrefs.GetString("用户名");
            // 从本地读取的密码需要转换为星号显示
            var realPwd = PlayerPrefs.GetString("密码");
            _隐藏密码文本 = realPwd;
            显示密码文本 = new string('*', realPwd.Length);
            登录();  // 自动登录
        }
        else if (PlayerPrefs.GetString("记住密码") == "true")
        {
            记住密码.ToggledOn = true;
            用户名.Text = PlayerPrefs.GetString("用户名");
            // 从本地读取的密码需要转换为星号显示
            var realPwd = PlayerPrefs.GetString("密码");
            _隐藏密码文本 = realPwd;
            显示密码文本 = new string('*', realPwd.Length);
        }
        else 
        {
            // 清除本地存储的用户名和密码
            PlayerPrefs.SetString("用户名", "");
            PlayerPrefs.SetString("密码", "");
            _隐藏密码文本 = "";
            显示密码文本 = "";
        }
    }

    // 密码输入变化时触发
    private void On密码变化()
    {
        // 忽略自身更新导致的事件
        if (_isUpdating)
            return;
        _needUpdateCursor = true;
        
    }

    // 更新显示文本为与真实密码长度相同的星号
    private void 更新显示文本()
    {
        int length = 隐藏密码文本?.Length ?? 0;
        显示密码文本 = new string('*', length);
    }

    // 自动登录状态变化处理（保留原有逻辑）
    void 是否自动登录(bool 是否)
    {
        int index = 密码.CursorPosition.Index;
        bool isRightSide = 密码.CursorPosition.IsRightSide;
        Debug.Log("[0]登录界面_韩 index:" + index);
        Debug.Log("[0]登录界面_韩 isRightSide:" + (isRightSide==true?"右边":"左边"));
        if(是否)
        {
            Debug.Log("[0]登录界面_韩 设置自动登录为true");
            PlayerPrefs.SetString("自动登录", "true");

            Debug.Log("[0]登录界面_韩 设置记住密码为true");
            PlayerPrefs.SetString("记住密码", "true");
            记住密码.ToggledOn = true;

        }else 
        {
            Debug.Log("[0]登录界面_韩 设置自动登录为false");
            PlayerPrefs.SetString("自动登录", "false");
        }
    }

    // 记住密码状态变化处理（保留原有逻辑）
    void 是否记住密码(bool 是否)
    {
        if(是否)
        {
            Debug.Log("[0]登录界面_韩 设置记住密码为true");
            PlayerPrefs.SetString("记住密码", "true");
        }else 
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
        else if (string.IsNullOrEmpty(隐藏密码文本))
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
                if (评人.密码 == 隐藏密码文本)
                {
                    Debug.Log("[0]登录界面_韩 登录成功");
                    PlayerPrefs.SetString("用户名", 用户名.Text);
                    // 存储真实密码到本地
                    PlayerPrefs.SetString("密码", 隐藏密码文本);
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
                if(用户名.Text == "admin" && 隐藏密码文本 == "123456")
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
        var 加载动画 = Instantiate(prefab,this.transform.parent);
        
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

    // 延迟更新光标位置（解决同步问题）
    void LateUpdate()
    {
        if (_needUpdateCursor)
        {
            Debug.Log("[0]登录界面_韩 密码变化:" + 密码.Text);
            _needUpdateCursor = false;

            // 获取当前输入的显示文本（星号）
            string newDisplayText = 密码.Text;
            // 原真实密码长度
            int previousLength = 隐藏密码文本?.Length ?? 0;
            // 新输入的显示文本长度
            int newLength = newDisplayText.Length;

            // 计算光标位置（用于插入操作）
            int cursorIndex = 密码.CursorPosition.StringIndex;

            if (newLength > previousLength)
            {
                // 有新字符输入：计算新增字符数
                int addCount = newLength - previousLength;
                // 获取用户实际输入的字符（从光标位置向前取新增数量）
                string addedChars = newDisplayText.Substring(cursorIndex - addCount, addCount);
                
                // 在真实密码的对应位置插入新字符
                if (string.IsNullOrEmpty(隐藏密码文本))
                {
                    隐藏密码文本 = addedChars;
                }
                else
                {
                    隐藏密码文本 = 隐藏密码文本.Insert(cursorIndex - addCount, addedChars);
                }
            }
            else if (newLength < previousLength)
            {
                // 有字符被删除：计算删除字符数
                int deleteCount = previousLength - newLength;
                
                // 从真实密码的对应位置删除字符
                if (previousLength > 0 && cursorIndex <= 隐藏密码文本.Length)
                {
                    隐藏密码文本 = 隐藏密码文本.Remove(cursorIndex, deleteCount);
                }
                else
                {
                    隐藏密码文本 = "";
                }
            }
        }
    }

    public void ShowAndroidKeyboard()
    {
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass inputMethodManager = new AndroidJavaClass("android.view.inputmethod.InputMethodManager");
            
            activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject inputMethod = inputMethodManager.CallStatic<AndroidJavaObject>(
                    "getInstance", activity);
                AndroidJavaObject window = activity.Call<AndroidJavaObject>("getWindow");
                AndroidJavaObject decorView = window.Call<AndroidJavaObject>("getDecorView");
                inputMethod.Call("showSoftInput", decorView, 0);
            }));
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to show keyboard: " + e.Message);
        }
    }
    // 实现IController接口，返回架构实例
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
