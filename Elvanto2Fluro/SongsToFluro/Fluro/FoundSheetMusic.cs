using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SongsToFluro.Fluro.SheetMusicSearch
{
 
        public class Rootobject
        {
            public List<Class1> Property1 { get; set; }
        }

        public class Class1
        {
            public string _id { get; set; }
            public string firstLine { get; set; }
            public string slug { get; set; }
            public string definition { get; set; }
            public string title { get; set; }
            public string account { get; set; }
            public string _type { get; set; }
            public Author author { get; set; }
            public Managedauthor managedAuthor { get; set; }
            public List<string> tags { get; set; }
            public List<Realm> realms { get; set; }
            public List<string> keywords { get; set; }
            public List<string> hashtags { get; set; }
            public string status { get; set; }
            public List<Managedowner> managedOwners { get; set; }
            public List<Owner> owners { get; set; }

            
         }

        public class Author
        {
            public string _id { get; set; }
            public string name { get; set; }
        }

        public class Managedauthor
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

        public class Managedowner
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


}
