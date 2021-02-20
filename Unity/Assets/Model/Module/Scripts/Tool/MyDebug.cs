using UnityEngine;

public static class MyDebug 
{
    private static void Log(object message)
    {
#if UNITY_EDITOR || SHOW_DEBUG
        Debug.Log("@TJD@" + message);
#endif
    }

    public static void LogWhite(object message)
    {
        Log(message);
    }
    public static void LogGreen(object message)
    {
        Log("<color=green>" + message + "</color>");
    }
    public static void LogYellow(object message)
    {
        Log("<color=yellow>" + message + "</color>");
    }
}
