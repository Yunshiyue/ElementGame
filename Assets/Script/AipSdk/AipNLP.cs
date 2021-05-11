using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AipNLP
{
    private string APP_ID = "23744642";
    private string API_KEY = "9bmV43mF7uZ8DBIptcCo6j2g";
    private string SECRET_KEY = "H9APqqIPnDmSTQHnmS0MLpZBFsTitHWW";
    Baidu.Aip.Nlp.Nlp client;
    public AipNLP()
    {
        client = new Baidu.Aip.Nlp.Nlp(API_KEY, SECRET_KEY);
        client.Timeout = 60000;  // 修改超时时间
    }
    public void LexerDemo()
    {
        var text = "百度是一家高科技公司";

        // 调用词法分析，可能会抛出网络等异常，请使用try/catch捕获
        var result = client.Lexer(text);
        Debug.Log(result);
    }
    public string SentimentClassifyResult(string asrResult)
    {
        //var text = "苹果是一家伟大的公司";

        // 调用情感倾向分析，可能会抛出网络等异常，请使用try/catch捕获
        Debug.Log(asrResult);
        var result = client.SentimentClassify(asrResult);
        Debug.Log(result);
        var re = result.GetValue("items");//.ToObject<string>();
        var value = re[0].ToObject<Newtonsoft.Json.Linq.JObject>();
        Debug.Log(value.GetValue("sentiment"));

        string resultValue="";
        if ((float)value.GetValue("confidence") > 0.5f)
        {
            switch ((int)value.GetValue("sentiment"))
            {
                case 0:resultValue = "消极态度";break;
                case 1:resultValue = "中立态度";break;
                case 2:resultValue = "积极态度";break;
                default:resultValue = "???";break;
            }
        }
        else
        {
            resultValue = "无法辨明你的态度，请再说一边吧！"; 
        }
        return resultValue;
    }


}
