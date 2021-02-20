using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ChapterItem: MonoBehaviour
{
    private AchievementInstance content;//详情
    private GameObject detailWin;

    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnItemClick);
    }

    private void InitDetailWin(GameObject detailWin)
    {
        this.detailWin = detailWin;
    }

    private void InitItem(AchievementInstance instance)
    {
        content = instance;
        List<GameObject> stars = CommonTool.GetGameObjectsContainName(gameObject, "Star");
        for (int i = 0; i < stars.Count; i++)
        {
            stars[i].SetActive((i + 1) <= content.star);
        }
        GetComponent<Image>().sprite = SkinController.Instance.GetSprite(content.chapterImageIndex);
    }

    private void OnItemClick()
    {
        detailWin.SetActive(true);
        CommonTool.GuiScale(detailWin, GuiController.Instance.CurCanvasGroup, true);
        Dictionary<string, GameObject> detailWinDict = CommonTool.InitGameObjectDict(detailWin);
        CommonTool.InitText(detailWin);
        CommonTool.InitImage(detailWin);
        Text chapterDetailPattern_Time = detailWinDict["ChapterDetailPattern_Time"].GetComponent<Text>();
        Text chapterDetailPattern_Number = detailWinDict["ChapterDetailPattern_Number"].GetComponent<Text>();
        Text chapterDetailTime = detailWinDict["ChapterDetailTime"].GetComponent<Text>();
        Text chapterDetailNumber = detailWinDict["ChapterDetailNumber"].GetComponent<Text>();
        Text chapterDetailSymbol = detailWinDict["ChapterDetailSymbol"].GetComponent<Text>();
        Text chapterDetailDigit = detailWinDict["ChapterDetailDigit"].GetComponent<Text>();
        Text chapterDetailOperand = detailWinDict["ChapterDetailOperand"].GetComponent<Text>();
        Text chapterDetailOneStarCondition = detailWinDict["ChapterDetailOneStarCondition"].GetComponent<Text>();
        Text chapterDetailTwoStarCondition = detailWinDict["ChapterDetailTwoStarCondition"].GetComponent<Text>();
        Text chapterDetailThreeStarCondition = detailWinDict["ChapterDetailThreeStarCondition"].GetComponent<Text>();
        GameObject chapter2FightFrameBtn = detailWinDict["Chapter2FightFrameBtn"];
        bool isTimePattern = content.cInstance.patternID == PatternID.Time;
        chapterDetailPattern_Time.gameObject.SetActive(isTimePattern);
        chapterDetailPattern_Number.gameObject.SetActive(!isTimePattern);
        chapterDetailTime.gameObject.SetActive(isTimePattern);
        chapterDetailNumber.gameObject.SetActive(!isTimePattern);
        if (isTimePattern)
        {
            int amount = FightController.Instance.GetTimeAmount(content.cInstance.amountID);
            chapterDetailTime.text = string.Format(chapterDetailTime.text, amount);
        }
        else
        {
            int amount = FightController.Instance.GetNumberAmount(content.cInstance.amountID);
            chapterDetailNumber.text = string.Format(chapterDetailNumber.text, amount);
        }
        string symbol = FightController.Instance.GetSymbol(content.cInstance.symbolID);
        chapterDetailSymbol.text = string.Format(chapterDetailSymbol.text, symbol);
        chapterDetailDigit.text = string.Format(chapterDetailDigit.text, (int)(content.cInstance.digitID + 2));
        chapterDetailOperand.text = string.Format(chapterDetailOperand.text, (int)(content.cInstance.operandID + 2));

        InitCondition(chapterDetailOneStarCondition, 1);
        InitCondition(chapterDetailTwoStarCondition, 2);
        InitCondition(chapterDetailThreeStarCondition, 3);

        CommonTool.AddEventTriggerListener(chapter2FightFrameBtn, EventTriggerType.PointerClick, OnFightClick);
    }
    private void InitCondition(Text condition, int starCount)
    {
        string text = LanguageController.Instance.GetLanguage(condition.GetIndex());
        condition.text = string.Format(text, content.accuracy - (3 - starCount) * 5, (content.meanTime * (1 + (3-starCount) * 0.1)).ToString("f1"));
    }
    private void OnFightClick(BaseEventData data)
    {
		GuiController.Instance.CompetitionGUI = GuiFrameID.ChapterFrame;
        FightController.Instance.CurCategoryInstance = content.cInstance;
        GuiController.Instance.SwitchWrapper(GuiFrameID.FightFrame);
    }
}
