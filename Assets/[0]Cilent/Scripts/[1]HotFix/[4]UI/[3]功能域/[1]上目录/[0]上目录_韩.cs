using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NovaSamples.UIControls;
using Nova;

public class 上目录_韩 : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public TextBlock 软件名称;
    [SerializeField] public NovaSamples.UIControls.Button 蓝牙;
    [SerializeField] public NovaSamples.UIControls.Button 用户名;
    [SerializeField] public NovaSamples.UIControls.Button 系统;
    [SerializeField] public NovaSamples.UIControls.Button 退出;
    void Start()
    {
        软件名称.Text = "软件名称";
        蓝牙.OnClicked.AddListener(() => 蓝牙设置());
        用户名.OnClicked.AddListener(() => 用户名设置());
        系统.OnClicked.AddListener(() => 系统设置());
        退出.OnClicked.AddListener(() => 退出登录());

        void 蓝牙设置()
        {
            Debug.Log("蓝牙设置");
        }
        void 用户名设置()
        {
            Debug.Log("用户名设置");
        }
        void 系统设置()
        {
            Debug.Log("系统设置");
        }
        void 退出登录()
        {
            Debug.Log("退出登录");
            PlayerPrefs.SetString("自动登录", "false");
            LoadYooAssetsTool.LoadSceneAsync("登录界面场景").Forget();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
