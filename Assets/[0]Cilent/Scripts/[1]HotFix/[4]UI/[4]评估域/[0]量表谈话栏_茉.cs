using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NovaSamples.UIControls;
using QFramework;
using UnityEngine;

public class 量表谈话栏_茉 : 界面基础_韩
{
    // Start is called before the first frame update
    [SerializeField] Button 返回按钮;
    List<选项组性_茉> 选项组性群;
    [SerializeField] Button 提交按钮;
    [SerializeField] Button 取消按钮;
    void Start()
    {
        // 获取所有选项组
        选项组性群 = FindObjectsOfType<选项组性_茉>().ToList();
        
        返回按钮.OnClicked.AddListener(() =>
        {
            FindObjectOfType<界面管理器_韩>().显示界面("量表评估显示");
        });
        提交按钮.OnClicked.AddListener(() =>
        {
            提交按钮点击().Forget();
        });
        取消按钮.OnClicked.AddListener(() =>
        {
            取消按钮点击().Forget();
        });
    }

    async UniTaskVoid 提交按钮点击()
    {
        // 检查所有选项组是否都已选择
        var 未选择的选项组 = new List<string>();
        
        foreach (var 选项组 in 选项组性群)
        {
            if (选项组.当前选中选项 == null)
            {
                string 组名 = string.IsNullOrEmpty(选项组.选项组名称) ? 选项组.name : 选项组.选项组名称;
                未选择的选项组.Add(组名);
            }
        }
        
        // 如果有未选择的选项组，显示提示
        if (未选择的选项组.Count > 0)
        {
            var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
            var 告示实例 = Instantiate(告示, FindObjectOfType<界面管理器_韩>().transform);
            var 告示组件 = 告示实例.GetComponent<告示_茉>();
            
            string 提示信息 = "请完成全部选项组后保存";
            await 告示组件.等待确认显示("提示", 提示信息);
            
            Destroy(告示实例.gameObject, 0.1f);
            return;
        }
        
        // 所有选项组都已选择，继续提交流程
        var 确认告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 确认告示实例 = Instantiate(确认告示, FindObjectOfType<界面管理器_韩>().transform);
        var 确认告示组件 = 确认告示实例.GetComponent<告示_茉>();
        var 确认 = await 确认告示组件.等待确认显示("提示", "确认保存并返回选择界面");
        if (确认)
        {
            // 保存逻辑
          //  保存选择结果();
            FindObjectOfType<界面管理器_韩>().显示界面("量表评估显示");
        }
        Destroy(确认告示实例.gameObject, 0.1f);
    }
    async UniTaskVoid 取消按钮点击()
    {
        var 告示 = await this.GetUtility<YooAssetPfbPoolUtility>().LoadPfb("告示");
        var 告示实例 = Instantiate(告示, FindObjectOfType<界面管理器_韩>().transform);
        var 告示组件 = 告示实例.GetComponent<告示_茉>();
        var 确认 = await 告示组件.等待确认显示("提示", "确认取消并返回选择界面");
        if (确认)
        {
            FindObjectOfType<界面管理器_韩>().显示界面("量表评估显示");
        }
        Destroy(告示实例.gameObject, 0.1f);
    }
    
    /// <summary>
    /// 保存所有选项组的选择结果
    /// </summary>
    private void 保存选择结果()
    {
        foreach (var 选项组 in 选项组性群)
        {
            if (选项组.当前选中选项 != null)
            {
                string 组名 = string.IsNullOrEmpty(选项组.选项组名称) ? 选项组.name : 选项组.选项组名称;
                string 选中项名称 = 选项组.当前选中名称;
                int 选中索引 = 选项组.当前选中索引;
                
                Debug.Log($"选项组 [{组名}] 选择了: {选中项名称} (索引: {选中索引})");
                
                // 这里可以添加具体的保存逻辑，比如保存到文件或数据库
                // 例如: SaveToFile(组名, 选中项名称, 选中索引);
            }
        }
    }
}
