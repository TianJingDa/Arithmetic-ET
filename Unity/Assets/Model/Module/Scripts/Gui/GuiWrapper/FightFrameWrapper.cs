using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;

/// <summary>
/// 答题界面
/// </summary>
public class FightFrameWrapper : GuiFrameWrapper
{
    private int                 countdownTime = 3;
    private float               amount;
    private float               startTime;
    private float               timeCost;
    private bool                order;//true: -->; false: <--
    private string              pattern;
    private string              symbol;
    private StringBuilder       result;
    private StringBuilder       question;

    private GameObject          equalImg;
    private GameObject          giveUpBg;
    private GameObject          countdownBg;
    private GameObject          reverseOrderImage;
    private GameObject          timeMaskImage;
    private Text                timeBtn_Text;
    private Text                resultImg_Text;
    private Text                questionImg_Text;
    private List<GameObject>    countdownNumsList;
    private List<int>           curInstance;
    private List<List<int>>     resultList;


    void Start () 
	{
        id = GuiFrameID.FightFrame;
        Dictionary<string, MyRectTransform> rectTransforms = LayoutController.Instance.GetLayoutData();
        InitLayout(rectTransforms);
        Init();

        timeCost    = 0;
        order       = true;
        result      = new StringBuilder();
        question    = new StringBuilder();
        resultList  = new List<List<int>>();
        equalImg.SetActive(false);
        countdownBg.SetActive(true);
        countdownNumsList = CommonTool.GetGameObjectsContainName(countdownBg, "Countdown_");
        FightController.Instance.GetFightParameter(out pattern, out amount, out symbol);
        FightController.Instance.ResetList();
        ClearAllText();
        StartCoroutine(StartFight());
    }
    
    protected override void OnStart(Dictionary<string, GameObject> gameObjectDict)
    {
        equalImg            			= gameObjectDict["EqualImg"];
        giveUpBg            			= gameObjectDict["GiveUpBg"];
        countdownBg         			= gameObjectDict["CountdownBg"];
        timeMaskImage       			= gameObjectDict["TimeMaskImage"];
        reverseOrderImage   			= gameObjectDict["ReverseOrderImage"];
        timeBtn_Text        			= gameObjectDict["TimeBtn_Text"].GetComponent<Text>();
        resultImg_Text      			= gameObjectDict["ResultImg_Text"].GetComponent<Text>();
        questionImg_Text    			= gameObjectDict["QuestionImg_Text"].GetComponent<Text>();
    }


    protected override void OnButtonClick(Button btn)
    {
        base.OnButtonClick(btn);
        switch (btn.name)
        {
            case "0":
            case "1":
            case "2":
            case "3":
            case "4":
            case "5":
            case "6":
            case "7":
            case "8":
            case "9":
                RefreshResultText(btn.name);
                break;
            case "NextBtn":
                ShowNextQuestion(false);
                break;
            case "ClearBtn":
                ClearResultText();
                break;
            case "OrderBtn":
                ChangeInputOrder();
                break;
            case "TimeBtn":
                ChangeTimeVisibility();
                break;
            case "Fight2SettlementFrameBtn":
                giveUpBg.SetActive(true);
                break;
            case "GiveUpBg":
            case "CancelBtn":
                giveUpBg.SetActive(false);
                break;
			case "ConfirmBtn":
				StopAllCoroutines ();
				CancelInvoke ();
                GuiController.Instance.SwitchWrapperWithMove(GuiController.Instance.CompetitionGUI, MoveID.RightOrUp, false);
	            break;
            default:
                MyDebug.LogYellow("Can not find Button: " + btn.name);
                break;
        }
    }

    private void InitLayout(Dictionary<string, MyRectTransform> transforms)
    {
        Dictionary<string, RectTransform> rectTransformDict = CommonTool.InitRectTransformDict(gameObject);
        RectTransform myTrans;
        foreach (KeyValuePair<string, MyRectTransform> pair in transforms)
        {
            myTrans = rectTransformDict[pair.Key];
            myTrans.pivot = new Vector2(pair.Value.pivot.x, pair.Value.pivot.y);
            myTrans.anchorMax = new Vector2(pair.Value.anchorMax.x, pair.Value.anchorMax.y);
            myTrans.anchorMin = new Vector2(pair.Value.anchorMin.x, pair.Value.anchorMin.y);
            myTrans.offsetMax = new Vector2(pair.Value.offsetMax.x, pair.Value.offsetMax.y);
            myTrans.offsetMin = new Vector2(pair.Value.offsetMin.x, pair.Value.offsetMin.y);
            myTrans.localEulerAngles = new Vector3(pair.Value.localEulerAngles.x, pair.Value.localEulerAngles.y, pair.Value.localEulerAngles.z);
        }

        if(LayoutController.Instance.CurKeyboardID == KeyboardID.Up)
        {
            ExchangePosition(rectTransformDict["1"], rectTransformDict["7"]);
            ExchangePosition(rectTransformDict["2"], rectTransformDict["8"]);
            ExchangePosition(rectTransformDict["3"], rectTransformDict["9"]);
        }
    }

