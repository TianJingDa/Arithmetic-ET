using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;




/// <summary>
/// 用于资源方面的转移
/// </summary>
public static class AssetHelper 
{
    /// <summary>  
    /// 将streaming path 下的文件copy到对应用  
    /// 为什么不直接用io函数拷贝，原因在于streaming目录不支持，  
    /// 不管理是用getStreamingPath_for_www，还是Application.streamingAssetsPath，  
    /// io方法都会说文件不存在  
    /// </summary>  
    /// <param name="fileName"></param>  
    public static IEnumerator CopyImage(string fileName)
    {
        string desDir = Application.persistentDataPath + "/Image/";
        if (!Directory.Exists(desDir)) Directory.CreateDirectory(desDir);
        string des = desDir + fileName;
        string src = GetStreamingPathForWWW() + "/Image/" +fileName;
        WWW www = new WWW(src);
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            MyDebug.LogYellow("www.error:" + www.error);
        }
        else
        {
            if (File.Exists(des))
            {
                File.Delete(des);
            }
            FileStream fsDes = File.Create(des);
            fsDes.Write(www.bytes, 0, www.bytes.Length);
            fsDes.Flush();
            fsDes.Close();
        }
        www.Dispose();
    }

    public static IEnumerator LoadImage(string path, Image image)
    {
        WWW www = new WWW(path);
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            MyDebug.LogYellow("www.error:" + www.error);
        }
        image.sprite = Sprite.Create(www.texture, new Rect(new Vector2(0, 0), new Vector2(www.texture.width, www.texture.height)), Vector2.zero);
        www.Dispose();
    }
    /// <summary>
    /// 这三个路径暂时看好使，以后待检验
    /// </summary>
    /// <returns></returns>
    public static string GetStreamingPathForWWW()
    {
        string pre = "file://";
    #if UNITY_EDITOR
        pre = "file://";
    #elif UNITY_ANDROID
        pre = "";  
    #elif UNITY_IPHONE
        pre = "file://";  
    #endif
        string path = pre + Application.streamingAssetsPath;
        return path;
    }
    /// <summary>
    /// 这三个路径暂时看好使，以后待检验
    /// </summary>
    /// <returns></returns>
    public static string GetPersistentPathForWWW()
    {
        string pre = "file://";
    #if UNITY_EDITOR || UNITY_STANDALONE_WIN
        pre = "file:///";
    #elif UNITY_ANDROID
        pre = "file://";  
    #elif UNITY_IPHONE
        pre = "file://";  
    #endif
        string path = pre + Application.persistentDataPath;
        return path;
    }
}
