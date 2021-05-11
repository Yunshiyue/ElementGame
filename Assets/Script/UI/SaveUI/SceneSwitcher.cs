using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    private SaveManager saveManager;
    private int targetSceneIndex;
    private Slider sceneLoadSlider;
    private GameObject sceneSwitchPanel;

    private void Awake()
    {
        saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        sceneLoadSlider = GameObject.Find("SceneSwitchSlider").GetComponent<Slider>();
        sceneSwitchPanel = GameObject.Find("SceneSwitchPanel");
    }
    private void Start()
    {
        sceneSwitchPanel.SetActive(false);
        //显示过场动画
    }
    public void ChangeScene(int targetSceneIndex)
    {
        this.targetSceneIndex = targetSceneIndex;
        sceneSwitchPanel.SetActive(true);
        StartCoroutine(nameof(StartChangingTargetScene));
    }
    private IEnumerator StartChangingTargetScene()
    {
        var changeResult = SceneManager.LoadSceneAsync(targetSceneIndex);
        changeResult.allowSceneActivation = false;

        //显示待机画面
        Debug.Log("开始切换场景");
        //到0.9时会掐掉所有协程，进入下一个场景，所以需要在下一个场景开始的时候加载类对象
        while (! changeResult.isDone)
        {
            //Debug.Log(string.Format("切换进度{0}%", changeResult.progress));
            sceneLoadSlider.value = changeResult.progress + 0.1f;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                changeResult.allowSceneActivation = true;
            }
            //滚动条
            yield return null;
        }
    }
}
