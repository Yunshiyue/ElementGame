/**
 * @Description: FrozenWater是流水机关类，初始状态下为冰，被火属性攻击后会融化成水
 * @Author: CuteRed

 *     
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrozenWater : Mechanism
{
    /// <summary>
    /// 流水的贴图
    /// </summary>
    public Sprite waterSprite;
    private SpriteRenderer renderer1;
    private GameObject childGameObject;


    protected override void Start()
    {
        base.Start();

        renderer1 = GetComponent<SpriteRenderer>();
        if (renderer1 == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到Renderer");
        }

        childGameObject = transform.GetChild(0).gameObject;
        if (childGameObject == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到childGameObject");
        }

    }

    public override void Trigger(TiggerType triggerType)
    {
        if (TriggerDetect())
        {
            if (triggerType == TiggerType.Fire)
            {
                //冰融化成水
                Melt();
                isTriggered = true;
            }
        }
    }

    protected override bool TriggerDetect()
    {
        return !isTriggered;
    }

    /// <summary>
    /// 冰融化成水，更换贴图
    /// </summary>
    private void Melt()
    {
        renderer1.sprite = waterSprite;

        //碰撞体可进入
        childGameObject.GetComponent<Collider2D>().isTrigger = true;
        childGameObject.gameObject.layer = LayerMask.NameToLayer("Mechanism");
    }
}
