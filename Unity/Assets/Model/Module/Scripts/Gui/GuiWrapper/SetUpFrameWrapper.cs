using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using cn.sharesdk.unity3d;
using DG.Tweening;

/// <summary>
/// 设置界面
/// </summary>
public class SetUpFrameWrapper : GuiFrameWrapper
{
    private int tempLanguageID;
    private int tempSkinID;
    private int tempLayoutID;
    private int tempHandednessID;
    private int tempKeyboardID;
    private bool isEnrolling;
    private bool firstInLayout;//用于标识进入布局设置界面
    private string inputNum;

    private List<int> resetTogglesIndexList;
    private List<Vector2> languageTogglesAnchoredPositonList;
    private List<Vector2> skinTogglesAnchoredPositonList;
    private List<System.Action> resetDelegateList;

    private GameObject strategyWin;
    private GameObject languageWin;
    private GameObject skinWin;
    private GameObject layoutWin;
    private GameObject horizontalLayoutTipBg;
    private GameObject resetWin;
    private GameObject feedbackWin;
    private GameObject activityWin;
    private GameObject activityEnrollBoard;
    private GameObject activityEnrollTipBoard;
    private GameObject aboutUsWin;
    private GameObject shareUsWin;
    private GameObject thankDevelopersWin;
    private GameObject thankDevelopersPage;
    private GameObject resetConfirmBg;
    private GameObject resetToggleGroup;
    private GameObject resetTipBg;
    private GameObject resetTipPageTitle_Text_Achievement;
    private GameObject resetTipPageTitle_Text_SaveFile;
    private GameObject layoutV_R_DImg;
    private GameObject layoutV_L_DImg;
    private GameObject layoutH_R_DImg;
    private GameObject layoutH_L_DImg;
    private GameObject layoutV_R_UImg;
    private GameObject layoutV_L_UImg;
    private GameObject layoutH_R_UImg;
    private GameObject layoutH_L_UImg;
    private GameObject activityRankDetailPattern_Time;
    private GameObject activityRankDetailPattern_Number;
    private Button languageApplyBtn;
    private Button skinApplyBtn;
    private Button layoutApplyBtn;
    private Button resetApplyBtn;
    private Button activityEnrollBtn;
    private Button activityRankDetailBtn;
    private Button activityRankDetailBoardBg;
    private Toggle resetTempTgl;
    private ToggleGroup languageToggleGroup;
    private ToggleGroup skinToggleGroup;
    private Dropdown layoutDropdown;
    private Dropdown handednessDropdown;
    private Dropdown keyboardDropdown;
    private InputField activityEnrollBoardInputField;
    private Text activityEnrollTipBoardContent;
    private Text editionImg_Text;
    private Text activityRankDetailAmount;
    private Text activityRankDetailTime;
    private Text activityRankDetailSymbol;
    private Text activityRankDetailDigit;
    private Text activityRankDetailOperand;

    void Start () 
	{
        id = GuiFrameID.SetUpFrame;
        Init();
        activityEnrollBoardInputField.onEndEdit.AddListener(OnEndEdit);
        string version = LanguageController.Instance.GetLanguage("Text_40008");
        editionImg_Text.text = string.Format(version, GameManager.Instance.Version);
        languageTogglesAnchoredPositonList = InitToggleAnchoredPositon(languageToggleGroup);
        skinTogglesAnchoredPositonList = InitToggleAnchoredPositon(skinToggleGroup);
        //activityEnrollBtn.gameObject.SetActive(!RankController.Instance.AlreadyEnroll);
        //activityRankDetailBtn.gameObject.SetActive(RankController.Instance.AlreadyEnroll);
    }

