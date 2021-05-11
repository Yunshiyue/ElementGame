using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class SaveMenu : MonoBehaviour
{
    private SaveManager.SaveMode mode;
    private SaveManager saveManager;
    private Image[] archiveImageUI = new Image[SaveManager.MAX_ARCHIVE_NUMBER + 1];
    private Text[] lastSaveTimeUI = new Text[SaveManager.MAX_ARCHIVE_NUMBER + 1];
    private Text[] playTimeUI = new Text[SaveManager.MAX_ARCHIVE_NUMBER + 1];
    private Text[] placeNameUI = new Text[SaveManager.MAX_ARCHIVE_NUMBER + 1];
    private Text[] searchingRateUI = new Text[SaveManager.MAX_ARCHIVE_NUMBER + 1];
    private Text titleText;
    private void Awake()
    {
        titleText = transform.Find("Title").transform.Find("Text").GetComponent<Text>();
        saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();

        StringBuilder stringBuilder = new StringBuilder("Save");
        Transform temp;
        for(int i = 1; i < SaveManager.MAX_ARCHIVE_NUMBER + 1; i ++)
        {
            stringBuilder.Append(i);
            temp = transform.Find(stringBuilder.ToString());
            archiveImageUI[i] = temp.Find("PlacePhoto").GetComponent<Image>();
            lastSaveTimeUI[i] = temp.Find("SaveTime").GetComponent<Text>();
            playTimeUI[i] = temp.Find("GameTime").GetComponent<Text>();
            placeNameUI[i] = temp.Find("PlaceName").GetComponent<Text>();
            searchingRateUI[i] = temp.Find("Image").transform.Find("SearchingRate").GetComponent<Text>();
            stringBuilder.Remove(4, 1);
        }
    }
    //private void Start()
    //{
    //    RefreshSavePanel(SaveManager.SaveMode.Save);
    //}
    public void RefreshSavePanel(SaveManager.SaveMode mode)
    {
        this.mode = mode;
        saveManager.LoadAllGlobalInfo();
        switch (mode)
        {
            case SaveManager.SaveMode.Save:
                titleText.text = "记载史诗";
                //改变贴图以区分save和load
                break;
            case SaveManager.SaveMode.Load:
                titleText.text = "回想史诗";
                //改变贴图以区分save和load
                break;
        }

        for (int i = 1; i < SaveManager.MAX_ARCHIVE_NUMBER + 1; i++)
        {
            archiveImageUI[i].sprite = saveManager.GetArichiveImage(i);
            lastSaveTimeUI[i].text = saveManager.GetStringDataFromArchive(SaveManager.lastSaveTimeKey, i);
            playTimeUI[i].text = saveManager.GetStringDataFromArchive(SaveManager.playTimeKey, i);
            placeNameUI[i].text = saveManager.GetStringDataFromArchive(SaveManager.placeNameKey, i);
            searchingRateUI[i].text = saveManager.GetStringDataFromArchive(SaveManager.completionRateKey, i);
        }
    }

    public void ChoseArchive(int archiveIndex)
    {
        if (mode == SaveManager.SaveMode.Load)
        {
            if(saveManager.GetArichiveValid(archiveIndex))
            {
                //显示messagebox提升当前存档可能丢失
                saveManager.LoadSceneIntoCurrentContext(archiveIndex);
            }
        }
        else
        {
            if (saveManager.GetArichiveValid(archiveIndex))
            {
                //显示messagebox提示可能覆盖存档
                saveManager.SaveScene(archiveIndex);

            }
            else
            {
                saveManager.SaveScene(archiveIndex);
            }
        }
        RefreshSavePanel(mode);
    }
}
