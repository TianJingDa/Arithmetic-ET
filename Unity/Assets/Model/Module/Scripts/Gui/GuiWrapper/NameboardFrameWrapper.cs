using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class NameboardFrameWrapper : GuiFrameWrapper
{
    private const float     TimeOut = 1f;
#if TEST
    private const string    NameURL = "http://182.92.68.73:8091/changeName";
#else
    private const string    NameURL = "http://47.105.77.226:8091/changeName";
#endif

    private bool            isCreating;
    private string          userName;

    private GameObject      nameBoardPage;
    private GameObject      nameTipBoard;

    private Text            nameTipBoardContent;
    private InputField      nameBoardInputField;

    void Start()
    {
        id = GuiFrameID.NameBoardFrame;
        Init();
        nameBoardInputField.onEndEdit.AddListener(OnEndEdit);
        CommonTool.GuiScale(nameBoardPage, canvasGroup, true);
    }

    protected override void OnStart(Dictionary<string, GameObject> gameObjectDict)
    {
        nameBoardPage           = gameObjectDict["NameBoardPage"];
        nameTipBoard            = gameObjectDict["NameTipBoard"];
        nameTipBoardContent     = gameObjectDict["NameTipBoardContent"].GetComponent<Text>();
        nameBoardInputField     = gameObjectDict["NameBoardInputField"].GetComponent<InputField>();
    }

    protected override void OnButtonClick(Button btn)
    {
        base.OnButtonClick(btn);
        switch (btn.name)
        {
            case "NameBoardInputFieldConfirmBtn":
                if (string.IsNullOrEmpty(userName)) return;
                nameTipBoard.SetActive(true);
                string curName = LanguageController.Instance.GetLanguage(nameTipBoardContent.GetIndex());
                nameTipBoardContent.text = string.Format(curName, userName);
                break;
            case "NameBoardInputFieldCancelBtn":
                GuiController.Instance.SwitchWrapper(GuiFrameID.None);
                break;
			case "NameTipBoardConfirmBtn":
                //OnSilentLoginFail();
                break;
            case "NameTipBoardCancelBtn":
                nameTipBoard.SetActive(false);
                break;
            default:
                MyDebug.LogYellow("Can not find Button: " + btn.name);
                break;
        }
    }

    private void OnEndEdit(string text)
    {
        userName = text;
    }

    private void OnSilentLoginFail(string message)
    {
        GuiController.Instance.CurCommonTipInstance = new CommonTipInstance(CommonTipID.Splash, message);
        GuiController.Instance.SwitchWrapper(GuiFrameID.CommonTipFrame, true);
    }
}