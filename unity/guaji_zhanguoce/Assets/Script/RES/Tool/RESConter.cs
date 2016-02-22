    /// <summary>
    /// 资源基类
    /// </summary>
using System;
public class RESConter :IComparable<RESConter>
    {
        /// <summary>
        /// 资源路径
        /// </summary>
        public string path;
        /// <summary>
        /// 资源类型
        /// </summary>
        public string type;
        /// <summary>
        /// 关联其它资源的个数
        /// </summary>
        public int dependenciesLength;

        public int CompareTo(RESConter other)
        {
            if (this.dependenciesLength > other.dependenciesLength)
                return 1;//往后移
            else if (this.dependenciesLength == other.dependenciesLength)
                return 0;
            else
                return -1;//往前移
        }
    }

