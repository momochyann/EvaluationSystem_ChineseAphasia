using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 主场景背景管理_茉 : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject PC背景;
    [SerializeField] GameObject 手机背景;
    private void Awake()
    {
#if UNITY_EDITOR
        // 在编辑器中根据选择的平台返回后缀
        if (UnityEditor.EditorPrefs.GetInt("TestPlatform", 0) == 0)
        {
            PC背景.SetActive(true);
            手机背景.SetActive(false);
        }
        else
        {
            PC背景.SetActive(false);
            手机背景.SetActive(true);
        }
#elif UNITY_ANDROID || UNITY_IOS
        PC背景.SetActive(false);
        手机背景.SetActive(true);
#else
        PC背景.SetActive(true);
        手机背景.SetActive(false);
#endif
    }
 


}
