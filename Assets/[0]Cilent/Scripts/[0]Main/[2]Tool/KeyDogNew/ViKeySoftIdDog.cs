using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

/// <summary>
/// ViKey startup verification script:
/// 1) Optional UserSN binding before Find
/// 2) Admin login
/// 3) Read and verify SoftID
/// </summary>
public class ViKeySoftIdDog : MonoBehaviour
{
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    [DllImport("ViKey")]
    private static extern uint VikeySetUserSN(byte[] pUserSN);

    [DllImport("ViKey")]
    private static extern uint VikeyFind(ref uint pdwCount);

    [DllImport("ViKey")]
    private static extern uint VikeySetApiKey(ushort index, byte[] pApiKey);

    [DllImport("ViKey")]
    private static extern uint VikeyAdminLogin(ushort index, byte[] pAdminPassword);

    [DllImport("ViKey")]
    private static extern uint VikeyGetSoftIDString(ushort index, byte[] softIDString);

    [DllImport("ViKey")]
    private static extern uint VikeyLogoff(ushort index);

    [DllImport("ViKey")]
    private static extern uint VikeyUninitialization();

    private const uint ViKeySuccess = 0;

    [Header("ViKey Config")]
    [SerializeField] private bool setUserSNBeforeFind = false;
    [SerializeField] private string userSN = "";
    [SerializeField] private string apiKey = "";
    [SerializeField] private string adminPassword = "00000000";

    [Header("SoftID Check")]
    [SerializeField] private string expectedSoftId = "";
    [SerializeField] private bool usePlayerPrefsExpectedSoftId = true;
    [SerializeField] private string expectedSoftIdPlayerPrefsKey = "ViKeyExpectedSoftID";
    [SerializeField] private int softIdReadBufferLength = 64;

    [Header("Error Message")]
    [SerializeField] private string errorMessage = "加密狗验证出错！\r\n请联系厂家：18731600771";

    private void Awake()
    {
        UpdateWindowTitle();
        VerifyOnStartup();
    }

