using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
public class GetModelByStringUtility : IUtility
{
    public Dictionary<string, IModel> canListDict;
    public void Register2System(IModel model)
    {
        if (canListDict == null)
        {
            canListDict = new Dictionary<string, IModel>();
        }
        canListDict.Add(model.GetType().Name, model);
        //  Debug.Log("注册了" + model.GetType().Name);
    }
    public T GetModel<T>(string modelName) where T : class, IModel
    {
        if (canListDict.TryGetValue(modelName, out var model))
        {
            // Debug.Log("找到了" + model.GetType().Name);
            return model as T;
        }
        return null;
    }

}
