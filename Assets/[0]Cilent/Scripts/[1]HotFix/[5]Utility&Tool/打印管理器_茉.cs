using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security;
using System.Text;
using UnityEngine;
using QFramework;
using Nova;
using Cysharp.Threading.Tasks;

/// <summary>
/// 打印栏目类型枚举
/// </summary>
public enum 打印栏目类型
{
    基本信息,
    评估报告表,
    综合评价指导建议,
    评估表格
}

/// <summary>
/// 打印管理器 - 专门负责报告打印逻辑
/// </summary>
public class 打印管理器_茉 : MonoBehaviour, IController
{
    [Header("打印相关设置")]
    [SerializeField] private Scroller 报告Scroller; // 原始报告滚动容器
    [SerializeField] private UIBlock2D 打印根Block; // 顶部需要铺满X、Y自适应的Block
    [SerializeField] private List<GameObject> 打印时隐藏对象; // 打印时需要去除的按钮/控件

    [Header("打印栏目")]
    [SerializeField] private GameObject 基本信息栏目;
    [SerializeField] private GameObject 评估报告表栏目;
    [SerializeField] private GameObject 综合评价栏目;
    [SerializeField] private GameObject 评估表格栏目;

    [Header("栏目间隔")]
    [SerializeField] private GameObject 基本信息栏目间隔;
    [SerializeField] private GameObject 评估报告表栏目间隔;
    [SerializeField] private GameObject 综合评价栏目间隔;
    [SerializeField] private GameObject 评估表格栏目间隔;
    [SerializeField] private GameObject 评估表格栏目间隔2;
    
    [Header("溢出检测（用于避免文本块被分页分割）")]
    [SerializeField] private Transform 发病前后语言状态_检测对象;
    [SerializeField] private Transform 发病时状况_检测对象;
    [SerializeField] private Transform 既往病史_检测对象;

    [SerializeField] private UIBlock2D 发病前后语言状态_block调整;
    [SerializeField] private UIBlock2D 发病时状况_block调整;
    [SerializeField] private UIBlock2D 既往病史_block调整;

    [Header("溢出阈值与调整Y（像素，编辑器可调）")]
    [SerializeField] private float 溢出阈值_发病前后 = 900f;
    [SerializeField] private float 溢出阈值_发病时 = 900f;
    [SerializeField] private float 溢出阈值_既往 = 900f;

    [SerializeField] private float 调整Y_发病前后 = 200f;
    [SerializeField] private float 调整Y_发病时 = 200f;
    [SerializeField] private float 调整Y_既往 = 200f;

    [Header("Word模板打印")]
    [SerializeField] private bool 启用Word模板打印 = true;
    [SerializeField] private string Word模板文件路径 = @"g:\Users\Desktop\马经理\2026.4.9汉语失语症检查表（ABC法）测验报告内容_tag模板.docx";
    [SerializeField] private string Word输出目录 = "";
    [SerializeField] private string StreamingAssets模板相对路径 = @"WordTemplates\2026.4.9汉语失语症检查表（ABC法）测验报告内容_tag模板.docx";
    [SerializeField] private bool 启用YooAssets读取模板 = false;
    [SerializeField] private string YooAssets模板RawFile名 = "ABC报告模板.docx";
    [SerializeField] private bool YooAssets模板从本地包读取 = true;
    [SerializeField] private bool Word插入评估表格截图 = true;
    [SerializeField] private string Word评估表格截图标签 = "assessment_table_image";
    [SerializeField] private float Word评估表格截图宽度厘米 = 16.5f;

    private IScreenshotPDFUtility _screenshotUtil;
    // 已经在此次打印过程中调整过的块，避免重复设置
    private HashSet<UIBlock2D> _已调整块 = new HashSet<UIBlock2D>();

    public IArchitecture GetArchitecture()
    {
        return HotFixTemplateArchitecture.Interface;
    }

    void Awake()
    {
        _screenshotUtil = this.GetUtility<IScreenshotPDFUtility>();
        if (_screenshotUtil == null)
        {
            Debug.LogError("[打印管理器] 未找到 ScreenshotPDFUtility");
        }
    }


    /// <summary>
    /// 设置显示特定栏目，隐藏其他栏目
    /// </summary>
    public void 设置显示栏目(打印栏目类型 目标栏目, GameObject 克隆根对象)
    {
        // 首先检查必要的引用是否已设置
        if (基本信息栏目 == null || 评估报告表栏目 == null || 综合评价栏目 == null || 评估表格栏目 == null)
        {
            Debug.LogError("[打印管理器] 栏目引用未设置，请先调用设置栏目引用方法");
            return;
        }

        // 获取所有栏目和间隔的字典
        var 栏目字典 = new Dictionary<打印栏目类型, GameObject>()
        {
            { 打印栏目类型.基本信息, 查找栏目对象(克隆根对象, 基本信息栏目) },
            { 打印栏目类型.评估报告表, 查找栏目对象(克隆根对象, 评估报告表栏目) },
            { 打印栏目类型.综合评价指导建议, 查找栏目对象(克隆根对象, 综合评价栏目) },
            { 打印栏目类型.评估表格, 查找栏目对象(克隆根对象, 评估表格栏目) }
        };

        var 间隔字典 = new Dictionary<打印栏目类型, List<GameObject>>()
        {
            { 打印栏目类型.基本信息, new List<GameObject> { 查找栏目对象(克隆根对象, 基本信息栏目间隔) } },
            { 打印栏目类型.评估报告表, new List<GameObject> { 查找栏目对象(克隆根对象, 评估报告表栏目间隔) } },
            { 打印栏目类型.综合评价指导建议, new List<GameObject> { 查找栏目对象(克隆根对象, 综合评价栏目间隔) } },
            { 打印栏目类型.评估表格, new List<GameObject> {
                查找栏目对象(克隆根对象, 评估表格栏目间隔),
                查找栏目对象(克隆根对象, 评估表格栏目间隔2) } }
        };

        // 先隐藏所有栏目和间隔
        foreach (var 栏目 in 栏目字典.Values)
        {
            if (栏目 != null)
            {
                栏目.SetActive(false);
            }
        }
        foreach (var 间隔列表 in 间隔字典.Values)
        {
            foreach (var 间隔 in 间隔列表)
            {
                if (间隔 != null)
                {
                    间隔.SetActive(false);
                }
            }
        }

        // 显示目标栏目及其对应的间隔
        if (栏目字典.ContainsKey(目标栏目) && 栏目字典[目标栏目] != null)
        {
            栏目字典[目标栏目].SetActive(true);

            // 同时显示对应的间隔（如果存在）
            if (间隔字典.ContainsKey(目标栏目))
            {
                int 间隔数量 = 0;
                foreach (var 间隔 in 间隔字典[目标栏目])
                {
                    if (间隔 != null)
                    {
                        间隔.SetActive(true);
                        间隔数量++;
                    }
                }
                Debug.Log($"[打印管理器] 显示栏目: {目标栏目} (包含 {间隔数量} 个间隔)");
            }
            else
            {
                Debug.Log($"[打印管理器] 显示栏目: {目标栏目} (无间隔)");
            }
        }
        else
        {
            Debug.LogWarning($"[打印管理器] 无法找到栏目对象: {目标栏目}");
        }
    }

    /// <summary>
    /// 在克隆对象中查找对应的栏目对象
    /// </summary>
    private GameObject 查找栏目对象(GameObject 克隆根对象, GameObject 原始栏目对象)
    {
        if (原始栏目对象 == null) return null;

        // 在克隆的层级中按名称查找对应的对象
        var allObjects = 克隆根对象.GetComponentsInChildren<Transform>(true);
        foreach (var t in allObjects)
        {
            if (t.name == 原始栏目对象.name)
            {
                return t.gameObject;
            }
        }
        return null;
    }

