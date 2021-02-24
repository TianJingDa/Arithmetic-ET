using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SummaryAchievementItem : AchievementItem 
{
    protected override void OnStart(Dictionary<string, GameObject> gameObjectDict)
    {
        achievementName = CommonTool.GetComponentContainsName<Text>(gameObject, "AchievementName");
        //achievementType = CommonTool.GetComponentContainsName<Text>(gameObject, "AchievementType");
        //achievementCondition = CommonTool.GetComponentContainsName<Text>(gameObject, "AchievementCondition");
        //achievementName_WithoutAchievement = CommonTool.GetComponentContainsName<Text>(gameObject, "AchievementName_WithoutAchievement");
        achievementItem_WithoutAchievement = CommonTool.GetGameObjectContainsName(gameObject, "AchievementItem_WithoutAchievement");
    }
    protected override void InitPrefabItem(object data)
    {
        content = data as AchievementInstance;
        if (content == null)
        {
            MyDebug.LogYellow("SummaryAchievementInstance is null!!");
            return;
        }

        Init();
        try
        {
            SymbolID symbol = (SymbolID)System.Enum.Parse(typeof(SymbolID), content.achievementName);
            if (!System.Enum.IsDefined(typeof(SymbolID), symbol))
            {
                MyDebug.LogYellow("Symbol is not Defined!!");
                return;
            }
            int countWithAchievement = 0;
            int countOfSymbol = GetAchievementCountBySymbol(symbol, out countWithAchievement);
            bool hasFinish = countWithAchievement == countOfSymbol;
            achievementItem_WithoutAchievement.SetActive(!hasFinish);
            //achievementCondition.text = GameManager.Instance.GetMutiLanguage(content.condition);
            //achievementType.text = string.Format(achievementType.text, countWithAchievement, countOfSymbol);
            achievementName.gameObject.SetActive(hasFinish);
            //achievementName_WithoutAchievement.gameObject.SetActive(!hasFinish);
            if (!hasFinish)
            {
                //achievementType.color = Color.gray;
                //achievementCondition.color = Color.gray;
                //achievementName_WithoutAchievement.color = Color.gray;
            }
            else
            {
                content.finishTime = "HasFinish";
                achievementName.text = LanguageController.Instance.GetLanguage(content.mainTitleIndex);
            }
        }
        catch
        {
            MyDebug.LogYellow("Can not get Symbol!!");
        }
    }
    protected new void OnShortPress()
    {
        if (content == null || string.IsNullOrEmpty(content.finishTime)) return;
        AchievementController.Instance.CurAchievementInstance = content;
        GameManager.Instance.CurShareInstance = new ShareInstance(ShareID.Achievement);
        GuiController.Instance.SwitchWrapper(GuiFrameID.ShareFrame, true);

        //detailWin.SetActive(true);
        //Image achievementDetailImageInStatistics = CommonTool.GetComponentByName<Image>(detailWin, "AchievementDetailImageInStatistics");
        //Text achievementDetailMainTitleInStatistics = CommonTool.GetComponentByName<Text>(detailWin, "AchievementDetailMainTitleInStatistics");
        //Text achievementDetailSubTitleInStatistics = CommonTool.GetComponentByName<Text>(detailWin, "AchievementDetailSubTitleInStatistics");
        //Text achievementDetailFinishTimeInStatistics = CommonTool.GetComponentByName<Text>(detailWin, "AchievementDetailFinishTimeInStatistics");
        //GameObject achievementDetailShareBtnInStatistics = CommonTool.GetGameObjectByName(detailWin, "AchievementDetailShareBtnInStatistics");
        ////GameObject achievementDetailSaveFileBtnInStatistics = CommonTool.GetGameObjectByName(detailWin, "AchievementDetailSaveFileBtnInStatistics");
        ////achievementDetailImageInStatistics.sprite = GameManager.Instance.GetSprite(content.imageIndex);
        //achievementDetailMainTitleInStatistics.text = GameManager.Instance.GetMutiLanguage(content.mainTitleIndex);
        //achievementDetailSubTitleInStatistics.text = GameManager.Instance.GetMutiLanguage(content.subTitleIndex);
        //achievementDetailFinishTimeInStatistics.text = GetFinishTime(content.finishTime);
        //CommonTool.AddEventTriggerListener(achievementDetailShareBtnInStatistics, EventTriggerType.PointerClick, OnShareBtn);
        ////if (achievementDetailSaveFileBtnInStatistics.activeSelf) achievementDetailSaveFileBtnInStatistics.SetActive(false);
    }
    private int GetAchievementCountBySymbol(SymbolID symbol, out int countWithAchievement)
    {
        List<AchievementInstance> instanceList = AchievementController.Instance.GetAchievementsBySymbol(symbol);
        countWithAchievement = 0;
        for (int i = 0; i < instanceList.Count; i++)
        {
            if (string.IsNullOrEmpty(instanceList[i].finishTime))
            {
                continue;
            } 
            countWithAchievement++;
        }
        return instanceList.Count;
    }
}
