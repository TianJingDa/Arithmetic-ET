using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChapterFrameWrapper : GuiFrameWrapper
{
    private Text chapterStarStatisticsImg_Text;
    private GameObject chapterWin;
    private GameObject chapterTipBg;
    private GameObject chapterDetailWin;
    private RectTransform chapterRect;
    private ChapterItem[] chapterItemList;
    private Dictionary<DifficultyID, List<AchievementInstance>> achievementDict;


    void Start ()
    {
        id = GuiFrameID.ChapterFrame;
        Init();
        chapterItemList = chapterWin.GetComponentsInChildren<ChapterItem>();
        chapterRect = chapterWin.GetComponent<RectTransform>();
        achievementDict = new Dictionary<DifficultyID, List<AchievementInstance>>
                {
                    {DifficultyID.Junior, AchievementController.Instance.GetAchievementByDifficulty((int)DifficultyID.Junior)},
                    {DifficultyID.Medium, AchievementController.Instance.GetAchievementByDifficulty((int)DifficultyID.Medium)},
                    {DifficultyID.Senior, AchievementController.Instance.GetAchievementByDifficulty((int)DifficultyID.Senior)},
                    {DifficultyID.Ultimate, AchievementController.Instance.GetAchievementByDifficulty((int)DifficultyID.Ultimate)},
                };
        chapterStarStatisticsImg_Text.text = string.Format(chapterStarStatisticsImg_Text.text, AchievementController.Instance.CalculateAllStar());
        List<GameObject> lockList = CommonTool.GetGameObjectsContainName(gameObject, "Lock");
        List<Image> classList = CommonTool.GetComponentsContainName<Image>(gameObject, "ClassBtn");
        //lockList[0].SetActive(false);
        for (int i = 1; i < lockList.Count; i++)
        {
            int star = CommonTool.CalculateStar(achievementDict[(DifficultyID)(i - 1)]);
            lockList[i].SetActive(star < 8);
            classList[i].color = star < 8 ? Color.clear : Color.white;
        }
        List<Text> starCountTextList = CommonTool.GetComponentsContainName<Text>(gameObject, "StarCount_Text");
        for(int i = 0; i < starCountTextList.Count; i++)
        {
            int star = CommonTool.CalculateStar(achievementDict[(DifficultyID)i]);
            starCountTextList[i].text = string.Format(starCountTextList[i].text, star);
        }
    }

    protected override void OnStart(Dictionary<string, GameObject> gameObjectDict)
    {
        chapterWin                      = gameObjectDict["ChapterWin"];
        chapterTipBg                    = gameObjectDict["ChapterTipBg"];
        chapterDetailWin                = gameObjectDict["ChapterDetailWin"];
        chapterStarStatisticsImg_Text   = gameObjectDict["ChapterStarStatisticsImg_Text"].GetComponent<Text>();
    }

    protected override void OnButtonClick(Button btn)
    {
        base.OnButtonClick(btn);
        switch (btn.name)
        {
            case "Chapter2StartFrameBtn":
            case "ChapterWin2StartFrameBtn":
                GuiController.Instance.SwitchWrapperWithScale(GuiFrameID.StartFrame, false);
                break;
            case "ChapterWin2ChapterFrameBtn":
				CommonTool.GuiHorizontalMove(chapterWin, Screen.width, MoveID.LeftOrDown, canvasGroup, false);
                break;
            case "ChapterTipBg":
            case "ChapterConfirmBtn":
                chapterTipBg.SetActive(false);
                break;
            case "MediumClassLock":
            case "SeniorClassLock":
            case "UltimateClassLock":
                chapterTipBg.SetActive(true);
                break;
            case "ChapterDetailWin":
                CommonTool.GuiScale(chapterDetailWin, canvasGroup, false);
                break;
            case "JuniorClassBtn":
                chapterRect.anchoredPosition = Vector2.zero;
                chapterWin.SetActive(true);
                InitAllChapterItem(achievementDict[DifficultyID.Junior]);
				CommonTool.GuiHorizontalMove(chapterWin, Screen.width, MoveID.LeftOrDown, canvasGroup, true);
                break;
            case "MediumClassBtn":
                chapterRect.anchoredPosition = Vector2.zero;
                chapterWin.SetActive(true);
                InitAllChapterItem(achievementDict[DifficultyID.Medium]);
				CommonTool.GuiHorizontalMove(chapterWin, Screen.width, MoveID.LeftOrDown, canvasGroup, true);
                break;
            case "SeniorClassBtn":
                chapterRect.anchoredPosition = Vector2.zero;
                chapterWin.SetActive(true);
                InitAllChapterItem(achievementDict[DifficultyID.Senior]);
				CommonTool.GuiHorizontalMove(chapterWin, Screen.width, MoveID.LeftOrDown, canvasGroup, true);
                break;
            case "UltimateClassBtn":
                chapterRect.anchoredPosition = Vector2.zero;
                chapterWin.SetActive(true);
                InitAllChapterItem(achievementDict[DifficultyID.Ultimate]);
				CommonTool.GuiHorizontalMove(chapterWin, Screen.width, MoveID.LeftOrDown, canvasGroup, true);
                break;
            default:
                MyDebug.LogYellow("Can not find Button:" + btn.name);
                break;
        }
    }

    private void InitAllChapterItem(List<AchievementInstance> instanceList)
    {
        for(int i = 0; i < chapterItemList.Length; i++)
        {
            chapterItemList[i].SendMessage("InitItem", instanceList[i]);
            chapterItemList[i].SendMessage("InitDetailWin", chapterDetailWin);
        }
    }
}
