using UnityEngine;
using System.Collections.Generic;

    /// <summary>
    /// 资源缓存引用单列，一个GameObject只能有一个
    /// 资源类型默认为 prefab
    /// </summary>
    public class RESRecommend : MonoBehaviour
    {
      
       private RESBase resBaseKey;

       public RESBase ResKey
       {
           get { return resBaseKey; }
       }
       /// <summary>
       /// 初始化数据
       /// </summary>
       /// <param name="reskey"></param>
       /// <param name="recommends"></param>
       public void Init(RESBase reskey, int num = 1)
       {
           if (reskey == null)
           {
               Logger.LogError("RESRecommend Init reskey null");
               return;
           }
           if (resBaseKey == null)
           {
               this.resBaseKey = reskey;
               AddRecommend(this.resBaseKey, num);
           }
           else if (reskey.key.Equals(resBaseKey.key)==false)
           {
               SubRecommend(this.resBaseKey, num);
               this.resBaseKey = reskey;
               AddRecommend(this.resBaseKey, num);
           }
           
           
       }
       /// <summary>
       /// 增加本资源缓存引用
       /// </summary>
       /// <param name="reskey"></param>
       /// <param name="num"></param>
       private void AddRecommend(RESBase reskey, int num = 1)
       {

           if (reskey != null && string.IsNullOrEmpty(reskey.key) == false)
           {
                //Logger.LogError("RESRecommend AddRecommend:" + reskey.key);
                RES.AddRESCacheRecommend(reskey.key, reskey.type, num);
           }
              
          
            
       }
       /// <summary>
       /// 减少本资源缓存引用
       /// </summary>
       /// <param name="num"></param>
       private void SubRecommend(RESBase reskey, int num = 1)
       {

           if (reskey != null && string.IsNullOrEmpty(reskey.key) == false)
           {
               //Logger.LogError("RESRecommend SubRecommend:" + reskey.key);
               RES.SubRESCacheRecommend(reskey.key, reskey.type, num);
           }
               
          
            
       }
        /// <summary>
        /// 减少本资源引用
        /// </summary>
       public void ExeSubRecommend()
       {
           if (this != null)
           {
               SubRecommend(this.resBaseKey);
               this.resBaseKey = null;
           }
        
       }
       public void OnDestroy()
       {
           SubRecommend(this.resBaseKey);
           this.resBaseKey = null;
       }
    }

