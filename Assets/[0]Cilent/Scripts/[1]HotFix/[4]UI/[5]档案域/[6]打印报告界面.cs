using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Nova;
using NovaSamples.UIControls;
using Cysharp.Threading.Tasks;

public class 打印报告界面_茉 : 界面基础_韩
{
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
    [SerializeField] private UIBlock2D 打印根Block; // 顶部需要铺满X、Y自适应的Block
    [SerializeField] private List<GameObject> 打印时隐藏对象; // 打印时需要去除的按钮/控件

    [Header("评估结果")]
    [SerializeField] private TextBlock 利手评定结果;
    [SerializeField] private TextBlock 失语症评定;
    [SerializeField] private TextBlock BDAE严重程度;

    private 档案 当前显示档案;

    [HideInInspector] public bool 是否为打印;
    [HideInInspector] public bool 显示量表报告;
    [HideInInspector] public bool 显示量表表格;

    [SerializeField] private Button 返回按钮;
    [SerializeField] private Button 返回按钮1;
    [SerializeField] private Button 确认打印;
    void Awake()
    {
        显示信息 += 显示档案详情;
        返回按钮.OnClicked.AddListener(() =>
     {
         处理返回按钮点击().Forget();

     });
        返回按钮1.OnClicked.AddListener(() =>
        {
            处理返回按钮点击().Forget();
        });
        确认打印.OnClicked.AddListener(() =>
        {
            处理确认打印按钮点击().Forget();
        });
    }
    async UniTaskVoid 处理确认打印按钮点击()
    {
        var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 界面管理器 = FindObjectOfType<界面管理器_韩>();
        var ins = Instantiate(告示, 界面管理器.transform);
        var 户择 = await ins.GetComponent<告示_茉>().等待确认显示("提示", "即将打印报告，请确保打印机连接正常");
        Destroy(ins);
        if (!户择)
        {
            return;
        }

        var screenshotUtil = this.GetUtility<IScreenshotPDFUtility>();
        if (screenshotUtil == null)
        {
            Debug.LogError("[打印报告] 未找到 ScreenshotPDFUtility");
            return;
        }

        if (报告Scroller == null)
        {
            Debug.LogError("[打印报告] 报告Scroller 未绑定，无法截图");
            return;
        }

        string fileBaseName = $"评估报告_{当前显示档案?.编号 ?? "未知"}_{System.DateTime.Now:yyyyMMdd_HHmmss}";

        // 1) 克隆用于打印的 Scroller
        var cloneGO = Instantiate(报告Scroller.gameObject, 报告Scroller.transform.parent.parent, false);
        cloneGO.name = 报告Scroller.gameObject.name + "_PrintCopy";
        cloneGO.SetActive(true); // 确保克隆体被激活并可见
        await UniTask.Yield(); // 等一帧让父容器完成布局
        cloneGO.transform.SetAsLastSibling(); // 放到父物体最后
        var cloneScroller = cloneGO.GetComponentInChildren<Scroller>(true);
        if (cloneScroller == null)
        {
            Debug.LogError("[打印报告] 克隆的 Scroller 获取失败");
            Destroy(cloneGO);
            return;
        }
        Debug.Log("克隆的 Scroller 获取成功" + cloneScroller.gameObject.name);

        // 2) 去除打印时不需要的按钮/控件
        if (打印时隐藏对象 != null)
        {
            foreach (var go in 打印时隐藏对象)
            {
                if (go == null) continue;
                var target = 在克隆中按名查找(cloneGO.transform, go.name);
                if (target != null)
                {
                    Destroy(target.gameObject);
                }
            }
        }
        var cloneRoot = cloneGO?.GetComponent<UIBlock2D>();
        // 3) 调整顶部Block：X铺满，Y自适应
        if (cloneRoot != null)
        {
            cloneRoot.AutoLayout.Offset = 0;
            cloneRoot.AutoSize.Y = AutoSize.Expand;
            cloneRoot.AutoSize.X = AutoSize.Expand;
            cloneRoot.CornerRadius = 0;
            cloneRoot.Position.Y=0;
            cloneRoot.Position.X=0;
        }
        // 再次确保克隆排在最后，避免上面操作改变顺序
        cloneGO.transform.SetAsLastSibling();

        // 4) 用克隆的 Scroller 截图打印
        string imgPath = await screenshotUtil.CaptureScrollViewAsync(cloneScroller, fileBaseName);
        await UniTask.Delay(2000);
        if (string.IsNullOrEmpty(imgPath))
        {
            Debug.LogError("[打印报告] 报告截图失败");
            Destroy(cloneGO);
            return;
        }
        Debug.Log($"[打印报告] 报告截图完成: {imgPath}");

        string htmlPath = screenshotUtil.ConvertImageToHTML(imgPath, "评估报告");

        if (string.IsNullOrEmpty(htmlPath))
        {
            Debug.LogError("[打印报告] 生成 HTML 失败");
            Destroy(cloneGO);
            return;
        }
        Debug.Log($"[打印报告] HTML 生成完成: {htmlPath}");

        if (!screenshotUtil.PrintFile(htmlPath))
        {
            Debug.LogWarning("[打印报告] 发送打印失败，已尝试打开文件请手动打印");
        }

        // 5) 清理克隆
        Destroy(cloneGO);
    }
    async UniTaskVoid 处理返回按钮点击()
    {
        var 界面管理器 = FindObjectOfType<界面管理器_韩>();
        FindObjectOfType<界面管理器_韩>().显示界面("档案管理报告界面");
        await UniTask.WaitUntil(() => 界面管理器._currentPanel != null && 界面管理器._currentPanel.name.Contains("档案管理报告界面"));
        界面管理器._currentPanel.显示信息?.Invoke(当前显示档案);
    }
    void 显示档案详情(I记贞 记)
    {
        当前显示档案 = 记 as 档案;

        if (当前显示档案 == null)
        {
            Debug.LogWarning("[打印报告] 传入的档案数据为空");
            return;
        }

        Debug.Log($"[打印报告] 显示档案: {当前显示档案.姓名}(编号:{当前显示档案.编号})");

        // 根据勾选状态显示对应界面
        控制界面显示();

        显示量表评估分数(当前显示档案);

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
    /// 更新量表表格可视化
    /// </summary>
    void 更新量表表格可视化(档案 档案)
    {
        Dictionary<string, int> 分数字典 = new Dictionary<string, int>();

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

    void OnDestroy()
    {
        显示信息 -= 显示档案详情;
    }

    /// <summary>
    /// 在克隆的层级中按名称查找节点
    /// </summary>
    Transform 在克隆中按名查找(Transform root, string name)
    {
        var all = root.GetComponentsInChildren<Transform>(true);
        foreach (var t in all)
        {
            if (t.name == name) return t;
        }
        return null;
    }
}
