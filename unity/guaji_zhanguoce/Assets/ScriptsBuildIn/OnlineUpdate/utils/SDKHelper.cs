using System;
using UnityEngine;
using System.Collections;
using LitJson;
using System.Runtime.InteropServices;
/// <summary>
/// buildIn里sdk关联类
/// </summary>
class SDKHelper
{
    [DllImport("__Internal")]
    private static extern void _initApp();
    [DllImport("__Internal")]
    private static extern string _getAppVersionCode();
     [DllImport("__Internal")]
    private static extern string _updatePf();
	[DllImport("__Internal")]
     private static extern string _SDKGetLoginURL();

	/// <summary>
     /// 获取登录地址
     /// </summary>
     /// <returns></returns>
     public static string getLoginURL()
     {
         #if UNITY_EDITOR
         Logger.LogError("这里需要写入默认的登陆地址");
         return "";
               // return GameConfig.getLoginURL();
         #elif UNITY_ANDROID
		 try{
                using (AndroidJavaClass jc = new AndroidJavaClass("net.sy599.common.SDKHelper"))
                {
                    string url = jc.CallStatic<string>("getLoginURL");
                    return url;
				}
         }catch(Exception e){
             return GameConfig.getLoginURL();
                }
            #elif UNITY_IOS
                  if (Application.platform != RuntimePlatform.OSXEditor) {
                                return _SDKGetLoginURL();
                    }
                
            #endif
         Logger.LogError("这里需要写入默认的登陆地址");
         return "";
           // return GameConfig.getLoginURL();
     }
    
    /// <summary>
    /// 获得app版本号
    /// </summary>
    /// <returns></returns>
    public static int getAppVersionCode()
    {
        #if UNITY_EDITOR
              return 0;
        #elif UNITY_ANDROID
                    try
                    {
                        using (AndroidJavaClass jc = new AndroidJavaClass("net.sy599.common.AndroidUtils"))
                        {
                            return (int)(jc.CallStatic<int>("getAppVersionCode"));
                        }
                    }
                    catch
                    {
                         Logger.LogError("取得android versionCode error");
                        return 0;
                    }
        #elif UNITY_IOS
                if (Application.platform != RuntimePlatform.OSXEditor) {
                    return int.Parse(_getAppVersionCode());
                }
        #endif
        return 0;
    }
    /// <summary>
    /// 调用sdk里的初始化代码，可能是初始化sdk,因为平台要求是进入游戏就初始化sdk
    /// </summary>
    /// <returns></returns>
    public static bool initApp()
    {
        #if UNITY_EDITOR
             return false;
        #elif UNITY_ANDROID 
                       try{
                            using (AndroidJavaClass jc = new AndroidJavaClass("net.sy599.common.SDKHelper"))
                            {
                                jc.CallStatic("initApp");
                            }
                            return true;
                         }catch(System.Exception ex){
                        }
        #elif UNITY_IOS
		                if (Application.platform != RuntimePlatform.OSXEditor) {
			                _initApp();
                            return true;
		                }
        #endif
        return false;
    }
    /// <summary>
    /// 获取系统剩余内存
    /// </summary>
    /// <returns></returns>
    public static float GetSystemUnusedMemory()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
           try{
                using (AndroidJavaClass jc = new AndroidJavaClass("net.sy599.common.AndroidUtils"))
                {
                    return (float)(jc.CallStatic<float>("GetUnuseMemory") / (1024*1024));
                }
            }catch(Exception ex){
                return 1024f;
            }
            return 1024f;
        #else
                return 1024f;
        #endif
    }
    /// <summary>
    /// 取得服务器更新接口地址
    /// </summary>
    /// <returns></returns>
    public static string GetUrl(string waiNetUrl, string mParam)
    {
        string url = "";
        #if UNITY_EDITOR
            url = waiNetUrl + mParam + "p=yiwan";
        #elif UNITY_ANDROID
                try{
                    using (AndroidJavaClass jc = new AndroidJavaClass("net.sy599.common.SDKHelper"))
                    {
                        url = jc.CallStatic<string>("getLoginURL") + mParam + "p=" + jc.CallStatic<string>("updatePf");
                    }
                }catch(System.Exception ex){
                    Logger.LogError("取得android GetUrl error");
                    url = waiNetUrl + mParam + "p=yiwan";
                }
        #elif UNITY_IOS
                if (Application.platform != RuntimePlatform.OSXEditor)
                {
                    try
                    {
                        url =   _SDKGetLoginURL() + mParam + "p="+_updatePf();
                    }
                    catch (Exception ex)
                    {
                        url = waiNetUrl + mParam + "p=ywios";
                        Logger.LogError("获取_updatePf出错");
                    }
                }
                else
                {
                    url = waiNetUrl + mParam + "p=ywios";
                }
        #else
                url = waiNetUrl + mParam + "p=yiwan";
        #endif
        return url;
    }
    /// <summary>
    /// 获取sd卡根目录
    /// </summary>
    /// <returns></returns>
    public static string getAndroidSDcardRootPath()
    {
        string appPersistentPath = "";
        try
        {
            #if UNITY_ANDROID && !UNITY_EDITORS
                using (AndroidJavaClass jc = new AndroidJavaClass("net.sy599.common.AndroidUtils"))
                {
                    appPersistentPath = jc.CallStatic<string>("getAndroidSDcardRootPath");
                }
            #endif
        }
        catch (System.Exception ex)
        {
            Logger.LogError("AppPersistentPath Android Exception:" + ex.Message);
        }
        return appPersistentPath;
    }
}

