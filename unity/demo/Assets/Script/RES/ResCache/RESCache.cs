using System.Collections.Generic;
using System;

    /// <summary>
    ///资源缓存封装类
    /// </summary>
    public class RESCache : IRESCache
    {
        /// <summary>
        /// 本资源标签
        /// </summary>
        private string resKey;
        /// <summary>
        /// 本资源类型
        /// </summary>
        private string resType;
        /// <summary>
        /// 原始资源
        /// </summary>
        private System.Object baseRes;
        /// <summary>
        /// 解析后资源
        /// </summary>
        private System.Object res;

        /// <summary>
        /// 是否实例化了
        /// </summary>
        private bool isInstance = false;

        /// <summary>
        /// 资源模式
        /// </summary>
        private RESManageType resManage = RESManageType.RESOURCES;

        /// <summary>
        /// 引用个数
        /// </summary>
        private int recommend = 0;

        /// <summary>
        /// 引用个数
        /// </summary>
        public int Recommend
        {
            get { return recommend; }
            set
            {
                if (!isInstance)
                    isInstance = true;
                recommend = value;
            }
        }


        /// <summary>
        /// 初始化资源
        /// </summary>
        /// <param name="resKey">资源标签</param>
        /// /// <param name="resType">资源类型</param>
        /// <param name="baseRes">原始资源</param>
        /// <param name="res">解析后资源</param>
        public void Init(string resKey, string resType, System.Object baseRes, System.Object res,RESManageType resManage)
        {

            this.resKey = resKey;
            this.resType = resType;
            this.baseRes = baseRes;
            this.res = res;
            this.resManage = resManage;

            Recommend = 0;
            isInstance = false;
   
        }
        /// <summary>
        /// 释放内存,true,释放内存。
        /// </summary>
        public  void Release()
        {
            if(res !=null)
                 res = null;

            if (baseRes != null)
            {
                if(baseRes is UnityEngine.AssetBundle)
                {
                    try
                    {
                        UnityEngine.AssetBundle ab = (baseRes as UnityEngine.AssetBundle);
                        if (null != ab)
                        {
                            ab.Unload(true);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Log("==============error:"+e.ToString());
                    }
                }                    
                else if (baseRes is UnityEngine.Object)
                {
                    if ( !( baseRes is UnityEngine.GameObject))
                    {
                        UnityEngine.Resources.UnloadAsset(baseRes as UnityEngine.Object);
                    }
                }
                  
            }
            if (baseRes != null)
                baseRes = null;
        }
        /// <summary>
        /// 增加引用个数
        /// </summary>
        /// <param name="num"></param>
        public  void AddRecommend(int num)
        {
            Recommend += num;
        }
        /// <summary>
        /// 减少引用个数,true,释放内存。
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public  bool SubRecommend(int num)
        {

            Recommend -= num;

            string szKey = resKey.Replace("_" + resType, "");
            if (RES.IsLoading(szKey))
            {
               // Logger.LogError(resKey + "在加载队里中，且引用为1,不能减除引用");
                return false;
            }

            if (isInstance && Recommend <= 0)
            {
                Release();
                return true;
            }
            return false;
        }
        /// <summary>
        /// 原始资源
        /// </summary>
        public System.Object BaseRes()
        {
            return baseRes;
        }
        /// <summary>
        /// 解析后的资源
        /// </summary>
        public  System.Object Res()
        {
            return res;
        }
        /// <summary>
        /// 本资源标签
        /// </summary>
        public  string ResKey()
        {
            return resKey;
        }
        /// <summary>
        /// 获取资源模式
        /// </summary>
        /// <returns></returns>
        public  RESManageType ResManage()
        {
            return resManage;
        }

        /// <summary>
        /// 缓存引用数量
        /// </summary>
        /// <returns></returns>
        public int GetRecommend()
        {
            return Recommend;
        }
    }



