# 截图和PDF导出打印功能使用说明

## 功能概述

本系统为失语症评测系统添加了完整的截图、PDF导出和打印功能，支持：

- 🖼️ **全屏截图**：捕获整个屏幕内容
- 📷 **相机截图**：捕获指定相机的渲染内容  
- 📄 **PDF导出**：将截图转换为PDF格式
- 🖨️ **自动打印**：直接发送到打印机
- 📁 **文件管理**：自动整理和打开文件夹

## 快速使用

### 1. 快捷键操作

- **F12**: 快速截图
- **Ctrl + P**: 截图并导出PDF  
- **Ctrl + Shift + P**: 截图、导出PDF并打印

### 2. 代码调用

```csharp
// 快捷静态方法
ScreenshotQuickActions.CaptureNow();           // 立即截图
ScreenshotQuickActions.CapturePDFNow();        // 截图并导出PDF
ScreenshotQuickActions.CapturePrintNow();      // 截图、导出PDF并打印

// 异步方法
await ScreenshotQuickActions.CaptureNowAsync();
await ScreenshotQuickActions.CapturePDFNowAsync();
await ScreenshotQuickActions.CapturePrintNowAsync();
```

### 3. 通过工具类使用

```csharp
// 获取工具类实例
var utility = this.GetUtility<IScreenshotPDFUtility>();

// 截图
string imagePath = await utility.CaptureScreenshotAsync("我的截图");

// 截图并导出PDF
string pdfPath = await utility.CaptureAndExportToPDFAsync("我的PDF");

// 打印PDF
bool success = utility.PrintPDF(pdfPath);

// 打开文件夹
utility.OpenFileDirectory(pdfPath);
```

## 详细功能说明

### 截图功能

#### 全屏截图
```csharp
// 自动生成文件名
string filePath = await utility.CaptureScreenshotAsync();

// 指定文件名
string filePath = await utility.CaptureScreenshotAsync("测试截图");
```

#### 相机截图
```csharp
Camera targetCamera = Camera.main;
string filePath = await utility.CaptureCameraViewAsync(targetCamera, "相机截图");
```

### PDF导出功能

#### 图片转PDF
```csharp
// 将现有图片转换为PDF
string pdfPath = await utility.ConvertImageToPDFAsync(imagePath, "输出PDF");
```

#### 截图直接导出PDF
```csharp
// 一步完成截图和PDF导出
string pdfPath = await utility.CaptureAndExportToPDFAsync("完整操作");
```

### 打印功能

#### 直接打印PDF
```csharp
bool success = utility.PrintPDF(pdfPath);
if (success) {
    Debug.Log("已发送到打印机");
} else {
    Debug.LogError("打印失败");
}
```

#### 自动打印流程
```csharp
// 截图 -> 导出PDF -> 自动打印
string pdfPath = await utility.CaptureAndExportToPDFAsync();
if (!string.IsNullOrEmpty(pdfPath)) {
    utility.PrintPDF(pdfPath);
}
```

### 文件管理

#### 获取目录路径
```csharp
string screenshotDir = utility.GetExportDirectory();
Debug.Log($"截图保存在: {screenshotDir}");
```

#### 打开文件夹
```csharp
// 打开指定文件所在文件夹
utility.OpenFileDirectory(filePath);

// 打开默认导出文件夹
utility.OpenFileDirectory(null);
```

## UI组件使用

### ScreenshotPDFPanel

这是一个完整的UI面板，包含所有功能的按钮：

1. **截图按钮**: 基础截图功能
2. **导出PDF按钮**: 截图并生成PDF
3. **打印按钮**: 打印最后生成的文件
4. **打开文件夹按钮**: 查看生成的文件
5. **相机模式切换**: 选择全屏或相机截图

使用方法：
1. 在场景中创建UI Canvas
2. 添加ScreenshotPDFPanel组件
3. 在Inspector中关联UI控件
4. 运行即可使用

### ScreenshotExample

这是一个简单的示例组件，演示基本用法：

1. 挂载到任何GameObject
2. 支持快捷键操作
3. 可以绑定到UI按钮

## 文件组织

生成的文件会自动保存到以下位置：

```
应用数据目录/
├── Screenshots/          # 截图文件 (.png)
└── PDFs/                # PDF文件 (.pdf)
```

文件命名规则：
- 截图: `Screenshot_2024-01-01_12-30-45.png`
- PDF: `ScreenshotPDF_2024-01-01_12-30-45.pdf`
- 自定义: 使用您提供的文件名

## 系统要求

### Windows平台
- **推荐**: Windows 10/11 + Microsoft Word (用于最佳PDF生成效果)
- **最低**: Windows 7+ (使用HTML转PDF备用方案)

### PDF生成方式
1. **首选**: PowerShell + Microsoft Word (高质量PDF)
2. **备用**: HTML转PDF (需要浏览器手动操作)

### 打印支持
- Windows: 自动调用系统默认打印程序
- 其他平台: 打开文件供手动打印

## 故障排除

### 常见问题

#### 1. 截图失败
- 检查磁盘空间是否充足
- 确认应用有文件写入权限
- 检查文件路径是否包含特殊字符

#### 2. PDF生成失败
- 确认安装了Microsoft Word（推荐）
- 检查PowerShell执行权限
- 如果失败会自动生成HTML文件作为替代

#### 3. 打印失败
- 检查是否连接了打印机
- 确认打印机驱动正常
- 检查PDF文件是否存在

#### 4. 权限问题
```csharp
// 检查文件是否存在
if (File.Exists(filePath)) {
    Debug.Log("文件存在");
} else {
    Debug.LogError("文件不存在或无权限访问");
}
```

### 调试方法

启用详细日志：
```csharp
// 在Unity Console中查看详细信息
Debug.Log("开始截图操作...");
```

检查生成的文件：
```csharp
// 自动打开文件夹查看结果
utility.OpenFileDirectory(generatedFilePath);
```

## 扩展开发

### 添加自定义格式支持

```csharp
// 扩展接口以支持其他格式
public interface IScreenshotPDFUtility : IUtility
{
    UniTask<string> ConvertImageToCustomFormat(string imagePath, string format);
}
```

### 集成通知系统

```csharp
// 在ScreenshotExample中替换ShowNotification方法
private void ShowNotification(string message)
{
    // 替换为您的UI通知系统
    UINotificationManager.ShowMessage(message);
}
```

### 添加水印功能

```csharp
// 在截图后添加水印
private async UniTask AddWatermark(string imagePath)
{
    // 实现水印添加逻辑
}
```

## 更新历史

- **v1.0**: 基础截图和PDF导出功能
- **v1.1**: 添加相机截图支持
- **v1.2**: 增强PDF生成，支持多种备用方案
- **v1.3**: 完善打印功能和错误处理

## 技术支持

如有问题，请查看：
1. Unity Console中的错误日志
2. 生成文件的完整路径
3. 系统权限设置

---

**注意**: 此功能需要QFramework架构支持，确保已正确注册相关工具类。 