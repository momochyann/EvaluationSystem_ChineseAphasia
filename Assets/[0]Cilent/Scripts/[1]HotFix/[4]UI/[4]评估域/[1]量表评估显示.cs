using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovaSamples.UIControls;
using QFramework;
using System.Reflection;
using System;

public class 量表评估显示 : 界面基础_韩
{
    // Start is called before the first frame update
    [SerializeField] List<Button> 评估按钮;
    private 档案 当前档案;
    [SerializeField] Button 返回按钮;
    [SerializeField] Button 返回按钮1;
    [SerializeField] GameObject 提示界面;
    [SerializeField] Button 下一步按钮;

    void Start()
    {
        foreach (var 评估按钮 in 评估按钮)
        {
            
            评估按钮.OnClicked.AddListener(() =>
            {
                进入评估界面(评估按钮);
            });
        }
        if (返回按钮 != null)
        {
            返回按钮.OnClicked.AddListener(() =>
            {
                FindObjectOfType<界面管理器_韩>().显示界面("量表待评测显示");
            });
        }
        if (返回按钮1 != null)
        {
            返回按钮1.OnClicked.AddListener(() =>
            {
                FindObjectOfType<界面管理器_韩>().显示界面("量表待评测显示");
            });
        }

        // 为提示界面添加点击关闭功能
        if (提示界面 != null)
        {
            提示界面.transform.Find("背景/确认按钮").GetComponent<Button>().OnClicked.AddListener(() =>
            {
                提示界面.gameObject.SetActive(false);
                if (当前档案.评估状态 == 评估状态.未评估)
                    当前档案.评估状态 = 评估状态.未完成;
            });
            提示界面.transform.Find("背景/取消按钮").GetComponent<Button>().OnClicked.AddListener(() =>
            {
                提示界面.gameObject.SetActive(false);
                FindObjectOfType<界面管理器_韩>().显示界面("量表待评测显示");
            });
        }

        // 初始化下一步按钮（如果存在）
        if (下一步按钮 != null)
        {
            下一步按钮.gameObject.SetActive(false);
            // 可以在这里添加下一步按钮的点击事件
            下一步按钮.OnClicked.AddListener(() =>
            {
                // TODO: 添加下一步的逻辑
                FindObjectOfType<界面管理器_韩>().显示界面("量表评估模板选择");
            });
        }
        更新按钮显示状态();
    }

    protected override void OnShow()
    {
        base.OnShow();
        // 获取当前档案
        var 档案录 = this.GetModel<档案录>();
        当前档案 = 档案录.当前档案;

        // 更新所有按钮的显示状态
        更新按钮显示状态();
        当前档案 = 档案录.当前档案;
        if (当前档案.评估状态 == 评估状态.未评估)
        {
            // 显示提示界面
            if (提示界面 != null)
            {
                提示界面.gameObject.SetActive(true);
            }
        }
        else
        {
            if (提示界面 != null)
            {
                提示界面.gameObject.SetActive(false);
            }
        }
    }

    protected override void OnHide()
    {
        base.OnHide();
        // 重置所有按钮的显示状态（隐藏所有标记）
        重置按钮显示状态();
    }

    /// <summary>
    /// 更新所有评估按钮的显示状态
    /// </summary>
    private void 更新按钮显示状态()
    {
        if (当前档案 == null) return;

        foreach (var 按钮 in 评估按钮)
        {
            更新单个按钮状态(按钮);
        }

        // 检查是否所有评估都已完成，更新下一步按钮的显示状态
        更新下一步按钮状态();
    }

