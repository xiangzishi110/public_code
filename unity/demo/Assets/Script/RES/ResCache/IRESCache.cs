using System.Collections.Generic;
using System;
/// <summary>
/// 加载资源缓存接口
/// </summary>
    public interface IRESCache
    {
        /// <summary>
        /// 初始化资源
        /// </summary>
        /// <param name="resKey">资源标签</param>
        /// <param name="baseRes">原始资源</param>
        /// <param name="res">解析后资源</param>
        void Init(string resKey, string resType, System.Object baseRes, System.Object res, RESManageType resManage);

        /// <summary>
        /// 释放内存,true,释放内存。
        /// </summary>
        void Release();

        /// <summary>
        /// 缓存引用数量
        /// </summary>
        int GetRecommend();

        /// <summary>
        /// 增加引用个数
        /// </summary>
        /// <param name="num"></param>
        void AddRecommend(int num);
        /// <summary>
        /// 减少引用个数,true,释放内存。
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        bool SubRecommend(int num);
        /// <summary>
        /// 原始资源
        /// </summary>
        System.Object BaseRes();
        /// <summary>
        /// 解析后的资源
        /// </summary>
        System.Object Res();
        /// <summary>
        /// 本资源标签
        /// </summary>
        string ResKey();
        /// <summary>
        /// 获取资源模式
        /// </summary>
        /// <returns></returns>
        RESManageType ResManage();

    }



