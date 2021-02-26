using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class NameboardFrameWrapper : GuiFrameWrapper
{
    private string          visitorName;

    private GameObject      nameBoardPage;
    private GameObject      nameTipBoard;

    private Text            nameTipBoardContent;
    private InputField      nameBoardInputField;

    void Start()
    {
        id = GuiFrameID.NameBoardFrame;
        Init();
        nameBoardInputField.onEndEdit.AddListener(value => visitorName = value);
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
                if (string.IsNullOrEmpty(visitorName)) return;
                nameTipBoard.SetActive(true);
                string curName = LanguageController.Instance.GetLanguage(nameTipBoardContent.GetIndex());
                nameTipBoardContent.text = string.Format(curName, visitorName);
                break;
            case "NameBoardInputFieldCancelBtn":
                GuiController.Instance.SwitchWrapper(GuiFrameID.None);
                break;
			case "NameTipBoardConfirmBtn":
                PlayerController.Instance.SetVisitorName(visitorName);
                GuiController.Instance.SwitchWrapper(GuiFrameID.None);
                GuiController.Instance.SwitchWrapper(GuiFrameID.StartFrame);
                break;
            case "NameTipBoardCancelBtn":
                nameTipBoard.SetActive(false);
                break;
            default:
                MyDebug.LogYellow("Can not find Button: " + btn.name);
                break;
        }
    }
}