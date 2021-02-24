using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;



public class QuestionItem : Item
{
    private QuestionInstance content;//详情
    private GameObject questionRightAnswerBg;
    private Text questionIndex;
    private Text questionContent;
    private Text questionRightAnswer_Text;

    protected override void OnStart(Dictionary<string, GameObject> gameObjectDict)
    {
        questionRightAnswerBg           = gameObjectDict["QuestionRightAnswerBg"];
        questionIndex                   = gameObjectDict["QuestionIndex"].GetComponent<Text>();
        questionContent                 = gameObjectDict["QuestionContent"].GetComponent<Text>();
        questionRightAnswer_Text        = gameObjectDict["QuestionRightAnswer_Text"].GetComponent<Text>();
    }

    protected override void InitPrefabItem(object data)
    {
        content = data as QuestionInstance;
        if (content == null)
        {
            MyDebug.LogYellow("QuestionInstance is null!!");
            return;
        }
        Init();
        questionIndex.text = content.index + ".";
        int count = content.instance.Count;
        StringBuilder question = new StringBuilder();
        question.Append(content.instance[0].ToString());
        for(int i = 1; i < count - 2; i++)
        {
            question.Append(content.symbol);
            question.Append(content.instance[i].ToString());
        }
        question.Append("=");
        question.Append(content.instance[count - 1].ToString());
        questionContent.text = question.ToString();
        if(content.instance[count - 2] != content.instance[count - 1])
        {
            questionRightAnswerBg.SetActive(true);
            questionRightAnswer_Text.text = content.instance[count - 2].ToString();
            questionContent.color = Color.red;
        }
    }
}
[Serializable]
public class QuestionInstance
{
    public string index;
    public string symbol;
    public List<int> instance;
}
