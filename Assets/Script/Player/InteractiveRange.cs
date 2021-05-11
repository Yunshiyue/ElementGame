using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveRange : MonoBehaviour
{
    private InteractivePlayer interactivePlayer;
    private void Awake()
    {
        interactivePlayer = GameObject.Find("Player").GetComponent<InteractivePlayer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Interactable target;
        if(collision.TryGetComponent<Interactable>(out target))
        {
            //Debug.Log(target.GetObjectNameOnUI() + "进入交互范围");
            interactivePlayer.AddInteractableAndShowUI(target);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Interactable target;
        if (collision.TryGetComponent<Interactable>(out target))
        {
            Debug.Log(target.GetObjectNameOnUI() + "进入交互范围");
            interactivePlayer.RemoveInteractableAndShowUI(target);
        }
    }
}
