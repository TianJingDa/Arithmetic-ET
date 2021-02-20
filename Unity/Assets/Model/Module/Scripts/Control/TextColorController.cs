using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TextColorController : Controller 
{
    #region C#单例
    private static TextColorController instance = null;
    private TextColorController()
    {
        base.id = ControllerID.SkinController;
        defaultColorDict = new Dictionary<string, string[]>();
        greenColorDict = new Dictionary<string, string[]>();
        pinkColorDict = new Dictionary<string, string[]>();
        blueColorDict = new Dictionary<string, string[]>();
        InitColorData();
        MyDebug.LogWhite("Loading Controller:" + id.ToString());
    }
    public static TextColorController Instance
    {
        get { return instance ?? (instance = new TextColorController()); }
    }
    #endregion
    private Dictionary<string, string[]> defaultColorDict;//默认皮肤的字体颜色
    private Dictionary<string, string[]> greenColorDict;//清新绿皮肤的字体颜色
    private Dictionary<string, string[]> pinkColorDict;//玫瑰红皮肤的字体颜色
    private Dictionary<string, string[]> blueColorDict;//天空蓝皮肤的字体颜色

    private void InitColorData()
    {
        InitColorDict(defaultColorDict, "TextColor/DefaultColor");
        InitColorDict(greenColorDict, "TextColor/GreenColor");
        InitColorDict(pinkColorDict, "TextColor/PinkColor");
        InitColorDict(blueColorDict, "TextColor/BlueColor");
    }
    private void InitColorDict(Dictionary<string, string[]> colorDict,string path)
    {
        TextAsset colorAsset = Resources.Load(path, typeof(TextAsset)) as TextAsset;
        if (colorAsset == null)
        {
            MyDebug.LogYellow("Load File Error!");
            return;
        }
        char[] charSeparators = new char[] { "\r"[0], "\n"[0] };
        string[] lineArray = colorAsset.text.Split(charSeparators, System.StringSplitOptions.RemoveEmptyEntries);
        List<string> lineList;
        for (int i = 0; i < lineArray.Length; i++)
        {
            lineList = new List<string>(lineArray[i].Split(','));
            colorDict.Add(lineList[0], lineList.GetRange(1, 4).ToArray());
        }
    }
    public Color GetColorData(SkinID id, string index)
    {
        Dictionary<string, string[]> colorDict = null;
        switch (id)
        {
            case SkinID.Default:
                colorDict = defaultColorDict;
                break;
            case SkinID.FreshGreen:
                colorDict = greenColorDict;
                break;
            case SkinID.RosePink:
                colorDict = pinkColorDict;
                break;
            case SkinID.SkyBlue:
                colorDict = blueColorDict;
                break;
        }
        string[] colorArray = null;
        colorDict.TryGetValue(index, out colorArray);
        if (colorArray == null)
        {
            MyDebug.LogYellow("Get Color Error! SkinID: " + id.ToString()+ ",index:"+index);
            return Color.white;
        }
        Color color = new Color();
        color.r = float.Parse(colorArray[0]) / 255f;
        color.g = float.Parse(colorArray[1]) / 255f;
        color.b = float.Parse(colorArray[2]) / 255f;
        color.a = float.Parse(colorArray[3]) / 255f;
        return color;
    }

}
