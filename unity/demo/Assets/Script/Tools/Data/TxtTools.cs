using System.Collections.Generic;

namespace Tools.Data
{
    /// <summary>
    /// txt辅助类
    /// </summary>
    public class TxtTools
    {
        /// <summary>
        /// 把txt转换成字典
        /// </summary>
        /// <typeparam name="K">主键的类型</typeparam>
        /// <typeparam name="V">info类的类型</typeparam>
        /// <param name="csvPath">csv配置名或路径</param>
        /// <param name="primaryKey">主键的名字</param>
        /// <returns></returns>
        public static Dictionary<K, V> TxtToDictionary<K, V>(string csvPath, string primaryKey) where V : new()
        {
            Dictionary<K, V> staticDic = new Dictionary<K, V>();
            try
            {

                List<Tools.Data.StringDictionary> csvData = TxtDecodeConfig.GetDataByRES(csvPath);
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
                Logger.LogError("TxtTools TxtToDictionary Exception:" + csvPath + ";" + ex.Message);
                return staticDic;
            }

        }
        public static string getChineseById(int id)
        {
            switch (id)
            {
                case 1: return "一"; break;
                case 2: return "二"; break;
                case 3: return "三"; break;
                case 4: return "四"; break;
                case 5: return "五"; break;
                case 6: return "六"; break;
                case 7: return "七"; break;
                case 8: return "八"; break;
                case 9: return "九"; break;
                case 10: return "十"; break;
                default: return "*"; break;
            }
        }
    }
}

