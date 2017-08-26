using System;
using System.Collections.Generic;

namespace Elvanto2Fluro.Elvanto
{
    public class Location
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Locations
    {
        public List<Location> location { get; set; }
    }

    public class Song
    {
        public string id { get; set; }
        public string status { get; set; }
        public string date_added { get; set; }
        public string date_modified { get; set; }
        public string title { get; set; }
        public string permalink { get; set; }
        public string number { get; set; }
        public int item { get; set; }
        public int learn { get; set; }
        public int allow_downloads { get; set; }
        public string artist { get; set; }
        public string album { get; set; }
        public string notes { get; set; }
        public object categories { get; set; }
        public Locations locations { get; set; }
        public object files { get; set; }
    }

    public class Songs
    {
        public int on_this_page { get; set; }
        public int page { get; set; }
        public int per_page { get; set; }
        public int total { get; set; }
        public List<Song> song { get; set; }
    }

    public class RootObject
    {
        public string generated_in { get; set; }
        public string status { get; set; }
        public Songs songs { get; set; }
    }

    public class SongWithFiles
    {
        public string id { get; set; }
        public string status { get; set; }
        public string date_added { get; set; }
        public string date_modified { get; set; }
        public string title { get; set; }
        public string permalink { get; set; }
        public string number { get; set; }
        public int item { get; set; }
        public int learn { get; set; }
        public int allow_downloads { get; set; }
        public string artist { get; set; }
        public string album { get; set; }
        public string notes { get; set; }
        public object categories { get; set; }
        public Locations locations { get; set; }
        public Files files { get; set; }
    }
}