using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkBackSpell : Spell
{
    private MovementPlayer movementComponent;

    //Time应该为DetectTime的整数倍才有意义
    private float blinkBackTime = 3f;
    private float blinkBackDetectTime = 0.1f;
    private int BBVectorSize;
    private Vector2[] blinkBackPositions;
    private float BBCurTime = 0f;
    private int BBCurPointer = 0;
    private Transform[] shadowPositions;
    private GameObject shadowParent;
    private GameObject shadow;

    private bool isEnabled = false;

    public override void Initialize()
    {
        base.Initialize();
        movementComponent = player.GetComponent<MovementPlayer>();

        BBVectorSize = (int)(blinkBackTime / blinkBackDetectTime);


        shadowParent = GameObject.Find("Shadows");
        if (shadowParent == null)
        {
            Debug.LogError("在" + player.gameObject.name + "中，没有找到Shadows");
        }
        shadow = GameObject.Find("Shadow");
        if (shadowParent == null)
        {
            Debug.LogError("在" + player.gameObject.name + "中，没有找到Shadow");
        }

        shadow.SetActive(false);
        playerAnim.SetSpell(this,SkillType.WindFire);

    }
    public override void Disable()
    {
        if(isEnabled)
        {
            for (int i = 0; i < BBVectorSize; i++)
            {
                MonoBehaviour.Destroy(shadowPositions[i].gameObject);
            }
            shadowPositions = null;
            blinkBackPositions = null;
            isEnabled = false;
        }
    }

    public override void Enable()
    {
        Debug.Log("调用BB的enable");
        shadowPositions = new Transform[BBVectorSize];
        blinkBackPositions = new Vector2[BBVectorSize];
        GameObject tempShadow;
        for (int i = 0; i < BBVectorSize; i++)
        {
            blinkBackPositions[i] = new Vector2(player.transform.position.x, player.transform.position.y);
        }
        for (int i = 0; i < BBVectorSize; i++)
        {
            tempShadow = MonoBehaviour.Instantiate(shadow);
            shadowPositions[i] = tempShadow.transform;
            shadowPositions[i].SetParent(shadowParent.transform);
            shadowPositions[i].position = blinkBackPositions[i];
            tempShadow.SetActive(true);
        }
        isEnabled = true;
    }

    public void BlinkBackClock()
    {
        if(isEnabled)
        {
            BBCurTime += Time.deltaTime;
            if (BBCurTime >= blinkBackDetectTime)
            {
                blinkBackPositions[BBCurPointer].x = player.transform.position.x;
                blinkBackPositions[BBCurPointer].y = player.transform.position.y;
                shadowPositions[BBCurPointer].position = blinkBackPositions[BBCurPointer];

                BBCurPointer = (BBCurPointer + 1) % BBVectorSize;
                BBCurTime = 0f;
            }
        }
    }

    public override void Cast()
    {
        playerAnim.SetUseSkillType(SkillType.WindFire);
    }
    public override void ReleaseSpell()
    {
        movementComponent.RequestMoveByFrame(blinkBackPositions[(BBCurPointer + 1) % BBVectorSize],
            MovementPlayer.MovementMode.Ability, Space.World);
    }
}