    protected override void OnStart(Dictionary<string, GameObject> gameObjectDict)
    {
        skinWin                                 = gameObjectDict["SkinWin"];
        resetWin                                = gameObjectDict["ResetWin"];
        resetTipBg                              = gameObjectDict["ResetTipBg"];
        layoutWin                               = gameObjectDict["LayoutWin"];
        horizontalLayoutTipBg                   = gameObjectDict["HorizontalLayoutTipBg"];
        activityWin                             = gameObjectDict["ActivityWin"];
        activityEnrollBoard                     = gameObjectDict["ActivityEnrollBoard"];
        activityEnrollTipBoard                  = gameObjectDict["ActivityEnrollTipBoard"];
        aboutUsWin                              = gameObjectDict["AboutUsWin"];
        shareUsWin                              = gameObjectDict["ShareUsWin"];
        feedbackWin                             = gameObjectDict["FeedbackWin"];
        languageWin                             = gameObjectDict["LanguageWin"];
        strategyWin                             = gameObjectDict["StrategyWin"];
        resetConfirmBg                          = gameObjectDict["ResetConfirmBg"];
        resetToggleGroup                        = gameObjectDict["ResetToggleGroup"];
        thankDevelopersWin                      = gameObjectDict["ThankDevelopersWin"];
        thankDevelopersPage                     = gameObjectDict["ThankDevelopersPage"];
        resetTipPageTitle_Text_Achievement      = gameObjectDict["ResetTipPageTitle_Text_Achievement"];
        resetTipPageTitle_Text_SaveFile         = gameObjectDict["ResetTipPageTitle_Text_SaveFile"];
        layoutV_R_DImg                          = gameObjectDict["LayoutV_R_DImg"];
        layoutV_L_DImg                          = gameObjectDict["LayoutV_L_DImg"];
        layoutH_R_DImg                          = gameObjectDict["LayoutH_R_DImg"];
        layoutH_L_DImg                          = gameObjectDict["LayoutH_L_DImg"];
        layoutV_R_UImg                          = gameObjectDict["LayoutV_R_UImg"];
        layoutV_L_UImg                          = gameObjectDict["LayoutV_L_UImg"];
        layoutH_R_UImg                          = gameObjectDict["LayoutH_R_UImg"];
        layoutH_L_UImg                          = gameObjectDict["LayoutH_L_UImg"];
        activityRankDetailPattern_Time          = gameObjectDict["ActivityRankDetailPattern_Time"];
        activityRankDetailPattern_Number        = gameObjectDict["ActivityRankDetailPattern_Number"];
        languageApplyBtn                        = gameObjectDict["LanguageApplyBtn"].GetComponent<Button>();
        skinApplyBtn                            = gameObjectDict["SkinApplyBtn"].GetComponent<Button>();
        layoutApplyBtn                          = gameObjectDict["LayoutApplyBtn"].GetComponent<Button>();
        resetApplyBtn                           = gameObjectDict["ResetApplyBtn"].GetComponent<Button>();
        activityEnrollBtn                       = gameObjectDict["ActivityEnrollBtn"].GetComponent<Button>();
        activityRankDetailBtn                   = gameObjectDict["ActivityRankDetailBtn"].GetComponent<Button>();
        activityRankDetailBoardBg               = gameObjectDict["ActivityRankDetailBoardBg"].GetComponent<Button>();
        languageToggleGroup                     = gameObjectDict["LanguageToggleGroup"].GetComponent<ToggleGroup>();
        skinToggleGroup                         = gameObjectDict["SkinToggleGroup"].GetComponent<ToggleGroup>();
        layoutDropdown                          = gameObjectDict["LayoutDropdown"].GetComponent<Dropdown>();
        handednessDropdown                      = gameObjectDict["HandednessDropdown"].GetComponent<Dropdown>();
        keyboardDropdown                        = gameObjectDict["KeyboardDropdown"].GetComponent<Dropdown>();
        activityEnrollBoardInputField           = gameObjectDict["ActivityEnrollBoardInputField"].GetComponent<InputField>();
        activityEnrollTipBoardContent           = gameObjectDict["ActivityEnrollTipBoardContent"].GetComponent<Text>();
        editionImg_Text                         = gameObjectDict["EditionImg_Text"].GetComponent<Text>();
        activityRankDetailAmount                = gameObjectDict["ActivityRankDetailAmount"].GetComponent<Text>();
        activityRankDetailTime                  = gameObjectDict["ActivityRankDetailTime"].GetComponent<Text>();
        activityRankDetailSymbol                = gameObjectDict["ActivityRankDetailSymbol"].GetComponent<Text>();
        activityRankDetailDigit                 = gameObjectDict["ActivityRankDetailDigit"].GetComponent<Text>();
        activityRankDetailOperand               = gameObjectDict["ActivityRankDetailOperand"].GetComponent<Text>();
    }

