using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SongsToFluro.Elvanto.Arrangement
{
    public class Arrangement
    {
        public string id { get; set; }
        public string date_added { get; set; }
        public string date_modified { get; set; }
        public string name { get; set; }
        public string copyright { get; set; }
        public List<string> sequence { get; set; }
        public string minutes { get; set; }
        public string seconds { get; set; }
        public string bpm { get; set; }
        public string key_male { get; set; }
        public string key_female { get; set; }
        public string lyrics { get; set; }
        public string chord_chart_key { get; set; }
        public string chord_chart { get; set; }
        [JsonProperty("files", NullValueHandling = NullValueHandling.Ignore)]
        public Object files { get; set; }
    }

    public class Arrangements
    {
        public int on_this_page { get; set; }
        public int page { get; set; }
        public int per_page { get; set; }
        public int total { get; set; }
        public List<Arrangement> arrangement { get; set; }
    }

    public class RootObject
    {
        public string generated_in { get; set; }
        public string status { get; set; }
        public Arrangements arrangements { get; set; }
    }
}
