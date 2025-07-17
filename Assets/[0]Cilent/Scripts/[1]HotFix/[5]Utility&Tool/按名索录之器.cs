using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
public class 按名索录之器 : IUtility
{
    public Dictionary<string, IModel> canListDict;
    public void 入录(IModel model)
    {
        if (canListDict == null)
        {
            canListDict = new Dictionary<string, IModel>();
        }
        canListDict.Add(model.GetType().Name, model);
        //  Debug.Log("注册了" + model.GetType().Name);
    }
    public T 寻录<T>(string modelName) where T : class, IModel
    {
        if (canListDict.TryGetValue(modelName, out var model))
        {
            // Debug.Log("找到了" + model.GetType().Name);
            return model as T;
        }
        return null;
    }

}
