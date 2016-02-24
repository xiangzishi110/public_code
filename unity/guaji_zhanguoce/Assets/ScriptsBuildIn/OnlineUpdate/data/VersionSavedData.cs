using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace update
{
    /// <summary>
    /// 保存资源和app版本，游戏内调用来显示在选服界面
    /// </summary>
    public class VersionSavedData
    {

        private int _appVersion = 0;
        private int _resVersionCode = 0;
        public void init(int appVersion, int resVersionCode)
        {
            _appVersion = appVersion;
            _resVersionCode = resVersionCode;
        }
        public int appVersion
        {
            get
            {
                return _appVersion;
            }
        }
        public int resVersionCode
        {
            get
            {
                return _resVersionCode;
            }
        }
        //===========================单例start===================================//
        private static VersionSavedData _instance = null;
        public static VersionSavedData getInstance()
        {
            if (_instance == null)
            {
                _instance = new VersionSavedData();
            }
            return _instance;
        }
        //===========================单例end===================================//
    }
}






