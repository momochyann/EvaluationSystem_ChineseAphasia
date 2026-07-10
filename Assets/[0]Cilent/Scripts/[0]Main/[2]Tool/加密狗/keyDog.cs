using UnityEngine;
using System;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using SerialPortUtility;
using System.IO.Ports;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Win32;
using System.Text;

/// <summary>
/// keyDog 引导脚本（合并加密狗检测功能，绕开 QFramework/YooAssets）
/// - 在应用启动前加载（BeforeSceneLoad）
/// - 直接创建并绑定串口工具组件，第一时间执行检测
/// - 检测失败时显示 Windows 原生 MessageBox 并退出应用
/// </summary>
public class keyDog : MonoBehaviour
{
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitBeforeSceneLoad()
    {
        // 仅在 Unity 编辑器或 Windows 平台上初始化
#if UNITY_EDITOR
        bool shouldInit = true;
#else
        bool shouldInit = Application.platform == RuntimePlatform.WindowsPlayer;
#endif
        if (!shouldInit)
        {
            Debug.Log("[keyDog] 非 Windows 平台，跳过 keyDog 初始化");
            return;
        }

        try
        {
            var go = new GameObject("KeyDogBootstrap");
            GameObject.DontDestroyOnLoad(go);
            go.AddComponent<keyDog>();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[keyDog] 初始化失败: {ex.Message}");
        }
    }

    private void Awake()
    {
        // 启动异步检测，不阻塞主线程
        启动检测流程().Forget();
        更新窗口标题();
        try
        {
            Application.wantsToQuit += OnWantsToQuit;
        }
        catch { }
    }

    private void OnDestroy()
    {
        try
        {
            Application.wantsToQuit -= OnWantsToQuit;
        }
        catch { }
    }

    private bool OnWantsToQuit()
    {
        try
        {
            CloseNativeDialogs();
            // give a short moment for dialogs to close
            try { System.Threading.Thread.Sleep(100); } catch { }
        }
        catch { }
        return true; // allow quit
    }

    // Ensure any native dialog windows are closed when application quits to avoid errors
    private void OnApplicationQuit()
    {
        try
        {
            CloseNativeDialogs();
        }
        catch { }
    }

    private void CloseNativeDialogs()
    {
        try
        {
            const int max = 512;
            var sb = new StringBuilder(512);
            EnumWindows((hWnd, lParam) =>
            {
                try
                {
                    if (!IsWindowVisible(hWnd)) return true;
                    sb.Clear();
                    GetClassName(hWnd, sb, max);
                    string cls = sb.ToString();
                    sb.Clear();
                    GetWindowText(hWnd, sb, max);
                    string title = sb.ToString();

                    // Common dialog class is #32770 (Dialog). Close dialogs with title containing 提示 or containing "加密狗"
                    if (cls == "#32770" || title.Contains("提示") || title.Contains("加密狗"))
                    {
                        SendMessage(hWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                    }
                }
                catch { }
                return true;
            }, IntPtr.Zero);
        }
        catch { }
    }

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);
    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
    private const uint WM_CLOSE = 0x0010;

