using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using QFramework;
using UnityEngine;
using LitJson;

/// <summary>
/// 蓝牙数据同步工具接口
/// </summary>
public interface I蓝牙数据同步工具 : IUtility
{
    /// <summary>
    /// 解析原始蓝牙数据（统一入口）
    /// </summary>
    /// <param name="原始数据">底层通知或串口回调字节</param>
    UniTask 处理接收到的蓝牙原始数据(byte[] 原始数据);

    /// <summary>
    /// 发送单条待同步数据
    /// </summary>
    /// <param name="待同步数据">待同步数据</param>
    /// <returns>是否发送成功</returns>
    UniTask<bool> 发送待同步数据(待同步数据 待同步数据);

    /// <summary>
    /// 根据录名标识获取录名
    /// </summary>
    /// <param name="录名标识">录名标识字节</param>
    /// <returns>对应的录名</returns>
    string 获取录名(byte 录名标识);

    /// <summary>
    /// 根据录名获取录名标识
    /// </summary>
    /// <param name="录名">录名</param>
    /// <returns>录名标识</returns>
    byte 获取录名标识(string 录名);

    /// <summary>
    /// 从JSON中获取标识符（编号或账号）
    /// </summary>
    /// <param name="录名">录名</param>
    /// <param name="json字符串">JSON字符串</param>
    /// <returns>标识符</returns>
    string 从Json获取标识符(string 录名, string json字符串);

    /// <summary>
    /// 检查录中是否已存在相同标识符的数据
    /// </summary>
    /// <param name="录">录对象</param>
    /// <param name="录名">录名</param>
    /// <param name="标识符">标识符</param>
    /// <returns>是否已存在</returns>
    bool 录中已存在(I录贞 录, string 录名, string 标识符);

    /// <summary>
    /// 同步数据到录（包含重复检查和告示提示）
    /// </summary>
    /// <param name="录名">录名</param>
    /// <param name="json字符串">JSON字符串</param>
    /// <returns>是否成功</returns>
    UniTask<bool> 同步数据到录(string 录名, string json字符串);

    /// <summary>
    /// 处理接收到的Data区数据（用于电脑端接收）
    /// </summary>
    /// <param name="data区">Data区字节数组</param>
    /// <returns>是否处理成功</returns>
    UniTask<bool> 处理接收到的Data区(byte[] data区);
}

/// <summary>
/// 蓝牙数据同步工具实现
/// </summary>
public class 蓝牙数据同步工具 : I蓝牙数据同步工具
{
    private const byte FRAME_HEADER = 0xAA;
    private const byte BLUETOOTH_FUNC_CODE = 0x03;
    private const byte FRAME_TAIL = 0x7E;
    private const int 最大缓冲区大小 = 8192;

    private const byte 数据帧标记_单包 = 0x10;
    private const byte 数据帧标记_分包开始 = 0x11;
    private const byte 数据帧标记_分包中间 = 0x12;
    private const byte 数据帧标记_分包结束 = 0x13;
    private const int 单帧最大载荷 = 90; // data区最多100字节，扣除5字节元信息后剩余90字节
    private SerialPortCenterUtility _串口工具;

    // 接收缓存与分包缓存（用于统一解析）
    private readonly List<byte> 数据缓冲区 = new List<byte>();
    private readonly Dictionary<byte, 分包缓存> 分包状态缓存 = new Dictionary<byte, 分包缓存>();

