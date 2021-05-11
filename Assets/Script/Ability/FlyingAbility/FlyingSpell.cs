using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class FlyingSpell : Spell
{
    protected PoolManager poolManager;
    protected MovementPlayer movementComponent;

    protected string spellName;

    //务必在子类中初始化Spellname
    public override void Initialize()
    {
        base.Initialize();

        poolManager = GameObject.Find("PoolManager").GetComponent<PoolManager>();
        if (poolManager == null)
        {
            Debug.LogError("在" + spellName + "技能中，没有找到PoolManager脚本或物体！");
        }

        movementComponent = player.GetComponent<MovementPlayer>();
        if (movementComponent == null)
        {
            Debug.LogError("在" + spellName + "火球术技能中，没有找到movementComponent脚本或物体！");
        }
    }
    public override void Enable()
    {
        //Debug.Log(spellName + "被启用了！");
    }
    public override void Disable()
    {
        //销毁池中的物体
        poolManager.ClearPool(spellName);
    }

    public abstract override void Cast();
}
