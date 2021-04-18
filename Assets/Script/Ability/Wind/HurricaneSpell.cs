﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurricaneSpell : FlyingSpell
{
    private Vector2 flyingStartPositon = new Vector2();
    private WindAbility windAbility;

    public override void Initialize()
    {
        base.Initialize();
        spellName = Hurricane.HURRICANE;
        windAbility = player.GetComponent<WindAbility>();
    }
    public override void Cast()
    {

    }
    public void ReleaseSpell()
    {
        GameObject hurricane = poolManager.GetGameObject("Hurricane");
        Hurricane a = hurricane.GetComponent<Hurricane>();
        a.SetThrower(player);

        //根据风瞄准镜设置生成位置
        a.SetStartPosition(windAbility.GetSightHeadPosition());
    }
}
