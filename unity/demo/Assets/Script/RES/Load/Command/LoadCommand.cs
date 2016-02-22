
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    /// <summary>
    /// 单个加载命令
    /// </summary>
    public class LoadCommand :MonoBehaviour, ILoad
    {
        private LoadBase _loadBase=new LoadBase ();

        public LoadBase loadBase
        {
            get { return _loadBase; }

        }

        private RESLoadSuccedDelegate resSucced;
        private RESLoadProgressDelegate resProgress;
        private RESLoadErrorDelegate resError;

        /// <summary>
        /// 原始资源
        /// </summary>
        //private System.Object baseRes;

        /// <summary>
        /// 解析后资源
        /// </summary>
       // private System.Object res;

       /// <summary>
        /// 是否处于可使用状态,Disable,空闲；Activity,已使用;
       /// </summary>
        private ActivityEnum isActivity = ActivityEnum.Disable;

        /// <summary>
        /// 是否处于可使用状态,Disable,空闲；Activity,已使用;
        /// </summary>
        public ActivityEnum IsActivity
        {
            get { return isActivity; }
            set { isActivity = value; }
        }

        /// <summary>
        /// 是否处于加载状态。是 Activity，处于;
        /// </summary>
        private ActivityEnum isLoad = ActivityEnum.Disable;

        /// <summary>
        /// 是否处于加载状态。是，处于;
        /// </summary>
        public ActivityEnum IsLoad
        {
            get { return isLoad; }
            set { isLoad = value; }
        }
        /// <summary>
        /// 缓存进度
        /// </summary>
        public float cacheProgress = 0f;
        /// <summary>
        /// 当前进度
        /// </summary>
        public float progress = 0f;

        /// <summary>
        /// 初始化数据,开始加载
        /// </summary>
        /// <param name="loadBase"></param>
        /// <param name="resSucced"></param>
        /// <param name="resProgress"></param>
        /// <param name="resError"></param>
        public virtual void InitData(LoadBase loadBase)
        {
            this.enabled = true;

            _loadBase = loadBase;

           
            if (ResExist())
            {
                LoadSucced();
            }
            else
            {
                IsActivity = ActivityEnum.Activity;
                IsLoad = ActivityEnum.Activity;
                cacheProgress = 0f;
                progress = 0f;
                StartCoroutine(Load());
                StartCoroutine(ListenerProgress());
            }
         
        }
        /// <summary>
        /// 关联委托事件,最先执行
        /// </summary>
        /// <param name="resSucced"></param>
        /// <param name="resProgress"></param>
        /// <param name="resError"></param>
        public virtual void AddDelegate(RESLoadSuccedDelegate resSucced, RESLoadProgressDelegate resProgress, RESLoadErrorDelegate resError)
        {

            if (resSucced != null)
                this.resSucced += resSucced;
            if (resProgress != null)
                this.resProgress += resProgress;
            if (resError != null)
                this.resError += resError;
        }
        /// <summary>
        /// 资源是否已经缓存了。true,存在
        /// </summary>
        /// <returns></returns>
        public virtual bool ResExist()
        {
            return RES.ResExist(loadBase.resPath,loadBase.resType,loadBase.resManageType);
        }
        /// <summary>
        /// 加载
        /// </summary>
        /// <returns></returns>
        public virtual System.Collections.IEnumerator Load()
        {
             return null;
        }

        /// <summary>
        /// 解析加载完成的原始资源
        /// </summary>
        public virtual void Resolve(System.Object baseRes)
        {
            if (ResExist())
            {
                LoadSucced();
            }
            else
            {
                BaseResolve baseResolve = RES.GetResolve(this.loadBase.resType);

                if (baseResolve != null)
                {
                    baseResolve.Resolve(baseRes, ResResolveSucced, ResResolveProgress, ResResolveError);
                }
                else
                {
                    ResError("Error Resolve null : "+this.loadBase.resType);
                }
            }
           
        }

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            if (this != null)
            {
                this.resSucced = null;
                this.resProgress = null;
                this.resError = null;
                this._loadBase = null;
                //this.baseRes = null;
                //this.res = null;
                IsLoad = ActivityEnum.Disable;
                IsActivity = ActivityEnum.Disable;
                this.enabled = false;
                this.cacheProgress = 0f;
                this.progress = 0f;
                Object.Destroy(this);
            }

            
        }
        /// <summary>
        /// 加载解析资源成功，处理事件
        /// </summary>
        public virtual void LoadSucced() 
        {
            ResProgress(1f);

            if (resSucced != null)
                resSucced(loadBase);

            Reset();
        } 
        /// <summary>
        /// 加载成功
        /// </summary>
        /// <param name="ob"></param>
        public void ResSucced(System.Object ob) 
        {
            IsLoad = ActivityEnum.Disable;
            if (ob != null)
            {
                //this.baseRes = ob;

                Resolve(ob);
            }
            else
            {
                ResError(this.loadBase.resPath + " Object null");
            }
        }
        /// <summary>
        /// 加载失败
        /// </summary>
        /// <param name="error"></param>
        public void ResError(string error) 
        {
            ResProgress(1f);

            IsLoad = ActivityEnum.Disable;
            Logger.LogError(this.loadBase.resKey + ";" + error);
            if (this.resError != null)
            {
                this.resError(this.loadBase, error);
            }
            Reset();
        }
        /// <summary>
        /// 加载进度
        /// </summary>
        /// <param name="Progress"></param>
        public void ResProgress(float progress) 
        {
            this.progress = progress;
            if (resProgress != null)
            {
                resProgress(loadBase,progress);
            }
        }
        /// <summary>
        /// 加载代理错误关闭
        /// </summary>
        /// <returns></returns>
        private IEnumerator ListenerProgress()
        {
            while (isLoad == ActivityEnum.Activity)
            {
                yield return new WaitForSeconds(RES.LoadErrorMaxTime);
                if (IsActivity == ActivityEnum.Activity)
                {
                    if (cacheProgress == progress)
                    {
                        Logger.LogError(_loadBase.resKey + ";加载" + RES.LoadErrorMaxTime + "秒无反应！");
                        ResError(_loadBase.resKey + ";加载" + RES.LoadErrorMaxTime + "秒无反应！");
                    }
                    else
                        cacheProgress = progress;
                }
            }
        }
        /// <summary>
        ///解析成功
        /// </summary>
        /// <param name="ob">解析成功后的资源</param>
        public void ResResolveSucced(System.Object baseRes, System.Object ob)
        {
            if (ResExist())
            {
                LoadSucced();
            }
            else
            {
                if (ob == null)
                {
                    ResError("LoadCommand ResResolveSucced   baseRes ob null");
                }
                else
                {
                    RES.AddRESCache(baseRes, ob, loadBase);

                    LoadSucced();
                }
            }
        
        }
        /// <summary>
        /// 解析失败
        /// </summary>
        /// <param name="error"></param>
        public void ResResolveError(string error)
        {
            ResProgress(1f);

            ResError(error);
        }
        /// <summary>
        /// 解析进度
        /// </summary>
        /// <param name="progress"></param>
        public void ResResolveProgress(float progress)
        {
            ResProgress(1f);
        }

    }

