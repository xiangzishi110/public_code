//----------------------------------------------
// csv辅助类
// @author: ChenXing
// @email:  onechenxing@163.com
// @date:   2015/1/17
//----------------------------------------------
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;


namespace Tools.Data
{
    /// <summary>
    /// csv辅助类
    /// </summary>
    public class CsvTool
    {
        /// <summary>
        /// 把csv转换成字典
        /// RES中必须先加载成功
        /// </summary>
        /// <typeparam name="K">主键的类型</typeparam>
        /// <typeparam name="V">info类的类型</typeparam>
        /// <param name="csvPath">csv配置名或路径</param>
        /// <param name="primaryKey">主键的名字</param>
        /// <returns></returns>
        public static Dictionary<K, V> CsvToDictionary<K, V>(string csvPath, string primaryKey) where V : new()
        {
            //float fTime = Time.realtimeSinceStartup;
            Dictionary<K, V> staticDic = new Dictionary<K, V>();

            int i = 0;
            string key = "";
            string value = "";
            List<Tools.Data.StringDictionary> csvData = CSVDecodeConfig.GetDataByRES(csvPath);
            try
            {
                for (int length = csvData.Count; i < length; i++)
                {
                    V info = new V();
                    foreach (var param in csvData[i])
                    {
                        key = param.Key;
                        value = param.Value;
                        ReflectionHelper.SetObjectByStringValueCheckEmpty(info, param.Key, param.Value);
                    }
                    staticDic.Add((K)ReflectionHelper.GetObjectValue(info, primaryKey), info);
                }

                key = null;
                value = null;
                //Logger.Log("csvPath : " + csvPath + " ||　解析时间 :　" + (Time.realtimeSinceStartup - fTime).ToString());
                return staticDic;
            }
            catch (System.Exception ex)
            {
                //错误的index，key，value
                Logger.LogError("CsvTool CsvToDictionary Exception:" + csvPath + "," + i + ";" + key + ";" + value + ";" + ex.Message);

                foreach (var param in csvData[i])
                {
                    Logger.LogError("CsvTool CsvToDictionary Exception:" + " key:" + param.Key + "; value:" + param.Value);
                }
            }


            return staticDic;
        }

        /// <summary>
        /// 把csv转换成字典
        /// RES中必须先加载成功
        /// </summary>
        /// <typeparam name="K">主键的类型</typeparam>
        /// <typeparam name="V">info类的类型</typeparam>
        /// <param name="csvPath">csv配置名或路径</param>
        /// <param name="primaryKey">主键的名字</param>
        /// <returns></returns>
        public static Dictionary<K, V> CsvToDictionary<K, V>(string csvPath, string primaryKey, Object csvRes) where V : new()
        {
            Dictionary<K, V> staticDic = new Dictionary<K, V>();
            int i = 0;
            string key = "";
            string value = "";
            float fTime = Time.realtimeSinceStartup;
            List<Tools.Data.StringDictionary> csvData = CSVDecodeConfig.GetDataByStr(csvRes.ToString(), csvPath);
            try
            {
                for (int length = csvData.Count; i < length; i++)
                {
                    V info = new V();
                    foreach (var param in csvData[i])
                    {
                        key = param.Key;
                        value = param.Value;
                        ReflectionHelper.SetObjectByStringValueCheckEmpty(info, param.Key, param.Value);
                    }
                    staticDic.Add((K)ReflectionHelper.GetObjectValue(info, primaryKey), info);
                }

                key = null;
                value = null;
                return staticDic;
            }
            catch (System.Exception ex)
            {
                //错误的index，key，value
                Logger.LogError("CsvTool CsvToDictionary Exception:" + csvPath + "," + i + ";" + key + ";" + value + ";" + ex.Message);

                foreach (var param in csvData[i])
                {
                    Logger.LogError("CsvTool CsvToDictionary Exception:" + " key:" + param.Key + "; value:" + param.Value);
                }

                return staticDic;
            }

        }
        /// <summary>
        /// 把csv转换成字典
        /// RES中必须先加载成功
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="csvPath"></param>
        /// <param name="primaryKeys"></param>
        /// <returns></returns>
        public static Dictionary<Tools.Data.DicKeyObjects, V> CsvToDictionary<V>(string csvPath, params string[] primaryKeys) where V : new()
        {
            Dictionary<Tools.Data.DicKeyObjects, V> staticDic = new Dictionary<Tools.Data.DicKeyObjects, V>();
            try
            {
                List<Tools.Data.StringDictionary> csvData = CSVDecodeConfig.GetDataByRES(csvPath);
                foreach (var line in csvData)
                {
                    V info = new V();
                    foreach (var param in line)
                    {
                        ReflectionHelper.SetObjectByStringValueCheckEmpty(info, param.Key, param.Value);
                    }
                    Tools.Data.DicKeyObjects dickey = new Tools.Data.DicKeyObjects();
                    foreach (var item in primaryKeys)
                    {
                        dickey.keys.Add(new Tools.Data.KeyObject(ReflectionHelper.GetObjectValue(info, item)));
                    }
                    staticDic[dickey] = info;
                }
                return staticDic;
            }
            catch (System.Exception ex)
            {
                Logger.LogError("CsvTool KeyList CsvToDictionary  Exception:" + csvPath + ";" + ex.Message);
                return staticDic;
            }

        }
        /// <summary>
        ///  把csv转换成字典，Resources下csv文件
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="csvPath"></param>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public static Dictionary<K, V> RESCsvToDictionary<K, V>(string csvPath, string primaryKey) where V : new()
        {
            Dictionary<K, V> staticDic = new Dictionary<K, V>();
            try
            {
                List<Tools.Data.StringDictionary> csvData = CSVDecodeConfig.GetDataByResources(csvPath);
                foreach (var line in csvData)
                {
                    V info = new V();
                    foreach (var param in line)
                    {
                        ReflectionHelper.SetObjectByStringValueCheckEmpty(info, param.Key, param.Value);
                    }
                    staticDic.Add((K)ReflectionHelper.GetObjectValue(info, primaryKey), info);
                }
                return staticDic;
            }
            catch (System.Exception ex)
            {
                Logger.LogError("CsvTool CsvToDictionary Exception:" + csvPath + ";" + ex.Message);
                return staticDic;
            }

        }


    }
}
