using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

public class UITool : Editor 
{
    /// <summary>
    /// 使用时需现将物体放在其父物体中心，锚点在0.5，0.5处
    /// </summary>
    [MenuItem("Custom Editor/将锚点设为本身大小")]
    public static void AnchorFitter()
    {
        for(int i=0;i< Selection.gameObjects.Length; i++)
        {
            RectTransform target = Selection.gameObjects[i].transform as RectTransform;
            RectTransform targetParent = Selection.gameObjects[i].transform.parent as RectTransform;
            if (target != null && targetParent != null)
            {
                float deltaX = target.localPosition.x;
                float deltaY = target.localPosition.y;
                float minX = 0.5f * (1 - target.rect.width / targetParent.rect.width) + deltaX / targetParent.rect.width;
                float minY = 0.5f * (1 - target.rect.height / targetParent.rect.height) + deltaY / targetParent.rect.height;
                float maxX = 0.5f * (1 + target.rect.width / targetParent.rect.width) + deltaX / targetParent.rect.width;
                float maxY = 0.5f * (1 + target.rect.height / targetParent.rect.height) + deltaY / targetParent.rect.height;
                target.anchorMin = new Vector2(minX, minY);
                target.anchorMax = new Vector2(maxX, maxY);
                target.offsetMin = Vector2.zero;
                target.offsetMax = Vector2.zero;
            }

        }
    }

