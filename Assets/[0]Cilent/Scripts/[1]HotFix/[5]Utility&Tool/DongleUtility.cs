using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using QFramework;
using UnityEngine;
using SerialPortUtility;
using Cysharp.Threading.Tasks;
/// <summary>
/// 加密狗工具接口
/// </summary>
public interface IDongleUtility : ISerialPortBaseUtility
{
    string DeviceId { get; }
    UniTask<bool> AuthenticateAsync();
    UniTask<byte[]> EncryptDataAsync(byte[] data);
    UniTask<byte[]> DecryptDataAsync(byte[] data);
}

/// <summary>
/// 加密狗数据帧结构
/// </summary>
public struct DongleFrame
{
    public const byte FRAME_HEADER = 0xAA;
    public const byte FRAME_TAIL = 0x55;

    public byte Header;
    public byte Length;
    public DongleCommand Command;
    public byte[] Data;
    public byte Checksum;
    public byte Tail;

    public byte[] ToBytes()
    {
        var frame = new List<byte>
            {
                Header,
                Length,
                (byte)Command
            };

        if (Data != null && Data.Length > 0)
            frame.AddRange(Data);

        frame.Add(Checksum);
        frame.Add(Tail);

        return frame.ToArray();
    }

    public static DongleFrame FromBytes(byte[] bytes)
    {
        if (bytes.Length < 5) throw new ArgumentException("Invalid frame length");

        return new DongleFrame
        {
            Header = bytes[0],
            Length = bytes[1],
            Command = (DongleCommand)bytes[2],
            Data = bytes.Length > 5 ? bytes[3..^2] : new byte[0],
            Checksum = bytes[^2],
            Tail = bytes[^1]
        };
    }

    public static byte CalculateChecksum(byte[] data)
    {
        byte sum = 0;
        for (int i = 0; i < data.Length - 2; i++)
            sum += data[i];
        return sum;
    }
}

/// <summary>
/// 加密狗命令枚举
/// </summary>
public enum DongleCommand : byte
{
    GetDeviceId = 0x01,
    Challenge = 0x02,
    Encrypt = 0x03,
    Decrypt = 0x04,
    Status = 0x05,
    Heartbeat = 0x06
}

/// <summary>
/// 加密狗工具实现类
/// </summary>
public class DongleUtility : SerialPortBaseUtility, IDongleUtility
{
    private string deviceId = "";
    private uint sessionKey = 0;
    private UniTaskCompletionSource<byte[]> responseWaiter;

    // TEA加密密钥（实际使用时应该更复杂）
    private readonly uint[] teaKey = { 0x12345678, 0x9ABCDEF0, 0x13579BDF, 0x2468ACE0 };

    public string DeviceId => deviceId;

    /// <summary>
    /// 设备名称
    /// </summary>
    protected override string DeviceName => "加密狗";

