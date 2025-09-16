using System;
using QFramework;
using UnityEngine;

/// <summary>
/// 软件配置数据模型
/// </summary>
[Serializable]
public class 软件配置 : I记贞
{
    public string 单位名称 { get; set; } = "";
    public string 软件名称 { get; set; } = "";
    public string 图片缓存名称 { get; set; } = "";

    // 实现I记贞接口的方法（如果需要用于数据管理）
    public string 获取编号()
    {
        return "软件配置";
    }
}

/// <summary>
/// 软件配置数据管理Model
/// </summary>
public class 软件配置录 : 录元<软件配置>
{
    private 软件配置 _当前配置;
    string SAVE_KEY = "软件配置";
    bool BlueToothState = false;
    public BindableProperty<bool> _蓝牙状态 = new BindableProperty<bool>();
    protected override void OnInit()
    {
        base.OnInit();
        // 检查是否需要添加默认管理员账号
        LoadConfig();
    }
    /// <summary>
    /// 当前软件配置
    /// </summary>
    public 软件配置 当前配置
    {
        get
        {
            if (_当前配置 == null)
            {
                LoadConfig();
            }
            return _当前配置;
        }
        private set { _当前配置 = value; }
    }



    /// <summary>
    /// 保存配置到本地
    /// </summary>
    public void 保存配置(软件配置 配置)
    {
        dataList[0] = 配置;
        _当前配置 = 配置;
        SaveData();
        // Save();
    }

    /// <summary>
    /// 从本地加载配置
    /// </summary>
    private void LoadConfig()
    {
        _当前配置 = dataList.Count > 0 ? dataList[0] : new 软件配置();
        if (dataList.Count == 0)
        {
            dataList.Add(_当前配置);
        }
    }

    /// <summary>
    /// 重置配置为默认值
    /// </summary>
    public void 重置配置()
    {
        _当前配置 = new 软件配置();
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();
        Debug.Log("软件配置已重置");
    }

    protected override string GetStorageKey()
    {
        return "软件配置";
    }

}