    /// <summary>
    /// 重置所有评估按钮的显示状态（隐藏所有标记）
    /// </summary>
    private void 重置按钮显示状态()
    {
        if (评估按钮 == null) return;

        foreach (var 按钮 in 评估按钮)
        {
            重置单个按钮状态(按钮);
        }

        // 隐藏下一步按钮
        if (下一步按钮 != null)
        {
            下一步按钮.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 重置单个按钮的显示状态（隐藏所有标记）
    /// </summary>
    /// <param name="按钮">要重置的按钮</param>
    private void 重置单个按钮状态(Button 按钮)
    {
        // 获取按钮的子物体标记（在内框的子物体中）
        Transform 内框 = 按钮.transform.Find("内框");
        if (内框 == null)
        {
            return;
        }

        Transform 完成标记 = 内框.Find("记录完成标记");
        Transform 待完成标记 = 内框.Find("记录待完成标记");

        // 隐藏所有标记，回到初始状态
        if (完成标记 != null)
        {
            完成标记.gameObject.SetActive(false);
        }
        if (待完成标记 != null)
        {
            待完成标记.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 更新单个按钮的显示状态
    /// </summary>
    /// <param name="按钮">要更新的按钮</param>
    private void 更新单个按钮状态(Button 按钮)
    {
        string 按钮名称 = 按钮.gameObject.name;

        // 获取按钮的子物体标记（在内框的子物体中）
        Transform 内框 = 按钮.transform.Find("内框");
        if (内框 == null)
        {
            Debug.LogWarning($"按钮 {按钮名称} 缺少内框子物体");
            return;
        }

        Transform 完成标记 = 内框.Find("记录完成标记");
        Transform 待完成标记 = 内框.Find("记录待完成标记");

        if (完成标记 == null || 待完成标记 == null)
        {
            Debug.LogWarning($"按钮 {按钮名称} 缺少完成标记或待完成标记子物体");
            return;
        }

        // 根据按钮名称获取对应的记录状态
        var 记录状态 = 获取记录完成状态(按钮名称);

        // 根据状态显示对应的标记
        if (记录状态 == 评估状态.评估完成)
        {
            完成标记.gameObject.SetActive(true);
            待完成标记.gameObject.SetActive(false);
        }
        else if (记录状态 == 评估状态.未完成)
        {
            完成标记.gameObject.SetActive(false);
            待完成标记.gameObject.SetActive(true);
        }
        else
        {
            完成标记.gameObject.SetActive(false);
            待完成标记.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 根据按钮名称获取对应记录的完成状态
    /// </summary>
    /// <param name="按钮名称">按钮名称，应该与记录类名相同</param>
    /// <returns>记录的完成状态</returns>
    private 评估状态 获取记录完成状态(string 按钮名称)
    {
        if (当前档案 == null) return 评估状态.未评估;

        try
        {
            // 使用反射获取档案中对应的记录字段
            var 档案类型 = typeof(档案);
            var 记录字段 = 档案类型.GetField(按钮名称);

            if (记录字段 != null)
            {
                var 记录对象 = 记录字段.GetValue(当前档案);
                if (记录对象 is 档案记录接口 记录接口)
                {
                    return 记录接口.是否完成;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"获取记录状态失败: {按钮名称}, 错误: {ex.Message}");
        }

        return 评估状态.未评估;
    }

    /// <summary>
    /// 进入评估界面
    /// </summary>
    /// <param name="按钮">点击的评估按钮</param>
    private void 进入评估界面(Button 按钮)
    {
        string 按钮名称 = 按钮.gameObject.name;

        // 不在进入时修改状态，只有保存时才修改
        // 更新记录状态为评估中
        // 更新记录状态(按钮名称, 评估状态.未完成);

        // 跳转到对应界面
        FindObjectOfType<界面管理器_韩>().显示界面("量表评估" + 按钮名称);

        Debug.Log($"进入评估界面: {按钮名称}");
    }

    /// <summary>
    /// 更新指定记录的状态
    /// </summary>
    /// <param name="记录名称">记录名称</param>
    /// <param name="新状态">新的评估状态</param>
    public void 更新记录状态(string 记录名称, 评估状态 新状态)
    {
        if (当前档案 == null)
        {
            Debug.LogWarning("当前档案为空");
            return;
        }

        try
        {
            var 档案类型 = typeof(档案);
            var 记录字段 = 档案类型.GetField(记录名称);

            if (记录字段 != null)
            {
                var 记录对象 = 记录字段.GetValue(当前档案);
                if (记录对象 is 档案记录接口 记录接口)
                {
                    记录接口.是否完成 = 新状态;
                    if (新状态 == 评估状态.评估完成)
                    {
                        记录接口.评估时间 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    }

                    // 保存档案更改
                    var 档案录 = this.GetModel<档案录>();
                    this.SendCommand(new 改记于录(当前档案, 当前档案, "档案录"));

                    Debug.Log($"记录 {记录名称} 状态已更新为: {新状态}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"更新记录状态失败: {记录名称}, 错误: {ex.Message}");
        }
    }

    /// <summary>
    /// 标记记录为完成（供其他界面调用）
    /// </summary>
    /// <param name="记录名称">记录名称</param>
    public void 标记记录完成(string 记录名称)
    {
        更新记录状态(记录名称, 评估状态.评估完成);
        更新按钮显示状态(); // 刷新按钮显示
    }

    /// <summary>
    /// 检查所有评估是否已完成
    /// </summary>
    /// <returns>如果所有评估都已完成返回true，否则返回false</returns>
    private bool 检查所有评估是否完成()
    {
        if (当前档案 == null || 评估按钮 == null || 评估按钮.Count == 0)
            return false;

        foreach (var 按钮 in 评估按钮)
        {
            string 按钮名称 = 按钮.gameObject.name;
            var 记录状态 = 获取记录完成状态(按钮名称);

            // 只要有一个不是"评估完成"状态，就返回false
            if (记录状态 != 评估状态.评估完成)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 更新下一步按钮的显示状态
    /// </summary>
    private void 更新下一步按钮状态()
    {
        if (下一步按钮 == null) return;

        bool 所有评估完成 = 检查所有评估是否完成();
        下一步按钮.gameObject.SetActive(所有评估完成);

        if (所有评估完成)
        {
            Debug.Log("所有评估已完成，显示下一步按钮");
        }
    }
}
