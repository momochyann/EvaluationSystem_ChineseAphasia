using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Nova;
using NovaSamples.UIControls;
using QFramework;
using System.Linq;
using LitJson;
using System.IO;

public class 备份管理界面_茉 : 界面基础_韩
{
    [Header("UI组件")]
    [SerializeField] private Button 确认备份按钮;
    [SerializeField] private Button 确认还原按钮;
    [SerializeField] private Button 蓝牙同步按钮;
    [SerializeField] private GameObject 选择数据备份;
    [SerializeField] private GameObject 选择数据同步;
    [SerializeField] private GameObject 数据显示界面;
    [SerializeField] private 显示条目_韩 显示条目面板;
     [SerializeField] private 安卓功能栏 同步安卓功能栏;
    [Header("蓝牙同步 - 勾选项")]
    [SerializeField] private List<蓝牙同步勾选项> 蓝牙同步选项 = new List<蓝牙同步勾选项>();
    [SerializeField] private Button 数据备份下一步按钮;

    [Header("蓝牙同步 - 日期筛选")]
    [SerializeField] private Button 开始日期按钮;
    [SerializeField] private Button 结束日期按钮;
    [SerializeField] private TextBlock 开始日期文本;
    [SerializeField] private TextBlock 结束日期文本;
    [SerializeField] private Button 日期下一步按钮;
    [SerializeField] private Button 日期上一步按钮;
    [SerializeField] private string 日期占位文案 = "请选择日期";

    [Header("蓝牙同步 - 数据显示")]
    [SerializeField] private TextBlock 数据显示文本;
    [SerializeField] private Button 返回日期选择按钮;
    [SerializeField] private Button 批量同步按钮;
    private 评人录 评人录;
    private 管员录 管员录;
    private 档案录 档案录;
    private 用户录 用户录;
    private 待同步数据录 待同步数据录;

    private readonly List<蓝牙同步勾选项> 当前已选选项 = new List<蓝牙同步勾选项>();
    private readonly List<string> 同步条目缓存 = new List<string>();
    private readonly Dictionary<蓝牙同步勾选项, int> 选项条数缓存 = new Dictionary<蓝牙同步勾选项, int>();
    private DateTime? 已选开始日期;
    private DateTime? 已选结束日期;
    private string 已选开始日期字符串 = string.Empty;
    private string 已选结束日期字符串 = string.Empty;
    private bool 择日器显示中;
    private GameObject 当前择日器;
    private bool 正在处理日期确认;

    private const string PC择日器资源名 = "择日器_PC";
    private const string 默认择日器资源名 = "择日器";

    void Start()
    {
        评人录 = this.GetModel<评人录>();
        管员录 = this.GetModel<管员录>();
        档案录 = this.GetModel<档案录>();
        用户录 = this.GetModel<用户录>();
        待同步数据录 = this.GetModel<待同步数据录>();
        if (待同步数据录 == null)
        {
            Debug.LogError("[备份管理界面_茉] 未能获取 待同步数据录 模型");
        }
        设置按钮监听();
        初始化蓝牙同步流程();
    }

    void 设置按钮监听()
    {
        确认备份按钮?.OnClicked.AddListener(执行备份);
        确认还原按钮?.OnClicked.AddListener(执行还原);
        蓝牙同步按钮?.OnClicked.AddListener(执行蓝牙同步);
    }

    private void 执行蓝牙同步()
    {
        Debug.Log("执行蓝牙同步");
        显示备份选择界面();
    }

    private void ImportTemplate()
    {
        var jsonDataUtility = this.GetUtility<JsonDataUtility>();
        评人录.ImportFromJson(false);
    }

    private void ExportTemplate()
    {
        评人录.ExportToJson();
    }

