using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class 讯授受_茉 : MonoBehaviour, IController
{
    // Start is called before the first frame update
    void Start()
    {
        BleManager.onCharacteristicNotificationAction += 受讯令;
        
    }
    void 受讯令(string arg0, string arg1, byte[] arg2)
    {
        if (arg2[arg2.Length - 1] == 0xaa)
        {
            string 录名 = "";
            switch (arg2[arg2.Length - 2])
            {
                case 0x00:
                    录名 = "评人录";
                    break;
                case 0x01:
                    录名 = "用户录";
                    //执行逻辑1
                    break;
                case 0x02:
                    录名 = "档案录";
                    //执行逻辑2 
                    break;
                default:
                    Debug.Log("未知指令");
                    break;
            }
            Debug.Log("受蓝牙令" + 录名);
             this.GetUtility<按名索录之器>().寻录<录元<I记贞>>(录名)
             .纳记于信(System.Text.Encoding.UTF8.GetString(arg2, 0, arg2.Length - 2));
        }
    }
    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
    
}
