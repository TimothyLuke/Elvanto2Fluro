using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elvanto2Fluro.Elvanto.IndividualKey
{


    public class Key
    {
        public string id { get; set; }
        public string date_added { get; set; }
        public string date_modified { get; set; }
        public string name { get; set; }
        public string key_starting { get; set; }
        public string key_ending { get; set; }
        public string arrangement_id { get; set; }
        public Files files { get; set; }
    }


    public class RootObject
    {
        public string generated_in { get; set; }
        public string status { get; set; }
        public List<Key> key { get; set; }
    }
}