    public async UniTask 处理接收到的蓝牙原始数据(byte[] 原始数据)
    {
        if (原始数据 == null || 原始数据.Length == 0)
        {
            Debug.LogWarning("[蓝牙数据同步工具] 接收到空的蓝牙数据");
            return;
        }
        Debug.Log($"处理接收到的蓝牙原始数据: {BitConverter.ToString(原始数据)}");
        数据缓冲区.AddRange(原始数据);

        if (数据缓冲区.Count > 最大缓冲区大小)
        {
            Debug.LogWarning($"[蓝牙数据同步工具] 数据缓冲区过大: {数据缓冲区.Count}，清空缓冲区");
            数据缓冲区.Clear();
            return;
        }

        while (数据缓冲区.Count > 0)
        {
            int 起始位置 = 数据缓冲区.FindIndex(b => b == FRAME_HEADER);
            if (起始位置 < 0)
            {
                Debug.LogWarning("[蓝牙数据同步工具] 未找到数据帧起始符，清空缓冲区");
                数据缓冲区.Clear();
                break;
            }

            if (起始位置 > 0)
            {
                数据缓冲区.RemoveRange(0, 起始位置);
            }

            if (数据缓冲区.Count < 3)
            {
                break;
            }

            byte 数据长度 = 数据缓冲区[2];
            int 完整帧长度 = 3 + 数据长度 + 2;

            if (数据缓冲区.Count < 完整帧长度)
            {
                Debug.Log($"[蓝牙数据同步工具] 数据不完整，当前: {数据缓冲区.Count}，需要: {完整帧长度}，等待更多数据");
                break;
            }

            byte[] 完整数据帧 = 数据缓冲区.Take(完整帧长度).ToArray();

            if (验证数据帧(完整数据帧))
            {
                byte 功能码 = 完整数据帧[1];
                if (功能码 != BLUETOOTH_FUNC_CODE)
                {
                    Debug.LogWarning($"[蓝牙数据同步工具] 未知功能码: 0x{功能码:X2}");
                    数据缓冲区.RemoveAt(0);
                    continue;
                }

                byte 数据区长度 = 完整数据帧[2];
                var data区 = new byte[数据区长度];
                Array.Copy(完整数据帧, 3, data区, 0, 数据区长度);

                await 处理接收到的Data区(data区);
                数据缓冲区.RemoveRange(0, 完整帧长度);
            }
            else
            {
                Debug.LogWarning("[蓝牙数据同步工具] 数据帧验证失败，移除起始符继续查找");
                数据缓冲区.RemoveAt(0);
            }
        }
    }

    private bool 验证数据帧(byte[] 数据)
    {
        if (数据 == null || 数据.Length < 6)
        {
            Debug.LogError("[蓝牙数据同步工具] 数据帧长度不足");
            return false;
        }

        if (数据[0] != FRAME_HEADER)
        {
            Debug.LogError($"[蓝牙数据同步工具] 起始符错误，期望0xAA，实际0x{数据[0]:X2}");
            return false;
        }

        if (数据[数据.Length - 1] != FRAME_TAIL)
        {
            Debug.LogError($"[蓝牙数据同步工具] 结束符错误，期望0x7E，实际0x{数据[数据.Length - 1]:X2}");
            return false;
        }

        byte 声明长度 = 数据[2];
        int 实际数据长度 = 数据.Length - 5;
        if (声明长度 != 实际数据长度)
        {
            Debug.LogError($"[蓝牙数据同步工具] 长度字段不匹配，声明{声明长度}，实际{实际数据长度}");
            return false;
        }

        byte 计算出的校验和 = 计算校验和(数据, 1, 数据.Length - 2);
        byte 接收校验和 = 数据[数据.Length - 2];
        if (计算出的校验和 != 接收校验和)
        {
            Debug.LogError($"[蓝牙数据同步工具] 校验和不匹配，计算0x{计算出的校验和:X2}，接收0x{接收校验和:X2}");
            return false;
        }

        return true;
    }

    private byte 计算校验和(byte[] 数据, int 起始索引, int 结束索引)
    {
        byte sum = 0;
        for (int i = 起始索引; i < 结束索引; i++)
        {
            sum += 数据[i];
        }
        return sum;
    }


    private class 分包缓存
    {
        public int 总包数;
        public byte[][] 片段集合;
        public int 已接收数量;
    }

