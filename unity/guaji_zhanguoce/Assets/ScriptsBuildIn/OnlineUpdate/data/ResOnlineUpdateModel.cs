using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace update
{
    /// <summary>
    /// 在线更新配套数据类,包括路径等等
    /// </summary>
    public class ResOnlineUpdateModel
    {
        /// <summary>
        /// 本地版本txt 文件路径
        /// </summary>
        private const string _codeTxtPath = "code.txt";
        public string codeTxtPath
        {
            get
            {
                return _codeTxtPath;
            }
        }
        /// <summary>
        /// 本地资源版本txt 文件路径
        /// </summary>
        private const string _versionTxtPath = "version.txt";

        public string versionTxtPath
        {
            get
            {
                return _versionTxtPath;
            }
        }
        /// <summary>
        /// 本地版本文件路径
        /// </summary>
        /// <returns></returns>
        public string getLocalCodeFileUrl()
        {
            string url = "";
            //是否在sd卡中
            if (File.Exists(VersionPersistentPath + codeTxtPath))
            {
                url = VersionPersistentUrl + codeTxtPath;
            }
            else
            {
                // 肯定是在游戏客户端 AB 中
                url = VersionAssetBundleURL + codeTxtPath;
            }
            return url;
        }
        /// <summary>
        /// 本地版本文件路径
        /// </summary>
        /// <returns></returns>
        public string getLocalVersionFileUrl()
        {
            string url = "";
            //是否在sd卡中
            if (File.Exists(VersionPersistentPath + versionTxtPath))
            {
                url = VersionPersistentUrl + versionTxtPath;
            }
            else
            {
                // 肯定是在游戏客户端 AB 中
                url = VersionAssetBundleURL + versionTxtPath;
            }
            return url;
        }




        private const string mParam = "user!resversion.guajilogin?";
        private string waiNetUrl = "http://testxsg.sy599.com/Guaji3DLogin/"; //"http://test.ntxz.kx7p.com/ntxzlogin/";
        /// <summary>
        /// 取得服务器更新接口地址
        /// </summary>
        /// <returns></returns>
        public string GetUrl()
        {

            return SDKHelper.GetUrl(waiNetUrl, mParam);
        }

        #region Version 路径
        private string _resServerUrl = string.Empty;
        /// <summary>
        /// 资源服务器url地址
        /// </summary>
        public string ResServerUrl
        {
            set { _resServerUrl = value; }
            get
            {
#if UNITY_ANDROID
                return _resServerUrl + "android/";
#elif UNITY_STANDALONE_WIN
                                return _resServerUrl + "windows/";
#elif UNITY_IPHONE 
                                return  _resServerUrl + "iphone/";

#elif   UNITY_STANDALONE_OSX
                                return  _resServerUrl + "mac/";
#else
                            return _resServerUrl + "Platform/";
#endif
            }
        }
        /// <summary>
        /// Version.txt 服务器URL地址
        /// </summary>
        public string ResServerVersionUrl
        {
            get
            {

#if UNITY_ANDROID
                return _resServerUrl + "Version/android/version.txt";
#elif UNITY_STANDALONE_WIN
                                return _resServerUrl + "Version/windows/version.txt";
#elif UNITY_IPHONE 
                                return  _resServerUrl + "Version/iphone/version.txt";

#elif   UNITY_STANDALONE_OSX
                                return  _resServerUrl + "Version/mac/version.txt";
#else
                            return _resServerUrl + "Version/Platform/version.txt";
#endif


            }
        }

        /// <summary>
        /// Version sd卡 path，带"/"。
        /// </summary>
        public string VersionPersistentPath
        {
            get
            {

#if UNITY_ANDROID && ! UNITY_EDITOR
                               return FileLoadPath.AppPersistentPath+"/Version/android/";

#elif UNITY_ANDROID && UNITY_EDITOR
                return Application.streamingAssetsPath + "/Version/android/";
#elif UNITY_STANDALONE_WIN
                                        return Application.streamingAssetsPath+"/Version/windows/";
#elif   UNITY_STANDALONE_OSX
                                     return  Application.streamingAssetsPath+"/Version/mac/";

#elif UNITY_IPHONE && ! UNITY_EDITOR
                                     return  FileLoadPath.AppPersistentPath+"/Version/iphone/";
#elif UNITY_IPHONE &&  UNITY_EDITOR
                                     return  Application.streamingAssetsPath+"/Version/iphone/";
#else
                            return FileLoadPath.AppPersistentPath + "/Version/Platform/";
#endif
            }
        }

        /// <summary>
        /// Version sd卡 path，带"/"。
        /// </summary>
        public string sdResourcePath
        {
            get
            {

#if UNITY_ANDROID && ! UNITY_EDITOR
                                           return FileLoadPath.AppPersistentPath+"/";

#elif UNITY_ANDROID && UNITY_EDITOR
                return Application.streamingAssetsPath + "/";
#elif UNITY_STANDALONE_WIN
                                                    return Application.streamingAssetsPath+"/";
#elif   UNITY_STANDALONE_OSX
                                                 return  Application.streamingAssetsPath+"/";

#elif UNITY_IPHONE && ! UNITY_EDITOR
                                                 return  FileLoadPath.AppPersistentPath+"/";
#elif UNITY_IPHONE &&  UNITY_EDITOR
                                                 return  Application.streamingAssetsPath+"/";
#else
                                        return FileLoadPath.AppPersistentPath + "/";
#endif
            }
        }


        /// <summary>
        /// Version sd卡 url，带"/"。
        /// </summary>
        public string VersionPersistentUrl
        {
            get
            {

#if UNITY_ANDROID && ! UNITY_EDITOR
                           return "file://" +FileLoadPath.AppPersistentPath+"/Version/android/";

#elif UNITY_ANDROID && UNITY_EDITOR
                return "file://" + Application.streamingAssetsPath + "/Version/android/";
#elif UNITY_STANDALONE_WIN
                                    return "file://" +Application.streamingAssetsPath+"/Version/windows/";
#elif   UNITY_STANDALONE_OSX
                                 return  "file://" +Application.streamingAssetsPath+"/Version/mac/";

#elif UNITY_IPHONE && ! UNITY_EDITOR
                                 return  "file://" +FileLoadPath.AppPersistentPath+"/Version/iphone/";
#elif UNITY_IPHONE &&  UNITY_EDITOR
                                 return  "file://" +Application.streamingAssetsPath+"/Version/iphone/";
#else
                        return "file://" + FileLoadPath.AppPersistentPath + "/Version/Platform/";
#endif
            }
        }

        /// <summary>
        ///Version AssetBundle url,带"/"。
        /// </summary>
        public string VersionAssetBundleURL
        {
            get
            {

#if UNITY_ANDROID && ! UNITY_EDITOR
                           return Application.streamingAssetsPath+"/Version/android/";

#elif UNITY_ANDROID && UNITY_EDITOR
                return "file://" + Application.streamingAssetsPath + "/Version/android/";
#elif UNITY_STANDALONE_WIN
                                    return "file://" +Application.streamingAssetsPath+"/Version/windows/";
#elif   UNITY_STANDALONE_OSX
                                 return  "file://" +Application.streamingAssetsPath+"/Version/mac/";

#elif UNITY_IPHONE && ! UNITY_EDITOR
                                 return  "file://" +Application.streamingAssetsPath+"/Version/iphone/";
#elif UNITY_IPHONE &&  UNITY_EDITOR
                                 return   "file://" +Application.streamingAssetsPath+"/Version/iphone/";
#else
                        return "file://" + Application.streamingAssetsPath + "/Version/Platform/";
#endif
            }
        }

        #endregion Version 路径




        //===========================单例start===================================//
        private static ResOnlineUpdateModel _instance = null;
        public static ResOnlineUpdateModel getInstance()
        {
            if (_instance == null)
            {
                _instance = new ResOnlineUpdateModel();
            }
            return _instance;
        }
        public static void Dispose()
        {
            if (_instance != null)
            {
                _instance = null;
            }

        }
        //===========================单例end===================================//
    }
}

