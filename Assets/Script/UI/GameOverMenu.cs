/**
 * @Description: GameOverMenu类负责控制游戏结束菜单，目前包括返回主界面和重新游戏
 * @Author: CuteRed

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    /// <summary>
    /// 返回主菜单
    /// </summary>
    public void ReturnToMainMenu()
    {
        //Time.timeScale = 1;
        SceneManager.LoadScene(2);
    }

    /// <summary>
    /// 重新游戏
    /// </summary>
    public void PlayAgain()
    {
        //Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
