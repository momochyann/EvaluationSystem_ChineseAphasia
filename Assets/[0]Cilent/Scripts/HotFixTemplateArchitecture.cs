using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
public class HotFixTemplateArchitecture : Architecture<HotFixTemplateArchitecture>
{
    protected override void Init()
    {
        // 注册系统
      
        // 注册工具类
        RegisterUtility<YooAssetPfbPoolUtility>(new YooAssetPfbPoolUtility());
        this.GetUtility<YooAssetPfbPoolUtility>().Init();
    }

}