    /// <summary>
    /// 刷新toggles的排列位置
    /// </summary>
    /// <param name="toggleGroup"></param>
    /// <param name="togglesAnchoredPositon"></param>
    /// <param name="curID"></param>
    private void RefreshToggleGroup(ToggleGroup toggleGroup, List<Vector2> togglesAnchoredPositon, int curID)
    {
        if (!toggleGroup)
        {
            MyDebug.LogYellow("toggleGroup is null!");
            return;
        }
        if (togglesAnchoredPositon == null || togglesAnchoredPositon.Count == 0)
        {
            MyDebug.LogYellow("togglesAnchoredPositon NO data!");
            return;
        }

        toggleGroup.SetAllTogglesOff();//待优化
        List<Vector2> tempTogglesAnchoredPositon = new List<Vector2>(togglesAnchoredPositon);
        Vector2 curToggleAnchoredPositon = tempTogglesAnchoredPositon[0];
        tempTogglesAnchoredPositon.RemoveAt(0);
        tempTogglesAnchoredPositon.Insert(curID, curToggleAnchoredPositon);
        for (int i = 0; i < toggleGroup.transform.childCount; i++)
        {
            RectTransform toggleRectTransform = toggleGroup.transform.GetChild(i) as RectTransform;
            toggleRectTransform.anchoredPosition = tempTogglesAnchoredPositon[i];
        }
        toggleGroup.ActiveToggles().ElementAt(curID).isOn = true;//待优化
    }
    /// <summary>
    /// 记录toggles初始的位置
    /// </summary>
    /// <param name="toggleGroup"></param>
    /// <returns></returns>
    private List<Vector2> InitToggleAnchoredPositon(ToggleGroup toggleGroup)
    {
        if (!toggleGroup)
        {
            MyDebug.LogYellow("toggleGroup is null");
            return null;
        }
        List<Vector2> toggleAnchoredPositon = new List<Vector2>();
        for(int i=0;i< toggleGroup.transform.childCount; i++)
        {
            RectTransform toggleRectTransform = toggleGroup.transform.GetChild(i) as RectTransform;
            toggleAnchoredPositon.Add(toggleRectTransform.anchoredPosition);
        }
        return toggleAnchoredPositon;
    }
    /// <summary>
    /// 刷新Dropdown的状态
    /// </summary>
    /// <param name="dpd"></param>
    /// <param name="index"></param>
    private void RefreshDropdown(Dropdown dpd, int index)
    {
        if (!dpd)
        {
            MyDebug.LogYellow("Dropdown is null!");
            return;
        }
        for(int i = 0; i < dpd.options.Count; i++)
        {
            dpd.options[i].text = LanguageController.Instance.GetLanguage(dpd.options[i].text);
        }
        dpd.value = index;
        dpd.RefreshShownValue();
    }
    /// <summary>
    /// 刷新重置界面
    /// </summary>
    private void RefreshResetWin()
    {
        for(int i = 0; i < resetToggleGroup.transform.childCount; i++)
        {
            resetToggleGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = false;
        }
        resetTogglesIndexList = new List<int>();
        if (resetDelegateList == null)
        {
            resetDelegateList = new List<System.Action>();
            //resetDelegateList.Add(ResetTotalTime);
            //resetDelegateList.Add(ResetTotalGame);
            resetDelegateList.Add(ResetAchievement);
            resetDelegateList.Add(ResetSaveFile);
            resetDelegateList.Add(ResetSetUp);
        }
    }
    /// <summary>
    /// 重置游戏时间
    /// </summary>
    private void ResetTotalTime()
    {
        PlayerPrefs.DeleteKey("TotalTime");
    }
    /// <summary>
    /// 重置游戏次数
    /// </summary>
    private void ResetTotalGame()
    {
        PlayerPrefs.DeleteKey("TotalGame");
    }
    /// <summary>
    /// 重置游戏成就
    /// </summary>
    private void ResetAchievement()
    {
        List<string> fileNameList = AchievementController.Instance.GetAllFileNameWithAchievement();
        RecordController.Instance.DeleteRecords(fileNameList);
        AchievementController.Instance.DeleteAllAchievement();
    }
    /// <summary>
    /// 重置所有游戏存档
    /// </summary>
    private void ResetSaveFile()
    {
        RecordController.Instance.DeleteAllRecords();
        AchievementController.Instance.DeleteAllAchievement();
    }
    /// <summary>
    /// 重置所有不带成就的游戏存档
    /// </summary>
    //private void ResetSaveFileWithoutAchievement()
    //{
    //    GameManager.Instance.ResetSaveFileWithoutAchievement();
    //}
    /// <summary>
    /// 重置游戏设置
    /// </summary>
    private void ResetSetUp()
    {
        PlayerPrefs.DeleteKey("LanguageID");
        PlayerPrefs.DeleteKey("SkinID");
        PlayerPrefs.DeleteKey("LayoutID");
        PlayerPrefs.DeleteKey("HandednessID");
        RefreshGui();
    }

