using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

/// <summary>
/// 结算界面
/// </summary>
public class SettlementFrameWrapper : GuiFrameWrapper
{
    private bool isUploading;
    private bool onlyWrong;
    private bool isBluetooth;

    private GameObject commonResult;
    private GameObject onlyWrongImage;
    private GameObject curAchievementBtn;
    private GameObject bluetoothResult;
    private GameObject bluetoothOnlyWrongImage;
    private GameObject achievementDetailBgInSettlement;
    private GameObject achievementDetailPageInSettlement;
    private Image achievementDetailImageInSettlement;
    private Text settlementTime;
    private Text settlementAmount;
    private Text settlementAccuracy;
    private Text saveFileOwnName;
    private Text saveFileOtherName;
    private Text achievementDetailMainTitleInSettlement;
    private Text achievementDetailSubTitleInSettlement;
    private Text achievementDetailFinishTimeInSettlement;
    private RectTransform settlementGrid;
    private SaveFileInstance curSaveFileInstance;
    private AchievementInstance curAchievementInstance;
    private List<QuestionInstance> onlyWrongList;
    private List<QuestionInstance> allInstanceList;



    void Start () 
	{
        id = GuiFrameID.SettlementFrame;
        Init();
        InitSettlement();
        InitAchievement();
    }

    protected override void OnStart(Dictionary<string, GameObject> gameObjectDict)
    {
        settlementGrid                              = gameObjectDict["SettlementGrid"].GetComponent<RectTransform>();
        settlementTime                              = gameObjectDict["SettlementTime"].GetComponent<Text>();
        settlementAmount                            = gameObjectDict["SettlementAmount"].GetComponent<Text>();
        settlementAccuracy                          = gameObjectDict["SettlementAccuracy"].GetComponent<Text>();
        achievementDetailMainTitleInSettlement      = gameObjectDict["AchievementDetailMainTitleInSettlement"].GetComponent<Text>();
        achievementDetailSubTitleInSettlement       = gameObjectDict["AchievementDetailSubTitleInSettlement"].GetComponent<Text>();
        achievementDetailFinishTimeInSettlement     = gameObjectDict["AchievementDetailFinishTimeInSettlement"].GetComponent<Text>();
        saveFileOwnName                             = gameObjectDict["SaveFileOwnName"].GetComponent<Text>();
        saveFileOtherName                           = gameObjectDict["SaveFileOtherName"].GetComponent<Text>();
        achievementDetailBgInSettlement             = gameObjectDict["AchievementDetailBgInSettlement"];
        achievementDetailImageInSettlement          = gameObjectDict["AchievementDetailImageInSettlement"].GetComponent<Image>();
        commonResult                                = gameObjectDict["CommonResult"];
        onlyWrongImage                              = gameObjectDict["OnlyWrongImage"];
        curAchievementBtn                           = gameObjectDict["CurAchievementBtn"];
        bluetoothResult                             = gameObjectDict["BluetoothResult"];
        bluetoothOnlyWrongImage                     = gameObjectDict["BluetoothOnlyWrongImage"];
        achievementDetailPageInSettlement           = gameObjectDict["AchievementDetailPageInSettlement"];
    }

    protected override void OnButtonClick(Button btn)
    {
        base.OnButtonClick(btn);
        switch (btn.name)
        {
            case "AchievementDetailBgInSettlement":
                CommonTool.GuiScale(achievementDetailPageInSettlement, canvasGroup, false,()=> achievementDetailBgInSettlement.SetActive(false));
                break;
            case "OnlyWrongBtn":
            case "BluetoothOnlyWrongBtn":
                onlyWrong = !onlyWrong;
                RefreshSettlementGrid();
                break;
            case "Settlement2CategoryFrameBtn":
                GuiController.Instance.SwitchWrapperWithMove(GuiController.Instance.CompetitionGUI,MoveID.RightOrUp,false);
                break;
            case "Settlement2StartFrameBtn":
                GuiController.Instance.SwitchWrapperWithScale(GuiFrameID.StartFrame,false);
                break;
            case "SettlementShareBtn":
                GameManager.Instance.CurShareInstance = new ShareInstance(ShareID.SaveFile);
                GuiController.Instance.SwitchWrapper(GuiFrameID.ShareFrame, true);
                break;
            case "BluetoothShareBtn":
                GameManager.Instance.CurShareInstance = new ShareInstance(ShareID.Bluetooth);
                GuiController.Instance.SwitchWrapper(GuiFrameID.ShareFrame, true);
                break;
            case "CurAchievementBtn":
                achievementDetailBgInSettlement.SetActive(true);
                achievementDetailPageInSettlement.SetActive(true);
                CommonTool.GuiScale(achievementDetailPageInSettlement, canvasGroup, true);
                break;
            case "UploadDataBtn":
                if (isUploading)
                {
                    return;
                }

                if (string.IsNullOrEmpty(GameManager.Instance.UserName))
                {
                    GuiController.Instance.SwitchWrapper(GuiFrameID.NameBoardFrame, true);
                    return;
                }

                if (curSaveFileInstance.isUpload)
                {
                    string message = LanguageController.Instance.GetLanguage("Text_90008");
                    GuiController.Instance.CurCommonTipInstance = new CommonTipInstance(CommonTipID.Splash, message);
                    GuiController.Instance.SwitchWrapper(GuiFrameID.CommonTipFrame, true);
                    return;
                }

                isUploading = true;
                WWWForm form = new WWWForm();
				form.AddField("userId", GameManager.Instance.UserID);
				form.AddField("jwttoken", GameManager.Instance.Token);
				form.AddField("model", (int)curSaveFileInstance.cInstance.patternID + 1);
				form.AddField("num", (int)curSaveFileInstance.cInstance.amountID + 1);
				form.AddField("calcu", (int)curSaveFileInstance.cInstance.symbolID + 1);
				form.AddField("digit", (int)curSaveFileInstance.cInstance.digitID + 2);
				form.AddField("operate", (int)curSaveFileInstance.cInstance.operandID + 2);
                form.AddField("timelast", RecordController.Instance.FillTimeLast(curSaveFileInstance));
                form.AddField("accuracy", curSaveFileInstance.accuracy.ToString("f1"));
                form.AddField("version", GameManager.Instance.Version);
                curSaveFileInstance.achievementName = "";//上传的战绩都没有成就
                string data = JsonUtility.ToJson(curSaveFileInstance);
                form.AddField("data", data);
                GameManager.Instance.UploadRecord(form, OnUploadSucceed, OnUploadFail);
                break;
            case "AchievementDetailShareBtn":
                AchievementController.Instance.CurAchievementInstance = curAchievementInstance;
                GameManager.Instance.CurShareInstance = new ShareInstance(ShareID.Achievement);
                GuiController.Instance.SwitchWrapper(GuiFrameID.ShareFrame, true);
                break;
            default:
                MyDebug.LogYellow("Can not find Button: " + btn.name);
                break;
        }
    }

