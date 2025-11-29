using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System;
using System.Linq;
using LitJson;
using System.IO;

public class Can2ListModelChangeEvent
{
    public string ModelTypeName;
}

public class Can2ListModelFramework
{

}

public interface I记贞
{

}
/// <summary>
/// 提供通用列表操作的接口，不依赖泛型参数
/// </summary>
public interface I录贞 : IModel
{
    /// <summary>
    /// 添加一个项到列表
    /// </summary>
    /// <param name="item">要添加的项</param>
    /// <param name="insertAtStart">是否插入到列表开头</param>
    void AddItem(I记贞 item, bool insertAtStart = true);

    /// <summary>
    /// 从列表中移除一个项
    /// </summary>
    /// <param name="item">要移除的项</param>
    void RemoveItem(I记贞 item);

    /// <summary>
    /// 清空列表
    /// </summary>
    void ClearItems();
    /// <summary>
    /// 编辑一个项
    /// </summary>
    bool EditItem(I记贞 oldItem, I记贞 newItem);
    /// <summary>
    /// 获取一个项的索引
    /// </summary>
    int GetIndex(I记贞 item);
    /// <summary>
    /// 获取所有项
    /// </summary>
    /// <returns>列表中的所有项</returns>
    List<I记贞> GetAllItems();

    /// <summary>
    /// 根据关键字搜索
    /// </summary>
    /// <param name="keyword">关键字</param>
    /// <returns>搜索到的项的索引列表</returns>
    List<int> SearchByName(string keyword);
    bool 纳记于信(string 信, bool 清空 = false);
}
[Serializable]
public abstract class 录元<T> : AbstractModel, ICanGetSystem, I录贞 where T : I记贞
{
    protected List<T> dataList;
    protected abstract string GetStorageKey();
    // 保存数据的方法
    protected override void OnInit()
    {
        this.GetUtility<按名索录之器>().入录(this);
        LoadData();
    }
    protected void SaveData()
    {
        var storage = this.GetUtility<StorageUtility>();
        this.SendEvent<Can2ListModelChangeEvent>(new Can2ListModelChangeEvent() { ModelTypeName = this.GetType().Name });
        storage.SaveValue(GetStorageKey(), dataList);
    }
    // 加载数据的方法
    protected void LoadData()
    {
        dataList = new List<T>();
        var storage = this.GetUtility<StorageUtility>();
        List<T> loadedData = new List<T>();
        if (storage.GetValue(GetStorageKey(), ref loadedData))
        {
            dataList = loadedData;
        }
    }
    // 提供操作列表的方法，自动调用保存
    public void AddItem(T item, bool InsertOfStart = true)
    {
        if (InsertOfStart)
        {
            dataList.Insert(0, item);
        }
        else
        {
            dataList.Add(item);
        }
        SaveData();
    }
    public void RemoveItem(T item)
    {
        dataList.Remove(item);
        SaveData();
    }
    public void ClearItems()
    {
        dataList.Clear();
        SaveData();
    }
    public bool EditItem(T oldItem, T newItem)
    {
        int index = dataList.IndexOf(oldItem);
        if (index != -1)
        {
            dataList[index] = newItem;
            SaveData();
            return true;
        }
        return false;
    }
    public List<T> GetDataList()
    {
        return dataList;
    }
    public int GetIndex(T item)
    {
        return dataList.IndexOf(item);
    }
    /// <summary>
    /// 添加一个ICan2List项到列表
    /// </summary>
    void I录贞.AddItem(I记贞 item, bool insertAtStart)
    {
        if (item is T typedItem)
        {
            AddItem(typedItem, insertAtStart);
        }
        else
        {
            Debug.LogError($"类型不匹配: 期望 {typeof(T).Name}，实际 {item.GetType().Name}");
        }
    }

    /// <summary>
    /// 从列表中移除一个ICan2List项
    /// </summary>
    void I录贞.RemoveItem(I记贞 item)
    {
        if (item is T typedItem)
        {
            RemoveItem(typedItem);
        }
        else
        {
            Debug.LogError($"类型不匹配: 期望 {typeof(T).Name}，实际 {item.GetType().Name}");
        }
    }



    public bool EditItem(I记贞 oldItem, I记贞 newItem)
    {
        if (oldItem is T typedOldItem && newItem is T typedNewItem)
        {
            return EditItem(typedOldItem, typedNewItem);
        }
        else
        {
            Debug.LogError($"类型不匹配: 期望 {typeof(T).Name}，实际 oldItem:{oldItem.GetType().Name}, newItem:{newItem.GetType().Name}");
            return false;
        }
    }
    int I录贞.GetIndex(I记贞 item)
    {
        if (item is T typedItem)
        {
            return GetIndex(typedItem);
        }
        else
        {
            Debug.LogError($"类型不匹配: 期望 {typeof(T).Name}，实际 {item.GetType().Name}");
            return -1;
        }
    }
    /// <summary>
    /// 获取所有项作为ICan2List列表
    /// </summary>
    List<I记贞> I录贞.GetAllItems()
    {
        return dataList.Cast<I记贞>().ToList();
    }

