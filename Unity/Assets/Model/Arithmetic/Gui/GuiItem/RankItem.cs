using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RankItem : Item, IPointerClickHandler
{
    private RankInstance content;

    private Text rankIndex;
    private Text rankUserName;
    private Text rankTimeCost;
    private Text rankAccuracy;

    protected override void InitPrefabItem(object data)
    {
        content = data as RankInstance;
        if (content == null)
        {
            MyDebug.LogYellow("RankInstance is null!!");
            return;
        }

        Init();
		rankIndex.text = content.rank.ToString();
		rankUserName.text = content.name;
        InitRankTimeCost();
        string accuracy = LanguageController.Instance.GetLanguage("Text_90007");
        rankAccuracy.text = string.Format(accuracy, content.accuracy.ToString("f1"));
    }

    private void InitRankTimeCost()
    {
        if (FightController.Instance.CurCategoryInstance.patternID == PatternID.Number)
        {
            string timeCost = LanguageController.Instance.GetLanguage("Text_90006");
            rankTimeCost.text = string.Format(timeCost, content.timelast.ToString("f1"));
        }
        else
        {
            string timeCost = LanguageController.Instance.GetLanguage("Text_90012");
            rankTimeCost.text = string.Format(timeCost, content.timelast.ToString());
        }
    }

    protected override void OnStart(Dictionary<string, GameObject> gameObjectDict)
    {
        rankIndex       = gameObjectDict["RankIndex"].GetComponent<Text>();
        rankUserName    = gameObjectDict["RankUserName"].GetComponent<Text>();
        rankTimeCost    = gameObjectDict["RankTimeCost"].GetComponent<Text>();
        rankAccuracy    = gameObjectDict["RankAccuracy"].GetComponent<Text>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }

	private void OnGetRankDetailSucceed(string data)
	{
		SaveFileInstance instance = JsonUtility.FromJson<SaveFileInstance>(data);
		if(instance != null)
		{
            instance.isUpload = true;
            //RankController.Instance.CurRankInstance = content;
            RecordController.Instance.CurSaveFileInstance = instance;
			GuiController.Instance.SwitchWrapper(GuiFrameID.SaveFileFrame, true);
		}
        else
        {
            string message = LanguageController.Instance.GetLanguage("Text_20066");
            OnGetRankDetailFail(message);
        }
	}

	private void OnGetRankDetailFail(string message)
	{
        GuiController.Instance.CurCommonTipInstance.SetInstance(CommonTipID.Splash, message);
		GuiController.Instance.SwitchWrapper(GuiFrameID.CommonTipFrame, true);
	}
}

[Serializable]
public class RankInstance
{
	public int rank;//排名
	public int id;//用于获取详情
	public string userId;
	public string name;
	public float timelast;
	public float accuracy;
	public float score;
}
