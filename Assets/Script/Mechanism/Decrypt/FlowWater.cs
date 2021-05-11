/**
 * @Description: FlowWater是流水类机关，被冰属性攻击击中后，会凝固
 * @Author: CuteRed

 *     
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowWater : Mechanism
{
    /// <summary>
    /// 冰柱的贴图
    /// </summary>
    public Sprite iceSprite;
    private SpriteRenderer renderer1;
    private GameObject childGameObject;


    // Start is called before the first frame update
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

        if (iceSprite == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到iceSprite");
        }
    }

    protected override bool TriggerDetect()
    {
        return !isTriggered;
    }

    public override void Trigger(TiggerType triggerType)
    {
        if (TriggerDetect())
        {
            //检测冰属性
            if (triggerType == Mechanism.TiggerType.Ice)
            {
                Froze();
                isTriggered = true;
            }
        }

    }

    /// <summary>
    /// 冻结
    /// </summary>
    private void Froze()
    {
        //改变贴图
        renderer1.sprite = iceSprite;

        //碰撞体可进入
        childGameObject.GetComponent<Collider2D>().isTrigger = false;
        childGameObject.gameObject.layer = LayerMask.NameToLayer("Platform");
    }
}
