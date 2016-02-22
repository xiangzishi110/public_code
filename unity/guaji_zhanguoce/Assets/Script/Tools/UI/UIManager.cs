using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace Tools.UI
{
    /// <summary>
    /// UI 控制类
    /// 自定义弹窗
    /// </summary>
    public class UIManager : MonoBehaviour
    {

        private static UIManager _intance;

        public static UIManager intance
        {
            get
            {
                return _intance;
            }
        }
        public void Destory()
        {
            GameObject.Destroy(_intance);
            _intance = null;
        }

        public Transform selfTrans;

        void Awake()
        {
            selfTrans = transform;
            mDoNotShutDownList = new List<string>(mDoNotShutDownArray);
            _intance = this;
            GameObject uiroot = GameObject.Find("UI Root/Camera");
            if (null != uiroot && null == uiroot.GetComponent<DontDestroy>())
            {
                uiroot.AddComponent<DontDestroy>();
            }
        }
        /// <summary>
        /// 显示主界面,true 显示。
        /// </summary>
        public void ShowMainUI(bool show)
        {

            if (show)
            {
                ClearAllUI();
            }
            else
            {
            }

        }


        /// <summary>
        /// UI GameObject List
        /// </summary>
        private List<GameObject> allUIList = new List<GameObject>();

        /// <summary>
        /// 不需要打断任务的UI
        /// </summary>
        private string[] mDoNotShutDownArray = new string[]
        {
            "NoticeUI"
        };
        string pauseUI = string.Empty;
        List<string> mDoNotShutDownList = null;
        /// <summary>
        /// 加载并显示UI，并放入UI管理器
        /// name,UI相对路径
        /// </summary>
        public void LoadAndShowUIByURL(string name, object call = null, RESLoadUICommand.FinishUIRESNotify notity = null, ActivityEnum showProgressUI = ActivityEnum.Activity)
        {
            LinkNetWorkTishiUI.instance.State(true, 2f);
            RESLoadUICommand.Instance.LoadUIRESByURL(name, call, notity, showProgressUI);
        }
        /// <summary>
        /// ui容器里的对象 
        /// </summary>
        /// <param name="go"></param>
        public List<GameObject> GetUIList()
        {
            return allUIList;
        }

        /// <summary>
        /// 获取UI
        /// </summary>
        public GameObject GetUIByName(string szName)
        {
            for (int iIndex = 0; iIndex < allUIList.Count; ++iIndex)
            {
                if (allUIList[iIndex] != null && allUIList[iIndex].name == szName)
                {
                    return allUIList[iIndex];
                }
            }
            return null;
        }

        /// <summary>
        /// ui容器添加UI GameObject
        /// </summary>
        /// <param name="go"></param>
        public void AddUI(GameObject go)
        {
            if (null == go)
            {
                Logger.LogError("UIManager添加UI失败，所要添加的UI为null");
                return;
            }
            GameObject oldUI = GetUIByName(go.name);
            if (null != oldUI)
            {
                allUIList.Remove(oldUI);
                oldUI.SetActive(true);
            }
            allUIList.Add(go);
            if (!go.activeSelf)
            {
                go.SetActive(true);
            }
            //检查缓存池
            CheckInActiveUI();
            if (allUIList.Count > UIConfig.mLimitCount)
            {
                Logger.LogWarning("ui缓存池容量溢出，当前个数为:" + allUIList.Count.ToString());
            }
        }

        /// <summary>
        /// 如果UI缓存池容量溢出，则关闭不活跃的UI
        /// </summary>
        private void CheckInActiveUI()
        {
            if (allUIList.Count > UIConfig.mLimitCount)
            {
                GameObject inActiveUI = FindInactiveUI();
                if (null != inActiveUI)
                {
                    DestroyUI(inActiveUI);
                    CheckInActiveUI();
                }
            }
        }

        /// <summary>
        /// 判断是否在隐藏/显示主UI列表中
        /// </summary>
        private bool IsInHideMainSceneLst(string szUIName)
        {
            foreach (string szHideName in UIConfig.mHideMainSceneUINameList)
            {
                if (szHideName == szUIName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 销毁所有 UI GameObject
        /// </summary>
        public void ClearAllUI()
        {

            for (int i = allUIList.Count - 1; i >= 0; i--)
            {
                if (allUIList[i] != null)
                {
                    CloseUI(allUIList[i]);
                }
            }

            //allUIList.Clear();

        }
        /// <summary>
        /// 销毁所有UI GameObject，除了noname GameObject
        /// </summary>
        /// <param name="noname"></param>
        public void ClearAllUI(string noname)
        {
            for (int i = allUIList.Count - 1; i >= 0; i--)
            {
                if (allUIList[i] != null && allUIList[i].name.Equals(noname) == false)
                {
                    Logger.Log("销毁UI GameObject:" + allUIList[i].name);
                    CloseUI(allUIList[i]);
                }
            }
        }

        /// <summary>
        ///隐藏所有 UI GameObject
        /// </summary>
        public void HideAllUI()
        {

            for (int i = 0, length = allUIList.Count; i < length; i++)
            {
                if (allUIList[i] != null && allUIList[i].activeSelf)
                    allUIList[i].SetActive(false);
            }
        }
        public void HideAllUI(string name)
        {

            for (int i = 0, length = allUIList.Count; i < length; i++)
            {
                if (allUIList[i] != null && allUIList[i].activeSelf)
                    allUIList[i].SetActive(false);
            }
        }
        /// <summary>
        /// 隐藏上一个UI
        /// </summary>
        /// <param name="go"></param>
        public void HideLastUI(GameObject go)
        {
            if (go == null)
            {
                if (allUIList != null && allUIList.Count > 0)
                {
                    go = allUIList[allUIList.Count - 1];
                }
            }

            if (go != null && go.activeSelf)
                go.SetActive(false);
        }
        /// <summary>
        /// 显示上一个UI
        /// </summary>
        /// <param name="go"></param>
        public void ShowLastUI(GameObject go)
        {
            if (go == null)
            {
                if (allUIList != null && allUIList.Count > 0)
                {
                    go = allUIList[allUIList.Count - 1];
                }
            }

            if (go != null && !go.activeSelf)
            {
                go.SetActive(true);
                UITools.UpdateUIPanleDepth(go, GetLayerPanelDepth());
                go.transform.localPosition = getPosition();
            }

        }

        /// <summary>
        /// 当前UI的层级，一层级的position的z轴间隔-500f，ui隐藏时，层级不增加。
        /// </summary>
        internal int currentUILayer
        {
            get
            {
                int layer = 0;
                /*
                for (int i = 0, length = allUIList.Count; i < length; i++)
                {
                    if (allUIList[i].activeSelf && allUIList[i].GetComponent<UIPanel>() != null && allUIList[i].GetComponent<UIPanel>().depth < UILayerPosition.UIMaxUIDepth)
                    {
                        int iLayer = allUIList[i].GetComponent<UIPanel>().depth / _layerPanelDepth;
                        if (iLayer > layer)
                        {
                            layer = iLayer;
                        }
                    }
                }
                */
                for (int i = 0, length = selfTrans.childCount; i < length; i++)
                {
                    Transform trans = selfTrans.GetChild(i);
                    if (trans.gameObject.activeSelf && trans.GetComponent<UIPanel>() != null)
                    {
                        int depth = trans.GetComponent<UIPanel>().depth;
                        if (depth >= 0 && depth < UILayerPosition.UIMaxUIDepth)
                        {
                            int iLayer = trans.GetComponent<UIPanel>().depth / _layerPanelDepth;
                            if (iLayer > layer)
                            {
                                layer = iLayer;
                            }
                        }

                    }
                }

                return layer + 1;
            }
        }
        private float layerSpace = -500f;
        public int _layerPanelDepth = 100;
        /// <summary>
        /// 获取UI的position
        /// </summary>
        public Vector3 getPosition()
        {
            return new Vector3(0, 0, layerSpace * currentUILayer);
        }

        /// <summary>
        /// 获取NGUI UIPanel Depth
        /// </summary>
        /// <returns></returns>
        public int GetLayerPanelDepth()
        {
            return (currentUILayer) * _layerPanelDepth;
        }


        /// <summary>
        /// 关闭UI
        /// </summary>
        public void CloseUI(GameObject uiGo, bool destroy = true)
        {

            Logger.LogError("这里要写关闭UI的逻辑");

        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        public void CloseUI(string szName)
        {
            szName += "(Clone)";
            GameObject uiObj = GetUIByName(szName);
            if (null == uiObj)
            {
                return;
            }

            CloseUI(uiObj);
        }

        void CloseUICallBack(GameObject uiGo)
        {

            if (uiGo == null)
            {
                if (allUIList.Count > 0)
                    uiGo = allUIList[allUIList.Count - 1];
            }
            if (uiGo == null)
            {
                ShowMainUI(true);
                return;
            }





            uiGo.SetActive(false);

            Logger.LogError("这里要添加检测内存是否满足的代码");
            if (!allUIList.Contains(uiGo))  //|| !MemoryCheck.Instance.CheckMemoryIsEnough(MemoryCheck.ECheckMemoryType.UI))
            {
                DestroyUI(uiGo);
            }
        }

        /// <summary>
        /// 清除UIList，不销毁GameObject
        /// </summary>
        /// <param name="uiGo"></param>
        public void CloseUIList(GameObject uiGo)
        {

            if (uiGo == null)
                return;

            if (allUIList.Contains(uiGo))
            {
                allUIList.Remove(uiGo);
            }
        }

        /// <summary>
        /// 获取不活跃的UI
        /// </summary>
        private GameObject FindInactiveUI()
        {
            for (int iIndex = 0; iIndex < allUIList.Count; ++iIndex)
            {
                if (!allUIList[iIndex].activeSelf)
                {
                    return allUIList[iIndex];
                }
            }

            return null;
        }

        /// <summary>
        /// 销毁UI
        /// </summary>
        public void DestroyUI(GameObject UIObj)
        {
            if (null == UIObj)
            {
                return;
            }

            if (allUIList.Contains(UIObj))
            {
                allUIList.Remove(UIObj);
            }



            NGUITools.Destroy(UIObj);
        }

        public void DestroyAllUI()
        {
            for (int i = allUIList.Count - 1; i >= 0; i--)
            {
                if (allUIList[i] != null)
                {
                    DestroyUI(allUIList[i]);
                }
            }

        }
        
        public GameObject AddChild(GameObject go)
        {
            if (go != null)
            {
                go.transform.parent = transform;
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
            }
            return go;
        }
    }
}