    private void OnEndEdit(string text)
    {
        inputNum = text;
    }

    protected override void OnButtonClick(Button btn)
    {
        base.OnButtonClick(btn);
        switch (btn.name)
        {
            case "AboutUsBtn":
                aboutUsWin.SetActive(true);
                CommonTool.GuiHorizontalMove(aboutUsWin, Screen.width, MoveID.RightOrUp, canvasGroup, true);
                break;
            case "AboutUs2SetUpFrameBtn":
                CommonTool.GuiHorizontalMove(aboutUsWin, Screen.width, MoveID.RightOrUp, canvasGroup, false);
                break;
            case "ActivityBtn":
                activityWin.SetActive(true);
                CommonTool.GuiHorizontalMove(activityWin, Screen.width, MoveID.RightOrUp, canvasGroup, true);
                break;
            case "Activity2SetUpFrameBtn":
                CommonTool.GuiHorizontalMove(activityWin, Screen.width, MoveID.RightOrUp, canvasGroup, false);
                break;
            case "ActivityEnrollBtn":
                //if (RankController.Instance.AlreadyEnroll)
                //{
                //    string message = LanguageController.Instance.GetLanguage("Text_40058");
                //    GuiController.Instance.CurCommonTipInstance = new CommonTipInstance(CommonTipID.Splash, message);
                //    GuiController.Instance.SwitchWrapper(GuiFrameID.CommonTipFrame, true);
                //    return;
                //}

                //if (string.IsNullOrEmpty(GameManager.Instance.UserName))
                //{
                //    GuiController.Instance.SwitchWrapper(GuiFrameID.NameBoardFrame, true);
                //    return;
                //}

                //inputNum = "";
                //activityEnrollBoardInputField.text = "";
                //activityEnrollBoard.SetActive(true);
                break;
            case "ActivityEnrollBoardInputFieldConfirmBtn":
                if (string.IsNullOrEmpty(inputNum)) return;
                activityEnrollTipBoard.SetActive(true);
                string curName = LanguageController.Instance.GetLanguage(activityEnrollTipBoardContent.GetIndex());
                activityEnrollTipBoardContent.text = string.Format(curName, inputNum);
                break;
            case "ActivityEnrollBoardInputFieldCancelBtn":
                activityEnrollBoard.SetActive(false);
                break;
            case "ActivityEnrollTipBoardConfirmBtn":
                //if (isEnrolling)
                //{
                //    return;
                //}

                //isEnrolling = true;
                //WWWForm form = new WWWForm();
                //form.AddField("userId", GameManager.Instance.UserID);
                //form.AddField("jwttoken", GameManager.Instance.Token);
                //form.AddField("info", inputNum);
                //GameManager.Instance.EnrollActivity(form, OnEnrollSucceed, OnEnrollFail);
                break;
            case "ActivityEnrollTipBoardCancelBtn":
                activityEnrollTipBoard.SetActive(false);
                break;
            case "ActivityRankDetailBtn":
                OpenRankDetailBoard();
                break;
            case "ActivityRankDetailBoardBg":
                activityRankDetailBoardBg.gameObject.SetActive(false);
                break;
            case "FeedbackBtn":
            case "FeedbackWin":
                feedbackWin.SetActive(!feedbackWin.activeSelf);
                break;
            case "LanguageBtn":
                tempLanguageID = -1;
                languageWin.SetActive(true);
                RefreshToggleGroup(languageToggleGroup, languageTogglesAnchoredPositonList, (int)LanguageController.Instance.CurLanguageID);
                languageApplyBtn.interactable = false;
                CommonTool.GuiHorizontalMove(languageWin, Screen.width, MoveID.RightOrUp, canvasGroup, true);
                break;
            case "Language2SetUpFrameBtn":
                CommonTool.GuiHorizontalMove(languageWin, Screen.width, MoveID.RightOrUp, canvasGroup, false);
                break;
            case "LanguageApplyBtn":
                LanguageController.Instance.CurLanguageID = (LanguageID)tempLanguageID;
                RefreshToggleGroup(languageToggleGroup, languageTogglesAnchoredPositonList, (int)LanguageController.Instance.CurLanguageID);
                languageApplyBtn.interactable = false;
                RefreshGui();
                break;
            case "LayoutBtn":
                RectTransform layoutRect = layoutWin.GetComponent<RectTransform>();
                if (layoutRect) layoutRect.anchoredPosition = Vector2.zero;
                firstInLayout = true;
                tempLayoutID = (int)LayoutController.Instance.CurLayoutID;
                tempHandednessID = (int)LayoutController.Instance.CurHandednessID;
                tempKeyboardID = (int)LayoutController.Instance.CurKeyboardID;
                layoutWin.SetActive(true);
                RefreshDropdown(layoutDropdown, tempLayoutID);
                RefreshDropdown(handednessDropdown, tempHandednessID);
                RefreshDropdown(keyboardDropdown, tempKeyboardID);
                RefreshLayoutSketch();
                layoutApplyBtn.interactable = false;
                firstInLayout = false;
                CommonTool.GuiHorizontalMove(layoutWin, Screen.width, MoveID.RightOrUp, canvasGroup, true);
                break;
            case "HorizontalLayoutTipConfirmBtn":
                horizontalLayoutTipBg.SetActive(false);
                break;
            case "Layout2SetUpFrameBtn":
                CommonTool.GuiHorizontalMove(layoutWin, Screen.width, MoveID.RightOrUp, canvasGroup, false);
                break;
            case "LayoutApplyBtn":
                LayoutController.Instance.CurLayoutID = (LayoutID)tempLayoutID;
                LayoutController.Instance.CurHandednessID = (HandednessID)tempHandednessID;
                LayoutController.Instance.CurKeyboardID = (KeyboardID)tempKeyboardID;
                layoutApplyBtn.interactable = false;
                break;
            case "AboutUs2StartFrameBtn":
            case "Activity2StartFrameBtn":
            case "Language2StartFrameBtn":
            case "Layout2StartFrameBtn":
            case "Reset2StartFrameBtn":
            case "SetUp2StartFrameBtn":
            case "Skin2StartFrameBtn":
            case "Strategy2StartFrameBtn":
                GuiController.Instance.SwitchWrapperWithMove(GuiFrameID.StartFrame, MoveID.RightOrUp, false);
                break;
            case "ShareUsBtn":
                shareUsWin.SetActive(true);
                ShowShareUsWin();
                break;
            case "ShareUsWin":
                shareUsWin.SetActive(false);
                break;
            case "ResetBtn":
                RectTransform resetRect = resetWin.GetComponent<RectTransform>();
                if (resetRect) resetRect.anchoredPosition = Vector2.zero;
                resetWin.SetActive(true);
                RefreshResetWin();
                CommonTool.GuiHorizontalMove(resetWin, Screen.width, MoveID.RightOrUp, canvasGroup, true);
                break;
            case "Reset2SetUpFrameBtn":
                CommonTool.GuiHorizontalMove(resetWin, Screen.width, MoveID.RightOrUp, canvasGroup, false);
                break;
            case "ResetApplyBtn":
                resetConfirmBg.SetActive(true);
                break;
            case "ResetCancelBtn":
                resetConfirmBg.SetActive(false);
                break;
            case "ResetConfirmBg":
                resetConfirmBg.SetActive(false);
                break;
            case "ResetConfirmBtn":
                resetConfirmBg.SetActive(false);
                for (int i = 0; i < resetTogglesIndexList.Count; i++)
                {
                    int index = resetTogglesIndexList[i];
                    resetDelegateList[index]();
                }
                RefreshResetWin();
                break;
            case "ResetTipCancelBtn":
                resetTipBg.SetActive(false);
                resetTempTgl.isOn = false;
                break;
            case "ResetTipConfirmBtn":
                resetTipBg.SetActive(false);
                resetApplyBtn.interactable = resetTogglesIndexList.Count != 0;
                break;
            case "SkinBtn":
                tempSkinID = -1;
                skinWin.SetActive(true);
                RefreshToggleGroup(skinToggleGroup, skinTogglesAnchoredPositonList, (int)SkinController.Instance.CurSkinID);
                skinApplyBtn.interactable = false;
                CommonTool.GuiHorizontalMove(skinWin, Screen.width, MoveID.RightOrUp, canvasGroup, true);
                break;
            case "Skin2SetUpFrameBtn":
                CommonTool.GuiHorizontalMove(skinWin, Screen.width, MoveID.RightOrUp, canvasGroup, false);
                break;
            case "SkinApplyBtn":
                SkinController.Instance.CurSkinID = (SkinID)tempSkinID;
                RefreshToggleGroup(skinToggleGroup, skinTogglesAnchoredPositonList, (int)SkinController.Instance.CurSkinID);
                skinApplyBtn.interactable = false;
                RefreshGui();
                break;
            case "StrategyBtn":
                strategyWin.SetActive(true);
                CommonTool.GuiHorizontalMove(strategyWin, Screen.width, MoveID.RightOrUp, canvasGroup, true);
                break;
            case "Strategy2SetUpFrameBtn":
                CommonTool.GuiHorizontalMove(strategyWin, Screen.width, MoveID.RightOrUp, canvasGroup, false);
                break;
            case "ThankDevelopersBtn":
                thankDevelopersWin.SetActive(true);
                thankDevelopersPage.SetActive(true);
                CommonTool.GuiScale(thankDevelopersPage, canvasGroup, true);
                break;
            case "ThankDevelopersWin":
                CommonTool.GuiScale(thankDevelopersPage, canvasGroup, false, () => thankDevelopersWin.SetActive(false));
                break;
            case "WeChatBtnInSetUp":
                GameManager.Instance.ShareUrl(PlatformType.WeChat);
                break;
            case "WeChatMomentsInSetUp":
                GameManager.Instance.ShareUrl(PlatformType.WeChatMoments);
                break;
            case "SinaWeiboBtnInSetUp":
                GameManager.Instance.ShareUrl(PlatformType.SinaWeibo);
                break;
            default:
                MyDebug.LogYellow("Can not find Button: " + btn.name);
                break;
        }
    }

