/**
 * @Description: SupportingElementButton类控制2个辅元素按钮（移动端）
 * @Author: CuteRed
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SupportingElementButton : MonoBehaviour, IPointerEnterHandler, IPointerUpHandler, IPointerExitHandler
{
    public bool isTrigger = false;   

    public void OnPointerEnter(PointerEventData eventData)
    {
        isTrigger = true;
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        isTrigger = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isTrigger = false;
    }

    public bool IsTrigger()
    {
        return isTrigger;
    }
}
