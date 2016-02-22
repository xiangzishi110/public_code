using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools.Data
{
    public class StringDictionary : Dictionary<string, string>
    {
        public string GetValue(string key)
        {
            if (this.ContainsKey(key))
                return this[key];
            else
            {
                Logger.LogError("StringDictionary GetValue Null Not Key:" + key + ";");
                return "";
            }
              
        }
        public void  SetValue(string key,string value)
        {
            if (this.ContainsKey(key) == false)
                this.Add(key, value);
            else
                this[key] = value;
        }
    }
}

