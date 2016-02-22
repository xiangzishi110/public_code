using UnityEngine;
using System.Collections;


    /// <summary>
    /// 加载资源并解析接口
    /// </summary>
    public interface ILoad
    {
        /// <summary>
        /// 初始化数据,开始加载
        /// </summary>
        /// <param name="loadBase"></param>
         void InitData(LoadBase loadBase);

        /// <summary>
        /// 关联委托事件,最先执行
        /// </summary>
        /// <param name="resSucced"></param>
        /// <param name="resProgress"></param>
        /// <param name="resError"></param>
        void AddDelegate(RESLoadSuccedDelegate resSucced, RESLoadProgressDelegate resProgress, RESLoadErrorDelegate resError);
        /// <summary>
        /// 资源是否已经缓存了
        /// </summary>
        /// <returns></returns>
        bool ResExist();

        /// <summary>
        /// 加载
        /// </summary>
        IEnumerator Load();

        /// <summary>
        /// 解析资源
        /// </summary>
         void Resolve(System.Object baseRes);
        /// <summary>
        /// 重置
        /// </summary>
        void Reset();

    }

