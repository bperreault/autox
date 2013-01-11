// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

#endregion

namespace AutoX.Basic
{
    public class Config
    {
        string Id {get;}
        private readonly Dictionary<string, string> variables = new Dictionary<string,string>();
        public Config(){
            Id = Guid.NewGuid().ToString();
        }

        public void Set(string key, string value){
            if(variables.Contains(key))
                variables[key] = value;
            else
                variables.Add(key,value);
        }
        public string Get(string key){
            return variables[key];
        }
    }
}