using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elvanto2Fluro.Fluro.Family
{


    public class Contact
    {
        public string _id { get; set; }
        public string title { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string gender { get; set; }
        public string dob { get; set; }
        public string _type { get; set; }
        public List<string> tags { get; set; }
        public List<string> realms { get; set; }
        public List<string> hashtags { get; set; }
        public string status { get; set; }
        public List<string> keywords { get; set; }
        public string householdRole { get; set; }
        public ContactData data { get; set; }
        public string family { get; set; }
        public List<string> phoneNumbers { get; set; }
        public List<string> emails { get; set; }
        public string maritalStatus { get; set; }
    }

    public class Address
    {
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string suburb { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public int postalCode { get; set; }
    }


    public class RootObject
    {
        public string _id { get; set; }
        public string firstLine { get; set; }
        public string title { get; set; }
        public Account account { get; set; }
        public string _type { get; set; }
        public Author author { get; set; }
        public ManagedAuthor managedAuthor { get; set; }
        public string updatedBy { get; set; }
        public List<object> distinctFrom { get; set; }
        public List<string> items { get; set; }
        public bool samePostal { get; set; }
        public Address address { get; set; }
        public List<string> emails { get; set; }
        public List<string> phoneNumbers { get; set; }
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

    public class ContactData
    {
        public string importId { get; set; }
        public string channel { get; set; }
        public string preferredname { get; set; }
        public bool volunteer { get; set; }
        public string photoURL { get; set; }
        public bool manualintervention { get; set; }
    }

    
}