    private void OnEnrollSucceed(CategoryInstance instance)
    {
        //isEnrolling = false;
        //RankController.Instance.AlreadyEnroll = true;
        //activityEnrollBtn.gameObject.SetActive(false);
        //activityRankDetailBtn.gameObject.SetActive(true);
        //activityEnrollBoard.SetActive(false);
        //RankController.Instance.ActivityCategory = instance;
        //OpenRankDetailBoard();

        //string message = LanguageController.Instance.GetLanguage("Text_20080");
        //GuiController.Instance.CurCommonTipInstance = new CommonTipInstance(CommonTipID.Splash, message);
        //GuiController.Instance.SwitchWrapper(GuiFrameID.CommonTipFrame, true);
    }

    private void OnEnrollFail(string message)
    {
        isEnrolling = false;
        GuiController.Instance.CurCommonTipInstance = new CommonTipInstance(CommonTipID.Splash, message);
        GuiController.Instance.SwitchWrapper(GuiFrameID.CommonTipFrame, true);
    }

    private void OpenRankDetailBoard()
    {
        //activityRankDetailBoardBg.gameObject.SetActive(true);
        //CategoryInstance instance = RankController.Instance.ActivityCategory;
        //activityRankDetailPattern_Time.SetActive(instance.patternID == PatternID.Time);
        //activityRankDetailPattern_Number.SetActive(instance.patternID == PatternID.Number);
        //activityRankDetailTime.gameObject.SetActive(instance.patternID == PatternID.Time);
        //activityRankDetailAmount.gameObject.SetActive(instance.patternID == PatternID.Number);
        //activityRankDetailTime.text = string.Format(activityRankDetailTime.text, FightController.Instance.GetTimeAmount(instance.amountID));
        //activityRankDetailAmount.text = string.Format(activityRankDetailAmount.text, FightController.Instance.GetNumberAmount(instance.amountID));
        //activityRankDetailSymbol.text = string.Format(activityRankDetailSymbol.text, FightController.Instance.GetSymbol(instance.symbolID));
        //activityRankDetailDigit.text = string.Format(activityRankDetailDigit.text, (int)(instance.digitID + 2));
        //activityRankDetailOperand.text = string.Format(activityRankDetailOperand.text, (int)(instance.operandID + 2));
    }

