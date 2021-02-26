using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

public sealed class PlayerController : Controller
{
    #region C#单例
    private static PlayerController instance = null;
    private PlayerController()
    {
        base.id = ControllerID.RecordController;
        MyDebug.LogWhite("Loading Controller:" + id.ToString());

        //PlayerPrefs.DeleteAll();
    }
    public static PlayerController Instance
    {
        get { return instance ?? (instance = new PlayerController()); }
    }
    #endregion

    private PlayerInfo info;

    public string PlayerName
    {
        get
        {
            if (string.IsNullOrEmpty(info?.Name))
            {
                return PlayerPrefs.GetString("PlayerName", "");
            }

            return info.Name;
        }
    }

    public void SetVisitorName(string name)
    {
        PlayerPrefs.SetString("PlayerName", name);
    }

    public void SetPlayerInfo(PlayerInfo info)
    {
        this.info = info;
    }
}
