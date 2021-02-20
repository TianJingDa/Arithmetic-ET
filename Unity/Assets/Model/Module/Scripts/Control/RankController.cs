using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class RankController : Controller
{
	#region C#单例
	private static RankController instance = null;
	private RankController()
	{
		base.id = ControllerID.RankController;
		InitRankData();
		MyDebug.LogWhite("Loading Controller:" + id.ToString());
	}
	public static RankController Instance
	{
		get { return instance ?? (instance = new RankController()); }
	}
    #endregion

	private const float RefreshInterval = 60f;
    private const float TimeOut = 1f;
#if TEST
    private const string DownloadURL = "http://182.92.68.73:8091/getData";
	private const string UploadURL = "http://182.92.68.73:8091/setData";
	private const string DetailURL = "http://182.92.68.73:8091/getDetail";
    private const string EnrollURL = "http://182.92.68.73:8091/join";
#else
    private const string DownloadURL = "http://47.105.77.226:8091/getData";
    private const string UploadURL = "http://47.105.77.226:8091/setData";
    private const string DetailURL = "http://47.105.77.226:8091/getDetail";
    private const string EnrollURL = "http://47.105.77.226:8091/join";
#endif

    private Dictionary<CategoryInstance, DateTime> lastRefreshTimeDict;
	private Dictionary<CategoryInstance, List<RankInstance>> rankDataDict;

    public RankInstance CurRankInstance { get; set; }

    public bool AlreadyEnroll
    {
        get
        {
            string enrollTime = PlayerPrefs.GetString("EnrollTime", "");
            if (!string.IsNullOrEmpty(enrollTime))
            {
                DateTime dateTime;
                if(DateTime.TryParse(enrollTime, out dateTime))
                {
                    if(dateTime.Year == DateTime.Now.Year && dateTime.Month == DateTime.Now.Month)
                    {
                        return true;
                    }
                }
            }
            PlayerPrefs.SetString("EnrollTime", "");
            return false;
        }
        set
        {
            if (value)
            {
                string curTime = DateTime.Now.ToString("yyyy-MM-dd");
                PlayerPrefs.SetString("EnrollTime", curTime);
            }
        }
    }

    public CategoryInstance ActivityCategory
    {
        get
        {
            string instance = PlayerPrefs.GetString("ActivityCategory", "");
            CategoryInstance category = JsonUtility.FromJson<CategoryInstance>(instance);
            return category;
        }
        set
        {
            string instance = JsonUtility.ToJson(value);
            PlayerPrefs.SetString("ActivityCategory", instance);
        }
    }

    private void InitRankData()
	{
		lastRefreshTimeDict = new Dictionary<CategoryInstance, DateTime>();
		rankDataDict = new Dictionary<CategoryInstance, List<RankInstance>>();
	}

    /// <summary>
    /// 拉取排行榜信息
    /// </summary>
    /// <param name="form"></param>
    /// <param name="OnSucceed"></param>
    /// <returns></returns>
	public IEnumerator DownloadRecord(CategoryInstance instance, Action<ArrayList> OnSucceed, Action<string> OnFail)
    {
		if(!CanRefreshRankData(instance))
		{
			List<RankInstance> instances;
			rankDataDict.TryGetValue(instance, out instances);
			if(instances != null && instances.Count > 0)
			{
				if(OnSucceed != null)
				{
					ArrayList dataList = new ArrayList(instances);
					OnSucceed(dataList);
				}
			}
			else
			{
				if(OnFail != null)
				{
					string msg = LanguageController.Instance.GetLanguage("Text_20071");
					OnFail(msg);
				}
			}
			yield break;
		}

		WWWForm form = new WWWForm();
		form.AddField("userId", GameManager.Instance.UserID);
		form.AddField("jwttoken", GameManager.Instance.Token);
		form.AddField("model", (int)instance.patternID + 1);
		form.AddField("num", (int)instance.amountID + 1);
		form.AddField("calcu", (int)instance.symbolID + 1);
		form.AddField("digit", (int)instance.digitID + 2);
		form.AddField("operate", (int)instance.operandID + 2);

        WWW www = new WWW(DownloadURL, form);

        float responseTime = 0;
        while (!www.isDone && responseTime < TimeOut)
        {
            responseTime += Time.deltaTime;
            yield return www;
        }

        string message = "";
        if (www.isDone)
        {
            DownloadDataResponse response = JsonUtility.FromJson<DownloadDataResponse>(www.text);
            if (response != null)
            {
				if (response.code == (int)CodeID.SUCCESS)
                {
                    MyDebug.LogGreen("Download Rank Data Succeed!");
					lastRefreshTimeDict[instance] = DateTime.Now;
					if(response.data != null && response.data.Count > 0)
					{
						rankDataDict[instance] = response.data;
						if(OnSucceed != null)
						{
							ArrayList dataList = new ArrayList(response.data);
							OnSucceed(dataList);
						}
						yield break;
					}
					else
					{
						message = LanguageController.Instance.GetLanguage("Text_20071");
					}
                }
                else if (response.code == (int)CodeID.GAME_VERSION_ERROR)
                {
                    MyDebug.LogYellow("Download Rank Data Fail:" + response.code);
                    message = LanguageController.Instance.GetLanguage("Text_20079");
                }
                else
                {
					MyDebug.LogYellow("Download Rank Data Fail:" + response.code);
                    message = LanguageController.Instance.GetLanguage("Text_20066");
                }
            }
            else
            {
				MyDebug.LogYellow("Download Rank Data Fail: Message Is Not Response!");
                message = LanguageController.Instance.GetLanguage("Text_20066");
            }
        }
        else
        {
			MyDebug.LogYellow("Download Rank Data Fail: Long Time!");
            message = LanguageController.Instance.GetLanguage("Text_20067");
        }

		if(OnFail != null)
		{
			OnFail(message);
		}
    }

	private bool CanRefreshRankData(CategoryInstance instance)
	{
		DateTime lastTime;
		bool hasLastTime = lastRefreshTimeDict.TryGetValue(instance, out lastTime);
		if(hasLastTime)
		{
			TimeSpan ts = DateTime.Now - lastTime;
			return ts.TotalSeconds > RefreshInterval;
		}

		return true;
	}

    /// <summary>
    /// 上传排行榜信息
    /// </summary>
    /// <returns></returns>
	public IEnumerator UploadRecord(WWWForm form, Action<string> OnSuccees, Action<string> OnFail)
    {
        WWW www = new WWW(UploadURL, form);

        float responseTime = 0;
        while (!www.isDone && responseTime < TimeOut)
        {
            responseTime += Time.deltaTime;
            yield return www;
        }

        string message = "";
        if (www.isDone)
        {
            UploadDataResponse response = JsonUtility.FromJson<UploadDataResponse>(www.text);
            if (response != null)
            {
				if (response.code == (int)CodeID.SUCCESS)
                {
                    MyDebug.LogGreen("Upload Rank Data Succeed!");
                    if (response.data.rank > 0)
                    {
                        message = LanguageController.Instance.GetLanguage("Text_20068");
                        message = string.Format(message, response.data.rank);
                    }
                    else
                    {
                        MyDebug.LogYellow("Upload Rank Data Fail:" + response.code);
                        message = LanguageController.Instance.GetLanguage("Text_20070");
                    }

                    if (OnSuccees != null)
                    {
                        OnSuccees(message);
                    }
                    yield break;
                }
                else if (response.code == (int)CodeID.GAME_VERSION_ERROR)
                {
                    MyDebug.LogYellow("Upload Rank Data Fail:" + response.code);
                    message = LanguageController.Instance.GetLanguage("Text_20079");
                }
                else
                {
					MyDebug.LogYellow("Upload Rank Data Fail:" + response.code);
                    message = LanguageController.Instance.GetLanguage("Text_20066");
                }
            }
            else
            {
                MyDebug.LogYellow("Upload Rank Data Fail: Message Is Not Response!");
                message = LanguageController.Instance.GetLanguage("Text_20066");
            }
        }
        else
        {
            MyDebug.LogYellow("Upload Rank Data Fail: Long Time!");
            message = LanguageController.Instance.GetLanguage("Text_20067");
        }

		if(OnFail != null)
		{
            OnFail(message);
		}     
    }

    /// <summary>
    /// 获取排行榜详情
    /// </summary>
    /// <param name="form"></param>
    /// <param name="OnSucceed"></param>
    /// <param name="OnFail"></param>
    /// <returns></returns>
	public IEnumerator GetRankDetail(WWWForm form, Action<string> OnSucceed, Action<string> OnFail)
	{
		WWW www = new WWW(DetailURL, form);

		float responseTime = 0;
		while (!www.isDone && responseTime < TimeOut)
		{
			responseTime += Time.deltaTime;
			yield return www;
		}

		string message = "";
		if (www.isDone)
		{
			GetDetailResponse response = JsonUtility.FromJson<GetDetailResponse>(www.text);
			if (response != null)
			{
				if (response.code == (int)CodeID.SUCCESS)
				{
					MyDebug.LogGreen("Get Rank Detail Succeed!");
					if(OnSucceed != null)
					{
						OnSucceed(response.data);
					}
                    yield break;
				}
                else if(response.code == (int)CodeID.GAME_VERSION_ERROR)
                {
                    MyDebug.LogYellow("Get Rank Detail Fail:" + response.code);
                    message = LanguageController.Instance.GetLanguage("Text_20079");
                }
                else
				{
					MyDebug.LogYellow("Get Rank Detail Fail:" + response.code);
					message = LanguageController.Instance.GetLanguage("Text_20066");
				}
			}
			else
			{
				MyDebug.LogYellow("Get Rank Detail Fail: Message Is Not Response!");
				message = LanguageController.Instance.GetLanguage("Text_20066");
			}
		}
		else
		{
			MyDebug.LogYellow("Get Rank Detail Fail: Long Time!");
			message = LanguageController.Instance.GetLanguage("Text_20067");
		}

		if(OnFail != null)
		{
			OnFail(message);
		}     
	}

    /// <summary>
    /// 活动报名
    /// </summary>
    /// <param name="form"></param>
    /// <param name="OnSuccees"></param>
    /// <param name="OnFail"></param>
    /// <returns></returns>
    public IEnumerator EnrollActivity(WWWForm form, Action<CategoryInstance> OnSuccees, Action<string> OnFail)
    {
        WWW www = new WWW(EnrollURL, form);

        float responseTime = 0;
        while (!www.isDone && responseTime < TimeOut)
        {
            responseTime += Time.deltaTime;
            yield return www;
        }

        string message = "";
        if (www.isDone)
        {
            EnrollActivityResponse response = JsonUtility.FromJson<EnrollActivityResponse>(www.text);
            if (response != null)
            {
                if (response.code == (int)CodeID.SUCCESS)
                {
                    MyDebug.LogGreen("Enroll Activity Succeed!");

                    if (OnSuccees != null)
                    {
                        CategoryInstance instance = ConvertToCategory(response.data);
                        OnSuccees(instance);
                    }
                    yield break;
                }
                else if(response.code == (int)CodeID.JOIN_TIME_ERROR)
                {
                    MyDebug.LogYellow("Enroll Activity Fail:" + response.code);
                    message = LanguageController.Instance.GetLanguage("Text_20081");
                }
                else if (response.code == (int)CodeID.GAME_VERSION_ERROR)
                {
                    MyDebug.LogYellow("Enroll Activity Fail:" + response.code);
                    message = LanguageController.Instance.GetLanguage("Text_20079");
                }
                else
                {
                    MyDebug.LogYellow("Enroll Activity Fail:" + response.code);
                    message = LanguageController.Instance.GetLanguage("Text_20066");
                }
            }
            else
            {
                MyDebug.LogYellow("Enroll Activity Fail: Message Is Not Response!");
                message = LanguageController.Instance.GetLanguage("Text_20066");
            }
        }
        else
        {
            MyDebug.LogYellow("Enroll Activity Fail: Long Time!");
            message = LanguageController.Instance.GetLanguage("Text_20067");
        }

        if (OnFail != null)
        {
            OnFail(message);
        }
    }

    private CategoryInstance ConvertToCategory(Category category)
    {
        CategoryInstance instance = new CategoryInstance();
        instance.patternID = (PatternID)(category.model - 1);
        instance.amountID = (AmountID)(category.num - 1);
        instance.symbolID = (SymbolID)(category.calcu - 1);
        instance.digitID = (DigitID)(category.digit - 2);
        instance.operandID = (OperandID)(category.operate - 2);
        return instance;
    }

    [Serializable]
    private class DownloadDataResponse
    {
		public int code;//200:成功
		public string errmsg;
		public List<RankInstance> data;
    }

    [Serializable]
    private class UploadDataResponse
    {
		public int code;//200:成功
		public string errmsg;
		public UploadData data;
    }

	[Serializable]
	private class UploadData
	{
		public int rank;
	}

	[Serializable]
	private class GetDetailResponse
	{
		public int code;//200:成功
		public string errmsg;
		public string data;
	}

    [Serializable]
    private class EnrollActivityResponse
    {
        public int code;//200:成功
        public string errmsg;
        public Category data;
    }

    [Serializable]
    private class Category
    {
        public int model;
        public int num;
        public int calcu;
        public int digit;
        public int operate;
    }
}
