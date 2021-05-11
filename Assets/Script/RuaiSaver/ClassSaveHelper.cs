using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class ClassSaveHelper
{
    Dictionary<string, string> classInfoContent = new Dictionary<string, string>();
    public bool LoadClassJsonString(string JsonContenet)
    {
        try
        {
            classInfoContent = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonContenet);
        }
        catch (System.Exception)
        {
            Debug.LogError("字符串不能被正确识别为一个Json类型");
            return false;
        }
        return true;
    }
    public bool LoadValue<T>(string nameOfValue, out T tParameter)
    {
        if(classInfoContent != null)
        {
            string jsonValue;
            if (classInfoContent.TryGetValue(nameOfValue, out jsonValue))
            {
                tParameter = JsonConvert.DeserializeObject<T>(jsonValue);
                return true;
            }
            Debug.Log(nameOfValue + "不是Dictionary中的一个Key");

            tParameter = default(T);
            return false;
        }
        Debug.LogError("还未调用LoadClass函数！");
        tParameter = default(T);
        return false;
    }

    public void SaveValue<T>(string nameOfValue, T tValue)
    {
        if(! SerializableConvert.CanSerializeType<T>())
        {
            Debug.LogError("无法序列化该类：" + nameof(T));
        }
        classInfoContent.Add(nameOfValue, JsonConvert.SerializeObject(tValue));
    }
    public string GetJsonString()
    {
        return JsonConvert.SerializeObject(classInfoContent);
    }
}
