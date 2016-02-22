using UnityEngine;
using System.Collections;

/// <summary>
/// 文件加载路径辅助类
/// </summary>
public class FileLoadPath
{
    #region 获取路径

    /// <summary>
    /// AssetBundle：StreamingAssets 路径,带+"平台/"。
    /// </summary>
    public static string AssetBundlePath
    {
        get
        {

#if UNITY_ANDROID && ! UNITY_EDITOR
               return Application.streamingAssetsPath+"/android/";

#elif UNITY_ANDROID && UNITY_EDITOR
                     return Application.streamingAssetsPath + "/android/";
#elif UNITY_STANDALONE_WIN
            return Application.streamingAssetsPath + "/windows/";
#elif   UNITY_STANDALONE_OSX
                     return  Application.streamingAssetsPath+"/mac/";

#elif UNITY_IPHONE && ! UNITY_EDITOR
                     return  Application.streamingAssetsPath+"/iphone/";
#elif UNITY_IPHONE &&  UNITY_EDITOR
                     return  Application.streamingAssetsPath+"/iphone/";
#else
                    return AppPersistentPath+"/Platform/"; 
#endif
        }
    }

    /// <summary>
    /// 安装包AssetBundle路径Url,带"/"。
    /// </summary>
    public static string AssetBundleURL
    {
        get
        {

#if UNITY_ANDROID && ! UNITY_EDITOR
               return Application.streamingAssetsPath+"/android/";

#elif UNITY_ANDROID && UNITY_EDITOR
            return "file://" + Application.streamingAssetsPath + "/android/";
#elif UNITY_STANDALONE_WIN
                        return "file://" +Application.streamingAssetsPath+"/windows/";
#elif   UNITY_STANDALONE_OSX
                     return  "file://" +Application.streamingAssetsPath+"/mac/";

#elif UNITY_IPHONE && ! UNITY_EDITOR
                     return  "file://" +Application.streamingAssetsPath+"/iphone/";
#elif UNITY_IPHONE &&  UNITY_EDITOR
                     return   "file://" +Application.streamingAssetsPath+"/iphone/";
#else
            return "file://" + Application.streamingAssetsPath + "/Platform/";
#endif
        }
    }

    /// <summary>
    /// SD卡AssetBundle检查路径，带"/"。
    /// </summary>
    public static string PersistentPath
    {
        get
        {

#if UNITY_ANDROID && ! UNITY_EDITOR
               return AppPersistentPath+"/android/";

#elif UNITY_ANDROID && UNITY_EDITOR
            return Application.streamingAssetsPath + "/android/";
#elif UNITY_STANDALONE_WIN
                        return Application.streamingAssetsPath+"/windows/";
#elif   UNITY_STANDALONE_OSX
                     return  Application.streamingAssetsPath+"/mac/";

#elif UNITY_IPHONE && ! UNITY_EDITOR
                     return  AppPersistentPath+"/iphone/";
#elif UNITY_IPHONE &&  UNITY_EDITOR
                     return  Application.streamingAssetsPath+"/iphone/";
#else
            return AppPersistentPath + "/Platform/";
#endif
        }
    }

    /// <summary>
    /// SD卡AssetBundle路径Url，带"/"。
    /// </summary>
    public static string PersistentUrl
    {
        get
        {

#if UNITY_ANDROID && ! UNITY_EDITOR
               return "file://" +AppPersistentPath+"/android/";

#elif UNITY_ANDROID && UNITY_EDITOR
            return "file://" + Application.streamingAssetsPath + "/android/";
#elif UNITY_STANDALONE_WIN
                        return "file://" +Application.streamingAssetsPath+"/windows/";
#elif   UNITY_STANDALONE_OSX
                     return  "file://" +Application.streamingAssetsPath+"/mac/";

#elif UNITY_IPHONE && ! UNITY_EDITOR
                     return  "file://" +AppPersistentPath+"/iphone/";
#elif UNITY_IPHONE &&  UNITY_EDITOR
                     return  "file://" +Application.streamingAssetsPath+"/iphone/";
#else
                    return "file://" +AppPersistentPath+"/Platform/"; 
#endif
        }
    }

    #endregion


    /// <summary>
    /// 获取文件绝对路径
    /// </summary>
    /// <param name="fileUrl">相对路径</param>
    /// <returns></returns>
    public static string GetResFullPath(string filePath)
    {
        //是否在sd卡中
        if (System.IO.File.Exists(PersistentPath + filePath))
        {
            return PersistentUrl + filePath;
        }
        else
        {
            return AssetBundleURL + filePath;
        }
    }
    private static string _appPersistentPath = "";
    /// <summary>
    /// 获取SDK目录
    /// </summary>
    public static string AppPersistentPath
    {
        get
        {
            if (string.IsNullOrEmpty(_appPersistentPath))
            {
                _appPersistentPath = Application.persistentDataPath;
                if (string.IsNullOrEmpty(_appPersistentPath))
                {
                    Logger.LogError("AppPersistentPath null 获取SD卡跟目录!");
                    if (Application.platform == RuntimePlatform.Android)
                    {

                        Logger.LogError("这里添加sdk代码");
						string androidPath ="";//= SDKHelper.getAndroidSDcardRootPath();
                        if (!androidPath.Equals(""))
                        {
                             _appPersistentPath = androidPath;
                        }
                    }
                    if (string.IsNullOrEmpty(_appPersistentPath))
                    {
                        _appPersistentPath = "com.sy599.ntxz";
                        Logger.LogError("AppPersistentPath null " + Application.platform);
                    }
                    else
                    {
                        //最好一个字符是否包含/
                        if (_appPersistentPath.LastIndexOf('/') == _appPersistentPath.Length - 1)
                            _appPersistentPath += "com.sy599.ntxz";
                        else
                            _appPersistentPath += "/com.sy599.ntxz";
                    }

                    Logger.LogError("AppPersistentPath " + Application.platform + ";" + _appPersistentPath);
                }
            }
            return _appPersistentPath;
        }
    }
}
