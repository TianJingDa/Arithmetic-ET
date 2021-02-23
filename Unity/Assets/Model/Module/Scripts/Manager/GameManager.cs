using System.Collections;
using UnityEngine;
using System.IO;
using cn.sharesdk.unity3d;

/// <summary>
/// 游戏控制层
/// </summary>
public class GameManager : MonoBehaviour
{
    private const float                                         TimeOut = 1f;

    private LanguageController                                  c_LanguageCtrl;
    private FightController                                     c_FightCtrl;
    private AchievementController                               c_AchievementCtrl;
    private SkinController                                      c_SkinCtrl;
    private LayoutController                                    c_LayoutCtrl;
    private FontController                                      c_FontCtrl;
    private TextColorController                                 c_TextColorCtrl;
    private RecordController                                    c_RecordCtrl;
    private GuiController                                       c_GuiCtrl;
    private BluetoothController                                 c_BluetoothCtrl;

    private bool                                                m_IsLogining;
    private System.Action                                       m_ShareAction;                      //用于分享时初始化用户名称
    private ShareSDK                                            m_ShareSDK;                         //用于分享成就和成绩
/*--------------------------------------------------------------------------------------------------------------------------------------------------*/

    public ShareInstance CurShareInstance { get; set; }

    public string UserName
    {
        get
        {
            return PlayerPrefs.GetString("UserName", null);
        }
        set
        {
            PlayerPrefs.SetString("UserName", value);
        }
    }

    public string Version
    {
        get
        {
#if UNITY_ANDROID
            return "3.1";
#elif UNITY_IOS
            return "1.0";
#else 
            return "1.0";
#endif
        }
    }

    public static GameManager Instance//单例
    {
        get;
        private set;
    }
    void Awake()
    {
        if (Instance == null){ Instance = this; }
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 30;

        c_AchievementCtrl       = AchievementController.Instance;
        c_FightCtrl             = FightController.Instance;
        c_FontCtrl              = FontController.Instance;
        c_LayoutCtrl            = LayoutController.Instance;
        c_LanguageCtrl          = LanguageController.Instance;
        c_SkinCtrl              = SkinController.Instance;
        c_TextColorCtrl         = TextColorController.Instance;
        c_RecordCtrl            = RecordController.Instance;
        c_GuiCtrl               = GuiController.Instance;
        c_BluetoothCtrl         = BluetoothController.Instance;
    }

    void Start()
    {
        m_ShareSDK = GetComponent<ShareSDK>();
        m_ShareSDK.shareHandler = OnShareResultHandler;
        InitShareIcon();
        c_GuiCtrl.SwitchWrapper(GuiFrameID.StartFrame, true);
//#if UNITY_EDITOR
//        gameObject.AddComponent<Camera>();
//#endif
    }

#region 公共方法

    public void ShareImage(Rect mRect, PlatformType type)
    {
        string directoryPath = Application.persistentDataPath + "/ScreenShot";
        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
        string fileName = System.DateTime.Now.ToString("yyyyMMddHHmmss");
        string filePath = directoryPath + "/" + fileName + ".png";
        StartCoroutine(CaptureScreenShotByRect(mRect, filePath, type));
    }

    public void ShareUrl(PlatformType type)
    {
        string title = c_LanguageCtrl.GetLanguage("Text_00072");
        string description = c_LanguageCtrl.GetLanguage("Text_00073");
        ShareContent content = new ShareContent();
        content.SetImagePath(Application.persistentDataPath + "/Image/ShareIcon.png");
        if (type == PlatformType.WeChatMoments || type == PlatformType.WeChat)
        {
            content.SetText(description);
            content.SetTitle(title);
            content.SetUrl("https://www.taptap.com/app/78306");
            content.SetShareType(ContentType.Webpage);
        }
        else if(type == PlatformType.SinaWeibo)
        {
            content.SetText(title + "https://www.taptap.com/app/78306");//text是Url
        }
        m_ShareSDK.ShareContent(type, content);
    }
#endregion

#region 私有方法

    /// <summary>
    /// 截屏
    /// </summary>
    /// <param name="mRect"></param>
    /// <param name="filePath"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private IEnumerator CaptureScreenShotByRect(Rect mRect, string filePath, PlatformType type)
    {
        //等待渲染线程结束
        yield return new WaitForEndOfFrame();
        //初始化Texture2D
        Texture2D mTexture = new Texture2D((int)mRect.width, (int)mRect.height, TextureFormat.RGB24, false);
        //读取屏幕像素信息并存储为纹理数据
        mTexture.ReadPixels(mRect, 0, 0,false);
        //应用
        mTexture.Apply(false);
        //将图片信息编码为字节信息
        byte[] bytes = mTexture.EncodeToPNG();
        //保存
        File.WriteAllBytes(filePath, bytes);

        ShareImage(filePath, type);
    }

    private void ShareImage(string filePath, PlatformType type)
    {
        ShareContent content = new ShareContent();
        content.SetImagePath(filePath);
        if (type == PlatformType.WeChatMoments || type == PlatformType.WeChat)
        {
            content.SetShareType(ContentType.Image);
        }
        m_ShareSDK.ShareContent(type, content);
    }

    /// <summary>
    /// ShareSDK分享回调
    /// </summary>
    /// <param name="reqID"></param>
    /// <param name="state"></param>
    /// <param name="type"></param>
    /// <param name="result"></param>
    private void OnShareResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        string message = "";

        if (state == ResponseState.Success)
        {
            message = c_LanguageCtrl.GetLanguage("Text_20076");
        }
        else if (state == ResponseState.Fail)
        {
            message = c_LanguageCtrl.GetLanguage("Text_20077");
        }
        else if (state == ResponseState.Cancel)
        {
            message = c_LanguageCtrl.GetLanguage("Text_20078");
        }

        GuiController.Instance.CurCommonTipInstance = new CommonTipInstance(CommonTipID.Splash, message);
        GuiController.Instance.SwitchWrapper(GuiFrameID.CommonTipFrame, true);
    }

    private void InitShareIcon()
    {
        string path = Application.persistentDataPath + "/Image/ShareIcon.png";
        if (!File.Exists(path)) StartCoroutine(AssetHelper.CopyImage("ShareIcon.png"));
    }
#endregion

}
