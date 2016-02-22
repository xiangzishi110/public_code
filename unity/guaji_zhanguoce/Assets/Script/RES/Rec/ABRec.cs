using System.Collections.Generic;

    /// <summary>
    /// AB本身即引用资源封装类
    /// </summary>
    public class ABRec
    {
        /// <summary>
        /// 本身资源
        /// </summary>
        public ABRecInfo abInfo ;
        /// <summary>
        /// 引用其它资源,包含本身资源。加载走此接口
        /// </summary>
        public List<ABRecAssets> abInfoRecAssets;

        /// <summary>
        /// 引用其它资源
        /// </summary>
        private List<string> abRecAssets;

        /// <summary>
        /// 引用其它资源,不包含本身资源。加载走此接口
        /// </summary>
        public List<ABRecAssets> GetRecAssets
        {
            get {
                if (abInfoRecAssets != null && abInfoRecAssets.Count > 1)
                {
                    return abInfoRecAssets.GetRange(0, abInfoRecAssets.Count - 1);
                }
                else
                {
                    return new List<ABRecAssets>();
                }
            }
        }

        public ABRec() 
        {
            abInfo = new ABRecInfo();
            abInfoRecAssets = new List<ABRecAssets>();
        }
        public ABRec(ABRecInfo  abinfo,List<ABRecAssets>  abInfoRecAssets)
        {
            this.abInfo = abinfo;
            this.abInfoRecAssets = abInfoRecAssets;
        }
    }

