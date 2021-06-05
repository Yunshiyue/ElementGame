/**
 * @Description: SimpleButton类用户控制普通按钮（点击，释放）
 * @Author: CuteRed
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SimpleButton : MonoBehaviour, IPointerClickHandler, IPointerUpHandler
{
    public bool isTrigger = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        isTrigger = true;
        Debug.Log("按下");
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        isTrigger = false;
        Debug.Log("松开");
    }

    public bool IsTrigger()
    {
        return isTrigger;
    }
}
