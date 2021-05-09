using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindFieldSpell : Spell
{
    private WindField windField;
    private GameObject inSprite;
    private GameObject outSprite;

    private bool isOn = false;
    private float windFieldCurTime = 0f;
    private float windFieldTotalTime = 10f;
    public override void Initialize()
    {
        base.Initialize();
        playerAnim.SetSpell(this, SkillType.WindField);

        windField = GameObject.Find("WindField").GetComponent<WindField>();
        windField.gameObject.SetActive(false);

        inSprite = windField.transform.Find("WindFieldInSprite").gameObject;
        inSprite.SetActive(false);

        outSprite = windField.transform.Find("WindFieldOutSprite").gameObject;
        outSprite.SetActive(false);
    }
    public override void Cast()
    {
        if(isOn)
        {
            windField.SetDragOrNot(!windField.GetIsDragging());
            if(windField.GetIsDragging())
            {
                inSprite.SetActive(true);
                outSprite.SetActive(false);
            }
            else
            {
                outSprite.SetActive(true);
                inSprite.SetActive(false);
            }
        }
        else
        {
            //设置动画
            playerAnim.SetUseSkillType(SkillType.WindField);
        }
    }
    public int GetNextAuxiliaryCost()
    {
        if (isOn)
            return 0;
        else
            return 1;
    }
    public override void ReleaseSpell()
    {
        Debug.Log("启动风域！");
        isOn = true;
        windFieldCurTime = 0f;
        windField.gameObject.SetActive(true);
        windField.SetDragOrNot(true);
        inSprite.SetActive(true);
        outSprite.SetActive(false);
    }
    public void windFieldClock()
    {
        if (isOn)
        {
            windFieldCurTime += Time.deltaTime;
            if(windFieldCurTime >= windFieldTotalTime)
            {
                isOn = false;
                windFieldCurTime = 0f;
                windField.gameObject.SetActive(false);
            }
        }
    }

    public override void Disable()
    {
    }

    public override void Enable()
    {
    }
}
