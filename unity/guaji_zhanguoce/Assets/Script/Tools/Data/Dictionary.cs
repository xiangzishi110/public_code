using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Tools.Data
{
    public class Dictionary : Dictionary<object,object>
    {
        public bool Exits(object key)
        {
            return this.ContainsKey(key);
        }
        public object GetValue(object key) 
        {
            if (this.ContainsKey(key))
                return this[key];
            else return null;
        }
    }
}

