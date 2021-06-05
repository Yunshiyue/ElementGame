/**
 * @Description: MainElementButton类控制主元素按钮（移动端）
 * @Author: CuteRed
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainElementButton : MonoBehaviour, IPointerEnterHandler, IPointerUpHandler
{
    /*
     * 释放必须是点击的
     * 
     */

    public bool isTrigger = false;
    


    public void OnPointerEnter(PointerEventData eventData)
    {
        isTrigger = true;
    }

    
    public void OnPointerUp(PointerEventData eventData)
    {
        isTrigger = false;
    }

    public bool IsTrigger()
    {
        return isTrigger;
    }
}
