using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace update
{
    /// <summary>
    /// 在线更新单个资源数据
    /// </summary>

    public class ResUpdateInfo
    {
        public string key;

        public string md5;

        public long length;
        /// <summary>
        /// 资源状态  0 未更新 1成功 -1失败
        /// </summary>
        public int state = 0;
        public ResUpdateInfo() { }

        public ResUpdateInfo(string key, string md5, string length)
        {
            this.key = key;
            this.md5 = md5;
            this.length = long.Parse(length);
        }
    }
}






