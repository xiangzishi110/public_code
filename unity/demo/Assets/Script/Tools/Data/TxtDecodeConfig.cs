using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools.Data
{
    /// <summary>
    /// 解析txt配置文件
    /// </summary>
    public class TxtDecodeConfig
    {
        /// <summary>
        /// 解析txt文件,RES中必须先加载成功。configPath,csv相对路径且没后缀名
        /// </summary>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static List<Tools.Data.StringDictionary> GetDataByRES(string configPath)
        {

            try
            {
                object res = RES.GetRES(configPath, RESFormat.TXT);
                if (res == null)
                {
                    Logger.LogError("TxtDecodeConfig GetDataByRES RES null :" + configPath);
                    return null;
                }

                return GetDataByStr(RES.GetRES(configPath) as string);
            }
            catch (System.Exception ex)
            {
                Logger.LogError("TxtDecodeConfig GetDataByRES Error : " + configPath + " ; " + ex.Message);
                return null;
            }

        }
        /// <summary>
        /// 解析txt文件，必须是Resources文件下。直接加载后再解析。
        /// </summary>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static List<Tools.Data.StringDictionary> GetDataByResources(string configPath)
        {
            try
            {
                string str = (Resources.Load(configPath) as TextAsset).text;
                return GetDataByStr(str);
            }
            catch (System.Exception ex)
            {
                Logger.LogError("TxtDecodeConfig GetDataByResources Error:" + configPath + " ; " + ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 解析txt文件中的字符串。
        /// </summary>
        /// <param name="strCSV"></param>
        /// <returns></returns>
        public static List<Tools.Data.StringDictionary> GetDataByStr(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                Logger.LogError("LanguageCofingLoad Txt String null");
                return null;
            }
            if (str.IndexOf("\r") > -1)
            {
                str = str.Replace("\r", "");
            }
            string[] strs = str.Split("\n".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
            //第一行,表头
            string[] keyStrs = strs[0].Split("\t".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
            int keylength = keyStrs.Length;
            // Logger.Log("KeyStrs "+keyStrs.Length);
            List<Tools.Data.StringDictionary> strDics = new List<Tools.Data.StringDictionary>();
            //解析每一行数据
            string[] mains;
            for (int i = 1, length = strs.Length; i < length; i++)
            {
                mains = strs[i].Split("\t".ToCharArray());
                // Logger.Log("mains " + mains.Length);
                Tools.Data.StringDictionary strDic = new Tools.Data.StringDictionary();

                for (int j = 0, mainL = keylength; j < mainL; j++)
                {
                    // Logger.Log("LanguageConfig:"+keyStrs[j]+";"+mains[j]);
                    strDic.SetValue(keyStrs[j], mains[j]);
                }
                strDics.Add(strDic);
            }
            mains = null;
            keyStrs = null;
            strs = null;
            str = null;
            return strDics;
        }
    }
}

