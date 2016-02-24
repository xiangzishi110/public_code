using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace update
{
/// <summary>
/// 在线更新工具类
/// </summary>
    public class ResUpdateUtils
    {
        /// <summary>
        /// 获取字节数组对应的md5值
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string getCheckMD5(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return string.Empty;
            try
            {
                string str = System.Text.UTF8Encoding.UTF8.GetString(bytes);
                bytes = System.Text.UTF8Encoding.UTF8.GetBytes(str);
                MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
                byte[] hashbytes = MD5.ComputeHash(bytes);

                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0, max = hashbytes.Length; i < max; i++)
                {
                    sBuilder.Append(hashbytes[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
            catch (System.Exception ex)
            {
                Logger.LogError("getCheckMD5 Error : " + ex.Message + " ; ");
                return string.Empty;
            }
        }
        /// <summary>
        /// 字节转换为kb,保留小数点后两位
        /// 1MB=1024KB  1KB=1024B。1B=1字节;
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static long ByteToTrillionLong(long bytes)
        {
            return bytes / 1024;
        }

        public static float longToFloat(long progressByte)
        {
            return System.Convert.ToSingle(progressByte);
        }

        /// <summary>
        /// 字节转换为kb,保留小数点后两位
        /// 1MB=1024KB  1KB=1024B。1B=1字节;
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ByteToTrillion(long bytes)
        {
            try
            {
                return Mathf.Floor(bytes / 1024).ToString() + " kb";
            }
            catch (System.Exception)
            {
                return "0 kb";
            }
        }
    }
    
}






