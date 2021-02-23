using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveFileFrameWrapper : GuiFrameWrapper
{
    private bool isUploading;
    private bool onlyWrong;
    private bool isBluetooth;
    private List<QuestionInstance> onlyWrongList;

    private SaveFileInstance content;
    private GameObject saveFileDetailBg;
    private GameObject commonResult;
    private GameObject onlyWrongImage;
    private GameObject achievementBtn;
    private GameObject uploadDataBtn;
    private GameObject shareBtn;
    private GameObject bluetoothResult;
    private GameObject bluetoothOnlyWrongImage;
    private Text saveFileDetailTime;
    private Text saveFileDetailAmount;
    private Text saveFileDetailAccuracy;
    private Text saveFileOwnName;
    private Text saveFileOtherName;
    private RectTransform saveFileDetailGrid;

    void Start()
    {
        id = GuiFrameID.SaveFileFrame;
        Init();
        content = RecordController.Instance.CurSaveFileInstance;
        saveFileDetailTime.text = string.Format(saveFileDetailTime.text, content.timeCost.ToString("f1"));
        saveFileDetailAmount.text = string.Format(saveFileDetailAmount.text, content.qInstancList.Count);
        saveFileDetailAccuracy.text = string.Format(saveFileDetailAccuracy.text, content.accuracy.ToString("f1"));
        achievementBtn.SetActive(GuiController.Instance.LastGUI != GuiFrameID.ShareFrame 
                              && !string.IsNullOrEmpty(content.achievementName));
        uploadDataBtn.SetActive(!content.isUpload);
        isBluetooth = !string.IsNullOrEmpty(content.opponentName);
        commonResult.SetActive(!isBluetooth);
        bluetoothResult.SetActive(isBluetooth);
        if (isBluetooth)
        {
            saveFileOwnName.text = GameManager.Instance.UserName;
            saveFileOtherName.text = content.opponentName;
        }
        onlyWrongList = content.qInstancList.FindAll(FindWrong);
        onlyWrong = false;
        RefreshSettlementGrid();
        CommonTool.GuiVerticalMove(saveFileDetailBg, Screen.height, MoveID.LeftOrDown, canvasGroup, true);
    }

    protected override void OnStart(Dictionary<string, GameObject> gameObjectDict)
    {
        saveFileDetailBg                = gameObjectDict["SaveFileDetailBg"];
        commonResult                    = gameObjectDict["CommonResult"];
        onlyWrongImage                  = gameObjectDict["OnlyWrongImage"];
        achievementBtn                  = gameObjectDict["AchievementBtn"];
        uploadDataBtn                   = gameObjectDict["UploadDataBtn"];
        shareBtn                        = gameObjectDict["ShareBtn"];
        bluetoothResult                 = gameObjectDict["BluetoothResult"];
        bluetoothOnlyWrongImage         = gameObjectDict["BluetoothOnlyWrongImage"];
        saveFileDetailTime              = gameObjectDict["SaveFileDetailTime"].GetComponent<Text>();
        saveFileDetailAmount            = gameObjectDict["SaveFileDetailAmount"].GetComponent<Text>();
        saveFileDetailAccuracy          = gameObjectDict["SaveFileDetailAccuracy"].GetComponent<Text>();
        saveFileOwnName                 = gameObjectDict["SaveFileOwnName"].GetComponent<Text>();
        saveFileOtherName               = gameObjectDict["SaveFileOtherName"].GetComponent<Text>();
        saveFileDetailGrid              = gameObjectDict["SaveFileDetailGrid"].GetComponent<RectTransform>();
    }

    protected override void OnButtonClick(Button btn)
    {
        base.OnButtonClick(btn);
        switch (btn.name)
        {
            case "ShareBtn":
                var shareID = GuiController.Instance.LastGUI == GuiFrameID.RankFrame ? ShareID.Rank : ShareID.SaveFile;
                GameManager.Instance.CurShareInstance = new ShareInstance(shareID);
                GuiController.Instance.SwitchWrapper(GuiFrameID.ShareFrame, true);
                break;
            case "BluetoothShareBtn":
                GameManager.Instance.CurShareInstance = new ShareInstance(ShareID.Bluetooth);
                GuiController.Instance.SwitchWrapper(GuiFrameID.ShareFrame, true);
                break;
            case "OnlyWrongBtn":
            case "BluetoothOnlyWrongBtn":
                onlyWrong = !onlyWrong;
                RefreshSettlementGrid();
                break;
            case "SaveFileDetailClosedBtn":
                CommonTool.GuiVerticalMove(saveFileDetailBg, Screen.height, MoveID.LeftOrDown, canvasGroup, false, () => GuiController.Instance.SwitchWrapper(GuiFrameID.None));
                break;
            case "AchievementBtn":
                AchievementInstance instance = AchievementController.Instance.GetAchievement(content.achievementName);
                AchievementController.Instance.CurAchievementInstance = instance;
                GameManager.Instance.CurShareInstance = new ShareInstance(ShareID.Achievement);
                GuiController.Instance.SwitchWrapper(GuiFrameID.ShareFrame, true);
                break;
			case "UploadDataBtn":
                if (isUploading)
                {
                    return;
                }

				if(string.IsNullOrEmpty(GameManager.Instance.UserName))
				{
                    GuiController.Instance.SwitchWrapper(GuiFrameID.NameBoardFrame, true);
					return;
				}

                if (content.isUpload)
                {
                    string message = LanguageController.Instance.GetLanguage("Text_90008");
                    GuiController.Instance.CurCommonTipInstance = new CommonTipInstance(CommonTipID.Splash, message);
                    GuiController.Instance.SwitchWrapper(GuiFrameID.CommonTipFrame, true);
                    return;
                }

                isUploading = true;
                break;
            default:
                MyDebug.LogYellow("Can not find Button: " + btn.name);
                break;
        }
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
            dataList = new ArrayList(content.qInstancList);
        }

        if (isBluetooth) CommonTool.RefreshScrollContent(saveFileDetailGrid, dataList, GuiItemID.BluetoothQuestionItem);
        else CommonTool.RefreshScrollContent(saveFileDetailGrid, dataList, GuiItemID.QuestionItem);
    }

    private void OnUploadSucceed(string message)
    {
        isUploading = false;
        content.isUpload = true;
        RecordController.Instance.RefreshRecord(content);
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
