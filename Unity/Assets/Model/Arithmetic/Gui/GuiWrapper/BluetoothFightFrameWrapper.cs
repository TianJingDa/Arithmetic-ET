using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;

/// <summary>
/// 蓝牙答题界面
/// </summary>
public class BluetoothFightFrameWrapper : GuiFrameWrapper
{
    private const string        end = "end";
    private const int           maxAnswer = 999999;
    private const int           countdownTime = 2;
    private int                 index;//问题序号
    private float               amount;
    private float               startTime;
    private float               timeCost;
    private bool                order;//true: -->; false: <--
    private bool                isSending;
    private bool                isReceiving;
    private bool                isPausing;
    private string              pattern;
    private string              symbol;
    private StringBuilder       result;
    private StringBuilder       question;

    private GameObject          equalImg;
    private GameObject          giveUpBg;
    private GameObject          countdownBg;
    private GameObject          reverseOrderImage;
    private GameObject          timeMaskImage;
    private Text                timeBtn_Text;
    private Text                resultImg_Text;
    private Text                questionImg_Text;
    private List<GameObject>    countdownNumsList;
    private List<int>           curInstance;
    private List<List<int>>     resultList;


    void Start () 
	{
        id = GuiFrameID.BluetoothFightFrame;
        BluetoothController.Instance.BLEReceiveMessage = OnReceiveMessage;
        Dictionary<string, MyRectTransform> rectTransforms = LayoutController.Instance.GetLayoutData();
        InitLayout(rectTransforms);
        Init();

        timeCost    = 0;
        index       = 0;
        order       = true;
        isSending   = false;
        isReceiving = false;
        isPausing   = false;
        result      = new StringBuilder();
        question    = new StringBuilder();
		curInstance = new List<int>();
        resultList  = new List<List<int>>();
        countdownNumsList = CommonTool.GetGameObjectsContainName(countdownBg, "Countdown_");
        FightController.Instance.GetFightParameter(out pattern, out amount, out symbol);
        FightController.Instance.ResetList();
        result.Length = 0;
        question.Length = 0;
        ClearAllText();
        StartFight();
    }

    void OnDestroy()
    {
        MyDebug.LogGreen("ReadUUID:" + BluetoothController.Instance.ReadUUID);
        MyDebug.LogGreen("WriteUUID:" + BluetoothController.Instance.WriteUUID);
        MyDebug.LogGreen("ServiceUUID:" + BluetoothController.Instance.ServiceUUID);

        BluetoothController.Instance.OnBluetoothFightFinish();
    }

    protected override void OnStart(Dictionary<string, GameObject> gameObjectDict)
    {
        equalImg            			= gameObjectDict["EqualImg"];
        giveUpBg            			= gameObjectDict["GiveUpBg"];
        countdownBg         			= gameObjectDict["CountdownBg"];
        timeMaskImage       			= gameObjectDict["TimeMaskImage"];
        reverseOrderImage   			= gameObjectDict["ReverseOrderImage"];
        timeBtn_Text        			= gameObjectDict["TimeBtn_Text"].GetComponent<Text>();
        resultImg_Text      			= gameObjectDict["ResultImg_Text"].GetComponent<Text>();
        questionImg_Text    			= gameObjectDict["QuestionImg_Text"].GetComponent<Text>();
    }


    protected override void OnButtonClick(Button btn)
    {
        if (isSending || isReceiving) return;

        base.OnButtonClick(btn);
        switch (btn.name)
        {
            case "0":
            case "1":
            case "2":
            case "3":
            case "4":
            case "5":
            case "6":
            case "7":
            case "8":
            case "9":
                RefreshResultText(btn.name);
                break;
            case "NextBtn":
                ShowNextQuestion(false);
                break;
            case "ClearBtn":
                ClearResultText();
                break;
            case "OrderBtn":
                ChangeInputOrder();
                break;
            case "TimeBtn":
                ChangeTimeVisibility();
                break;
            default:
                MyDebug.LogYellow("Can not find Button: " + btn.name);
                break;
        }
    }

