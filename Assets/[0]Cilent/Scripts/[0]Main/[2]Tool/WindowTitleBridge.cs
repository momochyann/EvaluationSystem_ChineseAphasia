using System;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// 窗口标题桥接实现（主程序集）
/// 通过 Win32 API 设置窗口标题，供热更新层调用
/// </summary>
public class WindowTitleBridge : IWindowTitleBridge
{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool SetWindowText(IntPtr hWnd, string lpString);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr GetActiveWindow();
#endif

    public void SetTitle(string title, string defaultTitle = null)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        try
        {
            var targetTitle = string.IsNullOrWhiteSpace(title)
                ? (string.IsNullOrWhiteSpace(defaultTitle) ? Application.productName : defaultTitle)
                : title;

            var hwnd = GetActiveWindow();
            if (hwnd != IntPtr.Zero)
            {
                SetWindowText(hwnd, targetTitle);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"更新窗口标题失败: {e.Message}");
        }
#else
        Debug.Log($"SetTitle ignored on current platform. title={title}");
#endif
    }
}


