using System.Collections;
using System.Collections.Generic;
using Nova;
using NovaSamples.UIControls;
using UnityEngine;

public class 左目录选择_韩 : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] public List<NovaSamples.UIControls.Button> 目录列表;
    void Start()
    {
        foreach (var item in 目录列表)
        {
            item.OnClicked.AddListener(() => 执行功能(item.name));
        }
    }
    void 执行功能(string 功能名称)
    {
        Debug.Log("执行功能：" + 功能名称);
        功能名称 = 功能名称 + "显示_PC";
        FindObjectOfType<界面管理器_韩>().显示界面(功能名称);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
