using UnityEngine;
using System.Collections.Generic;
using Tools.UI;
 /// <summary>
 /// 加载UI资源，并实例化。
 /// </summary>
public class RESLoadUICommand : MonoBehaviour
    {
        public static RESLoadUICommand Instance 
        {
            get 
            { 
                GameObject obj = new GameObject("RESLoadUICommand");                
                GameObject.DontDestroyOnLoad(obj);
                return obj.AddComponent<RESLoadUICommand>();
            }
        }

        void FinishNotity() 
        {
            UITools.destroy(gameObject);
        }

        //================================= 单个加载 ========================================//

        #region 单个加载 

        /// <summary>
        /// 加载UI资源完成的回调，true,成功。res,资源相对路径或名称;实例化返回GameObject
        /// </summary>
        public delegate void FinishUIRESNotify(string resOrName, UnityEngine.GameObject go);
        private FinishUIRESNotify _finishUIRESNotify;
      

        /// <summary>
        /// UI资源相对路径或名称
        /// </summary>
        public LoadBase loadBase;
        //private Vector3 _position;
        //private Transform _parent;
        private object _calldata;
        //private int _uiPanelDepth = 0;
        private UIMode _uimode;

        /// <summary>
        ///加载有引用其它素材的UI资源，并实例化返回GameObject。
        ///resPathOrName,相对路径且没有后缀名 or ResourceConfig 中的name 且必须是ResourceItem
        /// UI实例化后不加入到UI管理器
        /// </summary>
        public void LoadUIRESByURL3(string resPathOrName, FinishUIRESNotify notity = null,ActivityEnum showProgressUI = ActivityEnum.Disable,object calldata=null)
        {
            _uimode = UIMode.None;
            _finishUIRESNotify = notity;
            if (calldata != null)
                this._calldata = calldata;
            if (RES.ResourceConfigExits(resPathOrName))
            {
                loadBase = new LoadBase();

                loadBase.resPath = resPathOrName;
                loadBase.resType = RESFormat.PREFAB;
                
                RES.LoadByName(resPathOrName, UIComplete, showProgressUI);
            }
            else
            {
                loadBase = RES.GetLoadBase(resPathOrName, RESFormat.PREFAB);

                RES.LoadByURL(loadBase, UIComplete, showProgressUI);
            }
            
          
        }
        /// <summary>
        ///加载有引用其它素材的UI资源，并实例化返回GameObject。
        ///resPathOrName,相对路径且没有后缀名 or ResourceConfig 中的name 且必须是ResourceItem
        /// UI实例化后不加入到UI管理器，放入UIManager下
        /// </summary>
        public void LoadUIRESByURL2(string resPathOrName, FinishUIRESNotify notity = null, ActivityEnum showProgressUI = ActivityEnum.Disable, object calldata = null)
        {
            this._finishUIRESNotify = notity;
            if (calldata != null)
                this._calldata = calldata;
            if (RES.ResourceConfigExits(resPathOrName))
            {
                loadBase = new LoadBase();

                loadBase.resPath = resPathOrName;
                loadBase.resType = RESFormat.PREFAB;

                RES.LoadByName(resPathOrName, UIComplete2, showProgressUI);
            }
            else
            {
                loadBase = RES.GetLoadBase(resPathOrName, RESFormat.PREFAB);

                RES.LoadByURL(loadBase, UIComplete2, showProgressUI);
            }
        }
        
        /// <summary>
        ///加载有引用其它素材的UI资源，并实例化返回GameObject。
         ///resPathOrName,相对路径且没有后缀名 or ResourceConfig 中的name
        /// UI实例化后加入到UI管理器
        /// </summary>
        public void LoadUIRESByURL(string resPathOrName, object calldata = null, FinishUIRESNotify notity = null, ActivityEnum showProgressUI = ActivityEnum.Disable)
        {

            _uimode = UIMode.Container;
            //_position = UIManager.intance.getPosition();
            //_parent = UIManager.intance.transform;
           // _uiPanelDepth = UIManager.intance.GetLayerPanelDepth();
            _calldata = calldata;
 
            _finishUIRESNotify = notity;

          

            if (RES.ResourceConfigExits(resPathOrName))
            {
               
                //Logger.LogError("Load by Name");
                loadBase = new LoadBase();

                loadBase.resPath = resPathOrName;
                loadBase.resType = RESFormat.PREFAB;

                RES.LoadByName(resPathOrName, UIComplete, showProgressUI);
            }
            else
            {
               // Logger.LogError("Load by URL");
                loadBase = RES.GetLoadBase(resPathOrName, RESFormat.PREFAB);
                
                RES.LoadByURL(loadBase, UIComplete, showProgressUI);
            }

            
        }
        /// <summary>
        /// 加载完成
        /// </summary>
        private void UIComplete()
        {
            LinkNetWorkTishiUI.instance.State(false);
            //首先检查UI缓存列表中是否存在
            GameObject go = UIManager.intance.GetUIByName(loadBase.resName + "(Clone)");
            if (null == go)
            {
                //不存在就实例化出来
                go = RES.GetRESGo(loadBase.resPath);
            }

            if (go == null)
            {
                if (_finishUIRESNotify != null)
                    _finishUIRESNotify(loadBase.resPath, null);
     
                FinishNotity();
                Logger.LogError("RESLoadInstantiate UIComplete object null;" + loadBase.resPath + " ; " + loadBase.resType);
                return;
            }
            /*
            go.transform.parent =_parent;
            go.transform.localPosition = _position;
            go.transform.localScale = Vector3.one;

            //设置公共图集
            //UITools.SetPublicAtlasRef(go.transform);

           // Logger.LogError("UI Complete!! gameObject name = " + go.name);

            if (_uimode == UIMode.Container) 
            {
                UIManager.intance.AddUI(go);
                UITools.UpdateUIPanleDepth(go, _uiPanelDepth);
            }
             * */

            if (_uimode == UIMode.Container)
            {
                UITools.UpdateUIPanleDepth(go, UIManager.intance.GetLayerPanelDepth());
                UIManager.intance.AddUI(go);
                go.transform.parent = UIManager.intance.transform ;
                go.transform.localPosition = UIManager.intance.getPosition();
                go.transform.localScale = Vector3.one;
            }

            if (_calldata == null)
            {
                go.transform.SendMessage("InitDataUIControl", SendMessageOptions.DontRequireReceiver);
            }
            else 
            {
                go.transform.SendMessage("InitData", _calldata, SendMessageOptions.DontRequireReceiver);
            }
    
            if (_finishUIRESNotify != null)
                _finishUIRESNotify(loadBase.resPath, go);

            go = null;

            FinishNotity();

        }
        /// <summary>
        /// 加载完成
        /// </summary>
        private void UIComplete2()
        {

            GameObject go = RES.GetRESGo(loadBase.resPath);

            if (go == null)
            {
                if (_finishUIRESNotify != null)
                    _finishUIRESNotify(loadBase.resPath, null);

                FinishNotity();
                Logger.LogError("RESLoadInstantiate UIComplete object null;" + loadBase.resPath + " ; " + loadBase.resType);
                return;
            }

            go.transform.parent = UIManager.intance.transform;
            UITools.UpdateUIPanleDepth(go, UIManager.intance.GetLayerPanelDepth());
            go.transform.localPosition = UIManager.intance.getPosition();
            go.transform.localScale = Vector3.one;
            if (_calldata == null)
            {
                go.transform.SendMessage("InitDataUIControl", SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                go.transform.SendMessage("InitData", _calldata, SendMessageOptions.DontRequireReceiver);
            }

            if (_finishUIRESNotify != null)
                _finishUIRESNotify(loadBase.resPath, go);
            go = null;
            FinishNotity();
        }
        #endregion 单个加载

        //================================= 单个加载 ========================================//

    }

