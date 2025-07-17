using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public static class CommandExtensions
{
    /// <summary>
    /// 通过名称获取Model并转换为IListModel
    /// </summary>
    public static I录贞 按名取录(this AbstractCommand command, string modelName)
    {
        var model = command.GetUtility<按名索录之器>().寻录<I录贞>(modelName);
        return model;
    }
    public static I录贞 按名取录(this AbstractCommand<List<int>> command, string modelName)
    {
        var model = command.GetUtility<按名索录之器>().寻录<I录贞>(modelName);
        return model;
    }
    public static I录贞 按名取录(this IController controller, string modelName)
    {
        var model = controller.GetUtility<按名索录之器>().寻录<I录贞>(modelName);
        return model;
    }
  
}