    List<int> I录贞.SearchByName(string keyword)
    {
        return OnSearchByName(keyword);
    }
    protected virtual List<int> OnSearchByName(string keyword)
    {
        return new List<int>();
    }

    public bool 纳记于信(string 信, bool 清空 = false)
    {
        if (string.IsNullOrWhiteSpace(信))
        {
            Debug.LogWarning("纳记于信失败: 传入内容为空");
            return false;
        }

        T 数据;
        try
        {
            数据 = JsonMapper.ToObject<T>(信);
        }
        catch (Exception e)
        {
            Debug.LogError($"纳记于信失败: {e.Message}");
            return false;
        }

        if (数据 == null)
        {
            Debug.LogWarning("纳记于信结束: 解析结果为空");
            return false;
        }

        if (清空)
        {
            ClearItems();
        }

        AddItem(数据, false);
        Debug.Log($"纳记于信成功: {typeof(T).Name}");
        SaveData();
        return true;
    }
    #region JSON导入导出功能

    /// <summary>
    /// 导出数据为JSON文件
    /// </summary>
    /// <param name="fileName">文件名（不含扩展名，可选）</param>
    /// <returns>是否导出成功</returns>
    public bool ExportToJson(string fileName = null)
    {
        var jsonUtility = this.GetUtility<JsonDataUtility>();
        if (fileName == null)
        {
            fileName = GetStorageKey();
        }
        return jsonUtility.ExportToJson(dataList, fileName);
    }

    /// <summary>
    /// 从JSON文件导入数据
    /// </summary>
    /// <param name="fileName">文件名（含或不含扩展名）</param>
    /// <param name="clearExisting">是否清空现有数据</param>
    /// <returns>是否导入成功</returns>
    public bool ImportFromJson(bool clearExisting = true, string fileName = null)
    {
        var jsonUtility = this.GetUtility<JsonDataUtility>();
        if (fileName == null)
        {
            fileName = GetStorageKey();
        }
        var importedData = jsonUtility.ImportFromJson<T>(fileName);

        if (importedData == null)
        {
            return false;
        }

        // 清空现有数据（如果需要）
        if (clearExisting)
        {
            dataList.Clear();
        }

        // 添加导入的数据
        foreach (var item in importedData)
        {
            dataList.Add(item);
        }

        // 保存数据
        SaveData();

        return true;
    }

    /// <summary>
    /// 获取所有可用的JSON文件名
    /// </summary>
    /// <returns>文件名列表</returns>
    public List<string> GetAvailableJsonFileNames()
    {
        var jsonUtility = this.GetUtility<JsonDataUtility>();
        return jsonUtility.GetAvailableJsonFileNames();
    }

    /// <summary>
    /// 打开导出目录
    /// </summary>
    public void OpenExportDirectory()
    {
        var jsonUtility = this.GetUtility<JsonDataUtility>();
        jsonUtility.OpenExportDirectory();
    }

    #endregion
}
public abstract class CrisisIncidentFileBaseModel<T> : 录元<T> where T : I记贞
{
    /// <summary>
    /// 导出到特定路径（重写基类方法以支持更多文件格式）
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="format">文件格式</param>
    /// <returns>是否成功</returns>
    public virtual bool ExportToFile(string filePath, string format = "json")
    {
        switch (format.ToLower())
        {
            case "json":
                // 由于我们简化了设计，这里暂时不支持指定路径导出
                Debug.LogWarning("当前版本不支持指定路径导出，请使用默认路径导出");
                return ExportToJson();
            case "csv":
                return ExportToCsv(filePath);
            case "xml":
                return ExportToXml(filePath);
            default:
                Debug.LogError($"不支持的文件格式: {format}");
                return false;
        }
    }

    /// <summary>
    /// 导出为CSV格式（可选功能）
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>是否成功</returns>
    protected virtual bool ExportToCsv(string filePath)
    {
        Debug.LogWarning("CSV导出功能需要子类实现");
        return false;
    }

    /// <summary>
    /// 导出为XML格式（可选功能）
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>是否成功</returns>
    protected virtual bool ExportToXml(string filePath)
    {
        Debug.LogWarning("XML导出功能需要子类实现");
        return false;
    }
}