using System.Collections.Generic;

namespace ncache.models{
    public class HashModel
    {
        public HashModel()
        {
            key = string.Empty;
            fields = new Dictionary<string, string>();
        }

        public string key {get;set;}
        
        public Dictionary<string, string> fields {get; set; }
    }
}