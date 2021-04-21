using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Ability.Wind
{
    public class WindShortAttack : SpellAttackEvent
    {
        private Collider2D windShortMainAttackArea;
        private int windShortMainAttackDamage = 1;
        public const string WIND_NAME = "Wind";
        private float windShortMainStatusTime = 0.2f;
        //该技能攻击打断敌人参数
        private float windInterruptTime = 0.2f;
        private Vector2 windInterruptVector = new Vector2(5f, 0);
        // 加载collider
        protected override void Awake()
        {
            base.Awake();
            windShortMainAttackArea = GameObject.Find("WindShortMainCollider").GetComponent<Collider2D>();
            
        }
        private void Start()
        {
            gameObject.SetActive(false);
        }

        //攻击事件代码
        public void WindShortMainEvent()
        {
            Debug.Log("WindShortMainEvent");
            //找到攻击命中的单位 canFight.AttackArea实现了攻击
            targets = canFight.AttackArea(windShortMainAttackArea, windShortMainAttackDamage);
            if (targets != null)
            {
                foreach (CanBeFighted a in targets)
                {
                    targetsName.Append(" ");
                    targetsName.Append(a.gameObject.name);
                    //击退效果
                    //BeatBack(a, windInterruptTime, windInterruptVector);
                    a.BeatBack(transform, windInterruptTime, windInterruptVector);
                }
                Debug.Log("打到了： " + targetsName.ToString());

                targetsName.Clear();
            }
        }
    }
}