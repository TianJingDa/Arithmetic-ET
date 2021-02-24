using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class FontController : Controller 
{
    #region C#单例
    private static FontController instance = null;
    private FontController()
    {
        base.id = ControllerID.FontController;
        path = "Font/{0}/{1}";
        MyDebug.LogWhite("Loading Controller:" + id.ToString());
    }
    public static FontController Instance
    {
        get { return instance ?? (instance = new FontController()); }
    }
    #endregion

    private string path;

    public Font GetFont(SkinID sID,LanguageID lID)
    {
        GameObject resouce = Resources.Load<GameObject>(string.Format(path, sID, lID));
        return resouce.GetComponent<Text>().font;
    }

}
