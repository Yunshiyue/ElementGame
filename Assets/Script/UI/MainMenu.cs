/**
 * @Description: MainMenu类是主菜单的控制类，目前包括开始游戏、退出游戏控制
 * @Author: CuteRed
1-3-7 14:30
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

    private void Awake()
    {
        video = GameObject.Find("VideoPanel").GetComponent<VideoPlayer>();
        if (video == null)
        {
            Debug.LogError("在" + gameObject + "中，未找到VideoPlayer");
        }
    }

    private void OnEnable()
    {
        video = GameObject.Find("VideoPanel").GetComponent<VideoPlayer>();
        video.Play();
    }

    /// <summary>
    /// 开始游戏，加载场景
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    private void OnDisable()
    {
        video.Pause();
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }
}
