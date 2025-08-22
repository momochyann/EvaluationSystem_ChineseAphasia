using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Button2SceneChange : MonoBehaviour
{
    // Start is called before the first frame update
    public string sceneName;
    public bool isLocalLoad = false;
    void Start()
    {
        if (GetComponent<Button>() != null)
        {
            GetComponent<Button>().onClick.AddListener(OnClickButton);
        }
        else if(GetComponent<NovaSamples.UIControls.Button>()!=null)
        {
            GetComponent<NovaSamples.UIControls.Button>().OnClicked.AddListener(OnClickButton);
        }
    }

    private void OnClickButton()
    {
        LoadYooAssetsTool.LoadSceneAsync(sceneName, isLocalLoad).Forget();
    }

}
