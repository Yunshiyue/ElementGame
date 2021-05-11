using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAreaCheck : MonoBehaviour
{
    public MovementEnemies movement;
    private void Start()
    {
        //movement = GetComponent<MovementEnemies>();
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            movement.isPlayerInArea = true;
           // movement.enemyAnim.SetBool("idle", false);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            movement.isPlayerInArea = false;
        }
    }


}
