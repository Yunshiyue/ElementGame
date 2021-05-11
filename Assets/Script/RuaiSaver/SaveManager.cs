using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class SaveManager : MonoBehaviour
{
    public enum SaveMode { Save, Load };

    public static readonly string lastSceneIndexKey = "LastSceneIndex";
    public static readonly string lastSaveTimeKey = "LastSaveTime";
    public static readonly string playTimeKey = "PlayTime";
    public static readonly string placeNameKey = "LastSavePlaceName";
    public static readonly string completionRateKey = "completionRate";    

    public static readonly string saveFileExtension = ".arch";
    public static readonly string saveImageExtension = ".iarch";
    public static readonly string archivePath = "./Archives/";

    public static readonly int MAX_ARCHIVE_NUMBER = 3;

    //存档信息概览
    private bool[] isArchiveValid = new bool[MAX_ARCHIVE_NUMBER + 1];
    private Dictionary<string, string>[] globalDatas = new Dictionary<string, string>[MAX_ARCHIVE_NUMBER + 1];
    private Sprite[] archiveImage = new Sprite[MAX_ARCHIVE_NUMBER + 1];

    //保存截图信息
    private Camera mainCamera;
    private Image curImageToSave;
    private GameObject UICanvasObject;
    private Vector2 imageSize;

    //当前存档的信息
    private static Dictionary<string, string> sceneData = null;
    private static Dictionary<string, string> globalData = null;

    private System.DateTime archiveOnceStartTime;

    private int curSceneIndex;
    public static int curArchivesIndex = 1;

    //private bool isFirstGame = true;

    //场景切换器
    private SceneSwitcher sceneSwitcher;

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        SaveScene(curArchivesIndex);
    //    }
    //    if (Input.GetKeyDown(KeyCode.O))
    //    {
    //        LoadSceneIntoCurrentContext(curArchivesIndex);
    //    }
    //    if(Input.GetKeyDown(KeyCode.I))
    //    {
    //        SaveScreenCaptureImage();
    //    }
    //}
    private void Awake()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        UICanvasObject = GameObject.Find("UICameraCanvas");

        RectTransform imageTransform = GameObject.Find("SaveMenu").transform.Find("Save1").Find("PlacePhoto").GetComponent<RectTransform>();
        imageSize = imageTransform.offsetMax - imageTransform.offsetMin;

        sceneSwitcher = GameObject.Find("SceneSwitcher").GetComponent<SceneSwitcher>();

        //if (!isFirstGame)
        //{
        //    LoadSceneIntoCurrentContext(curArchivesIndex);
        //}
    }
    private void Start()
    {
        if(globalData != null)
        {
            AfterLoadScene();
        }
    }
    public void LoadAllGlobalInfo()
    {
        for(int i = 1; i < MAX_ARCHIVE_NUMBER + 1; i++)
        {
            isArchiveValid[i] = LoadGlobalFileIntoArray((uint)i);
            LoadArchiveImagesIntoArray(i);
        }
        //Debug.Log(string.Format("是否加载了存档globalArchives:{0} {1} {2}", isArchiveValid[1], isArchiveValid[2], isArchiveValid[3]));
        //Debug.Log(string.Format("是否加载了Image:{0} {1} {2}", archiveImage[1] != null, archiveImage[2] != null, archiveImage[3] != null));
    }
    public bool GetArichiveValid(int archiveIndex)
    {
        return isArchiveValid[archiveIndex];
    }
    public Sprite GetArichiveImage(int i)
    {
        if (isArchiveValid[i])
            return archiveImage[i];
        return null;
    }
    public string GetStringDataFromArchive(string keyInData, int archiveIndex)
    {
        string result;
        if(isArchiveValid[archiveIndex] && globalDatas[archiveIndex].TryGetValue(keyInData, out result))
        {
            return result;
        }
        return null;
    }
    
    public void LoadSceneIntoCurrentContext(int archivesIndex)
    {
        LoadGlobalFileIntoCurrentContext((uint)archivesIndex);
        //Debug.Log(string.Format("globaldata有没有值:{0}", globalData != null));
        //显示切换场景画面
        //开启协程切换场景
        sceneSwitcher.ChangeScene(curSceneIndex);
        //SceneManager.LoadScene(curSceneIndex);
        //AfterLoadScene();
    }

    public void AfterLoadScene()
    {
        Time.timeScale = 1;
        LoadSceneData((uint)curArchivesIndex);
        RestoreClassInScene();
        archiveOnceStartTime = System.DateTime.Now;
    }
    public void SaveScene(int archiveIndex)
    {
        Debug.Log(string.Format("保存Scene{0}", archiveIndex));
        SaveScreenCaptureImage(archiveIndex);
        SaveClassInScene();
        SaveSceneData((uint)archiveIndex);
        SaveGlobalData(archiveIndex);
    }
    private bool LoadGlobalFileIntoCurrentContext(uint archivesIndex)
    {
        try
        {
            //FileStream fs = new FileStream(archivePath + IndexToFileName(archivesIndex + archiveIndexOffset, archiveRightMoveShift) + saveFileExtension, FileMode.Open);
            //StreamReader streamReader = new StreamReader(fs);
            //string str = streamReader.ReadToEnd();
            ////globalData = JsonConvert.DeserializeObject<Dictionary<string, string>>(EncryptProvider.AESDecrypt(str, AESKey));
            //globalData = JsonConvert.DeserializeObject<Dictionary<string, string>>(str);
            curArchivesIndex = (int)archivesIndex;
            globalData = globalDatas[curArchivesIndex];
            Debug.Log(string.Format("存档{0}有没有值:{1}", archivesIndex, globalDatas[archivesIndex] != null));
            Debug.Log(string.Format("globaldata有没有值:{0}", globalData != null));
            string lastSceneIndexStr;
            if (globalData.TryGetValue(lastSceneIndexKey, out lastSceneIndexStr))
            {
                uint lastSceneRealIndex = JsonConvert.DeserializeObject<uint>(lastSceneIndexStr);
                curSceneIndex = (int)lastSceneRealIndex;

                return true;
            }

            Debug.LogError("没有在globalData中找到lastSceneIndexKey这个Key值");
            return false;
        }
        catch(DirectoryNotFoundException)
        {
            Debug.LogError(archivePath + IndexToFileName(archivesIndex, archiveRightMoveShift) + "找不到对应存档文件！游戏异常退出！");
            //返回登录界面
            return false;
        }
    }
    private bool LoadGlobalFileIntoArray(uint archivesIndex)
    {
        try
        {
            FileStream fs = new FileStream(archivePath + IndexToFileName(archivesIndex + archiveIndexOffset, archiveRightMoveShift) + saveFileExtension, FileMode.Open);
            StreamReader streamReader = new StreamReader(fs);
            string str = streamReader.ReadToEnd();
            //globalData = JsonConvert.DeserializeObject<Dictionary<string, string>>(EncryptProvider.AESDecrypt(str, AESKey));
            globalDatas[(int) archivesIndex] = JsonConvert.DeserializeObject<Dictionary<string, string>>(str);

            //string lastSceneIndexStr;
            //if (globalData.TryGetValue(lastSceneIndexKey, out lastSceneIndexStr))
            //{
            //    uint lastSceneRealIndex = JsonConvert.DeserializeObject<uint>(lastSceneIndexStr);
            //    curSceneIndex = (int)lastSceneRealIndex;

            //    return true;
            //}
            streamReader.Close();
            return true;
        }
        catch(FileNotFoundException)
        {
            Debug.LogError(string.Format("存档编号:{0}没有找到！", archivesIndex));
            return false;
        }
    }
    public void SaveScreenCaptureImage(int archiveIndex)
    {
        //将相机渲染到新建的texture上
        RenderTexture targetTexture = new RenderTexture(Screen.width, Screen.height, 0);
        mainCamera.targetTexture = targetTexture;
        mainCamera.Render();

        //将新建的texture设为屏幕的active，并从中读出pixels存到Texture2D中
        RenderTexture.active = targetTexture;
        Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenShot.Apply();

        //以PNG格式储存并写入文件
        byte[] screenShotPngEncoding = screenShot.EncodeToPNG();
        string imageName = archivePath + FileNameToImageName(IndexToFileName((uint)archiveIndex + archiveIndexOffset, archiveRightMoveShift)) + saveImageExtension;
        File.WriteAllBytes(imageName, screenShotPngEncoding);

        //重置camera信息，以及texture.active
        mainCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(targetTexture);

        ////按照当前archiveIndex储存屏幕截图
        //UICanvasObject.SetActive(false);
        //mainCamera.Render();
        //ScreenCapture.CaptureScreenshot(imageName);
        //UICanvasObject.SetActive(true);
        //mainCamera.Render();
    }
    private bool LoadArchiveImagesIntoArray(int archivesIndex)
    {
        try
        {
            //archiveImage[archivesIndex] = (Sprite)Resources.Load(archivePath + imageName + saveImageExtension);
            //解密文件名
            string imageName = FileNameToImageName(IndexToFileName((uint)archivesIndex + archiveIndexOffset, archiveRightMoveShift));

            //将png作为Bytes数组读入
            FileStream fs = new FileStream(archivePath + imageName + saveImageExtension, FileMode.Open, FileAccess.Read);
            byte[] pngBytes = new byte[fs.Length];
            fs.Read(pngBytes, 0, (int)fs.Length);

            //新建2D纹理，其大小为要显示的UI控件的大小，并从bytes中读入图片
            Texture2D texture = new Texture2D(Screen.width, Screen.height);
            texture.LoadImage(pngBytes);

            //新建sprite设置纹理、显示范围和锚点
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            archiveImage[archivesIndex] = sprite;
            Debug.Log(string.Format("找到了存档{0}的图片", archivesIndex));
            
            //释放文件流
            fs.Close();
            fs.Dispose();
            return true;
        }
        catch (DirectoryNotFoundException)
        {
            archiveImage[archivesIndex] = null;
            Debug.Log(string.Format("没有找到存档{0}的图片", archivesIndex));
            return false;
        }
    }
    private bool SaveGlobalData(int archiveIndex)
    {
        Debug.Log("储存到" + archiveIndex);

        FileStream fs = new FileStream(archivePath + IndexToFileName((uint)archiveIndex + archiveIndexOffset, archiveRightMoveShift) + saveFileExtension, FileMode.Create);
        StreamWriter streamWriter = new StreamWriter(fs);
        //正式模式需要删除
        if (globalData != null)
        {
            globalData.Clear();
        }
        else
        {
            globalData = new Dictionary<string, string>();
        }
        globalData.Add(lastSceneIndexKey, JsonConvert.SerializeObject((uint)curSceneIndex));
        globalData.Add(lastSaveTimeKey, JsonConvert.SerializeObject(System.DateTime.Now));

        string lastPlayTimeString;
        System.TimeSpan lastPlayTime;
        if (globalData.TryGetValue(playTimeKey, out lastPlayTimeString))
        {
            lastPlayTime = JsonConvert.DeserializeObject<System.TimeSpan>(lastPlayTimeString);
        }
        else
        {
            lastPlayTime = new System.TimeSpan(0, 0, 0);
        }
        globalData.Add(playTimeKey, JsonConvert.SerializeObject(System.DateTime.Now - archiveOnceStartTime + lastPlayTime));
        globalData.Add(placeNameKey, "无果之地");
        globalData.Add(completionRateKey, "0%");
        //string globalDataStr = JsonConvert.SerializeObject(globalData);
        //string afterEncryption = EncryptProvider.AESEncrypt(globalDataStr, AESKey);
        //streamWriter.Write(EncryptProvider.AESEncrypt(JsonConvert.SerializeObject(globalData), AESKey));
        streamWriter.Write(JsonConvert.SerializeObject(globalData));
        streamWriter.Close();
        return true;
    }
    private bool LoadSceneData(uint archiveIndex)
    {
        Debug.Log(string.Format("globaldata有没有值:{0}", globalData != null));
        if (globalData != null)
        {
            StreamReader streamReader = new StreamReader(archivePath + IndexToFileName((uint)curSceneIndex + sceneIndexOffset + archiveIndex * archiveIndexOffset, sceneRightMoveShift) + saveFileExtension);
            string str = streamReader.ReadToEnd();
            sceneData = JsonConvert.DeserializeObject<Dictionary<string, string>>(str);
            streamReader.Close();
            return true;
        }
        Debug.LogError("globalData == null");
        return false;
    }
    private bool SaveSceneData(uint archiveIndex)
    {
        //debug完成后需要删除

        Debug.Log("当前场景index" + curSceneIndex);
        curSceneIndex = SceneManager.GetActiveScene().buildIndex;
        FileStream fs = new FileStream(archivePath + IndexToFileName((uint)curSceneIndex + sceneIndexOffset + archiveIndex * archiveIndexOffset, sceneRightMoveShift) + saveFileExtension, FileMode.Create);
        StreamWriter streamWriter = new StreamWriter(fs);

        //Debug.Log("写入Scene文件:" + "./ Archives / " + IndexToFileName((uint)curSceneIndex, sceneRightMoveShift) + saveFileExtension + "  内容为：" + sceneContent);

        streamWriter.Write(JsonConvert.SerializeObject(sceneData));
        streamWriter.Close();
        return true;
    }
    private bool RestoreClassInScene()
    {
        if(sceneData != null)
        {
            var allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            ClassSaver[] savers;
            string objectData;
            StringBuilder classIdInDic = new StringBuilder();

            foreach(GameObject gameObject in allGameObjects)
            {
                if(gameObject.scene.isLoaded && (savers = gameObject.GetComponents<ClassSaver>()) != null)
                {
                    for(int i = 0; i < savers.Length; i++)
                    {
                        classIdInDic.Append(gameObject.name);
                        classIdInDic.Append(savers[i].GetID());

                        if (sceneData.TryGetValue(classIdInDic.ToString(), out objectData))
                        {
                            savers[i].LoadClass(objectData);
                        }
                        else
                        {
                            Debug.LogError("没有在存档文件中找到" + classIdInDic.ToString() + "该id");
                        }

                        classIdInDic.Clear();
                    }
                }
            }

            return true;
        }
        return false;
    }
    private void SaveClassInScene()
    {
        var allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        ClassSaver[] savers;
        StringBuilder classIdInDic = new StringBuilder();

        if(sceneData!= null)
        {
            sceneData.Clear();
        }
        else
        {
            sceneData = new Dictionary<string, string>();
        }

        foreach (GameObject gameObject in allGameObjects)
        {
            if (gameObject.scene.isLoaded && (savers = gameObject.GetComponents<ClassSaver>()) != null)
            {
                for(int i = 0; i < savers.Length; i++)
                {
                    classIdInDic.Append(gameObject.name);
                    classIdInDic.Append(savers[i].GetID());
                    Debug.Log("Scene场景数据中加入了键值对：(" + classIdInDic.ToString() + "," + savers[i].SaveClass() + ")");
                    sceneData.Add(classIdInDic.ToString(), savers[i].SaveClass());

                    classIdInDic.Clear();
                }
            }
        }
    }
    private int archiveRightMoveShift = 7;
    private int sceneRightMoveShift = 11;
    private uint xorKey = int.MaxValue - 4054531;
    private uint archiveIndexOffset = 71053;
    private uint sceneIndexOffset = 70415193;
    private string AESKey = "Mymut/G0x0H677AANhdQIEdHP/L9KrAP";
    private char[] encryption0 = { 'b', 'e', 'h', 'n', 'j', 'q', '4', 'y', 'o', 'm',
                                   'l', 'g', 'r', 'f', 'k', 'e', 'v', 'c', 'i', 'p',
                                   'q', 't', 'v', 's', 'a', 'u', 'w', 'x', 'z', '6', 
                                   'd', 'l'};
    private char[] encryption1 = { 'G', 'D', 'M', 'Y', 'X', 'A', 'T', 'C', 'P', 'Z',
                                   'F', 'N', 'O', 'S', 'B', 'U', 'E', 'V', 'K', 'Y',
                                   'R', '0', 'X', 'D', 'H', 'L', 'V', 'Q', 'W', '9',
                                   'I', 'J'};

    private readonly int[] positionOfImageNameExtension = { 5, 18, 13, 7, 31, 29};

    private string FileNameToImageName(string fileName)
    {
        StringBuilder imageStringBuilder = new StringBuilder(fileName);
        char[] fileNameChars = fileName.ToCharArray();
        for(int i = 0; i < positionOfImageNameExtension.Length; i++)
        {
            imageStringBuilder.Append(fileNameChars[positionOfImageNameExtension[i]]);
        }
        return imageStringBuilder.ToString();
    }

    //先将int循环右移7位，根据从低到高
    private string IndexToFileName(uint archivesIndex, int rightShift)
    {
        //Debug.Log("右移之前：" + System.Convert.ToString(archivesIndex, 2));
        uint afterShift = cycleShift(archivesIndex, rightShift, false);
        //Debug.Log("右移之后：" + System.Convert.ToString(afterShift, 2));
        uint afterXor = afterShift ^ xorKey;
        //Debug.Log("异或之后：" + System.Convert.ToString(afterXor, 2));
        StringBuilder builder = new StringBuilder(32);
        for(int i = 0; i < 32; i++)
        {
            uint bit = GetTargetBitInUint(afterXor, i);
            char temp = GetCharInEncryption(bit, i);
            //Debug.Log("第" + i + "位对应的字母为：" + temp);
            builder.Append(temp);
        }
        return builder.ToString();
    }

    private uint GetTargetBitInUint(uint val, int offset)
    {
        //Debug.Log("求第" + offset + "位输入为" + System.Convert.ToString(val, 2));
        uint result = 0;
        result |= val;
        result = result << (32 - offset) >> 31;
        //Debug.Log("第" + offset + "位对应：" + System.Convert.ToString(result, 2));
        return result;
    }
    private char GetCharInEncryption(uint bit, int offset)
    {
        if(bit == 0u)
        {
            return encryption0[offset];
        }
        else if(bit == 1u)
        {
            return encryption1[offset];
        }
        Debug.LogError("移位错误，查询字典的uint并非0\\1！");
        return '\0';
    }
    private uint cycleShift(uint val, int ShiftBit, bool isLeft)
    {
        uint temp = 0;
        uint result = 0;
        temp |= val;
        if(isLeft)
        {
            val <<= ShiftBit;
            temp >>= 32 - ShiftBit;
            result = val | temp;
        }
        else
        {
            val >>= ShiftBit;
            temp <<= 32 - ShiftBit;
            result = val | temp;
        }
        return result;
    }
}