    private async UniTask 启动检测流程()
    {
        // 极短延迟以确保其它系统完成基础初始化
        await UniTask.Delay(20);

        try
        {
            // 使用注册表枚举 COM 端口（不依赖 System.IO.Ports）
            var rawPorts = GetComPortsFromRegistry();

            // 先尝试历史保存的端口（仅尝试一次，不做握手）
            string savedPort = "";
            try
            {
                savedPort = PlayerPrefs.GetString(SAVED_PORT_KEY, "");
            }
            catch { }

            if (!string.IsNullOrEmpty(savedPort))
            {
                Debug.Log($"[keyDog] 尝试使用已保存端口（跳过握手）: {savedPort}");
                try
                {
                    bool ok = TryAuthenticatePortSync(savedPort, true);
                    if (ok)
                    {
                        Debug.Log($"[keyDog] 已保存端口 {savedPort} 直接解锁成功");
                        return;
                    }
                    else
                    {
                        Debug.LogWarning($"[keyDog] 已保存端口 {savedPort} 直接解锁失败，将进行全量扫描并握手");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[keyDog] 已保存端口 {savedPort} 直接验证异常: {ex.Message}，将进行全量扫描并握手");
                }
            }

            // 若保存端口不可用或未保存，则扫描系统端口并逐一握手与解锁
            if (rawPorts == null || rawPorts.Length == 0)
            {
                ShowWindowsMessageAndExit("加密狗验证出错！\r\n请联系厂家：18731600771");
                return;
            }

            Debug.Log($"[keyDog] 扫描到串口: {string.Join(",", rawPorts)}");

            bool success = false;
            foreach (var port in rawPorts)
            {
                try
                {
                    success = TryAuthenticatePortSync(port, false);
                    if (success) break;
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[keyDog] 端口 {port} 验证异常: {ex.Message}");
                }
            }

            if (!success)
            {
                ShowWindowsMessageAndExit("加密狗验证出错！\r\n请联系厂家：18731600771");
                return;
            }

            Debug.Log("[keyDog] 加密狗检测并解锁成功");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[keyDog] 检测流程异常: {ex.Message}");
            ShowWindowsMessageAndExit("加密狗验证出错！\r\n请联系厂家：18731600771");
        }
    }

    private Type FindTypeByName(string typeName)
    {
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                var t = asm.GetType(typeName);
                if (t != null) return t;
                // 也允许按短名匹配
                var types = asm.GetTypes();
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i].Name == typeName) return types[i];
                }
            }
            catch { }
        }
        return null;
    }

    private async UniTask<bool> InvokeAsyncBoolMethod(object target, string methodName, object[] args = null)
    {
        if (target == null) return false;
        try
        {
            var mi = target.GetType().GetMethod(methodName);
            if (mi == null) return false;
            object taskObj = mi.Invoke(target, args);
            if (taskObj == null) return false;

            // 常见返回类型：UniTask<bool> 或 Task<bool>
            if (taskObj is UniTask<bool> uniBool)
            {
                return await uniBool;
            }
            if (taskObj is System.Threading.Tasks.Task<bool> tBool)
            {
                return await tBool;
            }

            // 尝试动态 await（宽松回退）
            try
            {
                dynamic dyn = taskObj;
                bool res = await dyn;
                return res;
            }
            catch { return false; }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[keyDog] 调用异步方法 {methodName} 失败: {ex.Message}");
            return false;
        }
    }
    private void 更新窗口标题()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        try
        {
            var 配置 = PlayerPrefs.GetString("sofewareName");
            var 标题 = string.IsNullOrWhiteSpace(配置) ? "汉语失语症评测系统" : 配置;
            var 窗口标题桥 = new WindowTitleBridge();
            窗口标题桥?.SetTitle(标题, "");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"启动时更新窗口标题失败: {e.Message}");
        }
