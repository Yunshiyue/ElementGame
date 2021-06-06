using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypingNLP : MonoBehaviour
{
    private AipNLP ap;
    public Text typing_content;
    public Text analysisResult;

    private void Start()
    {
        ap = new AipNLP();
    }

    public  void Click()
    {
        if (typing_content.text != null)
        {
            Debug.Log(typing_content.text);
            analysisResult.text = ap.SentimentClassifyResult(typing_content.text);
        }
        else
        {
            analysisResult.text = "无法识别";
        }
    }
}