    private void ExchangePosition(RectTransform x, RectTransform y)
    {
        Vector3 pos = x.localPosition;
        x.localPosition = y.localPosition;
        y.localPosition = pos;
    }

    private IEnumerator StartFight()
    {
        while (countdownTime > 0)
        {
            for(int i = 0; i < countdownNumsList.Count; i++)
            {
                countdownNumsList[i].SetActive(i == countdownTime - 1);
            }
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }
        countdownBg.SetActive(false);
        ShowNextQuestion(true);
        equalImg.SetActive(true);
        startTime = Time.realtimeSinceStartup;
        InvokeRepeating(pattern + "Pattern", 0f, 0.1f);
    }

    private void ClearAllText()
    {
        timeBtn_Text.text = string.Empty;
        resultImg_Text.text = string.Empty;
        questionImg_Text.text = string.Empty;
    }

    private void TimePattern()
    {
        timeCost = Time.realtimeSinceStartup - startTime;
        timeBtn_Text.text = (amount - timeCost).ToString("f1") + "s";
        if (amount - timeCost <= 0)
        {
            FightOver();
        }
    }

    private void NumberPattern()
    {
        timeCost = Time.realtimeSinceStartup - startTime;
        timeBtn_Text.text = timeCost.ToString("f1") + "s";
    }

    private void RefreshResultText(string num)
    {
        StringBuilder lastResult = new StringBuilder(result.ToString());
        if (order) result.Append(num);
        else result.Insert(0, num);
        if (long.Parse(result.ToString()) > int.MaxValue) result = lastResult;
        if (result.Length > int.MaxValue.ToString().Length) result = lastResult;
        resultImg_Text.text = result.ToString();
    }

    private void ShowNextQuestion(bool isFirst)
    {
        if (result.Length <= 0 && !isFirst) return;

        if (result.Length > 0)//check
        {
            curInstance.Add(int.Parse(result.ToString()));
            resultList.Add(curInstance);
            if (pattern == "Number" && resultList.Count == amount)
            {
                FightOver();
                return;
            }
        }
        curInstance = FightController.Instance.GetQuestionInstance();
        if (curInstance == null)
        {
            MyDebug.LogYellow("curInstance is NULL!");
            FightOver();
        }
        question.Length = 0;
        question.Append(curInstance[0].ToString());
        for(int i = 1; i < curInstance.Count - 1; i++)
        {
            question.Append(symbol);
            question.Append(curInstance[i].ToString());
        }
        questionImg_Text.text = question.ToString();
        ClearResultText();
    }

    private void ClearResultText()
    {
        resultImg_Text.text = string.Empty;
        result.Length = 0;
    }

    private void ChangeInputOrder()
    {
        order = !order;
        reverseOrderImage.SetActive(!reverseOrderImage.activeSelf);//暂时先这么处理
    }

    private void ChangeTimeVisibility()
    {
        timeMaskImage.SetActive(!timeMaskImage.activeSelf);//暂时先这么处理
    }

    private void FightOver()
    {
        CancelInvoke();
        SaveFileInstance curSaveFileInstance = new SaveFileInstance();
        curSaveFileInstance.cInstance = FightController.Instance.CurCategoryInstance;
        RecordController.Instance.SaveRecord(curSaveFileInstance, resultList, symbol, timeCost);
        if(GuiController.Instance.CompetitionGUI == GuiFrameID.ChapterFrame
        && AchievementController.Instance.CheckAchievement(curSaveFileInstance))
        {
            RecordController.Instance.RefreshRecord(curSaveFileInstance);
        }
        GuiController.Instance.SwitchWrapper(GuiFrameID.SettlementFrame);
    }
}