    private void InitLayout(Dictionary<string, MyRectTransform> transforms)
    {
        Dictionary<string, RectTransform> rectTransformDict = CommonTool.InitRectTransformDict(gameObject);
        RectTransform myTrans;
        foreach (KeyValuePair<string, MyRectTransform> pair in transforms)
        {
            myTrans = rectTransformDict[pair.Key];
            myTrans.pivot = new Vector2(pair.Value.pivot.x, pair.Value.pivot.y);
            myTrans.anchorMax = new Vector2(pair.Value.anchorMax.x, pair.Value.anchorMax.y);
            myTrans.anchorMin = new Vector2(pair.Value.anchorMin.x, pair.Value.anchorMin.y);
            myTrans.offsetMax = new Vector2(pair.Value.offsetMax.x, pair.Value.offsetMax.y);
            myTrans.offsetMin = new Vector2(pair.Value.offsetMin.x, pair.Value.offsetMin.y);
            myTrans.localEulerAngles = new Vector3(pair.Value.localEulerAngles.x, pair.Value.localEulerAngles.y, pair.Value.localEulerAngles.z);
        }

        if (LayoutController.Instance.CurKeyboardID == KeyboardID.Up)
        {
            ExchangePosition(rectTransformDict["1"], rectTransformDict["7"]);
            ExchangePosition(rectTransformDict["2"], rectTransformDict["8"]);
            ExchangePosition(rectTransformDict["3"], rectTransformDict["9"]);
        }
    }

    private void ExchangePosition(RectTransform x, RectTransform y)
    {
        Vector3 pos = x.localPosition;
        x.localPosition = y.localPosition;
        y.localPosition = pos;
    }

    private void StartFight()
    {
        ShowNextQuestion(true);
        startTime = Time.realtimeSinceStartup;
        InvokeRepeating("NumberPattern", 0f, 0.1f);
    }

    private void ClearAllText()
    {
        timeBtn_Text.text = string.Empty;
        resultImg_Text.text = string.Empty;
        questionImg_Text.text = string.Empty;
    }

    private void NumberPattern()
    {
        if (isPausing) return;
        timeCost = Time.realtimeSinceStartup - startTime;
        timeBtn_Text.text = timeCost.ToString("f1") + "s";
    }

    private void RefreshResultText(string num)
    {
        StringBuilder lastResult = new StringBuilder(result.ToString());
        if (order) result.Append(num);
        else result.Insert(0, num);
        if (long.Parse(result.ToString()) > maxAnswer) result = lastResult;
        if (result.Length > maxAnswer.ToString().Length) result = lastResult;
        resultImg_Text.text = result.ToString();
    }

    private void ShowNextQuestion(bool isFirst)
    {
        if (result.Length <= 0 && !isFirst) return;

        if (result.Length > 0)
        {
            int resultInt = int.Parse(result.ToString()); 
            BluetoothMessage message = new BluetoothMessage(index, resultInt);
            if (!isReceiving)
            {
                isSending = true;
                BluetoothController.Instance.BLESendMessage(message);
            }
            else
            {
                StartCoroutine(ShowNextQuestion());
            }
        }
        else //第一个题
        {
            StartCoroutine(ShowNextQuestion());
        }
    }

    private IEnumerator ShowNextQuestion()
    {
        isPausing = true;
        countdownBg.SetActive(true);
        equalImg.SetActive(false);
        questionImg_Text.text = string.Empty;
        ClearResultText();
        int pauseTime = 1;
        while (pauseTime <= countdownTime)
        {
            for (int i = 0; i < countdownNumsList.Count; i++)
            {
                countdownNumsList[i].SetActive(i == countdownTime - pauseTime);
            }
            yield return new WaitForSeconds(1f);
            pauseTime++;
            startTime++;
        }
        equalImg.SetActive(true);
        countdownBg.SetActive(false);
        isPausing = false;

        lock (curInstance)
        {
            index++;
            curInstance = FightController.Instance.GetQuestionInstance();
            if (curInstance == null)
            {
                MyDebug.LogYellow("curInstance is NULL!");
                FightOver();
            }
            question.Length = 0;
            question.Append(curInstance[0].ToString());
            for (int i = 1; i < curInstance.Count - 1; i++)
            {
                question.Append(symbol);
                question.Append(curInstance[i].ToString());
            }
            questionImg_Text.text = question.ToString();
        }
    }

