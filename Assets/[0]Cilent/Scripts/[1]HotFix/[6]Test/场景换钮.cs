using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using NovaSamples.UIControls;
public class 场景换钮 : MonoBehaviour, IController
{
    [SerializeField] private string sceneName;
   // public int WorkSceneIndex;
    void Start()
    {
        this.GetComponent<Button>().OnClicked.AddListener(() =>
        {
            LoadYooAssetsTool.LoadSceneAsync(sceneName).Forget();
        });
    }

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }
}
