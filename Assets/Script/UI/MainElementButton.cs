/**
 * @Description: MainElementButton类控制主元素按钮（移动端）
 * @Author: CuteRed
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainElementButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    /*
     * 释放必须是点击的
     * 
     */

    public bool isTrigger = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("A点击");
        isTrigger = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("A进入");
        isTrigger = true;
    }

    
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("A释放");
        isTrigger = false;
    }

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    isTrigger = false;
    //}

    public bool IsTrigger()
    {
        return isTrigger;
    }
}
