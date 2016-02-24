using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace update
{
    /// <summary>
    /// 资源的更新数据
    /// </summary>
    public class ResVersionModel
    {
        private string _localVersionData = string.Empty;
        /// <summary>
        /// 本地资源的md5数据
        /// </summary>
        public void injectLovalVersionMD5Data(string value)
        {
            _localVersionData = value;
        }

        private Dictionary<string, ResUpdateInfo> _localVersionMd5Dic = new Dictionary<string, ResUpdateInfo>();
        /// <summary>
        /// 本地文件MD5数据集合
        /// </summary>
        public Dictionary<string, ResUpdateInfo> localVersionMd5Dic
        {
            get
            {
                if (_localVersionMd5Dic.Count == 0)
                {
                    GetVersionMd5Dic(_localVersionMd5Dic, _localVersionData, true);
                }
                return _localVersionMd5Dic;
            }
        }

        /// <summary>
        /// 服务器md5数据
        /// </summary>
        private string _serverVersionData = string.Empty;
        private Dictionary<string, ResUpdateInfo> _serverVersionMd5Dic = new Dictionary<string, ResUpdateInfo>();
        /// <summary>
        /// 服务器文件MD5数据集合
        /// </summary>
        public Dictionary<string, ResUpdateInfo> serverVersionMd5Dic
        {
            get
            {
                if (_serverVersionMd5Dic.Count == 0)
                {
                    GetVersionMd5Dic(_serverVersionMd5Dic, _serverVersionData, false);
                }
                return _serverVersionMd5Dic;
            }
        }
        /// <summary>
        /// 注入服务器资源数据
        /// </summary>
        /// <param name="data"></param>
        public void injectServerVersionMD5Data(string data)
        {
            if (data == null) return;
            _serverVersionData = data;
        }


        /// <summary>
        ///  根据字符串，获取version MD5 字典数据。
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="str"></param>
        /// <returns>true 成功</returns>
        private void GetVersionMd5Dic(Dictionary<string, ResUpdateInfo> dic, string str, bool isLocal)
        {
            if (string.IsNullOrEmpty(str))
            {
                Logger.LogError("GetVersionMd5Dic str null");
                return;
            }
            try
            {
                //Logger.Log("解密前：" + str);
                string decrypt = EnDeTool.DeStr(str);
                //Logger.Log("解密后："+decrypt);
                string[] strs = decrypt.Split("\n".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                string[] infos;
                for (int i = 0, length = strs.Length; i < length; i++)
                {
                    infos = strs[i].Split(",".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    if (infos[0] != "versionCode") //以前的版本容错
                    {
                        dic.Add(infos[0], new ResUpdateInfo(infos[0], infos[1], infos[2]));
                    }
                }
                infos = null;
                strs = null;
            }
            catch (System.Exception ex)
            {
                Logger.LogError("GetVersionMd5Dic Error : " + ex.Message);
            }
        }


        /// <summary>
        /// 取得本地的单条version数据string
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string getLocalVersionStr(string key)
        {
            if (_localVersionMd5Dic.ContainsKey(key))
            {
                return _localVersionMd5Dic[key].key + "," + _localVersionMd5Dic[key].md5 + "," + _localVersionMd5Dic[key].length + "\n";
            }
            return "";

        }




        public List<ResUpdateInfo> _updateRes = new List<ResUpdateInfo>();
        /// <summary>
        /// 需要更新的资源集合
        /// </summary>
        public List<ResUpdateInfo> updateRes
        {
            get
            {
                return _updateRes;
            }
        }
        private int _currentUpdateIndex = 0;
        /// <summary>
        /// 当前正在更新的文件的序号
        /// </summary>
        public int currentUpdateIndex
        {
            set
            {
                _currentUpdateIndex = value;
            }
            get
            {
                return _currentUpdateIndex;
            }
        }
        /// <summary>
        /// 比较版本，取得需要更新的数据
        /// </summary>
        /// <returns>是否执行版本比较成功</returns>
        public bool comparedVersion()
        {
            try
            {
                updateRes.Clear();
                Dictionary<string, ResUpdateInfo> localVersionMd5Dic = ResVersionModel.getInstance().localVersionMd5Dic;
                foreach (var item in ResVersionModel.getInstance().serverVersionMd5Dic)
                {
                    if (localVersionMd5Dic.ContainsKey(item.Key) == false || item.Value.md5.Equals(localVersionMd5Dic[item.Key].md5) == false)
                    {
                        //新增的资源 
                        updateRes.Add(item.Value);                    //Logger.Log("需要更新资源:" + item.Key + " ; " + item.Value.length);
                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                Logger.LogError("比较版本 CompareVersion Error :" + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 取得所有需要更新的资源的总大小
        /// </summary>
        /// <returns></returns>
        public long getTotalUpdateSize()
        {
            if (_updateRes == null || _updateRes.Count == 0)
            {
                return 0;
            }
            else
            {
                long size = 0;
                int length = _updateRes.Count;
                for (int i = 0; i < length; i++)
                {
                    size += _updateRes[i].length;
                    // Logger.Log("需要更新的资源: " + _upAddRes[i].key + " ; " + _upAddRes[i].length);
                }

                Logger.Log("需要更新的资源总大小: " + size);

                return size;

            }
        }

        /// <summary>
        /// 更新资源失败
        /// </summary>
        private Dictionary<string, ResUpdateInfo> _upResFail = new Dictionary<string, ResUpdateInfo>();

        /// <summary>
        /// 更改更新资源的状态
        /// </summary>
        /// <param name="info"></param>
        /// <param name="state">是否更新成功</param>
        public void updateResState(ResUpdateInfo info, bool state)
        {
            info.state = state ? 1 : -1;
        }
        /// <summary>
        /// 取得更新成功的资源
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, ResUpdateInfo> getSucceseUpdateResInfos()
        {
            if (_updateRes == null || _updateRes.Count == 0)
            {
                return null;
            }
            else
            {
                Dictionary<string, ResUpdateInfo> dic = new Dictionary<string, ResUpdateInfo>();
                foreach (var item in _updateRes)
                {
                    if (item.state == 1)
                    {
                        dic.Add(item.key, item);
                    }

                }
                return dic;
            }
        }
        /// <summary>
        /// 取得所有更新失败的资源集合
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, ResUpdateInfo> getFailUpdateResInfos()
        {
            if (_updateRes == null || _updateRes.Count == 0)
            {
                return null;
            }
            else
            {
                Dictionary<string, ResUpdateInfo> dic = new Dictionary<string, ResUpdateInfo>();
                foreach (var item in _updateRes)
                {
                    if (item.state == -1)
                    {
                        dic.Add(item.key, item);
                    }

                }
                return dic;
            }
        }
        //===========================单例start===================================//
        private static ResVersionModel _instance = null;
        public static ResVersionModel getInstance()
        {
            if (_instance == null)
            {
                _instance = new ResVersionModel();
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