    private void OnReceiveMessage(BluetoothMessage message)
    {
        MyDebug.LogGreen("index: " + message.index + ", result: " + message.result);
        isReceiving = true;
        if(message.index == index)
        {
            int resultInt = 0;
            if (!int.TryParse(result.ToString(), out resultInt))
            {
                resultInt = 0;//被抢答的情况
            }
            lock (curInstance)//倒数第一个是自己的答案，倒数第二个是正确答案，倒数第三个是对方的答案
            {
                curInstance.Insert(curInstance.Count - 1, message.result);
                curInstance.Add(resultInt);
            }
            lock (resultList)
            {
                resultList.Add(curInstance);
            }
            if (resultList.Count == amount)
            {
                if (!message.name.Equals(end))
                {
                    isSending = true;
                    BluetoothController.Instance.BLESendMessage(new BluetoothMessage(index, resultInt, end));
                }
                FightOver();
            }
            else
            {
                if (isSending)
                {
                    StartCoroutine(ShowNextQuestion());
                }
                else
                {
                    isSending = true;
                    BluetoothController.Instance.BLESendMessage(new BluetoothMessage(index, resultInt));
                    StartCoroutine(ShowNextQuestion());
                }
            }
            isSending = false;
            isReceiving = false;
        }
        else
        {
            string tip = "Index Wrong:" + message.index;
            GuiController.Instance.CurCommonTipInstance.SetInstance(CommonTipID.Single, tip);
            GuiController.Instance.SwitchWrapper(GuiFrameID.CommonTipFrame, true);
        }
    }

    private void ClearResultText()
    {
        resultImg_Text.text = string.Empty;
        result.Length = 0;
    }

    private void ChangeInputOrder()
    {
        order = !order;
        reverseOrderImage.SetActive(!reverseOrderImage.activeSelf);//暂时先这么处理
    }

    private void ChangeTimeVisibility()
    {
        timeMaskImage.SetActive(!timeMaskImage.activeSelf);//暂时先这么处理
    }

    private void FightOver()
    {
        CancelInvoke();
        SaveFileInstance curSaveFileInstance = new SaveFileInstance();
        curSaveFileInstance.cInstance = FightController.Instance.CurCategoryInstance;
        curSaveFileInstance.opponentName = BluetoothController.Instance.CurPeripheralInstance.name;
        RecordController.Instance.SaveRecord(curSaveFileInstance, resultList, symbol, timeCost);
        GuiController.Instance.SwitchWrapper(GuiFrameID.SettlementFrame);
    }
}

[Serializable]
public class BluetoothMessage
{
    public int index;
    public int result;
    public string name;
    public byte[] data;

    public BluetoothMessage() { }

    public BluetoothMessage(byte[] data)
    {
        this.data = data;
        index = data[0];
        result = 0;
        for (int i = 1; i <= 5; i++)
        {
            result = result * 100 + data[i];
        }
        List<byte> byteList = new List<byte>(data);
        byteList.RemoveRange(0, 6);
        name = Encoding.UTF8.GetString(byteList.ToArray());
    }

    public BluetoothMessage(int index, int result)
    {
        this.index = index;
        this.result = result;
        this.name = "";
        List<byte> byteList = new List<byte> { 0, 0, 0, 0, 0, 0 };
        byteList[0] = (byte)index;
        for(int i = 1; i <= 5; i++)
        {
            byteList[6 - i] = (byte)(result % 100);
            result /= 100;
        }
        data = byteList.ToArray();
    }

    public BluetoothMessage(int index, int result, string centralName)
    {
        this.index = index;
        this.result = result;
        this.name = centralName;
        List<byte> byteList = new List<byte> { 0, 0, 0, 0, 0, 0 };
        byteList[0] = (byte)index;
        for (int i = 1; i <= 5; i++)
        {
            byteList[6 - i] = (byte)(result % 100);
            result /= 100;
        }
        byteList.AddRange(Encoding.UTF8.GetBytes(centralName));
        data = byteList.ToArray();
    }
}