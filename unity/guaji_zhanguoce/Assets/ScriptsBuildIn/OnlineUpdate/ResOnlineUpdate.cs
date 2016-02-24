using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using update;

/*
 * 开发流程
1，游戏一启动，获取本地端的version.txt文件
2，对比本地端版本号与服务器版本号。
3,相同，不做更新处理
4,不同。获取服务器version.txt文件。
5,根据本地端version.txt对比服务器version.txt中的资源MD5，获取更新和新增资源名。
6，一一加载资源，加载资源成功后，保存到sdk，更新本地version.txt。
 */
/// <summary>
/// 资源在线更新
/// </summary>
public class ResOnlineUpdate  : MonoBehaviour
{
        //*****************************场景设置属性*****************************//
        /// <summary>
        /// 是否在线更新资源。true：更新;
        /// </summary>
        public bool isUpdate = false;


        //*****************************场景设置属性end*****************************//
        /// <summary>
        /// 网络连接失败，重连响应时间
        /// </summary>
        private const float nextAgainUITime = 2f;


        /// <summary>
        /// 更新完资源返回上层处理
        /// </summary>
        public System.Action<bool> UpdateEndCallback=null;
        public delegate void OnlineUpdateResult(bool isbool);
        public OnlineUpdateResult _resultdele;

        
        

        /// <summary>
        /// 已经加载完成资源大小
        /// </summary>
        private long finishLoadSize = 0;
		/// <summary>
        /// 成功下载的资源大小
        /// </summary>
        private long completeLoadSize = 0;

        public void Start()
        {
            ResOnlineUpdataUI.instance.initLoadUI();
        }


        

        /// <summary>
        /// 资源在线更新接口
        /// </summary>
        /// <param name="call"></param>
        public void InitExecution(OnlineUpdateResult call)
        {
            _resultdele += call;
            ResOnlineUpdataUI.instance.setStatus(UIShowStatus.CHECK);
            if (isUpdate)
            {
                ServerVersionReqMsg();
            }
            else if (null != _resultdele)
            {
                _resultdele(false);
            }
        }
        /// <summary>
        /// 重新更新资源
        /// </summary>
        void AgainOnlineUpdate()
        {
            ResOnlineUpdateModel.Dispose();
            VersionModel.Dispose();
            ResVersionModel.Dispose();
            ResOnlineUpdataUI.instance.showProgressNum = false;
            finishLoadSize = 0;
			completeLoadSize = 0;
            ServerVersionReqMsg();
        }



