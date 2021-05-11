using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteAutoFetchElement : RemoteAutoFetch
{
    protected ElementAbilityManager abilityManager;
    protected ElementAbilityManager.Element element;
    public int restorePoint = 1;
    protected override void Awake()
    {
        base.Awake();
        abilityManager = player.GetComponent<ElementAbilityManager>();
    }
    protected override void GetFetched()
    {
        abilityManager.RestoreElement(element, restorePoint);
    }
    protected void ColorMyself()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        switch(element)
        {
            case ElementAbilityManager.Element.Fire:
                sprite.color = fireColor;
                //Debug.Log("火属性晶石，颜色为" + sprite.color);
                break;
            case ElementAbilityManager.Element.Thunder:
                sprite.color = thunderColor;
                //Debug.Log("雷属性晶石，颜色为" + sprite.color);
                break;
            case ElementAbilityManager.Element.Ice:
                sprite.color = iceColor;
                //Debug.Log("冰属性晶石，颜色为" + sprite.color);
                break;
            case ElementAbilityManager.Element.Wind:
                sprite.color = windColor;
                //Debug.Log("风属性晶石，颜色为" + sprite.color);
                break;
        }
    }
    private Color fireColor = new Color(221.0f / 255, 60f / 255, 78f / 255, 255f / 255);
    private Color thunderColor = new Color(173f / 255, 83f / 255, 242f / 255, 255f / 255);
    private Color iceColor = new Color(87f / 255, 217f / 255, 242f / 255, 255f / 255);
    private Color windColor = new Color(54f / 255, 202f / 255, 150f / 255, 255f / 255);
}