    private void UpdateWindowTitle()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        try
        {
            string configName = PlayerPrefs.GetString("sofewareName");
            string title = string.IsNullOrWhiteSpace(configName) ? "汉语失语症ABC法评估管理系统" : configName;
            var bridge = new WindowTitleBridge();
            bridge.SetTitle(title, "");
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[ViKeySoftIdDog] Update title failed: {ex.Message}");
        }
#endif
    }

    private void VerifyOnStartup()
    {
#if UNITY_EDITOR
        bool shouldRun = true;
#else
        bool shouldRun = Application.platform == RuntimePlatform.WindowsPlayer;
#endif
        if (!shouldRun)
        {
            Debug.Log("[ViKeySoftIdDog] Non-Windows platform, skip dongle check.");
            return;
        }

        try
        {
            bool ok = TryVerifySoftId(out string actualSoftId, out string expected, out string failStep, out uint failCode);
            if (ok)
            {
                Debug.Log($"[ViKeySoftIdDog] Verify success. SoftID: {actualSoftId}");
                return;
            }

            Debug.LogError($"[ViKeySoftIdDog] Verify failed. Step: {failStep}, Code: {failCode} (0x{failCode:X8}), Expected: {expected}, Actual: {actualSoftId}");
            ShowWindowsMessageAndExit(errorMessage);
        }
        catch (DllNotFoundException ex)
        {
            Debug.LogError($"[ViKeySoftIdDog] ViKey DLL not found: {ex.Message}");
            ShowWindowsMessageAndExit(errorMessage);
        }
        catch (EntryPointNotFoundException ex)
        {
            Debug.LogError($"[ViKeySoftIdDog] ViKey entry not found: {ex.Message}");
            ShowWindowsMessageAndExit(errorMessage);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ViKeySoftIdDog] Verify exception: {ex.Message}");
            ShowWindowsMessageAndExit(errorMessage);
        }
    }

    private bool TryVerifySoftId(out string actualSoftId, out string expectedSoftIdValue, out string failStep, out uint failCode)
    {
        actualSoftId = string.Empty;
        expectedSoftIdValue = ResolveExpectedSoftId();
        failStep = string.Empty;
        failCode = ViKeySuccess;

        ushort index = 0;
        bool adminLoginSucceeded = false;

        try
        {
            if (setUserSNBeforeFind)
            {
                if (string.IsNullOrWhiteSpace(userSN))
                {
                    failStep = "UserSNEmpty";
                    failCode = 1;
                    return false;
                }

                failStep = "VikeySetUserSN";
                failCode = VikeySetUserSN(Encoding.ASCII.GetBytes(userSN.Trim()));
                if (failCode != ViKeySuccess) return false;
            }

            uint count = 0;
            failStep = "VikeyFind";
            failCode = VikeyFind(ref count);
            if (failCode != ViKeySuccess || count == 0) return false;

            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                failStep = "VikeySetApiKey";
                failCode = VikeySetApiKey(index, Encoding.ASCII.GetBytes(apiKey.Trim()));
                if (failCode != ViKeySuccess) return false;
            }

            if (string.IsNullOrWhiteSpace(adminPassword))
            {
                failStep = "AdminPasswordEmpty";
                failCode = 1;
                return false;
            }

            failStep = "VikeyAdminLogin";
            failCode = VikeyAdminLogin(index, Encoding.ASCII.GetBytes(adminPassword.Trim()));
            if (failCode != ViKeySuccess) return false;
            adminLoginSucceeded = true;

            int bufferLength = Mathf.Clamp(softIdReadBufferLength, 8, 256);
            if (!string.IsNullOrWhiteSpace(expectedSoftIdValue))
            {
                bufferLength = Mathf.Clamp(Math.Max(bufferLength, expectedSoftIdValue.Length), 8, 256);
            }

            var buffer = new byte[bufferLength];
            failStep = "VikeyGetSoftIDString";
            failCode = VikeyGetSoftIDString(index, buffer);
            if (failCode != ViKeySuccess) return false;

            actualSoftId = Encoding.ASCII.GetString(buffer).TrimEnd('\0', ' ', '\r', '\n', '\t');
            Debug.Log("[ViKeySoftIdDog] Read SoftID: " + actualSoftId);

            if (string.IsNullOrWhiteSpace(expectedSoftIdValue))
            {
                return !string.IsNullOrWhiteSpace(actualSoftId);
            }

            return string.Equals(actualSoftId, expectedSoftIdValue, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            try
            {
                if (adminLoginSucceeded)
                {
                    VikeyLogoff(index);
                }
            }
            catch
            {
            }

            try
            {
                VikeyUninitialization();
            }
            catch
            {
            }
        }
    }

    private string ResolveExpectedSoftId()
    {
        if (!string.IsNullOrWhiteSpace(expectedSoftId))
        {
            return expectedSoftId.Trim();
        }

        if (!usePlayerPrefsExpectedSoftId || string.IsNullOrWhiteSpace(expectedSoftIdPlayerPrefsKey))
        {
            return string.Empty;
        }

        try
        {
            return PlayerPrefs.GetString(expectedSoftIdPlayerPrefsKey, string.Empty).Trim();
        }
        catch
        {
            return string.Empty;
        }
    }

    private void ShowWindowsMessageAndExit(string message)
    {
#if UNITY_EDITOR
        return;
#endif
        try
        {
            string msgEsc = (message ?? string.Empty).Replace("'", "''");
            string titleEsc = "提示";
            string psCmd = $"Add-Type -AssemblyName System.Windows.Forms; [System.Windows.Forms.MessageBox]::Show('{msgEsc}','{titleEsc}')";

            try
            {
                var shellPsi = new System.Diagnostics.ProcessStartInfo("cmd.exe", $"/C start \"\" powershell -NoProfile -Command \"{psCmd}\"")
                {
                    CreateNoWindow = true,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(shellPsi);
            }
            catch (Exception exProc)
            {
                Debug.LogWarning($"[ViKeySoftIdDog] Failed to launch external dialog: {exProc.Message}");
                try
                {
                    MessageBox(IntPtr.Zero, message, "提示", 0);
                }
                catch
                {
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[ViKeySoftIdDog] Show dialog exception: {ex.Message}");
        }

        try
        {
            Application.Quit();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[ViKeySoftIdDog] Application.Quit() failed: {ex.Message}");
        }

        try
        {
            var p = System.Diagnostics.Process.GetCurrentProcess();
            if (p != null && !p.HasExited)
            {
                try
                {
                    System.Threading.Thread.Sleep(50);
                }
                catch
                {
                }

                p.Kill();
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[ViKeySoftIdDog] Force kill failed: {ex.Message}");
        }
    }
}
