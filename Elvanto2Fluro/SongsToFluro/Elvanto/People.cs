using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SongsToFluro.Elvanto.People
{
    public class Person
    {
        public string date_added { get; set; }
        public string date_modified { get; set; }
        public string firstname { get; set; }
        public string preferred_name { get; set; }
        public string middle_name { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string mobile { get; set; }
        public string status { get; set; }
        public string username { get; set; }
        public string last_login { get; set; }
        public string country { get; set; }
        public string timezone { get; set; }
        public string picture { get; set; }
        public string family_relationship { get; set; }
        public string id { get; set; }
        public string category_id { get; set; }
        public int admin { get; set; }
        public int contact { get; set; }
        public int archived { get; set; }
        public int deceased { get; set; }
        public int volunteer { get; set; }
        public string family_id { get; set; }
        public object departments { get; set; }
        public string birthday { get; set; }
        public string gender { get; set; }
        public string home_address { get; set; }
        public string home_address2 { get; set; }
        public string home_city { get; set; }
        public string home_postcode { get; set; }
        public string home_state { get; set; }
        public string marital_status { get; set; }
        [JsonProperty("custom_95d1c84c-6196-11e5-9d36-06ba798128be")]
        public VotingConstruct votingMember { get; set; }
}

    public class VotingConstruct
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class People
    {
        public int on_this_page { get; set; }
        public int page { get; set; }
        public int per_page { get; set; }
        public int total { get; set; }
        public List<Person> person { get; set; }
    }

    public class RootObject
    {
        public string generated_in { get; set; }
        public string status { get; set; }
        public People people { get; set; }
    }
}
