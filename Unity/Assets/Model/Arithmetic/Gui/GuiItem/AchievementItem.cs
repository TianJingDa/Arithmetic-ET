using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


public class AchievementItem : Item, IPointerDownHandler, IPointerExitHandler, IPointerClickHandler
{
    protected float durationThreshold = 1.0f;
    protected bool isLongPress;
    protected bool onlyWrong;

    protected AchievementInstance content;//详情
    protected GameObject achievementItem_WithoutAchievement;
    protected Text achievementName;
    protected Image achievementImage;
    protected Vector3 position;

    public void OnPointerDown(PointerEventData eventData)
    {
        isLongPress = false;
        position = ((RectTransform)transform).position;
        StartCoroutine("TimeCounter");
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        StopCoroutine("TimeCounter");
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isLongPress)
        {
            StopCoroutine("TimeCounter");
            OnShortPress();
        }
    }
    protected IEnumerator TimeCounter()
    {
        float duration = 0;
        while (duration < durationThreshold)
        {
            duration += Time.deltaTime;
            yield return null;
        }
        isLongPress = true;
        Vector3 curPosition = ((RectTransform)transform).position;
        float distance = Mathf.Abs(position.y - curPosition.y);
        if (distance <= 2 && isLongPress) OnLongPress();
    }
    protected void OnShortPress()
    {
        if (content == null || string.IsNullOrEmpty(content.finishTime)) return;
        AchievementController.Instance.CurAchievementInstance = content;
        GameManager.Instance.CurShareInstance = new ShareInstance(ShareID.Achievement);
        GuiController.Instance.SwitchWrapper(GuiFrameID.ShareFrame, true);
    }

    protected void OnLongPress()
    {
        if (content == null || string.IsNullOrEmpty(content.finishTime)) return;
        string tip = LanguageController.Instance.GetLanguage("Text_20027");
        GuiController.Instance.CurCommonTipInstance.SetInstance(CommonTipID.Double, tip, OnDeleteConfirmed);
        GuiController.Instance.SwitchWrapper(GuiFrameID.CommonTipFrame, true);
    }

    private void OnDeleteConfirmed()
    {
        RecordController.Instance.DeleteRecord(content.finishTime);
        AchievementController.Instance.DeleteAchievement(content.achievementName, true);
    }

    protected override void OnStart(Dictionary<string, GameObject> gameObjectDict)
    {
        achievementName                     = gameObjectDict["AchievementName"].GetComponent<Text>();
        achievementImage                    = gameObjectDict["AchievementImage"].GetComponent<Image>();
        achievementItem_WithoutAchievement  = gameObjectDict["AchievementItem_WithoutAchievement"];
    }
    protected override void InitPrefabItem(object data)
    {
        content = data as AchievementInstance;
        if (content == null)
        {
            MyDebug.LogYellow("AchievementInstance is null!!");
            return;
        }

        Init();
        bool notHasAchievement = string.IsNullOrEmpty(content.finishTime);
        achievementName.text = LanguageController.Instance.GetLanguage(content.mainTitleIndex);
        achievementItem_WithoutAchievement.SetActive(notHasAchievement);
        if (!notHasAchievement) achievementImage.sprite = SkinController.Instance.GetSprite(content.imageIndex);
        List<GameObject> stars = CommonTool.GetGameObjectsContainName(gameObject, "Star");
        for(int i = 0; i < stars.Count; i++)
        {
            stars[i].SetActive((i + 1) <= content.star);
        }
    }
}
[Serializable]
public class AchievementInstance
{
    public string achievementName;
    public float accuracy;
    public float meanTime;
    public string mainTitleIndex;
    public string subTitleIndex;
    public string imageIndex;
    public string chapterImageIndex;
    public string finishTime;//完成时间
    public CategoryInstance cInstance;
    public int star;
    public int difficulty;
}