    public async UniTask<bool> 发送待同步数据(待同步数据 待同步数据)
    {
        try
        {
            // 获取架构
            var architecture = HotFixTemplateArchitecture.Interface;
            if (architecture == null)
            {
                Debug.LogError("[蓝牙数据同步工具] 无法获取架构");
                return false;
            }

            // 序列化数据
            string json = 序列化待同步数据(待同步数据, architecture);
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError($"[蓝牙数据同步工具] 无法序列化数据，数据录: {待同步数据.原数据录名称}，索引: {待同步数据.待同步索引}");
                return false;
            }

            // 获取录名标识
            byte 录名标识 = 获取录名标识(待同步数据.原数据录名称);
            if (录名标识 == 0xFF)
            {
                Debug.LogError($"[蓝牙数据同步工具] 未知的数据录名称: {待同步数据.原数据录名称}");
                return false;
            }

            // 转换为字节数组
            byte[] 数据字节 = Encoding.UTF8.GetBytes(json);
            bool 需要分包 = 数据字节.Length > 单帧最大载荷;

            if (!需要分包)
            {
                var 数据帧 = 构建数据分包(数据帧标记_单包, 录名标识, 1, 1, 数据字节);
                return 发送数据帧(数据帧);
            }

            return await 分包发送数据(数据字节, 录名标识);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[蓝牙数据同步工具] 发送数据异常: {ex.Message}\n{ex.StackTrace}");
            return false;
        }
    }

    /// <summary>
    /// 序列化待同步数据为JSON
    /// </summary>
    private string 序列化待同步数据(待同步数据 待同步数据, IArchitecture architecture)
    {
        switch (待同步数据.原数据录名称)
        {
            case nameof(档案录):
                var 档案录模型 = architecture.GetModel<档案录>();
                var 档案列表 = 档案录模型?.GetDataList();
                if (档案列表 != null && 待同步数据.待同步索引 >= 0 && 待同步数据.待同步索引 < 档案列表.Count)
                {
                    var 档案 = 档案列表[待同步数据.待同步索引];
                    return 构建纳记于信格式(档案);
                }
                break;

            case nameof(评人录):
                var 评人录模型 = architecture.GetModel<评人录>();
                var 评人列表 = 评人录模型?.GetDataList();
                if (评人列表 != null && 待同步数据.待同步索引 >= 0 && 待同步数据.待同步索引 < 评人列表.Count)
                {
                    var 评人 = 评人列表[待同步数据.待同步索引];
                    return 构建纳记于信格式(评人);
                }
                break;

            case nameof(管员录):
                var 管员录模型 = architecture.GetModel<管员录>();
                var 管员列表 = 管员录模型?.GetDataList();
                if (管员列表 != null && 待同步数据.待同步索引 >= 0 && 待同步数据.待同步索引 < 管员列表.Count)
                {
                    var 管员 = 管员列表[待同步数据.待同步索引];
                    return 构建纳记于信格式(管员);
                }
                break;

            case nameof(用户录):
                var 用户录模型 = architecture.GetModel<用户录>();
                var 用户列表 = 用户录模型?.GetDataList();
                if (用户列表 != null && 待同步数据.待同步索引 >= 0 && 待同步数据.待同步索引 < 用户列表.Count)
                {
                    var 用户 = 用户列表[待同步数据.待同步索引];
                    return 构建纳记于信格式(用户);
                }
                break;

            default:
                Debug.LogError($"[蓝牙数据同步工具] 未知的数据录名称: {待同步数据.原数据录名称}");
                return string.Empty;
        }

        return string.Empty;
    }

    /// <summary>
    /// 构建纳记于信格式的JSON
    /// </summary>
    private string 构建纳记于信格式<T>(T 数据) where T : class
    {
        if (数据 == null)
        {
            return string.Empty;
        }

        return JsonMapper.ToJson(数据);
    }

    /// <summary>
    /// 分包发送数据
    /// </summary>
    private async UniTask<bool> 分包发送数据(byte[] 完整数据, byte 录名标识)
    {
        try
        {
            int 总包数 = (完整数据.Length + 单帧最大载荷 - 1) / 单帧最大载荷;

            for (int 包序号 = 0; 包序号 < 总包数; 包序号++)
            {
                int 起始位置 = 包序号 * 单帧最大载荷;
                int 当前包数据大小 = Math.Min(单帧最大载荷, 完整数据.Length - 起始位置);
                var 片段 = new byte[当前包数据大小];
                Array.Copy(完整数据, 起始位置, 片段, 0, 当前包数据大小);

                byte 帧标记;
                if (总包数 == 1)
                {
                    帧标记 = 数据帧标记_单包;
                }
                else if (包序号 == 0)
                {
                    帧标记 = 数据帧标记_分包开始;
                }
                else if (包序号 == 总包数 - 1)
                {
                    帧标记 = 数据帧标记_分包结束;
                }
                else
                {
                    帧标记 = 数据帧标记_分包中间;
                }

                var 数据帧 = 构建数据分包(帧标记, 录名标识, (byte)总包数, (byte)(包序号 + 1), 片段);
                if (!发送数据帧(数据帧))
                {
                    Debug.LogError($"[蓝牙数据同步工具] 分包发送失败，包序号: {包序号 + 1}/{总包数}");
                    return false;
                }

                if (包序号 < 总包数 - 1)
                {
                    await UniTask.Delay(100);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[蓝牙数据同步工具] 分包发送异常: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 构建数据分包
    /// </summary>
    private byte[] 构建数据分包(byte 帧标记, byte 录名标识, byte 总包数, byte 当前序号, byte[] 载荷数据)
    {
        if (载荷数据.Length > 单帧最大载荷)
        {
            throw new ArgumentException($"载荷长度超限: {载荷数据.Length}");
        }

        var 数据 = new byte[5 + 载荷数据.Length];
        数据[0] = 帧标记;
        数据[1] = 录名标识;
        数据[2] = 总包数;
        数据[3] = 当前序号;
        数据[4] = (byte)载荷数据.Length;
        Array.Copy(载荷数据, 0, 数据, 5, 载荷数据.Length);
        return 数据;
    }

    /// <summary>
    /// 发送数据帧
    /// </summary>
    private bool 发送数据帧(byte[] 数据帧)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (_串口工具 == null)
        {
            _串口工具 = HotFixTemplateArchitecture.Interface?.GetUtility<SerialPortCenterUtility>();
            if (_串口工具 == null)
            {
                Debug.LogError("[蓝牙数据同步工具] 无法获取蓝牙工具");
                return false;
            }
        }

        if (!_串口工具.SendBluetoothData(数据帧))
        {
            Debug.LogError("[蓝牙数据同步工具] 蓝牙发送失败");
            return false;
        }
        return true;
#elif UNITY_ANDROID || UNITY_IOS
        var bleAddress = PlayerPrefs.GetString("BLEAddress");
        var bleServiceUUID = PlayerPrefs.GetString("BLEServiceUUID");
        var bleCharacteristicUUID = PlayerPrefs.GetString("BLECharacteristicUUID");

        if (string.IsNullOrEmpty(bleAddress) || string.IsNullOrEmpty(bleServiceUUID) || string.IsNullOrEmpty(bleCharacteristicUUID))
        {
            Debug.LogError("[蓝牙数据同步工具] 未配置蓝牙地址或特征值");
            return false;
        }

        try
        {
            var 完整帧 = 构建完整蓝牙帧(数据帧);
            BleManager.SendData(bleAddress, bleServiceUUID, bleCharacteristicUUID, 完整帧);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[蓝牙数据同步工具] BLE发送失败: {ex.Message}");
            return false;
        }
#else
        Debug.LogWarning("[蓝牙数据同步工具] 未识别的平台，发送逻辑未实现");
        return false;
#endif
    }

    /// <summary>
    /// 根据录名标识获取录名
    /// </summary>
    public string 获取录名(byte 录名标识)
    {
        switch (录名标识)
        {
            case 0x00:
                return "评人录";
            case 0x01:
                return "用户录";
            case 0x02:
                return "档案录";
            default:
                return null;
        }
    }

    /// <summary>
    /// 根据录名获取录名标识
    /// </summary>
    public byte 获取录名标识(string 录名)
    {
        if (录名 == nameof(评人录) || 录名 == "评人录" || 录名 == nameof(管员录) || 录名 == "管员录")
        {
            return 0x00;
        }
        if (录名 == nameof(用户录) || 录名 == "用户录")
        {
            return 0x01;
        }
        if (录名 == nameof(档案录) || 录名 == "档案录")
        {
            return 0x02;
        }
        return 0xFF; // 未知标识
    }

    /// <summary>
    /// 从JSON中获取标识符（编号或账号）
    /// </summary>
    public string 从Json获取标识符(string 录名, string json字符串)
    {
        try
        {
            JsonData jsonData = JsonMapper.ToObject(json字符串);
            if (jsonData == null) return null;

            if (录名 == nameof(档案录) || 录名 == "档案录")
            {
                if (jsonData.Keys.Contains("编号"))
                {
                    return jsonData["编号"]?.ToString();
                }
            }
            else if (录名 == nameof(用户录) || 录名 == "用户录")
            {
                if (jsonData.Keys.Contains("编号"))
                {
                    return jsonData["编号"]?.ToString();
                }
            }
            else if (录名 == nameof(评人录) || 录名 == "评人录" || 录名 == nameof(管员录) || 录名 == "管员录")
            {
                if (jsonData.Keys.Contains("账号"))
                {
                    return jsonData["账号"]?.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[蓝牙数据同步工具] 解析JSON获取标识符失败: {ex.Message}");
        }

        return null;
    }

    /// <summary>
    /// 检查录中是否已存在相同标识符的数据
    /// </summary>
    public bool 录中已存在(I录贞 录, string 录名, string 标识符)
    {
        if (录 == null || string.IsNullOrEmpty(标识符))
        {
            return false;
        }

        var 所有项 = 录.GetAllItems();
        if (所有项 == null) return false;

        if (录名 == nameof(档案录) || 录名 == "档案录")
        {
            foreach (var 项 in 所有项)
            {
                if (项 is 档案 档案项 && 档案项.编号 == 标识符)
                {
                    return true;
                }
            }
        }
        else if (录名 == nameof(用户录) || 录名 == "用户录")
        {
            foreach (var 项 in 所有项)
            {
                if (项 is 用户 用户项 && 用户项.编号 == 标识符)
                {
                    return true;
                }
            }
        }
        else if (录名 == nameof(评人录) || 录名 == "评人录" || 录名 == nameof(管员录) || 录名 == "管员录")
        {
            foreach (var 项 in 所有项)
            {
                if (项 is 评人 评人项 && 评人项.账号 == 标识符)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 同步数据到录（包含重复检查和告示提示）
    /// </summary>
    public async UniTask<bool> 同步数据到录(string 录名, string json字符串)
    {
        var architecture = HotFixTemplateArchitecture.Interface;
        if (architecture == null)
        {
            Debug.LogError("[蓝牙数据同步工具] 无法获取架构");
            return false;
        }

        var 录 = architecture.GetUtility<按名索录之器>()?.寻录<I录贞>(录名);
        if (录 == null)
        {
            Debug.LogError($"[蓝牙数据同步工具] 未找到录: {录名}");
            return false;
        }

        // 解析JSON获取编号/账号
        string 标识符 = 从Json获取标识符(录名, json字符串);
        if (string.IsNullOrEmpty(标识符))
        {
            Debug.LogWarning($"[蓝牙数据同步工具] 无法从JSON中获取标识符");
            // 即使无法获取标识符，也尝试添加
        }
        else
        {
            // 检查录中是否已有相同标识符的数据
            if (录中已存在(录, 录名, 标识符))
            {
                await 显示告示("提示", $"数据已拥有\n录名: {录名}\n编号: {标识符}");
                Debug.Log($"[蓝牙数据同步工具] 数据已存在 - 录名: {录名}, 编号: {标识符}");
                return false;
            }
        }

        // 执行添加
        bool 成功 = 录.纳记于信(json字符串);
        if (成功)
        {
            string 提示内容 = string.IsNullOrEmpty(标识符)
                ? $"加入成功\n录名: {录名}"
                : $"加入成功\n录名: {录名}\n编号: {标识符}";
            await 显示告示("提示", 提示内容);
            Debug.Log($"[蓝牙数据同步工具] 数据同步成功 - 录名: {录名}, 编号: {标识符}");
        }
        else
        {
            Debug.LogError($"[蓝牙数据同步工具] 数据同步失败 - 录名: {录名}");
        }

        return 成功;
    }

    /// <summary>
    /// 显示告示
    /// </summary>
    private async UniTask 显示告示(string 标题, string 内容)
    {
        try
        {
            var architecture = HotFixTemplateArchitecture.Interface;
            if (architecture == null)
            {
                Debug.LogError("[蓝牙数据同步工具] 无法获取架构");
                return;
            }

            var utility = architecture.GetUtility<YooAssetPfbPoolUtility>();
            if (utility == null)
            {
                Debug.LogError("[蓝牙数据同步工具] 无法获取YooAssetPfbPoolUtility");
                return;
            }

            var prefab = await utility.LoadPfb("告示");
            if (prefab == null)
            {
                Debug.LogError("[蓝牙数据同步工具] 加载告示预制体失败");
                return;
            }

            var 界面管理器 = UnityEngine.Object.FindObjectOfType<界面管理器_韩>();
            if (界面管理器 == null)
            {
                Debug.LogError("[蓝牙数据同步工具] 未找到界面管理器");
                return;
            }

            var tips = UnityEngine.Object.Instantiate(prefab, 界面管理器.transform);
            var 告示组件 = tips.GetComponent<告示_茉>();
            if (告示组件 != null)
            {
                await 告示组件.等待确认显示(标题, 内容);
            }
            UnityEngine.Object.Destroy(tips);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[蓝牙数据同步工具] 显示告示异常: {ex.Message}");
        }
    }

    /// <summary>
    /// 构建完整的蓝牙帧（0xAA | 0x03 | 长度 | Data区 | 校验和 | 0x7E）
    /// </summary>
    private byte[] 构建完整蓝牙帧(byte[] data区)
    {
        const byte FRAME_HEADER = 0xAA;
        const byte BLUETOOTH_FUNC_CODE = 0x03;
        const byte FRAME_TAIL = 0x7E;

        List<byte> frame = new List<byte>();

        frame.Add(FRAME_HEADER);
        frame.Add(BLUETOOTH_FUNC_CODE);
        frame.Add((byte)data区.Length);
        frame.AddRange(data区);

        // 计算校验和（从功能码到数据区结束）
        byte checksum = 0;
        for (int i = 1; i < frame.Count; i++)
        {
            checksum += frame[i];
        }
        frame.Add(checksum);
        frame.Add(FRAME_TAIL);

        return frame.ToArray();
    }

    /// <summary>
    /// 处理接收到的Data区数据（用于电脑端接收）
    /// </summary>
    public async UniTask<bool> 处理接收到的Data区(byte[] data区)
    {
        if (data区 == null || data区.Length == 0)
        {
            Debug.LogError("[蓝牙数据同步工具] Data区为空");
            return false;
        }
        Debug.Log($"处理接收到的Data区: {BitConverter.ToString(data区)}");
        byte 帧标记 = data区[0];

        // 处理连接命令
        if (帧标记 == 0x01)
        {
            Debug.Log("[蓝牙数据同步工具] 收到蓝牙连接命令");
            return true;
        }

        // 检查是否为合法的数据帧标记
        if (帧标记 != 数据帧标记_单包
            && 帧标记 != 数据帧标记_分包开始
            && 帧标记 != 数据帧标记_分包中间
            && 帧标记 != 数据帧标记_分包结束)
        {
            Debug.LogWarning($"[蓝牙数据同步工具] 未知的数据帧标记: 0x{帧标记:X2}");
            return false;
        }

        if (data区.Length < 5)
        {
            Debug.LogError("[蓝牙数据同步工具] 数据帧长度不足，缺少头部信息");
            return false;
        }

        byte 录名标识 = data区[1];
        byte 总包数 = data区[2];
        byte 当前序号 = data区[3];
        byte 载荷长度 = data区[4];

        if (data区.Length < 5 + 载荷长度)
        {
            Debug.LogError($"[蓝牙数据同步工具] 数据帧载荷长度不匹配，声明:{载荷长度} 实际:{data区.Length - 5}");
            return false;
        }

        var 载荷 = new byte[载荷长度];
        Array.Copy(data区, 5, 载荷, 0, 载荷长度);

        if (总包数 <= 1 || 帧标记 == 数据帧标记_单包)
        {
            // 单包数据，直接处理
            string 录名 = 获取录名(录名标识);
            if (string.IsNullOrEmpty(录名))
            {
                Debug.LogWarning($"[蓝牙数据同步工具] 未知录名标识: 0x{录名标识:X2}");
                return false;
            }

            string json字符串 = Encoding.UTF8.GetString(载荷);
            Debug.Log($"[蓝牙数据同步工具] 接收到完整数据 - 录名:{录名} 长度:{json字符串.Length}");
            return await 同步数据到录(录名, json字符串);
        }

        // 分包数据，需要缓存和重组
        return await 处理分包数据(帧标记, 录名标识, 总包数, 当前序号, 载荷);
    }

    /// <summary>
    /// 处理分包数据
    /// </summary>
    private async UniTask<bool> 处理分包数据(byte 帧标记, byte 录名标识, byte 总包数, byte 当前序号, byte[] 载荷)
    {
        if (当前序号 == 0 || 当前序号 > 总包数)
        {
            Debug.LogError($"[蓝牙数据同步工具] 分包序号非法: {当前序号}/{总包数}");
            return false;
        }

        if (!分包状态缓存.TryGetValue(录名标识, out var 状态))
        {
            状态 = new 分包缓存
            {
                总包数 = 总包数,
                片段集合 = new byte[总包数][],
                已接收数量 = 0
            };
            分包状态缓存[录名标识] = 状态;
        }

        if (状态.总包数 != 总包数)
        {
            Debug.LogWarning($"[蓝牙数据同步工具] 分包总数不一致，旧:{状态.总包数} 新:{总包数}，重置缓存");
            分包状态缓存.Remove(录名标识);
            状态 = new 分包缓存
            {
                总包数 = 总包数,
                片段集合 = new byte[总包数][],
                已接收数量 = 0
            };
            分包状态缓存[录名标识] = 状态;
        }

        int 索引 = 当前序号 - 1;
        状态.片段集合[索引] = 载荷;
        状态.已接收数量 = 状态.片段集合.Count(p => p != null);

        Debug.Log($"[蓝牙数据同步工具] 接收分包 {当前序号}/{总包数} 录名标识:0x{录名标识:X2}");

        if (帧标记 == 数据帧标记_分包结束 || 状态.已接收数量 == 状态.总包数)
        {
            if (状态.片段集合.Any(p => p == null))
            {
                Debug.LogWarning($"[蓝牙数据同步工具] 分包未完整，仍缺少 {状态.片段集合.Count(p => p == null)} 个片段");
                return false;
            }

            int 总长度 = 状态.片段集合.Sum(p => p.Length);
            var 合并 = new byte[总长度];
            int 偏移 = 0;
            foreach (var 片段 in 状态.片段集合)
            {
                Array.Copy(片段, 0, 合并, 偏移, 片段.Length);
                偏移 += 片段.Length;
            }

            分包状态缓存.Remove(录名标识);

            string 录名 = 获取录名(录名标识);
            if (string.IsNullOrEmpty(录名))
            {
                Debug.LogWarning($"[蓝牙数据同步工具] 未知录名标识: 0x{录名标识:X2}");
                return false;
            }

            string json字符串 = Encoding.UTF8.GetString(合并);
            Debug.Log($"[蓝牙数据同步工具] 接收到完整分包数据 - 录名:{录名} 长度:{json字符串.Length}");
            return await 同步数据到录(录名, json字符串);
        }

        return true; // 分包未完成，等待更多数据
    }

}

