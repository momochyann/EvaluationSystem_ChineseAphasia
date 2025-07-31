using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
using QFramework;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class 登录界面_韩 : MonoBehaviour, IController
{
    
    // Start is called before the first frame update
    [SerializeField] NovaSamples.UIControls.TextField 用户名;
    [SerializeField] NovaSamples.UIControls.TextField 密码;
    [SerializeField] NovaSamples.UIControls.Toggle 记住密码;
    [SerializeField] NovaSamples.UIControls.Toggle 自动登录;
    [SerializeField] NovaSamples.UIControls.Button 登录按键;
   
    
    void Start()
    {
        Debug.Log("[0]登录界面_韩 Start");
        登录按键?.OnClicked.AddListener(登录);
        自动登录?.OnToggled.AddListener(是否自动登录);
        记住密码?.OnToggled.AddListener(是否记住密码);
        
        if (PlayerPrefs.GetString("自动登录") == "true")
        {
            自动登录.ToggledOn = true;
            用户名.Text = PlayerPrefs.GetString("用户名");
            密码.Text = PlayerPrefs.GetString("密码");
            登录();
        }else if (PlayerPrefs.GetString("记住密码") == "true")
        {
            记住密码.ToggledOn = true;
            用户名.Text = PlayerPrefs.GetString("用户名");
            密码.Text = PlayerPrefs.GetString("密码");
        }else 
        {
            PlayerPrefs.SetString("用户名", "");
            PlayerPrefs.SetString("密码", "");
        }

    }
    void 是否自动登录(bool 是否)
    {
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
    // Update is called once per frame
    void 登录()
    {
        if (用户名.Text == "")
        {
            Debug.Log("[0]登录界面_韩 用户名不能为空");
            return;
        }
        else if (密码.Text == "")
        {
            Debug.Log("[0]登录界面_韩 密码不能为空");
            return;
        }
        else
        {
            var 用户录 = this.GetModel<用户录>();
            var 用户 = 用户录.GetDataList().Find(x => x.账号 == 用户名.Text);
            if (用户 != null)   // 判断用户是否存在
            {
                if (用户.密码 == 密码.Text)
                {
                    Debug.Log("[0]登录界面_韩 登录成功");
                    PlayerPrefs.SetString("用户名", 用户名.Text);
                    PlayerPrefs.SetString("密码", 密码.Text);
                    跳转界面("选择场景");
                }
                else
                {
                    Debug.Log("[0]登录界面_韩 密码错误");
                }
            }
        }
    }

    async void 跳转界面(string 界面名称)
    {
        Debug.Log("[0]登录界面_韩 跳转界面:" + 界面名称);
        var prefab = await LoadYooAssetsTool.LoadAsset<GameObject>("界面加载动画");
        var 加载动画 = Instantiate(prefab,this.transform.parent);
        while (加载动画.GetComponent<加载动画_韩>().加载动画状态 == 加载动画状态.加载中)
        {
            await UniTask.Delay(100);
        }
        if (加载动画.GetComponent<加载动画_韩>().加载动画状态 == 加载动画状态.取消)
        {
            Debug.Log("[0]登录界面_韩 取消加载");
            return;
        }else if (加载动画.GetComponent<加载动画_韩>().加载动画状态 == 加载动画状态.完成)
        {
            Destroy(加载动画);
            await UniTask.Delay(100);
            LoadYooAssetsTool.LoadSceneAsync(界面名称).Forget();
        }
    }
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }


    
}
