using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class AchievementController : Controller 
{
    #region C#单例
    private static AchievementController instance = null;
    private AchievementController()
    {
        base.id = ControllerID.AchievementController;
        InitAchievementData();
        MyDebug.LogWhite("Loading Controller:" + id.ToString());
    }
    public static AchievementController Instance
    {
        get { return instance ?? (instance = new AchievementController()); }
    }
    #endregion

    private List<AchievementInstance> achievementList;

    public AchievementInstance CurAchievementInstance { get; set; }

    public Action OnAchievementDeleted;

    public string CurAchievementName
    {
        get;
        private set;
    }

    private string LastestAchievement
    {
        get
        {
            string lastestAchievementString = PlayerPrefs.GetString("LastestAchievement", "");
            if (string.IsNullOrEmpty(lastestAchievementString)) return "";
            char[] charSeparators = new char[] { ',' };
            string[] lastestAchievementArray = lastestAchievementString.Split(charSeparators, System.StringSplitOptions.RemoveEmptyEntries);
            if (lastestAchievementArray.Length == 0) return null;
            return lastestAchievementArray[lastestAchievementArray.Length - 1];
        }
        set
        {
            string lastestAchievementString = PlayerPrefs.GetString("LastestAchievement", "") + value + ",";
            PlayerPrefs.SetString("LastestAchievement", lastestAchievementString);
        }
    }

    public bool NewAchievement
    {
        get
        {
            int newAchievement = PlayerPrefs.GetInt("NewAchievement", 0);
            return newAchievement != 0;
        }
        private set
        {
            int newAchievement = value ? 1 : 0;
            PlayerPrefs.SetInt("NewAchievement", newAchievement);
        }
    }

    public bool FinishAllAchievement
    {
        get
        {
            AchievementInstance achievement = achievementList.Find(x => x.cInstance.symbolID >= 0
                                                                     && string.IsNullOrEmpty(x.finishTime));
            return achievement == null;
        }
    }

    private void InitAchievementData()
    {
        string data = CommonTool.GetDataFromResources("Achievement/Achievement");
        achievementList = ETModel.JsonHelper.FromListJson<AchievementInstance>(data);
        WriteAllFinishTime(achievementList);
    }

    public AchievementInstance GetCurAchievement()
    {
        if (string.IsNullOrEmpty(CurAchievementName))
        {
            return new AchievementInstance();
        } 
        return GetAchievement(CurAchievementName);
    }

    public AchievementInstance GetLastestAchievement()
    {
        if (string.IsNullOrEmpty(LastestAchievement))
        {
            return new AchievementInstance();
        } 
        return GetAchievement(LastestAchievement);
    }

    public AchievementInstance GetAchievement(string achievementName)
    {
        return achievementList.Find(x => x.achievementName == achievementName);
    }

    public List<AchievementInstance> GetAchievementsBySymbol(SymbolID symbol)
    {
        return achievementList.FindAll(x => x.cInstance.symbolID == symbol);
    }

    public List<AchievementInstance> GetAchievementByDifficulty(int difficulty)
    {
        return achievementList.FindAll(x => x.difficulty == difficulty);
    }

    public int CalculateAllStar()
    {
        int total = 0;
        for (int i = 0; i < achievementList.Count; i++)
        {
            total += achievementList[i].star;
        }
        return total;
    }

    public bool CheckAchievement(SaveFileInstance instance)
    {
        instance.achievementName = "";
        int star = 0;
        float meanTime = instance.timeCost / instance.qInstancList.Count;
        List<AchievementInstance> achievementList = GetAchievementUnFinish();
        for (int i = 0; i < achievementList.Count; i++)
        {
            if (achievementList[i].cInstance.Equals(instance.cInstance))
            {
                instance.achievementName = achievementList[i].achievementName;
                if (achievementList[i].accuracy <= instance.accuracy && achievementList[i].meanTime >= meanTime)
                {
                    star = 3;
                }
                else if (achievementList[i].accuracy - 5 <= instance.accuracy && achievementList[i].meanTime * 1.1 >= meanTime)
                {
                    star = 2;
                }
                else if (achievementList[i].accuracy - 10 <= instance.accuracy && achievementList[i].meanTime * 1.2 >= meanTime)
                {
                    star = 1;
                }
                break;
            }
        }

        switch (star)
        {
            case 3:
                PlayerPrefs.SetString(instance.achievementName, instance.fileName);
                PlayerPrefs.SetInt(instance.achievementName + "Star", 3);
                WriteFinishTime(instance.achievementName, instance.fileName, 3);
                LastestAchievement = instance.achievementName;
                break;
            case 2:
            case 1:
                PlayerPrefs.SetInt(instance.achievementName + "Star", star);
                WriteFinishTime(instance.achievementName, "", star);
                break;
            default:
                break;
        }

        return star == 3;
    }

    private List<AchievementInstance> GetAchievementUnFinish()
    {
        return achievementList.FindAll(x => x.cInstance.symbolID >= 0 && string.IsNullOrEmpty(x.finishTime));
    }

    public void DeleteAllAchievement()
    {
        PlayerPrefs.DeleteKey("LastestAchievement");

        for (int i = 0; i < achievementList.Count; i++)
        {
            PlayerPrefs.DeleteKey(achievementList[i].achievementName);
            PlayerPrefs.DeleteKey(achievementList[i].achievementName + "Star");
            achievementList[i].star = 0;
            achievementList[i].finishTime = "";
        }
    }
    public void DeleteAchievement(string achievementName, bool withAction = false)
    {
        if (!PlayerPrefs.HasKey(achievementName))
        {
            MyDebug.LogYellow("Wrong AchievementName!");
            return;
        }

        PlayerPrefs.DeleteKey(achievementName);
        PlayerPrefs.DeleteKey(achievementName + "Star");
        WriteFinishTime(achievementName, "", 0);

        string lastestAchievement = PlayerPrefs.GetString("LastestAchievement", "");
        if (lastestAchievement.Contains(achievementName))
        {
            lastestAchievement = lastestAchievement.Replace(achievementName + ",", "");
            PlayerPrefs.SetString("LastestAchievement", lastestAchievement);
        }
        
        if(withAction && OnAchievementDeleted != null)
        {
            OnAchievementDeleted();
        }
    }

    public List<string> GetAllFileNameWithAchievement()
    {
        List<string> fileNameList = new List<string>();
        for (int i = 0; i < achievementList.Count; i++)
        {
            string fileName = PlayerPrefs.GetString(achievementList[i].achievementName, "");
            if (!string.IsNullOrEmpty(fileName))
            {
                fileNameList.Add(fileName);
            } 
        }
        return fileNameList;
    }

    private void WriteFinishTime(string achievementName, string finishTime, int star)
    {
        AchievementInstance instance = achievementList.Find(x => x.achievementName == achievementName);
        if (instance != null)
        {
            instance.finishTime = finishTime;
            instance.star = star;
        } 
    }
    private void WriteAllFinishTime(List<AchievementInstance> instanceList)
    {
        for(int i = 0; i < instanceList.Count; i++)
        {
            instanceList[i].finishTime = PlayerPrefs.GetString(instanceList[i].achievementName, "");
            instanceList[i].star = PlayerPrefs.GetInt(instanceList[i].achievementName + "Star", 0);
        }
    }
}