        private void ServerVersionReqMsg()
        {
            //外网服务器 
            string url = ResOnlineUpdateModel.getInstance().GetUrl();
            StartCoroutine(HttpUtils.GetOrCreate(gameObject).send(url, ResMsg));
        }




        
        /// <summary>
        /// 取得服务器更新数据
        /// //{"md5":"13611bb287301388f3fbec8bd4d93e47","vc":"100","url":"http://testxsg.sy599.com/Guaji3DLogin/resupdate/"}
        /// </summary>
        /// <param name="json"></param>
        private void ResMsg(LitJson.JsonData json)
        {
            if (((IDictionary)json).Contains("code"))
            {
                string text = "网络连接失败，重新连接继续更新！";//请求后台获取md5失败
                StartCoroutine(OnlineUpdateFailYield(nextAgainUITime, text));
                return;
            }
            Dictionary<string, LitJson.JsonData> jsonDic = json.Inst_Object as Dictionary<string, LitJson.JsonData>;
            if (jsonDic.ContainsKey("vc") && jsonDic.ContainsKey("url") && jsonDic.ContainsKey("svc"))
            {
                VersionModel.getInstance().vcServer = int.Parse(jsonDic["vc"] + "");
                ResOnlineUpdateModel.getInstance().ResServerUrl = jsonDic["url"] + "";
                VersionModel.getInstance().serverSupperedVersionCode = int.Parse(jsonDic["svc"] + "");
                Logger.Log("==收到资源服务器第一次返回结果===");
                Logger.Log("==vc:" + jsonDic["vc"]);
                Logger.Log("==URL:" + ResOnlineUpdateModel.getInstance().ResServerUrl);
                Logger.Log("==svc:" + VersionModel.getInstance().serverSupperedVersionCode);
                getLocalVersionNum();
            }
            else
            {
                Logger.LogError("http 资源版本号或下载地址不存在，或网络链接中断！");
                string text = "网络连接中断，重新连接继续更新！";//http 资源版本号或下载地址不存在，或网络链接中断！
                StartCoroutine(OnlineUpdateFailYield(nextAgainUITime, text));
            }
    
        }

        
        /// <summary>
        ///取得本地版本号
        /// </summary>
        private void getLocalVersionNum()
        {
         
            string url = ResOnlineUpdateModel.getInstance().getLocalCodeFileUrl();
            StartCoroutine(HttpUtils.GetOrCreate(gameObject).LoadYield(url,
                                    delegate(WWW www)
                                    {
                                        if (string.IsNullOrEmpty(www.error) == false)
                                        {
                                            Logger.LogError("本地版本号txt :" + www.url + "; 加载错误 :" + www.error);
                                        }
                                        else
                                        {
                                            int localVersionCode = int.Parse(Encoding.UTF8.GetString(www.bytes));//本地资源版本号,如果是app目录中，则跟app一样，否则需要比app大
                                            Logger.Log("本地资源版本号:" + localVersionCode);
                                            if (VersionModel.getInstance().vcServer == localVersionCode)
                                            {
                                                Logger.Log("版本号相同:" + localVersionCode + ",不需要更新");
                                                Release();
                                                return;
                                            }
                                          
                                            //判断此版本号是否支持更新
                                            int appVersionCode = VersionModel.getInstance().appVersionCode;//app版本号
                                            int supperedVersionCode = VersionModel.getInstance().serverSupperedVersionCode;
                                            Logger.Log("===>localVersionCode：" + localVersionCode + ",appVersionCode:" + appVersionCode + ",supperedVersionCode:" + supperedVersionCode);
                                            if (appVersionCode != 0 && appVersionCode < supperedVersionCode)
                                            {
                                                Logger.Log("===本app的版本号过低:版本号" + appVersionCode + "/" + supperedVersionCode);
                                                //如果服务器支持的更新的版本号比app版本号大,就不更新
                                                ResOnlineUpdataUI.instance.OnlineUpdateNotSuppered();
                                                return;
                                            }
                                             
                                            //删除本地多余资源
                                            if (localVersionCode < appVersionCode || localVersionCode == 0) //如果本地资源版本号比app低，证明是残留资源
                                            {
                                                Logger.Log("===>sd卡资源版本号低,执行删除sd卡资源");

                                                bool isWrong = false;//是否更新出错，出错则是因为不是sd资源，在app目录中删除不掉
                                                //删除sd卡中的资源，重新走更新流程,加上trycatch防止出错
                                                try
                                                {
                                                    Directory.Delete(ResOnlineUpdateModel.getInstance().sdResourcePath + "Version", true);
                                                }
                                                catch (System.Exception ex) { isWrong = true; }
                                                try
                                                {
                                                    Directory.Delete(ResOnlineUpdateModel.getInstance().sdResourcePath + "android", true);
                                                }
                                                catch (System.Exception ex) { }

                                                if (isWrong == false)
                                                {
                                                    AgainOnlineUpdate();
                                                    return;
                                                }
                                            }

                                            //下载本地version文件
                                            loadLocalVersionFile();

                                        }
                                 }
             ));

        }
        private void loadLocalVersionFile(){
            Logger.Log("===下载本地 version文件");
            string url=ResOnlineUpdateModel.getInstance().getLocalVersionFileUrl();
            StartCoroutine(HttpUtils.GetOrCreate(gameObject).LoadYield(url, 
                                    delegate(WWW www)
                                    {
                                        if (string.IsNullOrEmpty(www.error) == false)
                                        {
                                            Logger.LogError("本地版本号txt :"+www.url+"; 加载错误 :"+www.error);
                                            string text = "网络连接中断，重新连接继续更新！";//"下载Version.txt服务器文档失败:" + www.url + ";" + www.error : 
                                            StartCoroutine(OnlineUpdateFailYield(nextAgainUITime, text));
                                        }
                                        else
                                        {
                                            ResVersionModel.getInstance().injectLovalVersionMD5Data(Encoding.UTF8.GetString(www.bytes));//注入本地资源的md5数据
                                        }
                                        LoadServerVersionTxt();
                                    }
                           )
                );
        }
      

        
        /// <summary>
        /// 加载Version.txt服务器文档,解析数据
        /// </summary>
        /// <returns></returns>
       private  void LoadServerVersionTxt()
        {
            
            Logger.Log("===下载服务器Version文件");
            string url=ResOnlineUpdateModel.getInstance().ResServerVersionUrl;
            StartCoroutine(HttpUtils.GetOrCreate(gameObject).LoadYield(url,  delegate( WWW www )
            {
                if (string.IsNullOrEmpty(www.error) == false)
                {
                    Logger.LogError("下载Version.txt服务器文档失败:" + www.url + ";" + www.error);
                    string text = "网络连接中断，重新连接继续更新！";//"下载Version.txt服务器文档失败:" + www.url + ";" + www.error : 
                    StartCoroutine(OnlineUpdateFailYield(nextAgainUITime, text));
                }
                else
                {
                    if (www.bytes == null)
                    {
                        Logger.LogError("下载Version.txt服务器文档错误 www.bytes null :");
                    }
                    else
                    {
                        //Logger.LogError("下载Version.txt服务器文档内容 :" + System.Text.UTF8Encoding.UTF8.GetString(www.bytes));
                    }
                    if (string.IsNullOrEmpty(www.text))
                    {
                        Logger.LogError("下载Version.txt服务器文档错误 www.text null :");
                    }
                    else
                    {
                       // Logger.LogError("下载Version.txt服务器文档 www.text :" + www.text);
                    }
                   
                    ResVersionModel.getInstance().injectServerVersionMD5Data(Encoding.UTF8.GetString(www.bytes));
                    /// 比较版本
                    if (ResVersionModel.getInstance().comparedVersion() == false)//比较失败
                    {
                        StartCoroutine(OnlineUpdateFailYield(nextAgainUITime, "网络连接中断，重新连接继续更新！"));
                        return;
                    }
                    
                    //获取更新资源的总大小
                    long totalUpdateSize = ResVersionModel.getInstance().getTotalUpdateSize();
                    if (totalUpdateSize==0)
                    {
                        Release();
                        return;
                    }
                    finishLoadSize = 0;
					 completeLoadSize = 0;
                    ResOnlineUpdataUI.instance.showProgressNum = true;
                    ResOnlineUpdataUI.instance.SetResNeedUpdateTotalSize(totalUpdateSize, UpdataRes);
                    

                }
            }
            )
            );
        }


