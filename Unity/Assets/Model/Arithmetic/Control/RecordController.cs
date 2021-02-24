using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public sealed class RecordController : Controller 
{
    #region C#单例
    private static RecordController instance = null;
    private RecordController()
    {
        base.id = ControllerID.RecordController;
        saveDir = Application.persistentDataPath + "/Save";
        if (!Directory.Exists(saveDir)) Directory.CreateDirectory(saveDir);
        fileFullName = saveDir + "/{0}.sav";
        InitRecordData();
        MyDebug.LogWhite("Loading Controller:" + id.ToString());
    }
    public static RecordController Instance
    {
        get { return instance ?? (instance = new RecordController()); }
    }
    #endregion

    private readonly string saveDir;
    private readonly string fileFullName;

    public SaveFileInstance CurSaveFileInstance { get; set; }

    public Action OnRecordDeleted;

    public float TotalTime
    {
        get
        {
            float totalTime = PlayerPrefs.GetFloat("TotalTime", 0);
            return totalTime;
        }
        private set
        {
            float totalTime = value;
            PlayerPrefs.SetFloat("TotalTime", totalTime);
        }
    }
    public int TotalGame
    {
        get
        {
            int totalGame = PlayerPrefs.GetInt("TotalGame", 0);
            return totalGame;
        }
        private set
        {
            int totalGame = value;
            PlayerPrefs.SetInt("TotalGame", totalGame);
        }
    }

    public bool NewSaveFile
    {
        get
        {
            int newSaveFile = PlayerPrefs.GetInt("NewSaveFile", 0);
            return newSaveFile != 0;
        }
        private set
        {
            int newSaveFile = value ? 1 : 0;
            PlayerPrefs.SetInt("NewSaveFile", newSaveFile);
        }
    }

    private void InitRecordData()
    {

    }

    public void SaveRecord(SaveFileInstance curSaveFileInstance, List<List<int>> resultList, string symbol, float timeCost)
    {
        curSaveFileInstance.isUpload = false;

        curSaveFileInstance.timeCost = timeCost;

        string finishTime = DateTime.Now.ToString("yyyyMMddHHmmss");
        curSaveFileInstance.fileName = finishTime;

        float accuracy = CalculateAccuracy(resultList);
        curSaveFileInstance.accuracy = accuracy;

        List<QuestionInstance> qInstanceList = ConvertToInstanceList(resultList, symbol);
        curSaveFileInstance.qInstancList = qInstanceList;

        curSaveFileInstance.achievementName = "";

        curSaveFileInstance.opponentName = "";

        CurSaveFileInstance = curSaveFileInstance;
        string toSave = JsonUtility.ToJson(curSaveFileInstance);
        SaveRecord(toSave, finishTime);

        TotalGame++;
        TotalTime += timeCost;
    }

    public void RefreshRecord(SaveFileInstance instance)
    {
        string toSave = JsonUtility.ToJson(instance);
        SaveRecord(toSave, instance.fileName);
    }

    private void SaveRecord(string toSave, string fileName)
    {
        if (!Directory.Exists(saveDir)) Directory.CreateDirectory(saveDir);
        string fullName = string.Format(fileFullName, fileName);
        CommonTool.SetData(fullName, toSave);
    }

    public List<SaveFileInstance> ReadAllRecords()
    {
        List<SaveFileInstance> recordList = new List<SaveFileInstance>();
        string[] fileNames = Directory.GetFiles(saveDir, "*.sav");
        string data;
        for (int i = 0; i < fileNames.Length; i++)
        {
            data = CommonTool.GetDataFromDataPath(fileNames[i]);
            if (string.IsNullOrEmpty(data)) continue;
            SaveFileInstance saveFileInstance = JsonUtility.FromJson<SaveFileInstance>(data);
            recordList.Add(saveFileInstance);
        }
        return recordList;
    }

    public SaveFileInstance ReadRecord(string fileName)
    {
        string fullName = string.Format(fileFullName, fileName);
        string data = CommonTool.GetDataFromDataPath(fullName);
        SaveFileInstance saveFileInstance = JsonUtility.FromJson<SaveFileInstance>(data);
        return saveFileInstance;
    }

    public void DeleteAllRecords()
    {
        string[] fileNames = Directory.GetFiles(saveDir, "*.sav");
        for(int i = 0; i < fileNames.Length; i++)
        {
            File.Delete(fileNames[i]);
        }
    }

    public void DeleteRecords(List<string> fileNameList)
    {
        string[] fileNames = Directory.GetFiles(saveDir, "*.sav");
        for (int i = 0; i < fileNames.Length; i++)
        {
            string fileName = Path.GetFileNameWithoutExtension(fileNames[i]);
            if (fileNameList.Contains(fileName)) File.Delete(fileNames[i]);
        }
    }

    public void DeleteRecord(string fileName, bool withAction = false)
    {
        string fullName = string.Format(fileFullName, fileName);
        if (File.Exists(fullName))
        {
            File.Delete(fullName);
            MyDebug.LogGreen("Delete:" + fileName);

            if (withAction && OnRecordDeleted != null)
            {
                OnRecordDeleted();
            }
        }
        else
        {
            MyDebug.LogYellow("The file does not exist!!!");
        }
    }

    private float CalculateAccuracy(List<List<int>> resultList)
    {
        if(resultList == null || resultList.Count <= 0)
        {
            return 0;
        }
        List<List<int>> rightList = resultList.FindAll(x => x[x.Count - 1] == x[x.Count - 2]);
        float accuracy = (float)rightList.Count * 100 / resultList.Count;
        return accuracy;
    }

    private List<QuestionInstance> ConvertToInstanceList(List<List<int>> resultList, string symbol)
    {
        List<QuestionInstance> qInstanceList = new List<QuestionInstance>();
        string count = resultList.Count.ToString();
        for (int i = 0; i < resultList.Count; i++)
        {
            QuestionInstance questionInstance = new QuestionInstance();
            questionInstance.index = (i + 1).ToString().PadLeft(count.Length, '0');
            questionInstance.symbol = symbol;
            questionInstance.instance = resultList[i];
            qInstanceList.Add(questionInstance);
        }
        return qInstanceList;
    }

    /// <summary>
    /// “TimeLast”字段，在限数模式下传耗时，在限时模式下传答题数
    /// </summary>
    /// <returns></returns>
    public string FillTimeLast(SaveFileInstance content)
    {
        if (content.cInstance.patternID == PatternID.Number)
        {
            return content.timeCost.ToString("f1");
        }
        else
        {
            return content.qInstancList.Count.ToString();
        }
    }
}