    /// <summary>
    /// 测试方法：演示设置显示栏目的功能（异步版本）
    /// </summary>
    public async UniTaskVoid 测试设置显示栏目(GameObject 克隆根对象)
    {
        Debug.Log("[打印管理器] 开始测试设置显示栏目功能");

        // 依次显示每个栏目并等待
        设置显示栏目(打印栏目类型.基本信息, 克隆根对象);
        await UniTask.Delay(1000); // 等待1秒

        设置显示栏目(打印栏目类型.评估报告表, 克隆根对象);
        await UniTask.Delay(1000); // 等待1秒

        设置显示栏目(打印栏目类型.综合评价指导建议, 克隆根对象);
        await UniTask.Delay(1000); // 等待1秒

        设置显示栏目(打印栏目类型.评估表格, 克隆根对象);
        await UniTask.Delay(1000); // 等待1秒

        Debug.Log("[打印管理器] 测试设置显示栏目功能完成");
    }

    /// <summary>
    /// 执行报告打印
    /// </summary>
    public async UniTask<bool> 执行打印(档案 当前档案, string 指定打印机名 = null)
    {
        if (当前档案 == null)
        {
            Debug.LogError("[打印管理器] 当前档案为空，无法打印");
            return false;
        }

        // 优先走 Word 模板打印（字段映射 + docx 直接打印）
        if (await 尝试Word模板打印(当前档案, 指定打印机名))
        {
            return true;
        }

        Debug.LogWarning("[打印管理器] Word模板打印未完成，回退到截图HTML打印流程");

        if (_screenshotUtil == null)
        {
            Debug.LogError("[打印管理器] ScreenshotPDFUtility 未初始化");
            return false;
        }

        if (报告Scroller == null)
        {
            Debug.LogError("[打印管理器] 报告Scroller 未绑定，无法截图");
            return false;
        }

        string fileBaseName = $"评估报告_{当前档案?.编号 ?? "未知"}_{System.DateTime.Now:yyyyMMdd_HHmmss}";

        // 1) 克隆用于打印的 Scroller
        var cloneGO = await 克隆报告界面(fileBaseName);
        if (cloneGO == null) return false;

        List<string> imagePaths = new List<string>();

        try
        {
            // 2) 依次截图每个栏目
            打印栏目类型[] 栏目列表 = {
                打印栏目类型.基本信息,
                打印栏目类型.评估报告表,
                打印栏目类型.综合评价指导建议,
                打印栏目类型.评估表格
            };

            foreach (var 栏目 in 栏目列表)
            {
                // 显示当前栏目
                设置显示栏目(栏目, cloneGO);

                // 等待布局更新
                await UniTask.WaitForEndOfFrame();

                // 应用溢出检测与调整（如果某文本块位置超过阈值，则调整对应 UIBlock2D 的 Y）
                // 应用溢出调整(cloneGO);

                // await UniTask.Delay(500); // 等待500ms确保布局稳定

                // 清空已调整块记录（为每个栏目单独判断）
                // _已调整块.Clear();

                // 截图当前栏目（直接截取完整内容，不管多长）
                string imgPath = await 截图报告(cloneGO, $"{fileBaseName}_{栏目}");
                if (!string.IsNullOrEmpty(imgPath))
                {
                    imagePaths.Add(imgPath);
                    Debug.Log($"[打印管理器] {栏目}截图完成: {imgPath}");
                }
                else
                {
                    Debug.LogError($"[打印管理器] {栏目}截图失败");
                }
            }

            if (imagePaths.Count == 0)
            {
                Debug.LogError("[打印管理器] 没有成功的截图，无法生成打印文件");
                return false;
            }

            // 3) 生成与历史一致的多页HTML（保留原有分页/样式规则）
            string htmlPath = _screenshotUtil.ConvertMultipleImagesToHTML(imagePaths.ToArray(), "评估报告打印预览");
            if (string.IsNullOrEmpty(htmlPath))
            {
                Debug.LogError("[打印管理器] 生成打印预览HTML失败");
                return false;
            }

            // 4) 不打开预览，直接弹打印机选择并打印HTML
            bool printOk = string.IsNullOrWhiteSpace(指定打印机名)
                ? _screenshotUtil.PrintFilesWithPrinterDialog(new[] { htmlPath })
                : _screenshotUtil.PrintFilesWithPrinter(new[] { htmlPath }, 指定打印机名);

            if (!printOk)
            {
                Debug.LogWarning("[打印管理器] 直接打印失败或用户取消选择打印机");
            }

            return printOk;
        }
        finally
        {
            // 5) 清理克隆
            清理克隆对象(cloneGO);
        }
    }

    /// <summary>
    /// 尝试使用 Word 模板填充并直接打印 docx（通过系统关联程序/WPS）。
    /// </summary>
    private async UniTask<bool> 尝试Word模板打印(档案 当前档案, string 指定打印机名)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        if (!启用Word模板打印)
        {
            return false;
        }

        string 输出文件路径 = await 导出Word模板文档(当前档案);
        if (string.IsNullOrEmpty(输出文件路径))
        {
            return false;
        }

        if (_screenshotUtil == null)
        {
            Debug.LogError("[打印管理器] ScreenshotPDFUtility 未初始化，无法调用打印机选择");
            return false;
        }

        bool printOk = string.IsNullOrWhiteSpace(指定打印机名)
            ? _screenshotUtil.PrintFilesWithPrinterDialog(new[] { 输出文件路径 })
            : _screenshotUtil.PrintFilesWithPrinter(new[] { 输出文件路径 }, 指定打印机名);
        if (!printOk)
        {
            Debug.LogWarning($"[打印管理器] Word文档打印失败或用户取消: {输出文件路径}");
            return false;
        }

