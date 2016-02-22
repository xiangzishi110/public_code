using System;

/// <summary>
/// 资源解析接口
/// </summary>
    public interface IResolve
    {
        /// <summary>
        /// 解析
        /// </summary>
        void Resolve(System.Object baseRes, RESResolveSuccedDelegate succed, RESResolveProgressDelegate progress, RESResolveErrorDelegate error);
 
        /// <summary>
        /// 重置
        /// </summary>
       void Reset();
      
    }


