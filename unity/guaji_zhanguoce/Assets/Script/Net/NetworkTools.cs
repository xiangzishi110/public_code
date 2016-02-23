
using UnityEngine;
using System.Collections.Generic;
public class NetworkTools
    {
     /// <summary>
     /// 后台反馈的json字符串转换为字典数据格式,错误，待解决。
        ///字符串格式 {\"1\":2,\"2\":3,\"3\":4,\"4\":5,\"5\":6,\"6\":7,\"7\":8,\"8\":9,\"9\":10,\"10\":11,\"11\":12,\"12\":13}
     /// </summary>
     /// <param name="str"></param>
     /// <returns></returns>
       public static Dictionary<string, string> GetResMsgData(string str)
       {
           /*
           Dictionary<string, string> dic = new Dictionary<string, string>();
           try
           {
               Logger.Log("GetResMsgData:" + str);
          
              str= str.Trim(new char[] { '{', '}' });
               Logger.Log("GetResMsgData:" + str);
               string[] strs = str.Split(",".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);

               string[] datas;
               foreach (var item in strs)
               {
                   datas = item.Split(":".ToCharArray());
                   dic[datas[0]] = datas[1];
               }
               datas = null;
               str = null;
               return dic;
           }
           catch (System.Exception ex)
           {
               Logger.LogError("NetworkTools GetResMsgData Exception "+ex.Message);
               return dic;
           }
            */
           return null;
       }
    }

