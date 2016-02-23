using System;
using UnityEngine;
using System.Collections;
using LitJson;
namespace net
{
    class HttpUtils : MonoBehaviour
    {
        public HttpUtils()
        {

        }
        public static HttpUtils GetOrCreate(GameObject gameObject)
        {
            if (gameObject == null) { return new HttpUtils(); }
            var existed = gameObject.GetComponent<HttpUtils>();
            return existed ?? gameObject.AddComponent<HttpUtils>();
        }
        public IEnumerator send(string url, string param, HTTP_NET_CALBACK callBack)
        {

            Logger.Log(">>>>>>>>>>>>>>>>>>>" + url + "?" + param);
            //表单   
            WWWForm form = new WWWForm();
            Hashtable headers = form.headers;
            headers["Content-Type"] = "text/html;charset=UTF-8";
            headers["Charset"] = "UTF-8";
            headers["cache-control"] = "no-cache";
            DateTime date = DateTime.Now;
            string time = date.ToString("ddd, yyyy-mm-dd HH':'mm':'ss 'UTC'");
            headers["Date"] = time;
            WWW www = new WWW(url + "?" + param, null, headers);
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

    }
}