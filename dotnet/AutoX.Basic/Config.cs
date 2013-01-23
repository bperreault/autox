// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Collections.Generic;

#endregion

namespace AutoX.Basic
{
    public class Config
    {
        
        string Id { get; set; }
        private readonly Dictionary<string, string> _variables = new Dictionary<string,string>();
        public Config(){
            Id = Guid.NewGuid().ToString();
            _variables.Add("_id",Id);
        }

        public void Set(string key, string value){
            if(_variables.ContainsKey(key))
                _variables[key] = value;
            else
                _variables.Add(key,value);
        }
        public string Get(string key){
            return _variables[key];
        }
        public Dictionary<string, string> GetList()
        {
            return _variables;
        } 

    }
}