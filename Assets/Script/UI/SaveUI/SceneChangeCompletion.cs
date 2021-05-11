using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class SceneChangeCompletion : MonoBehaviour
{
    private Text sceneChangeCompletionText;
    private Slider sceneSwitchSlider;
    private StringBuilder stringBuilder = new StringBuilder();
    private float lastRatio = 0f;
    private void Awake()
    {
        sceneChangeCompletionText = GetComponent<Text>();
        sceneSwitchSlider = GameObject.Find("SceneSwitchSlider").GetComponent<Slider>();
    }
    private void Start()
    {
        sceneChangeCompletionText.text = "0%";
    }

    void Update()
    {
        if(lastRatio != sceneSwitchSlider.value)
        {
            lastRatio = sceneSwitchSlider.value;
            stringBuilder.Clear();
            stringBuilder.Append(lastRatio * 100);
            stringBuilder.Append('%');

            if(lastRatio - 1f <= 0.001f)
            {
                stringBuilder.Append("  (PRESS SPACE TO CONTINUE)");
            }
            sceneChangeCompletionText.text = stringBuilder.ToString();
        }
    }
}
