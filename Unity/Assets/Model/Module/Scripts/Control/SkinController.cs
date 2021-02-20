using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SkinController : Controller 
{
    #region C#单例
    private static SkinController instance = null;
    private SkinController()
    {
        base.id = ControllerID.SkinController;
        path = "Skin/{0}/";
        MyDebug.LogWhite("Loading Controller:" + id.ToString());
    }
    public static SkinController Instance
    {
        get { return instance ?? (instance = new SkinController()); }
    }
    #endregion

    private string path;

    public SkinID CurSkinID
    {
        get
        {
            int skinID = PlayerPrefs.GetInt("SkinID", 0);
            return (SkinID)skinID;
        }
        set
        {
            int skinID = (int)value;
            PlayerPrefs.SetInt("SkinID", skinID);
        }
    }

    public Sprite GetSprite(string index)
    {
        GameObject resouce = Resources.Load<GameObject>(string.Format(path, CurSkinID) + index);
        if (resouce) return resouce.GetComponent<SpriteRenderer>().sprite;
        else return null;
    }
}
