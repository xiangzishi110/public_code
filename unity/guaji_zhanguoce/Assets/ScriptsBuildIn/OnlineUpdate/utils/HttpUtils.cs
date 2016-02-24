using System;
using UnityEngine;
using System.Collections;
using LitJson;

namespace update
{
    /// <summary>
    /// http通讯类
    /// </summary>
    public class HttpUtils : MonoBehaviour
    {
        public HttpUtils(){}
        public static HttpUtils GetOrCreate(GameObject gameObject)
        {
            if (gameObject == null) { return new HttpUtils(); }
            var existed = gameObject.GetComponent<HttpUtils>();
            return existed ?? gameObject.AddComponent<HttpUtils>();
        }
        public IEnumerator send(string url, System.Action<JsonData> callBack)
        {
            Logger.Log(">>>>>>>>>>>>>>>>>>>" + url);
            //表单   
            WWWForm form = new WWWForm();
            Hashtable headers = form.headers;
            headers["Content-Type"] = "text/html;charset=UTF-8";
            headers["Charset"] = "UTF-8";
            headers["cache-control"] = "no-cache";
            DateTime date = DateTime.Now;
            string time = date.ToString("ddd, yyyy-mm-dd HH':'mm':'ss 'UTC'");
            headers["Date"] = time;
            WWW www = new WWW(url, null, headers);
            yield return www;
            JsonData jd = null;
            if (www.error != null)
            {  //请求失败
                Logger.Log("<<<<<<<<<<<<<<<<请求失败:" + www.error);
                jd = JsonMapper.ToObject("{\"code\":1000}");
            }
            else
            {  //POST请求成功  
                Logger.Log("<<<<<<<<<<<<<<<<" + www.text);
                jd = JsonMapper.ToObject(www.text);
            }
            callBack(jd);

        }
        /// <summary>
        /// 异步下载资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="call"></param>
        /// <returns></returns>
        public IEnumerator LoadYield(string url, System.Action<WWW> call)
        {
            using (WWW www = new WWW(url))
            {
                yield return www;
                if (call != null)
                {
                    call(www);
                }
            }
        }
    }
}

