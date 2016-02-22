using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Tools.Data{
    /// <summary>
    /// 解析CSV配置文件
    /// </summary>
    public class CSVDecodeConfig  {
        /// <summary>
        /// 解析csv文件,RES中必须先加载成功。configPath,csv相对路径且没后缀名
        /// </summary>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static List< Tools.Data.StringDictionary> GetDataByRES(string configPath) 
        {
    
            try
            {
                object res=RES.GetRES(configPath,RESFormat.CSV);

                //csvNameTest=configPath;

                if (res == null)
                {
                    Logger.LogError("CSVDecodeConfig GetDataByRES RES null :"+configPath);
                    return null;
                }
        
                return GetDataByStr(res as string,configPath);
            }
            catch (System.Exception ex)
            {
                Logger.LogError("CSVDecodeConfig GetDataByRES Error : " + configPath + " ; " + ex.Message);
                return null;
            }
     
        }
        /// <summary>
        /// 解析csv文件，必须是Resources文件下。直接加载后再解析。
        /// </summary>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static List<Tools.Data.StringDictionary> GetDataByResources(string configPath) 
        {
            try
            {
                string str = (Resources.Load(configPath) as TextAsset).text;
                return GetDataByStr(str,configPath);
            }
            catch (System.Exception ex)
            {
                Logger.LogError("CSVDecodeConfig GetDataByResources Error:" + configPath+" ; "+ex.Message);
                return null;
            }
        }
        //private static string csvNameTest="";
        /// <summary>
        /// 解析csv文件中的字符串。
        /// </summary>
        /// <param name="strCSV"></param>
        /// <returns></returns>
        public static List<Tools.Data.StringDictionary> GetDataByStr(string strCSV, string csvPath) 
        {
            if (string.IsNullOrEmpty(strCSV))
            {
                Logger.LogError("CSVDecodeConfig GetDataByStr String null ");
                return null;
            }

            List<Tools.Data.StringDictionary> datas = new List<Tools.Data.StringDictionary>();
            string key = "";
            string line="";
            int i = 1;
            int j = 0;
            int keylength = 0;
            //解析每一行数据
            string[] lieStr = null;
            try
            {
                if (strCSV.IndexOf("\r") > -1)
                {
                    // Debug.LogError(" csv文件有"+" \r " + csvNameTest);
                    strCSV = strCSV.Replace("\r", "");
                }

                string[] strs = strCSV.Split("\n".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);

                //第二行，表头
                string[] firstLine = strs[0].Split(",".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                keylength = firstLine.Length;

                for (int length = strs.Length; i < length; i++)
                {

                    Tools.Data.StringDictionary data = new Tools.Data.StringDictionary();
                    line= strs[i];
                    lieStr = line.Split(',');
                    j=0;
                    for (; j < keylength; j++)
			        {
			            data.SetValue(firstLine[j], lieStr[j]);
			        }
                    datas.Add(data);
                }
                lieStr = null;
                firstLine = null;
                strs = null;
                strCSV = null;
            }
            catch (Exception ex)
            {
                Logger.LogError("CSVDecodeConfig GetDataByStr Error:" + csvPath + " ; " + ex.Message + ";index:" + j + ";lineIndex:"+i+";" + line);
                return datas;
            }

            return datas;
        }

    }
}

