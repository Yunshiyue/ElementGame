/**
 * @Description: GameMenu类是游戏菜单的控制类，界面上按钮的控制和暂停菜单的控制
 * @Author: CuteRed
1-3-7 14:30
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameMenu : MonoBehaviour
{
    /// <summary>
    /// 暂停菜单
    /// </summary>
    private GameObject pauseMenu;
    private GameObject switchButton;
    private GameObject pauseButton;

    /// <summary>
    /// 切换元素菜单
    /// </summary>
    private ElementMenu elementMenu;

    /// <summary>
    /// 死亡界面
    /// </summary>
    private GameObject gameOverPanel;

    private void Awake()
    {
        pauseMenu = GameObject.Find("Canvas/GameMenu/PauseMenu");
        if (pauseMenu == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到pauseMenu");
        }

        switchButton = GameObject.Find("Canvas/GameMenu/SwitchButton");
        if (switchButton == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到switchButton");
        }

        pauseButton = GameObject.Find("Canvas/GameMenu/PauseButton");
        if (pauseButton == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到pauseButton");
        }

        elementMenu = GameObject.Find("SwitchElementMenu").GetComponent<ElementMenu>();
        if (elementMenu == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到switchElementMenu");
        }

        gameOverPanel = GameObject.Find("GameOverPanel");
        if (gameOverPanel == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到GameOverPanel");
        }

        //所有菜单隐藏
        pauseMenu.SetActive(false);
        elementMenu.gameObject.SetActive(false);
        gameOverPanel.SetActive(false);

        Debug.Log("UI加载完成");
    }
    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void PauseGame()
    {
        Debug.Log("暂停");
        //显示暂停菜单
        pauseMenu.SetActive(true);

        //原界面按钮失效
        switchButton.SetActive(false);
        pauseButton.SetActive(false);

        elementMenu.gameObject.SetActive(false);

        //暂停
        Time.timeScale = 0;
    }

    /// <summary>
    /// 继续游戏
    /// </summary>
    public void ContinueGame()
    {
        //隐藏暂停菜单
        pauseMenu.SetActive(false);

        //原界面按钮生效
        switchButton.SetActive(true);
        pauseButton.SetActive(true);

        //游戏继续
        Time.timeScale = 1;
    }

    /// <summary>
    /// 返回主菜单
    /// </summary>
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(2);
        Time.timeScale = 1;
    }

    /// <summary>
    /// 点击切换元素按钮
    /// </summary>
    public void SwitchElementMenu()
    {
        elementMenu.gameObject.SetActive(true);
        elementMenu.RefreshItem();
    }

    /// <summary>
    /// 游戏结束（死亡）
    /// </summary>
    public void GameOver()
    {
        //界面上所有按钮及菜单隐藏
        switchButton.SetActive(false);
        pauseButton.SetActive(false);
        elementMenu.gameObject.SetActive(false);

        //死亡界面显示
        gameOverPanel.SetActive(true);

        //Time.timeScale = 0;
    }

    public void DebugText()
    {
        Debug.Log("按钮点击");
    }
}