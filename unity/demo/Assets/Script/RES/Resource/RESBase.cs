
/// <summary>
/// 资源核心封装数据类
/// </summary>
   public class RESBase
    {
       /// <summary>
       /// 资源唯一标签，资源相对路径不带后缀名
       /// </summary>
       public string key;

       /// <summary>
       /// 资源的类型,RESFormat中定义
       /// </summary>
       public string type;

       public RESBase() { }
       public RESBase(string key,string type)
       {
           this.key = key;
           this.type = type;
       }
       public override bool Equals(System.Object obj)
       {
           RESBase other = obj as RESBase;
           if (other == null)
           {
               return false;
           }

           return (other.GetHashCode() == GetHashCode());
       }

       public override int GetHashCode()
       {
           return (key+type).GetHashCode();
       }
    }

