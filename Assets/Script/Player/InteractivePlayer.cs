using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractivePlayer : MonoBehaviour
{
    private List<Interactable> interactableList = new List<Interactable>(16);
    private Interactable closestInteractiable;
    private Text interactiveUI;
    private void Awake()
    {
        interactiveUI = GameObject.Find("InteractableText").GetComponent<Text>();
        interactiveUI.gameObject.SetActive(false);
    }

    public void AddInteractableAndShowUI(Interactable target)
    {
        interactableList.Add(target);
        closestInteractiable = GetClosestTargetInList(interactableList);
        if(closestInteractiable != null)
        {
            interactiveUI.text = closestInteractiable.GetObjectNameOnUI();
            interactiveUI.gameObject.SetActive(true);
        }
    }
    public void RemoveInteractableAndShowUI(Interactable target)
    {
        interactableList.Remove(target);
        closestInteractiable = GetClosestTargetInList(interactableList);
        if (closestInteractiable != null)
        {
            interactiveUI.text = closestInteractiable.GetObjectNameOnUI();
            interactiveUI.gameObject.SetActive(true);
        }
        else
        {
            interactiveUI.gameObject.SetActive(false);
        }
    }
    public void InteractiveWithClosetObject()
    {
        Interactable a = GetClosestTargetInList(interactableList);
        if(a != null)
        {
            a.Interactive();
            interactableList.Remove(a);
        }
    }
    private Interactable GetClosestTargetInList(List<Interactable> gameObjects)
    {
        float minDistance = float.MaxValue;
        float tempDistance;
        Interactable cloestTarget = null;
        foreach (Interactable target in gameObjects)
        {
            tempDistance = Vector2.Distance(target.transform.position, transform.position);
            if (tempDistance < minDistance)
            {
                minDistance = tempDistance;
                cloestTarget = target;
            }
        }
        return cloestTarget;
    }
}
