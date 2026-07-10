using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using QFramework;
using Nova;
using NovaSamples.UIControls;
using Cysharp.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class 打印报告界面_茉 : 界面基础_韩
{
    [Header("基本信息")]

    [SerializeField] private List<TextBlock> 报告编号列表 = new List<TextBlock>(); // 多个报告编号显示位置
    [SerializeField] private TextBlock 姓名;
    [SerializeField] private TextBlock 性别;
    [SerializeField] private TextBlock 出生日期;
    [SerializeField] private TextBlock 编号;
    [SerializeField] private TextBlock 年龄;
    [SerializeField] private TextBlock 评估人员;
    [SerializeField] private TextBlock 评估时间;

    [Header("个人/背景信息")]
    [SerializeField] private TextBlock 身份证;
    [SerializeField] private TextBlock 联系方式;
    [SerializeField] private TextBlock 发病日期;
    [SerializeField] private TextBlock 文化程度;
    [SerializeField] private TextBlock 血压;
    [SerializeField] private TextBlock 神志状态;
    [SerializeField] private TextBlock 家庭成员;
    [SerializeField] private TextBlock 烟龄;
    [SerializeField] private TextBlock 酒龄;
    [SerializeField] private TextBlock 发病前后语言状态;
    [SerializeField] private TextBlock 发病时状况;
    [SerializeField] private TextBlock 既往病史;

    [Header("界面容器")]
    [SerializeField] private GameObject 量表报告界面;
    [SerializeField] private GameObject 量表表格界面;
    [SerializeField] private 量表表格可视化_茉 量表表格可视化;



    [Header("量表评估分数")]
    [SerializeField] private TextBlock 谈话信息量总分;
    [SerializeField] private TextBlock 谈话流利性总分;
    [SerializeField] private TextBlock 系列语言总分;
    [SerializeField] private TextBlock 是否题总分;
    [SerializeField] private TextBlock 听辨认总分;

    [SerializeField] private TextBlock 口头指令总分;
    [SerializeField] private TextBlock 词复述总分;
    [SerializeField] private TextBlock 句复述总分;
    [SerializeField] private TextBlock 词命名总分;
    [SerializeField] private TextBlock 列名总分;
    [SerializeField] private TextBlock 颜色识别总分;
    [SerializeField] private TextBlock 颜色命名总分;
    [SerializeField] private TextBlock 反应性命名总分;
    [SerializeField] private TextBlock 视读总分;
    [SerializeField] private TextBlock 阅读指令总分;
    [SerializeField] private TextBlock 字画匹配总分;
    [SerializeField] private TextBlock 执行指令总分;

    [SerializeField] private TextBlock 填空总分;
    [SerializeField] private TextBlock 运算总分;
    [SerializeField] private TextBlock 写姓名地址总分;
    [SerializeField] private TextBlock 抄写总分;
    [SerializeField] private TextBlock 系列书写总分;
    [SerializeField] private TextBlock 听写总分;
    [SerializeField] private TextBlock 看图写字总分;
    [SerializeField] private TextBlock 写病情总分;
    [SerializeField] private TextBlock 照画图总分;
    [SerializeField] private TextBlock 摆方块总分;
    [SerializeField] private TextBlock 面部动作总分;
    [SerializeField] private TextBlock 上肢动作总分;
    [SerializeField] private TextBlock 复杂动作总分;

    [Header("打印相关")]
    [SerializeField] private Scroller 报告Scroller; // 原始报告滚动容器
    [SerializeField] private GameObject 打印机选择界面;
    [SerializeField] private Dropdown 打印机下拉框;
    [SerializeField] private Button 刷新打印机按钮;
    [SerializeField] private Button 确认选择打印机按钮;
    [SerializeField] private Button 取消选择打印机按钮;
    [SerializeField] private TextBlock 打印机状态文本;
    [SerializeField] private bool 仅显示真实打印机 = true;
    [SerializeField] private List<string> 虚拟打印机关键词 = new List<string>
    {
        "pdf",
        "xps",
        "fax",
        "onenote",
        "send to onenote",
        "microsoft print to pdf",
        "microsoft xps document writer"
    };
    private 打印管理器_茉 打印管理器; // 打印管理器
    private IScreenshotPDFUtility 截图打印工具;
    private readonly List<string> 可用打印机列表 = new List<string>();
    private string 当前选中打印机名 = string.Empty;

    [Header("评估结果")]
    [SerializeField] private TextBlock 利手评定结果;
    [SerializeField] private TextBlock 失语症评定;
    [SerializeField] private TextBlock BDAE严重程度;

    [Header("综合信息")]
    [SerializeField] private TextBlock 综合评价;
    [SerializeField] private TextBlock 指导建议;

    // 背景信息选项组
    private List<选项组性_茉> 选项组性列表 = new List<选项组性_茉>();
    private List<选项组性多选_茉> 选项组性多选列表 = new List<选项组性多选_茉>();

    private 档案 当前显示档案;

    [HideInInspector] public bool 是否为打印;
    [HideInInspector] public bool 显示量表报告;
    [HideInInspector] public bool 显示量表表格;

    [SerializeField] private Button 返回按钮;
    [SerializeField] private Button 返回按钮1;
    [SerializeField] private Button 确认打印;
    [SerializeField] private TextBlock 确认打印按钮文本;
    [SerializeField] private string 确认打印文案 = "确认打印";
    [SerializeField] private string 确认下载文案 = "确认下载";
    private bool 打印机下拉框已初始化;
    private bool 打印机选择按钮已初始化;
    void Awake()
    {
        初始化选项组();
        显示信息 += 显示档案详情;
        返回按钮.OnClicked.AddListener(() =>
     {
         处理返回按钮点击().Forget();

     });
        返回按钮1.OnClicked.AddListener(() =>
        {
            处理返回按钮点击().Forget();
        });
        确认打印.OnClicked.AddListener(处理确认打印按钮点击);

        // 初始化打印管理器
        if (打印管理器 == null)
        {
            打印管理器 = gameObject.GetComponent<打印管理器_茉>();
        }
        截图打印工具 = this.GetUtility<IScreenshotPDFUtility>();
        初始化打印机下拉框();
        初始化打印机选择按钮();
        隐藏打印机选择界面();

     
    }
    void 处理确认打印按钮点击()
    {
        if (!是否为打印)
        {
            处理确认下载按钮点击().Forget();
            return;
        }

        显示打印机选择界面();
        if (!打印机下拉框已初始化)
        {
            初始化打印机下拉框();
            return;
        }
        刷新打印机列表();
    }

    async UniTaskVoid 处理确认下载按钮点击()
    {
        if (当前显示档案 == null)
        {
            Debug.LogError("[打印报告] 当前档案为空，无法下载Word文档");
            return;
        }

        if (打印管理器 == null)
        {
            打印管理器 = gameObject.GetComponent<打印管理器_茉>();
        }
        if (打印管理器 == null)
        {
            Debug.LogError("[打印报告] 打印管理器未初始化，无法下载Word文档");
            return;
        }

        string 默认文件名 = $"评估报告_{当前显示档案.编号 ?? "未知"}_{DateTime.Now:yyyyMMdd_HHmmss}.docx";
        string 目标路径 = 选择下载保存路径(默认文件名);
        if (string.IsNullOrWhiteSpace(目标路径))
        {
            Debug.Log("[打印报告] 用户取消下载保存位置选择");
            return;
        }

        string 导出文件路径 = await 打印管理器.导出Word模板文档(当前显示档案);
        if (string.IsNullOrWhiteSpace(导出文件路径) || !File.Exists(导出文件路径))
        {
            Debug.LogError("[打印报告] Word文档导出失败，未生成可下载文件");
            return;
        }

        try
        {
            string 目标目录 = Path.GetDirectoryName(目标路径);
            if (!string.IsNullOrWhiteSpace(目标目录))
            {
                Directory.CreateDirectory(目标目录);
            }

            string 标准目标路径 = Path.GetFullPath(目标路径);
            string 标准导出路径 = Path.GetFullPath(导出文件路径);
            if (!标准目标路径.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
            {
                标准目标路径 += ".docx";
            }

            if (!string.Equals(标准导出路径, 标准目标路径, StringComparison.OrdinalIgnoreCase))
            {
                File.Copy(标准导出路径, 标准目标路径, true);
            }

            Debug.Log($"[打印报告] 下载完成: {标准目标路径}");
            await 显示告示提示("下载成功", $"文档已保存");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[打印报告] 复制下载文件失败: {ex.Message}");
            await 显示告示提示("下载失败", ex.Message);
        }
    }

    string 选择下载保存路径(string 默认文件名)
    {
#if UNITY_EDITOR
        string editorPath = EditorUtility.SaveFilePanel("选择下载位置", "", 默认文件名, "docx");
        return string.IsNullOrWhiteSpace(editorPath) ? null : editorPath;
#else
        try
        {
            IFilePickerBridge filePicker = new FilePickerBridge();
            string 初始目录 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return filePicker.SaveFileDialog(
                "选择下载位置",
                默认文件名,
                "Word文档|*.docx|所有文件|*.*",
                初始目录);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[打印报告] 打开下载保存对话框失败: {ex.Message}");
            return null;
        }
#endif
    }

    void 刷新确认按钮文案()
    {
        if (确认打印按钮文本 == null && 确认打印 != null)
        {
            确认打印按钮文本 = 确认打印.GetComponentInChildren<TextBlock>(true);
        }

        if (确认打印按钮文本 == null)
        {
            return;
        }

        确认打印按钮文本.Text = 是否为打印 ? 确认打印文案 : 确认下载文案;
    }

    async UniTask 显示告示提示(string 标题, string 内容)
    {
        try
        {
            var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
            var 界面管理器 = FindObjectOfType<界面管理器_韩>();
            if (告示 == null || 界面管理器 == null)
            {
                return;
            }

            var ins = Instantiate(告示, 界面管理器.transform);
            var 组件 = ins.GetComponent<告示_茉>();
            if (组件 != null)
            {
                await 组件.等待确认显示(标题, 内容);
            }
            Destroy(ins);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[打印报告] 显示告示失败: {ex.Message}");
        }
    }

    async UniTaskVoid 处理确认选择打印机按钮点击()
    {
        if (打印机选择界面 == null || !打印机选择界面.activeSelf)
        {
            显示打印机选择界面();
            return;
        }

        var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 界面管理器 = FindObjectOfType<界面管理器_韩>();
        var ins = Instantiate(告示, 界面管理器.transform);
        var 户择 = await ins.GetComponent<告示_茉>().等待确认显示("提示", "即将打印报告，请确保打印机连接正常");
        Destroy(ins);
        if (!户择)
        {
            return;
        }

        if (打印管理器 == null)
        {
            Debug.LogError("[打印报告] 打印管理器未初始化");
            return;
        }
        if (string.IsNullOrWhiteSpace(当前选中打印机名))
        {
            自动选择默认打印机();
        }
        if (string.IsNullOrWhiteSpace(当前选中打印机名))
        {
            Debug.LogError("[打印报告] 未选择有效打印机，请先选择打印机");
            return;
        }

        // if (当前显示档案 == null)
        // {
        //     Debug.LogError("[打印报告] 当前显示档案为空");
        //     return;
        // }

        // 调用打印管理器执行打印
        bool 打印成功 = await 打印管理器.执行打印(当前显示档案, 当前选中打印机名);
        隐藏打印机选择界面();

        if (打印成功)
        {
            await 显示告示提示("打印成功", $"报告已发送到打印机：{当前选中打印机名}");
        }
        else
        {
            await 显示告示提示("打印失败", "未能发送到打印机，请检查打印机状态后重试。");
        }
    }
    async UniTaskVoid 处理返回按钮点击()
    {
        var 界面管理器 = FindObjectOfType<界面管理器_韩>();
        FindObjectOfType<界面管理器_韩>().显示界面("档案管理报告界面");
        await UniTask.WaitUntil(() => 界面管理器._currentPanel != null && 界面管理器._currentPanel.name.Contains("档案管理报告界面"));
        界面管理器._currentPanel.显示信息?.Invoke(当前显示档案);
    }

    void 初始化打印机下拉框()
    {
        确保打印机选择界面激活();

        if (打印机下拉框已初始化)
        {
            刷新打印机列表();
            return;
        }

        if (打印机下拉框 == null)
        {
            var 下拉框列表 = GetComponentsInChildren<Dropdown>(true);
            foreach (var item in 下拉框列表)
            {
                if (item != null && item.name.Contains("打印机"))
                {
                    打印机下拉框 = item;
                    break;
                }
            }
        }

        if (打印机下拉框 == null)
        {
            Debug.LogWarning("[打印报告] 未绑定打印机下拉框，无法使用自定义打印机选择");
            自动选择默认打印机();
            return;
        }

        打印机下拉框.OnValueChanged.AddListener(打印机下拉框值变化);
        if (刷新打印机按钮 != null)
        {
            刷新打印机按钮.OnClicked.AddListener(刷新打印机列表);
        }

        打印机下拉框已初始化 = true;
        刷新打印机列表();
    }

    void 初始化打印机选择按钮()
    {
        if (打印机选择按钮已初始化)
        {
            return;
        }

        if (打印机选择界面 == null)
        {
            var 所有节点 = GetComponentsInChildren<Transform>(true);
            foreach (var 节点 in 所有节点)
            {
                if (节点 != null && 节点.name.Contains("打印机选择"))
                {
                    打印机选择界面 = 节点.gameObject;
                    break;
                }
            }
        }

        if (打印机选择界面 != null && (确认选择打印机按钮 == null || 取消选择打印机按钮 == null))
        {
            var 按钮列表 = 打印机选择界面.GetComponentsInChildren<Button>(true);
            foreach (var 按钮 in 按钮列表)
            {
                if (按钮 == null)
                {
                    continue;
                }

                if (确认选择打印机按钮 == null && 按钮.name.Contains("确认选择"))
                {
                    确认选择打印机按钮 = 按钮;
                }
                else if (取消选择打印机按钮 == null && (按钮.name.Contains("取消") || 按钮.name.Contains("关闭")))
                {
                    取消选择打印机按钮 = 按钮;
                }
            }
        }

        if (确认选择打印机按钮 != null)
        {
            确认选择打印机按钮.OnClicked.AddListener(() =>
            {
                处理确认选择打印机按钮点击().Forget();
            });
        }
        else
        {
            Debug.LogWarning("[打印报告] 未绑定“确认选择打印机”按钮，无法在打印机选择界面中执行打印");
        }

        if (取消选择打印机按钮 != null)
        {
            取消选择打印机按钮.OnClicked.AddListener(隐藏打印机选择界面);
        }

        打印机选择按钮已初始化 = true;
    }

    void 显示打印机选择界面()
    {
        确保打印机选择界面激活();
    }

    void 隐藏打印机选择界面()
    {
        if (打印机选择界面 == null)
        {
            return;
        }

        if (打印机选择界面.activeSelf)
        {
            打印机选择界面.SetActive(false);
        }
    }

    void 确保打印机选择界面激活()
    {
        if (打印机选择界面 == null)
        {
            var 所有节点 = GetComponentsInChildren<Transform>(true);
            foreach (var 节点 in 所有节点)
            {
                if (节点 != null && 节点.name.Contains("打印机选择"))
                {
                    打印机选择界面 = 节点.gameObject;
                    break;
                }
            }
        }

        if (打印机选择界面 != null && !打印机选择界面.activeSelf)
        {
            打印机选择界面.SetActive(true);
            Debug.Log("[打印报告] 初始化打印机前激活打印机选择界面");
        }
    }

    void 打印机下拉框值变化(string _)
    {
        更新当前选中打印机();
    }

    void 刷新打印机列表()
    {
        if (打印机下拉框 == null)
        {
            return;
        }

        可用打印机列表.Clear();
        if (截图打印工具 != null)
        {
            var 列表 = 截图打印工具.GetAvailablePrinterNames();
            if (列表 != null)
            {
                可用打印机列表.AddRange(列表);
            }
        }
        过滤打印机列表(可用打印机列表);

        打印机下拉框.DropdownOptions.Options.Clear();

        if (可用打印机列表.Count == 0)
        {
            打印机下拉框.DropdownOptions.Options.Add("未检测到打印机");
            打印机下拉框.DropdownOptions.SelectedIndex = 0;
            当前选中打印机名 = string.Empty;
            刷新打印机下拉框显示();
            更新打印机状态文本();
            Debug.LogWarning("[打印报告] 未检测到可用打印机");
            return;
        }

        foreach (var 打印机 in 可用打印机列表)
        {
            打印机下拉框.DropdownOptions.Options.Add(打印机);
        }

        int 默认索引 = 0;
        if (截图打印工具 != null)
        {
            string 默认打印机 = 截图打印工具.GetDefaultPrinterName();
            if (!string.IsNullOrEmpty(默认打印机))
            {
                int idx = 可用打印机列表.IndexOf(默认打印机);
                if (idx >= 0)
                {
                    默认索引 = idx;
                }
            }
        }

        打印机下拉框.DropdownOptions.SelectedIndex = 默认索引;
        当前选中打印机名 = 可用打印机列表[默认索引];

        刷新打印机下拉框显示();
        更新打印机状态文本();
        Debug.Log($"[打印报告] 打印机列表已刷新，共 {可用打印机列表.Count} 台，当前: {当前选中打印机名}");
    }

    void 自动选择默认打印机()
    {
        可用打印机列表.Clear();
        if (截图打印工具 != null)
        {
            var 列表 = 截图打印工具.GetAvailablePrinterNames();
            if (列表 != null)
            {
                可用打印机列表.AddRange(列表);
            }
        }
        过滤打印机列表(可用打印机列表);

        if (可用打印机列表.Count == 0)
        {
            当前选中打印机名 = string.Empty;
            更新打印机状态文本();
            return;
        }

        int 默认索引 = 0;
        string 默认打印机 = 截图打印工具?.GetDefaultPrinterName();
        if (!string.IsNullOrEmpty(默认打印机))
        {
            int idx = 可用打印机列表.IndexOf(默认打印机);
            if (idx >= 0)
            {
                默认索引 = idx;
            }
        }

        当前选中打印机名 = 可用打印机列表[默认索引];
        更新打印机状态文本();
        Debug.Log($"[打印报告] 自动选择打印机: {当前选中打印机名}");
    }

    void 过滤打印机列表(List<string> 打印机列表)
    {
        if (!仅显示真实打印机 || 打印机列表 == null || 打印机列表.Count == 0)
        {
            return;
        }

        var 过滤后列表 = new List<string>();
        foreach (var 名称 in 打印机列表)
        {
            if (string.IsNullOrWhiteSpace(名称))
            {
                continue;
            }

            string lowerName = 名称.Trim().ToLowerInvariant();
            bool 是虚拟打印机 = false;
            foreach (var 关键词 in 虚拟打印机关键词)
            {
                if (string.IsNullOrWhiteSpace(关键词))
                {
                    continue;
                }

                if (lowerName.Contains(关键词.ToLowerInvariant()))
                {
                    是虚拟打印机 = true;
                    break;
                }
            }

            if (!是虚拟打印机)
            {
                过滤后列表.Add(名称);
            }
        }

        if (过滤后列表.Count > 0)
        {
            打印机列表.Clear();
            打印机列表.AddRange(过滤后列表);
        }
    }

    void 更新当前选中打印机()
    {
        if (打印机下拉框 == null || 可用打印机列表.Count == 0)
        {
            当前选中打印机名 = string.Empty;
            更新打印机状态文本();
            return;
        }

        int 索引 = 打印机下拉框.DropdownOptions.SelectedIndex;
        if (索引 < 0 || 索引 >= 可用打印机列表.Count)
        {
            当前选中打印机名 = string.Empty;
            更新打印机状态文本();
            return;
        }

        当前选中打印机名 = 可用打印机列表[索引];
        更新打印机状态文本();
    }

    void 刷新打印机下拉框显示()
    {
        if (打印机下拉框 == null)
        {
            return;
        }

        if (打印机下拉框.Visuals != null && 打印机下拉框.Visuals.OptionsView != null)
        {
            打印机下拉框.Visuals.OptionsView.Refresh();
        }

        if (打印机下拉框.Visuals != null && 打印机下拉框.Visuals.SelectionLabel != null)
        {
            打印机下拉框.Visuals.SelectionLabel.Text = 打印机下拉框.DropdownOptions.CurrentSelection;
        }
    }

    void 更新打印机状态文本()
    {
        if (打印机状态文本 == null)
        {
            return;
        }

        打印机状态文本.Text = string.IsNullOrEmpty(当前选中打印机名)
            ? "当前打印机：未选择"
            : $"当前打印机：{当前选中打印机名}";
    }
    void 显示档案详情(I记贞 记)
    {
        当前显示档案 = 记 as 档案;
        刷新确认按钮文案();
        隐藏打印机选择界面();

        if (当前显示档案 == null)
        {
            Debug.LogWarning("[打印报告] 传入的档案数据为空");
            return;
        }

        Debug.Log($"[打印报告] 显示档案: {当前显示档案.姓名}(编号:{当前显示档案.编号})");

        // 根据勾选状态显示对应界面
        控制界面显示();

        // 基本与背景信息
        显示基本信息(当前显示档案);
        显示背景信息(当前显示档案);
        还原背景信息选项组(当前显示档案);

        显示量表评估分数(当前显示档案);

        // 显示综合信息（综合评价和指导建议）
        显示综合信息(当前显示档案);

        // 如果显示量表表格，更新可视化
        if (显示量表表格 && 量表表格可视化 != null)
        {
            更新量表表格可视化(当前显示档案);
        }
    }

    /// <summary>
    /// 根据勾选状态控制界面显示
    /// </summary>
    void 控制界面显示()
    {
        // 默认都隐藏
        if (量表报告界面 != null) 量表报告界面.SetActive(false);
        if (量表表格界面 != null) 量表表格界面.SetActive(false);

        // 根据勾选状态显示对应界面
        if (显示量表报告 && 量表报告界面 != null)
        {
            量表报告界面.SetActive(true);
            Debug.Log("[打印报告] 显示量表报告界面");
        }

        if (显示量表表格 && 量表表格界面 != null)
        {
            量表表格界面.SetActive(true);
            Debug.Log("[打印报告] 显示量表表格界面");
        }
        Debug.Log("[打印报告] 显示量表报告: " + 显示量表报告 + " 显示量表表格: " + 显示量表表格);
        // 如果两个都勾选，都显示
        if (显示量表报告 && 显示量表表格)
        {
            Debug.Log("[打印报告] 同时显示量表报告和量表表格");
        }

        // 如果都没勾选，默认显示量表报告
        if (!显示量表报告 && !显示量表表格 && 量表报告界面 != null)
        {
            量表报告界面.SetActive(true);
            Debug.LogWarning("[打印报告] 未勾选任何选项，默认显示量表报告界面");
        }
    }

    void 显示量表评估分数(档案 档案)
    {
        // 谈话记录
        if (档案.谈话记录 != null)
        {
            if (谈话信息量总分 != null) 谈话信息量总分.Text = 档案.谈话记录.谈话信息量总分.ToString();
            if (谈话流利性总分 != null) 谈话流利性总分.Text = 档案.谈话记录.谈话流利性总分.ToString();
            if (系列语言总分 != null) 系列语言总分.Text = 档案.谈话记录.系列语言总分.ToString();
        }

        // 理解记录
        if (档案.理解记录 != null)
        {
            if (是否题总分 != null) 是否题总分.Text = 档案.理解记录.是否题总分.ToString();
            if (听辨认总分 != null) 听辨认总分.Text = 档案.理解记录.听辨认图形总分.ToString();
            if (口头指令总分 != null) 口头指令总分.Text = 档案.理解记录.口头指令总分.ToString();
        }

        // 复述记录
        if (档案.复述记录 != null)
        {
            if (词复述总分 != null) 词复述总分.Text = 档案.复述记录.词复述总分.ToString();
            if (句复述总分 != null) 句复述总分.Text = 档案.复述记录.句复述总分.ToString();
        }

        // 命名记录
        if (档案.命名记录 != null)
        {
            if (词命名总分 != null) 词命名总分.Text = 档案.命名记录.词命名总分.ToString();
            if (颜色识别总分 != null) 颜色识别总分.Text = 档案.命名记录.颜色识别总分.ToString();
            if (颜色命名总分 != null) 颜色命名总分.Text = 档案.命名记录.颜色命名总分.ToString();
            if (反应性命名总分 != null) 反应性命名总分.Text = 档案.命名记录.反应性命名总分.ToString();
            if (列名总分 != null) 列名总分.Text = 档案.命名记录.列名 ?? "";
        }

        // 阅读记录
        if (档案.阅读记录 != null)
        {
            if (视读总分 != null) 视读总分.Text = 档案.阅读记录.视读总分.ToString();
            if (阅读指令总分 != null) 阅读指令总分.Text = 档案.阅读记录.阅读指令总分.ToString();
            if (字画匹配总分 != null) 字画匹配总分.Text = (档案.阅读记录.字画匹配朗读总分 + 档案.阅读记录.字画匹配配画总分).ToString();
            if (执行指令总分 != null) 执行指令总分.Text = (档案.阅读记录.执行指令朗读总分 + 档案.阅读记录.执行指令配画总分).ToString();
            if (填空总分 != null) 填空总分.Text = 档案.阅读记录.填空总分.ToString();
        }

        // 计算记录
        if (档案.计算记录 != null)
        {
            if (运算总分 != null) 运算总分.Text = 档案.计算记录.运算总分.ToString();
        }

        // 书写记录
        if (档案.书写记录 != null)
        {
            if (写姓名地址总分 != null) 写姓名地址总分.Text = 档案.书写记录.写姓名地址总分.ToString();
            if (抄写总分 != null) 抄写总分.Text = 档案.书写记录.抄写总分.ToString();
            if (系列书写总分 != null) 系列书写总分.Text = 档案.书写记录.系列书写总分.ToString();
            if (听写总分 != null) 听写总分.Text = 档案.书写记录.听写总分.ToString();
            if (看图写字总分 != null) 看图写字总分.Text = 档案.书写记录.看图写字总分.ToString();
            if (写病情总分 != null) 写病情总分.Text = 档案.书写记录.写病情分数.ToString();
        }

        // 结构与空间记录
        if (档案.结构与空间记录 != null)
        {
            if (照画图总分 != null) 照画图总分.Text = 档案.结构与空间记录.照画图总分.ToString();
            if (摆方块总分 != null) 摆方块总分.Text = 档案.结构与空间记录.摆方块总分.ToString();
        }

        // 运用记录
        if (档案.运用记录 != null)
        {
            if (面部动作总分 != null) 面部动作总分.Text = 档案.运用记录.面部动作总分.ToString("0.##");
            if (上肢动作总分 != null) 上肢动作总分.Text = 档案.运用记录.上肢动作总分.ToString("0.##");
            if (复杂动作总分 != null) 复杂动作总分.Text = 档案.运用记录.复杂动作总分.ToString();
        }

        // 利手记录
        if (档案.利手记录 != null)
        {
            if (利手评定结果 != null) 利手评定结果.Text = 档案.利手记录.利手评定结果 ?? "";
        }

        // 失语症评定
        显示失语症评定结果(档案);

        // BDAE严重程度
        显示BDAE严重程度(档案);
    }

    void 显示失语症评定结果(档案 档案)
    {
        if (失语症评定 == null) return;

        string 失语症类型 = 失语症评定工具.计算失语症类型(档案);
        string 完整名称 = 失语症评定工具.获取失语症类型全称(失语症类型);
        失语症评定.Text = 完整名称;

        Debug.Log($"[打印报告] 失语症评定结果: {完整名称}");
    }

    void 显示BDAE严重程度(档案 档案)
    {
        if (BDAE严重程度 == null) return;

        int 等级 = 档案.BDAE严重程度等级;
        string 描述 = BDAE严重程度评定工具.获取严重程度描述(等级);
        BDAE严重程度.Text = 描述;

        Debug.Log($"[打印报告] BDAE严重程度: {等级}级 - {描述}");
    }

    /// <summary>
    /// 显示综合信息（综合评价和指导建议）
    /// </summary>
    void 显示综合信息(档案 档案)
    {
        if (综合评价 != null) 综合评价.Text = 档案.综合评价 ?? "";
        if (指导建议 != null) 指导建议.Text = 档案.指导建议 ?? "";

        Debug.Log($"[打印报告] 综合评价: {档案.综合评价 ?? "无"}, 指导建议: {档案.指导建议 ?? "无"}");
    }

    /// <summary>
    /// 更新量表表格可视化
    /// </summary>
    void 更新量表表格可视化(档案 档案)
    {
        Dictionary<string, float> 分数字典 = new Dictionary<string, float>();

        // 谈话记录
        if (档案.谈话记录 != null)
        {
            分数字典["谈话信息量"] = 档案.谈话记录.谈话信息量总分 * 100 / 6;
            分数字典["谈话流利性"] = 档案.谈话记录.谈话流利性总分 * 100 / 27;
            分数字典["系列语言"] = 档案.谈话记录.系列语言总分 * 100 / 21;
        }

        // 理解记录
        if (档案.理解记录 != null)
        {
            分数字典["是否题"] = 档案.理解记录.是否题总分 * 100 / 60;
            分数字典["听辨认"] = 档案.理解记录.听辨认图形总分 * 100 / 20;
            分数字典["口头指令"] = 档案.理解记录.口头指令总分 * 100 / 80;
        }

        // 复述记录
        if (档案.复述记录 != null)
        {
            分数字典["词复述"] = (档案.复述记录.词复述总分 + 档案.复述记录.句复述总分) * 100 / 100;
            //    分数字典["句复述"] = 档案.复述记录.句复述总分 *100 /76;
        }

        // 命名记录
        if (档案.命名记录 != null)
        {
            分数字典["词命名"] = 档案.命名记录.词命名总分 * 100 / 40;
            分数字典["颜色识别"] = 档案.命名记录.颜色识别总分 * 100 / 6;
            分数字典["颜色命名"] = 档案.命名记录.颜色命名总分 * 100 / 6;
            分数字典["反应性命名"] = 档案.命名记录.反应性命名总分 * 100 / 10;
        }

        // 阅读记录
        if (档案.阅读记录 != null)
        {
            分数字典["视读"] = 档案.阅读记录.视读总分 * 100 / 10;
            分数字典["阅读指令"] = 档案.阅读记录.阅读指令总分 * 100 / 10;
            分数字典["字画匹配朗读"] = 档案.阅读记录.字画匹配朗读总分 * 100 / 20;
            分数字典["字画匹配理解"] = 档案.阅读记录.字画匹配配画总分 * 100 / 20;

            分数字典["执行指令朗读"] = 档案.阅读记录.执行指令朗读总分 * 100 / 15;
            分数字典["执行指令理解"] = 档案.阅读记录.执行指令配画总分 * 100 / 15;
            分数字典["填空"] = 档案.阅读记录.填空总分 * 100 / 30;
        }

        // 计算记录
        if (档案.计算记录 != null)
        {
            分数字典["运算"] = 档案.计算记录.运算总分 * 100 / 24;
        }

        // 书写记录
        if (档案.书写记录 != null)
        {
            分数字典["写姓名地址"] = 档案.书写记录.写姓名地址总分 * 100 / 10;
            分数字典["抄写"] = 档案.书写记录.抄写总分 * 100 / 7;
            分数字典["系列书写"] = 档案.书写记录.系列书写总分 * 100 / 24;
            分数字典["听写"] = 档案.书写记录.听写总分 * 100 / 34;
            分数字典["看图写字"] = 档案.书写记录.看图写字总分 * 100 / 20;
            分数字典["写病情"] = 档案.书写记录.写病情分数 * 100 / 5;
        }

        // 结构与空间记录
        if (档案.结构与空间记录 != null)
        {
            分数字典["照画图"] = 档案.结构与空间记录.照画图总分 * 100 / 10;
            分数字典["摆方块"] = 档案.结构与空间记录.摆方块总分 * 100 / 10;
        }

        // 运用记录
        if (档案.运用记录 != null)
        {
            分数字典["面部动作"] = Mathf.RoundToInt(档案.运用记录.面部动作总分 * 100f / 8f);
            分数字典["上肢动作"] = Mathf.RoundToInt(档案.运用记录.上肢动作总分 * 100f / 8f);
            分数字典["复杂动作"] = 档案.运用记录.复杂动作总分 * 100 / 14;
        }

        // 更新可视化
        量表表格可视化.设置分数(分数字典);

        Debug.Log($"[打印报告] 量表表格可视化已更新，共{分数字典.Count}个维度");
    }

    void 初始化选项组()
    {
        选项组性列表.Clear();
        选项组性多选列表.Clear();

        选项组性列表.AddRange(GetComponentsInChildren<选项组性_茉>(true));
        选项组性多选列表.AddRange(GetComponentsInChildren<选项组性多选_茉>(true));

        Debug.Log($"[打印报告] 找到 {选项组性列表.Count} 个选项组性，{选项组性多选列表.Count} 个选项组性多选");
    }

    async void 还原背景信息选项组(档案 档案)
    {
        if (档案 == null || 档案.背景信息选项组选择 == null || 档案.背景信息选项组选择.Count == 0) return;
        await UniTask.Delay(100);

        foreach (var 选项组 in 选项组性列表)
        {
            if (选项组 != null && !string.IsNullOrEmpty(选项组.选项组名称) &&
                档案.背景信息选项组选择.ContainsKey(选项组.选项组名称))
            {
                var 数据 = 档案.背景信息选项组选择[选项组.选项组名称].Split('|');
                if (数据.Length == 2 && int.TryParse(数据[1], out int 索引))
                    选项组.设置选中选项(索引);
            }
        }

        foreach (var 选项组 in 选项组性多选列表)
        {
            if (选项组 != null && !string.IsNullOrEmpty(选项组.选项组名称) &&
                档案.背景信息选项组选择.ContainsKey(选项组.选项组名称))
            {
                var 数据 = 档案.背景信息选项组选择[选项组.选项组名称].Split('|');
                if (数据.Length == 2)
                {
                    var 索引列表 = new List<int>();
                    foreach (var 索引字符串 in 数据[1].Split(','))
                    {
                        if (int.TryParse(索引字符串, out int 索引))
                            索引列表.Add(索引);
                    }
                    选项组.批量设置选中选项(索引列表);
                }
            }
        }
    }

    /// <summary>
    /// 显示基本信息与评定人/日期
    /// </summary>
    void 显示基本信息(档案 档案)
    {
        string 报告编号文本 = $"报告编号:{档案.编号 ?? ""}";

     
        // 设置多个报告编号显示位置
        foreach (var textBlock in 报告编号列表)
        {
            if (textBlock != null) textBlock.Text = 报告编号文本;
        }

        if (姓名 != null) 姓名.Text = 档案.姓名 ?? "";
        if (性别 != null) 性别.Text = 档案.性别 ?? "";
        if (出生日期 != null) 出生日期.Text = 档案.出生日期 ?? "";
        if (编号 != null) 编号.Text = 档案.编号 ?? "";
        if (年龄 != null) 年龄.Text = 档案.年龄 ?? "";
        if (评估人员 != null) 评估人员.Text = 档案.评估人员 ?? "";
        if (评估时间 != null) 评估时间.Text = 档案.评估时间 ?? "";
    }

    /// <summary>
    /// 显示背景资料
    /// </summary>
    void 显示背景信息(档案 档案)
    {
        if (身份证 != null) 身份证.Text = 档案.身份证 ?? "";
        if (联系方式 != null) 联系方式.Text = 档案.联系方式 ?? "";
        if (发病日期 != null) 发病日期.Text = 档案.发病日期 ?? "";
        if (文化程度 != null) 文化程度.Text = 档案.文化程度 ?? "";
        if (血压 != null) 血压.Text = 档案.血压 ?? "";
        if (神志状态 != null) 神志状态.Text = 档案.神志状态 ?? "";
        if (家庭成员 != null) 家庭成员.Text = 档案.家庭成员 ?? "";
        if (烟龄 != null) 烟龄.Text = 档案.烟龄 ?? "";
        if (酒龄 != null) 酒龄.Text = 档案.酒龄 ?? "";
        if (发病前后语言状态 != null) 发病前后语言状态.Text = 档案.发病前后语言状态 ?? "";
        if (发病时状况 != null) 发病时状况.Text = 档案.发病时状况 ?? "";
        if (既往病史 != null) 既往病史.Text = 档案.既往病史 ?? "";
    }

    void OnDestroy()
    {
        显示信息 -= 显示档案详情;
    }

}
