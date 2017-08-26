using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elvanto2Fluro.Fluro.File
{



    public class RootObject
    {
        public string _id { get; set; }
        public string firstLine { get; set; }
        public string url { get; set; }
        public string slug { get; set; }
        public string definition { get; set; }
        public string title { get; set; }
        public int filesize { get; set; }
        public string mimetype { get; set; }
        public string _type { get; set; }
        public string filename { get; set; }
        public string extension { get; set; }
        public string filepath { get; set; }
        public string cloudPath { get; set; }
        public string assetType { get; set; }
        public Account account { get; set; }
        public int __v { get; set; }
        public List<string> tags { get; set; }
        public List<Realm> realms { get; set; }
        public string privacy { get; set; }
        public List<string> keywords { get; set; }
        public List<string> hashtags { get; set; }
        public string status { get; set; }
        public string updated { get; set; }
        public string created { get; set; }
        public List<object> managedOwners { get; set; }
        public List<object> owners { get; set; }
    }
}
