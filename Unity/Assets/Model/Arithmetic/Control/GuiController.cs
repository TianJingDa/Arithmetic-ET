using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class GuiController : Controller
{
    #region C#单例
    private static GuiController instance = null;
    private GuiController()
    {
        base.id = ControllerID.GuiController;
        frameAddressDict = new Dictionary<GuiFrameID, string>();
        frameDict = new Dictionary<GuiFrameID, Object>();
        itemAddressDict = new Dictionary<GuiItemID, string>();
        itemDict = new Dictionary<GuiItemID, Object>();
        guiFrameStack = new Stack<GuiFrameWrapper>();
        root = GameObject.Find("UIRoot");
        InitGuiData();
        MyDebug.LogWhite("Loading Controller:" + id.ToString());
    }
    public static GuiController Instance
    {
        get { return instance ?? (instance = new GuiController()); }
    }
    #endregion

    private const float TweenDuration = 0.5f;                           //Tween动画持续时间

    private GameObject root;                                            //UI对象的根对象
    private Stack<GuiFrameWrapper> guiFrameStack;                       //当前激活的GuiWrapper
    private Dictionary<GuiFrameID, string> frameAddressDict;//key：GuiFrameID，value：资源路径
    private Dictionary<GuiFrameID, Object> frameDict;//key：GuiFrameID，value：资源
    private Dictionary<GuiItemID, string> itemAddressDict;//key：Item名称，value：资源路径
    private Dictionary<GuiItemID, Object> itemDict;//key：GuiFrameID，value：资源

    public CommonTipInstance CurCommonTipInstance { get; } = new CommonTipInstance();

    //获取栈顶第二个元素的ID
    public GuiFrameID LastGUI
    {
        get
        {
            GuiFrameID id = GuiFrameID.None;
            lock (guiFrameStack)
            {
                GuiFrameWrapper wrapper = guiFrameStack.Pop();
                if (guiFrameStack.Count > 0)
                {
                    id = guiFrameStack.Peek().id;
                }
                guiFrameStack.Push(wrapper);
            }
            return id;
        }
    }

    public GuiFrameID CurGUI
    {
        get
        {
            GuiFrameID id = GuiFrameID.None;
            if (guiFrameStack.Count > 0)
            {
                id = guiFrameStack.Peek().id;
            }
            return id;
        }
    }

    /// <summary>
    /// 用于区分三种竞赛方式：成就、自由、蓝牙 
    /// </summary>
    public GuiFrameID CompetitionGUI
    {
        get;
        set;
    }

    public CanvasGroup CurCanvasGroup
    {
        get
        {
            CanvasGroup canvas = null;
            if (guiFrameStack.Count > 0)
            {
                GuiFrameWrapper gui = guiFrameStack.Peek();
                canvas = gui.GetComponent<CanvasGroup>();
            }
            return canvas;
        }
    }

    /// <summary>
    /// 注册所有资源地址
    /// </summary>
    private void InitGuiData()
    {
        frameAddressDict.Add(GuiFrameID.StartFrame, "GuiWrapper/StartFrame");
        frameAddressDict.Add(GuiFrameID.StatisticsFrame, "GuiWrapper/StatisticsFrame");
        frameAddressDict.Add(GuiFrameID.CategoryFrame, "GuiWrapper/CategoryFrame");
        frameAddressDict.Add(GuiFrameID.SetUpFrame, "GuiWrapper/SetUpFrame");
        frameAddressDict.Add(GuiFrameID.FightFrame, "GuiWrapper/FightFrame");
        frameAddressDict.Add(GuiFrameID.SettlementFrame, "GuiWrapper/SettlementFrame");
        frameAddressDict.Add(GuiFrameID.ChapterFrame, "GuiWrapper/ChapterFrame");
        frameAddressDict.Add(GuiFrameID.BluetoothFrame, "GuiWrapper/BluetoothFrame");
        frameAddressDict.Add(GuiFrameID.NameBoardFrame, "GuiWrapper/NameBoardFrame");
        frameAddressDict.Add(GuiFrameID.CommonTipFrame, "GuiWrapper/CommonTipFrame");
        frameAddressDict.Add(GuiFrameID.BluetoothFightFrame, "GuiWrapper/BluetoothFightFrame");
        frameAddressDict.Add(GuiFrameID.SaveFileFrame, "GuiWrapper/SaveFileFrame");
        frameAddressDict.Add(GuiFrameID.ShareFrame, "GuiWrapper/ShareFrame");
        frameAddressDict.Add(GuiFrameID.RankFrame, "GuiWrapper/RankFrame");
        frameAddressDict.Add(GuiFrameID.LoginFrame, "GuiWrapper/LoginFrame");

        itemAddressDict.Add(GuiItemID.AchievementItem, "GuiItem/AchievementItem");
        itemAddressDict.Add(GuiItemID.SaveFileItem, "GuiItem/SaveFileItem");
        itemAddressDict.Add(GuiItemID.QuestionItem, "GuiItem/QuestionItem");
        itemAddressDict.Add(GuiItemID.PeripheralItem, "GuiItem/PeripheralItem");
        itemAddressDict.Add(GuiItemID.BluetoothQuestionItem, "GuiItem/BluetoothQuestionItem");
        itemAddressDict.Add(GuiItemID.RankItem, "GuiItem/RankItem");
    }

    /// <summary>
    /// 获取资源实例
    /// </summary>
    /// <param name="id">GuiFrameID</param>
    /// <returns>资源实例</returns>
    public Object GetGuiResource(GuiFrameID id)
    {
        Object resouce = null;
        if (!frameDict.TryGetValue(id, out resouce))
        {
            resouce = Resources.Load(frameAddressDict[id]);
            frameDict.Add(id, resouce);
        }
        return resouce;
    }

    public GameObject GetPrefabItem(GuiItemID id)
    {
        Object resource = GetItemResource(id);
        return Object.Instantiate(resource) as GameObject;
    }

    private Object GetItemResource(GuiItemID id)
    {
        Object resouce = null;
        if (!itemDict.TryGetValue(id, out resouce))
        {
            resouce = Resources.Load(itemAddressDict[id]);
            itemDict.Add(id, resouce);
        }
        return resouce;
    }

    /// <summary>
    /// GuiWrapper切换，无动画
    /// </summary>
    /// <param name="targetID"></param>
    public void SwitchWrapper(GuiFrameID targetID, bool isAdd = false)
    {
        if (!isAdd)
        {
            if (guiFrameStack.Count > 0)
            {
                GuiFrameWrapper oldGui = guiFrameStack.Pop();
                if (oldGui) Object.Destroy(oldGui.gameObject);
            }
            if (guiFrameStack.Count > 0)
            {
                GuiFrameWrapper oldGui = guiFrameStack.Peek();
                if (oldGui) oldGui.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
        }
        else
        {
            if (guiFrameStack.Count > 0)
            {
                GuiFrameWrapper oldGui = guiFrameStack.Peek();
                if (oldGui) oldGui.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }

        if (targetID != GuiFrameID.None)
        {
            Object reource = GetGuiResource(targetID);
            if (reource == null)
            {
                MyDebug.LogYellow("Can not load reousce:" + targetID.ToString());
                return;
            }
            GameObject newGui = Object.Instantiate(reource, root.transform) as GameObject;
            guiFrameStack.Push(newGui.GetComponent<GuiFrameWrapper>());
        }
    }
    /// <summary>
    /// GuiWrapper切换，有移动动画
    /// </summary>
    /// <param name="targetID"></param>
    /// <param name="mID"></param>
    /// <param name="isIn"></param>
    public void SwitchWrapperWithMove(GuiFrameID targetID, MoveID mID, bool isIn)
    {
        root.GetComponent<GraphicRaycaster>().enabled = false;
        Object reource = GetGuiResource(targetID);
        if (reource == null)
        {
            MyDebug.LogYellow("Can not load reousce: " + targetID.ToString());
            return;
        }
        GameObject targetWrapper = Object.Instantiate(reource, root.transform) as GameObject;
        if (isIn)
        {
            targetWrapper.transform.DOLocalMoveX(Screen.width * (int)mID, TweenDuration, true).
                                    From().
                                    SetEase(Ease.OutQuint).
                                    OnComplete(() => TweenComplete(targetWrapper));
        }
        else
        {
            targetWrapper.transform.SetAsFirstSibling();
            GuiFrameWrapper topGui = null;
            if (guiFrameStack.Count > 0) topGui = guiFrameStack.Peek();
            if (topGui) topGui.transform.DOLocalMoveX(Screen.width * (int)mID, TweenDuration, true).
                                    SetEase(Ease.OutQuint).
                                    OnComplete(() => TweenComplete(targetWrapper));
        }
    }
    /// <summary>
    /// GuiWrapper切换，有缩放动画
    /// </summary>
    /// <param name="targetID"></param>
    /// <param name="isIn"></param>
    public void SwitchWrapperWithScale(GuiFrameID targetID, bool isIn)
    {
        root.GetComponent<GraphicRaycaster>().enabled = false;
        Object reource = GetGuiResource(targetID);
        if (reource == null)
        {
            MyDebug.LogYellow("Can not load reousce: " + targetID.ToString());
            return;
        }
        GameObject targetWrapper = Object.Instantiate(reource, root.transform) as GameObject;
        if (isIn)
        {
            targetWrapper.transform.DOScale(Vector3.zero, TweenDuration).
                                    From().
                                    SetEase(Ease.OutQuint).
                                    OnComplete(() => TweenComplete(targetWrapper));
        }
        else
        {
            targetWrapper.transform.SetAsFirstSibling();
            GuiFrameWrapper topGui = null;
            if (guiFrameStack.Count > 0) topGui = guiFrameStack.Peek();
            if (topGui) topGui.transform.DOScale(Vector3.zero, TweenDuration).
                                   SetEase(Ease.OutQuint).
                                   OnComplete(() => TweenComplete(targetWrapper));
        }
    }

    private void TweenComplete(GameObject targetWrapper)
    {
        if (guiFrameStack.Count > 0)
        {
            GuiFrameWrapper oldGui = guiFrameStack.Pop();
            if (oldGui) Object.Destroy(oldGui.gameObject);
        }
        guiFrameStack.Push(targetWrapper.GetComponent<GuiFrameWrapper>());
        root.GetComponent<GraphicRaycaster>().enabled = true;
    }
}
