using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using NovaSamples.UIControls;
public class 测试钮 : MonoBehaviour, IController
{
   
    // public int WorkSceneIndex;
    void Start()
    {
        this.GetComponent<Button>().OnClicked.AddListener(() =>
        {
            var 临评人 = new 评人();
            临评人.名 = "张三";
            临评人.诞辰 = "1990-01-01";
            临评人.账号 = "zhangsan";
            临评人.密码 = Random.Range(100000, 999999).ToString();
            临评人.电话 = Random.Range(10000000000, 99999999999).ToString();
            临评人.身份证 = "123456789012345678";
            临评人.性别 = "男";
            this.SendCommand<增记于录>(new 增记于录(临评人, "评人录"));
        });
    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
