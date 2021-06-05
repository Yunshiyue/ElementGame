using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//该脚本挂在NPC上面，用于NPC与玩家的对话
public class TalkUI : MonoBehaviour
{

    public GameObject Dialog1;//对话的内容,第一段对话
    public GameObject Dialog2;//第二段对话内容
    public GameObject Dialog3;//第三段对话内容
    public GameObject Button_trigger;//点击按钮触发对话

    public bool isActivateMission;//是否激活了任务
    public bool isFinishMission;//是否完成了任务

    public bool isEndDialog1;//是否结束对话一
    public bool isEndDialog2;//是否结束对话二
    public bool isEndDialog3;//是否结束对话三

  

    private void OnTriggerEnter2D(Collider2D other)
    {
        Button_trigger.SetActive(true);
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Button_trigger.SetActive(false);
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Button_trigger.activeSelf && Input.GetKeyDown(KeyCode.Q) && !isEndDialog1)//如果对话一没进行过，则进行对话一 
        {
            Dialog1.SetActive(true);
        }
        if (Button_trigger.activeSelf && Input.GetKeyDown(KeyCode.Q) && isEndDialog1 && !isEndDialog2)//接受任务但是没完成则进行这个对话
        {
            Dialog2.SetActive(true);
        }
        if(Button_trigger.activeSelf && Input.GetKeyDown(KeyCode.Q) && isEndDialog1 && isEndDialog2 && !isEndDialog3)//完成任务后进行这个对话
        {
            Dialog3.SetActive(true);
        }
    }
}
