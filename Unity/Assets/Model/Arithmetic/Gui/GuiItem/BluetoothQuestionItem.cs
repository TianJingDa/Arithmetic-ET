using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class BluetoothQuestionItem : Item
{
    private QuestionInstance content;//详情
    private Image questionOwnAnswerPage;
    private Image questionOtherAnswerPage;
    private Text questionOwnAnswer_Text;
    private Text questionOtherAnswer_Text;
    private Text questionIndex;
    private Text questionContent;


    protected override void OnStart(Dictionary<string, GameObject> gameObjectDict)
    {
        questionOwnAnswerPage           = gameObjectDict["QuestionOwnAnswerPage"].GetComponent<Image>();
        questionOtherAnswerPage         = gameObjectDict["QuestionOtherAnswerPage"].GetComponent<Image>();
        questionOwnAnswer_Text          = gameObjectDict["QuestionOwnAnswer_Text"].GetComponent<Text>();
        questionOtherAnswer_Text        = gameObjectDict["QuestionOtherAnswer_Text"].GetComponent<Text>();
        questionIndex                   = gameObjectDict["QuestionIndex"].GetComponent<Text>();
        questionContent                 = gameObjectDict["QuestionContent"].GetComponent<Text>();
    }

    protected override void InitPrefabItem(object data)
    {
        content = data as QuestionInstance;
        if (content == null)
        {
            MyDebug.LogYellow("BluetoothQuestionInstance is null!!");
            return;
        }
        Init();
        questionIndex.text = content.index + ".";
        int count = content.instance.Count;
        StringBuilder question = new StringBuilder();
        question.Append(content.instance[0].ToString());
        for (int i = 1; i < count - 3; i++)
        {
            question.Append(content.symbol);
            question.Append(content.instance[i].ToString());
        }
        questionContent.text = question.ToString();

        if(content.instance[count - 1] == 0)
        {
            questionOwnAnswer_Text.text = "";
            questionOwnAnswerPage.color = Color.yellow;
        }
        else
        {
            questionOwnAnswer_Text.text = content.instance[count - 1].ToString();
            questionOwnAnswerPage.color = content.instance[count - 1] == content.instance[count - 2] ? Color.green : Color.red;
        }

        if(content.instance[count - 3] == 0)
        {
            questionOtherAnswer_Text.text = "";
            questionOtherAnswerPage.color = Color.yellow;
        }
        else
        {
            questionOtherAnswer_Text.text = content.instance[count - 3].ToString();
            questionOtherAnswerPage.color = content.instance[count - 3] == content.instance[count - 2] ? Color.green : Color.red;
        }
    }
}
