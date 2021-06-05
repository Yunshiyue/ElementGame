using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//该脚本挂在DialogPanel上面，是一个panel
public class DialogSystem : MonoBehaviour
{
    [Header("UI组件")]
    public Text textLabel;
    public Image faceImage;

    [Header("文本文件")]
    public TextAsset DialogContent;//对话内容

    [Header("头像")]
    public Sprite face01, face02;

    [Header("触发事件")]
    public GameObject enemy;
    

    public GameObject NPC;
    private TalkUI talkUI;

    public int index;//对话的行

    public float textSpeed;//文本显示的速度
    

    List<string> textList = new List<string>();//将文件分成行后储存到这个列表里

    bool textFinish;
    bool cancelTyping;//控制文字出现的完成


    // Start is called before the first frame update
    void Awake()
    {
        GetTextContent(DialogContent);
        talkUI = NPC.GetComponent<TalkUI>();
    }
    private void OnEnable()
    {
        //textLabel.text = textList[index];
        //index++;
        textFinish = true;
        StartCoroutine(setTextUI());
    }
    /*Update方法主要是更新检测
     * 是否打完一行字，没打完就按一定的速度打，打完一行字就等待下一次按Q键
     * 下一次按Q键则打下一行字，没按则出于等待的界面
     * 当全部文本呈现完毕后，关闭对话框
     * 关闭语音输入
     */
    // Update is called once per frame
    void Update()
    {
        

        if(Input.GetKeyDown(KeyCode.Q) && index == textList.Count)
        {
            gameObject.SetActive(false);
            index = 0;
            if(talkUI.isEndDialog1 == false)//如果没接任务，触发接任务的对话，结束对话一，激活任务
            {
                enemy.SetActive(true);
                talkUI.isEndDialog1 = true;
                talkUI.isActivateMission = true;
            }


            return;
        }
        

        //if (Input.GetKeyDown(KeyCode.Q) && textFinish)
        //{

        //    StartCoroutine(setTextUI());
        //}

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if(textFinish && !cancelTyping)
            {
                StartCoroutine(setTextUI());
            }
            else if(!textFinish && !cancelTyping)
            {
                cancelTyping = true;
            }
            
        }
    }
    /*
     * 得到文本的内容
     * 首先清空文本文字，将索引清零
     * 将打字的文本按行输入
     */
    void GetTextContent(TextAsset file)
    {
        textList.Clear();
        index = 0;
        var lineData = file.text.Split('\n');

        foreach(var line in lineData)
        {
            textList.Add(line);
        }
    }
    /*
     * 协程打字
     * 读取文本内容，判断说话的人
     * 判断好后给一个头像，把说话内容拆成字，一个个一个打印到对话框来
     * 同时判定是否在打字
     * 如果在打字是否要结束打字（就是快速将一行字呈现出来，而不是等待文字按一定速度显示）
     */
    IEnumerator setTextUI()//进行打字的操作
    {
        textFinish = false;
        textLabel.text = "";

        switch (textList[index])//判断说话的人是主角还是NPC，B是主角说的，A是NPC说的
        {
            case "B":
                
                faceImage.sprite = face01;
                index++;
                break;

            case "A":
                faceImage.sprite = face02;
                index++;
                break;
        }

       
        int letter = 0;
        while(!cancelTyping && letter < textList[index].Length -1)
        {
            textLabel.text += textList[index][letter];
            letter++;
            yield return new WaitForSeconds(textSpeed);
        }
        textLabel.text = textList[index];
        cancelTyping = false;
        textFinish = true;
        index++;

        
    }
}