    /// <summary>
    /// 设备识别逻辑：发送设备ID查询命令
    /// </summary>
    protected override async UniTask<bool> RecognizeDevice(string portName, int portSkip)
    {
        try
        {
            // 发送设备ID查询命令
            var response = await SendCommandAsync(DongleCommand.GetDeviceId, null, ConnectionTimeoutMs);
            if (response != null && response.Length >= 8)
            {
                deviceId = BitConverter.ToString(response).Replace("-", "");
                Debug.Log($"[{DeviceName}] 识别到设备ID: {deviceId}");
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{DeviceName}] 设备识别失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 处理接收到的加密狗数据
    /// </summary>
    protected override void ProcessReceivedData(byte[] data)
    {
        try
        {
            // 验证数据帧格式
            if (data.Length < 5 || data[0] != DongleFrame.FRAME_HEADER || data[^1] != DongleFrame.FRAME_TAIL)
            {
                Debug.LogWarning($"[{DeviceName}] 收到无效的数据帧");
                return;
            }

            // 验证校验和
            var expectedChecksum = DongleFrame.CalculateChecksum(data);
            if (data[^2] != expectedChecksum)
            {
                Debug.LogWarning($"[{DeviceName}] 数据帧校验和错误");
                return;
            }

            // 提取数据部分
            // data[3..^2] 是C#的范围运算符语法，表示：
            // 从索引3开始，到倒数第2个元素结束（不包含倒数第2个元素）
            // 3.. 表示从索引3开始到数组末尾
            // ^2 表示倒数第2个元素（^1是最后一个元素）
            // 所以 data[3..^2] 提取的是从第4个字节到倒数第3个字节的数据
            var responseData = data.Length > 5 ? data[3..^2] : new byte[0];

            // 完成响应等待
            lock (lockObject)
            {
                responseWaiter?.TrySetResult(responseData);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{DeviceName}] 处理接收数据时出错: {ex.Message}");
            lock (lockObject)
            {
                responseWaiter?.TrySetResult(null);
            }
        }
    }

    /// <summary>
    /// 检查加密狗健康状态：发送心跳命令
    /// </summary>
    protected override async UniTask<bool> CheckDeviceHealth()
    {
        try
        {
            var response = await SendCommandAsync(DongleCommand.Heartbeat, null, 2000);
            return response != null;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 执行挑战应答认证
    /// </summary>
    public async UniTask<bool> AuthenticateAsync()
    {
        if (!IsConnected) return false;

        try
        {
            // 生成随机挑战数据
            var challenge = new byte[8];
            System.Security.Cryptography.RandomNumberGenerator.Fill(challenge);

            // 发送挑战
            var response = await SendCommandAsync(DongleCommand.Challenge, challenge, 3000);

            if (response != null && response.Length >= 8)
            {
                // 验证响应（简化版本，实际应该更复杂）
                var expectedResponse = TEAEncrypt(challenge);

                if (response.Take(8).SequenceEqual(expectedResponse.Take(8)))
                {
                    sessionKey = BitConverter.ToUInt32(response, 8);
                    Debug.Log($"[{DeviceName}] 认证成功");
                    return true;
                }
            }

            Debug.LogError($"[{DeviceName}] 认证失败");
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{DeviceName}] 认证过程出错: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 加密数据
    /// </summary>
    public async UniTask<byte[]> EncryptDataAsync(byte[] data)
    {
        if (!IsConnected) return null;

        try
        {
            return await SendCommandAsync(DongleCommand.Encrypt, data, 5000);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{DeviceName}] 数据加密失败: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 解密数据
    /// </summary>
    public async UniTask<byte[]> DecryptDataAsync(byte[] data)
    {
        if (!IsConnected) return null;

        try
        {
            return await SendCommandAsync(DongleCommand.Decrypt, data, 5000);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{DeviceName}] 数据解密失败: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 发送命令并等待响应
    /// </summary>
    private async UniTask<byte[]> SendCommandAsync(DongleCommand command, byte[] data, int timeoutMs)
    {
        if (serialPort == null || !serialPort.IsOpened()) return null;

        lock (lockObject)
        {
            // 构建数据帧
            var frame = new DongleFrame
            {
                Header = DongleFrame.FRAME_HEADER,
                Length = (byte)(3 + (data?.Length ?? 0)),
                Command = command,
                Data = data ?? new byte[0],
                Tail = DongleFrame.FRAME_TAIL
            };

            var frameBytes = frame.ToBytes();
            frame.Checksum = DongleFrame.CalculateChecksum(frameBytes);
            frameBytes[^2] = frame.Checksum;

            // 设置响应等待器
            responseWaiter = new UniTaskCompletionSource<byte[]>();

            // 发送数据
            SendData(frameBytes);
        }

        try
        {
            // 等待响应或超时
            var result = await responseWaiter.Task.Timeout(TimeSpan.FromMilliseconds(timeoutMs));
            return result;
        }
        catch (TimeoutException)
        {
            Debug.LogWarning($"[{DeviceName}] 命令 {command} 响应超时");
            return null;
        }
    }

    /// <summary>
    /// TEA加密算法（简化版本）
    /// </summary>
    private byte[] TEAEncrypt(byte[] data)
    {
        // 简化的TEA加密实现
        // 实际项目中应该使用完整的TEA算法
        var result = new byte[8];
        for (int i = 0; i < Math.Min(data.Length, 8); i++)
        {
            result[i] = (byte)(data[i] ^ (teaKey[i % 4] & 0xFF));
        }
        return result;
    }

    /// <summary>
    /// 重写断开连接方法，清理加密狗特有资源
    /// </summary>
    public override void Disconnect()
    {
        deviceId = "";
        sessionKey = 0;
        base.Disconnect();
    }
}
