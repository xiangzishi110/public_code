using UnityEngine;
using System.Collections.Generic;
using System.Collections;




    /// <summary>
    /// 解析资源命令
    /// </summary>
    public class BaseResolve : MonoBehaviour,IResolve
    {
        /// <summary>
        ///  是否处于可使用状态,Disable,空闲；Activity,已使用;
        /// </summary>
        private ActivityEnum isActivity = ActivityEnum.Disable;

        /// <summary>
        ///  是否处于可使用状态,Disable,空闲；Activity,已使用;
        /// </summary>
        public ActivityEnum IsActivity
        {
            get { return isActivity; }
            set { isActivity = value; }
        }

        public virtual void Resolve(System.Object baseRes, RESResolveSuccedDelegate succed, RESResolveProgressDelegate progress, RESResolveErrorDelegate error)
        {
              
        }

        public virtual void Reset()
        {
         
            if (this != null)
            {
                IsActivity = ActivityEnum.Disable;
                Object.Destroy(this);
            }
            
        }
    }

