using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine.Scripting;

/// <summary>
/// 打印桥接实现（主程序集）
/// 所有 DllImport 都放在这里，热更层只通过接口调用。
/// </summary>
[Preserve]
public class PrinterBridge : IPrinterBridge
{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    [DllImport("user32.dll")]
    private static extern IntPtr SetThreadDpiAwarenessContext(IntPtr dpiContext);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr ShellExecute(
        IntPtr hwnd,
        string lpOperation,
        string lpFile,
        string lpParameters,
        string lpDirectory,
        int nShowCmd);

    [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool SetDefaultPrinter(string pszPrinter);

    [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool GetDefaultPrinter(StringBuilder pszBuffer, ref int pcchBuffer);

    [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool EnumPrinters(
        uint flags,
        string name,
        uint level,
        IntPtr pPrinterEnum,
        uint cbBuf,
        out uint pcbNeeded,
        out uint pcReturned);

    private static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = new IntPtr(-4);
    private const uint PRINTER_ENUM_LOCAL = 0x00000002;
    private const uint PRINTER_ENUM_CONNECTIONS = 0x00000004;
    private const int SW_HIDE = 0;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct PRINTER_INFO_4
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pPrinterName;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string pServerName;

        public uint Attributes;
    }

    private static Type GetTypeAny(params string[] assemblyQualifiedTypeNames)
    {
        if (assemblyQualifiedTypeNames == null)
        {
            return null;
        }

        for (int i = 0; i < assemblyQualifiedTypeNames.Length; i++)
        {
            string n = assemblyQualifiedTypeNames[i];
            if (string.IsNullOrEmpty(n))
            {
                continue;
            }

            var t = Type.GetType(n, false);
            if (t != null)
            {
                return t;
            }

            // 回退：从已加载程序集扫描（有些环境下 Type.GetType 拿不到）
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int j = 0; j < assemblies.Length; j++)
            {
                try
                {
                    t = assemblies[j]?.GetType(n, false);
                    if (t != null)
                    {
                        return t;
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        return null;
    }

    private static bool EnsureWinFormsAndPrintingAvailable(out string reason)
    {
        reason = null;

        var missing = new List<string>();

        if (GetTypeAny("System.Windows.Forms.Application, System.Windows.Forms") == null) missing.Add("System.Windows.Forms.Application");
        if (GetTypeAny("System.Windows.Forms.Form, System.Windows.Forms") == null) missing.Add("System.Windows.Forms.Form");
        if (GetTypeAny("System.Windows.Forms.Label, System.Windows.Forms") == null) missing.Add("System.Windows.Forms.Label");
        if (GetTypeAny("System.Windows.Forms.ComboBox, System.Windows.Forms") == null) missing.Add("System.Windows.Forms.ComboBox");
        if (GetTypeAny("System.Windows.Forms.Button, System.Windows.Forms") == null) missing.Add("System.Windows.Forms.Button");
        if (GetTypeAny("System.Windows.Forms.Control, System.Windows.Forms") == null) missing.Add("System.Windows.Forms.Control");
        if (GetTypeAny("System.Windows.Forms.DialogResult, System.Windows.Forms") == null) missing.Add("System.Windows.Forms.DialogResult");
        if (GetTypeAny("System.Windows.Forms.FormStartPosition, System.Windows.Forms") == null) missing.Add("System.Windows.Forms.FormStartPosition");
        if (GetTypeAny("System.Windows.Forms.FormBorderStyle, System.Windows.Forms") == null) missing.Add("System.Windows.Forms.FormBorderStyle");
        if (GetTypeAny("System.Windows.Forms.AutoScaleMode, System.Windows.Forms") == null) missing.Add("System.Windows.Forms.AutoScaleMode");
        if (GetTypeAny("System.Windows.Forms.ComboBoxStyle, System.Windows.Forms") == null) missing.Add("System.Windows.Forms.ComboBoxStyle");
        if (GetTypeAny("System.Drawing.Size, System.Drawing", "System.Drawing.Size, System.Drawing.Primitives") == null) missing.Add("System.Drawing.Size");
        if (GetTypeAny("System.Drawing.Point, System.Drawing", "System.Drawing.Point, System.Drawing.Primitives") == null) missing.Add("System.Drawing.Point");
        if (GetTypeAny("System.Drawing.Printing.PrinterSettings, System.Drawing", "System.Drawing.Printing.PrinterSettings, System.Drawing.Common") == null) missing.Add("System.Drawing.Printing.PrinterSettings");

        if (missing.Count == 0)
        {
            return true;
        }

        string loadedAssemblies;
        try
        {
            var names = new List<string>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                var n = assemblies[i]?.GetName()?.Name;
                if (!string.IsNullOrEmpty(n))
                {
                    names.Add(n);
                }
            }
            loadedAssemblies = string.Join(", ", names);
        }
        catch
        {
            loadedAssemblies = "unknown";
        }

        reason = $"缺失类型: {string.Join(", ", missing)}; 已加载程序集: {loadedAssemblies}";
        return false;
    }
#endif

    public string GetDefaultPrinterName()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        try
        {
            int needed = 0;
            GetDefaultPrinter(null, ref needed);
            if (needed <= 0)
            {
                return null;
            }

            var sb = new StringBuilder(needed);
            if (GetDefaultPrinter(sb, ref needed))
            {
                return sb.ToString();
            }
        }
        catch
        {
            // ignored
        }
#endif
        return null;
    }

    public bool SetDefaultPrinterName(string printerName)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        if (string.IsNullOrEmpty(printerName))
        {
            return false;
        }

        try
        {
            return SetDefaultPrinter(printerName);
        }
        catch
        {
            return false;
        }
#else
        return false;
#endif
    }

    public List<string> GetInstalledPrinters()
    {
        var printers = new List<string>();
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        try
        {
            uint needed = 0;
            uint returned = 0;
            uint flags = PRINTER_ENUM_LOCAL | PRINTER_ENUM_CONNECTIONS;

            EnumPrinters(flags, null, 4, IntPtr.Zero, 0, out needed, out returned);
            if (needed == 0)
            {
                return printers;
            }

            IntPtr buffer = Marshal.AllocHGlobal((int)needed);
            try
            {
                if (!EnumPrinters(flags, null, 4, buffer, needed, out needed, out returned))
                {
                    return printers;
                }

                int structSize = Marshal.SizeOf(typeof(PRINTER_INFO_4));
                for (int i = 0; i < returned; i++)
                {
                    IntPtr itemPtr = IntPtr.Add(buffer, i * structSize);
                    var info = (PRINTER_INFO_4)Marshal.PtrToStructure(itemPtr, typeof(PRINTER_INFO_4));
                    if (!string.IsNullOrEmpty(info.pPrinterName) && !printers.Contains(info.pPrinterName))
                    {
                        printers.Add(info.pPrinterName);
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
        catch
        {
            // ignored
        }
#endif
        return printers;
    }

    [Preserve]
    public bool ShellPrintFile(string filePath)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        if (string.IsNullOrEmpty(filePath))
        {
            return false;
        }

        try
        {
            string fullPath = Path.GetFullPath(filePath);
            string workDir = Path.GetDirectoryName(fullPath) ?? string.Empty;

            IntPtr ret = ShellExecute(IntPtr.Zero, "print", fullPath, null, workDir, SW_HIDE);
            long code = ret.ToInt64();
            if (code <= 32)
            {
                int winErr = Marshal.GetLastWin32Error();
                UnityEngine.Debug.LogWarning($"[PrinterBridge] Shell print失败, code={code}, lastError={winErr}, file={fullPath}");
                return false;
            }

            Thread.Sleep(1200);
            return true;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogWarning($"[PrinterBridge] Shell print失败: {e.GetType().Name}: {e.Message}");
            return false;
        }
#else
        return false;
#endif
    }

    [Preserve]
    public bool ShellPrintToFile(string filePath, string printerName)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(printerName))
        {
            return false;
        }

        try
        {
            string fullPath = Path.GetFullPath(filePath);
            string workDir = Path.GetDirectoryName(fullPath) ?? string.Empty;
            string args = $"\"{printerName}\"";
            IntPtr ret = ShellExecute(IntPtr.Zero, "printto", fullPath, args, workDir, SW_HIDE);
            long code = ret.ToInt64();
            if (code <= 32)
            {
                int winErr = Marshal.GetLastWin32Error();
                UnityEngine.Debug.LogWarning($"[PrinterBridge] Shell printto失败, code={code}, lastError={winErr}, file={fullPath}, printer={printerName}");
                return false;
            }

            Thread.Sleep(1200);
            return true;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogWarning($"[PrinterBridge] Shell printto失败: {e.GetType().Name}: {e.Message}");
            return false;
        }
#else
        return false;
#endif
    }

    [Preserve]
    public bool PrintHtmlFile(string htmlFilePath)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        if (string.IsNullOrEmpty(htmlFilePath))
        {
            return false;
        }

        try
        {
            string fullHtmlPath = Path.GetFullPath(htmlFilePath);
            string fileUrl = new Uri(fullHtmlPath).AbsoluteUri;
            string systemDir = Environment.SystemDirectory;
            string rundll32Path = Path.Combine(systemDir, "rundll32.exe");
            string mshtmlPath = Path.Combine(systemDir, "mshtml.dll");

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = File.Exists(rundll32Path) ? rundll32Path : "rundll32.exe",
                Arguments = $"\"{mshtmlPath}\",PrintHTML \"{fileUrl}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = Path.GetDirectoryName(fullHtmlPath) ?? string.Empty
            };

            using (Process p = Process.Start(startInfo))
            {
                if (p == null)
                {
                    return false;
                }

                p.WaitForExit(15000);
            }

            Thread.Sleep(800);
            return true;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogWarning($"[PrinterBridge] PrintHTML失败: {e.GetType().Name}: {e.Message}");
            return false;
        }
#else
        return false;
#endif
    }

    public string SelectPrinterWithCustomDialog()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        string selectedPrinter = null;
        Exception dialogException = null;
        ManualResetEvent doneEvent = new ManualResetEvent(false);

        Thread dialogThread = new Thread(() =>
        {
            IntPtr oldDpiContext = IntPtr.Zero;
            object form = null;
            try
            {
                oldDpiContext = SetThreadDpiAwarenessContextPerMonitorAwareV2();

                if (!EnsureWinFormsAndPrintingAvailable(out string reason))
                {
                    throw new Exception($"无法加载WinForms或Printing相关类型: {reason}");
                }

                var applicationType = GetTypeAny("System.Windows.Forms.Application, System.Windows.Forms");
                var formType = GetTypeAny("System.Windows.Forms.Form, System.Windows.Forms");
                var labelType = GetTypeAny("System.Windows.Forms.Label, System.Windows.Forms");
                var comboBoxType = GetTypeAny("System.Windows.Forms.ComboBox, System.Windows.Forms");
                var buttonType = GetTypeAny("System.Windows.Forms.Button, System.Windows.Forms");
                var controlType = GetTypeAny("System.Windows.Forms.Control, System.Windows.Forms");
                var dialogResultType = GetTypeAny("System.Windows.Forms.DialogResult, System.Windows.Forms");
                var formStartPositionType = GetTypeAny("System.Windows.Forms.FormStartPosition, System.Windows.Forms");
                var formBorderStyleType = GetTypeAny("System.Windows.Forms.FormBorderStyle, System.Windows.Forms");
                var autoScaleModeType = GetTypeAny("System.Windows.Forms.AutoScaleMode, System.Windows.Forms");
                var comboBoxStyleType = GetTypeAny("System.Windows.Forms.ComboBoxStyle, System.Windows.Forms");
                var sizeType = GetTypeAny("System.Drawing.Size, System.Drawing", "System.Drawing.Size, System.Drawing.Primitives");
                var pointType = GetTypeAny("System.Drawing.Point, System.Drawing", "System.Drawing.Point, System.Drawing.Primitives");
                var printerSettingsType = GetTypeAny("System.Drawing.Printing.PrinterSettings, System.Drawing", "System.Drawing.Printing.PrinterSettings, System.Drawing.Common");

                if (formType == null || labelType == null || comboBoxType == null || buttonType == null ||
                    controlType == null || dialogResultType == null || formStartPositionType == null ||
                    formBorderStyleType == null || autoScaleModeType == null || comboBoxStyleType == null ||
                    sizeType == null || pointType == null || printerSettingsType == null)
                {
                    throw new Exception("自定义打印窗口依赖类型缺失");
                }

                applicationType?.GetMethod("EnableVisualStyles", Type.EmptyTypes)?.Invoke(null, null);

                form = Activator.CreateInstance(formType);
                formType.GetProperty("Text")?.SetValue(form, "选择打印机");
                formType.GetProperty("StartPosition")?.SetValue(form, Enum.ToObject(formStartPositionType, 1)); // CenterScreen
                formType.GetProperty("FormBorderStyle")?.SetValue(form, Enum.ToObject(formBorderStyleType, 1)); // FixedDialog
                formType.GetProperty("AutoScaleMode")?.SetValue(form, Enum.ToObject(autoScaleModeType, 0)); // None
                formType.GetProperty("MaximizeBox")?.SetValue(form, false);
                formType.GetProperty("MinimizeBox")?.SetValue(form, false);
                formType.GetProperty("TopMost")?.SetValue(form, true);
                formType.GetProperty("ShowInTaskbar")?.SetValue(form, false);
                formType.GetProperty("ClientSize")?.SetValue(form, Activator.CreateInstance(sizeType, 460, 180));

                object label = Activator.CreateInstance(labelType);
                controlType.GetProperty("Text")?.SetValue(label, "打印机");
                controlType.GetProperty("Location")?.SetValue(label, Activator.CreateInstance(pointType, 20, 25));
                controlType.GetProperty("Size")?.SetValue(label, Activator.CreateInstance(sizeType, 60, 24));

                object combo = Activator.CreateInstance(comboBoxType);
                controlType.GetProperty("Location")?.SetValue(combo, Activator.CreateInstance(pointType, 80, 22));
                controlType.GetProperty("Size")?.SetValue(combo, Activator.CreateInstance(sizeType, 350, 28));
                comboBoxType.GetProperty("DropDownStyle")?.SetValue(combo, Enum.ToObject(comboBoxStyleType, 2)); // DropDownList

                var installedPrinters = printerSettingsType.GetProperty("InstalledPrinters")?.GetValue(null);
                var items = comboBoxType.GetProperty("Items")?.GetValue(combo);
                var addItemMethod = items?.GetType().GetMethod("Add", new[] { typeof(object) });
                int printerCount = 0;
                if (installedPrinters is IEnumerable printerEnumerable && addItemMethod != null)
                {
                    foreach (var p in printerEnumerable)
                    {
                        addItemMethod.Invoke(items, new object[] { p?.ToString() ?? string.Empty });
                        printerCount++;
                    }
                }

                if (printerCount == 0)
                {
                    throw new Exception("未检测到可用打印机");
                }

                object defaultPrinterSettings = Activator.CreateInstance(printerSettingsType);
                string defaultPrinterName = printerSettingsType.GetProperty("PrinterName")?.GetValue(defaultPrinterSettings) as string;
                if (!string.IsNullOrEmpty(defaultPrinterName))
                {
                    comboBoxType.GetProperty("SelectedItem")?.SetValue(combo, defaultPrinterName);
                }

                int selectedIndex = (int)(comboBoxType.GetProperty("SelectedIndex")?.GetValue(combo) ?? -1);
                if (selectedIndex < 0)
                {
                    comboBoxType.GetProperty("SelectedIndex")?.SetValue(combo, 0);
                }

                object okButton = Activator.CreateInstance(buttonType);
                controlType.GetProperty("Text")?.SetValue(okButton, "确定");
                controlType.GetProperty("Location")?.SetValue(okButton, Activator.CreateInstance(pointType, 250, 120));
                controlType.GetProperty("Size")?.SetValue(okButton, Activator.CreateInstance(sizeType, 80, 30));
                buttonType.GetProperty("DialogResult")?.SetValue(okButton, dialogResultType.GetField("OK")?.GetValue(null));

                object cancelButton = Activator.CreateInstance(buttonType);
                controlType.GetProperty("Text")?.SetValue(cancelButton, "取消");
                controlType.GetProperty("Location")?.SetValue(cancelButton, Activator.CreateInstance(pointType, 350, 120));
                controlType.GetProperty("Size")?.SetValue(cancelButton, Activator.CreateInstance(sizeType, 80, 30));
                buttonType.GetProperty("DialogResult")?.SetValue(cancelButton, dialogResultType.GetField("Cancel")?.GetValue(null));

                var controls = formType.GetProperty("Controls")?.GetValue(form);
                var addControlMethod = controls?.GetType().GetMethod("Add", new[] { controlType });
                addControlMethod?.Invoke(controls, new[] { label });
                addControlMethod?.Invoke(controls, new[] { combo });
                addControlMethod?.Invoke(controls, new[] { okButton });
                addControlMethod?.Invoke(controls, new[] { cancelButton });

                formType.GetProperty("AcceptButton")?.SetValue(form, okButton);
                formType.GetProperty("CancelButton")?.SetValue(form, cancelButton);

                object result = formType.GetMethod("ShowDialog", Type.EmptyTypes)?.Invoke(form, null);
                object okValue = dialogResultType.GetField("OK")?.GetValue(null);

                if (result != null && okValue != null && Convert.ToInt32(result) == Convert.ToInt32(okValue))
                {
                    var selectedItem = comboBoxType.GetProperty("SelectedItem")?.GetValue(combo);
                    selectedPrinter = selectedItem?.ToString();
                }
            }
            catch (Exception ex)
            {
                dialogException = ex;
            }
            finally
            {
                if (form is IDisposable disposableForm)
                {
                    disposableForm.Dispose();
                }

                RestoreThreadDpiAwarenessContext(oldDpiContext);
                doneEvent.Set();
            }
        });

        dialogThread.SetApartmentState(ApartmentState.STA);
        dialogThread.Start();
        doneEvent.WaitOne();

        if (dialogException != null)
        {
            UnityEngine.Debug.LogWarning($"[PrinterBridge] 自定义打印机选择窗口失败: {dialogException.Message}\n{dialogException.StackTrace}");
            return null;
        }

        return selectedPrinter;
#else
        return null;
#endif
    }

    public IntPtr SetThreadDpiAwarenessContextPerMonitorAwareV2()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        try
        {
            return SetThreadDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
        }
        catch
        {
            return IntPtr.Zero;
        }
#else
        return IntPtr.Zero;
#endif
    }

    public void RestoreThreadDpiAwarenessContext(IntPtr oldContext)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        if (oldContext == IntPtr.Zero)
        {
            return;
        }
        try
        {
            SetThreadDpiAwarenessContext(oldContext);
        }
        catch
        {
            // ignored
        }
#endif
    }
}
