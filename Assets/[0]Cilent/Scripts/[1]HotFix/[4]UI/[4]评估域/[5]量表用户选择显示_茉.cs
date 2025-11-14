using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;
using QFramework;
using Cysharp.Threading.Tasks;
public class 量表用户选择显示_茉 : 界面基础_韩
{
    // Start is called before the first frame update
    [SerializeField] Button 确认按钮;
    [SerializeField] Button 取消按钮;
    [SerializeField] 条目组性_茉 条组;
    [SerializeField] Toggle 全选勾选;

    bool 全选监听;
    bool 是否为第一次加载 = true;
    void Start()
    {
        if (确认按钮 != null)
        {
            确认按钮.OnClicked.AddListener(确认);
        }
        if (取消按钮 != null)
        {
            取消按钮.OnClicked.AddListener(取消);
        }

        全选勾选.OnToggled.AddListener(全选);
        是否为第一次加载 = false;

    }

    protected override void OnShow()
    {
        base.OnShow();
        Debug.Log("OnShow");
        if (!是否为第一次加载)
        {
            Debug.Log("不是第一次加载");
            条组.勾组.ForEach(勾 =>
            {
                勾.ToggledOn = false;
            });
            条组.CurrentSelectedEntries.Clear();
            //        条组.沐浴();
        }
    }
    private void 全选(bool 是否选中)
    {
        if (是否选中)
        {
            条组.勾组.ForEach(勾 =>
                  {
                      勾.ToggledOn = 是否选中;
                  });
            全选监听 = true;
        }
        else
        {
            if (全选监听)
            {
                全选监听 = false;
                条组.勾组.ForEach(勾 =>
                  {
                      勾.ToggledOn = 是否选中;
                  });
            }
        }

    }
    async void 确认()
    {
        // FindObjectOfType<界面管理器_韩>().显示界面("量表待评测显示");
        var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 界面管理器 = FindObjectOfType<界面管理器_韩>();
        var ins = Instantiate(告示, 界面管理器.transform);
        var 户择 = await ins.GetComponent<告示_茉>().等待确认显示("确认添加", "确定要添加为待测试人员吗？");
        if (户择)
        {
            int 选择数量 = 条组.CurrentSelectedEntries.Count;
            foreach (var 条目 in 条组.CurrentSelectedEntries)
            {
                var 条目下标 = 条目.GetComponent<IentryDis>().索引下标;

                // 根据用户下标创建档案（该方法内部会处理重复检查）
                await 添加档案(条目下标);
            }

            条组.CurrentSelectedEntries.Clear();
            await UniTask.Delay(100);
            条组.沐浴();

            // 显示操作结果
            Debug.Log($"[确认] 处理完成，共选择 {选择数量} 个用户");

            // 切换到量表待评测显示界面
            FindObjectOfType<界面管理器_韩>().显示界面("量表待评测显示");
        }
        Destroy(ins);
        // Debug.Log("确认");
    }

