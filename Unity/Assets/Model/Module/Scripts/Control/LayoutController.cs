using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public sealed class LayoutController : Controller 
{
    #region C#单例
    private static LayoutController instance = null;
    private LayoutController()
    {
        base.id = ControllerID.LayoutController;
        layoutAssetDict = new Dictionary<LayoutID, List<Dictionary<string, MyRectTransform>>>();
        InitLayoutData();
        MyDebug.LogWhite("Loading Controller:" + id.ToString());
    }
    public static LayoutController Instance
    {
        get { return instance ?? (instance = new LayoutController()); }
    }
    #endregion
    private Dictionary<LayoutID, List<Dictionary<string, MyRectTransform>>> layoutAssetDict;
    private void InitLayoutData()
    {
        string pad = IsPad ? "_Pad" : "";

        string data = CommonTool.GetDataFromResources("Layout/Vertical/Right" + pad);
        LayoutDataWrapper wrapper = JsonUtility.FromJson<LayoutDataWrapper>(data);
        Dictionary<string, MyRectTransform> vertical_Right = ConvertToDict(wrapper);

        data = CommonTool.GetDataFromResources("Layout/Vertical/Left" + pad);
        wrapper = JsonUtility.FromJson<LayoutDataWrapper>(data);
        Dictionary<string, MyRectTransform> vertical_Left = ConvertToDict(wrapper);

        data = CommonTool.GetDataFromResources("Layout/Horizontal/Right"+ pad);
        wrapper = JsonUtility.FromJson<LayoutDataWrapper>(data);
        Dictionary<string, MyRectTransform> horizontal_Right = ConvertToDict(wrapper);

        data = CommonTool.GetDataFromResources("Layout/Horizontal/Left"+ pad);
        wrapper = JsonUtility.FromJson<LayoutDataWrapper>(data);
        Dictionary<string, MyRectTransform> horizontal_Left = ConvertToDict(wrapper);

        layoutAssetDict.Add(LayoutID.Vertical, new List<Dictionary<string, MyRectTransform>> { vertical_Right, vertical_Left });
        layoutAssetDict.Add(LayoutID.Horizontal, new List<Dictionary<string, MyRectTransform>> { horizontal_Right, horizontal_Left });
    }

    private bool IsPad
    {
        get
        {
            float width = Screen.width;
            float height = Screen.height;
            return width / height > 0.6f;
        }
    }

    public LayoutID CurLayoutID
    {
        get
        {
            int layoutID = PlayerPrefs.GetInt("LayoutID", 0);
            return (LayoutID)layoutID;
        }
        set
        {
            int layout = (int)value;
            PlayerPrefs.SetInt("LayoutID", layout);
        }
    }

    public HandednessID CurHandednessID
    {
        get
        {
            int handednessID = PlayerPrefs.GetInt("HandednessID", 0);
            return (HandednessID)handednessID;
        }
        set
        {
            int handednessID = (int)value;
            PlayerPrefs.SetInt("HandednessID", handednessID);
        }
    }

    public KeyboardID CurKeyboardID
    {
        get
        {
            int keyboardID = PlayerPrefs.GetInt("KeyboardID", 0);
            return (KeyboardID)keyboardID;
        }
        set
        {
            int keyboardID = (int)value;
            PlayerPrefs.SetInt("KeyboardID", keyboardID);
        }
    }

    private Dictionary<string, MyRectTransform> ConvertToDict(LayoutDataWrapper wrapper)
    {
        Dictionary<string, MyRectTransform> dict = new Dictionary<string, MyRectTransform>();

        for(int i = 0; i < Mathf.Min(wrapper.names.Count, wrapper.transforms.Count); i++)
        {
            dict[wrapper.names[i]] = wrapper.transforms[i];
        }

        return dict;
    }

    public Dictionary<string, MyRectTransform> GetLayoutData()
    {
        return layoutAssetDict[CurLayoutID][(int)CurHandednessID];
    }
}

[Serializable]
public class LayoutDataWrapper
{
    public List<string> names;
    public List<MyRectTransform> transforms;

    public LayoutDataWrapper()
    {
        names = new List<string>();
        transforms = new List<MyRectTransform>();
    }
}

[Serializable]
public class MyRectTransform
{
    public MyVector2 pivot;
    public MyVector2 anchorMax;
    public MyVector2 anchorMin;
    public MyVector2 offsetMax;
    public MyVector2 offsetMin;
    public MyVector3 localEulerAngles;
}
[Serializable]
public class MyVector2
{
    public float x;
    public float y;
    public MyVector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}
[Serializable]
public class MyVector3
{
    public float x;
    public float y;
    public float z;
    public MyVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}
