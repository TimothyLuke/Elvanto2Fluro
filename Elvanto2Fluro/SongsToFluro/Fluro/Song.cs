using System;
using System.Collections.Generic;

namespace Fluro
{
    public class External
    {
    }

    public class SheetMusic
    {
        public string _id { get; set; }
        public string firstLine { get; set; }
        public string slug { get; set; }
        public string definition { get; set; }
        public string assetType { get; set; }
        public string title { get; set; }
        public string mimetype { get; set; }
        public string _type { get; set; }
        public string extension { get; set; }
        public int filesize { get; set; }
        public List<object> tags { get; set; }
        public List<string> realms { get; set; }
        public List<object> keywords { get; set; }
        public List<object> hashtags { get; set; }
        public string status { get; set; }
    }

    public class Data
    {
        public External external { get; set; }
        public List<object> videos { get; set; }
        public List<SheetMusic> sheetMusic { get; set; }
        public List<object> lyrics { get; set; }
        public string firstLine { get; set; }
        public string artist { get; set; }
        public string album { get; set; }
        public string ccli { get; set; }
        public string key { get; set; }
    }

    public class Account
    {
        public string _id { get; set; }
        public string title { get; set; }
    }

    public class Author
    {
        public string _id { get; set; }
        public string name { get; set; }
    }

    public class ManagedAuthor
    {
        public string _id { get; set; }
        public string title { get; set; }
        public string _type { get; set; }
    }

    public class Realm
    {
        public string _id { get; set; }
        public string title { get; set; }
        public string bgColor { get; set; }
        public string color { get; set; }
        public string _type { get; set; }
    }

    public class ManagedOwner
    {
        public string _id { get; set; }
        public string title { get; set; }
        public string _type { get; set; }
    }

    public class Owner
    {
        public string _id { get; set; }
        public string name { get; set; }
    }

    public class RootObject
    {
        public string _id { get; set; }
        public string firstLine { get; set; }
        public string slug { get; set; }
        public string definition { get; set; }
        public Data data { get; set; }
        public string title { get; set; }
        public Account account { get; set; }
        public string _type { get; set; }
        public Author author { get; set; }
        public ManagedAuthor managedAuthor { get; set; }
        public string updatedBy { get; set; }
        public List<object> tags { get; set; }
        public List<Realm> realms { get; set; }
        public string privacy { get; set; }
        public List<object> keywords { get; set; }
        public List<object> hashtags { get; set; }
        public string status { get; set; }
        public string updated { get; set; }
        public string created { get; set; }
        public List<ManagedOwner> managedOwners { get; set; }
        public List<Owner> owners { get; set; }
        public int __v { get; set; }
    }
}