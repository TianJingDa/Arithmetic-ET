using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class HiddenAchievementItem : AchievementItem 
{
    protected override void OnStart(Dictionary<string, GameObject> gameObjectDict)
    {
        achievementName = CommonTool.GetComponentContainsName<Text>(gameObject, "AchievementName");
        achievementImage = CommonTool.GetComponentContainsName<Image>(gameObject, "AchievementImage");
        achievementItem_WithoutAchievement = CommonTool.GetGameObjectContainsName(gameObject, "AchievementItem_WithoutAchievement");
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
        //achievementDetailImageInStatistics.sprite = GameManager.Instance.GetSprite(content.imageIndex);
        //achievementDetailMainTitleInStatistics.text = GameManager.Instance.GetMutiLanguage(content.mainTitleIndex);
        //achievementDetailSubTitleInStatistics.text = GameManager.Instance.GetMutiLanguage(content.subTitleIndex);
        //achievementDetailFinishTimeInStatistics.text = GetFinishTime(content.finishTime);
        //achievementDetailShareBtnInStatistics.SetActive(true);
        //CommonTool.AddEventTriggerListener(achievementDetailShareBtnInStatistics, EventTriggerType.PointerClick, OnShareBtn);
    }

}
