using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;
public class 蓝牙连接界面_茉 : MonoBehaviour 
{
    // Start is called before the first frame update
    [SerializeField] public Button 退出按钮;
    void Start()
    {
        退出按钮.OnClicked.AddListener(执行退出);
    }

    private void 执行退出()
    {
        Destroy(gameObject);
    }

  
}
