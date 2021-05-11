using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;

public class StatisticsCollector : MonoBehaviour, ClassSaver
{
    public static int[] deathNumberInScenes = new int[5];
    public static int deadBySlimer = 0;
    public static int deadByMinotaur = 0;
    public static int deadByWitch = 0;
    public static int deadByFalling = 0;
    public static int deadByYourself = 0;
    public static int hitBySlimer = 0;
    public static int hitByMinotaur = 0;
    public static int hitByWitch = 0;
    public static int fireElementConsumption = 0;
    public static int thunderElementConsumption = 0;
    public static int iceElementConsumption = 0;
    public static int windElementConsumption = 0;
    public static int MSpellTime = 0;
    public static int MASpellTime = 0;
    public static int MBSpellTime = 0;
    public static int ASpellTime = 0;
    public static int BSpellTime = 0;
    public static int MABSpellTime = 0;
    public static int jumpNumber = 0;

    public static int shortSpellTime = 0;
    private static int fullySpellTime = 0;
    private static int wantFullyButFailedTimeForShortPressedTime = 0;
    public static int wantFullyButFailedTimeForElementNotEnough = 0;
    public static int wantFullyButFailedTimeForBeInterrupted = 0;

    private readonly static int maxReciteTimeSize = 1024;
    private static float[] reciteTime = new float[maxReciteTimeSize];
    private static System.DateTime[] recitTimeStamp = new System.DateTime[maxReciteTimeSize];
    private static int reciteTimePointer;


    private readonly static int maxSolverPuzzleTimeSize = 128;
    private static float[] solvePuzzleTime = new float[maxSolverPuzzleTimeSize];
    private static int solvePuzzleTimePointer = 0;

    public static readonly float wantFullyButFailedTimeThreshold = ElementAbilityManager.fullyCastTime * 0.8f;

    private float[] passSceneTimes;
    private float passSceneTime = 0;
    private void OnEnable()
    {
        passSceneTime = Time.realtimeSinceStartup;
        passSceneTimes = new float[SceneManager.sceneCount];
    }
    private void RecordPassSceneTime()
    {
        float lastMinTime = passSceneTimes[SceneManager.GetActiveScene().buildIndex];
        float thisTime = Time.realtimeSinceStartup - passSceneTime;
        passSceneTimes[SceneManager.GetActiveScene().buildIndex] = lastMinTime < thisTime ? lastMinTime : thisTime;
    }
    public static void RecordSpell(float spellTime)
    {
        if(spellTime < wantFullyButFailedTimeThreshold && spellTime > 0)
        {
            shortSpellTime++;
        }
        else
        {
            if(spellTime < ElementAbilityManager.fullyCastTime)
            {
                wantFullyButFailedTimeForShortPressedTime++;
            }
            else
            {
                fullySpellTime++;
            }
            reciteTime[reciteTimePointer] = spellTime;
            recitTimeStamp[reciteTimePointer] = System.DateTime.Now;
            reciteTimePointer++;

            if(reciteTimePointer >= maxReciteTimeSize)
            {
                SaveReciteTimeAndClearArray();
            }
        }
    }

    private readonly static string filePath = "./Statistics/";
    private readonly static string reciteTimeSaveFileName = "ReciteTimeArchive";
    private readonly static string archiveFileExtension = ".arch";
    private static void SaveReciteTimeAndClearArray()
    {
        FileStream fs = new FileStream(filePath + reciteTimeSaveFileName + SaveManager.curArchivesIndex + archiveFileExtension, FileMode.Append, FileAccess.Write);
        StreamWriter streamWriter = new StreamWriter(fs);

        StringBuilder stringBuilder = new StringBuilder();
        for(int i = 0; i < reciteTimePointer; i++)
        {
            stringBuilder.Append(reciteTime[i].ToString());
            stringBuilder.Append("\t");
            stringBuilder.Append(recitTimeStamp[i].ToString());
            streamWriter.WriteLine(stringBuilder.ToString());
            stringBuilder.Clear();
        }
        reciteTime = new float[maxReciteTimeSize];
        recitTimeStamp = new System.DateTime[maxReciteTimeSize];
        reciteTimePointer = 0;
        streamWriter.Flush();
        fs.Close();
    }