    async UniTask 添加档案(int index)
    {
        // 获取用户录数据
        var 用户录 = this.GetModel<用户录>();
        var 用户列表 = 用户录.GetDataList();

        // 检查索引是否有效
        if (index < 0 || index >= 用户列表.Count)
        {
            Debug.LogError($"[添加档案] 用户索引 {index} 超出范围，用户总数: {用户列表.Count}");
            return;
        }

        // 根据索引获取用户信息
        var 当前用户 = 用户列表[index];

        // 获取档案录数据并检查是否已存在相同编号的档案
        var 档案录 = this.GetModel<档案录>();
        var 档案列表 = 档案录.GetDataList();

        // 查找是否已存在相同编号的档案
        var 已存在档案 = 档案列表.Find(档案 => 档案.编号 == 当前用户.编号);

        if (已存在档案 != null && 已存在档案.评估状态 != 评估状态.评估完成)
        {
            // 如果存在且状态不为完成，显示告示提示
            var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
            var 界面管理器 = FindObjectOfType<界面管理器_韩>();
            var ins = Instantiate(告示, 界面管理器.transform);
            await ins.GetComponent<告示_茉>().等待确认显示("重复添加", $"用户 {当前用户.名}已在待测名单中");
            Destroy(ins);
            return;
        }
        var 评人录 = this.GetModel<评人录>();
        string 当前评人 = "管理员";
        if (评人录.登陆评人 != null)
        {
            当前评人 = 评人录.登陆评人.名;
        }

        // 创建新的档案对象
        var 新档案 = new 档案
        {
            评估状态 = 评估状态.未评估,
            姓名 = 当前用户.名,
            性别 = 当前用户.性别,
            出生日期 = 当前用户.诞辰,
            编号 = 当前用户.编号,
            添加时间 = System.DateTime.Now.ToString("yyyy-MM-dd"),
            年龄 = 计算年龄(当前用户.诞辰),
            评估人员 = 当前评人, // 可根据需要设置当前评估人员
            评估时间 = "",
            
            // 复制背景信息
            身份证 = 当前用户.身份证,
            民族 = 当前用户.民族,
            监护人 = 当前用户.监护人,
            关系 = 当前用户.关系,
            联系方式 = 当前用户.联系方式,
            居住地址 = 当前用户.居住地址,
            发病日期 = 当前用户.发病日期,
            文化程度 = 当前用户.文化程度,
            血压 = 当前用户.血压,
            神志状态 = 当前用户.神志状态,
            家庭成员 = 当前用户.家庭成员,
            发病前后语言状态 = 当前用户.发病前后语言状态,
            发病时状况 = 当前用户.发病时状况,
            既往病史 = 当前用户.既往病史,
            背景信息选项组选择 = 当前用户.背景信息选项组选择 != null
                ? new Dictionary<string, string>(当前用户.背景信息选项组选择)
                : new Dictionary<string, string>(),

            // 初始化各项记录为空，后续评估时填充
            谈话记录 = new 谈话记录(),
            理解记录 = new 理解记录(),
            复述记录 = new 复述记录(),
            命名记录 = new 命名记录(),
            阅读记录 = new 阅读记录(),
            计算记录 = new 计算记录(),
            书写记录 = new 书写记录(),
            结构与空间记录 = new 结构与空间记录(),
            视觉记录 = new 视觉记录(),
            运用记录 = new 运用记录(),
            利手记录 = new 利手记录(),
            是否选择BDAE等级 = false
        };

        // 使用命令将档案添加到档案录
        this.SendCommand(new 增记于录(新档案, "档案录"));

        Debug.Log($"[添加档案] 成功为用户 {当前用户.名}(编号:{当前用户.编号}) 创建档案");
    }

    /// <summary>
    /// 根据出生日期计算年龄
    /// </summary>
    /// <param name="出生日期">格式: yyyy-MM-dd</param>
    /// <returns>年龄字符串</returns>
    private string 计算年龄(string 出生日期)
    {
        try
        {
            if (string.IsNullOrEmpty(出生日期))
                return "未知";

            if (System.DateTime.TryParse(出生日期, out System.DateTime 出生))
            {
                var 今天 = System.DateTime.Today;

                int 年 = 今天.Year - 出生.Year;
                int 月 = 今天.Month - 出生.Month;
                int 天 = 今天.Day - 出生.Day;

                // 如果天数为负数，向月份借位
                if (天 < 0)
                {
                    天 += 30;  // 每月按30天计算
                    月--;
                }

                // 如果月数为负数，向年份借位
                if (月 < 0)
                {
                    月 += 12;
                    年--;
                }

                // 构建年龄字符串
                string 年龄字符串 = "";

                if (年 > 0)
                {
                    年龄字符串 += 年 + "岁";
                }

                if (月 > 0)
                {
                    年龄字符串 += 月 + "月";
                }

                if (天 > 0)
                {
                    年龄字符串 += 天 + "天";
                }

                // 如果是出生当天
                if (string.IsNullOrEmpty(年龄字符串))
                {
                    年龄字符串 = "0天";
                }

                return 年龄字符串;
            }
            return "未知";
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[计算年龄] 解析出生日期失败: {出生日期}, 错误: {ex.Message}");
            return "未知";
        }
    }
    async void 取消()
    {
        var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 界面管理器 = FindObjectOfType<界面管理器_韩>();
        var ins = Instantiate(告示, 界面管理器.transform);
        var 户择 = await ins.GetComponent<告示_茉>().等待确认显示("提示", "确认要取消吗？");
        if (户择)
        {
            FindObjectOfType<界面管理器_韩>().显示界面("量表待评测显示");
            //     Debug.Log("取消");
        }
        Destroy(ins);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
