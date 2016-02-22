using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 资源工具类
/// </summary>
   public class RESTool
    {
        //例如:ABResources/UIAtlas/Icon.prefab,返回 prefab

        /// <summary>
        /// 获取文件后缀名,根据文件路径。例如:ABResources/UIAtlas/Icon.prefab,返回 prefab
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string Extension(string path)
        {
            string[] strs = path.Split('.');
            if (strs.Length>=2)
            {
                return strs[strs.Length-1];
            }
            return "";
        }
        /// <summary>
        /// 是否正确文件格式,默认返回true，正确格式。
        /// 排除.meta或 mac 下的svn数据文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool RightFile(string path)
        {
            if (Extension(path).Equals("meta") || Extension(path).Equals(".doc"))
            {
                return false;
            }
            return true;
        }
       /// <summary>
       /// 转换文件夹分隔符
       /// \ 转换为 /
       /// </summary>
       /// <param name="path"></param>
       /// <returns></returns>
        public static string FolderDelimiter(string path)
        {
          return  path.Replace("\\","/");
        }
        /// <summary>
        /// 获取文件名称。
        /// 例如:ABResources/UIAtlas/Icon.prefab,返回Icon
        /// 例如:ABResources/UIAtlas/Icon,返回Icon
        /// 例如:ABResources/UIAtlas/,返回Empty
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static string GetFileName(string path) 
        {
            string name = string.Empty;
            try
            {
                if (path.Contains("."))
                {
                    string[] strs = path.Split('.')[0].Split('/');
                    name = strs[strs.Length - 1];
                }
                else
                {
                    if (path.Length > path.LastIndexOf("/"))
                    {
                        string[] strs = path.Split('/');
                        name = strs[strs.Length - 1];
                    }
                }
                return name;
            }
            catch (System.Exception e)
            {
                Debug.LogError("RESTool GetFileNameNew Exception : " + path + " ; " + e.Message);
                return string.Empty;
            }
          
        }

       /// <summary>
        /// 除掉文件后缀名 例如:ABResources/UIAtlas/Icon.prefab,返回 ABResources/UIAtlas/Icon
       /// </summary>
       /// <param name="path"></param>
       /// <returns></returns>
        public static string SubExtension(string path)
        {
            if(path.Contains("."))
            {
                return path.Split('.')[0];
            }
            return path;
        }
       /// <summary>
       /// 获取文件对应AssetBundle后缀名,根据文件路径或带后缀名文件
       /// 如不需要打包成AssetBundle，则返回""
       /// </summary>
       /// <param name="path"></param>
       /// <returns></returns>
       public static string ABExtension(string path)
       {
           string str = "";

           string[] strs = path.Split('.');

           if (strs.Length ==2)
           {
               switch (strs[1])
               {
                   //case RESFormat.CS:
                   //    break;
                   case RESFormat.SHADER:
                   case RESFormat.FBX:
                   case RESFormat.MAT:
                   case RESFormat.PREFAB:
                   case RESFormat.TTF:
                   case RESFormat.CSV:
                   case RESFormat.TXT:
                       str = strs[1];
                       break;
                   case RESFormat.PNG:
                   case RESFormat.JPG:
                       str = RESFormat.PNG;
                       break;
                   case RESFormat.Mp3:
                   case RESFormat.Wav:
                       str = RESFormat.Mp3;
                       break;
               }
           }

           return str;
       }

       public static string ABExtensiobByEx(string extension)
       {
           if (extension.Equals(RESFormat.JPG))
           {
               return RESFormat.PNG;
           }

           return extension;
       }


       /// <summary>
       /// 获取Assets父级绝对路径，带"/"
       /// 例如:D:/GameProject/GuaJi_RPG/
       /// </summary>
       public static string AssetsParentAbPath
       {
           get
           {
               return FolderDelimiter(Application.dataPath.Replace(Assets, ""));
           }
       }


       /// <summary>
       /// 获取assets相对路径的跟目录
       /// 例如:Assets/ABResources/UIAtlas/Icon.prefab,返回 ABResources
       /// </summary>
       /// <param name="resPath">Assets 相对路径</param>
       /// <returns></returns>
       public static string ResRoot(string resPath)
       {
           return resPath.Split('/')[1];
       }


       /// <summary>
       /// AssetBunlde资源跟目录 :StreamingAssets+"平台str"
       /// </summary>
       public static string ABRoot{
            get{

#if UNITY_ANDROID && ! UNITY_EDITOR
               return "StreamingAssets/android";
#elif UNITY_ANDROID && UNITY_EDITOR
                return "StreamingAssets/android";
#elif UNITY_STANDALONE_WIN
                return "StreamingAssets/windows";
#elif   UNITY_STANDALONE_OSX
               return "StreamingAssets/mac";
#elif UNITY_IPHONE && ! UNITY_EDITOR
               return  "StreamingAssets/iphone";
#elif UNITY_IPHONE &&  UNITY_EDITOR
               return  "StreamingAssets/iphone";
#else
               return "StreamingAssets/Platform"; 
#endif
            }
        }
           
           
    
       /// <summary>
       /// Assets资源跟目录 :Assets
       /// </summary>
       public const string Assets = "Assets";
       /// <summary>
       /// AssetBundle资源后缀名 : assetbundle
       /// </summary>
       public const string ABhzm = "assetbundle";

       /// <summary>
       /// assetbundle默认资源格式，assetbundle 或 bytes，只能二选一
       /// </summary>
       public const string ABLoadDefault = "assetbundle";

       /// <summary>
       /// 获取AssetBundle打包保存绝对路径。
       /// 例如:Assets/ABResources/UIAtlas/Icon.prefab,返回 Assets/StreamingAssets/UIAtlas/Icon_prefab.assetbundle
       /// </summary>
       /// <param name="resPath"> 资源绝对路径</param>
       /// <returns></returns>
       public static string GetBuildABPath(string resPath)
       {
            resPath = resPath.Replace(ResRoot(resPath), ABRoot);

           string[] strs = resPath.Split('.');

           if (strs.Length == 2)
           {
               resPath = strs[0] + "_" + ABExtensiobByEx(strs[1])+ "." + ABhzm;
          
               if (System.IO.File.Exists(resPath)==false)
               {
                   GetBuildABDirectory(resPath);
               }
               return resPath;
           }
           return "";
        
       }
       /// <summary>
       /// 获取AB保存的相对路径,
       /// 例如:Assets/ABResources/UIAtlas/Icon.prefab,返回 UIAtlas/Icon 
       /// </summary>
       /// <param name="resPath">Res资源相对路径</param>
       /// <returns></returns>
       public static string GetABPath(string resPath)
       {

           return SubExtension(resPath).Replace(Assets + "/" + ResRoot(resPath) + "/", "");
       }
       /// <summary>
       /// AB xml引用文件中下载的绝对路径中的相对路径
       /// 例如:Assets/ABResources/UIAtlas/Icon.prefab,返回 UIAtlas/Icon_prefab.assetbundle
       /// </summary>
       /// <param name="resPath">Res资源相对路径</param>
       /// <returns></returns>
       public static string GetABxmlAbPath(string resPath) 
       {

           return GetBuildABPath(resPath).Replace(Assets + "/" + ABRoot + "/", "");
       }

       /// <summary>
       /// AB xml引用文件中 Info 相对路径
       /// 例如:Assets/ABResources/UIAtlas/Icon.prefab,返回 UIAtlas/Icon_prefab
       /// </summary>
       /// <param name="resPath">Res资源相对路径</param>
       /// <returns></returns>
       public static string GetABxmlInforesPath(string resPath)
       {

           return   SubExtension( GetBuildABPath(resPath).Replace(Assets + "/" + ABRoot + "/", "") );
       }
       /// <summary>
       /// 获取AssetBundle打包保存目录。resPath,打包AB保存绝对路径
       /// 例如:Assets/ABResources/UIAtlas/Icon.prefab,返回 Assets/ABResources/UIAtlas
       /// </summary>
       /// <param name="resPath">资源绝对路径</param>
       /// <returns></returns>
       public static string GetBuildABDirectory(string resPath)
       {
         string directory = resPath.Substring(0, resPath.LastIndexOf('/'));
          CreateDirectory(directory);
         return directory;
       }
       /// <summary>
       /// 获取AB xml 引用文件保存目录,返回 ....../Assets/StreamingAssets/Rec/
       /// </summary>
       /// <param name="resPath">资源相对路径</param>
       /// <returns></returns>
       public static string GetABxmlRecSaveDirectory()
       {
           CreateDirectory(Application.dataPath + "/Rec/");
           return Application.dataPath + "/Rec/";

       }
       /// <summary>
       /// 获取AB xml 引用文件名称
       /// Assets/ABResources/UIAtlas/Icon.prefab,返回 UIAtlas_Icon_prefab.xml
       /// </summary>
       /// <param name="resPath">资源相对路径</param>
       /// <returns></returns>
       public static string GetABxmlRecName(string resPath)
       {
           string[] strs = resPath.Replace(Assets + "/" + ResRoot(resPath) + "/", "").Replace("/", "_").Split('.');
          return strs[0]+"_"+strs[1]+".xml";
       }
       /// <summary>
       /// 创建目录
       /// </summary>
       /// <param name="directoryPath"></param>
       public static void CreateDirectory(string directoryPath)
       {
           if (System.IO.Directory.Exists(directoryPath) ==false)
               System.IO.Directory.CreateDirectory(directoryPath);
       }
       /// <summary>
       /// 判断文件是否存在
       /// </summary>
       /// <param name="resAbPath"></param>
       /// <returns></returns>
       public static bool GetExists(string resAbPath) 
       {
           return System.IO.File.Exists(resAbPath);
       }
       /// <summary>
       /// 删除文件
       /// </summary>
       /// <param name="filePath"></param>
       public static void DeleteFile(string filePath)
       {
           if (System.IO.File.Exists(filePath))
               System.IO.File.Delete(filePath);
        
       }
       /// <summary>
       /// 获取下载的绝对路径url。path:相对路径，不带后缀名
       /// </summary>
       /// <param name="path">相对路径，不带后缀名</param>
       /// <param name="resType">资源类型</param>
       /// <param name="resManage">资源模式resources 或 assetbundle</param>
       /// <param name="hzm">资源后缀名</param>
       /// <returns></returns>
       public static string LoadAbPathByPath(string path, string resType, RESManageType resManage,string hzm=RESTool.ABLoadDefault)
       {
           if (resManage == RESManageType.RESOURCES)
           {
               return SubExtension(path);
           }

           //不指定文件格式，默认格式assetbundle 或 bytes，只能二选一

           //是否在sdk中
           if (System.IO.File.Exists(FileLoadPath.PersistentPath + path + "_" + resType + "." + hzm))
           {
               return FileLoadPath.PersistentUrl + path + "_" + resType + "." + hzm;
           }
           else
           {
               // 肯定是在游戏客户端 AB 中
               return FileLoadPath.AssetBundleURL + path + "_" + resType + "." + hzm;
           }

       }
        /// <summary>
       /// 获取下载的绝对路径 url，suffix,文件后缀名(格式);resPath,相对路径不带后缀名
        /// </summary>
        /// <param name="resPath"></param>
        /// <param name="suffix"></param>
        /// <param name="resManage"></param>
        /// <returns></returns>
       public static string LoadAbPathByAbPath(string resPath, string suffix, RESManageType resManage)
       {
           if (resManage == RESManageType.RESOURCES)
           {
               return SubExtension(resPath);
           }
           //是否在sdk中
           if (System.IO.File.Exists(FileLoadPath.PersistentPath + resPath + "." + suffix))
           {
               return FileLoadPath.PersistentUrl + resPath + "." + suffix;
           }
           else
           {
               // 肯定是在游戏客户端 AB 中
               return FileLoadPath.AssetBundleURL + resPath + "." + suffix;
           }
       }

       /// <summary>
       /// 获取AB的绝对路径。path:相对路径，不带后缀名,自动加上 "_"+resType+".assetbundle";
       /// </summary>
       /// <param name="path">不带后缀名</param>
       /// <param name="resManageType">资源模式</param>
       /// <param name="resType">类型</param>
       /// <returns></returns>
       public static string GetABAbPath(string path, string resType)
       {
           //是否在sdk中
           if (System.IO.File.Exists(FileLoadPath.PersistentPath + path + "_" + resType + "." + RESTool.ABLoadDefault))
           {
               return FileLoadPath.PersistentPath + path + "_" + resType + "." + RESTool.ABLoadDefault;
           }
           else
           {
               // 肯定是在游戏客户端 AB 中
               return FileLoadPath.AssetBundlePath + path + "_" + resType + "." + RESTool.ABLoadDefault;
           }

       }

       /// <summary>
       /// 类型字符 转换为type
       /// cs 和 unity3d 不转换
       /// </summary>
       /// <param name="resFormat"></param>
       /// <returns></returns>
       public static System.Type GetTypeByRESFormat(string resFormat)
       {
           System.Type type = null;
           switch (resFormat)
           {
               case RESFormat.FBX:
               case RESFormat.PREFAB:
               case RESFormat.Anim:
                   type = typeof(GameObject);
                   break;
               case RESFormat.PNG:
               case RESFormat.JPG:
                   type = typeof(Texture2D);
                   break;
               case RESFormat.CSV:
               case RESFormat.TTF:
               case RESFormat.TXT:
               case RESFormat.XML:
                   type = typeof(TextAsset);
                   break;
               case RESFormat.SHADER:
                   type = typeof(Shader);
                   break;
               case RESFormat.MAT:
                   type = typeof(Material);
                   break;
               case RESFormat.UNITY3D:

                   break;
           }

           return type;
       }

    }

