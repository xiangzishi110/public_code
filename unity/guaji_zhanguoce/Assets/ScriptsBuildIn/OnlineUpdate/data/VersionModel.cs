using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace update
{
    /// <summary>
    /// 整个资源的版本数据
    /// </summary>

    public class VersionModel
    {

        private int _vcServer = 0;
        /// <summary>
        /// 服务器返回的vc值
        /// </summary>
        public int vcServer
        {
            set
            {
                _vcServer = value;
            }
            get
            {
                return _vcServer;
            }
        }



        private int _serverSupperedVersionCode = 0;
        /// <summary>
        /// 服务器支持的更新的app版本
        /// </summary>
        public int serverSupperedVersionCode
        {
            set
            {

                _serverSupperedVersionCode = value;
            }
            get
            {
                return _serverSupperedVersionCode;
            }
        }

        private int _localVersionCode = 0;
        /// <summary>
        /// 本地的资源版本
        /// </summary>
        public int localVersionCode
        {
            set
            {
                _localVersionCode = value;
            }
            get
            {
                return _localVersionCode;
            }
        }



        /// <summary>
        /// 取得安装包的版本
        /// </summary>
        public int appVersionCode
        {
            get
            {

                return SDKHelper.getAppVersionCode();
            }
        }



        //===========================单例start===================================//
        private static VersionModel _instance = null;
        public static VersionModel getInstance()
        {
            if (_instance == null)
            {
                _instance = new VersionModel();
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