    private void InitSettlement()
    {
        curSaveFileInstance = RecordController.Instance.CurSaveFileInstance;
        settlementTime.text = string.Format(settlementTime.text, curSaveFileInstance.timeCost.ToString("f1"));
        settlementAmount.text = string.Format(settlementAmount.text, curSaveFileInstance.qInstancList.Count);
        settlementAccuracy.text = string.Format(settlementAccuracy.text, curSaveFileInstance.accuracy.ToString("f1"));
        isBluetooth = !string.IsNullOrEmpty(curSaveFileInstance.opponentName);
        commonResult.SetActive(!isBluetooth);
        bluetoothResult.SetActive(isBluetooth);
        if (isBluetooth)
        {
            saveFileOwnName.text = GameManager.Instance.UserName;
            saveFileOtherName.text = curSaveFileInstance.opponentName;
        }
        allInstanceList = curSaveFileInstance.qInstancList;
        onlyWrongList = allInstanceList.FindAll(FindWrong);
        onlyWrong = false;
        RefreshSettlementGrid();
    }

    private bool FindWrong(QuestionInstance questionInstance)
    {
        int count = questionInstance.instance.Count;
        return questionInstance.instance[count - 1] != questionInstance.instance[count - 2];
    }

    private void RefreshSettlementGrid()
    {
        onlyWrongImage.SetActive(onlyWrong);
        bluetoothOnlyWrongImage.SetActive(onlyWrong);
        ArrayList dataList;
        if (onlyWrong)
        {
            dataList = new ArrayList(onlyWrongList);
        }
        else
        {
            dataList = new ArrayList(allInstanceList);
        }

        if (isBluetooth) CommonTool.RefreshScrollContent(settlementGrid, dataList, GuiItemID.BluetoothQuestionItem);
        else CommonTool.RefreshScrollContent(settlementGrid, dataList, GuiItemID.QuestionItem);
    }
    private void InitAchievement()
    {
		if (!string.IsNullOrEmpty(AchievementController.Instance.CurAchievementName) && GuiController.Instance.CompetitionGUI == GuiFrameID.ChapterFrame)
        {
            curAchievementBtn.SetActive(true);
            achievementDetailBgInSettlement.SetActive(true);
            achievementDetailPageInSettlement.SetActive(true);
            CommonTool.GuiScale(achievementDetailPageInSettlement, canvasGroup, true);
            curAchievementInstance = AchievementController.Instance.GetCurAchievement();
            achievementDetailImageInSettlement.sprite = SkinController.Instance.GetSprite(curAchievementInstance.imageIndex);
            achievementDetailMainTitleInSettlement.text = LanguageController.Instance.GetLanguage(curAchievementInstance.mainTitleIndex);
            achievementDetailSubTitleInSettlement.text = LanguageController.Instance.GetLanguage(curAchievementInstance.subTitleIndex);
            achievementDetailFinishTimeInSettlement.text = GetFinishTime(curAchievementInstance.finishTime);
        }
        else
        {
            curAchievementBtn.SetActive(false);
        }
    }
    private string GetFinishTime(string time)
    {
        StringBuilder newTime = new StringBuilder(time.Substring(0, 8));
        newTime.Insert(4, ".");
        newTime.Insert(7, ".");
        return newTime.ToString();
    }

    private void OnUploadSucceed(string message)
    {
        isUploading = false;
        curSaveFileInstance.isUpload = true;
        RecordController.Instance.RefreshRecord(curSaveFileInstance);
        GuiController.Instance.CurCommonTipInstance = new CommonTipInstance(CommonTipID.Splash, message);
        GuiController.Instance.SwitchWrapper(GuiFrameID.CommonTipFrame, true);
    }

    private void OnUploadFail(string message)
	{
        isUploading = false;
        GuiController.Instance.CurCommonTipInstance = new CommonTipInstance(CommonTipID.Splash, message);
        GuiController.Instance.SwitchWrapper(GuiFrameID.CommonTipFrame, true);
	}
}
