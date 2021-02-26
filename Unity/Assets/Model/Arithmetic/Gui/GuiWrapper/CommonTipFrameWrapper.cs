using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class CommonTipFrameWrapper : GuiFrameWrapper
{
    private const float splashTime = 1.5f;

    private CommonTipInstance instance;

    private GameObject commonTipPage;
    private GameObject confirmBtn;
    private GameObject cancelBtn;
    private Text message;

    void Start()
    {
        id = GuiFrameID.CommonTipFrame;
        Init();
        instance = GuiController.Instance.CurCommonTipInstance;
        InitBtns();
        message.text = instance.message;
        CommonTool.GuiScale(commonTipPage, canvasGroup, true);
        if (instance.id == CommonTipID.Splash)
        {
            //开启自动关闭
            StartCoroutine(AutoClose());
        }
    }

    protected override void OnStart(Dictionary<string, GameObject> gameObjectDict)
    {
        commonTipPage   = gameObjectDict["CommonTipPage"];
        confirmBtn      = gameObjectDict["ConfirmBtn"];
        cancelBtn       = gameObjectDict["CancelBtn"];
        message         = gameObjectDict["Message"].GetComponent<Text>();
    }

    protected override void OnButtonClick(Button btn)
    {
        base.OnButtonClick(btn);
        switch (btn.name)
        {
            case "ConfirmBtn":
                if (instance.confirmAction != null)
                {
                    instance.confirmAction();
                }
                GuiController.Instance.SwitchWrapper(GuiFrameID.None);
                break;
            case "CancelBtn":
                if (instance.cancelAction != null)
                {
                    instance.cancelAction();
                }
                GuiController.Instance.SwitchWrapper(GuiFrameID.None);
                break;
            default:
                MyDebug.LogYellow("Can not find Button: " + btn.name);
                break;
        }
    }

    private void InitBtns()
    {
        switch (instance.id)
        {
            case CommonTipID.Single:
                cancelBtn.SetActive(false);
                RectTransform confirmBtnRect = confirmBtn.GetComponent<RectTransform>();
                Vector2 anPos = confirmBtnRect.anchoredPosition;
                confirmBtnRect.anchorMax = new Vector2(0.5f, 0);
                confirmBtnRect.anchorMin = new Vector2(0.5f, 0);
                confirmBtnRect.anchoredPosition = new Vector2(0, anPos.y);
                break;
            case CommonTipID.Splash:
                confirmBtn.SetActive(false);
                cancelBtn.SetActive(false);
                break;
            case CommonTipID.Double:
            default:
                break;
        }
    }

    private IEnumerator AutoClose()
    {
        yield return new WaitForSeconds(splashTime);
        GuiController.Instance.SwitchWrapper(GuiFrameID.None);
    }
}

public class CommonTipInstance
{
    public CommonTipID id;
    public string message;
    public Action confirmAction;
    public Action cancelAction;

    public void SetInstance(CommonTipID id, string message, Action confirmAction = null, Action cancelAction = null)
    {
        this.id = id;
        this.message = message;
        this.confirmAction = confirmAction;
        this.cancelAction = cancelAction;
    }
}
