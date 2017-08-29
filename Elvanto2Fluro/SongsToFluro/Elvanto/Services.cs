using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elvanto2Fluro.Elvanto
{

    public class ServicesRootobject
    {
        public string generated_in { get; set; }
        public string status { get; set; }
        public Services services { get; set; }
    }

    public class Services
    {
        public int on_this_page { get; set; }
        public int page { get; set; }
        public int per_page { get; set; }
        public int total { get; set; }
        public List<Service> service { get; set; }
    }

    public class Service
    {
        public string id { get; set; }
        public int status { get; set; }
        public string date_added { get; set; }
        public string date_modified { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public string date { get; set; }
        public string series_name { get; set; }
        public Service_Type service_type { get; set; }
        public Location location { get; set; }
        public Service_Times service_times { get; set; }
        public object plans { get; set; }
        public object volunteers { get; set; }
        public object notes { get; set; }
        public object files { get; set; }
        public object songs { get; set; }
    }

    public class Service_Type
    {
        public string id { get; set; }
        public string name { get; set; }
    }


    public class Service_Times
    {
        public List<Service_Time> service_time { get; set; }
    }

    public class Service_Time
    {
        public string id { get; set; }
        public string date_added { get; set; }
        public string date_modified { get; set; }
        public string name { get; set; }
        public string starts { get; set; }
        public string ends { get; set; }
    }

}