        /// <summary>
        /// 更新资源
        /// </summary>
       private void UpdataRes()
        {
            
            if (ResVersionModel.getInstance().currentUpdateIndex>=ResVersionModel.getInstance().updateRes.Count  )//所有资源下载完成
            {
                ResOnlineUpdataUI.instance.showProgressNum = false;
                UpdateLocalVersionFile();
            }
            else
            {
                ResUpdateInfo resUp = ResVersionModel.getInstance().updateRes[ResVersionModel.getInstance().currentUpdateIndex];               
                string szUrl = ResOnlineUpdateModel.getInstance().ResServerUrl + resUp.key;
               ResOnlineUpdataUI.instance.setWillLoadedSize( finishLoadSize+resUp.length);

                Logger.Log("更新资源服务器地址:" + szUrl );
                StartCoroutine(HttpUtils.GetOrCreate(gameObject).LoadYield(szUrl, delegate(WWW www) 
                {
                    finishLoadSize += resUp.length;
                   // ResOnlineUpdataUI.instance.ShowProgress(finishLoadSize);
                    if ( null == www.error )
                    {
						completeLoadSize += resUp.length;
                        ReplaceLocalRes(FileLoadPath.PersistentPath + resUp.key, www.bytes);
                        ResVersionModel.getInstance().updateResState(resUp,true);
                        SaveServerVersionToLocal(resUp);
                        if( null != www.assetBundle )
                        {
                            www.assetBundle.Unload(false);
                        }
                    }
                    else
                    {
                        Logger.LogError("下载服务器资源错误:" + www.url + www.error);
                        ResVersionModel.getInstance().updateResState(resUp, false);
                    }
                    ResVersionModel.getInstance().currentUpdateIndex++;
                    UpdataRes();//即时加载错误也加载更新下一个资源
                }));
            }
        }

        

