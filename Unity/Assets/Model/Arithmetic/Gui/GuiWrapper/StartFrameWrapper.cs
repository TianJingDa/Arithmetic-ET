using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// 开始界面
/// </summary>
public class StartFrameWrapper : GuiFrameWrapper
{
    void Start()
    {
        id = GuiFrameID.StartFrame;
        Init();

        //string path = Application.dataPath + "/Resources/Layout/Vertical/Default.txt";
        //Dictionary<string, MyRectTransform> dataList = (Dictionary<string, MyRectTransform>)IOHelper.GetDataFromResources(path,typeof(Dictionary<string, MyRectTransform>));//(Dictionary<string, RectTransform>)IOHelper.GetDataFromResources(path, typeof(Dictionary<string, RectTransform>));
    }

    protected override void OnButtonClick(Button btn)
    {
        base.OnButtonClick(btn);
        switch (btn.name)
        {
            case "StatisticsBtn":
                GuiController.Instance.SwitchWrapperWithMove(GuiFrameID.StatisticsFrame, MoveID.LeftOrDown, true);
                break;
            case "CategoryBtn":
                GuiController.Instance.SwitchWrapperWithScale(GuiFrameID.CategoryFrame, true);
                break;
            case "SetUpBtn":
                GuiController.Instance.SwitchWrapperWithMove(GuiFrameID.SetUpFrame, MoveID.RightOrUp, true);
                break;
            case "ChapterBtn":
                GuiController.Instance.SwitchWrapperWithScale(GuiFrameID.ChapterFrame, true);
                break;
            case "BluetoothBtn":
                //if (Application.isEditor) return;
                GuiController.Instance.SwitchWrapperWithScale(GuiFrameID.BluetoothFrame, true);
                break;
			case "RankBtn":
                GuiController.Instance.SwitchWrapperWithScale(GuiFrameID.RankFrame, true);
				break;
            default:
                MyDebug.LogYellow("Can not find Button: " + btn.name);
                break;
        }
    }
}
