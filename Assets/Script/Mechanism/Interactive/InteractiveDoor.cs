using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractiveDoor : Interactable
{
    public int nextSceneIndex;
    private void Start()
    {
        objectNameOnUI = "Door";        
    }
    public override void Interactive()
    {
        SceneManager.LoadScene(nextSceneIndex);
    }
}
