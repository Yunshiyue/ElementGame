using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTheBoss : MonoBehaviour
{

    public GameObject NPC;
    private TalkUI talker;
    public GameObject Boss;
    // Start is called before the first frame update
    void Start()
    {
        if (NPC != null)//如果有NPC布置了击杀Boss的任务
        {
            //完成任务需求的对象
            talker = NPC.GetComponent<TalkUI>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(NPC != null)
        {
            if (talker.isFinishMission)
            {
                Boss.SetActive(true);
            }
        }
    }
}