        /// <summary>
        ///更新一个资源成功后，更新本地端version MD5文件。
        /// </summary>
        private void SaveServerVersionToLocal(ResUpdateInfo info)
        {
            StringBuilder sb = new StringBuilder();
            if (ResVersionModel.getInstance().localVersionMd5Dic.Count == 0)
            {
                Dictionary<string, ResUpdateInfo> dic  =ResVersionModel.getInstance().getSucceseUpdateResInfos();
                foreach (var item in dic.Values) 
                {
                    
                    sb.Append(item.key + "," + item.md5 + "," + item.length + "\n");
                }
            }
            else
            {
                List<ResUpdateInfo> localVersionMD5 = new List<ResUpdateInfo>(ResVersionModel.getInstance().localVersionMd5Dic.Values);
                for (int i = 0, length = localVersionMD5.Count; i < length; i++)
                {
                    if (localVersionMD5[i].key == info.key)
                    {
                        //本地的version MD5 替换为 服务器 MD5
                       ResVersionModel.getInstance().localVersionMd5Dic[info.key] = info;
                        sb.Append(info.key + "," + info.md5 + "," + info.length + "\n");
                    }
                    else
                    {
                        sb.Append(localVersionMD5[i].key + "," + localVersionMD5[i].md5 + "," + localVersionMD5[i].length + "\n");
                    }
                }
            }

            string pasdf = EnDeTool.EnStr(sb.ToString());
            ReplaceLocalRes(ResOnlineUpdateModel.getInstance().VersionPersistentPath + ResOnlineUpdateModel.getInstance().versionTxtPath, Encoding.UTF8.GetBytes(pasdf));
        }
        /// <summary>
        /// 更新资源完毕,保存服务器version md5 至 本地端。
        /// </summary>
        private void UpdateLocalVersionFile()
        {
                StringBuilder sb = new StringBuilder();
                Dictionary<string, ResUpdateInfo>  failUpdateInfosDic = ResVersionModel.getInstance().getFailUpdateResInfos();//更新失败的文件
                foreach (var item in ResVersionModel.getInstance().serverVersionMd5Dic.Values)
                {
                    //更新失败
                    if (failUpdateInfosDic.ContainsKey(item.key))
                    {
                        //替换失败,保留老版本
                        sb.Append(ResVersionModel.getInstance().getLocalVersionStr(item.key));
                    }
                    else
                    {
                        sb.Append(item.key + "," + item.md5 + "," + item.length + "\n");
                    }
                }

                string pasdf = EnDeTool.EnStr(sb.ToString());
                ReplaceLocalRes(ResOnlineUpdateModel.getInstance().VersionPersistentPath + ResOnlineUpdateModel.getInstance().versionTxtPath, Encoding.UTF8.GetBytes(pasdf));
                #if UNITY_EDITOR
                                UnityEditor.AssetDatabase.SaveAssets();
                                //UnityEditor.AssetDatabase.Refresh();
                #endif
                if (completeLoadSize < ResVersionModel.getInstance().getTotalUpdateSize())
                {
                    string text = "本次更新资源包中有 " + ResUpdateUtils.ByteToTrillion(ResVersionModel.getInstance().getTotalUpdateSize() - completeLoadSize) + "资源没有更新成功,请检查网络环境,更新剩余资源！";
                    Logger.LogError(text );
                    StartCoroutine(OnlineUpdateFailYield(nextAgainUITime, text));
                }
                else
                {
                    Invoke("ResUpFinishEex", 0f);
                }
  
        }

        /// <summary>
        /// 下载资源更新完成延迟执行事件
        /// </summary>
        private void ResUpFinishEex()
        {
            //更新本地code.txt
            Logger.Log("替换本地code.txt");
            string code = VersionModel.getInstance().vcServer + "";
            ReplaceLocalRes(ResOnlineUpdateModel.getInstance().VersionPersistentPath + ResOnlineUpdateModel.getInstance().codeTxtPath,Encoding.UTF8.GetBytes(code));
            Release();
        }
        /// <summary>
        /// 将下载的资源更新到本地。
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        private void ReplaceLocalRes(string fileName, byte[] data)
        {
            Logger.Log("ReplaceLocalRes : " + fileName);

            FileInfo fileInfo = new FileInfo(fileName);
            if (fileInfo.Exists)
            {
                //存在先销毁
                File.Delete(fileName);
                //Logger.Log("ReplaceLocalRes 存在先销毁: " + fileName);
            }
            else
            {
                if (fileInfo.Directory.Exists == false)
                {
                    fileInfo.Directory.Create();
                }
             
            }
            FileStream stream = new FileStream(fileName, FileMode.Create,FileAccess.ReadWrite);
            stream.Write(data, 0, data.Length);
            stream.Flush();
            stream.Close();
     
        }


        /// <summary>
        /// 网络失败，提示
        /// </summary>
        /// <param name="time"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public IEnumerator OnlineUpdateFailYield(float time, string text)
        {
            yield return new WaitForSeconds(time);
            Logger.LogError("延迟" + time + "秒，执行重新更新：");
            ResOnlineUpdataUI.instance.OnlineUpdateFail(AgainOnlineUpdate, text);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        private  void Release()
        {
            if (_resultdele != null)
            {
                VersionSavedData.getInstance().init(VersionModel.getInstance().appVersionCode, VersionModel.getInstance().vcServer);
                _resultdele(true);
            }
            Object.Destroy(this);
            ResOnlineUpdateModel.Dispose();
            VersionModel.Dispose();
            ResVersionModel.Dispose();
            ResOnlineUpdataUI.instance.updateComplete = true;
            if (null != UpdateEndCallback)
            {
                UpdateEndCallback(true);
            }

        }


        



    }