    protected override void OnToggleClick(Toggle tgl)
    {
        base.OnToggleClick(tgl);
        if (tgl.name.Contains("LanguageToggle"))
        {
            OnToggleClick(languageApplyBtn, ref tempLanguageID, tgl);
        }
        else if (tgl.name.Contains("SkinToggle"))
        {
            OnToggleClick(skinApplyBtn, ref tempSkinID, tgl);
        }
        else if (tgl.name.Contains("ResetToggle"))
        {
            if (tgl.isOn)
            {
                resetTogglesIndexList.Add(tgl.GetIndex());
                //if (tgl.index == 2 || tgl.index == 3)
                //{
                //    resetTempTgl = tgl;
                //    ShowTip();
                //    return;
                //}
            }
            else
            {
                resetTogglesIndexList.Remove(tgl.GetIndex());
            }
            resetApplyBtn.interactable = resetTogglesIndexList.Count != 0;
        }
    }
    private void OnToggleClick(Button confirmBtn, ref int tempID, Toggle tgl)
    {
        confirmBtn.interactable = true;

        if (tgl.isOn)
        {
            tempID = tgl.GetIndex();
        }
    }
    private void ShowShareUsWin()
    {
        Sequence shareUsSequence = DOTween.Sequence();
        shareUsSequence.Append(shareUsWin.transform.DOLocalMoveX(Screen.width, 0.2f, true).From().SetEase(Ease.OutQuint));
        RectTransform shareUsPage = CommonTool.GetComponentByName<RectTransform>(shareUsWin, "ShareUsPage");
        shareUsSequence.Append(shareUsPage.DOMoveY(shareUsPage.rect.y, 0.2f, true).From());
        shareUsSequence.OnStart(() => canvasGroup.blocksRaycasts = false).
                        OnComplete(() => canvasGroup.blocksRaycasts = true);

    }