    async void 执行备份()
    {
        var tip = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var tips = Instantiate(tip, this.transform);
        var 确认 = await tips.GetComponent<告示_茉>().等待确认显示("提示", "确认备份数据？");
        Destroy(tips);
        
        if (!确认)
        {
            return;
        }

        try
        {
            // 显示备份进度
            var 进度告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
            var 进度告示实例 = Instantiate(进度告示, this.transform);
            var 告示组件 = 进度告示实例.GetComponent<告示_茉>();
            告示组件.显示进度("备份中", "正在备份数据...");

            // 获取三个录的数据
            var 评人列表 = 评人录?.GetDataList() ?? new List<评人>();
            var 用户列表 = 用户录?.GetDataList() ?? new List<用户>();
            var 档案列表 = 档案录?.GetDataList() ?? new List<档案>();

            int 总条数 = 评人列表.Count + 用户列表.Count + 档案列表.Count;
            告示组件.更新内容($"正在备份...\n共 {总条数} 条");

            // 构建备份数据结构
            var 备份数据 = new Dictionary<string, object>
            {
                ["评人录"] = 评人列表,
                ["用户录"] = 用户列表,
                ["档案录"] = 档案列表,
                ["备份时间"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                ["版本"] = Application.version
            };

            // 序列化为JSON
            string json数据 = JsonMapper.ToJson(备份数据);

            告示组件.更新内容("正在保存备份文件...");

            // 获取备份目录
            string 备份目录 = 获取备份目录();
            if (!Directory.Exists(备份目录))
            {
                Directory.CreateDirectory(备份目录);
            }

            // 生成备份文件名（包含时间戳）
            string 文件名 = $"备份_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            string 文件路径 = Path.Combine(备份目录, 文件名);

            // 保存文件
            await File.WriteAllTextAsync(文件路径, json数据, Encoding.UTF8);
        
            // 显示成功提示
//            告示组件.显示进度("备份成功", $"备份完成！\n文件路径: {文件路径}");
            await 告示组件.等待确认显示("备份成功", $"储存文件路径: {文件路径}");
            // await UniTask.Delay(2000);
            告示组件.关闭();
            Destroy(进度告示实例);

            Debug.Log($"[备份管理] 备份成功: {文件路径}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[备份管理] 备份失败: {ex.Message}\n{ex.StackTrace}");
            var 错误告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
            var 错误告示实例 = Instantiate(错误告示, this.transform);
            await 错误告示实例.GetComponent<告示_茉>().等待确认显示("备份失败", ex.Message);
            Destroy(错误告示实例);
        }
    }

    async void 执行还原()
    {
        var tip = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var tips = Instantiate(tip, this.transform);
        var 确认 = await tips.GetComponent<告示_茉>().等待确认显示("确认还原", "还原后将覆盖现有数据\n确认继续？");
        Destroy(tips);

        if (!确认)
        {
            return;
        }

        try
        {
            // 获取备份目录
            string 备份目录 = 获取备份目录();
            if (!Directory.Exists(备份目录))
            {
                await 显示提示信息("还原失败", "备份目录不存在");
                return;
            }

            // 查找最新的备份文件
            string[] 备份文件 = Directory.GetFiles(备份目录, "备份_*.json");
            if (备份文件 == null || 备份文件.Length == 0)
            {
                await 显示提示信息("还原失败", "未找到备份文件");
                return;
            }

            // 按修改时间排序，获取最新的备份文件
            string 最新备份文件 = 备份文件.OrderByDescending(f => new FileInfo(f).LastWriteTime).FirstOrDefault();
            if (string.IsNullOrEmpty(最新备份文件) || !File.Exists(最新备份文件))
            {
                await 显示提示信息("还原失败", "备份文件不存在或已损坏");
                return;
            }

            // 显示还原进度
            var 进度告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
            var 进度告示实例 = Instantiate(进度告示, this.transform);
            var 告示组件 = 进度告示实例.GetComponent<告示_茉>();
            告示组件.显示进度("还原中", $"正在从备份文件还原数据...\n文件: {Path.GetFileName(最新备份文件)}");

            // 读取备份文件
            告示组件.更新内容("正在读取备份文件...");
            string json数据 = await File.ReadAllTextAsync(最新备份文件, Encoding.UTF8);

            // 解析JSON数据
            告示组件.更新内容("正在解析备份数据...");
            JsonData 备份数据 = JsonMapper.ToObject(json数据);
            if (备份数据 == null)
            {
                throw new Exception("备份文件格式错误，无法解析");
            }

            int 还原成功数量 = 0;
            int 还原失败数量 = 0;

            // 先清空现有数据
            告示组件.更新内容("正在清空现有数据...");
            评人录?.ClearItems();
            用户录?.ClearItems();
            档案录?.ClearItems();

            // 还原评人录
            if (备份数据.Keys.Contains("评人录") && 评人录 != null)
            {
                告示组件.更新内容("正在还原评人录...");
                try
                {
                    var 评人数据 = 备份数据["评人录"];
                    if (评人数据 != null && 评人数据.IsArray)
                    {
                        var 评人列表 = JsonMapper.ToObject<List<评人>>(评人数据.ToJson());
                        if (评人列表 != null)
                        {
                            foreach (var 评人 in 评人列表)
                            {
                                if (评人 != null)
                                {
                                    评人录.AddItem(评人, false);
                                    还原成功数量++;
                                }
                                else
                                {
                                    还原失败数量++;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[备份管理] 还原评人录失败: {ex.Message}");
                    还原失败数量++;
                }
            }

            // 还原用户录
            if (备份数据.Keys.Contains("用户录") && 用户录 != null)
            {
                告示组件.更新内容("正在还原用户录...");
                try
                {
                    var 用户数据 = 备份数据["用户录"];
                    if (用户数据 != null && 用户数据.IsArray)
                    {
                        var 用户列表 = JsonMapper.ToObject<List<用户>>(用户数据.ToJson());
                        if (用户列表 != null)
                        {
                            foreach (var 用户 in 用户列表)
                            {
                                if (用户 != null)
                                {
                                    用户录.AddItem(用户, false);
                                    还原成功数量++;
                                }
                                else
                                {
                                    还原失败数量++;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[备份管理] 还原用户录失败: {ex.Message}");
                    还原失败数量++;
                }
            }

            // 还原档案录
            if (备份数据.Keys.Contains("档案录") && 档案录 != null)
            {
                告示组件.更新内容("正在还原档案录...");
                try
                {
                    var 档案数据 = 备份数据["档案录"];
                    if (档案数据 != null && 档案数据.IsArray)
                    {
                        var 档案列表 = JsonMapper.ToObject<List<档案>>(档案数据.ToJson());
                        if (档案列表 != null)
                        {
                            foreach (var 档案 in 档案列表)
                            {
                                if (档案 != null)
                                {
                                    档案录.AddItem(档案, false);
                                    还原成功数量++;
                                }
                                else
                                {
                                    还原失败数量++;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[备份管理] 还原档案录失败: {ex.Message}");
                    还原失败数量++;
                }
            }

            // 显示还原结果
            string 结果消息 = 还原失败数量 > 0 
                ? $"从{Path.GetFileName(最新备份文件)}还原完成成功: {还原成功数量} 条\n失败: {还原失败数量} 条"
                : $"从{Path.GetFileName(最新备份文件)}还原完成成功: {还原成功数量} 条";
         await   告示组件.等待确认显示("还原成功", 结果消息);
            
         //   await UniTask.Delay(2000);
            告示组件.关闭();
            Destroy(进度告示实例);

            Debug.Log($"[备份管理] 还原成功: 成功 {还原成功数量} 条, 失败 {还原失败数量} 条");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[备份管理] 还原失败: {ex.Message}\n{ex.StackTrace}");
            var 错误告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
            var 错误告示实例 = Instantiate(错误告示, this.transform);
            await 错误告示实例.GetComponent<告示_茉>().等待确认显示("还原失败", ex.Message);
            Destroy(错误告示实例);
        }
    }

    /// <summary>
    /// 获取备份目录路径（跨平台）
    /// </summary>
    private string 获取备份目录()
    {
        string 备份目录;
        
#if UNITY_ANDROID && !UNITY_EDITOR
        // 安卓端：使用persistentDataPath
        备份目录 = Path.Combine(Application.persistentDataPath, "Backups");
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        // 电脑端：使用persistentDataPath或文档目录
        string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        备份目录 = Path.Combine(documentsPath, Application.productName, "Backups");
#else
        // 其他平台：使用persistentDataPath
        备份目录 = Path.Combine(Application.persistentDataPath, "Backups");
#endif
        
        return 备份目录;
    }

    private void 显示备份选择界面()
    {
        当前已选选项.Clear();
        清空同步结果();
        重置日期文本();
        设置界面显隐(true, false, false);
        清空待同步数据();
    }

    private void 初始化蓝牙同步流程()
    {
        设置界面显隐(false, false, false);
        重置日期文本();
        清空同步结果();

        数据备份下一步按钮?.OnClicked.AddListener(处理备份选项确认);
        开始日期按钮?.OnClicked.AddListener(() => 打开择日器(true).Forget());
        结束日期按钮?.OnClicked.AddListener(() => 打开择日器(false).Forget());
        日期下一步按钮?.OnClicked.AddListener(() => 处理日期确认().Forget());
        日期上一步按钮?.OnClicked.AddListener(返回备份选择界面);
        返回日期选择按钮?.OnClicked.AddListener(返回日期选择界面);
        批量同步按钮?.OnClicked.AddListener(() => 处理批量同步().Forget());
    //    执行同步按钮?.OnClicked.AddListener(处理开始同步);
    }

    private void 设置界面显隐(bool 备份, bool 同步, bool 显示)
    {
        if (选择数据备份 != null) 选择数据备份.SetActive(备份);
        if (选择数据同步 != null) 选择数据同步.SetActive(同步);
        if (数据显示界面 != null) 数据显示界面.SetActive(显示);
    }

    private void 重置日期文本()
    {
        已选开始日期 = null;
        已选结束日期 = null;
        已选开始日期字符串 = 日期占位文案;
        已选结束日期字符串 = 日期占位文案;
        更新日期文本(开始日期文本, 日期占位文案);
        更新日期文本(结束日期文本, 日期占位文案);
    }

    private void 更新日期文本(TextBlock 文本块, string 内容)
    {
        if (文本块 == null) return;
        文本块.Text = 内容;
        var 颜色 = 内容 == 日期占位文案 ? Color.gray * 0.8f : Color.black;
        文本块.Color = 颜色;
    }

    private void 清空同步结果()
    {
        同步条目缓存.Clear();
        选项条数缓存.Clear();
        if (数据显示文本 != null)
        {
            数据显示文本.Text = string.Empty;
        }
    }

    private void 处理备份选项确认()
    {
        if (!收集勾选项())
        {
            Debug.LogWarning("[备份蓝牙同步] 请至少勾选一个数据类别");
            return;
        }

        设置界面显隐(false, true, false);
    }

    private bool 收集勾选项()
    {
        当前已选选项.Clear();
        if (蓝牙同步选项 == null || 蓝牙同步选项.Count == 0)
        {
            Debug.LogWarning("[备份蓝牙同步] 未配置任何勾选选项");
            return false;
        }

        foreach (var 选项 in 蓝牙同步选项)
        {
            if (选项 == null || 选项.勾选框 == null)
            {
                continue;
            }

            if (选项.勾选框.ToggledOn)
            {
                当前已选选项.Add(选项);
            }
        }

        return 当前已选选项.Count > 0;
    }

    private void 处理开始同步()
    {
        if (当前已选选项.Count == 0 && !收集勾选项())
        {
            Debug.LogWarning("[备份蓝牙同步] 请先选择需要同步的数据");
            return;
        }

        if (!日期范围已准备好(out var 提示))
        {
            Debug.LogWarning($"[备份蓝牙同步] {提示}");
            return;
        }

        构建同步条目缓存();
        更新数据显示界面();
    }

    private bool 日期范围已准备好(out string 提示)
    {
        提示 = string.Empty;
        if (!已选开始日期.HasValue)
        {
            提示 = "请选择开始日期";
            return false;
        }

        if (!已选结束日期.HasValue)
        {
            提示 = "请选择结束日期";
            return false;
        }

        if (已选开始日期.Value > 已选结束日期.Value)
        {
            提示 = "开始日期不能晚于结束日期";
            return false;
        }

        return true;
    }

    private void 构建同步条目缓存()
    {
        同步条目缓存.Clear();
        if (当前已选选项.Count == 0) return;

        foreach (var 选项 in 当前已选选项)
        {
            var 名称 = 获取选项名称(选项);
            if (选项 == null || 选项.数据源 == 蓝牙数据源.未指定)
            {
                同步条目缓存.Add($"{名称} | 未绑定数据源");
                continue;
            }

            var 条数 = 获取选项条数(选项);
            var 结果 = $"{名称} | 时间段 {已选开始日期字符串} ~ {已选结束日期字符串} | 条目数 {条数}";
            同步条目缓存.Add(结果);
        }
    }

    private void 更新数据显示界面()
    {
        设置界面显隐(false, false, true);
        if (数据显示文本 == null)
        {
            Debug.LogWarning("[备份蓝牙同步] 未绑定数据显示文本组件");
            return;
        }

        var builder = new StringBuilder();
        builder.AppendLine("蓝牙同步任务：");

        if (同步条目缓存.Count == 0)
        {
            builder.AppendLine("• 暂无同步条目");
        }
        else
        {
            foreach (var 条目 in 同步条目缓存)
            {
                builder.AppendLine($"• {条目}");
            }
        }

        数据显示文本.Text = builder.ToString();
    }

    private string 获取选项名称(蓝牙同步勾选项 选项)
    {
        if (选项 == null) return "未命名数据";
        if (!string.IsNullOrWhiteSpace(选项.名称)) return 选项.名称;
        if (选项.勾选框 != null) return 选项.勾选框.name;
        return "未命名数据";
    }

    private int 获取选项条数(蓝牙同步勾选项 选项)
    {
        if (选项 != null && 选项条数缓存.TryGetValue(选项, out var 条数))
        {
            return 条数;
        }

        return 获取数据条数(选项);
    }

    private int 获取数据条数(蓝牙同步勾选项 选项)
    {
        switch (选项.数据源)
        {
            case 蓝牙数据源.评人录:
                return 评人录?.GetDataList()?.Count ?? 0;
            case 蓝牙数据源.管员录:
                return 管员录?.GetDataList()?.Count ?? 0;
            case 蓝牙数据源.档案录:
                return 档案录?.GetDataList()?.Count ?? 0;
            case 蓝牙数据源.用户录:
                return 用户录?.GetDataList()?.Count ?? 0;
            default:
                return 0;
        }
    }

    private async UniTaskVoid 打开择日器(bool 是开始日期)
    {
        if (择日器显示中)
        {
            return;
        }

        var utility = this.GetUtility<YooAssetPfbPoolUtility>();
        var prefab = await utility.LoadPfb(PC择日器资源名);
        if (prefab == null)
        {
            Debug.LogWarning("[备份蓝牙同步] 未找到PC择日器，尝试通用版本");
            prefab = await utility.LoadPfb(默认择日器资源名);
        }

        if (prefab == null)
        {
            Debug.LogError("[备份蓝牙同步] 加载择日器失败");
            return;
        }

        择日器显示中 = true;
        当前择日器 = Instantiate(prefab, transform);
        var 择日器组件 = 当前择日器.GetComponent<择日器_茉>();
        if (择日器组件 == null)
        {
            Debug.LogError("[备份蓝牙同步] 择日器预制缺少择日器_茉组件");
            Destroy(当前择日器);
            当前择日器 = null;
            择日器显示中 = false;
            return;
        }

        System.Action<int, int, int> 保存回调 = (年, 月, 日) =>
        {
            var 结果 = $"{年}-{月:D2}-{日:D2}";
            var 日期 = new DateTime(年, 月, 日);
            if (是开始日期)
            {
                已选开始日期 = 日期;
                已选开始日期字符串 = 结果;
                更新日期文本(开始日期文本, 已选开始日期字符串);
            }
            else
            {
                已选结束日期 = 日期;
                已选结束日期字符串 = 结果;
                更新日期文本(结束日期文本, 已选结束日期字符串);
            }

            if (当前择日器 != null)
            {
                Destroy(当前择日器);
                当前择日器 = null;
            }
        };

        择日器组件.On保存日期 += 保存回调;
        监控择日器生命周期(择日器组件, 保存回调).Forget();
    }

    private async UniTaskVoid 监控择日器生命周期(择日器_茉 择日器组件, System.Action<int, int, int> 保存回调)
    {
        if (当前择日器 == null)
        {
            择日器显示中 = false;
            return;
        }

        var 目标 = 当前择日器;
        await UniTask.WaitUntil(() => 目标 == null);

        if (择日器组件 != null)
        {
            择日器组件.On保存日期 -= 保存回调;
        }

        择日器显示中 = false;
        当前择日器 = null;
    }

    private void 返回备份选择界面()
    {
        设置界面显隐(true, false, false);
    }

    private void 返回日期选择界面()
    {
        设置界面显隐(false, true, false);
        清空同步结果();
        清空待同步数据();
    }

    private async UniTaskVoid 处理日期确认()
    {
        if (正在处理日期确认)
        {
            return;
        }

        if (!日期范围已准备好(out var 提示))
        {
            await 显示提示信息("日期不完整", 提示);
            return;
        }

        if (当前已选选项.Count == 0 && !收集勾选项())
        {
            await 显示提示信息("缺少数据类别", "请至少勾选一个需要同步的数据");
            return;
        }

        if (!已选开始日期.HasValue || !已选结束日期.HasValue)
        {
            return;
        }

        正在处理日期确认 = true;
        try
        {
            var 待同步列表 = 生成待同步数据列表(已选开始日期.Value, 已选结束日期.Value);
            var 总条数 = 待同步列表.Count;

            if (总条数 == 0)
            {
                await 显示提示信息("暂无数据", "选择的时间范围内没有可同步的数据");
                return;
            }

            var 是否全选 = await 显示全选确认(总条数);
            保存待同步数据(待同步列表);
            构建同步条目缓存();
            // 更新数据显示界面();
            设置界面显隐(false, false, true);
            显示条目面板?.刷新显示();

            if (是否全选)
            {
                同步安卓功能栏?.设置全选(true);
            }
        }
        finally
        {
            正在处理日期确认 = false;
        }
    }

    private List<待同步数据> 生成待同步数据列表(DateTime 开始日期, DateTime 结束日期)
    {
        var 结果 = new List<待同步数据>();
        选项条数缓存.Clear();
        var 范围开始 = 开始日期.Date;
        var 范围结束 = 结束日期.Date;

        foreach (var 选项 in 当前已选选项)
        {
            if (选项 == null || 选项.数据源 == 蓝牙数据源.未指定)
            {
                continue;
            }

            List<待同步数据> 子列表 = 选项.数据源 switch
            {
                蓝牙数据源.档案录 => 构建档案待同步数据(选项, 范围开始, 范围结束),
                蓝牙数据源.评人录 => 构建评人待同步数据(选项, 范围开始, 范围结束, 评人录?.GetDataList(), nameof(评人录)),
                蓝牙数据源.管员录 => 构建评人待同步数据(选项, 范围开始, 范围结束, 管员录?.GetDataList(), nameof(管员录)),
                蓝牙数据源.用户录 => 构建用户待同步数据(选项, 范围开始, 范围结束),
                _ => new List<待同步数据>()
            };

            结果.AddRange(子列表);
            选项条数缓存[选项] = 子列表.Count;
        }

        return 结果;
    }

    private List<待同步数据> 构建档案待同步数据(蓝牙同步勾选项 选项, DateTime 范围开始, DateTime 范围结束)
    {
        var 结果 = new List<待同步数据>();
        var 列表 = 档案录?.GetDataList();
        if (列表 == null || 列表.Count == 0)
        {
            return 结果;
        }

        var 类型名称 = 获取选项名称(选项);

        for (int i = 0; i < 列表.Count; i++)
        {
            var 档案 = 列表[i];
            if (档案 == null)
            {
                continue;
            }
            if(档案.评估状态 != 评估状态.评估完成)
            {
                continue;
            }
            var 时间文本 = 获取首个非空(档案.评估时间, 档案.添加时间);
            if (!Try解析日期(时间文本, out var 时间) || !日期在范围内(时间, 范围开始, 范围结束))
            {
                continue;
            }

            结果.Add(new 待同步数据
            {
                原数据录名称 = nameof(档案录),
                待同步索引 = i,
                编号 = 档案.编号 ?? string.Empty,
                姓名 = 档案.姓名 ?? string.Empty,
                性别 = 档案.性别 ?? string.Empty,
                添加时间 = 时间文本 ?? string.Empty,
                类型 = 类型名称
            });
        }

        return 结果;
    }

    private List<待同步数据> 构建评人待同步数据(蓝牙同步勾选项 选项, DateTime 范围开始, DateTime 范围结束, List<评人> 列表, string 来源名称)
    {
        var 结果 = new List<待同步数据>();
        if (列表 == null || 列表.Count == 0)
        {
            return 结果;
        }

        var 类型名称 = 获取选项名称(选项);

        for (int i = 0; i < 列表.Count; i++)
        {
            var 记录 = 列表[i];
            if (记录 == null)
            {
                continue;
            }

            var 时间文本 = 获取首个非空(记录.诞辰);
            if (!Try解析日期(时间文本, out var 时间) || !日期在范围内(时间, 范围开始, 范围结束))
            {
                continue;
            }

            结果.Add(new 待同步数据
            {
                原数据录名称 = 来源名称,
                待同步索引 = i,
                编号 = 记录.账号 ?? string.Empty,
                姓名 = 记录.名 ?? string.Empty,
                性别 = 记录.性别 ?? string.Empty,
                添加时间 = 时间文本 ?? string.Empty,
                类型 = 类型名称
            });
        }

        return 结果;
    }

    private List<待同步数据> 构建用户待同步数据(蓝牙同步勾选项 选项, DateTime 范围开始, DateTime 范围结束)
    {
        var 结果 = new List<待同步数据>();
        var 列表 = 用户录?.GetDataList();
        if (列表 == null || 列表.Count == 0)
        {
            return 结果;
        }

        var 类型名称 = 获取选项名称(选项);

        for (int i = 0; i < 列表.Count; i++)
        {
            var 用户 = 列表[i];
            if (用户 == null)
            {
                continue;
            }

            var 时间文本 = 获取首个非空(用户.发病日期, 用户.诞辰);
            if (!Try解析日期(时间文本, out var 时间) || !日期在范围内(时间, 范围开始, 范围结束))
            {
                continue;
            }

            结果.Add(new 待同步数据
            {
                原数据录名称 = nameof(用户录),
                待同步索引 = i,
                编号 = 用户.编号 ?? string.Empty,
                姓名 = 用户.名 ?? string.Empty,
                性别 = 用户.性别 ?? string.Empty,
                添加时间 = 时间文本 ?? string.Empty,
                类型 = 类型名称
            });
        }

        return 结果;
    }

    private void 保存待同步数据(List<待同步数据> 数据列表)
    {
        if (待同步数据录 == null)
        {
            Debug.LogError("[备份管理界面_茉] 待同步数据录为空，无法保存数据");
            return;
        }

        待同步数据录.ClearItems();
        foreach (var 数据 in 数据列表)
        {
            if (数据 == null) continue;
            待同步数据录.AddItem(数据, false);
        }
    }

    private void 清空待同步数据()
    {
        if (待同步数据录 == null)
        {
            return;
        }

        if (待同步数据录.GetDataList().Count == 0)
        {
            return;
        }

        待同步数据录.ClearItems();
    }

    private async UniTaskVoid 处理批量同步()
    {
        if (同步安卓功能栏 == null)
        {
            Debug.LogWarning("[备份蓝牙同步] 未绑定安卓功能栏，无法批量同步");
            return;
        }

        var 选中条目 = 同步安卓功能栏.获取当前选中条目();
        if (选中条目 == null || 选中条目.Count == 0)
        {
            Debug.Log("[备份蓝牙同步] 当前没有已选条目可用于批量同步");
            return;
        }
        var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 界面管理器 = FindObjectOfType<界面管理器_韩>();
        var ins = Instantiate(告示, 界面管理器.transform);
        var 户择 = await ins.GetComponent<告示_茉>().等待确认显示("提示", "所有选中数据将同步至客户端，是否继续？");
        Destroy(ins);
        
        if (!户择)
        {
            Debug.Log("[备份蓝牙同步] 用户取消批量同步");
            return;
        }

        Debug.Log($"[备份蓝牙同步] 执行批量同步，条目数：{选中条目.Count}");
        
        // 获取蓝牙数据同步工具
        var 同步工具 = this.GetUtility<I蓝牙数据同步工具>();
        if (同步工具 == null)
        {
            await 显示提示信息("错误", "无法获取蓝牙数据同步工具，请检查设备连接");
            return;
        }

        // 获取待同步数据录
        if (待同步数据录 == null)
        {
            await 显示提示信息("错误", "待同步数据录为空，无法执行同步");
            return;
        }

        var 待同步列表 = 待同步数据录.GetDataList();
        if (待同步列表 == null || 待同步列表.Count == 0)
        {
            await 显示提示信息("错误", "待同步数据列表为空");
            return;
        }

        // 显示同步进度告示
        var 进度告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 进度告示实例 = Instantiate(进度告示, 界面管理器.transform);
        var 告示组件 = 进度告示实例.GetComponent<告示_茉>();
        
        // 计算总预计时间
        int 总预计秒数 = 计算批量同步预计时间(选中条目, 待同步列表);
        string 预计时间文本 = 总预计秒数 > 0 ? $"预计耗时: {总预计秒数}秒" : "预计耗时: 计算中...";
        
        // 显示进度告示
        告示组件.显示进度("批量同步中", $"正在同步 {选中条目.Count} 条数据...\n{预计时间文本}");
        
        int 成功数量 = 0;
        int 失败数量 = 0;
        int 当前进度 = 0;
        var 开始时间 = DateTime.Now;

        // 遍历选中的条目
        foreach (var 条目对象 in 选中条目)
        {
            if (条目对象 == null) continue;

            // 获取条目显示组件
            var 条目显示 = 条目对象.GetComponent<待同步数据条目显示_茉>();
            if (条目显示 == null)
            {
                Debug.LogWarning($"[备份蓝牙同步] 条目对象 {条目对象.name} 缺少 待同步数据条目显示_茉 组件");
                失败数量++;
                当前进度++;
                更新批量同步进度(告示组件, 当前进度, 选中条目.Count, 总预计秒数, 开始时间);
                continue;
            }

            // 获取索引
            int 索引 = 条目显示.索引下标;
            if (索引 < 0 || 索引 >= 待同步列表.Count)
            {
                Debug.LogWarning($"[备份蓝牙同步] 索引无效: {索引}");
                失败数量++;
                当前进度++;
                更新批量同步进度(告示组件, 当前进度, 选中条目.Count, 总预计秒数, 开始时间);
                continue;
            }

            // 获取待同步数据
            var 待同步数据 = 待同步列表[索引];
            if (待同步数据 == null)
            {
                Debug.LogWarning($"[备份蓝牙同步] 待同步数据为空，索引: {索引}");
                失败数量++;
                当前进度++;
                更新批量同步进度(告示组件, 当前进度, 选中条目.Count, 总预计秒数, 开始时间);
                continue;
            }

            // 更新进度显示
            更新批量同步进度(告示组件, 当前进度, 选中条目.Count, 总预计秒数, 开始时间, 待同步数据.姓名);

            // 使用工具类发送数据
            bool 发送成功 = await 同步工具.发送待同步数据(待同步数据);
            if (发送成功)
            {
                成功数量++;
                Debug.Log($"[备份蓝牙同步] 成功发送数据: {待同步数据.姓名} ({待同步数据.类型})");
            }
            else
            {
                失败数量++;
                Debug.LogWarning($"[备份蓝牙同步] 发送失败: {待同步数据.姓名} ({待同步数据.类型})");
            }

            当前进度++;
            更新批量同步进度(告示组件, 当前进度, 选中条目.Count, 总预计秒数, 开始时间);

            // 添加小延迟，避免发送过快
            await UniTask.Delay(50);
        }

        // 更新告示显示同步结果
        var 结果消息 = $"同步完成\n成功: {成功数量} 条\n失败: {失败数量} 条";
        告示组件.显示进度("同步成功", 结果消息);
        
        // 等待1秒后关闭告示
        await UniTask.Delay(1000);
        告示组件.关闭();
    }

    /// <summary>
    /// 计算批量同步预计时间（秒）
    /// </summary>
    private int 计算批量同步预计时间(System.Collections.Generic.IReadOnlyList<GameObject> 选中条目, List<待同步数据> 待同步列表)
    {
        try
        {
            int 总预计时间 = 0;
            
            foreach (var 条目对象 in 选中条目)
            {
                if (条目对象 == null) continue;
                
                var 条目显示 = 条目对象.GetComponent<待同步数据条目显示_茉>();
                if (条目显示 == null) continue;
                
                int 索引 = 条目显示.索引下标;
                if (索引 < 0 || 索引 >= 待同步列表.Count) continue;
                
                var 待同步数据 = 待同步列表[索引];
                if (待同步数据 == null) continue;
                
                // 根据数据类型估算时间
                int 基础时间 = 2; // 基础时间2秒
                int 额外时间 = 0;
                
                switch (待同步数据.原数据录名称)
                {
                    case nameof(档案录):
                        额外时间 = 5; // 档案数据较大，额外5秒
                        break;
                    case nameof(评人录):
                    case nameof(管员录):
                    case nameof(用户录):
                        额外时间 = 2; // 其他数据较小，额外2秒
                        break;
                    default:
                        额外时间 = 3;
                        break;
                }
                
                总预计时间 += 基础时间 + 额外时间;
            }
            
            return 总预计时间;
        }
        catch
        {
            // 默认估算：每条数据5秒
            return 选中条目.Count * 5;
        }
    }

    /// <summary>
    /// 更新批量同步进度显示
    /// </summary>
    private void 更新批量同步进度(告示_茉 告示组件, int 当前进度, int 总数量, int 总预计秒数, DateTime 开始时间, string 当前姓名 = null)
    {
        if (告示组件 == null) return;
        
        var 已过时间 = DateTime.Now - 开始时间;
        int 已过秒数 = (int)已过时间.TotalSeconds;
        
        // 根据进度估算剩余时间
        int 剩余秒数 = 0;
        if (当前进度 > 0 && 总数量 > 0)
        {
            float 进度比例 = (float)当前进度 / 总数量;
            int 预计总时间 = (int)(已过秒数 / 进度比例);
            剩余秒数 = Math.Max(0, 预计总时间 - 已过秒数);
        }
        else if (总预计秒数 > 0)
        {
            剩余秒数 = Math.Max(0, 总预计秒数 - 已过秒数);
        }
        
        string 进度文本 = $"正在同步 {当前进度}/{总数量} 条数据...\n";
        
        if (!string.IsNullOrEmpty(当前姓名))
        {
            进度文本 += $"当前: {当前姓名}\n";
        }
        
        if (剩余秒数 > 0)
        {
            进度文本 += $"预计剩余: {剩余秒数}秒";
        }
        else
        {
            进度文本 += "即将完成...";
        }
        
        告示组件.更新内容(进度文本);
    }

    private string 获取首个非空(params string[] 文本集合)
    {
        foreach (var 文本 in 文本集合)
        {
            if (!string.IsNullOrWhiteSpace(文本))
            {
                return 文本;
            }
        }

        return string.Empty;
    }

    private bool Try解析日期(string 输入, out DateTime 时间)
    {
        时间 = default;
        if (string.IsNullOrWhiteSpace(输入))
        {
            return false;
        }

        string[] 格式 =
        {
            "yyyy-MM-dd",
            "yyyy/MM/dd",
            "yyyy.MM.dd",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy/MM/dd HH:mm:ss",
            "yyyy.MM.dd HH:mm:ss"
        };

        if (DateTime.TryParseExact(输入, 格式, CultureInfo.InvariantCulture, DateTimeStyles.None, out 时间))
        {
            return true;
        }

        return DateTime.TryParse(输入, out 时间);
    }

    private bool 日期在范围内(DateTime 时间, DateTime 范围开始, DateTime 范围结束)
    {
        var 日期 = 时间.Date;
        return 日期 >= 范围开始 && 日期 <= 范围结束;
    }

    private async UniTask<bool> 显示全选确认(int 总条数)
    {
        var 内容 = $"总共有{总条数}条数据 请问是否要全选";
        return await 显示确认告示("蓝牙同步", 内容);
    }

    private async UniTask 显示提示信息(string 标题, string 内容)
    {
        await 显示确认告示(标题, 内容);
    }

    private async UniTask<bool> 显示确认告示(string 标题, string 内容)
    {
        var utility = this.GetUtility<YooAssetPfbPoolUtility>();
        var prefab = await utility.LoadPfb("告示");
        if (prefab == null)
        {
            Debug.LogError("[备份蓝牙同步] 加载告示预制体失败");
            return false;
        }

        var tips = Instantiate(prefab, transform);
        var 结果 = await tips.GetComponent<告示_茉>().等待确认显示(标题, 内容);
        Destroy(tips);
        return 结果;
    }

    void 返回上个界面()
    {
        Debug.Log("返回上个界面");
        var 界面管理器 = FindObjectOfType<界面管理器_韩>();
        if (界面管理器 != null)
        {
            界面管理器.隐藏界面();
        }
    }

    protected override void OnShow()
    {
        Debug.Log("[备份管理界面_茉] OnShow");
    }

    protected override void OnHide()
    {
        Debug.Log("[备份管理界面_茉] OnHide");
    }

    void OnDestroy()
    {
        确认备份按钮?.OnClicked.RemoveAllListeners();
        确认还原按钮?.OnClicked.RemoveAllListeners();
        蓝牙同步按钮?.OnClicked.RemoveAllListeners();
        数据备份下一步按钮?.OnClicked.RemoveAllListeners();
        开始日期按钮?.OnClicked.RemoveAllListeners();
        结束日期按钮?.OnClicked.RemoveAllListeners();
        日期下一步按钮?.OnClicked.RemoveAllListeners();
        日期上一步按钮?.OnClicked.RemoveAllListeners();
        返回日期选择按钮?.OnClicked.RemoveAllListeners();
        批量同步按钮?.OnClicked.RemoveAllListeners();
        
       // 执行同步按钮?.OnClicked.RemoveAllListeners();
    }

    [Serializable]
    public class 蓝牙同步勾选项
    {
        public string 名称 = "未命名数据";
        public Toggle 勾选框;
        public 蓝牙数据源 数据源 = 蓝牙数据源.未指定;
    }

    public enum 蓝牙数据源
    {
        未指定,
        评人录,
        管员录,
        用户录,
        档案录
    }
}
