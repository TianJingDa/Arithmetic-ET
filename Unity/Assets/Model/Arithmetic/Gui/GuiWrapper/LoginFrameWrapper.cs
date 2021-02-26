using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ETModel;
using System.Text.RegularExpressions;

public class LoginFrameWrapper : GuiFrameWrapper
{
    private GameObject registerPage;

    private InputField accountInputField;
    private InputField passwordInputField;
    private InputField playerNameInputField;
    private InputField newAccountInputField;
    private InputField newPasswordInputField;
    private InputField againPasswordInputField;

    private bool logining;
    private bool registering;
    private string account;
    private string password;
    private string playerName;
    private string newAccount;
    private string newPassword;
    private string againPassword;

    private Regex accountReg = new Regex(@"^[a-z0-9]{6,}$");
    private Regex passwordReg = new Regex(@"^[A-Za-z0-9]{6,}$");

    void Start()
    {
        id = GuiFrameID.LoginFrame;
        Init();

        accountInputField.onEndEdit.AddListener(value => account = value);
        passwordInputField.onEndEdit.AddListener(value => password = value);
        playerNameInputField.onEndEdit.AddListener(value => playerName = value);
        newAccountInputField.onEndEdit.AddListener(value => newAccount = value);
        newPasswordInputField.onEndEdit.AddListener(value => newPassword = value);
        againPasswordInputField.onEndEdit.AddListener(value => againPassword = value);
    }

    protected override void OnStart(Dictionary<string, GameObject> gameObjectDict)
    {
        registerPage = gameObjectDict["RegisterPage"];

        accountInputField = gameObjectDict["AccountInputField"].GetComponent<InputField>();
        passwordInputField = gameObjectDict["PasswordInputField"].GetComponent<InputField>();
        playerNameInputField = gameObjectDict["PlayerNameInputField"].GetComponent<InputField>();
        newAccountInputField = gameObjectDict["NewAccountInputField"].GetComponent<InputField>();
        newPasswordInputField = gameObjectDict["NewPasswordInputField"].GetComponent<InputField>();
        againPasswordInputField = gameObjectDict["AgainPasswordInputField"].GetComponent<InputField>();
    }

    protected override void OnButtonClick(Button btn)
    {
        base.OnButtonClick(btn);

        switch (btn.name)
        {
            case "LoginBtn":
                if (!logining)
                {
                    OnLoginBtnClicked();
                }
                break;
            case "RegisterBtn":
                registerPage.SetActive(true);
                playerName = "";
                newAccount = "";
                newPassword = "";
                againPassword = "";
                break;
            case "VisitorBtn":
                if (string.IsNullOrEmpty(PlayerController.Instance.PlayerName))
                {
                    GuiController.Instance.SwitchWrapper(GuiFrameID.NameBoardFrame, true);
                }
                else
                {
                    GuiController.Instance.SwitchWrapper(GuiFrameID.StartFrame);
                }
                break;
            case "ConfirmBtn":
                if (!registering)
                {
                    OnRegisterConfirmed();
                }
                break;
            case "CancelBtn":
                registerPage.SetActive(false);
                break;
            default:
                MyDebug.LogYellow("Can not find Button: " + btn.name);
                break;
        }
    }

    private async void OnLoginBtnClicked()
    {
        if(string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
        {
            string message = LanguageController.Instance.GetLanguage("Text_11009");
            GuiController.Instance.CurCommonTipInstance.SetInstance(CommonTipID.Splash, message);
            GuiController.Instance.SwitchWrapper(GuiFrameID.CommonTipFrame, true);
            return;
        }

        if (!accountReg.IsMatch(account) || !passwordReg.IsMatch(password))
        {
            string message = LanguageController.Instance.GetLanguage("Text_11010");
            GuiController.Instance.CurCommonTipInstance.SetInstance(CommonTipID.Splash, message);
            GuiController.Instance.SwitchWrapper(GuiFrameID.CommonTipFrame, true);
            return;
        }

        logining = true;
        int code = await NetworkController.Instance.LoginAsync(account, password);
        switch (code)
        {
            case ErrorCode.ERR_Success:
                PlayerInfo info = await NetworkController.Instance.GetPlayerInfo();
                PlayerController.Instance.SetPlayerInfo(info);
                GuiController.Instance.SwitchWrapper(GuiFrameID.StartFrame);
                break;
            case ErrorCode.ERR_AccountOrPasswordError:
                string message = LanguageController.Instance.GetLanguage("Text_11010");
                GuiController.Instance.CurCommonTipInstance.SetInstance(CommonTipID.Splash, message);
                GuiController.Instance.SwitchWrapper(GuiFrameID.CommonTipFrame, true);
                break;
            default:
                MyDebug.LogYellow($"Login Error: {code}");
                break;
        }
        logining = false;
    }

    private async void OnRegisterConfirmed()
    {
        if (string.IsNullOrEmpty(playerName))
        {
            string message = LanguageController.Instance.GetLanguage("Text_11011");
            GuiController.Instance.CurCommonTipInstance.SetInstance(CommonTipID.Splash, message);
            GuiController.Instance.SwitchWrapper(GuiFrameID.CommonTipFrame, true);
            return;
        }

        if (string.IsNullOrEmpty(newAccount) || string.IsNullOrEmpty(newPassword))
        {
            string message = LanguageController.Instance.GetLanguage("Text_11012");
            GuiController.Instance.CurCommonTipInstance.SetInstance(CommonTipID.Splash, message);
            GuiController.Instance.SwitchWrapper(GuiFrameID.CommonTipFrame, true);
            return;
        }

        if (!string.Equals(newPassword, againPassword))
        {
            string message = LanguageController.Instance.GetLanguage("Text_11013");
            GuiController.Instance.CurCommonTipInstance.SetInstance(CommonTipID.Splash, message);
            GuiController.Instance.SwitchWrapper(GuiFrameID.CommonTipFrame, true);
            return;
        }

        if (!accountReg.IsMatch(newAccount) || !passwordReg.IsMatch(newPassword))
        {
            string message = LanguageController.Instance.GetLanguage("Text_11017");
            GuiController.Instance.CurCommonTipInstance.SetInstance(CommonTipID.Splash, message);
            GuiController.Instance.SwitchWrapper(GuiFrameID.CommonTipFrame, true);
            return;
        }

        registering = true;
        int code = await NetworkController.Instance.RegisterAsync(playerName, newAccount, newPassword);
        switch (code)
        {
            case ErrorCode.ERR_Success:
                string rMessage = LanguageController.Instance.GetLanguage("Text_11016");
                GuiController.Instance.CurCommonTipInstance.SetInstance(CommonTipID.Splash, rMessage);
                GuiController.Instance.SwitchWrapper(GuiFrameID.CommonTipFrame, true);
                registerPage.SetActive(false);
                break;
            case ErrorCode.ERR_AccountRepeat:
                string aMessage = LanguageController.Instance.GetLanguage("Text_11014");
                GuiController.Instance.CurCommonTipInstance.SetInstance(CommonTipID.Splash, aMessage);
                GuiController.Instance.SwitchWrapper(GuiFrameID.CommonTipFrame, true);
                break;
            case ErrorCode.ERR_PlayerNameRepeat:
                string pMessage = LanguageController.Instance.GetLanguage("Text_11015");
                GuiController.Instance.CurCommonTipInstance.SetInstance(CommonTipID.Splash, pMessage);
                GuiController.Instance.SwitchWrapper(GuiFrameID.CommonTipFrame, true);
                break;
            default:
                MyDebug.LogYellow($"Register Error: {code}");
                break;
        }
        registering = false;
    }
}