    private void ShowTip()
    {
        resetTipBg.SetActive(true);
        resetTipPageTitle_Text_Achievement.SetActive(resetTempTgl.GetIndex() == 2);
        resetTipPageTitle_Text_SaveFile.SetActive(resetTempTgl.GetIndex() == 3);
    }
    protected override void OnDropdownClick(Dropdown dpd)
    {
        base.OnDropdownClick(dpd);
        switch (dpd.name)
        {
            case "LayoutDropdown":
                tempLayoutID = dpd.value;
                horizontalLayoutTipBg.SetActive((LayoutID)tempLayoutID == LayoutID.Horizontal && !firstInLayout);
                break;
            case "HandednessDropdown":
                tempHandednessID = dpd.value;
                break;
            case "KeyboardDropdown":
                tempKeyboardID = dpd.value;
                break;
            default:
                MyDebug.LogYellow("Can not find Dropdown:" + dpd.name);
                break;
        }
        RefreshLayoutSketch();
        layoutApplyBtn.interactable = true;
    }

    private void RefreshLayoutSketch()
    {
        layoutV_R_DImg.SetActive(tempLayoutID == (int)LayoutID.Vertical && tempHandednessID == (int)HandednessID.Right && tempKeyboardID == (int)KeyboardID.Down);
        layoutV_L_DImg.SetActive(tempLayoutID == (int)LayoutID.Vertical && tempHandednessID == (int)HandednessID.Left && tempKeyboardID == (int)KeyboardID.Down);
        layoutH_R_DImg.SetActive(tempLayoutID == (int)LayoutID.Horizontal && tempHandednessID == (int)HandednessID.Right && tempKeyboardID == (int)KeyboardID.Down);
        layoutH_L_DImg.SetActive(tempLayoutID == (int)LayoutID.Horizontal && tempHandednessID == (int)HandednessID.Left && tempKeyboardID == (int)KeyboardID.Down);

        layoutV_R_UImg.SetActive(tempLayoutID == (int)LayoutID.Vertical && tempHandednessID == (int)HandednessID.Right && tempKeyboardID == (int)KeyboardID.Up);
        layoutV_L_UImg.SetActive(tempLayoutID == (int)LayoutID.Vertical && tempHandednessID == (int)HandednessID.Left && tempKeyboardID == (int)KeyboardID.Up);
        layoutH_R_UImg.SetActive(tempLayoutID == (int)LayoutID.Horizontal && tempHandednessID == (int)HandednessID.Right && tempKeyboardID == (int)KeyboardID.Up);
        layoutH_L_UImg.SetActive(tempLayoutID == (int)LayoutID.Horizontal && tempHandednessID == (int)HandednessID.Left && tempKeyboardID == (int)KeyboardID.Up);
    }
    //public override void OnToggleClick(bool check)
    //{
    //    if (check)
    //    {
    //        tempLanguageID = languageToggleGroup.ActiveToggles().FirstOrDefault().index;
    //        Debug.Log("tempLanguageID:" + tempLanguageID);
    //    }
    //}


}
