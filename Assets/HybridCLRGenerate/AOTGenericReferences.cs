using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"DOTween.dll",
		"EaseSave3.dll",
		"LitJson.dll",
		"Main.dll",
		"Nova.dll",
		"QFramework.CoreKit.dll",
		"QFramework.dll",
		"System.Core.dll",
		"UniTask.dll",
		"UnityEngine.AndroidJNIModule.dll",
		"UnityEngine.CoreModule.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<GameStartAfterLoad.<WaitLoadAnimation>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ImagePickerUtility.<LoadImageFromCacheAsync>d__19,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ImagePickerUtility.<LoadTextureFromFileAsync>d__13,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ImagePickerUtility.<PickImageFromFileDialogAsync>d__11,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ImagePickerUtility.<PickImageFromGalleryAsync>d__10,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ImagePickerUtility.<PickImageFromMobileGalleryAsync>d__12,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ImagePickerUtility.<RequestGalleryPermissionAsync>d__27,byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ImagePickerUtility.<SaveImageToCacheAsync>d__18,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<QuickScreenshot.<CaptureAndPrintAsync>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<QuickScreenshot.<CaptureAsync>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<QuickScreenshot.<CaptureNovaScrollViewAsync>d__5>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ScreenshotPDFPanel.<CaptureAndExportAsync>d__17>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ScreenshotPDFPanel.<CaptureScreenshotAsync>d__16>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ScreenshotPDFUtility.<CaptureAndExportToPDFAsync>d__5,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ScreenshotPDFUtility.<CaptureCameraViewAsync>d__3,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ScreenshotPDFUtility.<CaptureScreenshotAsync>d__2,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ScreenshotPDFUtility.<CaptureScrollViewAsync>d__4,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ScreenshotPDFUtility.<WaitForFileCreated>d__9>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<SerialPortBaseUtility.<GetComPortsWithSkip>d__24,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<SerialPortBaseUtility.<SearchAndConnectAsync>d__26,byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<SerialPortBaseUtility.<TryConnectPort>d__27,byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<SerialPortCenterUtility.<RecognizeDevice>d__19,byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<SerialPortCenterUtility.<RequestUnlockAsync>d__25,byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<SerialPortCenterUtility.<WaitForHandshake>d__20>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<YooAssetCacheManager.<LoadAndCacheAsset>d__18>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<YooAssetCacheManager.<WarmupAssets>d__17>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<YooAssetPfbPoolUtility.<LoadConfig>d__14<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<YooAssetPfbPoolUtility.<LoadPfb>d__13,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<YooAssetPfbPoolUtility.<PreloadCommonAssets>d__15>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<告示_茉.<等待确认显示>d__8,byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<截图钮.<截图>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<电脑端蓝牙串口检测.<启动连接加密狗>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<界面管理界面_茉.<显示提示>d__17,byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<GameStartAfterLoad.<WaitLoadAnimation>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ImagePickerUtility.<LoadImageFromCacheAsync>d__19,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ImagePickerUtility.<LoadTextureFromFileAsync>d__13,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ImagePickerUtility.<PickImageFromFileDialogAsync>d__11,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ImagePickerUtility.<PickImageFromGalleryAsync>d__10,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ImagePickerUtility.<PickImageFromMobileGalleryAsync>d__12,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ImagePickerUtility.<RequestGalleryPermissionAsync>d__27,byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ImagePickerUtility.<SaveImageToCacheAsync>d__18,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<QuickScreenshot.<CaptureAndPrintAsync>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<QuickScreenshot.<CaptureAsync>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<QuickScreenshot.<CaptureNovaScrollViewAsync>d__5>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ScreenshotPDFPanel.<CaptureAndExportAsync>d__17>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ScreenshotPDFPanel.<CaptureScreenshotAsync>d__16>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ScreenshotPDFUtility.<CaptureAndExportToPDFAsync>d__5,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ScreenshotPDFUtility.<CaptureCameraViewAsync>d__3,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ScreenshotPDFUtility.<CaptureScreenshotAsync>d__2,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ScreenshotPDFUtility.<CaptureScrollViewAsync>d__4,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ScreenshotPDFUtility.<WaitForFileCreated>d__9>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<SerialPortBaseUtility.<GetComPortsWithSkip>d__24,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<SerialPortBaseUtility.<SearchAndConnectAsync>d__26,byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<SerialPortBaseUtility.<TryConnectPort>d__27,byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<SerialPortCenterUtility.<RecognizeDevice>d__19,byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<SerialPortCenterUtility.<RequestUnlockAsync>d__25,byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<SerialPortCenterUtility.<WaitForHandshake>d__20>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<YooAssetCacheManager.<LoadAndCacheAsset>d__18>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<YooAssetCacheManager.<WarmupAssets>d__17>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<YooAssetPfbPoolUtility.<LoadConfig>d__14<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<YooAssetPfbPoolUtility.<LoadPfb>d__13,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<YooAssetPfbPoolUtility.<PreloadCommonAssets>d__15>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<告示_茉.<等待确认显示>d__8,byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<截图钮.<截图>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<电脑端蓝牙串口检测.<启动连接加密狗>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<界面管理界面_茉.<显示提示>d__17,byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<BleConnecter.<ConnectBLE>d__10>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<BleConnecter.<SendDataLoop>d__9>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<BleScaner.<StartScan>d__12>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<SerialPortBaseUtility.<MonitorDeviceStatus>d__34>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<图片选择组件.<OnImageClicked>d__7>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<安卓功能栏.<删>d__18>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<择域.<长生>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<择木栏安卓_茉.<显记于目>d__15>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<显记栏_茉.<现目于栏>d__8>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<条目查看按钮_茉.<显录于增板>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<用户管理添加_韩.<选择出生日期>d__17>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<用户管理添加安卓_茉.<选择出生日期>d__20>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<索录栏契韩_茉.<显记于目>d__9>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<蓝牙连接界面_茉.<开始扫描>d__12>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<评定人管理添加_韩.<选择出生日期>d__14>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<评定人管理添加安卓_茉.<选择出生日期>d__19>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<遂牖_茉.<显示提示>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<BleConnecter.<ConnectBLE>d__10>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<BleConnecter.<SendDataLoop>d__9>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<BleScaner.<StartScan>d__12>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<SerialPortBaseUtility.<MonitorDeviceStatus>d__34>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<图片选择组件.<OnImageClicked>d__7>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<安卓功能栏.<删>d__18>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<择域.<长生>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<择木栏安卓_茉.<显记于目>d__15>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<显记栏_茉.<现目于栏>d__8>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<条目查看按钮_茉.<显录于增板>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<用户管理添加_韩.<选择出生日期>d__17>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<用户管理添加安卓_茉.<选择出生日期>d__20>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<索录栏契韩_茉.<显记于目>d__9>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<蓝牙连接界面_茉.<开始扫描>d__12>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<评定人管理添加_韩.<选择出生日期>d__14>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<评定人管理添加安卓_茉.<选择出生日期>d__19>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<遂牖_茉.<显示提示>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.IStateMachineRunnerPromise<byte>
	// Cysharp.Threading.Tasks.CompilerServices.IStateMachineRunnerPromise<object>
	// Cysharp.Threading.Tasks.ITaskPoolNode<object>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,byte>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,int>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,object>>
	// Cysharp.Threading.Tasks.IUniTaskSource<byte>
	// Cysharp.Threading.Tasks.IUniTaskSource<int>
	// Cysharp.Threading.Tasks.IUniTaskSource<object>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,byte>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,int>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,object>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<byte>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<int>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<object>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,byte>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,int>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,object>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<byte>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<int>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<object>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,byte>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,int>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,object>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<byte>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<int>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<object>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,byte>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,int>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,object>>
	// Cysharp.Threading.Tasks.UniTask<byte>
	// Cysharp.Threading.Tasks.UniTask<int>
	// Cysharp.Threading.Tasks.UniTask<object>
	// Cysharp.Threading.Tasks.UniTaskCompletionSource<byte>
	// Cysharp.Threading.Tasks.UniTaskCompletionSource<object>
	// Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<byte>
	// Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<object>
	// DG.Tweening.Core.DOGetter<UnityEngine.Color>
	// DG.Tweening.Core.DOGetter<UnityEngine.Vector3>
	// DG.Tweening.Core.DOSetter<UnityEngine.Color>
	// DG.Tweening.Core.DOSetter<float>
	// DG.Tweening.Core.TweenerCore<UnityEngine.Vector3,UnityEngine.Vector3,DG.Tweening.Plugins.Options.VectorOptions>
	// DG.Tweening.Plugins.Core.ABSTweenPlugin<UnityEngine.Vector3,UnityEngine.Vector3,DG.Tweening.Plugins.Options.VectorOptions>
	// Nova.Internal.Collections.CollectionPool<object,Nova.Events.EventModule.ScopedCallback>
	// Nova.Internal.Collections.HashList<Nova.Events.EventModule.ScopedCallback>
	// Nova.Internal.Collections.HashList<object>
	// Nova.Internal.Collections.UID<object>
	// Nova.Internal.Core.DataStore<object,object>
	// Nova.Internal.DataBinding.DataBinder<object>
	// Nova.Internal.DataBinding.ListWrapper<object>
	// Nova.Internal.DataBinding.PrefabPool<int>
	// Nova.PrefabProviderCallback<int>
	// Nova.UIEventHandler<Nova.Gesture.OnClick,object>
	// Nova.UIEventHandler<Nova.Gesture.OnClick>
	// QFramework.Architecture.<>c<object>
	// QFramework.Architecture<object>
	// QFramework.BindableProperty.<>c<byte>
	// QFramework.BindableProperty.<>c__DisplayClass17_0<byte>
	// QFramework.BindableProperty<byte>
	// QFramework.EasyEvent.<>c<byte>
	// QFramework.EasyEvent.<>c__DisplayClass1_0<byte>
	// QFramework.EasyEvent.<>c__DisplayClass4_0<byte>
	// QFramework.EasyEvent<byte>
	// System.Action<BluetoothLEHardwareInterface.iBeaconData>
	// System.Action<Cysharp.Threading.Tasks.UniTask>
	// System.Action<DropdownItem>
	// System.Action<Nova.Events.EventModule.ScopedCallback>
	// System.Action<Nova.Internal.Collections.UID<object>>
	// System.Action<System.ValueTuple<object,int>>
	// System.Action<System.ValueTuple<object,object>>
	// System.Action<byte>
	// System.Action<int,int,int>
	// System.Action<int>
	// System.Action<object,int>
	// System.Action<object,object,int,object>
	// System.Action<object,object,object>
	// System.Action<object,object>
	// System.Action<object>
	// System.Action<下拉框帜>
	// System.ByReference<UnityEngine.jvalue>
	// System.Collections.Generic.ArraySortHelper<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.ArraySortHelper<DropdownItem>
	// System.Collections.Generic.ArraySortHelper<Nova.Events.EventModule.ScopedCallback>
	// System.Collections.Generic.ArraySortHelper<Nova.Internal.Collections.UID<object>>
	// System.Collections.Generic.ArraySortHelper<System.ValueTuple<object,int>>
	// System.Collections.Generic.ArraySortHelper<System.ValueTuple<object,object>>
	// System.Collections.Generic.ArraySortHelper<int>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.ArraySortHelper<下拉框帜>
	// System.Collections.Generic.Comparer<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.Comparer<DropdownItem>
	// System.Collections.Generic.Comparer<Nova.Events.EventModule.ScopedCallback>
	// System.Collections.Generic.Comparer<Nova.Internal.Collections.UID<object>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,byte>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,int>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,object>>
	// System.Collections.Generic.Comparer<System.ValueTuple<object,int>>
	// System.Collections.Generic.Comparer<System.ValueTuple<object,object>>
	// System.Collections.Generic.Comparer<byte>
	// System.Collections.Generic.Comparer<int>
	// System.Collections.Generic.Comparer<long>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Comparer<下拉框帜>
	// System.Collections.Generic.Dictionary.Enumerator<Nova.Events.EventModule.ScopedCallback,int>
	// System.Collections.Generic.Dictionary.Enumerator<Nova.Internal.Collections.UID<object>,object>
	// System.Collections.Generic.Dictionary.Enumerator<Nova.Internal.Core.DataStoreID,System.ValueTuple<object,int>>
	// System.Collections.Generic.Dictionary.Enumerator<Nova.Internal.Core.DataStoreID,int>
	// System.Collections.Generic.Dictionary.Enumerator<Nova.Internal.Core.DataStoreID,object>
	// System.Collections.Generic.Dictionary.Enumerator<int,System.ValueTuple<object,object,Nova.Internal.Core.DataStoreID>>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,ES3Internal.ES3Data>
	// System.Collections.Generic.Dictionary.Enumerator<object,Nova.Events.EventModule.Callbacks>
	// System.Collections.Generic.Dictionary.Enumerator<object,Nova.Internal.Collections.UID<object>>
	// System.Collections.Generic.Dictionary.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<Nova.Events.EventModule.ScopedCallback,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<Nova.Internal.Collections.UID<object>,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<Nova.Internal.Core.DataStoreID,System.ValueTuple<object,int>>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<Nova.Internal.Core.DataStoreID,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<Nova.Internal.Core.DataStoreID,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,System.ValueTuple<object,object,Nova.Internal.Core.DataStoreID>>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,ES3Internal.ES3Data>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,Nova.Events.EventModule.Callbacks>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,Nova.Internal.Collections.UID<object>>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<Nova.Events.EventModule.ScopedCallback,int>
	// System.Collections.Generic.Dictionary.KeyCollection<Nova.Internal.Collections.UID<object>,object>
	// System.Collections.Generic.Dictionary.KeyCollection<Nova.Internal.Core.DataStoreID,System.ValueTuple<object,int>>
	// System.Collections.Generic.Dictionary.KeyCollection<Nova.Internal.Core.DataStoreID,int>
	// System.Collections.Generic.Dictionary.KeyCollection<Nova.Internal.Core.DataStoreID,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,System.ValueTuple<object,object,Nova.Internal.Core.DataStoreID>>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,ES3Internal.ES3Data>
	// System.Collections.Generic.Dictionary.KeyCollection<object,Nova.Events.EventModule.Callbacks>
	// System.Collections.Generic.Dictionary.KeyCollection<object,Nova.Internal.Collections.UID<object>>
	// System.Collections.Generic.Dictionary.KeyCollection<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<Nova.Events.EventModule.ScopedCallback,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<Nova.Internal.Collections.UID<object>,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<Nova.Internal.Core.DataStoreID,System.ValueTuple<object,int>>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<Nova.Internal.Core.DataStoreID,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<Nova.Internal.Core.DataStoreID,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,System.ValueTuple<object,object,Nova.Internal.Core.DataStoreID>>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,ES3Internal.ES3Data>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,Nova.Events.EventModule.Callbacks>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,Nova.Internal.Collections.UID<object>>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<Nova.Events.EventModule.ScopedCallback,int>
	// System.Collections.Generic.Dictionary.ValueCollection<Nova.Internal.Collections.UID<object>,object>
	// System.Collections.Generic.Dictionary.ValueCollection<Nova.Internal.Core.DataStoreID,System.ValueTuple<object,int>>
	// System.Collections.Generic.Dictionary.ValueCollection<Nova.Internal.Core.DataStoreID,int>
	// System.Collections.Generic.Dictionary.ValueCollection<Nova.Internal.Core.DataStoreID,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,System.ValueTuple<object,object,Nova.Internal.Core.DataStoreID>>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,ES3Internal.ES3Data>
	// System.Collections.Generic.Dictionary.ValueCollection<object,Nova.Events.EventModule.Callbacks>
	// System.Collections.Generic.Dictionary.ValueCollection<object,Nova.Internal.Collections.UID<object>>
	// System.Collections.Generic.Dictionary.ValueCollection<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary<Nova.Events.EventModule.ScopedCallback,int>
	// System.Collections.Generic.Dictionary<Nova.Internal.Collections.UID<object>,object>
	// System.Collections.Generic.Dictionary<Nova.Internal.Core.DataStoreID,System.ValueTuple<object,int>>
	// System.Collections.Generic.Dictionary<Nova.Internal.Core.DataStoreID,int>
	// System.Collections.Generic.Dictionary<Nova.Internal.Core.DataStoreID,object>
	// System.Collections.Generic.Dictionary<int,System.ValueTuple<object,object,Nova.Internal.Core.DataStoreID>>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<object,ES3Internal.ES3Data>
	// System.Collections.Generic.Dictionary<object,Nova.Events.EventModule.Callbacks>
	// System.Collections.Generic.Dictionary<object,Nova.Internal.Collections.UID<object>>
	// System.Collections.Generic.Dictionary<object,int>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.EqualityComparer<ES3Internal.ES3Data>
	// System.Collections.Generic.EqualityComparer<Nova.Events.EventModule.Callbacks>
	// System.Collections.Generic.EqualityComparer<Nova.Events.EventModule.ScopedCallback>
	// System.Collections.Generic.EqualityComparer<Nova.Internal.Collections.UID<object>>
	// System.Collections.Generic.EqualityComparer<Nova.Internal.Core.DataStoreID>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,byte>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,int>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,object>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<object,int>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<object,object,Nova.Internal.Core.DataStoreID>>
	// System.Collections.Generic.EqualityComparer<byte>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<long>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.HashSetEqualityComparer<object>
	// System.Collections.Generic.ICollection<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.ICollection<DropdownItem>
	// System.Collections.Generic.ICollection<Nova.Events.EventModule.ScopedCallback>
	// System.Collections.Generic.ICollection<Nova.Internal.Collections.UID<object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<Nova.Events.EventModule.ScopedCallback,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<Nova.Internal.Collections.UID<object>,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<Nova.Internal.Core.DataStoreID,System.ValueTuple<object,int>>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<Nova.Internal.Core.DataStoreID,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<Nova.Internal.Core.DataStoreID,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,System.ValueTuple<object,object,Nova.Internal.Core.DataStoreID>>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,ES3Internal.ES3Data>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,Nova.Events.EventModule.Callbacks>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,Nova.Internal.Collections.UID<object>>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<System.ValueTuple<object,int>>
	// System.Collections.Generic.ICollection<System.ValueTuple<object,object>>
	// System.Collections.Generic.ICollection<int>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.ICollection<下拉框帜>
	// System.Collections.Generic.IComparer<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.IComparer<DropdownItem>
	// System.Collections.Generic.IComparer<Nova.Events.EventModule.ScopedCallback>
	// System.Collections.Generic.IComparer<Nova.Internal.Collections.UID<object>>
	// System.Collections.Generic.IComparer<System.ValueTuple<object,int>>
	// System.Collections.Generic.IComparer<System.ValueTuple<object,object>>
	// System.Collections.Generic.IComparer<int>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IComparer<下拉框帜>
	// System.Collections.Generic.IDictionary<object,LitJson.ArrayMetadata>
	// System.Collections.Generic.IDictionary<object,LitJson.ObjectMetadata>
	// System.Collections.Generic.IDictionary<object,LitJson.PropertyMetadata>
	// System.Collections.Generic.IDictionary<object,object>
	// System.Collections.Generic.IEnumerable<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.IEnumerable<DropdownItem>
	// System.Collections.Generic.IEnumerable<Nova.Events.EventModule.ScopedCallback>
	// System.Collections.Generic.IEnumerable<Nova.Internal.Collections.UID<object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<Nova.Events.EventModule.ScopedCallback,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<Nova.Internal.Collections.UID<object>,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<Nova.Internal.Core.DataStoreID,System.ValueTuple<object,int>>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<Nova.Internal.Core.DataStoreID,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<Nova.Internal.Core.DataStoreID,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,System.ValueTuple<object,object,Nova.Internal.Core.DataStoreID>>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,ES3Internal.ES3Data>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,Nova.Events.EventModule.Callbacks>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,Nova.Internal.Collections.UID<object>>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<System.ValueTuple<object,int>>
	// System.Collections.Generic.IEnumerable<System.ValueTuple<object,object>>
	// System.Collections.Generic.IEnumerable<byte>
	// System.Collections.Generic.IEnumerable<int>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerable<下拉框帜>
	// System.Collections.Generic.IEnumerator<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.IEnumerator<DropdownItem>
	// System.Collections.Generic.IEnumerator<Nova.Events.EventModule.ScopedCallback>
	// System.Collections.Generic.IEnumerator<Nova.Internal.Collections.UID<object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<Nova.Events.EventModule.ScopedCallback,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<Nova.Internal.Collections.UID<object>,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<Nova.Internal.Core.DataStoreID,System.ValueTuple<object,int>>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<Nova.Internal.Core.DataStoreID,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<Nova.Internal.Core.DataStoreID,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,System.ValueTuple<object,object,Nova.Internal.Core.DataStoreID>>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,ES3Internal.ES3Data>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,Nova.Events.EventModule.Callbacks>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,Nova.Internal.Collections.UID<object>>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<System.ValueTuple<object,int>>
	// System.Collections.Generic.IEnumerator<System.ValueTuple<object,object>>
	// System.Collections.Generic.IEnumerator<byte>
	// System.Collections.Generic.IEnumerator<int>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEnumerator<下拉框帜>
	// System.Collections.Generic.IEqualityComparer<Nova.Events.EventModule.ScopedCallback>
	// System.Collections.Generic.IEqualityComparer<Nova.Internal.Collections.UID<object>>
	// System.Collections.Generic.IEqualityComparer<Nova.Internal.Core.DataStoreID>
	// System.Collections.Generic.IEqualityComparer<byte>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IList<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.IList<DropdownItem>
	// System.Collections.Generic.IList<Nova.Events.EventModule.ScopedCallback>
	// System.Collections.Generic.IList<Nova.Internal.Collections.UID<object>>
	// System.Collections.Generic.IList<System.ValueTuple<object,int>>
	// System.Collections.Generic.IList<System.ValueTuple<object,object>>
	// System.Collections.Generic.IList<int>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.IList<下拉框帜>
	// System.Collections.Generic.KeyValuePair<Nova.Events.EventModule.ScopedCallback,int>
	// System.Collections.Generic.KeyValuePair<Nova.Internal.Collections.UID<object>,object>
	// System.Collections.Generic.KeyValuePair<Nova.Internal.Core.DataStoreID,System.ValueTuple<object,int>>
	// System.Collections.Generic.KeyValuePair<Nova.Internal.Core.DataStoreID,int>
	// System.Collections.Generic.KeyValuePair<Nova.Internal.Core.DataStoreID,object>
	// System.Collections.Generic.KeyValuePair<int,System.ValueTuple<object,object,Nova.Internal.Core.DataStoreID>>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<object,ES3Internal.ES3Data>
	// System.Collections.Generic.KeyValuePair<object,Nova.Events.EventModule.Callbacks>
	// System.Collections.Generic.KeyValuePair<object,Nova.Internal.Collections.UID<object>>
	// System.Collections.Generic.KeyValuePair<object,int>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.List.Enumerator<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.List.Enumerator<DropdownItem>
	// System.Collections.Generic.List.Enumerator<Nova.Events.EventModule.ScopedCallback>
	// System.Collections.Generic.List.Enumerator<Nova.Internal.Collections.UID<object>>
	// System.Collections.Generic.List.Enumerator<System.ValueTuple<object,int>>
	// System.Collections.Generic.List.Enumerator<System.ValueTuple<object,object>>
	// System.Collections.Generic.List.Enumerator<int>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List.Enumerator<下拉框帜>
	// System.Collections.Generic.List<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.List<DropdownItem>
	// System.Collections.Generic.List<Nova.Events.EventModule.ScopedCallback>
	// System.Collections.Generic.List<Nova.Internal.Collections.UID<object>>
	// System.Collections.Generic.List<System.ValueTuple<object,int>>
	// System.Collections.Generic.List<System.ValueTuple<object,object>>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.List<下拉框帜>
	// System.Collections.Generic.ObjectComparer<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.ObjectComparer<DropdownItem>
	// System.Collections.Generic.ObjectComparer<Nova.Events.EventModule.ScopedCallback>
	// System.Collections.Generic.ObjectComparer<Nova.Internal.Collections.UID<object>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,byte>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,int>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,object>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<object,int>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<object,object>>
	// System.Collections.Generic.ObjectComparer<byte>
	// System.Collections.Generic.ObjectComparer<int>
	// System.Collections.Generic.ObjectComparer<long>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectComparer<下拉框帜>
	// System.Collections.Generic.ObjectEqualityComparer<ES3Internal.ES3Data>
	// System.Collections.Generic.ObjectEqualityComparer<Nova.Events.EventModule.Callbacks>
	// System.Collections.Generic.ObjectEqualityComparer<Nova.Events.EventModule.ScopedCallback>
	// System.Collections.Generic.ObjectEqualityComparer<Nova.Internal.Collections.UID<object>>
	// System.Collections.Generic.ObjectEqualityComparer<Nova.Internal.Core.DataStoreID>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,byte>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,int>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,object>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<object,int>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<object,object,Nova.Internal.Core.DataStoreID>>
	// System.Collections.Generic.ObjectEqualityComparer<byte>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<long>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.Generic.Queue.Enumerator<object>
	// System.Collections.Generic.Queue<object>
	// System.Collections.Generic.Stack.Enumerator<object>
	// System.Collections.Generic.Stack<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.ObjectModel.ReadOnlyCollection<DropdownItem>
	// System.Collections.ObjectModel.ReadOnlyCollection<Nova.Events.EventModule.ScopedCallback>
	// System.Collections.ObjectModel.ReadOnlyCollection<Nova.Internal.Collections.UID<object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.ValueTuple<object,int>>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.ValueTuple<object,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<int>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<下拉框帜>
	// System.Comparison<Cysharp.Threading.Tasks.UniTask>
	// System.Comparison<DropdownItem>
	// System.Comparison<Nova.Events.EventModule.ScopedCallback>
	// System.Comparison<Nova.Internal.Collections.UID<object>>
	// System.Comparison<System.ValueTuple<object,int>>
	// System.Comparison<System.ValueTuple<object,object>>
	// System.Comparison<int>
	// System.Comparison<object>
	// System.Comparison<下拉框帜>
	// System.Func<byte,byte,byte>
	// System.Func<byte>
	// System.Func<int>
	// System.Func<object,byte>
	// System.Func<object,int>
	// System.Func<object,object,object>
	// System.Func<object,object>
	// System.Func<object>
	// System.Linq.Buffer<object>
	// System.Linq.Enumerable.<CastIterator>d__99<object>
	// System.Linq.Enumerable.<TakeIterator>d__25<byte>
	// System.Linq.Enumerable.Iterator<object>
	// System.Linq.Enumerable.WhereArrayIterator<object>
	// System.Linq.Enumerable.WhereEnumerableIterator<object>
	// System.Linq.Enumerable.WhereListIterator<object>
	// System.Nullable<System.ValueTuple<object,object>>
	// System.Nullable<int>
	// System.Predicate<Cysharp.Threading.Tasks.UniTask>
	// System.Predicate<DropdownItem>
	// System.Predicate<Nova.Events.EventModule.ScopedCallback>
	// System.Predicate<Nova.Internal.Collections.UID<object>>
	// System.Predicate<System.ValueTuple<object,int>>
	// System.Predicate<System.ValueTuple<object,object>>
	// System.Predicate<int>
	// System.Predicate<object>
	// System.Predicate<下拉框帜>
	// System.ReadOnlySpan<UnityEngine.jvalue>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<int>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<object>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<int>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<object>
	// System.Runtime.CompilerServices.TaskAwaiter<int>
	// System.Runtime.CompilerServices.TaskAwaiter<object>
	// System.Span<UnityEngine.jvalue>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<int>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<object>
	// System.Threading.Tasks.Task<int>
	// System.Threading.Tasks.Task<object>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<int>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<object>
	// System.Threading.Tasks.TaskFactory<int>
	// System.Threading.Tasks.TaskFactory<object>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,byte>>
	// System.ValueTuple<byte,System.ValueTuple<byte,int>>
	// System.ValueTuple<byte,System.ValueTuple<byte,object>>
	// System.ValueTuple<byte,byte>
	// System.ValueTuple<byte,int>
	// System.ValueTuple<byte,object>
	// System.ValueTuple<int,int,int,object,byte>
	// System.ValueTuple<int,int,long>
	// System.ValueTuple<object,int>
	// System.ValueTuple<object,object>
	// UnityEngine.Events.InvokableCall<byte>
	// UnityEngine.Events.InvokableCall<object>
	// UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene,int>
	// UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene>
	// UnityEngine.Events.UnityAction<byte>
	// UnityEngine.Events.UnityAction<object,object,object>
	// UnityEngine.Events.UnityAction<object,object>
	// UnityEngine.Events.UnityAction<object>
	// UnityEngine.Events.UnityEvent<byte>
	// UnityEngine.Events.UnityEvent<object>
	// }}

	public void RefMethods()
	{
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,GameStartAfterLoad.<WaitLoadAnimation>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter&,GameStartAfterLoad.<WaitLoadAnimation>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ScreenshotPDFUtility.<WaitForFileCreated>d__9>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ScreenshotPDFUtility.<WaitForFileCreated>d__9&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,SerialPortCenterUtility.<WaitForHandshake>d__20>(Cysharp.Threading.Tasks.UniTask.Awaiter&,SerialPortCenterUtility.<WaitForHandshake>d__20&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,YooAssetCacheManager.<WarmupAssets>d__17>(Cysharp.Threading.Tasks.UniTask.Awaiter&,YooAssetCacheManager.<WarmupAssets>d__17&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,YooAssetPfbPoolUtility.<PreloadCommonAssets>d__15>(Cysharp.Threading.Tasks.UniTask.Awaiter&,YooAssetPfbPoolUtility.<PreloadCommonAssets>d__15&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,截图钮.<截图>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,截图钮.<截图>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,电脑端蓝牙串口检测.<启动连接加密狗>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,电脑端蓝牙串口检测.<启动连接加密狗>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,QuickScreenshot.<CaptureAndPrintAsync>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,QuickScreenshot.<CaptureAndPrintAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,QuickScreenshot.<CaptureAsync>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,QuickScreenshot.<CaptureAsync>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,QuickScreenshot.<CaptureNovaScrollViewAsync>d__5>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,QuickScreenshot.<CaptureNovaScrollViewAsync>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ScreenshotPDFPanel.<CaptureAndExportAsync>d__17>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ScreenshotPDFPanel.<CaptureAndExportAsync>d__17&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ScreenshotPDFPanel.<CaptureScreenshotAsync>d__16>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ScreenshotPDFPanel.<CaptureScreenshotAsync>d__16&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,YooAssetCacheManager.<LoadAndCacheAsset>d__18>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,YooAssetCacheManager.<LoadAndCacheAsset>d__18&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,截图钮.<截图>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,截图钮.<截图>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,SerialPortBaseUtility.<TryConnectPort>d__27>(Cysharp.Threading.Tasks.UniTask.Awaiter&,SerialPortBaseUtility.<TryConnectPort>d__27&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,SerialPortCenterUtility.<RequestUnlockAsync>d__25>(Cysharp.Threading.Tasks.UniTask.Awaiter&,SerialPortCenterUtility.<RequestUnlockAsync>d__25&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,SerialPortBaseUtility.<SearchAndConnectAsync>d__26>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,SerialPortBaseUtility.<SearchAndConnectAsync>d__26&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,SerialPortBaseUtility.<TryConnectPort>d__27>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,SerialPortBaseUtility.<TryConnectPort>d__27&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,告示_茉.<等待确认显示>d__8>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,告示_茉.<等待确认显示>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,界面管理界面_茉.<显示提示>d__17>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,界面管理界面_茉.<显示提示>d__17&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<int>,SerialPortCenterUtility.<RecognizeDevice>d__19>(Cysharp.Threading.Tasks.UniTask.Awaiter<int>&,SerialPortCenterUtility.<RecognizeDevice>d__19&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,SerialPortBaseUtility.<SearchAndConnectAsync>d__26>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,SerialPortBaseUtility.<SearchAndConnectAsync>d__26&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,界面管理界面_茉.<显示提示>d__17>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,界面管理界面_茉.<显示提示>d__17&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<int>,ImagePickerUtility.<RequestGalleryPermissionAsync>d__27>(System.Runtime.CompilerServices.TaskAwaiter<int>&,ImagePickerUtility.<RequestGalleryPermissionAsync>d__27&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ScreenshotPDFUtility.<CaptureScreenshotAsync>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ScreenshotPDFUtility.<CaptureScreenshotAsync>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,ImagePickerUtility.<PickImageFromMobileGalleryAsync>d__12>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,ImagePickerUtility.<PickImageFromMobileGalleryAsync>d__12&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ImagePickerUtility.<PickImageFromFileDialogAsync>d__11>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ImagePickerUtility.<PickImageFromFileDialogAsync>d__11&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ImagePickerUtility.<PickImageFromGalleryAsync>d__10>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ImagePickerUtility.<PickImageFromGalleryAsync>d__10&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ImagePickerUtility.<PickImageFromMobileGalleryAsync>d__12>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ImagePickerUtility.<PickImageFromMobileGalleryAsync>d__12&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ScreenshotPDFUtility.<CaptureAndExportToPDFAsync>d__5>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ScreenshotPDFUtility.<CaptureAndExportToPDFAsync>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,YooAssetPfbPoolUtility.<LoadConfig>d__14<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,YooAssetPfbPoolUtility.<LoadConfig>d__14<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,YooAssetPfbPoolUtility.<LoadPfb>d__13>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,YooAssetPfbPoolUtility.<LoadPfb>d__13&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.YieldAwaitable.Awaiter,ScreenshotPDFUtility.<CaptureCameraViewAsync>d__3>(Cysharp.Threading.Tasks.YieldAwaitable.Awaiter&,ScreenshotPDFUtility.<CaptureCameraViewAsync>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.YieldAwaitable.Awaiter,ScreenshotPDFUtility.<CaptureScreenshotAsync>d__2>(Cysharp.Threading.Tasks.YieldAwaitable.Awaiter&,ScreenshotPDFUtility.<CaptureScreenshotAsync>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.YieldAwaitable.Awaiter,ScreenshotPDFUtility.<CaptureScrollViewAsync>d__4>(Cysharp.Threading.Tasks.YieldAwaitable.Awaiter&,ScreenshotPDFUtility.<CaptureScrollViewAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.YieldAwaitable.Awaiter,SerialPortBaseUtility.<GetComPortsWithSkip>d__24>(Cysharp.Threading.Tasks.YieldAwaitable.Awaiter&,SerialPortBaseUtility.<GetComPortsWithSkip>d__24&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,ImagePickerUtility.<SaveImageToCacheAsync>d__18>(System.Runtime.CompilerServices.TaskAwaiter&,ImagePickerUtility.<SaveImageToCacheAsync>d__18&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,ScreenshotPDFUtility.<CaptureCameraViewAsync>d__3>(System.Runtime.CompilerServices.TaskAwaiter&,ScreenshotPDFUtility.<CaptureCameraViewAsync>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,ScreenshotPDFUtility.<CaptureScrollViewAsync>d__4>(System.Runtime.CompilerServices.TaskAwaiter&,ScreenshotPDFUtility.<CaptureScrollViewAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,ImagePickerUtility.<LoadImageFromCacheAsync>d__19>(System.Runtime.CompilerServices.TaskAwaiter<object>&,ImagePickerUtility.<LoadImageFromCacheAsync>d__19&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,ImagePickerUtility.<LoadTextureFromFileAsync>d__13>(System.Runtime.CompilerServices.TaskAwaiter<object>&,ImagePickerUtility.<LoadTextureFromFileAsync>d__13&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<GameStartAfterLoad.<WaitLoadAnimation>d__2>(GameStartAfterLoad.<WaitLoadAnimation>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<QuickScreenshot.<CaptureAndPrintAsync>d__4>(QuickScreenshot.<CaptureAndPrintAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<QuickScreenshot.<CaptureAsync>d__3>(QuickScreenshot.<CaptureAsync>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<QuickScreenshot.<CaptureNovaScrollViewAsync>d__5>(QuickScreenshot.<CaptureNovaScrollViewAsync>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ScreenshotPDFPanel.<CaptureAndExportAsync>d__17>(ScreenshotPDFPanel.<CaptureAndExportAsync>d__17&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ScreenshotPDFPanel.<CaptureScreenshotAsync>d__16>(ScreenshotPDFPanel.<CaptureScreenshotAsync>d__16&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ScreenshotPDFUtility.<WaitForFileCreated>d__9>(ScreenshotPDFUtility.<WaitForFileCreated>d__9&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<SerialPortCenterUtility.<WaitForHandshake>d__20>(SerialPortCenterUtility.<WaitForHandshake>d__20&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<YooAssetCacheManager.<LoadAndCacheAsset>d__18>(YooAssetCacheManager.<LoadAndCacheAsset>d__18&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<YooAssetCacheManager.<WarmupAssets>d__17>(YooAssetCacheManager.<WarmupAssets>d__17&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<YooAssetPfbPoolUtility.<PreloadCommonAssets>d__15>(YooAssetPfbPoolUtility.<PreloadCommonAssets>d__15&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<截图钮.<截图>d__1>(截图钮.<截图>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<电脑端蓝牙串口检测.<启动连接加密狗>d__1>(电脑端蓝牙串口检测.<启动连接加密狗>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.Start<ImagePickerUtility.<RequestGalleryPermissionAsync>d__27>(ImagePickerUtility.<RequestGalleryPermissionAsync>d__27&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.Start<SerialPortBaseUtility.<CheckDeviceHealth>d__35>(SerialPortBaseUtility.<CheckDeviceHealth>d__35&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.Start<SerialPortBaseUtility.<SearchAndConnectAsync>d__26>(SerialPortBaseUtility.<SearchAndConnectAsync>d__26&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.Start<SerialPortBaseUtility.<TryConnectPort>d__27>(SerialPortBaseUtility.<TryConnectPort>d__27&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.Start<SerialPortCenterUtility.<RecognizeDevice>d__19>(SerialPortCenterUtility.<RecognizeDevice>d__19&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.Start<SerialPortCenterUtility.<RequestUnlockAsync>d__25>(SerialPortCenterUtility.<RequestUnlockAsync>d__25&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.Start<告示_茉.<等待确认显示>d__8>(告示_茉.<等待确认显示>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.Start<界面管理界面_茉.<显示提示>d__17>(界面管理界面_茉.<显示提示>d__17&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ImagePickerUtility.<LoadImageFromCacheAsync>d__19>(ImagePickerUtility.<LoadImageFromCacheAsync>d__19&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ImagePickerUtility.<LoadTextureFromFileAsync>d__13>(ImagePickerUtility.<LoadTextureFromFileAsync>d__13&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ImagePickerUtility.<PickImageFromFileDialogAsync>d__11>(ImagePickerUtility.<PickImageFromFileDialogAsync>d__11&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ImagePickerUtility.<PickImageFromGalleryAsync>d__10>(ImagePickerUtility.<PickImageFromGalleryAsync>d__10&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ImagePickerUtility.<PickImageFromMobileGalleryAsync>d__12>(ImagePickerUtility.<PickImageFromMobileGalleryAsync>d__12&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ImagePickerUtility.<SaveImageToCacheAsync>d__18>(ImagePickerUtility.<SaveImageToCacheAsync>d__18&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<LoadYooAssetsTool.<LoadAsset>d__4<object>>(LoadYooAssetsTool.<LoadAsset>d__4<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ScreenshotPDFUtility.<CaptureAndExportToPDFAsync>d__5>(ScreenshotPDFUtility.<CaptureAndExportToPDFAsync>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ScreenshotPDFUtility.<CaptureCameraViewAsync>d__3>(ScreenshotPDFUtility.<CaptureCameraViewAsync>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ScreenshotPDFUtility.<CaptureScreenshotAsync>d__2>(ScreenshotPDFUtility.<CaptureScreenshotAsync>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ScreenshotPDFUtility.<CaptureScrollViewAsync>d__4>(ScreenshotPDFUtility.<CaptureScrollViewAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<SerialPortBaseUtility.<GetComPortsWithSkip>d__24>(SerialPortBaseUtility.<GetComPortsWithSkip>d__24&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<YooAssetPfbPoolUtility.<LoadConfig>d__14<object>>(YooAssetPfbPoolUtility.<LoadConfig>d__14<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<YooAssetPfbPoolUtility.<LoadPfb>d__13>(YooAssetPfbPoolUtility.<LoadPfb>d__13&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,BleConnecter.<ConnectBLE>d__10>(Cysharp.Threading.Tasks.UniTask.Awaiter&,BleConnecter.<ConnectBLE>d__10&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,BleConnecter.<SendDataLoop>d__9>(Cysharp.Threading.Tasks.UniTask.Awaiter&,BleConnecter.<SendDataLoop>d__9&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,BleScaner.<StartScan>d__12>(Cysharp.Threading.Tasks.UniTask.Awaiter&,BleScaner.<StartScan>d__12&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,SerialPortBaseUtility.<MonitorDeviceStatus>d__34>(Cysharp.Threading.Tasks.UniTask.Awaiter&,SerialPortBaseUtility.<MonitorDeviceStatus>d__34&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,安卓功能栏.<删>d__18>(Cysharp.Threading.Tasks.UniTask.Awaiter&,安卓功能栏.<删>d__18&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,条目查看按钮_茉.<显录于增板>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter&,条目查看按钮_茉.<显录于增板>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,蓝牙连接界面_茉.<开始扫描>d__12>(Cysharp.Threading.Tasks.UniTask.Awaiter&,蓝牙连接界面_茉.<开始扫描>d__12&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,SerialPortBaseUtility.<MonitorDeviceStatus>d__34>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,SerialPortBaseUtility.<MonitorDeviceStatus>d__34&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,安卓功能栏.<删>d__18>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,安卓功能栏.<删>d__18&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,图片选择组件.<OnImageClicked>d__7>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,图片选择组件.<OnImageClicked>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,安卓功能栏.<删>d__18>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,安卓功能栏.<删>d__18&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,择域.<长生>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,择域.<长生>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,择木栏安卓_茉.<显记于目>d__15>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,择木栏安卓_茉.<显记于目>d__15&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,显记栏_茉.<现目于栏>d__8>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,显记栏_茉.<现目于栏>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,用户管理添加_韩.<选择出生日期>d__17>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,用户管理添加_韩.<选择出生日期>d__17&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,用户管理添加安卓_茉.<选择出生日期>d__20>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,用户管理添加安卓_茉.<选择出生日期>d__20&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,索录栏契韩_茉.<显记于目>d__9>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,索录栏契韩_茉.<显记于目>d__9&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,评定人管理添加_韩.<选择出生日期>d__14>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,评定人管理添加_韩.<选择出生日期>d__14&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,评定人管理添加安卓_茉.<选择出生日期>d__19>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,评定人管理添加安卓_茉.<选择出生日期>d__19&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,遂牖_茉.<显示提示>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,遂牖_茉.<显示提示>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<BleConnecter.<ConnectBLE>d__10>(BleConnecter.<ConnectBLE>d__10&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<BleConnecter.<SendDataLoop>d__9>(BleConnecter.<SendDataLoop>d__9&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<BleScaner.<StartScan>d__12>(BleScaner.<StartScan>d__12&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<SerialPortBaseUtility.<MonitorDeviceStatus>d__34>(SerialPortBaseUtility.<MonitorDeviceStatus>d__34&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<图片选择组件.<OnImageClicked>d__7>(图片选择组件.<OnImageClicked>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<安卓功能栏.<删>d__18>(安卓功能栏.<删>d__18&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<择域.<长生>d__1>(择域.<长生>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<择木栏安卓_茉.<显记于目>d__15>(择木栏安卓_茉.<显记于目>d__15&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<显记栏_茉.<现目于栏>d__8>(显记栏_茉.<现目于栏>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<条目查看按钮_茉.<显录于增板>d__4>(条目查看按钮_茉.<显录于增板>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<用户管理添加_韩.<选择出生日期>d__17>(用户管理添加_韩.<选择出生日期>d__17&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<用户管理添加安卓_茉.<选择出生日期>d__20>(用户管理添加安卓_茉.<选择出生日期>d__20&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<索录栏契韩_茉.<显记于目>d__9>(索录栏契韩_茉.<显记于目>d__9&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<蓝牙连接界面_茉.<开始扫描>d__12>(蓝牙连接界面_茉.<开始扫描>d__12&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<评定人管理添加_韩.<选择出生日期>d__14>(评定人管理添加_韩.<选择出生日期>d__14&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<评定人管理添加安卓_茉.<选择出生日期>d__19>(评定人管理添加安卓_茉.<选择出生日期>d__19&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<遂牖_茉.<显示提示>d__0>(遂牖_茉.<显示提示>d__0&)
		// Cysharp.Threading.Tasks.UniTask<object> Cysharp.Threading.Tasks.UniTask.FromResult<object>(object)
		// DG.Tweening.Core.TweenerCore<UnityEngine.Vector3,UnityEngine.Vector3,DG.Tweening.Plugins.Options.VectorOptions> DG.Tweening.TweenSettingsExtensions.From<UnityEngine.Vector3,UnityEngine.Vector3,DG.Tweening.Plugins.Options.VectorOptions>(DG.Tweening.Core.TweenerCore<UnityEngine.Vector3,UnityEngine.Vector3,DG.Tweening.Plugins.Options.VectorOptions>,UnityEngine.Vector3,bool,bool)
		// object DG.Tweening.TweenSettingsExtensions.SetDelay<object>(object,float)
		// object ES3.Deserialize<object>(byte[],ES3Settings)
		// object ES3.Load<object>(string)
		// object ES3.Load<object>(string,ES3Settings)
		// System.Void ES3.Save<object>(string,object)
		// System.Void ES3.Save<object>(string,object,ES3Settings)
		// object ES3File.Load<object>(string)
		// System.Void ES3File.Save<object>(string,object)
		// object ES3Reader.Read<object>(ES3Types.ES3Type)
		// object ES3Reader.Read<object>(string)
		// object ES3Reader.ReadObject<object>(ES3Types.ES3Type)
		// System.Type ES3Reader.ReadTypeFromHeader<object>()
		// object ES3Types.ES3Type.Read<object>(ES3Reader)
		// System.Void ES3Writer.Write<object>(string,object)
		// object LitJson.JsonMapper.ToObject<object>(string)
		// Cysharp.Threading.Tasks.UniTask<object> LoadYooAssetsTool.LoadAsset<object>(string,bool)
		// System.Void Nova.Events.EventDispatcher.RegisterHandler<Nova.Gesture.OnClick,object>(Nova.UIEventHandler<Nova.Gesture.OnClick,object>)
		// System.Void Nova.Events.EventDispatcher.UnregisterHandler<Nova.Gesture.OnClick,object>(Nova.UIEventHandler<Nova.Gesture.OnClick,object>)
		// System.Void Nova.Events.EventModule.HandOff<Nova.Gesture.OnClick>(Nova.Gesture.OnClick,Nova.Events.EventModule)
		// System.Void Nova.Events.EventModule.RegisterHandler<Nova.Gesture.OnClick>(Nova.UIEventHandler<Nova.Gesture.OnClick>,bool)
		// System.Void Nova.Events.EventModule.UnregisterHandler<Nova.Gesture.OnClick>(Nova.UIEventHandler<Nova.Gesture.OnClick>)
		// System.Void Nova.ListView.SetDataSource<object>(System.Collections.Generic.IList<object>)
		// System.Void Nova.UIBlock.AddEventHandler<Nova.Gesture.OnClick>(Nova.UIEventHandler<Nova.Gesture.OnClick>,bool)
		// System.Void Nova.UIBlock.AddGestureHandler<Nova.Gesture.OnClick>(Nova.UIEventHandler<Nova.Gesture.OnClick>,bool)
		// System.Void Nova.UIBlock.RemoveEventHandler<Nova.Gesture.OnClick>(Nova.UIEventHandler<Nova.Gesture.OnClick>)
		// System.Void Nova.UIBlock.RemoveGestureHandler<Nova.Gesture.OnClick>(Nova.UIEventHandler<Nova.Gesture.OnClick>)
		// object QFramework.Architecture<object>.GetUtility<object>()
		// System.Void QFramework.Architecture<object>.RegisterModel<object>(object)
		// System.Void QFramework.Architecture<object>.RegisterUtility<object>(object)
		// QFramework.IUnRegister QFramework.CanRegisterEventExtension.RegisterEvent<object>(QFramework.ICanRegisterEvent,System.Action<object>)
		// System.Void QFramework.CanRegisterEventExtension.UnRegisterEvent<object>(QFramework.ICanRegisterEvent,System.Action<object>)
		// System.Void QFramework.CanSendCommandExtension.SendCommand<object>(QFramework.ICanSendCommand,object)
		// System.Void QFramework.CanSendEventExtension.SendEvent<object>(QFramework.ICanSendEvent,object)
		// QFramework.IUnRegister QFramework.IArchitecture.RegisterEvent<object>(System.Action<object>)
		// System.Void QFramework.IArchitecture.SendCommand<object>(object)
		// System.Void QFramework.IArchitecture.SendEvent<object>(object)
		// System.Void QFramework.IArchitecture.UnRegisterEvent<object>(System.Action<object>)
		// System.Void QFramework.IOCContainer.Register<object>(object)
		// object QFramework.UnityEngineGameObjectExtension.Hide<object>(object)
		// object QFramework.UnityEngineGameObjectExtension.Show<object>(object)
		// object[] System.Array.Empty<object>()
		// System.Void System.Array.Reverse<byte>(byte[])
		// System.Void System.Array.Reverse<byte>(byte[],int,int)
		// bool System.Linq.Enumerable.Any<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Cast<object>(System.Collections.IEnumerable)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.CastIterator<object>(System.Collections.IEnumerable)
		// object System.Linq.Enumerable.FirstOrDefault<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// bool System.Linq.Enumerable.SequenceEqual<byte>(System.Collections.Generic.IEnumerable<byte>,System.Collections.Generic.IEnumerable<byte>)
		// bool System.Linq.Enumerable.SequenceEqual<byte>(System.Collections.Generic.IEnumerable<byte>,System.Collections.Generic.IEnumerable<byte>,System.Collections.Generic.IEqualityComparer<byte>)
		// System.Collections.Generic.IEnumerable<byte> System.Linq.Enumerable.Take<byte>(System.Collections.Generic.IEnumerable<byte>,int)
		// System.Collections.Generic.IEnumerable<byte> System.Linq.Enumerable.TakeIterator<byte>(System.Collections.Generic.IEnumerable<byte>,int)
		// object[] System.Linq.Enumerable.ToArray<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.List<object> System.Linq.Enumerable.ToList<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Where<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,右上角功能端PC_茉.<执行用户名管理>d__17>(Cysharp.Threading.Tasks.UniTask.Awaiter&,右上角功能端PC_茉.<执行用户名管理>d__17&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,安卓功能栏.<<寻>b__17_0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,安卓功能栏.<<寻>b__17_0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,安卓功能栏.<消寻>d__16>(Cysharp.Threading.Tasks.UniTask.Awaiter&,安卓功能栏.<消寻>d__16&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,条目组性_茉.<OnEnable>d__8>(Cysharp.Threading.Tasks.UniTask.Awaiter&,条目组性_茉.<OnEnable>d__8&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,条目组性_茉.<择>d__10>(Cysharp.Threading.Tasks.UniTask.Awaiter&,条目组性_茉.<择>d__10&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,条目组性_茉.<长生>d__7>(Cysharp.Threading.Tasks.UniTask.Awaiter&,条目组性_茉.<长生>d__7&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,用户显示条目_韩.<查看信息>d__15>(Cysharp.Threading.Tasks.UniTask.Awaiter&,用户显示条目_韩.<查看信息>d__15&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,登录界面_韩.<跳转界面>d__21>(Cysharp.Threading.Tasks.UniTask.Awaiter&,登录界面_韩.<跳转界面>d__21&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,评定人显示条目_韩.<查看信息>d__14>(Cysharp.Threading.Tasks.UniTask.Awaiter&,评定人显示条目_韩.<查看信息>d__14&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,PC端条目功能按钮_茉.<删除>d__5>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,PC端条目功能按钮_茉.<删除>d__5&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,备份管理界面_茉.<执行备份>d__7>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,备份管理界面_茉.<执行备份>d__7&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,备份管理界面_茉.<执行还原>d__8>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,备份管理界面_茉.<执行还原>d__8&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,界面管理界面_茉.<保存配置>d__15>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,界面管理界面_茉.<保存配置>d__15&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,登录界面_茉.<退出应用程序>d__28>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,登录界面_茉.<退出应用程序>d__28&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,蓝牙连接界面_茉.<订阅成功回调>d__16>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,蓝牙连接界面_茉.<订阅成功回调>d__16&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,评定人显示条目_韩.<删除信息>d__15>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,评定人显示条目_韩.<删除信息>d__15&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,LoadNewGameVesionUI.<OnClickConfimButton>d__5>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,LoadNewGameVesionUI.<OnClickConfimButton>d__5&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,PC端条目功能按钮_茉.<删除>d__5>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,PC端条目功能按钮_茉.<删除>d__5&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,图片选择组件.<LoadLastImage>d__8>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,图片选择组件.<LoadLastImage>d__8&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,备份管理界面_茉.<执行备份>d__7>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,备份管理界面_茉.<执行备份>d__7&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,备份管理界面_茉.<执行还原>d__8>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,备份管理界面_茉.<执行还原>d__8&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,安卓功能栏.<寻>d__17>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,安卓功能栏.<寻>d__17&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,界面管理器_韩.<加载界面>d__7>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,界面管理器_韩.<加载界面>d__7&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,界面管理界面_茉.<LoadConfigData>d__14>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,界面管理界面_茉.<LoadConfigData>d__14&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,登录界面_茉.<退出应用程序>d__28>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,登录界面_茉.<退出应用程序>d__28&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,登录界面_韩.<跳转界面>d__21>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,登录界面_韩.<跳转界面>d__21&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,蓝牙标识提示.<初始化蓝牙状态监听>d__6>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,蓝牙标识提示.<初始化蓝牙状态监听>d__6&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,蓝牙连接界面_茉.<订阅成功回调>d__16>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,蓝牙连接界面_茉.<订阅成功回调>d__16&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,评定人显示条目_韩.<删除信息>d__15>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,评定人显示条目_韩.<删除信息>d__15&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,选择界面_茉.<执行用户名管理>d__13>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,选择界面_茉.<执行用户名管理>d__13&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,选择界面_茉.<执行蓝牙连接>d__12>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,选择界面_茉.<执行蓝牙连接>d__12&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.YieldAwaitable.Awaiter,登录界面_茉.<跳转界面>d__29>(Cysharp.Threading.Tasks.YieldAwaitable.Awaiter&,登录界面_茉.<跳转界面>d__29&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<GameStartAfterLoad.<GameLoadedInit>d__1>(GameStartAfterLoad.<GameLoadedInit>d__1&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<LoadNewGameVesionUI.<OnClickConfimButton>d__5>(LoadNewGameVesionUI.<OnClickConfimButton>d__5&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<PC端条目功能按钮_茉.<删除>d__5>(PC端条目功能按钮_茉.<删除>d__5&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<右上角功能端PC_茉.<执行用户名管理>d__17>(右上角功能端PC_茉.<执行用户名管理>d__17&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<图片选择组件.<LoadLastImage>d__8>(图片选择组件.<LoadLastImage>d__8&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<备份管理界面_茉.<执行备份>d__7>(备份管理界面_茉.<执行备份>d__7&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<备份管理界面_茉.<执行还原>d__8>(备份管理界面_茉.<执行还原>d__8&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<安卓功能栏.<<寻>b__17_0>d>(安卓功能栏.<<寻>b__17_0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<安卓功能栏.<寻>d__17>(安卓功能栏.<寻>d__17&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<安卓功能栏.<消寻>d__16>(安卓功能栏.<消寻>d__16&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<条目组性_茉.<OnEnable>d__8>(条目组性_茉.<OnEnable>d__8&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<条目组性_茉.<择>d__10>(条目组性_茉.<择>d__10&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<条目组性_茉.<择>d__11>(条目组性_茉.<择>d__11&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<条目组性_茉.<长生>d__7>(条目组性_茉.<长生>d__7&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<用户显示条目_韩.<查看信息>d__15>(用户显示条目_韩.<查看信息>d__15&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<界面管理器_韩.<加载界面>d__7>(界面管理器_韩.<加载界面>d__7&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<界面管理界面_茉.<LoadConfigData>d__14>(界面管理界面_茉.<LoadConfigData>d__14&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<界面管理界面_茉.<保存配置>d__15>(界面管理界面_茉.<保存配置>d__15&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<界面管理界面_茉.<取消操作>d__16>(界面管理界面_茉.<取消操作>d__16&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<登录界面_茉.<跳转界面>d__29>(登录界面_茉.<跳转界面>d__29&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<登录界面_茉.<退出应用程序>d__28>(登录界面_茉.<退出应用程序>d__28&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<登录界面_韩.<跳转界面>d__21>(登录界面_韩.<跳转界面>d__21&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<蓝牙标识提示.<初始化蓝牙状态监听>d__6>(蓝牙标识提示.<初始化蓝牙状态监听>d__6&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<蓝牙连接界面_茉.<订阅成功回调>d__16>(蓝牙连接界面_茉.<订阅成功回调>d__16&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<评定人显示条目_韩.<删除信息>d__15>(评定人显示条目_韩.<删除信息>d__15&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<评定人显示条目_韩.<查看信息>d__14>(评定人显示条目_韩.<查看信息>d__14&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<选择界面_茉.<执行用户名管理>d__13>(选择界面_茉.<执行用户名管理>d__13&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<选择界面_茉.<执行蓝牙连接>d__12>(选择界面_茉.<执行蓝牙连接>d__12&)
		// byte& System.Runtime.CompilerServices.Unsafe.Add<byte>(byte&,int)
		// byte& System.Runtime.CompilerServices.Unsafe.As<byte,byte>(byte&)
		// object& System.Runtime.CompilerServices.Unsafe.As<object,object>(object&)
		// System.Void* System.Runtime.CompilerServices.Unsafe.AsPointer<object>(object&)
		// bool System.Runtime.CompilerServices.Unsafe.IsAddressLessThan<byte>(byte&,byte&)
		// object UnityEngine.AndroidJNIHelper.ConvertFromJNIArray<object>(System.IntPtr)
		// System.IntPtr UnityEngine.AndroidJNIHelper.GetFieldID<object>(System.IntPtr,string,bool)
		// System.IntPtr UnityEngine.AndroidJNIHelper.GetMethodID<object>(System.IntPtr,string,object[],bool)
		// object UnityEngine.AndroidJavaObject.Call<object>(string,object[])
		// object UnityEngine.AndroidJavaObject.CallStatic<object>(string,object[])
		// object UnityEngine.AndroidJavaObject.FromJavaArrayDeleteLocalRef<object>(System.IntPtr)
		// object UnityEngine.AndroidJavaObject.GetStatic<object>(string)
		// object UnityEngine.AndroidJavaObject._Call<object>(System.IntPtr,object[])
		// object UnityEngine.AndroidJavaObject._Call<object>(string,object[])
		// object UnityEngine.AndroidJavaObject._CallStatic<object>(System.IntPtr,object[])
		// object UnityEngine.AndroidJavaObject._CallStatic<object>(string,object[])
		// object UnityEngine.AndroidJavaObject._GetStatic<object>(System.IntPtr)
		// object UnityEngine.AndroidJavaObject._GetStatic<object>(string)
		// object UnityEngine.Component.GetComponent<object>()
		// object UnityEngine.Component.GetComponentInChildren<object>()
		// object UnityEngine.Component.GetComponentInParent<object>()
		// object[] UnityEngine.Component.GetComponents<object>()
		// object[] UnityEngine.Component.GetComponentsInChildren<object>()
		// object[] UnityEngine.Component.GetComponentsInChildren<object>(bool)
		// bool UnityEngine.Component.TryGetComponent<object>(object&)
		// object UnityEngine.GameObject.AddComponent<object>()
		// object UnityEngine.GameObject.GetComponent<object>()
		// object UnityEngine.GameObject.GetComponentInChildren<object>()
		// object UnityEngine.GameObject.GetComponentInChildren<object>(bool)
		// object[] UnityEngine.GameObject.GetComponents<object>()
		// object[] UnityEngine.GameObject.GetComponentsInChildren<object>(bool)
		// bool UnityEngine.GameObject.TryGetComponent<object>(object&)
		// object UnityEngine.Object.FindObjectOfType<object>()
		// object[] UnityEngine.Object.FindObjectsOfType<object>()
		// object UnityEngine.Object.Instantiate<object>(object)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform,bool)
		// object[] UnityEngine.Resources.ConvertObjects<object>(UnityEngine.Object[])
		// object UnityEngine._AndroidJNIHelper.ConvertFromJNIArray<object>(System.IntPtr)
		// System.IntPtr UnityEngine._AndroidJNIHelper.GetFieldID<object>(System.IntPtr,string,bool)
		// System.IntPtr UnityEngine._AndroidJNIHelper.GetMethodID<object>(System.IntPtr,string,object[],bool)
		// string UnityEngine._AndroidJNIHelper.GetSignature<object>(object[])
	}
}