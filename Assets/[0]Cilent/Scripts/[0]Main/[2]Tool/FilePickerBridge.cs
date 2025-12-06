using System;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// 文件选择桥接类实现（主程序集）
/// 使用Windows API实现文件选择对话框
/// </summary>
public class FilePickerBridge : IFilePickerBridge
{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private class OpenFileName
    {
        public int lStructSize;
        public IntPtr hwndOwner;
        public IntPtr hInstance;
        public string lpstrFilter;
        public string lpstrCustomFilter;
        public int nMaxCustFilter;
        public int nFilterIndex;
        public string lpstrFile;
        public int nMaxFile;
        public string lpstrFileTitle;
        public int nMaxFileTitle;
        public string lpstrInitialDir;
        public string lpstrTitle;
        public int Flags;
        public short nFileOffset;
        public short nFileExtension;
        public string lpstrDefExt;
        public IntPtr lCustData;
        public IntPtr lpfnHook;
        public string lpTemplateName;
    }

    [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName([In, Out] OpenFileName ofn);
    
    [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool GetSaveFileName([In, Out] OpenFileName ofn);

    /// <summary>
    /// 打开文件选择对话框
    /// </summary>
    public string OpenFileDialog(string title = "选择文件", string filter = null, string initialDir = null)
    {
        try
        {
            // 默认过滤器：图片文件
            if (filter == null)
            {
                filter = "图片文件\0*.png;*.jpg;*.jpeg;*.bmp;*.tga;*.gif\0所有文件\0*.*\0";
            }
            else
            {
                // 将Unity风格的过滤器转换为Windows格式
                // Unity格式: "图片文件|*.png;*.jpg"
                // Windows格式: "图片文件\0*.png;*.jpg\0"
                filter = filter.Replace("|", "\0") + "\0";
            }

            OpenFileName ofn = new OpenFileName();
            ofn.lStructSize = Marshal.SizeOf(ofn);
            ofn.hwndOwner = IntPtr.Zero;
            ofn.lpstrFilter = filter;
            ofn.lpstrFile = new string(new char[260]);
            ofn.nMaxFile = ofn.lpstrFile.Length;
            ofn.lpstrFileTitle = new string(new char[260]);
            ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
            ofn.lpstrInitialDir = initialDir ?? Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            ofn.lpstrTitle = title;
            ofn.Flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008; // OFN_EXPLORER | OFN_FILEMUSTEXIST | OFN_PATHMUSTEXIST | OFN_NOCHANGEDIR

            if (GetOpenFileName(ofn))
            {
                return ofn.lpstrFile;
            }
            else
            {
                int error = Marshal.GetLastWin32Error();
                if (error != 0) // 0通常表示用户取消
                {
                    Debug.LogWarning($"文件选择对话框错误: {error}");
                }
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"打开文件选择对话框失败: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 打开保存文件对话框
    /// </summary>
    public string SaveFileDialog(string title = "保存文件", string defaultFileName = "备份.json", string filter = null, string initialDir = null)
    {
        try
        {
            // 默认过滤器：JSON文件
            if (filter == null)
            {
                filter = "JSON文件\0*.json\0所有文件\0*.*\0";
            }
            else
            {
                // 将Unity风格的过滤器转换为Windows格式
                filter = filter.Replace("|", "\0") + "\0";
            }

            OpenFileName ofn = new OpenFileName();
            ofn.lStructSize = Marshal.SizeOf(ofn);
            ofn.hwndOwner = IntPtr.Zero;
            ofn.lpstrFilter = filter;
            ofn.lpstrFile = defaultFileName ?? "备份.json";
            ofn.nMaxFile = 260;
            ofn.lpstrFileTitle = new string(new char[260]);
            ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
            ofn.lpstrInitialDir = initialDir ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            ofn.lpstrTitle = title;
            ofn.lpstrDefExt = "json";
            ofn.Flags = 0x00080000 | 0x00000008 | 0x00000002; // OFN_EXPLORER | OFN_NOCHANGEDIR | OFN_OVERWRITEPROMPT

            if (GetSaveFileName(ofn))
            {
                return ofn.lpstrFile;
            }
            else
            {
                int error = Marshal.GetLastWin32Error();
                if (error != 0) // 0通常表示用户取消
                {
                    Debug.LogWarning($"保存文件对话框错误: {error}");
                }
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"打开保存文件对话框失败: {e.Message}");
            return null;
        }
    }
#else
    /// <summary>
    /// 非Windows平台不支持
    /// </summary>
    public string OpenFileDialog(string title = "选择文件", string filter = null, string initialDir = null)
    {
        Debug.LogWarning("文件选择对话框仅在Windows平台支持");
        return null;
    }
    
    /// <summary>
    /// 非Windows平台不支持
    /// </summary>
    public string SaveFileDialog(string title = "保存文件", string defaultFileName = "备份.json", string filter = null, string initialDir = null)
    {
        Debug.LogWarning("保存文件对话框仅在Windows平台支持");
        return null;
    }
#endif
}

