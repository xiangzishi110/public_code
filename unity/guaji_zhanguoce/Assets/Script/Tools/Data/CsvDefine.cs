using UnityEngine;
using System.Collections;

namespace Tools.Data
{
    public static class CsvDefine
    {
        /// <summary>
        /// 表格枚举
        /// </summary>
        public enum ECSV_NAME
        {
            npc,
            equip,
            Max,
        }

        /// <summary>
        /// 表格名称
        /// </summary>
        public static string[] CSV_NAME = new string[(int)ECSV_NAME.Max]
	    {
		    "csv_npc",
            "csv_Equip"
	    };

        /// <summary>
        /// 通过表格名称获取枚举类型
        /// </summary>
        public static ECSV_NAME GetCsvType(string szName)
        {
            for (int iIndex = 0; iIndex < CSV_NAME.Length; ++iIndex)
            {
                if (CSV_NAME[iIndex] == szName)
                {
                    return (ECSV_NAME)iIndex;
                }
            }

            return ECSV_NAME.Max;
        }
    }
}
