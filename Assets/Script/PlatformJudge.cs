/**
 * @Description: Platform类用户判断平台，并负责监控跳跃事件（移动端）
 * @Author: CuteRed
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public class PlatformJudge : MonoBehaviour
{
#if UNITY_WEBGL
    [DllImport("PlatformJudge")]
    private static extern float HelloFloat();
#endif

    public Platfrom platform;
    public enum Platfrom { IOS, ANDROID, PC, WEB_MOBILE, WEB_PC };

    /// <summary>
    /// 所有控制按钮的父物体
    /// </summary>
    private GameObject buttons;

    [Header("按钮")]
    private Button interactiveButton;

    /// <summary>
    /// 跳跃状态
    /// </summary>
    private bool isJump = false;
    

    void Awake()
    {
        buttons = GameObject.Find("ControllerButton");
        if (buttons == null)
        {
            Debug.LogError("获取移动端按钮失败");
        }


        //以下处理平台
#if UNITY_ANDROID
    Debug.Log("这里是安卓设备");
    platform = Platform.ANDROID;
#endif

#if UNITY_IPHONE
        Debug.Log("这里是苹果设备");
        platform = Platfrom.IOS;
#endif

#if UNITY_WEBGL
        Debug.Log("这里是WEB");
        //f为1，为手机端，为2，为pc
        float f = HelloFloat();
        if (f == 1.0f)
        {
            platform = Platfrom.WEB_MOBILE;
        }
        else if (f == 2.0f)
        {
            platform = Platfrom.WEB_PC;
        }
#endif

#if UNITY_STANDALONE_WIN
        Debug.Log("我是从Windows的电脑上运行的");
        platform = Platfrom.PC;
#endif

        //如果是移动端，则打开所有移动端的按钮
        if (platform == PlatformJudge.Platfrom.PC || platform == PlatformJudge.Platfrom.WEB_PC)
        {
            buttons.SetActive(false);
        }
    }


    /// <summary>
    /// 返回当前平台类型
    /// </summary>
    /// <returns></returns>
    public Platfrom GetPlatform()
    {
        return platform;
    }

    public void Jump()
    {
        isJump = true;
    }

    /// <summary>
    /// 判断是否按下了跳跃键
    /// </summary>
    /// <returns></returns>
    public bool IsJump()
    {
        return isJump;
    }

    /// <summary>
    /// 清空跳跃
    /// </summary>
    public void ClearJump()
    {
        isJump = false;
    }

    /// <summary>
    /// 重新加载场景
    /// </summary>
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}