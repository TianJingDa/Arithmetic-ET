using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LastestAchievementItem : AchievementItem, IPointerDownHandler, IPointerExitHandler
{
    protected override void OnStart(Dictionary<string, GameObject> gameObjectDict)
    {
        achievementName = CommonTool.GetComponentContainsName<Text>(gameObject, "AchievementName");
        achievementImage = CommonTool.GetComponentContainsName<Image>(gameObject, "AchievementImage");
        achievementItem_WithoutAchievement = CommonTool.GetGameObjectContainsName(gameObject, "AchievementItem_WithoutAchievement");
    }
    protected override void InitPrefabItem(object data)
    {
        content = data as AchievementInstance;
        if (content == null)
        {
            MyDebug.LogYellow("LastestAchievementInstance is null!!");
            return;
        }

        Init();
        bool hasLastestAchievement = !string.IsNullOrEmpty(content.achievementName);
        achievementName.gameObject.SetActive(hasLastestAchievement);
        achievementItem_WithoutAchievement.SetActive(!hasLastestAchievement);
        GameObject lastestAchievementName_WithoutAchievement = CommonTool.GetGameObjectByName(gameObject, "LastestAchievementName_WithoutAchievement");
        lastestAchievementName_WithoutAchievement.SetActive(!hasLastestAchievement);
        if (hasLastestAchievement)
        {
            achievementName.text = LanguageController.Instance.GetLanguage(content.mainTitleIndex);
            achievementImage.sprite = SkinController.Instance.GetSprite(content.imageIndex);
        }
    }

    public new void OnPointerDown(PointerEventData eventData) { }
    public new void OnPointerExit(PointerEventData eventData) { }
}
