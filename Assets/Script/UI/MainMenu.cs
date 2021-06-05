/**
 * @Description: MainMenu类是主菜单的控制类，目前包括开始游戏、退出游戏控制
 * @Author: CuteRed

*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MainMenu : MonoBehaviour
{
    bool isActive = false;
    private VideoPlayer video;

    private GameObject savePanel;
    private GameObject mainMenu;

    private SaveMenu saveMenu;

    private void Awake()
    {
        //video = GameObject.Find("VideoPanel").GetComponent<VideoPlayer>();
        //savePanel = GameObject.Find("SavePanel");
        mainMenu = GameObject.Find("Menu");
        //saveMenu = GameObject.Find("SaveMenu").GetComponent<SaveMenu>();

        //if (video == null)
        //{
        //    Debug.LogError("在" + gameObject + "中，未找到VideoPlayer");
        //}
    }
    private void Start()
    {
        //savePanel.SetActive(false);
    }

    private void OnEnable()
    {
        //video = GameObject.Find("VideoPanel").GetComponent<VideoPlayer>();
        //video.Play();
    }

    /// <summary>
    /// 开始游戏，加载场景
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ShowSavePanel()
    {
        //saveMenu.RefreshSavePanel(SaveManager.SaveMode.Load);
        //mainMenu.SetActive(false);
        //savePanel.SetActive(true);
    }
    public void ShowMainPanel()
    {
        mainMenu.SetActive(true);
        //savePanel.SetActive(false);
    }

    private void OnDisable()
    {
        //video.Pause();
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }
}
