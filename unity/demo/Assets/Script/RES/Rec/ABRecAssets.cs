


    /// <summary>
    /// 引用其它资源封装类
    /// </summary>
    public class ABRecAssets
    {
        /// <summary>
        /// 资源标签，相对路径 
        /// 例如：Prefab/UI/RESLoadProgressUI_prefab
        /// </summary>
        public string resPath;

        /// <summary>
        /// 资源加载相对路径
        /// 例如：Prefab/UI/RESLoadProgressUI_prefab.assetbundle
        /// </summary>
        //public string resAbPath;

        /// <summary>
        /// 资源类型
        /// 例如：prefab
        /// </summary>
        public string resType;
        /// <summary>
        /// 资源类型
        /// 例如：RESLoadProgressUI
        /// </summary>
        public string resName;

        public ABRecAssets() { }

        /*
        public ABRecAssets(string resPath, string resAbPath, string resType, string resName)
        {
            this.resPath = resPath;
            this.resAbPath = resAbPath;
            this.resType = resType;
            this.resName = resName;
        }
         * */
        public ABRecAssets(string resPath, string resType, string resName)
        {
            this.resPath = resPath;
            this.resType = resType;
            this.resName = resName;
        }
    }

