using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 所有GUI显示层的基类，先用数据初始化再找物体
/// </summary>
public abstract class GuiFrameWrapper : MonoBehaviour
{
    [HideInInspector]
    public GuiFrameID id;

    protected CanvasGroup canvasGroup;
    
    protected void Init()
    {
        CommonTool.InitText(gameObject);
        CommonTool.InitImage(gameObject);
        canvasGroup = GetComponent<CanvasGroup>();
        InitButton(OnButtonClick);
        InitToggle(OnToggleClick);
        InitDropdown(OnDropdownClick);
        var dict = CommonTool.InitGameObjectDict(gameObject);
        OnStart(dict);
    }

    protected void RefreshGui()
    {
        CommonTool.InitText(gameObject);
        CommonTool.InitImage(gameObject);
    }

    private void InitButton(Action<Button> btnDelegate)
    {
        Button[] buttonArray = GetComponentsInChildren<Button>(true);
        for(int i = 0; i < buttonArray.Length; i++)
        {
            Button curButton = buttonArray[i];
            curButton.onClick.AddListener(() => btnDelegate(curButton));
        }
    }
    private void InitToggle(Action<Toggle> tglDelegate)
    {
        Toggle[] toggleArray = GetComponentsInChildren<Toggle>(true);
        for(int i = 0; i < toggleArray.Length; i++)
        {
            Toggle curToggle = toggleArray[i];
            curToggle.onValueChanged.AddListener(value => tglDelegate(curToggle));
        }
    }
    private void InitDropdown(Action<Dropdown> dpdDelegate)
    {
        Dropdown[] dropdownArray = GetComponentsInChildren<Dropdown>(true);
        for(int i = 0; i < dropdownArray.Length; i++)
        {
            Dropdown curDropdown = dropdownArray[i];
            curDropdown.onValueChanged.AddListener(index => dpdDelegate(curDropdown));
        }
    }

    protected virtual void OnStart(Dictionary<string, GameObject> gameObjectDict)
    {

    }
    protected virtual void OnButtonClick(Button btn)
    {
        if (!btn)
        {
            MyDebug.LogYellow("Button is NULL!");
            return;
        }
    }
    protected virtual void OnToggleClick(Toggle tgl)
    {
        if (!tgl)
        {
            MyDebug.LogYellow("Toggle is NULL!");
            return;
        }
    }
    protected virtual void OnDropdownClick(Dropdown dpd)
    {
        if (!dpd)
        {
            MyDebug.LogYellow("Dropdown is NULL!");
            return;
        }
    }
}

