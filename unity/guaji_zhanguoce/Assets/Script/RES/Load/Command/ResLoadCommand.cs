using UnityEngine;
using System.Collections.Generic;
using System.Collections;

    /// <summary>
    /// Resources 加载单列
    /// </summary>
   public class ResLoadCommand : LoadCommand
    {
        private ResourceRequest resRequest;
        /// <summary>
        /// 加载
        /// </summary>
        public override IEnumerator Load()
        {
            if (base.loadBase == null)
            {
                base.ResError("ResLoadCommand loadBase null");
                yield return null;
            }
            else if (string.IsNullOrEmpty(base.loadBase.resAbPath))
            {
                base.ResError("ResLoadCommand loadBase.resAbPath null ");
                yield return null;
            }
            else
            {
                System.Type type = RESTool.GetTypeByRESFormat(base.loadBase.resType);
                if (type != null)
                    resRequest = Resources.LoadAsync(base.loadBase.resAbPath, type);
                else
                    resRequest = Resources.LoadAsync(base.loadBase.resAbPath);
                yield return resRequest;
            }

        }
        void Update()
        {
            if (base.IsLoad == ActivityEnum.Disable || resRequest == null)
                return;
           
            if (resRequest.isDone)
            {
                base.ResProgress(resRequest.progress / 2f);
                base.ResSucced(resRequest.asset);
            }
            else
            {
                base.ResProgress(resRequest.progress/2f);
            }
        }
        public override void Reset()
        {
            resRequest = null;
            base.Reset();
        }
    }
