using System.Collections;
using System.Collections.Generic;
using UnityEngine;



    /// <summary>
    /// AssetBundle 引用资源管理器
    /// </summary>
    public class ABRecManage : MonoBehaviour
    {
        /*
        private static ABRecManage _instance;

        public static ABRecManage Instance
        {
            get{


             if (_instance == null && Application.isPlaying)
            {
                GameObject go = new GameObject("ABRecManage");
                go.AddComponent<DontDestroy>();
                _instance=go.AddComponent<ABRecManage>();
            }
            return _instance;
            }
        }
        */
        /// <summary>
        /// AssetBundle 引用资源字典
        /// </summary>
        private Dictionary<string, ABRec> _abRec;

        /// <summary>
        /// 判断资源是否引用了其它资源。true，引用了
        /// </summary>
        /// <param name="resKey"></param>
        /// <returns></returns>
        public bool RecExist(string resKey)
        {
            if (_abRec != null)
                return _abRec.ContainsKey(resKey);

            return false;
        }
        /// <summary>
        /// 获取 ABRec,必须先判断是否存在。RecExist
        /// </summary>
        /// <param name="resKey"></param>
        /// <returns></returns>
        public ABRec GetABRec(string resKey) 
        {
            if (_abRec != null && _abRec.ContainsKey(resKey))
                return _abRec[resKey];

            return null;
        }

        public delegate void ABRecComplete();

        private ABRecComplete _complete;
        /// <summary>
        /// assetbundle 所有引用资源数据初始化
        /// </summary>
        /// <param name="complete"></param>
        public void Init(ABRecComplete complete)
        {
            if (complete !=null)
            {
                _complete += complete;
            }

            if (_abRec == null)
            {
                _abRec = new Dictionary<string, ABRec>();
      
                StartCoroutine(LoadRecsYield());
            }

        }
        IEnumerator LoadRecsYield()
        {

            WWW www = new WWW(RESTool.LoadAbPathByPath("Rec",RESFormat.XML,RESManageType.ASSETBUNDLE));

            yield return www;
           
            if (string.IsNullOrEmpty(www.error))
            {
                AssetBundle ab = www.assetBundle;

                if (ab != null)
                {
                    foreach (var item in  ab.LoadAll())
                    {
                        InitData(System.Text.UTF8Encoding.UTF8.GetString((item as TextAsset).bytes));
                    }

                    if (_complete != null)
                    {
                        _complete();
                    }

                    yield return new WaitForSeconds(2f);

                    ab.Unload(false);

                    www = null;
                }
                else
                {
                    www = null;
                    Logger.LogError("ABRecManage Rex Assetbundle null");
                    if (_complete != null)
                    {
                        _complete();
                    }
                }
            }
            else
            {
                if (_complete != null)
                {
                    _complete();
                }
            }
           

           
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="xmlStr"></param>
        public void InitData(string xmlStr)
        {
            InitData(new XMLParser().Parse(xmlStr));
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="node"></param>
        public void InitData(XMLNode  node)
        {
            if (node == null)
            {
                return;
            }

            XMLNode infoNode = node.GetNode("root>0>info>0");
            
            XMLNodeList assetsNodes = node.GetNodeList("root>0>assets>0>asset");

            if (infoNode != null && assetsNodes != null)
            {

                ABRecInfo abInfo = new ABRecInfo(infoNode.GetValue("@resKey"), infoNode.GetValue("@resType"));

                List<ABRecAssets> abRecAssetsList = new List<ABRecAssets>();

                foreach (XMLNode item in assetsNodes)
                {
                    //abRecAssetsList.Add(new ABRecAssets(item.GetValue("@resPath"), item.GetValue("@resAbPath"), item.GetValue("@resType"), item.GetValue("@resName")));

                    abRecAssetsList.Add(new ABRecAssets(item.GetValue("@resPath"), item.GetValue("@resType"), item.GetValue("@resName")));
                }
                _abRec[abInfo.resKey] =  new ABRec(abInfo, abRecAssetsList);
            }
          
          
        }

    }

