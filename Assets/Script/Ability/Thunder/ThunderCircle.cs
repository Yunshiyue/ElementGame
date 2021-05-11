using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderCircle : MonoBehaviour
{
    private CircleCollider2D thunderCircleCollider;
    private ThunderAbility abilityComponent;
    private LayerMask enemyLayer;
    private Vector3 smallCircleScale = new Vector3(10, 10, 10);
    private Vector3 largeCircleScale = new Vector3(15, 15, 15);
    private CanFight canFight;
    private void Start()
    {
        enemyLayer = LayerMask.NameToLayer("Enemy");
        thunderCircleCollider = GetComponent<CircleCollider2D>();
        if(thunderCircleCollider == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到雷圈collider");
        }
        GameObject player = GameObject.Find("Player");
        abilityComponent = player.GetComponent<ThunderAbility>();
        if(abilityComponent == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到abilityComponent");
        }
        canFight = player.GetComponent<CanFight>();
    }
    public void ExpendCircle()
    {
        transform.localScale = largeCircleScale;
    }
    public void LessenCircle()
    {
        transform.localScale = smallCircleScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == enemyLayer)
        {
            abilityComponent.AddInThunderCircleTarget(collision.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == enemyLayer)
        {
            abilityComponent.RemoveInThunderCircleTarget(collision.gameObject);
        }
    }
    public void AttackThunderCircle()
    {
        canFight.AttackArea(thunderCircleCollider, 1, AttackInterruptType.WEAK, ElementAbilityManager.Element.Thunder);
    }
}
