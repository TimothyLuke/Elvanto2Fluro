using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elvanto2Fluro.Elvanto.GroupCollection
{


    public class GroupRootobject
    {
        public string generated_in { get; set; }
        public string status { get; set; }
        public Groups groups { get; set; }
    }

    public class Groups
    {
        public int on_this_page { get; set; }
        public int page { get; set; }
        public int per_page { get; set; }
        public int total { get; set; }
        public List<Group> group { get; set; }
    }

    public class Group
    {
        public string date_added { get; set; }
        public string date_modified { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string status { get; set; }
        public string meeting_address { get; set; }
        public string meeting_city { get; set; }
        public string meeting_state { get; set; }
        public string meeting_postcode { get; set; }
        public string meeting_country { get; set; }
        public string meeting_day { get; set; }
        public string meeting_time { get; set; }
        public string meeting_frequency { get; set; }
        public string picture { get; set; }
        public string id { get; set; }
        public object people { get; set; }
    }


}