    public void LoadClass(string content)
    {
        ClassSaveHelper helper = new ClassSaveHelper();
        helper.LoadClassJsonString(content);
        helper.LoadValue(nameof(deathNumberInScenes), out deathNumberInScenes);
        helper.LoadValue(nameof(deadBySlimer), out deadBySlimer);
        helper.LoadValue(nameof(deadByMinotaur), out deadByMinotaur);
        helper.LoadValue(nameof(deadByWitch), out deadByWitch);
        helper.LoadValue(nameof(deadByFalling), out deadByFalling);
        helper.LoadValue(nameof(deadByYourself), out deadByYourself);
        helper.LoadValue(nameof(hitBySlimer), out hitBySlimer);
        helper.LoadValue(nameof(hitByMinotaur), out hitByMinotaur);
        helper.LoadValue(nameof(hitByWitch), out hitByWitch);
        helper.LoadValue(nameof(fireElementConsumption), out fireElementConsumption);
        helper.LoadValue(nameof(thunderElementConsumption), out thunderElementConsumption);
        helper.LoadValue(nameof(iceElementConsumption), out iceElementConsumption);
        helper.LoadValue(nameof(windElementConsumption), out windElementConsumption);
        helper.LoadValue(nameof(passSceneTimes), out passSceneTimes);
        helper.LoadValue(nameof(MSpellTime), out MSpellTime);
        helper.LoadValue(nameof(MASpellTime), out MASpellTime);
        helper.LoadValue(nameof(MBSpellTime), out MBSpellTime);
        helper.LoadValue(nameof(ASpellTime), out ASpellTime);
        helper.LoadValue(nameof(BSpellTime), out BSpellTime);
        helper.LoadValue(nameof(MABSpellTime), out MABSpellTime);
        helper.LoadValue(nameof(jumpNumber), out jumpNumber);
        helper.LoadValue(nameof(shortSpellTime), out shortSpellTime);
        helper.LoadValue(nameof(fullySpellTime), out fullySpellTime);
        helper.LoadValue(nameof(wantFullyButFailedTimeForShortPressedTime), out wantFullyButFailedTimeForShortPressedTime);
        helper.LoadValue(nameof(wantFullyButFailedTimeForElementNotEnough), out wantFullyButFailedTimeForElementNotEnough);
        helper.LoadValue(nameof(wantFullyButFailedTimeForBeInterrupted), out wantFullyButFailedTimeForBeInterrupted);
    }
    public string SaveClass()
    {
        SaveReciteTimeAndClearArray();
        RecordPassSceneTime();
        ClassSaveHelper helper = new ClassSaveHelper();
        helper.SaveValue(nameof(deathNumberInScenes), deathNumberInScenes);
        helper.SaveValue(nameof(deadBySlimer), deadBySlimer);
        helper.SaveValue(nameof(deadByMinotaur), deadByMinotaur);
        helper.SaveValue(nameof(deadByWitch), deadByWitch);
        helper.SaveValue(nameof(deadByFalling), deadByFalling);
        helper.SaveValue(nameof(deadByYourself), deadByYourself);
        helper.SaveValue(nameof(hitBySlimer), hitBySlimer);
        helper.SaveValue(nameof(hitByMinotaur), hitByMinotaur);
        helper.SaveValue(nameof(hitByWitch), hitByWitch);
        helper.SaveValue(nameof(fireElementConsumption), fireElementConsumption);
        helper.SaveValue(nameof(thunderElementConsumption), thunderElementConsumption);
        helper.SaveValue(nameof(iceElementConsumption), iceElementConsumption);
        helper.SaveValue(nameof(windElementConsumption), windElementConsumption);
        helper.SaveValue(nameof(passSceneTimes), passSceneTimes);
        helper.SaveValue(nameof(MSpellTime), MSpellTime);
        helper.SaveValue(nameof(MASpellTime), MASpellTime);
        helper.SaveValue(nameof(MBSpellTime), MBSpellTime);
        helper.SaveValue(nameof(ASpellTime), ASpellTime);
        helper.SaveValue(nameof(BSpellTime), BSpellTime);
        helper.SaveValue(nameof(MABSpellTime), MABSpellTime);
        helper.SaveValue(nameof(jumpNumber), jumpNumber);
        helper.SaveValue(nameof(shortSpellTime), shortSpellTime);
        helper.SaveValue(nameof(fullySpellTime), fullySpellTime);
        helper.SaveValue(nameof(wantFullyButFailedTimeForShortPressedTime), wantFullyButFailedTimeForShortPressedTime);
        helper.SaveValue(nameof(wantFullyButFailedTimeForElementNotEnough), wantFullyButFailedTimeForElementNotEnough);
        helper.SaveValue(nameof(wantFullyButFailedTimeForBeInterrupted), wantFullyButFailedTimeForBeInterrupted);
        return helper.GetJsonString();
    }

    public string GetID()
    {
        return nameof(StatisticsCollector);
    }
}
