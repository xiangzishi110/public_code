using System.Collections.Generic;

namespace Tools.Data
{
    /// <summary>
    /// 字典主键集合，主键只能是基本数据类型
    /// </summary>
    public class DicKeyObjects
    {
        /// <summary>
        /// 主键集合
        /// </summary>
        public List<KeyObject> keys=new List<KeyObject> ();

        public DicKeyObjects() { }

        public DicKeyObjects(params object[] keyObs) 
        {
            if (keys == null)
                keys = new List<KeyObject>();
            foreach (var ob in keyObs)
	        {
                keys.Add(new KeyObject(ob));
	        }
        }
        public override int GetHashCode()
        {
            int hashCode=0;
            for (int i = 0,length=keys.Count; i < length; i++)
            {
                hashCode += keys[i].GetHashCode();
            }
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is DicKeyObjects)
            {
                try
                {
                    DicKeyObjects dicKey = obj as DicKeyObjects;
                    for (int i = 0, length = keys.Count; i < length; i++)
                    {
                        if (!keys[i].Equals(dicKey.keys[i]))
                            return false;
                    }
                }
                catch (System.Exception)
                {

                    return false;
                }
             
                return true;
            }
            return false;
        }
    }
    public class KeyObject :System.Object
    {
        public object ob;

        public KeyObject()
        {
        }
        public KeyObject(object ob)
        {
            this.ob = ob;
        }
       public override bool Equals(object obj) 
        {
            return (this.GetHashCode() == obj.GetHashCode());
        }
       public override int GetHashCode()
        {
            return ob.GetHashCode();
        }
    }
}
