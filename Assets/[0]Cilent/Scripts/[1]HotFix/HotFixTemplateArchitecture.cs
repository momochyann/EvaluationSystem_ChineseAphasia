using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class HotFixTemplateArchitecture : Architecture<HotFixTemplateArchitecture>
{
    protected override void Init()
    {
        // 注册工具类
        RegisterUtility<StorageUtility>(new StorageUtility());
        RegisterUtility<按名索录之器>(new 按名索录之器());
        RegisterUtility<YooAssetPfbPoolUtility>(new YooAssetPfbPoolUtility());
        this.GetUtility<YooAssetPfbPoolUtility>().Init();
        RegisterUtility<SerialPortCenterUtility>(new SerialPortCenterUtility());
        
    
        RegisterUtility<ImagePickerUtility>(new ImagePickerUtility());
        // 注册截图和PDF工具类
        RegisterUtility<IScreenshotPDFUtility>(new ScreenshotPDFUtility());

        // 注册模型
        RegisterModel<用户录>(new 用户录());
        RegisterModel<评人录>(new 评人录());
        RegisterModel<管员录>(new 管员录());
        RegisterModel<软件配置录>(new 软件配置录());
        RegisterModel<档案录>(new 档案录());
        RegisterModel<综合评价模板录>(new 综合评价模板录());
        RegisterModel<指导建议模板录>(new 指导建议模板录());
        
        // 注册系统
        RegisterSystem<选项组高亮管理System>(new 选项组高亮管理System());
        // this.RegisterUtility<IDongleUtility>(new DongleUtility());
        
    }

}