        Debug.Log($"[打印管理器] Word模板打印完成: {输出文件路径}");
        await UniTask.Yield();
        return true;
#else
        await UniTask.Yield();
        return false;
#endif
    }

    /// <summary>
    /// 仅导出Word模板文档（不打印），成功时返回输出文件完整路径。
    /// </summary>
    public async UniTask<string> 导出Word模板文档(档案 当前档案, string 指定输出文件路径 = null)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        if (当前档案 == null)
        {
            Debug.LogError("[打印管理器] 当前档案为空，无法导出Word文档");
            return null;
        }

        string 输出目录 = 获取Word输出目录();
        if (!string.IsNullOrWhiteSpace(指定输出文件路径))
        {
            string 自定义目录 = Path.GetDirectoryName(指定输出文件路径);
            if (!string.IsNullOrWhiteSpace(自定义目录))
            {
                输出目录 = 自定义目录;
            }
        }

        if (string.IsNullOrEmpty(输出目录))
        {
            Debug.LogError("[打印管理器] Word输出目录无效");
            return null;
        }

        Directory.CreateDirectory(输出目录);
        string 模板路径 = await 获取可用Word模板路径Async(输出目录);
        if (string.IsNullOrEmpty(模板路径))
        {
            Debug.LogWarning("[打印管理器] 未找到可用Word模板路径，无法导出Word文档");
            return null;
        }
        Debug.Log($"[打印管理器] 当前使用Word模板: {模板路径}");

        string 输出文件路径 = 指定输出文件路径;
        if (string.IsNullOrWhiteSpace(输出文件路径))
        {
            输出文件路径 = Path.Combine(
                输出目录,
                $"评估报告_{当前档案.编号 ?? "未知"}_{DateTime.Now:yyyyMMdd_HHmmss}.docx");
        }

        if (!输出文件路径.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
        {
            输出文件路径 += ".docx";
        }

        Dictionary<string, string> 标签映射 = 构建Word标签映射(当前档案);
        string 评估表格截图路径 = null;
        if (Word插入评估表格截图)
        {
            评估表格截图路径 = await 截图评估表格栏目用于Word(当前档案);
        }

        if (!生成并填充Word文档(
            模板路径,
            输出文件路径,
            标签映射,
            评估表格截图路径,
            out string fillError))
        {
            Debug.LogError($"[打印管理器] Word模板填充失败: {fillError}");
            return null;
        }

        Debug.Log($"[打印管理器] Word文档导出完成: {输出文件路径}");
        return 输出文件路径;
#else
        await UniTask.Yield();
        Debug.LogWarning("[打印管理器] 当前平台不支持Word模板导出");
        return null;
#endif
    }

    private async UniTask<string> 获取可用Word模板路径Async(string 输出目录)
    {
        if (!string.IsNullOrEmpty(Word模板文件路径) && File.Exists(Word模板文件路径))
        {
            return Word模板文件路径;
        }

        // 支持相对路径（相对于工程根目录）
        if (!string.IsNullOrEmpty(Word模板文件路径) && !Path.IsPathRooted(Word模板文件路径))
        {
            string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            string relativePath = Word模板文件路径.Replace("/", Path.DirectorySeparatorChar.ToString()).Replace("\\", Path.DirectorySeparatorChar.ToString());
            string combined = Path.Combine(projectRoot, relativePath);
            if (File.Exists(combined))
            {
                return combined;
            }
        }

        // 支持 StreamingAssets 模板
        if (!string.IsNullOrEmpty(StreamingAssets模板相对路径))
        {
            string rel = StreamingAssets模板相对路径.Replace("/", Path.DirectorySeparatorChar.ToString()).Replace("\\", Path.DirectorySeparatorChar.ToString());
            string streamingPath = Path.Combine(Application.streamingAssetsPath, rel);
            if (File.Exists(streamingPath))
            {
                return streamingPath;
            }
        }

        // 支持 YooAssets RawFile 模板：读取后落地到输出目录再使用
        if (启用YooAssets读取模板 && !string.IsNullOrEmpty(YooAssets模板RawFile名))
        {
            try
            {
                byte[] bytes = await LoadYooAssetsTool.LoadRawFile_DP(YooAssets模板RawFile名, YooAssets模板从本地包读取);
                if (bytes != null && bytes.Length > 0)
                {
                    string cachePath = Path.Combine(输出目录, "_Word模板缓存.docx");
                    File.WriteAllBytes(cachePath, bytes);
                    return cachePath;
                }

                Debug.LogWarning($"[打印管理器] YooAssets模板读取为空: {YooAssets模板RawFile名}");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[打印管理器] YooAssets模板读取失败: {YooAssets模板RawFile名}, 错误: {e.Message}");
            }
        }

        // 兜底：允许把模板放到 persistentDataPath/Screenshots 下
        string fallback = Path.Combine(
            Application.persistentDataPath,
            "Screenshots",
            "2026.4.9汉语失语症检查表（ABC法）测验报告内容_tag模板.docx");

        if (File.Exists(fallback))
        {
            return fallback;
        }

        return null;
    }

    private string 获取Word输出目录()
    {
        if (!string.IsNullOrEmpty(Word输出目录))
        {
            return Word输出目录;
        }

        string baseDir = _screenshotUtil != null
            ? _screenshotUtil.GetExportDirectory()
            : Path.Combine(Application.persistentDataPath, "Screenshots");

        return Path.Combine(baseDir, "WordReports");
    }

    private bool 生成并填充Word文档(
        string 模板路径,
        string 输出文件路径,
        Dictionary<string, string> 标签映射,
        string 评估表格截图路径,
        out string error)
    {
        error = null;

        try
        {
            File.Copy(模板路径, 输出文件路径, true);

            using (var fs = new FileStream(输出文件路径, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            using (var zip = new ZipArchive(fs, ZipArchiveMode.Update))
            {
                var xmlEntryNames = new List<string>();
                foreach (var entry in zip.Entries)
                {
                    if (entry.FullName.StartsWith("word/", StringComparison.OrdinalIgnoreCase) &&
                        entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                    {
                        xmlEntryNames.Add(entry.FullName);
                    }
                }

                foreach (var entryName in xmlEntryNames)
                {
                    var entry = zip.GetEntry(entryName);
                    if (entry == null)
                    {
                        continue;
                    }

                    string xml = 读取ZipEntry文本(entry);
                    if (string.IsNullOrEmpty(xml) || xml.IndexOf("{{", StringComparison.Ordinal) < 0)
                    {
                        continue;
                    }

                    string replaced = 替换Xml标签(xml, 标签映射);
                    if (entryName.Equals("word/document.xml", StringComparison.OrdinalIgnoreCase))
                    {
                        replaced = 确保评估数据段前分页(replaced);
                    }
                    if (replaced == xml)
                    {
                        continue;
                    }

                    entry.Delete();
                    写入ZipEntry文本(zip, entryName, replaced);
                }

                if (!string.IsNullOrWhiteSpace(评估表格截图路径) &&
                    !string.IsNullOrWhiteSpace(Word评估表格截图标签))
                {
                    if (!替换标签为图片(zip, Word评估表格截图标签, 评估表格截图路径, Word评估表格截图宽度厘米, out string imageError))
                    {
                        Debug.LogWarning($"[打印管理器] 插入评估表格截图失败: {imageError}");
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }

    private static string 读取ZipEntry文本(ZipArchiveEntry entry)
    {
        using (var stream = entry.Open())
        using (var reader = new StreamReader(stream, Encoding.UTF8, true))
        {
            return reader.ReadToEnd();
        }
    }

    private static void 写入ZipEntry文本(ZipArchive zip, string entryName, string content)
    {
        var newEntry = zip.CreateEntry(entryName, System.IO.Compression.CompressionLevel.Optimal);
        using (var stream = newEntry.Open())
        using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
        {
            writer.Write(content);
        }
    }

    private static string 替换Xml标签(string xml, Dictionary<string, string> 标签映射)
    {
        string result = xml;
        foreach (var kv in 标签映射)
        {
            string tag = "{{" + kv.Key + "}}";
            string value = Xml转义(kv.Value);
            result = result.Replace(tag, value);
        }
        return result;
    }

    private async UniTask<string> 截图评估表格栏目用于Word(档案 当前档案)
    {
        if (_screenshotUtil == null || 报告Scroller == null)
        {
            return null;
        }

        string 编号 = 当前档案?.编号 ?? "未知";
        string fileBaseName = $"Word评估表格_{编号}_{DateTime.Now:yyyyMMdd_HHmmss}";
        var cloneGO = await 克隆报告界面(fileBaseName);
        if (cloneGO == null)
        {
            return null;
        }

        try
        {
            设置显示栏目(打印栏目类型.评估表格, cloneGO);
            await UniTask.WaitForEndOfFrame();
            await UniTask.Delay(100);

            string imgPath = await 截图报告(cloneGO, fileBaseName);
            if (!string.IsNullOrWhiteSpace(imgPath))
            {
                Debug.Log($"[打印管理器] Word评估表格截图完成: {imgPath}");
            }
            return imgPath;
        }
        finally
        {
            清理克隆对象(cloneGO);
        }
    }

    private bool 替换标签为图片(
        ZipArchive zip,
        string 标签名,
        string 图片路径,
        float 宽度厘米,
        out string error)
    {
        error = null;

        if (zip == null)
        {
            error = "ZipArchive为空";
            return false;
        }
        if (string.IsNullOrWhiteSpace(标签名))
        {
            error = "图片标签名为空";
            return false;
        }
        if (string.IsNullOrWhiteSpace(图片路径) || !File.Exists(图片路径))
        {
            error = "图片路径无效或文件不存在";
            return false;
        }

        const string documentXmlPath = "word/document.xml";
        const string relsXmlPath = "word/_rels/document.xml.rels";
        string 标签文本 = "{{" + 标签名 + "}}";

        var docEntry = zip.GetEntry(documentXmlPath);
        var relEntry = zip.GetEntry(relsXmlPath);
        if (docEntry == null || relEntry == null)
        {
            error = "未找到 document.xml 或 document.xml.rels";
            return false;
        }

        string documentXml = 读取ZipEntry文本(docEntry);
        string relsXml = 读取ZipEntry文本(relEntry);
        if (string.IsNullOrEmpty(documentXml))
        {
            error = "document.xml 为空";
            return false;
        }
        bool 模板包含图片标签 = documentXml.IndexOf(标签文本, StringComparison.Ordinal) >= 0;

        byte[] imageBytes = File.ReadAllBytes(图片路径);
        if (imageBytes == null || imageBytes.Length == 0)
        {
            error = "图片字节为空";
            return false;
        }

        string ext = Path.GetExtension(图片路径);
        if (string.IsNullOrWhiteSpace(ext))
        {
            ext = ".png";
        }
        ext = ext.ToLowerInvariant();
        string mediaFileName = $"assessment_table_{DateTime.Now:yyyyMMddHHmmssfff}{ext}";
        string mediaEntryName = "word/media/" + mediaFileName;

        var oldMediaEntry = zip.GetEntry(mediaEntryName);
        oldMediaEntry?.Delete();
        var imageEntry = zip.CreateEntry(mediaEntryName, System.IO.Compression.CompressionLevel.Optimal);
        using (var stream = imageEntry.Open())
        {
            stream.Write(imageBytes, 0, imageBytes.Length);
        }

        string relationId = 生成可用关系Id(relsXml);
        string relationNode =
            $"<Relationship Id=\"{relationId}\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/image\" Target=\"media/{mediaFileName}\" />";
        int relEnd = relsXml.LastIndexOf("</Relationships>", StringComparison.Ordinal);
        if (relEnd < 0)
        {
            error = "document.xml.rels 格式异常";
            return false;
        }
        relsXml = relsXml.Insert(relEnd, relationNode);

        long maxWidthEmu = 计算页面可用宽度Emu(documentXml, 宽度厘米);
        获取图片尺寸Emu(imageBytes, maxWidthEmu, out long cx, out long cy);
        long docPrId = DateTime.Now.Ticks % int.MaxValue;
        string 图像段落Xml = 构建图片段落Xml(relationId, mediaFileName, cx, cy, docPrId);

        documentXml = 确保Word图片命名空间(documentXml);
        if (模板包含图片标签)
        {
            documentXml = 用图片段落替换标签段落(documentXml, 标签文本, 图像段落Xml);
        }
        else
        {
            documentXml = 在文末新页追加图片(documentXml, 图像段落Xml);
            Debug.LogWarning($"[打印管理器] 模板中未找到图片标签 {标签文本}，已自动在文末新页追加评估表格截图");
        }

        docEntry.Delete();
        写入ZipEntry文本(zip, documentXmlPath, documentXml);
        relEntry.Delete();
        写入ZipEntry文本(zip, relsXmlPath, relsXml);
        return true;
    }

    private static string 生成可用关系Id(string relsXml)
    {
        int maxId = 0;
        int searchStart = 0;
        while (true)
        {
            int idx = relsXml.IndexOf("Id=\"rId", searchStart, StringComparison.Ordinal);
            if (idx < 0)
            {
                break;
            }

            int numStart = idx + "Id=\"rId".Length;
            int numEnd = numStart;
            while (numEnd < relsXml.Length && char.IsDigit(relsXml[numEnd]))
            {
                numEnd++;
            }

            if (numEnd > numStart)
            {
                string numText = relsXml.Substring(numStart, numEnd - numStart);
                if (int.TryParse(numText, out int idNum) && idNum > maxId)
                {
                    maxId = idNum;
                }
            }

            searchStart = numEnd;
        }

        string candidate = "rId" + (maxId + 1);
        while (relsXml.IndexOf($"Id=\"{candidate}\"", StringComparison.Ordinal) >= 0)
        {
            maxId++;
            candidate = "rId" + (maxId + 1);
        }
        return candidate;
    }

    private static void 获取图片尺寸Emu(byte[] imageBytes, long maxWidthEmu, out long cx, out long cy)
    {
        if (maxWidthEmu <= 0)
        {
            maxWidthEmu = (long)(16.5f * 360000f);
        }
        cx = maxWidthEmu;
        cy = cx * 3 / 4;

        try
        {
            var tex = new Texture2D(2, 2);
            if (tex.LoadImage(imageBytes))
            {
                float ratio = tex.width > 0 ? (float)tex.height / tex.width : 0.75f;
                cy = (long)(cx * ratio);
            }
            UnityEngine.Object.Destroy(tex);
        }
        catch
        {
            // ignore, keep default ratio
        }

        if (cy <= 0)
        {
            cy = cx * 3 / 4;
        }
    }

    private static long 计算页面可用宽度Emu(string documentXml, float fallbackWidthCm)
    {
        const long emuPerTwip = 635L;
        const long emuPerCm = 360000L;
        if (string.IsNullOrEmpty(documentXml))
        {
            return fallbackWidthCm > 0 ? (long)(fallbackWidthCm * emuPerCm) : 0;
        }

        try
        {
            int marIdx = documentXml.LastIndexOf("<w:pgMar", StringComparison.Ordinal);
            int szIdx = documentXml.LastIndexOf("<w:pgSz", StringComparison.Ordinal);
            if (marIdx < 0 || szIdx < 0)
            {
                return fallbackWidthCm > 0 ? (long)(fallbackWidthCm * emuPerCm) : 0;
            }

            int marEnd = documentXml.IndexOf("/>", marIdx, StringComparison.Ordinal);
            int szEnd = documentXml.IndexOf("/>", szIdx, StringComparison.Ordinal);
            if (marEnd < 0 || szEnd < 0)
            {
                return fallbackWidthCm > 0 ? (long)(fallbackWidthCm * emuPerCm) : 0;
            }

            string marTag = documentXml.Substring(marIdx, marEnd - marIdx + 2);
            string szTag = documentXml.Substring(szIdx, szEnd - szIdx + 2);

            int pageW = 11906; // A4 default twips
            int left = 1800;
            int right = 1800;

            尝试读取整型属性(szTag, "w:w", ref pageW);
            尝试读取整型属性(marTag, "w:left", ref left);
            尝试读取整型属性(marTag, "w:right", ref right);

            int printableTwips = Math.Max(1000, pageW - left - right);
            return printableTwips * emuPerTwip;
        }
        catch
        {
            return fallbackWidthCm > 0 ? (long)(fallbackWidthCm * emuPerCm) : 0;
        }
    }

    private static void 尝试读取整型属性(string tagXml, string attrName, ref int value)
    {
        string key = attrName + "=\"";
        int idx = tagXml.IndexOf(key, StringComparison.Ordinal);
        if (idx < 0)
        {
            return;
        }
        int start = idx + key.Length;
        int end = tagXml.IndexOf("\"", start, StringComparison.Ordinal);
        if (end <= start)
        {
            return;
        }

        string text = tagXml.Substring(start, end - start);
        if (int.TryParse(text, out int parsed))
        {
            value = parsed;
        }
    }

    private static string 构建图片段落Xml(string relationId, string fileName, long cx, long cy, long docPrId)
    {
        string safeName = Xml转义(fileName);
        return
            "<w:p><w:r><w:drawing>" +
            "<wp:inline distT=\"0\" distB=\"0\" distL=\"0\" distR=\"0\">" +
            $"<wp:extent cx=\"{cx}\" cy=\"{cy}\"/>" +
            $"<wp:docPr id=\"{docPrId}\" name=\"{safeName}\"/>" +
            "<wp:cNvGraphicFramePr><a:graphicFrameLocks noChangeAspect=\"1\"/></wp:cNvGraphicFramePr>" +
            "<a:graphic><a:graphicData uri=\"http://schemas.openxmlformats.org/drawingml/2006/picture\">" +
            "<pic:pic><pic:nvPicPr>" +
            $"<pic:cNvPr id=\"0\" name=\"{safeName}\"/>" +
            "<pic:cNvPicPr/>" +
            "</pic:nvPicPr>" +
            "<pic:blipFill>" +
            $"<a:blip r:embed=\"{relationId}\"/>" +
            "<a:stretch><a:fillRect/></a:stretch>" +
            "</pic:blipFill>" +
            "<pic:spPr>" +
            "<a:xfrm><a:off x=\"0\" y=\"0\"/>" +
            $"<a:ext cx=\"{cx}\" cy=\"{cy}\"/></a:xfrm>" +
            "<a:prstGeom prst=\"rect\"><a:avLst/></a:prstGeom>" +
            "</pic:spPr>" +
            "</pic:pic></a:graphicData></a:graphic>" +
            "</wp:inline></w:drawing></w:r></w:p>";
    }

    private static string 确保Word图片命名空间(string documentXml)
    {
        if (string.IsNullOrEmpty(documentXml))
        {
            return documentXml;
        }

        int docStart = documentXml.IndexOf("<w:document", StringComparison.Ordinal);
        if (docStart < 0)
        {
            return documentXml;
        }
        int docTagEnd = documentXml.IndexOf(">", docStart, StringComparison.Ordinal);
        if (docTagEnd < 0)
        {
            return documentXml;
        }

        string docTag = documentXml.Substring(docStart, docTagEnd - docStart + 1);
        docTag = 补充命名空间(docTag, "r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
        docTag = 补充命名空间(docTag, "wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
        docTag = 补充命名空间(docTag, "a", "http://schemas.openxmlformats.org/drawingml/2006/main");
        docTag = 补充命名空间(docTag, "pic", "http://schemas.openxmlformats.org/drawingml/2006/picture");

        return documentXml.Substring(0, docStart) + docTag + documentXml.Substring(docTagEnd + 1);
    }

    private static string 补充命名空间(string docTag, string prefix, string uri)
    {
        string nsMark = $"xmlns:{prefix}=\"";
        if (docTag.IndexOf(nsMark, StringComparison.Ordinal) >= 0)
        {
            return docTag;
        }

        int closePos = docTag.LastIndexOf(">", StringComparison.Ordinal);
        if (closePos <= 0)
        {
            return docTag;
        }

        return docTag.Insert(closePos, $" {nsMark}{uri}\"");
    }

    private static string 用图片段落替换标签段落(string documentXml, string 标签文本, string 图像段落Xml)
    {
        int textIndex = documentXml.IndexOf(标签文本, StringComparison.Ordinal);
        if (textIndex < 0)
        {
            return documentXml;
        }

        int paragraphStart = documentXml.LastIndexOf("<w:p", textIndex, StringComparison.Ordinal);
        int paragraphEnd = documentXml.IndexOf("</w:p>", textIndex, StringComparison.Ordinal);
        if (paragraphStart < 0 || paragraphEnd < 0)
        {
            return documentXml.Replace(标签文本, string.Empty);
        }

        paragraphEnd += "</w:p>".Length;
        return documentXml.Substring(0, paragraphStart) + 图像段落Xml + documentXml.Substring(paragraphEnd);
    }

    private static string 在文末新页追加图片(string documentXml, string 图像段落Xml)
    {
        if (string.IsNullOrEmpty(documentXml) || string.IsNullOrEmpty(图像段落Xml))
        {
            return documentXml;
        }

        int bodyEnd = documentXml.LastIndexOf("</w:body>", StringComparison.Ordinal);
        if (bodyEnd < 0)
        {
            return documentXml;
        }

        string 分页段落 = "<w:p><w:r><w:br w:type=\"page\"/></w:r></w:p>";
        return documentXml.Substring(0, bodyEnd) + 分页段落 + 图像段落Xml + documentXml.Substring(bodyEnd);
    }

    /// <summary>
    /// 保险处理：确保“评估数据”标题段前分页，避免被上一节内容挤压导致跨页断裂。
    /// </summary>
    private static string 确保评估数据段前分页(string xml)
    {
        if (string.IsNullOrEmpty(xml))
        {
            return xml;
        }

        const string 目标文本 = "评估数据";
        int textIndex = xml.IndexOf(目标文本, StringComparison.Ordinal);
        if (textIndex < 0)
        {
            return xml;
        }

        int paragraphStart = xml.LastIndexOf("<w:p", textIndex, StringComparison.Ordinal);
        int paragraphEnd = xml.IndexOf("</w:p>", textIndex, StringComparison.Ordinal);
        if (paragraphStart < 0 || paragraphEnd < 0)
        {
            return xml;
        }
        paragraphEnd += "</w:p>".Length;

        string paragraphXml = xml.Substring(paragraphStart, paragraphEnd - paragraphStart);
        if (paragraphXml.IndexOf("w:pageBreakBefore", StringComparison.Ordinal) >= 0)
        {
            return xml;
        }

        string newParagraphXml = 插入段前分页到段落(paragraphXml);
        if (newParagraphXml == paragraphXml)
        {
            return xml;
        }

        return xml.Substring(0, paragraphStart) + newParagraphXml + xml.Substring(paragraphEnd);
    }

    private static string 插入段前分页到段落(string paragraphXml)
    {
        if (string.IsNullOrEmpty(paragraphXml))
        {
            return paragraphXml;
        }

        int pPrSelfClosing = paragraphXml.IndexOf("<w:pPr/>", StringComparison.Ordinal);
        if (pPrSelfClosing >= 0)
        {
            return paragraphXml.Replace("<w:pPr/>", "<w:pPr><w:pageBreakBefore/></w:pPr>");
        }

        int pPrStart = paragraphXml.IndexOf("<w:pPr", StringComparison.Ordinal);
        if (pPrStart >= 0)
        {
            int pPrEnd = paragraphXml.IndexOf("</w:pPr>", pPrStart, StringComparison.Ordinal);
            if (pPrEnd >= 0)
            {
                return paragraphXml.Insert(pPrEnd, "<w:pageBreakBefore/>");
            }
        }

        int pTagEnd = paragraphXml.IndexOf(">", StringComparison.Ordinal);
        if (pTagEnd < 0)
        {
            return paragraphXml;
        }

        return paragraphXml.Insert(pTagEnd + 1, "<w:pPr><w:pageBreakBefore/></w:pPr>");
    }

    private static string Xml转义(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        string escaped = SecurityElement.Escape(value) ?? string.Empty;
        escaped = escaped.Replace("\r\n", "\n").Replace("\n", "&#10;");
        return escaped;
    }

    private Dictionary<string, string> 构建Word标签映射(档案 档案)
    {
        // 背景选项组的“已选项集合”（用于勾选框）
        var 方言选中 = 获取选项组已选集合(档案, "方言");
        var 吞咽障碍选中 = 获取选项组已选集合(档案, "吞咽障碍");
        var 瘫痪侧选中 = 获取选项组已选集合(档案, "瘫痪侧", "瘫痪", "瘫 痪 侧");
        var brunnstrom选中 = 获取选项组已选集合(档案, "Brunnstrom分级", "Brunnstrom");
        var 感觉障碍选中 = 获取选项组已选集合(档案, "感觉障碍");
        var 腱反射选中 = 获取选项组已选集合(档案, "腱反射", "腱 反 射");
        var 病理反射选中 = 获取选项组已选集合(档案, "病理反射");
        var 口面失用选中 = 获取选项组已选集合(档案, "口面失用");
        var 四高选中 = 获取选项组已选集合(档案, "四高", "四 高");
        var 心脏病选中 = 获取选项组已选集合(档案, "心脏病", "心 脏 病");
        var 吸烟选中 = 获取选项组已选集合(档案, "烟", "吸烟");
        var 饮酒选中 = 获取选项组已选集合(档案, "酒", "饮酒");

        var map = new Dictionary<string, string>
        {
            ["report_date"] = DateTime.Now.ToString("yyyy-MM-dd"),
            ["report_no"] = 档案?.编号 ?? string.Empty,
            ["patient_name"] = 档案?.姓名 ?? string.Empty,
            ["patient_gender"] = 档案?.性别 ?? string.Empty,
            ["evaluator"] = 档案?.评估人员 ?? string.Empty,
            ["birth_date"] = 档案?.出生日期 ?? string.Empty,
            ["actual_age"] = 档案?.年龄 ?? string.Empty,
            ["phone"] = 档案?.联系方式 ?? string.Empty,
            ["id_number"] = 档案?.身份证 ?? string.Empty,
            ["onset_date"] = 档案?.发病日期 ?? string.Empty,
            ["education"] = 档案?.文化程度 ?? string.Empty,
            ["blood_pressure"] = 档案?.血压 ?? string.Empty,
            ["consciousness"] = string.IsNullOrEmpty(档案?.神志状态)
                ? 提取选项组已选内容(档案, "神志状态")
                : 档案.神志状态,
            ["family_members"] = 档案?.家庭成员 ?? string.Empty,
            ["smoking_years"] = 去掉末尾年(档案?.烟龄),
            ["drinking_years"] = 去掉末尾年(档案?.酒龄),
            ["language_status"] = 档案?.发病前后语言状态 ?? string.Empty,
            ["onset_condition"] = 档案?.发病时状况 ?? string.Empty,
            ["past_history"] = 档案?.既往病史 ?? string.Empty,

            ["dialect"] = 提取选项组已选内容(档案, "方言"),
            ["paralysis_side"] = 提取选项组已选内容(档案, "瘫痪侧", "瘫痪", "瘫 痪 侧"),
            ["brunnstrom_grade"] = 提取选项组已选内容(档案, "Brunnstrom分级", "Brunnstrom"),
            ["adl_score"] = 提取选项组已选内容(档案, "ADL"),
            ["sensory_disorder"] = 提取选项组已选内容(档案, "感觉障碍"),
            ["tendon_reflex"] = 提取选项组已选内容(档案, "腱反射", "腱 反 射"),
            ["pathological_reflex"] = 提取选项组已选内容(档案, "病理反射"),
            ["dysphagia"] = 提取选项组已选内容(档案, "吞咽障碍"),
            ["oral_facial_apraxia"] = 提取选项组已选内容(档案, "口面失用"),
            ["four_high"] = 提取选项组已选内容(档案, "四高", "四 高"),
            ["heart_disease"] = 提取选项组已选内容(档案, "心脏病", "心 脏 病"),

            // ===== 可勾选标签（Word中建议使用 {{tag}}有 / {{tag}}无 这种布局）=====
            // 方言
            ["dialect_yes"] = 勾选符(包含任一(方言选中, "有", "是")),
            ["dialect_no"] = 勾选符(包含任一(方言选中, "无", "否")),
            // 吞咽障碍
            ["dysphagia_yes"] = 勾选符(包含任一(吞咽障碍选中, "有", "是")),
            ["dysphagia_no"] = 勾选符(包含任一(吞咽障碍选中, "无", "否")),
            // 瘫痪侧
            ["paralysis_left"] = 勾选符(包含任一(瘫痪侧选中, "左")),
            ["paralysis_right"] = 勾选符(包含任一(瘫痪侧选中, "右")),
            // Brunnstrom
            ["brunnstrom_hand"] = 勾选符(包含任一(brunnstrom选中, "手")),
            ["brunnstrom_upper"] = 勾选符(包含任一(brunnstrom选中, "上肢")),
            ["brunnstrom_lower"] = 勾选符(包含任一(brunnstrom选中, "下肢")),
            // 感觉障碍
            ["sensory_shallow"] = 勾选符(包含任一(感觉障碍选中, "浅")),
            ["sensory_deep"] = 勾选符(包含任一(感觉障碍选中, "深")),
            ["sensory_left"] = 勾选符(包含任一(感觉障碍选中, "左")),
            ["sensory_right"] = 勾选符(包含任一(感觉障碍选中, "右")),
            ["sensory_upper"] = 勾选符(包含任一(感觉障碍选中, "上肢")),
            ["sensory_lower"] = 勾选符(包含任一(感觉障碍选中, "下肢")),
            // 腱反射
            ["tendon_left_ud"] = 勾选符(包含任一(腱反射选中, "左上下", "左上", "左下")),
            ["tendon_right_ud"] = 勾选符(包含任一(腱反射选中, "右上下", "右上", "右下")),
            ["tendon_hyper"] = 勾选符(包含任一(腱反射选中, "亢")),
            ["tendon_weak"] = 勾选符(包含任一(腱反射选中, "弱")),
            // 病理反射
            ["pathological_left"] = 勾选符(包含任一(病理反射选中, "左")),
            ["pathological_right"] = 勾选符(包含任一(病理反射选中, "右")),
            ["pathological_pos"] = 勾选符(包含任一(病理反射选中, "+", "阳性")),
            ["pathological_neg"] = 勾选符(包含任一(病理反射选中, "-", "阴性")),
            // 口面失用
            ["oral_stick_tongue"] = 勾选符(包含任一(口面失用选中, "伸舌")),
            ["oral_puff_cheek"] = 勾选符(包含任一(口面失用选中, "鼓腮")),
            ["oral_grin"] = 勾选符(包含任一(口面失用选中, "咧嘴")),
            ["oral_tongue_trill"] = 勾选符(包含任一(口面失用选中, "振舌")),
            // 四高
            ["four_high_bp"] = 勾选符(包含任一(四高选中, "血压")),
            ["four_high_bs"] = 勾选符(包含任一(四高选中, "血糖")),
            ["four_high_bl"] = 勾选符(包含任一(四高选中, "血脂")),
            ["four_high_weight"] = 勾选符(包含任一(四高选中, "体重")),
            // 心脏病
            ["heart_disease_yes"] = 勾选符(包含任一(心脏病选中, "有", "是")),
            ["heart_disease_no"] = 勾选符(包含任一(心脏病选中, "无", "否")),
            // 烟酒
            ["smoking_yes"] = 勾选符(包含任一(吸烟选中, "有", "是")),
            ["smoking_no"] = 勾选符(包含任一(吸烟选中, "无", "否")),
            ["drinking_yes"] = 勾选符(包含任一(饮酒选中, "有", "是")),
            ["drinking_no"] = 勾选符(包含任一(饮酒选中, "无", "否")),

            ["info_amount"] = 档案?.谈话记录 != null ? 格式化分数(档案.谈话记录.谈话信息量总分) : string.Empty,
            ["fluency"] = 档案?.谈话记录 != null ? 格式化分数(档案.谈话记录.谈话流利性总分) : string.Empty,
            ["serial_speech"] = 档案?.谈话记录 != null ? 格式化分数(档案.谈话记录.系列语言总分) : string.Empty,
            ["yes_no"] = 档案?.理解记录 != null ? 格式化分数(档案.理解记录.是否题总分) : string.Empty,
            ["auditory_recognition"] = 档案?.理解记录 != null ? 格式化分数(档案.理解记录.听辨认图形总分) : string.Empty,
            ["verbal_commands"] = 档案?.理解记录 != null ? 格式化分数(档案.理解记录.口头指令总分) : string.Empty,
            ["word_repetition"] = 档案?.复述记录 != null ? 格式化分数(档案.复述记录.词复述总分) : string.Empty,
            ["sentence_repetition"] = 档案?.复述记录 != null ? 格式化分数(档案.复述记录.句复述总分) : string.Empty,
            ["word_naming"] = 档案?.命名记录 != null ? 格式化分数(档案.命名记录.词命名总分) : string.Empty,
            ["listing"] = 档案?.命名记录?.列名 ?? string.Empty,
            ["color_recognition"] = 档案?.命名记录 != null ? 格式化分数(档案.命名记录.颜色识别总分) : string.Empty,
            ["color_naming"] = 档案?.命名记录 != null ? 格式化分数(档案.命名记录.颜色命名总分) : string.Empty,
            ["responsive_naming"] = 档案?.命名记录 != null ? 格式化分数(档案.命名记录.反应性命名总分) : string.Empty,
            ["visual_reading"] = 档案?.阅读记录 != null ? 格式化分数(档案.阅读记录.视读总分) : string.Empty,
            ["reading_commands"] = 档案?.阅读记录 != null ? 格式化分数(档案.阅读记录.阅读指令总分) : string.Empty,
            ["word_picture_match_total"] = 档案?.阅读记录 != null
                ? 格式化分数(档案.阅读记录.字画匹配朗读总分 + 档案.阅读记录.字画匹配配画总分)
                : string.Empty,
            ["execute_command_total"] = 档案?.阅读记录 != null
                ? 格式化分数(档案.阅读记录.执行指令朗读总分 + 档案.阅读记录.执行指令配画总分)
                : string.Empty,
            ["cloze"] = 档案?.阅读记录 != null ? 格式化分数(档案.阅读记录.填空总分) : string.Empty,
            ["write_name_address"] = 档案?.书写记录 != null ? 格式化分数(档案.书写记录.写姓名地址总分) : string.Empty,
            ["copy_writing"] = 档案?.书写记录 != null ? 格式化分数(档案.书写记录.抄写总分) : string.Empty,
            ["serial_writing"] = 档案?.书写记录 != null ? 格式化分数(档案.书写记录.系列书写总分) : string.Empty,
            ["dictation"] = 档案?.书写记录 != null ? 格式化分数(档案.书写记录.听写总分) : string.Empty,
            ["picture_writing"] = 档案?.书写记录 != null ? 格式化分数(档案.书写记录.看图写字总分) : string.Empty,
            ["illness_writing"] = 档案?.书写记录 != null ? 格式化分数(档案.书写记录.写病情分数) : string.Empty,
            ["drawing"] = 档案?.结构与空间记录 != null ? 格式化分数(档案.结构与空间记录.照画图总分) : string.Empty,
            ["block_design"] = 档案?.结构与空间记录 != null ? 格式化分数(档案.结构与空间记录.摆方块总分) : string.Empty,
            ["facial_actions"] = 档案?.运用记录 != null ? 格式化分数(档案.运用记录.面部动作总分) : string.Empty,
            ["upper_limb_actions"] = 档案?.运用记录 != null ? 格式化分数(档案.运用记录.上肢动作总分) : string.Empty,
            ["complex_actions"] = 档案?.运用记录 != null ? 格式化分数(档案.运用记录.复杂动作总分) : string.Empty,
            ["calculation"] = 档案?.计算记录 != null ? 格式化分数(档案.计算记录.运算总分) : string.Empty,
            ["handedness_result"] = 档案?.利手记录?.利手评定结果 ?? string.Empty,
            ["summary_evaluation"] = 档案?.综合评价 ?? string.Empty,
            ["guidance_advice"] = 档案?.指导建议 ?? string.Empty
        };

        try
        {
            string 失语症类型 = 失语症评定工具.计算失语症类型(档案);
            map["aphasia_result"] = 失语症评定工具.获取失语症类型全称(失语症类型) ?? string.Empty;
        }
        catch
        {
            map["aphasia_result"] = string.Empty;
        }

        try
        {
            map["bdae_severity"] = BDAE严重程度评定工具.获取严重程度描述(档案.BDAE严重程度等级) ?? string.Empty;
        }
        catch
        {
            map["bdae_severity"] = string.Empty;
        }

        // 短标签别名（用于模板排版更紧凑）
        复制标签别名(map, "dyY", "dialect_yes");
        复制标签别名(map, "dyN", "dialect_no");
        复制标签别名(map, "od", "onset_date");
        复制标签别名(map, "edu", "education");
        复制标签别名(map, "bp", "blood_pressure");
        复制标签别名(map, "cs", "consciousness");
        复制标签别名(map, "adl", "adl_score");
        复制标签别名(map, "fm", "family_members");
        复制标签别名(map, "ls", "language_status");
        复制标签别名(map, "oc", "onset_condition");
        复制标签别名(map, "ph", "past_history");
        复制标签别名(map, "sy", "smoking_years");
        复制标签别名(map, "dy", "drinking_years");
        复制标签别名(map, "dia", "dialect");
        复制标签别名(map, "sw", "dysphagia");
        复制标签别名(map, "ps", "paralysis_side");
        复制标签别名(map, "br", "brunnstrom_grade");
        复制标签别名(map, "sd", "sensory_disorder");
        复制标签别名(map, "tr", "tendon_reflex");
        复制标签别名(map, "pr", "pathological_reflex");
        复制标签别名(map, "ofa", "oral_facial_apraxia");
        复制标签别名(map, "fh", "four_high");
        复制标签别名(map, "hd", "heart_disease");
        复制标签别名(map, "swY", "dysphagia_yes");
        复制标签别名(map, "swN", "dysphagia_no");
        复制标签别名(map, "paL", "paralysis_left");
        复制标签别名(map, "paR", "paralysis_right");
        复制标签别名(map, "brH", "brunnstrom_hand");
        复制标签别名(map, "brU", "brunnstrom_upper");
        复制标签别名(map, "brD", "brunnstrom_lower");
        复制标签别名(map, "seQ", "sensory_shallow");
        复制标签别名(map, "seS", "sensory_deep");
        复制标签别名(map, "seL", "sensory_left");
        复制标签别名(map, "seR", "sensory_right");
        复制标签别名(map, "seU", "sensory_upper");
        复制标签别名(map, "seD", "sensory_lower");
        复制标签别名(map, "teL", "tendon_left_ud");
        复制标签别名(map, "teR", "tendon_right_ud");
        复制标签别名(map, "teH", "tendon_hyper");
        复制标签别名(map, "teW", "tendon_weak");
        复制标签别名(map, "prL", "pathological_left");
        复制标签别名(map, "prR", "pathological_right");
        复制标签别名(map, "prP", "pathological_pos");
        复制标签别名(map, "prN", "pathological_neg");
        复制标签别名(map, "ofS", "oral_stick_tongue");
        复制标签别名(map, "ofP", "oral_puff_cheek");
        复制标签别名(map, "ofG", "oral_grin");
        复制标签别名(map, "ofT", "oral_tongue_trill");
        复制标签别名(map, "fhB", "four_high_bp");
        复制标签别名(map, "fhS", "four_high_bs");
        复制标签别名(map, "fhL", "four_high_bl");
        复制标签别名(map, "fhW", "four_high_weight");
        复制标签别名(map, "hdY", "heart_disease_yes");
        复制标签别名(map, "hdN", "heart_disease_no");
        复制标签别名(map, "smY", "smoking_yes");
        复制标签别名(map, "smN", "smoking_no");
        复制标签别名(map, "drY", "drinking_yes");
        复制标签别名(map, "drN", "drinking_no");

        return map;
    }

    private static void 复制标签别名(Dictionary<string, string> map, string alias, string sourceKey)
    {
        if (map == null || string.IsNullOrEmpty(alias) || string.IsNullOrEmpty(sourceKey))
        {
            return;
        }

        if (!map.ContainsKey(alias) && map.TryGetValue(sourceKey, out string value))
        {
            map[alias] = value ?? string.Empty;
        }
    }

    private static string 格式化分数(float value)
    {
        return Mathf.Abs(value - Mathf.Round(value)) < 0.001f
            ? Mathf.RoundToInt(value).ToString()
            : value.ToString("0.##");
    }

    private static string 去掉末尾年(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        string value = text.Trim();
        if (value.EndsWith("年", StringComparison.Ordinal))
        {
            value = value.Substring(0, value.Length - 1);
        }
        return value;
    }

    private static string 提取选项组已选内容(档案 档案, params string[] 候选键)
    {
        if (档案 == null || 档案.背景信息选项组选择 == null || 档案.背景信息选项组选择.Count == 0)
        {
            return string.Empty;
        }

        foreach (string key in 候选键)
        {
            if (尝试从背景选项组读取(档案.背景信息选项组选择, key, out string value))
            {
                return value;
            }
        }

        return string.Empty;
    }

    private static bool 尝试从背景选项组读取(Dictionary<string, string> optionMap, string key, out string value)
    {
        value = string.Empty;
        if (optionMap == null || string.IsNullOrEmpty(key))
        {
            return false;
        }

        string target = 标准化键(key);
        foreach (var kv in optionMap)
        {
            string current = 标准化键(kv.Key);
            if (current == target || current.Contains(target) || target.Contains(current))
            {
                value = 提取选项文本(kv.Value);
                return true;
            }
        }

        return false;
    }

    private static string 标准化键(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        return text
            .Replace(" ", string.Empty)
            .Replace("　", string.Empty)
            .Replace("_", string.Empty)
            .ToLowerInvariant();
    }

    private static string 提取选项文本(string rawValue)
    {
        if (string.IsNullOrEmpty(rawValue))
        {
            return string.Empty;
        }

        int separatorIndex = rawValue.IndexOf('|');
        string content = separatorIndex >= 0 ? rawValue.Substring(0, separatorIndex) : rawValue;
        return content.Trim();
    }

    private static HashSet<string> 获取选项组已选集合(档案 档案, params string[] 候选键)
    {
        var result = new HashSet<string>();
        if (档案 == null || 档案.背景信息选项组选择 == null || 档案.背景信息选项组选择.Count == 0)
        {
            return result;
        }

        string raw = string.Empty;
        foreach (string key in 候选键)
        {
            if (尝试从背景选项组读取(档案.背景信息选项组选择, key, out string value))
            {
                raw = value;
                break;
            }
        }

        if (string.IsNullOrEmpty(raw))
        {
            return result;
        }

        string normalizedRaw = raw.Replace("，", ",").Replace("、", ",");
        string[] arr = normalizedRaw.Split(',');
        foreach (string item in arr)
        {
            string v = item.Trim();
            if (string.IsNullOrEmpty(v))
            {
                continue;
            }

            if (v.StartsWith("选项", StringComparison.Ordinal))
            {
                v = v.Substring(2).Trim();
            }

            if (!string.IsNullOrEmpty(v))
            {
                result.Add(v);
            }
        }

        return result;
    }

    private static string 勾选符(bool checkedState)
    {
        return checkedState ? "☑" : "☐";
    }

    private static bool 包含任一(HashSet<string> selectedValues, params string[] candidates)
    {
        if (selectedValues == null || selectedValues.Count == 0 || candidates == null || candidates.Length == 0)
        {
            return false;
        }

        foreach (string rawCandidate in candidates)
        {
            string candidate = 标准化键(rawCandidate);
            if (string.IsNullOrEmpty(candidate))
            {
                continue;
            }

            foreach (string val in selectedValues)
            {
                string normalizedVal = 标准化键(val);
                if (normalizedVal == candidate || normalizedVal.Contains(candidate) || candidate.Contains(normalizedVal))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 克隆报告界面用于打印
    /// </summary>
    public async UniTask<GameObject> 克隆报告界面(string fileBaseName)
    {
        var cloneGO = Instantiate(报告Scroller.gameObject, 报告Scroller.transform.parent.parent, false);
        cloneGO.name = 报告Scroller.gameObject.name + "_PrintCopy";
        cloneGO.SetActive(true); // 确保克隆体被激活并可见
        await UniTask.Yield(); // 等一帧让父容器完成布局
        cloneGO.transform.SetAsLastSibling(); // 放到父物体最后

        var cloneScroller = cloneGO.GetComponentInChildren<Scroller>(true);
        if (cloneScroller == null)
        {
            Debug.LogError("[打印管理器] 克隆的 Scroller 获取失败");
            Destroy(cloneGO);
            return null;
        }
        Debug.Log("[打印管理器] 克隆的 Scroller 获取成功" + cloneScroller.gameObject.name);

        // 去除打印时不需要的按钮/控件
        if (打印时隐藏对象 != null)
        {
            foreach (var go in 打印时隐藏对象)
            {
                if (go == null) continue;
                var target = 在克隆中按名查找(cloneGO.transform, go.name);
                if (target != null)
                {
                    // Nova 组件在打印克隆中频繁增删节点会触发 DataStore 异常，这里改为隐藏而非销毁
                    target.gameObject.SetActive(false);
                }
            }
        }

        // 调整顶部Block：X铺满，Y自适应
        var cloneRoot = cloneGO?.GetComponent<UIBlock2D>();
        if (cloneRoot != null)
        {
            cloneRoot.AutoLayout.Offset = 0;
            cloneRoot.AutoSize.Y = AutoSize.Expand;
            cloneRoot.AutoSize.X = AutoSize.Expand;
            cloneRoot.CornerRadius = 0;
            cloneRoot.Position.Y = 0;
            cloneRoot.Position.X = 0;
        }

        // 再次确保克隆排在最后，避免上面操作改变顺序
        cloneGO.transform.SetAsLastSibling();

        return cloneGO;
    }


    /// <summary>
    /// 截图报告内容
    /// </summary>
    private async UniTask<string> 截图报告(GameObject cloneGO, string fileBaseName)
    {
        var cloneScroller = cloneGO.GetComponentInChildren<Scroller>(true);
        if (cloneScroller == null) return null;

        string imgPath = await _screenshotUtil.CaptureScrollViewAsync(cloneScroller, fileBaseName);
        await UniTask.Delay(1000); // 等待截图完成

        if (string.IsNullOrEmpty(imgPath))
        {
            Debug.LogError("[打印管理器] 报告截图失败");
            return null;
        }

        Debug.Log($"[打印管理器] 报告截图完成: {imgPath}");
        return imgPath;
    }

    /// <summary>
    /// 在克隆体上应用溢出检测并调整对应的 UIBlock2D 的 Y 值（避免文本块被分页分割）
    /// </summary>
    public void 应用溢出调整(GameObject cloneRoot)
    {
        if (cloneRoot == null) return;

        // 发病前后语言状态
        // 按顺序检测，若高序项已调整则后续不再检测
        if (ApplyIfOverflow(cloneRoot, 发病前后语言状态_检测对象, 发病前后语言状态_block调整, 溢出阈值_发病前后, 调整Y_发病前后, "发病前后语言状态"))
        {
            return;
        }

        if (ApplyIfOverflow(cloneRoot, 发病时状况_检测对象, 发病时状况_block调整, 溢出阈值_发病时, 调整Y_发病时, "发病时状况"))
        {
            return;
        }

        ApplyIfOverflow(cloneRoot, 既往病史_检测对象, 既往病史_block调整, 溢出阈值_既往, 调整Y_既往, "既往病史");
    }

    private bool ApplyIfOverflow(GameObject cloneRoot, Transform 原始检测对象, UIBlock2D 原始调整Block, float threshold, float setY, string 名称)
    {
        if (原始检测对象 == null || 原始调整Block == null) return false;

        // 在克隆中按原始对象名称查找对应对象
        var target = 查找栏目对象(cloneRoot, 原始检测对象.gameObject);
        if (target == null) return false;

        // 优先尝试使用 Nova UI 的 UIBlock2D.Position.Y 作为位置判断
        float y;
        var blockOnTarget = target.GetComponent<UIBlock2D>() ?? target.GetComponentInParent<UIBlock2D>();
        if (blockOnTarget != null)
        {
            // 使用 Raw 数值（单位像素或长度单位）作为判断依据
            y = blockOnTarget.Position.Raw.y;
            Debug.Log($"[打印管理器] {名称} UIBlock2D.Position.Raw.y: {y}");
        }
        else
        {
            // 回退到 transform.localPosition.y
            y = target.transform.localPosition.y;
        }

        // 如果Y超过阈值，则在克隆中找到对应的 Block 并设置其 Position.Y
        if (y > threshold)
        {
            var blockGO = 查找栏目对象(cloneRoot, 原始调整Block.gameObject);
            if (blockGO != null)
            {
                var uiBlock = blockGO.GetComponent<UIBlock2D>();
                if (uiBlock != null)
                {
                    // 如果已经调整过则跳过
                    if (_已调整块.Contains(uiBlock))
                    {
                        Debug.Log($"[打印管理器] {名称} 块已调整，跳过重复设置: {blockGO.name}");
                        return false;
                    }

                    // 按照要求：设置其 Size 的 Y 值为 (阈值 - 当前Y)
                    float sizeY = threshold - 原始调整Block.Position.Raw.y;
                    uiBlock.Size.Y = Length.FixedValue(sizeY);
                    _已调整块.Add(uiBlock);
                    Debug.Log($"[打印管理器] {名称} 溢出：{y:F0} > {threshold:F0}, 已设置 {blockGO.name}.Size.Y = {sizeY:F0}");
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 清理克隆对象
    /// </summary>
    private void 清理克隆对象(GameObject cloneGO)
    {
        if (cloneGO != null)
        {
            // 先隐藏再延迟销毁，避免与同帧 Nova 布局更新冲突
            cloneGO.SetActive(false);
            Destroy(cloneGO, 0.1f);
            Debug.Log("[打印管理器] 清理克隆对象完成");
        }
    }

    /// <summary>
    /// 在克隆的层级中按名称查找节点
    /// </summary>
    private Transform 在克隆中按名查找(Transform root, string name)
    {
        var all = root.GetComponentsInChildren<Transform>(true);
        foreach (var t in all)
        {
            if (t.name == name) return t;
        }
        return null;
    }
}
