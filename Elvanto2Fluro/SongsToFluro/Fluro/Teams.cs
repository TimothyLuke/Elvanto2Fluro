using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elvanto2Fluro.Fluro
{

    public class Team
    {
        public string _id { get; set; }
        public string slug { get; set; }
        public string title { get; set; }
        public bool allowProvisional { get; set; }
        public Account account { get; set; }
        public string _type { get; set; }
        public Author author { get; set; }
        public string updatedBy { get; set; }
        public List<object> tags { get; set; }
        public List<Realm> realms { get; set; }
        public List<Provisionalmember> provisionalMembers { get; set; }
        public List<object> assignments { get; set; }
        public string privacy { get; set; }
        public List<string> hashtags { get; set; }
        public string status { get; set; }
        public DateTime updated { get; set; }
        public DateTime created { get; set; }
        public List<object> managedOwners { get; set; }
        public List<Owner> owners { get; set; }
        public int __v { get; set; }
        public TeamData data { get; set; }
        public object track { get; set; }
    }



    public class Provisionalmember
    {
        public string _id { get; set; }
        public string title { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string _type { get; set; }
    }

    public class TeamData
    {
        public string meeting_address { get; set; }
        public string meeting_city { get; set; }
        public string meeting_state { get; set; }
        public string meeting_postcode { get; set; }
        public string meeting_country { get; set; }
        public string meeting_day { get; set; }
        public string meeting_time { get; set; }
        public string meeting_frequency { get; set; }
        public string importId { get; set; }
    }
}
