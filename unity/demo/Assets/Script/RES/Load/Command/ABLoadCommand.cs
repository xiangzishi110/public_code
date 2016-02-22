using UnityEngine;
using System.Collections.Generic;
using System.Collections;

    public class ABLoadCommand : LoadCommand
    {
        private WWW www;
        /// <summary>
        /// 加载
        /// </summary>
        public override IEnumerator Load()
        {
            //Debug.Log("ABLoadCommand 下载路径 : "+base.loadBase.resAbPath);
            if (base.loadBase == null)
            {
                base.ResError("ABLoadCommand loadBase null");
                yield return null;
            }
            else if (string.IsNullOrEmpty(base.loadBase.resAbPath))
            {
                base.ResError("ABLoadCommand loadBase.resAbPath null ");
                yield return null;
            }
            else
            {
                www = new WWW(base.loadBase.resAbPath);

                yield return www;
            }
        }
        void Update() 
        {
            if (base.IsLoad == ActivityEnum.Disable || www==null)
                return;
            if (www.isDone)
            {
                base.ResProgress(www.progress / 2f);

                if (string.IsNullOrEmpty(www.error))
                {
                    AssetBundle abRes = www.assetBundle;
                    if (AESTool.NeedDecrypt(www.bytes))
                    {
                        abRes = AESTool.DecryptCreateFromMemory(www.bytes);
                    }

                    base.ResSucced(abRes);
                }
                else
                {
                    Logger.LogError("加载错误:" + www.error);
                    base.ResError(www.error);                    
                }
            }
            else
            {
                base.ResProgress(www.progress/2f);
            }
        }

        public override void Reset()
        {
            www = null;
            base.Reset();
        }
    }

 


