using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[RequireComponent(typeof(FlyingEyeMovement))]
[RequireComponent(typeof(FlyingEyeAttack))]
[RequireComponent(typeof(DefenceEnemies))]
public class FlyingEye : Enemies
{
    //查看有无击杀飞眼的任务
    public GameObject NPC;
    private TalkUI talker;
    

    //private FlyingEyeMovement movementComponent;
    private FlyingEyeAttack attackComponent;

    public override void Initialize()
    {
        if (NPC != null)//如果有NPC布置了击杀FlyEyes的任务
        {
            //完成任务需求的对象
            talker = NPC.GetComponent<TalkUI>();
        }

        priorityInType = 2;

        //movementComponent = GetComponent<FlyingEyeMovement>();
        //if (movementComponent == null)
        //{
        //    Debug.LogError("在FlyingEye中，没有找到Movement脚本！");
        //}
        attackComponent = GetComponent<FlyingEyeAttack>();
        if (attackComponent == null)
        {
            Debug.LogError("在FlyingEye中，没有找到Attack脚本！");
        }

        defenceComponent = GetComponent<DefenceEnemies>();
        if (defenceComponent == null)
        {
            Debug.LogError("在FlyingEye中，没有找到Defence脚本！");
        }
        //设置最大生命值 
        defenceComponent.Initialize(3);

    }

    public override void MyUpdate()
    {
        DefenceCheck(); 
       // MoveControl();
    }

    //移动流程
    //private void MoveControl()
    //{
    //    movementComponent.RequestMoveByFrame(FlyingEyeMovement.MovementMode.Normal);
    //}

    private void DefenceCheck()
    {
        defenceComponent.AttackCheck();
        if (defenceComponent.getIsDead())
        {
            gameObject.SetActive(false);
        }
        defenceComponent.Clear();
    }
    
    public override int GetPriorityInType()
    {
        return priorityInType;
    }
    protected override void OnDisable()//接受了杀死FlyEyes的任务会调用这个方法
    {
        if (NPC != null)
        {
            talker.killCount++;
            if (talker.killCount >= 2)
            {
                talker.isFinishMission = true;
                talker.isEndDialog2 = true;
            }
            
        }
        
    }

}
