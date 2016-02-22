


    /// <summary>
    ///AB本身资源,引用了其它资源的
    /// </summary>
    public class ABRecInfo
    {
        /// <summary>
        /// 资源相对路径 
        /// 例如：Prefab/UI/RESLoadProgressUI
        /// </summary>
        public string resKey;
        /// <summary>
        ///资源类型
        /// 例如：prefab
        /// </summary>
        public string resType;

        public ABRecInfo() { }

        public ABRecInfo(string resKey, string resType)
        {
            this.resKey = resKey;
            this.resType = resType;
        }
    }

