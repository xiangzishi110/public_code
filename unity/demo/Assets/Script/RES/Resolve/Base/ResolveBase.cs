using System;


    /// <summary>
    /// 资源解析数据封装类
    /// </summary>
   public  class ResolveBase
    {
       /// <summary>
       /// 资源名
       /// </summary>
       public string resName="";
       /// <summary>
       /// 资源类型
       /// </summary>
       public string resType="";

       public  ResolveBase(string resName,string resType)
       {
           this.resName = resName;
           this.resType = resType;
       }
    }

