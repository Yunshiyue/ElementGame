using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderCircle : MonoBehaviour
{
    private CircleCollider2D thunderCircleCollider;
    private ThunderAbility abilityComponent;
    private void Start()
    {
        thunderCircleCollider = GetComponent<CircleCollider2D>();
        if(thunderCircleCollider == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到雷圈collider");
        }

        abilityComponent = GameObject.Find("Player").GetComponent<ThunderAbility>();
        if(abilityComponent == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到abilityComponent");
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            abilityComponent.AddInThunderCircleTarget(collision.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            abilityComponent.RemoveInThunderCircleTarget(collision.gameObject);
        }
    }
}
