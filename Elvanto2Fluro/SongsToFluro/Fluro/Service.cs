using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elvanto2Fluro.Fluro
{

    public class Rootobject
    {
        public List<Service> Property1 { get; set; }
    }

    public class Service
    {
        public string _id { get; set; }
        public string endDate { get; set; }
        public string slug { get; set; }
        public string title { get; set; }
        public string _type { get; set; }
        public Track track { get; set; }
        public Account account { get; set; }
        public List<object> forms { get; set; }
        public List<object> assets { get; set; }
        public List<object> videos { get; set; }
        public List<object> images { get; set; }
        public List<Plan> plans { get; set; }
        public List<string> tags { get; set; }
        public List<string> realms { get; set; }
        public List<string> volunteers { get; set; }
        public List<string> rooms { get; set; }
        public List<string> assignmentSlots { get; set; }
        public List<string> schedule { get; set; }
        public List<string> locations { get; set; }
        public string startDate { get; set; }
        public string privacy { get; set; }
        public List<string> hashtags { get; set; }
        public string status { get; set; }
        public DateTime updated { get; set; }
        public DateTime created { get; set; }
        public List<string> managedOwners { get; set; }
        public List<string> owners { get; set; }
        public int __v { get; set; }
        public string definition { get; set; }
        public List<string> keywords { get; set; }
        public List<string> reminders { get; set; }
        public Checkindata checkinData { get; set; }
        public Data data { get; set; }
        public object mainImage { get; set; }
        public string updatedBy { get; set; }
    }

    public class Track
    {
        public string _id { get; set; }
        public string slug { get; set; }
        public string title { get; set; }
        public string _type { get; set; }
        public List<string> realms { get; set; }
        public List<string> hashtags { get; set; }
        public string status { get; set; }
        public List<string> keywords { get; set; }
    }


    public class Checkindata
    {
        public int checkinEndOffset { get; set; }
        public int checkinStartOffset { get; set; }
    }



    public class Plan
    {
        public string _id { get; set; }
        public string title { get; set; }
        public string _type { get; set; }
        public List<string> realms { get; set; }
        public List<string> teams { get; set; }
        public List<string> schedules { get; set; }
        public DateTime startDate { get; set; }
        public string status { get; set; }
    }

    public class Schedule
    {
        public string detail { get; set; }
        public string title { get; set; }
        public int duration { get; set; }
    }



}