#endif
    }
    private void ShowWindowsMessageAndExit(string message)
    {
        try
        {
            // Launch an external small process (PowerShell) to show a MessageBox so the dialog
            // stays independent from this process. Escape single quotes for PowerShell single-quoted string.
            string msgEsc = message.Replace("'", "''");
            string titleEsc = "提示".Replace("'", "''");
            string psCmd = $"Add-Type -AssemblyName System.Windows.Forms; [System.Windows.Forms.MessageBox]::Show('{msgEsc}','{titleEsc}')";
            var psi = new System.Diagnostics.ProcessStartInfo("powershell.exe", $"-NoProfile -WindowStyle Hidden -Command \"{psCmd}\"")
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            try
            {
                // Use shell "start" to detach the process so it won't be tied to this process.
                var shellPsi = new System.Diagnostics.ProcessStartInfo("cmd.exe", $"/C start \"\" powershell -NoProfile -Command \"{psCmd}\"")
                {
                    CreateNoWindow = true,
                    UseShellExecute = true,
                };
                System.Diagnostics.Process.Start(shellPsi);
            }
            catch (Exception exProc)
            {
                Debug.LogWarning($"[keyDog] 无法启动外部对话框进程: {exProc.Message}");
                // Fallback to in-process MessageBox if starting powershell failed
                try { MessageBox(IntPtr.Zero, message, "提示", 0); } catch { }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[keyDog] 显示对话框异常: {ex.Message}");
        }

        //  先尝试用 Unity 正常退出，再强制结束进程以保证关闭
        try
        {
            // First request Unity to quit gracefully
            Application.Quit();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[keyDog] Application.Quit() failed: {ex.Message}");
        }

        // Try graceful quit then force-kill immediately to ensure the process terminates
        try
        {
            var p = System.Diagnostics.Process.GetCurrentProcess();
            if (p != null && !p.HasExited)
            {
                try
                {
                    // give a tiny moment for Unity to start shutdown hooks
                    System.Threading.Thread.Sleep(50);
                }
                catch { }

                try
                {
                    p.Kill();
                }
                catch (Exception exKill)
                {
                    Debug.LogWarning($"[keyDog] 强制结束进程失败: {exKill.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[keyDog] 退出时遇到异常: {ex.Message}");
        }
    }

    // ---- 以下为串口直接验证实现 ----
    private const byte FRAME_HEADER = 0xAA;
    private const byte FRAME_TAIL = 0x7E;
    private const byte ENCRYPT_REQUEST_CODE = 0x01;
    private const byte ENCRYPT_RESPONSE_CODE = 0x02;
    private const byte STATUS_AUTH_SUCCESS = 0x01;
    private static readonly byte[] HANDSHAKE_COMMAND = { 0x57, 0x73, 0x3A, 0x7E }; // "ws:~"
    private static readonly byte[] HANDSHAKE_RESPONSE = { 0x57, 0x73, 0x3A };
    private const int HANDSHAKE_TIMEOUT_MS = 800;
    private const string SAVED_PORT_KEY = "SerialPort_Dongle_LastPortSkip";

    private bool TryAuthenticatePortSync(string portName, bool skipHandshake = false)
    {
        var received = new List<byte>();
        NativeSerial native = null;
        try
        {
            Debug.Log($"[keyDog] 使用原生 Win32 串口打开: {portName}");
            native = new NativeSerial();
            // 先用 cmd mode 设置波特率等参数，避免复杂 DCB 操作
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo("cmd.exe", $"/C mode {portName}: baud=115200 parity=n data=8 stop=1")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                };
                using (var p = System.Diagnostics.Process.Start(psi))
                {
                    p?.WaitForExit(500);
                }
            }
            catch (Exception exMode)
            {
                Debug.LogWarning($"[keyDog] 设置端口参数失败 (mode): {exMode.Message}");
            }

            if (!native.Open(portName))
            {
                Debug.LogWarning($"[keyDog] 原生打开端口失败: {portName}");
                return false;
            }

            Debug.Log($"[keyDog] 原生端口已打开: {portName}");

            // 如果不跳过握手，则先执行握手流程，再发送解锁请求
            if (!skipHandshake)
            {
                try
                {
                    received.Clear();
                    // single handshake attempt (simplified, faster)
                    native.Write(HANDSHAKE_COMMAND, 0, HANDSHAKE_COMMAND.Length);
                    var swh = System.Diagnostics.Stopwatch.StartNew();
                    bool handshakeOk = false;
                    while (swh.ElapsedMilliseconds < HANDSHAKE_TIMEOUT_MS)
                    {
                        var hb = native.ReadAvailable();
                        if (hb != null && hb.Length > 0)
                        {
                            received.AddRange(hb);
                            if (ContainsSequence(received.ToArray(), HANDSHAKE_RESPONSE))
                            {
                                handshakeOk = true;
                                Debug.Log($"[keyDog] 端口 {portName} 握手成功");
                                break;
                            }
                        }
                        Thread.Sleep(20);
                    }
                    if (!handshakeOk)
                    {
                        Debug.LogWarning($"[keyDog] 端口 {portName} 握手超时或失败");
                        return false;
                    }
                }
                catch (Exception exH)
                {
                    Debug.LogWarning($"[keyDog] 端口 {portName} 握手异常: {exH.Message}");
                    return false;
                }

                // 握手成功后清空接收缓冲，开始发送解锁请求
                received.Clear();
            }
            string mac = GetMacAddress();
            uint timestamp = (uint)(DateTime.Now.Ticks / TimeSpan.TicksPerSecond);
            var frame = CreateDongleRequestFrame(mac, timestamp);
            Debug.Log($"[keyDog] 发送请求帧到 {portName}: {BitConverter.ToString(frame)}");
            if (!native.Write(frame, 0, frame.Length))
            {
                Debug.LogWarning($"[keyDog] 写入端口失败: {portName}");
                return false;
            }

            int timeoutMs = 800;
            var sw = System.Diagnostics.Stopwatch.StartNew();

            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                var buf = native.ReadAvailable();
                if (buf != null && buf.Length > 0)
                {
                    Debug.Log($"[keyDog] 端口 {portName} 读取到 {buf.Length} 字节: {BitConverter.ToString(buf)}");
                    received.AddRange(buf);
                    var recvArr = received.ToArray();
                    if (ParseDongleResponse(recvArr, out bool authSuccess))
                    {
                        Debug.Log($"[keyDog] 端口 {portName} 解析到加密狗响应: {BitConverter.ToString(recvArr)}, 认证结果: {authSuccess}");
                        if (authSuccess)
                        {
                            try
                            {
                                PlayerPrefs.SetString(SAVED_PORT_KEY, portName);
                                PlayerPrefs.Save();
                                return true;
                            }
                            catch (Exception exSave)
                            {
                                Debug.LogWarning($"[keyDog] 保存端口失败: {exSave.Message}，将重新扫描并握手");
                                // treat as non-success so caller continues scanning and handshake
                                return false;
                            }
                        }
                        return false;
                    }
                    else
                    {
                        Debug.Log($"[keyDog] 端口 {portName} 收到数据但未解析通过，当前缓冲: {BitConverter.ToString(recvArr)}");
                    }
                }
                Thread.Sleep(50);
            }

            return false;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[keyDog] 端口 {portName} IO 异常: {ex.Message}");
            return false;
        }
        finally
        {
            try { native?.Close(); } catch { }
        }
    }

    private byte[] CreateDongleRequestFrame(string macAddress, uint timestamp)
    {
        var frame = new List<byte>();
        frame.Add(FRAME_HEADER);
        frame.Add(ENCRYPT_REQUEST_CODE);
        frame.AddRange(BitConverter.GetBytes(timestamp));
        frame.AddRange(EncryptMac(macAddress, timestamp));

        // 计算校验和 (从索引1到最后一项之前)
        byte checksum = 0;
        for (int i = 1; i < frame.Count; i++) checksum += frame[i];
        frame.Add(checksum);
        frame.Add(FRAME_TAIL);
        return frame.ToArray();
    }

    private byte[] EncryptMac(string macAddress, uint timestamp)
    {
        string cleanMac = macAddress.Replace("-", "").Replace(":", "");
        byte[] macBytes = new byte[6];
        for (int i = 0; i < 6; i++)
        {
            macBytes[i] = Convert.ToByte(cleanMac.Substring(i * 2, 2), 16);
        }

        byte[] timestampBytes = BitConverter.GetBytes(timestamp);
        byte[] keyBytes = new byte[6];
        for (int i = 0; i < 6; i++)
        {
            keyBytes[i] = timestampBytes[3 - (i % 4)];
        }

        byte[] encryptedMac = new byte[6];
        for (int i = 0; i < 6; i++) encryptedMac[i] = (byte)(macBytes[i] ^ keyBytes[i]);
        return encryptedMac;
    }

    private string GetMacAddress()
    {
        try
        {
            string deviceId = SystemInfo.deviceUniqueIdentifier;
            if (string.IsNullOrEmpty(deviceId)) return "000000000000";
            string mac = "";
            for (int i = 0; i < Math.Min(12, deviceId.Length); i += 2)
            {
                if (i + 1 < deviceId.Length) mac += deviceId.Substring(i, 2);
                else mac += deviceId.Substring(i, 1) + "0";
            }
            if (mac.Length < 12)
            {
                string hashSuffix = deviceId.GetHashCode().ToString("X8");
                mac = (mac + hashSuffix).Substring(0, 12);
            }
            return mac.ToUpper();
        }
        catch { return "000000000000"; }
    }

    private bool ParseDongleResponse(byte[] data, out bool authSuccess)
    {
        authSuccess = false;
        if (data == null || data.Length < 8) return false;
        if (data[0] != FRAME_HEADER) return false;
        if (data[1] != ENCRYPT_RESPONSE_CODE) return false;

        int len = data.Length;
        bool hasTail = data[len - 1] == FRAME_TAIL;
        int checksumIndex = hasTail ? len - 2 : len - 1;

        if (checksumIndex <= 1) return false;

        byte checksum = 0;
        // checksum is computed from index 1 up to checksumIndex-1 (exclusive of checksum byte)
        for (int i = 1; i < checksumIndex; i++) checksum += data[i];
        if (checksum != data[checksumIndex]) return false;

        // 状态码位置保持为索引6（与原实现一致）
        if (len <= 6) return false;
        byte statusCode = data[6];
        authSuccess = (statusCode == STATUS_AUTH_SUCCESS);
        return true;
    }

    private string[] GetComPortsFromRegistry()
    {
        var list = new List<string>();
        try
        {
            using (var key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DEVICEMAP\SERIALCOMM"))
            {
                if (key != null)
                {
                    foreach (var name in key.GetValueNames())
                    {
                        var val = key.GetValue(name) as string;
                        if (!string.IsNullOrEmpty(val))
                        {
                            list.Add(val);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[keyDog] 枚举注册表 COM 端口失败: {ex.Message}");
        }

        return list.ToArray();
    }

    // Simple native serial wrapper using CreateFile/ReadFile/WriteFile
    private class NativeSerial
    {
        private IntPtr handle = IntPtr.Zero;
        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint OPEN_EXISTING = 3;
        private static readonly IntPtr INVALID_HANDLE = new IntPtr(-1);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetCommTimeouts(IntPtr hFile, ref COMMTIMEOUTS lpCommTimeouts);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool BuildCommDCB(string lpDef, out DCB lpDCB);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetCommState(IntPtr hFile, [In] ref DCB lpDCB);

        [StructLayout(LayoutKind.Sequential)]
        private struct DCB
        {
            public uint DCBlength;
            public uint BaudRate;
            public uint Flags;
            public ushort wReserved;
            public ushort XonLim;
            public ushort XoffLim;
            public byte ByteSize;
            public byte Parity;
            public byte StopBits;
            public byte XonChar;
            public byte XoffChar;
            public byte ErrorChar;
            public byte EofChar;
            public byte EvtChar;
            public ushort wReserved1;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct COMMTIMEOUTS
        {
            public uint ReadIntervalTimeout;
            public uint ReadTotalTimeoutMultiplier;
            public uint ReadTotalTimeoutConstant;
            public uint WriteTotalTimeoutMultiplier;
            public uint WriteTotalTimeoutConstant;
        }

        public bool Open(string portName)
        {
            string full = @"\\.\" + portName;
            handle = CreateFile(full, GENERIC_READ | GENERIC_WRITE, 0, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            if (handle == INVALID_HANDLE) return false;

            var timeouts = new COMMTIMEOUTS
            {
                ReadIntervalTimeout = 50,
                ReadTotalTimeoutMultiplier = 0,
                ReadTotalTimeoutConstant = 200,
                WriteTotalTimeoutMultiplier = 0,
                WriteTotalTimeoutConstant = 500,
            };
            SetCommTimeouts(handle, ref timeouts);
            // 确保端口缓冲区清空并给设备一点时间初始化
            Purge(handle);
            try { Thread.Sleep(80); } catch { }
            // 使用 BuildCommDCB + SetCommState 来替代外部 mode 命令，确保在 Player 环境中可用
            try
            {
                string dcbDef = "baud=115200 parity=N data=8 stop=1";
                if (BuildCommDCB(dcbDef, out DCB dcb))
                {
                    dcb.DCBlength = (uint)Marshal.SizeOf(typeof(DCB));
                    if (!SetCommState(handle, ref dcb))
                    {
                        UnityEngine.Debug.LogWarning($"[keyDog.NativeSerial] SetCommState 失败: {Marshal.GetLastWin32Error()}");
                    }
                    else
                    {
                        UnityEngine.Debug.Log($"[keyDog.NativeSerial] SetCommState 成功: {dcbDef}");
                    }
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"[keyDog.NativeSerial] BuildCommDCB 失败: {Marshal.GetLastWin32Error()}");
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning($"[keyDog.NativeSerial] 配置端口 DCB 异常: {ex.Message}");
            }
            return true;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool PurgeComm(IntPtr hFile, uint dwFlags);

        private const uint PURGE_TXCLEAR = 0x0004;
        private const uint PURGE_RXCLEAR = 0x0008;

        private void Purge(IntPtr h)
        {
            try
            {
                PurgeComm(h, PURGE_RXCLEAR | PURGE_TXCLEAR);
            }
            catch { }
        }

        // Purge buffers helper left inside Open; no external toggle required to simplify behavior.

        public bool Write(byte[] buffer, int offset, int count)
        {
            if (handle == INVALID_HANDLE || handle == IntPtr.Zero) return false;
            var toWrite = new byte[count];
            Array.Copy(buffer, offset, toWrite, 0, count);
            Debug.Log("发送数据: " + BitConverter.ToString(toWrite));
            if (!WriteFile(handle, toWrite, (uint)count, out uint written, IntPtr.Zero)) return false;
            return written == count;
        }

        public byte[] ReadAvailable()
        {
            if (handle == INVALID_HANDLE || handle == IntPtr.Zero) return null;
            // try reading up to 1024 bytes (non-blocking due to timeouts)
            var buf = new byte[1024];
            if (!ReadFile(handle, buf, (uint)buf.Length, out uint read, IntPtr.Zero)) return null;
            if (read == 0) return null;
            var outBuf = new byte[read];
            Array.Copy(buf, 0, outBuf, 0, read);
            return outBuf;
        }

        public void Close()
        {
            if (handle != INVALID_HANDLE && handle != IntPtr.Zero)
            {
                try { CloseHandle(handle); } catch { }
                handle = IntPtr.Zero;
            }
        }
    }
    private bool ContainsSequence(byte[] buffer, byte[] seq)
    {
        if (buffer == null || seq == null || buffer.Length < seq.Length) return false;
        for (int i = 0; i <= buffer.Length - seq.Length; i++)
        {
            bool ok = true;
            for (int j = 0; j < seq.Length; j++)
            {
                if (buffer[i + j] != seq[j]) { ok = false; break; }
            }
            if (ok) return true;
        }
        return false;
    }
}


