using UnityEngine;
using System.Collections;
using System.Collections.Generic;
    /// <summary>
    /// 加载代理器
    /// 一个接着一个加载
    /// </summary>
    public class BatchProxy : MonoBehaviour
    {

        public BatchProxy()
        {
            progressDic = new Dictionary<string, float>();
            cacheProgressDic = new Dictionary<string, float>();
            _hashCode = this.GetHashCode();
        }
        /// <summary>
        /// 委托事件
        /// </summary>
        private RESSuccedDelegate _succed;
        private RESProgressDelegate _progress;
        private RESErrorDelegate _error;
        /// <summary>
        /// 加载列。前引用资源,后无引用资源.
        /// </summary>
        private LoadBase[] _loadBases;
        /// <summary>
        /// 总进度
        /// </summary>
        public Dictionary<string, float> progressDic;
        /// <summary>
        /// 缓存进度
        /// </summary>
        public Dictionary<string, float> cacheProgressDic;
        /// <summary>
        /// 可用，Disable 空闲可用；Activity 工作不可用
        /// </summary>
        private ActivityEnum _activity=ActivityEnum.Disable;
        /// <summary>
        /// 获取可用状态,Disable 空闲可用；Activity 工作不可用
        /// </summary>
        public ActivityEnum GetAcitivity
        {
            get { return _activity; }
        }
        /// <summary>
        /// 是否显示进度条。Activity，显示; 
        /// </summary>
        private ActivityEnum _showProgressUI = ActivityEnum.Disable;
        /// <summary>
        /// 唯一HashCode
        /// </summary>
        private int _hashCode;
        /// <summary>
        /// 更新批量(单个)加载代理
        /// </summary>
        /// <param name="loadBase"></param>
        public void UpdateLoadInfo(LoadBase[] loadBases,ActivityEnum  showProgressUI)
        {
            if (loadBases == null || loadBases.Length == 0)
            {
                Logger.LogError("BattchProxy UpdateLoadInfo loadBasses null To AllSucced");
                AllSucced();
                return;
            }
            else
            {
                _activity = ActivityEnum.Activity;
                _showProgressUI = showProgressUI;
                _loadBases = loadBases;
                //初始缓存进度
                if (_loadBases != null)
                {
                    for (int i = 0, length = _loadBases.Length; i < length; i++)
                    {
                        cacheProgressDic[_loadBases[i].resKey] = 0;
                    }
                }
            }

        }
        /// <summary>
        /// 关联委托事件,最先执行
        /// </summary>
        /// <param name="succed"></param>
        /// <param name="progress"></param>
        /// <param name="error"></param>
        public void AddDelegate(RESSuccedDelegate succed = null, RESProgressDelegate progress = null, RESErrorDelegate error = null)
        {
            if (succed != null)
            {
                if (this._succed ==null)
                    this._succed = succed;
                else
                     this._succed += succed;
            }
            if (progress != null)
            {
                if (this._progress == null)
                    this._progress = progress;
                else
                    this._progress += progress;
               
            }
            if (error != null)
            {
                if (this._error == null)
                    this._error = error;
                else
                    this._error += error;
            }
        }

        #region 一个接着一个加载
        
        /// <summary>
        /// 当前
        /// </summary>
        private int currentIndex;
        
        /// <summary>
        /// 开始加载
        /// </summary>
        public virtual void Load()
        {
            currentIndex = 0;
            CheckLoad();
        }
        /// <summary>
        /// 检查加载
        /// </summary>
        private void CheckLoad()
        {
            //检查是否加载完成
            if (currentIndex >= _loadBases.Length)
            {
                return;
            }
            LoadBase loadBase = _loadBases[currentIndex];
            //检查是否加载过
            if (RES.ResExist(loadBase.resPath, loadBase.resType,loadBase.resManageType))
            {
                RESItemLoadSucced(loadBase);
                return;
            }
            _loadBases[currentIndex].isLoad = ActivityEnum.Activity;
            //Resources中加载
            if (loadBase.resManageType == RESManageType.RESOURCES)
            {
                //检查是否已经在加载了
                ResLoadCommand[] loadCmdArray = RES.main.GetComponentsInChildren<ResLoadCommand>();
                foreach (LoadCommand item in loadCmdArray)
                {
                    if (item.IsActivity == ActivityEnum.Activity && item.loadBase != null && item.loadBase.resKey.Equals(loadBase.resKey))
                    {
                        item.AddDelegate(RESItemLoadSucced, RESItemLoadProgress, RESItemLoadError);
                        return;
                    }
                }

                ILoad loader = RES.GetLoader(loadBase.resManageType);
                loader.AddDelegate(RESItemLoadSucced, RESItemLoadProgress, RESItemLoadError);
                loader.InitData(loadBase);

            }
            else
            {
                ABLoadCommand[] loadList = RES.main.GetComponentsInChildren<ABLoadCommand>();
                //检查是否已经在加载了
                foreach (LoadCommand item in loadList)
                {

                    if (item.IsActivity == ActivityEnum.Activity && item.loadBase != null && item.loadBase.resKey.Equals(loadBase.resKey))
                    {
                        item.AddDelegate(RESItemLoadSucced, RESItemLoadProgress, RESItemLoadError);
                        return;
                    }
                }

                ILoad loader = RES.GetLoader(loadBase.resManageType);
                loader.AddDelegate(RESItemLoadSucced, RESItemLoadProgress, RESItemLoadError);
                loader.InitData(loadBase);
            }
        }

        /// <summary>
        /// 单个加载错误
        /// </summary>
        /// <param name="loadBase"></param>
        /// <param name="error"></param>
        private void RESItemLoadError(LoadBase loadBase, string error)
        {
            if (this._error != null)
            {
                this._error(loadBase, error);
            }
            //判断是否全部加载完成(错误也算完成)
            RESItemLoadSucced(loadBase);
        }
        /// <summary>
        /// 单个加载进度
        /// 代理总进度
        /// </summary>
        /// <param name="progress"></param>
        private void RESItemLoadProgress(LoadBase loadBase, float progress)
        {
            progressDic[loadBase.resKey] = progress;
            float fProgress = 0f;
            foreach (var item in progressDic.Values)
            {
                fProgress += item;
            }

            if (_loadBases.Length > 0)
            {
                fProgress /= (float)_loadBases.Length;
            }
                        
            if (_showProgressUI == ActivityEnum.Activity)
            {
                RES.RESProgressUINotify(_hashCode, fProgress);
            }
            if (this._progress != null)
            {
                this._progress(fProgress);
            }
        }

        /// <summary>
        /// 单个加载完成
        /// </summary>
        /// <param name="loadBase"></param>
        private void RESItemLoadSucced(LoadBase loadBase)
        {

            //Logger.Log("单个加载成功 " + loadBase.resKey+"; "+loadBase.resType);

            currentIndex++;
            RESItemLoadProgress(loadBase, 1f);
            if (canRESSucced())
            {
                AllSucced();
            }
            else
            {
                CheckLoad();
            }
        }

        /// <summary>
        /// 是否所有加载完成
        /// </summary>
        /// <returns></returns>
        private bool canRESSucced()
        {
            if (currentIndex >= _loadBases.Length)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 所有加载完成后执行事件
        /// </summary>
        private void AllSucced()
        {
            if (this._succed != null)
            {
                this._succed();
            }

            Reset();
        }

        /// <summary>
        /// 重置
        /// </summary>
        protected void Reset()
        {
            if (this != null)
            {
                this.currentIndex = 0;
                this._succed = null;
                this._progress = null;
                this._error = null;
                this.progressDic.Clear();
                this.cacheProgressDic.Clear();
                this._loadBases = null;
                this._activity = ActivityEnum.Disable;
                Destroy(this);
            }

        }
        

        #endregion 一个接着一个加载
        /// <summary>
        /// 是否在加载队列中
        /// </summary>
        public bool IsInProxy(string szKey)
        {
            if (null == _loadBases || _activity == ActivityEnum.Disable)
            {
                return false;
            }

            foreach( var load in _loadBases )
            {
                if (load.resPath == szKey)
                {
                    return true;
                }
            }

            return false;
        }
    }