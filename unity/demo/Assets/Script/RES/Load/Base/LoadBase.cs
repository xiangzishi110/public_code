
using UnityEngine;
using System.Collections.Generic;

    /// <summary>
    /// 加载资源封装基类
    /// </summary>
    public class LoadBase
    {
        /// <summary>
        /// 资源名称
        /// </summary>
        public string resName;
        /// <summary>
        /// 资源相对路径,不可带文件后缀名
        /// </summary>
        public string resPath;
        /// <summary>
        /// 资源加载模式。RESManageType:Resources or AB
        /// </summary>
        public RESManageType resManageType;
        /// <summary>
        /// 资源类型。RESFormat
        /// </summary>
        public string resType;
        /// <summary>
        /// 资源下载的绝对路径
        /// </summary>
        public string resAbPath;

        /// <summary>
        /// 加载解析资源完成。Activity，完成; 
        /// </summary>
        public ActivityEnum isLoadCoomand = ActivityEnum.Disable;

        /// <summary>
        /// 在加载状态.Activity，加载状态中; 
        /// </summary>
        public ActivityEnum isLoad = ActivityEnum.Disable;

        /// <summary>
        /// 资源缓存唯一(标签)主键
        /// </summary>
        public string resKey
        {
            get
            {

                if (resManageType == RESManageType.ASSETBUNDLE)
                    return resPath + "_" + resType;
                else
                    return resPath;
            }
        }

        public LoadBase()
        {

        }
        /// <summary>
        /// 初始化数据。resPath 相对路径不带后缀名;AB模式：resAbPath加载绝对路径自动加上 "_"+resType+".assetbundle 或 bytes"。
        /// </summary>
        /// <param name="resName">资源名称</param>
        /// <param name="resPath">资源相对路径不带后缀名</param>
        /// <param name="resType">资源类型</param>
        /// <param name="resManageType">资源模式</param>
        public LoadBase(string resName, string resPath, string resType, RESManageType resManageType=RESManageType.AUTO)
        {
            this.resName = resName;

            this.resPath = resPath;

            if (resManageType == RESManageType.AUTO)
                this.resManageType = RES.resManageType;
            else
                this.resManageType = resManageType;

            this.resType = resType;

            this.resAbPath = RESTool.LoadAbPathByPath(resPath, resType, this.resManageType);

            //Debug.Log("加载资源路径: " + this.resAbPath + " ; " + this.resKey);
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="resName">资源名称</param>
        /// <param name="resPath">资源相对路径</param>
        /// <param name="resType">资源类型</param>
        /// <param name="resAbPath">资源完整相对路径包含".+后缀名"</param>
        /// <param name="resManageType">资源模式</param>
        public LoadBase(string resName, string resPath, string resType, string resAbPath, RESManageType resManageType = RESManageType.AUTO)
        {
            this.resName = resName;

            this.resPath = resPath;

            if (resManageType == RESManageType.AUTO)
                this.resManageType = RES.resManageType;
            else
                this.resManageType = resManageType;

            this.resType = resType;

            this.resAbPath = resAbPath;

            //Debug.Log("加载资源路径: " + this.resAbPath + " ; " + this.resKey);
        }
    }

