using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class SavePlayer : MonoBehaviour, ClassSaver
{
    public string GetID()
    {
        return nameof(SavePlayer);
    }
    public void LoadClass(string content)
    {
        ClassSaveHelper helper = new ClassSaveHelper();
        helper.LoadClassJsonString(content);
        SerializableVector3 temp;
        helper.LoadValue(nameof(transform.position), out temp);
        transform.position = SerializableConvert.DeserializableVector3(temp);

        //Dictionary<string, string> dicContent = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
        //string temp;
        //dicContent.TryGetValue(nameof(transform.position), out temp);
        //transform.position = SerializableConvert.DeserializableVector3(JsonConvert.DeserializeObject<SerializableVector3>(temp));
    }
    public string SaveClass()
    {
        ClassSaveHelper helper = new ClassSaveHelper();
        helper.SaveValue(nameof(transform.position), SerializableConvert.GetSerializableVector3(transform.position));
        return helper.GetJsonString();

        //Dictionary<string, string> content = new Dictionary<string, string>();
        //content.Add(nameof(transform.position), JsonConvert.SerializeObject(SerializableConvert.GetSerializableVector3(transform.position)));
        //return JsonConvert.SerializeObject(content);
    }
}
