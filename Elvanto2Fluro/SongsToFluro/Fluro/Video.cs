using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elvanto2Fluro.Fluro
{

    public class Video
    {
        public string _id { get; set; }
        public string firstLine { get; set; }
        public string slug { get; set; }
        public string assetType { get; set; }
        public External external { get; set; }
        public string title { get; set; }
        public Account account { get; set; }
        public string _type { get; set; }
        public Author author { get; set; }
        public Managedauthor managedAuthor { get; set; }
        public string updatedBy { get; set; }
        public object poster { get; set; }
        public object[] tags { get; set; }
        public Realm[] realms { get; set; }
        public string privacy { get; set; }
        public object[] keywords { get; set; }
        public object[] hashtags { get; set; }
        public string status { get; set; }
        public DateTime updated { get; set; }
        public DateTime created { get; set; }
        public Managedowner[] managedOwners { get; set; }
        public Owner[] owners { get; set; }
        public int __v { get; set; }
    }

    public class External
    {
        public string youtube { get; set; }
    }

  

    public class Managedauthor
    {
        public string _id { get; set; }
        public string title { get; set; }
        public string _type { get; set; }
    }

  
    public class Managedowner
    {
        public string _id { get; set; }
        public string title { get; set; }
        public string _type { get; set; }
    }
    
}
