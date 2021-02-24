using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;
using System.IO;
using System.Text;
using System.Collections;

public static class CommonTool 
{
    private static float tweenDuration = 0.5f;

    public static void SetData(string path, string toSave)
    {
        StreamWriter streamWriter = File.CreateText(path);
        streamWriter.Write(toSave);
        streamWriter.Close();
    }

    public static string GetDataFromResources(string path)
    {
        string data = ((TextAsset)Resources.Load(path)).text;
        return data;
    }

    public static string GetDataFromDataPath(string path)
    {
        if (!File.Exists(path))
        {
            MyDebug.LogYellow("File does not exit:" + path);
            return null;
        }
        StreamReader streamReader = File.OpenText(path);
        string data = streamReader.ReadToEnd();
        streamReader.Close();
        return data;
    }

    public static T GetComponentByName<T>(GameObject root, string name) where T : Component
    {
        T[] array = root.GetComponentsInChildren<T>(true);
        T result = null;
        if (array != null)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].name == name)
                {
                    result = array[i];
                    break;
                }
            }
        }
        if (result == null)
        {
            MyDebug.LogYellow("Can not find :" + name);
        }
        return result;
    }
    public static T GetComponentContainsName<T>(GameObject root, string name) where T : Component
    {
        T[] array = root.GetComponentsInChildren<T>(true);
        T result = null;
        if (array != null)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].name.Contains(name))
                {
                    result = array[i];
                    break;
                }
            }
        }
        if (result == null)
        {
            MyDebug.LogYellow("Can not find :" + name);
        }
        return result;
    }
    public static List<T> GetComponentsContainName<T>(GameObject root, string name) where T : Component
    {
        List<T> result = new List<T>();
        T[] array = root.GetComponentsInChildren<T>(true);
        if (array != null)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].name.Contains(name))
                {
                    result.Add(array[i]);
                }
            }
        }
        if (result.Count == 0)
        {
            MyDebug.LogYellow("Can not find :" + name);
        }
        return result;
    }
    public static GameObject GetGameObjectByName(GameObject root, string name)
    {
        GameObject result = null;
        if (root != null)
        {
            Transform[] objArray = root.GetComponentsInChildren<Transform>(true);
            if (objArray != null)
            {
                for (int i = 0; i < objArray.Length; i++)
                {
                    if (objArray[i].name == name)
                    {
                        result = objArray[i].gameObject;
                        break;
                    }
                }
            }
        }
        if (result == null)
        {
            MyDebug.LogYellow("Can not find :" + name);
        }
        return result;
    }
    public static GameObject GetGameObjectContainsName(GameObject root, string name)
    {
        GameObject result = null;
        if (root != null)
        {
            Transform[] objArray = root.GetComponentsInChildren<Transform>(true);
            if (objArray != null)
            {
                for (int i = 0; i < objArray.Length; i++)
                {
                    if (objArray[i].name.Contains(name))
                    {
                        result = objArray[i].gameObject;
                        break;
                    }
                }
            }
        }
        if (result == null)
        {
            MyDebug.LogYellow("Can not find :" + name);
        }
        return result;
    }
    public static List<GameObject> GetGameObjectsContainName(GameObject root, string name)
    {
        List<GameObject> result = new List<GameObject>();
        Transform[] objArray = root.GetComponentsInChildren<Transform>(true);
        if (objArray != null)
        {
            for (int i = 0; i < objArray.Length; i++)
            {
                if (objArray[i].name.Contains(name))  
                {
                    result.Add(objArray[i].gameObject);
                }
            }
        }
        if (result.Count == 0)
        {
            MyDebug.LogYellow("Can not find :" + name);
        }
        return result;
    }
    public static GameObject GetParentByName(GameObject child,string name)
    {
        GameObject result = null;

        if(child.transform.parent.gameObject.name == name)
        {
            result = child.transform.parent.gameObject;
        }
        else if(child.transform.parent != null)
        {
            result = GetParentByName(child.transform.parent.gameObject, name);
        }

        if (result == null)
        {
            MyDebug.LogYellow("Can not find :" + name);
        }

        return result;


    }
    public static void InitText(GameObject root)
    {
        Text[] textArray = root.GetComponentsInChildren<Text>(true);
        if (textArray.Length == 0)
        {
            return;
        }
        var skinID = SkinController.Instance.CurSkinID;
        var languageID = LanguageController.Instance.CurLanguageID;
        Font curFont = FontController.Instance.GetFont(skinID, languageID);
        for (int i = 0; i < textArray.Length; i++)
        {
            textArray[i].font = curFont;
            //textArray[i].color = GameManager.Instance.GetColor(textArray[i].index);
            if (string.IsNullOrEmpty(textArray[i].GetIndex()))
            {
                textArray[i].text = "";
            }
            else
            {
                textArray[i].text = LanguageController.Instance.GetLanguage(textArray[i].GetIndex());
            }
        }
    }
    public static void InitImage(GameObject root)
    {
        Image[] imageArray = root.GetComponentsInChildren<Image>(true);
        if (imageArray.Length == 0)
        {
            return;
        }
        for (int i = 0; i < imageArray.Length; i++)
        {
            if (string.IsNullOrEmpty(imageArray[i].GetIndex()))
            {
                imageArray[i].color = Color.white;
                continue;
            }
            Sprite sprite = SkinController.Instance.GetSprite(imageArray[i].GetIndex());
            if (sprite != null)
            {
                imageArray[i].color = Color.white;
                imageArray[i].sprite = sprite;
            }
            //else
            //{
            //    MyDebug.LogYellow("Can not load Sprite:" + imageArray[i].index);
            //}
        }
    }
    public static Dictionary<string, GameObject> InitGameObjectDict(GameObject root)
    {
        Dictionary<string, GameObject> gameObjectDict = new Dictionary<string, GameObject>();
        Transform[] gameObjectArray = root.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < gameObjectArray.Length; i++)
        {
            try
            {
                gameObjectDict.Add(gameObjectArray[i].name, gameObjectArray[i].gameObject);
            }
            catch
            {
                if (!gameObjectArray[i].name.Contains("Question"))
                {
                    MyDebug.LogYellow(gameObjectArray[i].name);
                }
            }
        }

        return gameObjectDict;
    }
    public static Dictionary<string, RectTransform> InitRectTransformDict(GameObject root)
    {
        Dictionary<string, RectTransform> rectTransformDict = new Dictionary<string, RectTransform>();
        RectTransform[] rectTransformArray = root.GetComponentsInChildren<RectTransform>(true);
        for (int i = 0; i < rectTransformArray.Length; i++)
        {
            rectTransformDict.Add(rectTransformArray[i].name, rectTransformArray[i]);
        }
        return rectTransformDict;
    }
    public static void AddEventTriggerListener(GameObject obj, EventTriggerType eventID, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = obj.AddComponent<EventTrigger>();
        }
        trigger.triggers = new List<EventTrigger.Entry>();
        UnityAction<BaseEventData> callback = new UnityAction<BaseEventData>(action);
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventID;
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }
    public static Rect GetShotTargetRect(RectTransform targetRect)
    {
        float maxWidth = Screen.width;
        float maxHeight = Screen.height;
        float startX = targetRect.anchorMin.x * maxWidth;
        float startY = targetRect.anchorMin.y * maxHeight;
        float width = targetRect.anchorMax.x * maxWidth - startX;
        float height = targetRect.anchorMax.y * maxHeight - startY;
        return new Rect(startX, startY, width, height);
    }
    /// <summary>
    /// Gui水平动画
    /// </summary>
    /// <param name="gui"></param>
    /// <param name="endValue"></param>
    /// <param name="mID"></param>
    /// <param name="isIn"></param>
    public static void GuiHorizontalMove(GameObject gui, float endValue, MoveID mID, CanvasGroup canvasGroup, bool isIn, System.Action completed = null)
    {
        if (!gui)
        {
            MyDebug.LogYellow("gui is NULL!");
            return;
        }
        if (isIn)
        {
            gui.transform.DOLocalMoveX(endValue * (int)mID, tweenDuration, true).
                          From().
                          SetEase(Ease.OutQuint).
                          OnStart(() => canvasGroup.blocksRaycasts = false).
                          OnComplete(() => 
                          {
                              if (completed != null) completed();
                              canvasGroup.blocksRaycasts = true;
                          });
        }
        else
        {
            gui.transform.DOLocalMoveX(endValue * (int)mID, tweenDuration, true).
                          SetEase(Ease.OutQuint).
                          OnStart(() => canvasGroup.blocksRaycasts = false).
                          OnComplete(() =>
                          {
                              if (completed != null) completed();
                              canvasGroup.blocksRaycasts = true;
                              gui.SetActive(false);
                              gui.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                          });
        }
    }
    /// <summary>
    /// Gui垂直动画
    /// </summary>
    /// <param name="gui"></param>
    /// <param name="endValue"></param>
    /// <param name="mID"></param>
    /// <param name="isIn"></param>
    public static void GuiVerticalMove(GameObject gui, float endValue, MoveID mID, CanvasGroup canvasGroup, bool isIn, System.Action completed = null)
    {
        if (isIn)
        {
            gui.transform.DOLocalMoveY(endValue * (int)mID, tweenDuration, true).
                          From().
                          SetEase(Ease.OutQuint).
                          OnStart(() => canvasGroup.blocksRaycasts = false).
                          OnComplete(() =>
                          {
                              if (completed != null) completed();
                              canvasGroup.blocksRaycasts = true;
                          });
        }
        else
        {
            gui.transform.DOLocalMoveY(endValue * (int)mID, tweenDuration, true).
                          SetEase(Ease.OutQuint).
                          OnStart(() => canvasGroup.blocksRaycasts = false).
                          OnComplete(() =>
                          {
                              if (completed != null) completed();
                              canvasGroup.blocksRaycasts = true;
                              gui.SetActive(false);
                              gui.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                          });
        }
    }
    /// <summary>
    /// Gui缩放动画
    /// </summary>
    /// <param name="gui"></param>
    /// <param name="isIn"></param>
    public static void GuiScale(GameObject gui, CanvasGroup canvasGroup, bool isIn, System.Action completed = null)
    {
        if (isIn)
        {
            gui.transform.DOScale(Vector3.zero, tweenDuration).
                          From().
                          SetEase(Ease.OutQuint).
                          OnStart(() => canvasGroup.blocksRaycasts = false).
                          OnComplete(() =>
                          {
                              if (completed != null) completed();
                              canvasGroup.blocksRaycasts = true;
                              gui.transform.localScale = Vector3.one;
                          });
        }
        else
        {
            gui.transform.DOScale(Vector3.zero, tweenDuration).
                          SetEase(Ease.OutQuint).
                          OnStart(() => canvasGroup.blocksRaycasts = false).
                          OnComplete(() =>
                          {
                              if (completed != null) completed();
                              canvasGroup.blocksRaycasts = true;
                              gui.transform.localScale = Vector3.one;
                              gui.SetActive(false);
                          });
        }
    }

    public static int CalculateStar(List<AchievementInstance> instanceList)
    {
        int total = 0;
        for (int i = 0; i < instanceList.Count; i++)
        {
            total += instanceList[i].star;
        }
        return total;
    }

	//TODO：不要每次都new一个stringbuilder
	public static string BytesToString (byte[] bytes)
	{
		StringBuilder sOutput = new StringBuilder(bytes.Length);
		for (int i = 0; i < bytes.Length; i++)
		{
			sOutput.Append(bytes[i].ToString("X2"));  
		}
		return sOutput.ToString();
	}

	public static byte[] StringToBytes(string message)
	{
		return null;
	}

    public static void RefreshScrollContent(RectTransform parent, ArrayList dataList, GuiItemID id, GameObject detailWin = null)
    {
        parent.anchoredPosition = Vector2.zero;
        for(int i = parent.childCount - 1; i >= 0; i--)
        {
            Object.Destroy(parent.GetChild(i).gameObject);
        }
        for (int i = 0; i < dataList.Count; i++)
        {
            GameObject item = GuiController.Instance.GetPrefabItem(id);
            item.name = id.ToString() + i;
            item.SendMessage("InitPrefabItem", dataList[i]);
            if (detailWin) item.SendMessage("InitDetailWin", detailWin);
            item.transform.SetParent(parent);
            item.transform.localScale = Vector3.one;
        }
    }
}
