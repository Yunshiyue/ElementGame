using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class TestSaveClass : MonoBehaviour, ClassSaver
{
    public int time = 2;
    public string GetID()
    {
        return nameof(TestSaveClass);
    }

    public void LoadClass(string content)
    {
        Debug.Log(gameObject.name + " " + GetID() + "进入LoadClass");
        Dictionary<string, string> toBeLoad = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
        string timeStr;
        toBeLoad.TryGetValue(nameof(time), out timeStr);
        time = JsonConvert.DeserializeObject<int>(timeStr);
    }

    public string SaveClass()
    {
        Debug.Log(gameObject.name + " " + GetID() + "进入SaveClass");
        Dictionary<string, string> toBeSave = new Dictionary<string, string>();
        toBeSave.Add(nameof(time), JsonConvert.SerializeObject(time));
        string result = JsonConvert.SerializeObject(toBeSave);
        Debug.Log(gameObject.name + " " + GetID() + "返回值为" + result);
        return result;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
