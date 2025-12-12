using UnityEngine;
using Nova;
using NovaSamples.UIControls;
/// <summary>
/// 量表书写栏 - 使用栏目组统一管理
/// 包含6个维度：写姓名地址、抄写、系列书写、听写、看图写字（表分择性）、写病情（直接输入0-5分）
/// </summary>
public class 量表书写栏_茉 : 量表评估基类_茉
{
    [Header("写病情输入")]
    [Tooltip("写病情分数输入框（0-5分）")]
    [SerializeField] private TextField 写病情输入框;
    
    [Header("按钮开启对象")]
    [Tooltip("Nova UI Button，点击后启用目标对象")]
    [SerializeField] private Button 点击开启按钮;
    [Tooltip("被启用的目标对象")]
    [SerializeField] private GameObject 需要启用的对象;
    
    private int 写病情当前分数 = 0;
    
    protected override string 获取记录字段名称() => "书写记录";

    protected override bool 检查是否全部完成()
    {
        // 检查所有栏目组（表分择性）是否完成
        if (栏目组性群 == null) return false;
        bool 栏目组全部完成 = 栏目组性群.TrueForAll(组 => 组?.是否全部已选择 ?? false);
        
        // 检查写病情分数是否已输入（0-5分都是有效的）
        bool 写病情已输入 = 写病情当前分数 >= 0 && 写病情当前分数 <= 5;
        
        return 栏目组全部完成 && 写病情已输入;
    }

    protected override string 获取未完成提示信息()
    {
        if (栏目组性群 != null && !栏目组性群.TrueForAll(组 => 组?.是否全部已选择 ?? false))
        {
            return "请完成所有表分择性评估项";
        }
        
        if (写病情当前分数 < 0 || 写病情当前分数 > 5)
        {
            return "请输入写病情分数（0-5分）";
        }
        
        return "请完成全部书写评估后保存";
    }

    protected override void OnStartExtra()
    {
        // 初始化写病情输入框
        if (写病情输入框 != null)
        {
            写病情输入框.Text = "";
            写病情当前分数 = 0;
            
            // 监听输入框文本变化事件
            写病情输入框.OnTextChanged += 处理输入框文本变化;
        }

        if (点击开启按钮 != null)
        {
            点击开启按钮.OnClicked.AddListener(当按钮被点击时);
        }
    }
    
    protected override void OnDestroyExtra()
    {
        // 取消监听输入框事件
        if (写病情输入框 != null)
        {
            写病情输入框.OnTextChanged -= 处理输入框文本变化;
        }

        if (点击开启按钮 != null)
        {
            点击开启按钮.OnClicked.RemoveListener(当按钮被点击时);
        }
    }
    
    /// <summary>
    /// 处理输入框文本变化
    /// </summary>
    private void 处理输入框文本变化()
    {
        if (写病情输入框 == null) return;
        
        // 从输入框获取文本并尝试解析为分数
        从文本设置写病情分数(写病情输入框.Text);
    }

    protected override void 还原其他数据(object 记录)
    {
        var 书写记录 = 记录 as 书写记录;
        Debug.Log($"[量表书写栏] 还原其他数据: {书写记录.写病情分数}");
        if (书写记录 == null) return;

        // 还原写病情分数
        写病情当前分数 = 书写记录.写病情分数;
        if (写病情输入框 != null)
        {
            if(写病情当前分数 <= 0)
            {
                写病情输入框.Text = "";
            }
            else
            {
                写病情输入框.Text = 写病情当前分数.ToString();
            }
        }
        
        Debug.Log($"[量表书写栏] 还原写病情分数: {写病情当前分数}");
    }

    protected override void 保存额外数据(object 记录)
    {
        var 书写记录 = 记录 as 书写记录;
        if (书写记录 == null || 栏目组性群 == null) return;

        // 保存各维度总分（表分择性）
        foreach (var 栏目组 in 栏目组性群)
        {
            if (栏目组 == null) continue;
            
            switch (栏目组.维度名称)
            {
                case "写姓名地址":
                    书写记录.写姓名地址总分 = 栏目组.当前总分;
                    Debug.Log($"[量表书写栏] 保存写姓名地址总分: {栏目组.当前总分}");
                    break;
                case "抄写":
                    书写记录.抄写总分 = 栏目组.当前总分;
                    Debug.Log($"[量表书写栏] 保存抄写总分: {栏目组.当前总分}");
                    break;
                case "系列书写":
                    书写记录.系列书写总分 = 栏目组.当前总分;
                    Debug.Log($"[量表书写栏] 保存系列书写总分: {栏目组.当前总分}");
                    break;
                case "听写":
                    书写记录.听写总分 = 栏目组.当前总分;
                    Debug.Log($"[量表书写栏] 保存听写总分: {栏目组.当前总分}");
                    break;
                case "看图写字":
                    书写记录.看图写字总分 = 栏目组.当前总分;
                    Debug.Log($"[量表书写栏] 保存看图写字总分: {栏目组.当前总分}");
                    break;
            }
        }
        
        // 保存写病情分数
        书写记录.写病情分数 = 写病情当前分数;
        Debug.Log($"[量表书写栏] 保存写病情分数: {写病情当前分数}");
    }
    
    /// <summary>
    /// 公共方法：设置写病情分数（供UI调用）
    /// </summary>
    /// <param name="分数">0-5分</param>
    public void 设置写病情分数(int 分数)
    {
        if (分数 < 0 || 分数 > 5)
        {
            Debug.LogWarning($"[量表书写栏] 写病情分数超出范围: {分数}，有效范围是0-5");
            return;
        }
        
        写病情当前分数 = 分数;
        if (写病情输入框 != null)
        {
            写病情输入框.Text = 分数.ToString();
        }
        
        Debug.Log($"[量表书写栏] 写病情分数已设置为: {分数}");
    }
    
    /// <summary>
    /// 公共方法：从输入框文本设置写病情分数
    /// </summary>
    /// <param name="分数文本">分数字符串</param>
    public void 从文本设置写病情分数(string 分数文本)
    {
        if (int.TryParse(分数文本, out int 分数))
        {
            设置写病情分数(分数);
        }
        else
        {
            Debug.LogWarning($"[量表书写栏] 无法解析写病情分数: {分数文本}");
        }
    }
    
    /// <summary>
    /// 获取当前写病情分数
    /// </summary>
    public int 获取写病情分数()
    {
        return 写病情当前分数;
    }

    /// <summary>
    /// 点击按钮时启用目标对象
    /// </summary>
    private void 当按钮被点击时()
    {
        if (需要启用的对象 == null) return;

        if (!需要启用的对象.activeSelf)
        {
            需要启用的对象.SetActive(true);
        }
    }
}
