/**
 * @Description: SupportingElementButton类控制2个辅元素按钮（移动端）
 * @Author: CuteRed
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SupportingElementButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isTrigger = false;

    public void OnPointerEnter(PointerEventData eventData)
    {

        if (!isTrigger)
        {
            isTrigger = true;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isTrigger)
        {
            StartCoroutine(SleepExit());
            isTrigger = false;
        }
    }

    IEnumerator SleepExit()
    {
        yield return new WaitForSeconds(0.005f);
        isTrigger = false;
    }

    public bool IsTrigger()
    {
        return isTrigger;
    }
}
