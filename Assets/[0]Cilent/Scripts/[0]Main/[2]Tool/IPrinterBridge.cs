using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

/// <summary>
/// 打印桥接接口（主程序集）
/// 供 HybridCLR 热更新代码调用，避免在热更程序集直接执行 DllImport。
/// </summary>
[Preserve]
public interface IPrinterBridge
{
    /// <summary>
    /// 获取默认打印机名称
    /// </summary>
    string GetDefaultPrinterName();

    /// <summary>
    /// 设置默认打印机
    /// </summary>
    bool SetDefaultPrinterName(string printerName);

    /// <summary>
    /// 弹出自定义打印机选择窗口并返回用户选择
    /// </summary>
    string SelectPrinterWithCustomDialog();

    /// <summary>
    /// 获取系统已安装打印机列表
    /// </summary>
    List<string> GetInstalledPrinters();

    /// <summary>
    /// 使用系统 Shell 的 print 动词打印文件
    /// </summary>
    bool ShellPrintFile(string filePath);

    /// <summary>
    /// 使用系统 Shell 的 printto 动词打印到指定打印机
    /// </summary>
    bool ShellPrintToFile(string filePath, string printerName);

    /// <summary>
    /// 使用 rundll32+mshtml 的 PrintHTML 打印 html 文件
    /// </summary>
    bool PrintHtmlFile(string htmlFilePath);

    /// <summary>
    /// 提升线程 DPI 感知（可选）
    /// </summary>
    IntPtr SetThreadDpiAwarenessContextPerMonitorAwareV2();

    /// <summary>
    /// 恢复线程 DPI 感知（可选）
    /// </summary>
    void RestoreThreadDpiAwarenessContext(IntPtr oldContext);
}
