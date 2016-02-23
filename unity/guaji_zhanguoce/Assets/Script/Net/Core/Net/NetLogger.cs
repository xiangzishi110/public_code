//----------------------------------------------
// socket通信日志类
// @author: ChenXing
// @email:  onechenxing@163.com
// @date:   2014/12/24
//----------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CxLib.Net
{
    /// <summary>
    /// 网络日志输出
    /// </summary>
    public class NetLogger
    {
        /// <summary>
        /// 是否在Unity中打log
        /// </summary>
        public static bool IfInUnity = true;
        /// <summary>
        /// 日志过滤等级
        /// </summary>
        public static int ShowLogLevel = 2;

        /// <summary>
        /// 简单的文本日志
        /// </summary>
        /// <param name="info"></param>
        public static void Log(string info, int level = 1)
        {
            if (level > ShowLogLevel)
            {
                if(IfInUnity)
                {
                    unity_log("[NetLogger] " + info);
                }
                else
                {
                    net_log("[NetLogger] " + info);
                }
            }
        }

        private static void unity_log(string info)
        {
            Logger.Log(info);
        }

        private static void net_log(string info)
        {
            Console.WriteLine(info);
        }
    }
}
