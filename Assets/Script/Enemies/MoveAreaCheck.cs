using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAreaCheck : MonoBehaviour
{
    public GameObject enemy;
    private MovementEnemies movement;
    private void Start()
    {
        movement = enemy.GetComponent<MovementEnemies>();
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (enemy.name == other.gameObject.name)
        {
            movement.isInMoveArea = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (enemy.name == other.gameObject.name)
        {
            movement.isInMoveArea = false;
        }
    }


}
