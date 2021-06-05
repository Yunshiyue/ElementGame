using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TaskPanel : MonoBehaviour
{
    private Text currentScene;
    private Text sceneDescription;
    public string[] SceneDes;
    public string[] SceneName;
    // Start is called before the first frame update
    private void Awake()
    {
        currentScene = GameObject.Find("Current Scene").GetComponent<Text>();
        sceneDescription = GameObject.Find("Scene Description").GetComponent<Text>();
    }
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "Level3_ice")
        {
            currentScene.text = SceneName[1];
            sceneDescription.text = SceneDes[1];
        }
        else if(SceneManager.GetActiveScene().name == "Level2_MesteriousHole")
        {
            currentScene.text = SceneName[0];
            sceneDescription.text = SceneDes[0];
        }
    }

    public void ExitTaskPanel()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