    [MenuItem("Custom Editor/显示UI信息")]
    public static void ShowUIInfo()
    {
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            RectTransform target = Selection.gameObjects[i].transform as RectTransform;
            RectTransform targetParent = Selection.gameObjects[i].transform.parent as RectTransform;
            if (target != null && targetParent != null)
            {
                Debug.Log("target.rect.min:" + target.rect.min.ToString());
                Debug.Log("target.rect.max:" + target.rect.max.ToString());
                Debug.Log("target.offsetMax:" + target.offsetMax.ToString());
                Debug.Log("target.offsetMin:" + target.offsetMin.ToString());
                Debug.Log("target.anchorMax:" + target.anchorMax.ToString());
                Debug.Log("target.anchorMin:" + target.anchorMin.ToString());
                Debug.Log("target.rect.x:" + target.rect.x.ToString());
                Debug.Log("target.rect.y:" + target.rect.y.ToString());
                Debug.Log("target.rect.xMin:" + target.rect.xMin.ToString());
                Debug.Log("target.rect.xMax:" + target.rect.xMax.ToString());
                Debug.Log("target.rect.yMin:" + target.rect.yMin.ToString());
                Debug.Log("target.rect.yMax:" + target.rect.yMax.ToString());
                Debug.Log("target.rect.position:" + target.rect.position.ToString());
                Debug.Log("target.position:" + target.position.ToString());
                Debug.Log("target.localPosition:" + target.localPosition.ToString());
                Debug.Log("target.anchoredPosition:" + target.anchoredPosition.ToString());
            }

        }
    }
    [MenuItem("Custom Editor/将锚点设为本身大小（水平）")]
    public static void AnchorFitter_H()
    {
        for(int i=0;i< Selection.gameObjects.Length; i++)
        {
            RectTransform target = Selection.gameObjects[i].transform as RectTransform;
            RectTransform targetParent = Selection.gameObjects[i].transform.parent as RectTransform;
            if (target != null && targetParent != null)
            {
                float width = target.rect.width;
                float height = target.rect.height;
                float deltaX = target.localPosition.x;
                float deltaY = target.localPosition.y;
                float minX = 0.5f * (1 - target.rect.width / targetParent.rect.width) + deltaX / targetParent.rect.width;
                float minY = 0.5f * (1 - target.rect.height / targetParent.rect.height) + deltaY / targetParent.rect.height;
                float maxX = 0.5f * (1 + target.rect.width / targetParent.rect.width) + deltaX / targetParent.rect.width;
                float maxY = 0.5f * (1 + target.rect.height / targetParent.rect.height) + deltaY / targetParent.rect.height;
                target.anchorMin = new Vector2(minX, (maxY + minY) / 2);
                target.anchorMax = new Vector2(maxX, (maxY + minY) / 2);
                target.offsetMin = new Vector2(0, -(height / 2));
                target.offsetMax = new Vector2(0, (height / 2));
            }
        }
    }
    [MenuItem("Custom Editor/将锚点设为本身大小（垂直）")]
    public static void AnchorFitter_V()
    {
        for(int i=0;i< Selection.gameObjects.Length; i++)
        {
            RectTransform target = Selection.gameObjects[i].transform as RectTransform;
            RectTransform targetParent = Selection.gameObjects[i].transform.parent as RectTransform;
            if (target != null && targetParent != null)
            {
                float width = target.rect.width;
                float height = target.rect.height;
                float deltaX = target.localPosition.x;
                float deltaY = target.localPosition.y;
                float minX = 0.5f * (1 - target.rect.width / targetParent.rect.width) + deltaX / targetParent.rect.width;
                float minY = 0.5f * (1 - target.rect.height / targetParent.rect.height) + deltaY / targetParent.rect.height;
                float maxX = 0.5f * (1 + target.rect.width / targetParent.rect.width) + deltaX / targetParent.rect.width;
                float maxY = 0.5f * (1 + target.rect.height / targetParent.rect.height) + deltaY / targetParent.rect.height;
                target.anchorMin = new Vector2((minX + maxX) / 2, minY);
                target.anchorMax = new Vector2((minX + maxX) / 2, maxY);
                target.offsetMin = new Vector2(-(width / 2), 0);
                target.offsetMax = new Vector2((width / 2), 0);
            }
        }
    }
    [MenuItem("Custom Editor/将锚点设为本身中心")]
    public static void AnchorCenter()
    {
        for(int i=0;i< Selection.gameObjects.Length; i++)
        {
            RectTransform target = Selection.gameObjects[0].transform as RectTransform;
            RectTransform targetParent = Selection.gameObjects[0].transform.parent as RectTransform;
            if (target != null && targetParent != null)
            {
                float width = target.rect.width;
                float height = target.rect.height;
                float X = 0.5f + target.localPosition.x / targetParent.rect.width;
                float Y = 0.5f + target.localPosition.y / targetParent.rect.height;
                target.anchorMin = new Vector2(X, Y);
                target.anchorMax = new Vector2(X, Y);
                target.anchoredPosition = Vector2.zero;
            }
        }
    }
    [MenuItem("Custom Editor/恢复TextIndex")]
    public static void RecoverTextIndex()
    {
        return;
        //for(int i = 0; i < Selection.gameObjects.Length; i++)
        //{
        //    Text[] textArray = Selection.gameObjects[i].GetComponentsInChildren<Text>(true);
        //    for(int j = 0; j < textArray.Length; j++)
        //    {
        //        if(!string.IsNullOrEmpty(textArray[j].text) && textArray[j].text.Contains("Text_"))
        //        {
        //            textArray[j].index = textArray[j].text;
        //        }
        //    }
        //}
    }
    [MenuItem("Custom Editor/恢复ImageIndex")]
    public static void RecoverImageIndex()
    {
        return;
        //int num = 0;
        //Dictionary<string, string> imageDict = InitImageData();
        //for (int i = 0; i < Selection.gameObjects.Length; i++)
        //{
        //    Image[] imageArray = Selection.gameObjects[i].GetComponentsInChildren<Image>(true);
        //    for (int j = 0; j < imageArray.Length; j++)
        //    {
        //        if (imageDict.ContainsKey(imageArray[j].name))
        //        {
        //            imageArray[j].index = imageDict[imageArray[j].name];
        //            num++;
        //        }
        //    }
        //}
        //MyDebug.LogGreen("num:" + num);
    }
    private static Dictionary<string,string> InitImageData()
    {
        TextAsset imageAsset = Resources.Load("Language/Image", typeof(TextAsset)) as TextAsset;
        if (imageAsset == null)
        {
            MyDebug.LogYellow("Load File Error!");
            return null;
        }
        char[] charSeparators = new char[] { "\r"[0], "\n"[0] };
        string[] lineArray = imageAsset.text.Split(charSeparators, System.StringSplitOptions.RemoveEmptyEntries);
        List<string> lineList;
        Dictionary<string, string> imageDict = new Dictionary<string, string>();
        for (int i = 0; i < lineArray.Length; i++)
        {
            lineList = new List<string>(lineArray[i].Split(','));
            imageDict.Add(lineList[1], lineList[0]);
        }

        return imageDict;
    }
    [MenuItem("Custom Editor/抓取布局")]
    public static void GetLayout()
    {
        Dictionary<string, MyRectTransform> rectTransformDict = new Dictionary<string, MyRectTransform>();
        RectTransform[] rectTransformArray = Selection.gameObjects[0].GetComponentsInChildren<RectTransform>(true);
        for (int i = 0; i < rectTransformArray.Length; i++)
        {
            if (rectTransformArray[i].name.Contains("FightFrame")) continue;
            MyRectTransform rect = new MyRectTransform();
            rect.pivot = new MyVector2(rectTransformArray[i].pivot.x, rectTransformArray[i].pivot.y);
            rect.anchorMax = new MyVector2(rectTransformArray[i].anchorMax.x, rectTransformArray[i].anchorMax.y);
            rect.anchorMin = new MyVector2(rectTransformArray[i].anchorMin.x, rectTransformArray[i].anchorMin.y);
            rect.offsetMax = new MyVector2(rectTransformArray[i].offsetMax.x, rectTransformArray[i].offsetMax.y);
            rect.offsetMin = new MyVector2(rectTransformArray[i].offsetMin.x, rectTransformArray[i].offsetMin.y);
            rect.localEulerAngles = new MyVector3(rectTransformArray[i].localEulerAngles.x, rectTransformArray[i].localEulerAngles.y, rectTransformArray[i].localEulerAngles.z);

            rectTransformDict.Add(rectTransformArray[i].name, rect);
        }
        string path = Application.dataPath + "/Temp/Vertical/Left_Pad.txt";
        LayoutDataWrapper wrapper = ConvertToDataWrapper(rectTransformDict);
        string toSave = JsonUtility.ToJson(wrapper);
        CommonTool.SetData(path, toSave);
    }

    private static LayoutDataWrapper ConvertToDataWrapper(Dictionary<string, MyRectTransform> rectTransformDict)
    {
        LayoutDataWrapper wrapper = new LayoutDataWrapper();
        wrapper.names.AddRange(rectTransformDict.Keys);
        wrapper.transforms.AddRange(rectTransformDict.Values);
        return wrapper;
    }

    [MenuItem("Custom Editor/生成试题")]
    public static void ProduceQuestion()
    {
        List<List<int>> questionList = new List<List<int>>();
        for(int i = 10; i < 1000; i++)
        {
            for(int j = i + 1; j < 1000; j++)
            {
                if (i * j % 10 != 0 && i * j > 1000 && i * j < 10000)
                {
                    List<int> list = new List<int>();
                    list.Add(i);
                    list.Add(j);
                    questionList.Add(list);
                }

                //for (int k = j + 1; k < 100; k++)
                //{
                //}
            }
        }
        //Debug.Log(questionList.Count.ToString());
        DivisionDataBase data = new DivisionDataBase();
        data.digitID = DigitID.FourDigits;
        data.operandID = OperandID.TwoNumbers;
        //data.questionList = questionList;
        string path = Application.dataPath + "/Resources/FightData/d_4_2List.txt";
        //IOHelper.SetData(path, data);
    }
    [MenuItem("Custom Editor/生成成就")]
    public static void ExchangeAchievement()
    {
        string addressList = Application.dataPath + "/Achievement.txt";
        string targetList = Application.dataPath + "/Resources/Achievement/Achievement.txt";
        StreamReader mutiLanguageAsset = new StreamReader(addressList);
        if (mutiLanguageAsset == null)
        {
            MyDebug.LogYellow("Can not find: " + addressList);
            return;
        }
        char[] charSeparators = new char[] { "\r"[0], "\n"[0] };
        string asset = mutiLanguageAsset.ReadToEnd();
        string[] lineArray = asset.Split(charSeparators, System.StringSplitOptions.RemoveEmptyEntries);
        string[] lineList;
        List<AchievementInstance> achList = new List<AchievementInstance>();
        for (int j = 1; j < lineArray.Length; j++)
        {
            AchievementInstance instance = new AchievementInstance();
            lineList = lineArray[j].Split(',');
            instance.achievementName = lineList[0];
            float.TryParse(lineList[1], out instance.accuracy);
            float.TryParse(lineList[2], out instance.meanTime);
            instance.mainTitleIndex = lineList[3];
            instance.subTitleIndex = lineList[4];
            instance.imageIndex = lineList[5];
            instance.chapterImageIndex = lineList[6];
            int patternID = 0;
            int amountID = 0;
            int symbolID = 0;
            int digitID = 0;
            int operandID = 0;
            int.TryParse(lineList[7], out patternID);
            int.TryParse(lineList[8], out amountID);
            int.TryParse(lineList[9], out symbolID);
            int.TryParse(lineList[10], out digitID);
            int.TryParse(lineList[11], out operandID);
            CategoryInstance cInstance = new CategoryInstance(patternID, amountID, symbolID, digitID, operandID);
            instance.cInstance = cInstance;
            int.TryParse(lineList[12], out instance.difficulty);
            instance.finishTime = "";
            instance.star = 0;
            achList.Add(instance);
        }
        if (File.Exists(targetList)) File.Delete(targetList);
        string toSave = ETModel.JsonHelper.ToListJson(achList);
        CommonTool.SetData(targetList, toSave);

    }
    [MenuItem("Custom Editor/转换prefab/Default")]
    public static void DefaultPrefab()
    {
        string spriteDir = Application.dataPath + "/Resources/Skin/Default";

        if (!Directory.Exists(spriteDir))
        {
            Directory.CreateDirectory(spriteDir);
        }

        DirectoryInfo rootDirInfo = new DirectoryInfo(Application.dataPath + "/Image/Default");
        foreach (DirectoryInfo dirInfo in rootDirInfo.GetDirectories())
        {
            foreach (FileInfo pngFile in dirInfo.GetFiles("*.png", SearchOption.AllDirectories))
            {
                string allPath = pngFile.FullName;
                string assetPath = allPath.Substring(allPath.IndexOf("Assets"));
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                GameObject go = new GameObject(sprite.name);
                go.AddComponent<SpriteRenderer>().sprite = sprite;
                allPath = spriteDir + "/" + sprite.name + ".prefab";
                if (File.Exists(allPath)) File.Delete(allPath);
                string prefabPath = allPath.Substring(allPath.IndexOf("Assets"));
                PrefabUtility.CreatePrefab(prefabPath, go);
                DestroyImmediate(go);
            }
        }
    }

    [MenuItem("Assets/单个转换DefaultImage")]
    public static void AssetsCommonImage()
    {
        string spriteDir = Application.dataPath + "/Resources/Skin/Default";

        if (!Directory.Exists(spriteDir))
        {
            Directory.CreateDirectory(spriteDir);
        }

        for (int i = 0; i < Selection.objects.Length; i++)
        {
            string path = AssetDatabase.GetAssetPath(Selection.objects[i]);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (!sprite)
            {
                MyDebug.LogYellow("Can not get SPRITE!");
                return;
            }
            GameObject go = new GameObject(sprite.name);
            go.AddComponent<SpriteRenderer>().sprite = sprite;
            string allPath = spriteDir + "/" + sprite.name + ".prefab";
            if (File.Exists(allPath)) File.Delete(allPath);
            string prefabPath = allPath.Substring(allPath.IndexOf("Assets"));
            PrefabUtility.CreatePrefab(prefabPath, go);
            DestroyImmediate(go);
        }
    }

    [MenuItem("Custom Editor/查找无用UISprite")]
    public static void FindUISprite()
    {
        Transform[] objArray = Selection.gameObjects[0].GetComponentsInChildren<Transform>(true);
        for(int i = 0; i < objArray.Length; i++)
        {
            Image image = objArray[i].GetComponent<Image>();
            if (image && image.sprite && image.sprite.name == "UISprite" && image.color == Color.white)
            {
                string path = GetPath(objArray[i]);
                MyDebug.LogYellow(path); 
            } 

        }
    }

    [MenuItem("Custom Editor/清除数据")]
    public static void ClearPlayerData()
    {
        PlayerPrefs.DeleteAll();
    }

    private static string GetPath(Transform tra)
    {
        string path = tra.name;
        Transform parent = tra.parent;
        while (parent)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        return path;
    }


    [MenuItem("Assets/转换prefab-Default")]
    public static void AssetsDefaultPrefab()
    {
        string spriteDir = Application.dataPath + "/Resources/Skin/Default";

        if (!Directory.Exists(spriteDir))
        {
            Directory.CreateDirectory(spriteDir);
        }

        for(int i = 0; i < Selection.objects.Length; i++)
        {
            string path = AssetDatabase.GetAssetPath(Selection.objects[i]);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (!sprite)
            {
                MyDebug.LogYellow("Can not get SPRITE!");
                return;
            }
            GameObject go = new GameObject(sprite.name);
            go.AddComponent<SpriteRenderer>().sprite = sprite;
            string allPath = spriteDir + "/" + sprite.name + ".prefab";
            if (File.Exists(allPath)) File.Delete(allPath);
            string prefabPath = allPath.Substring(allPath.IndexOf("Assets"));
            PrefabUtility.CreatePrefab(prefabPath, go);
            DestroyImmediate(go);
        }
    }
}
