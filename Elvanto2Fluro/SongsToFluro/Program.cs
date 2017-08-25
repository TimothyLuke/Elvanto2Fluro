
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using System.Text;
using System.Threading.Tasks;
using SongsToFluro.Elvanto;
using System.Threading;
using SongsToFluro.Fluro;
using System.Net.Http;
using System.IO;
using SongsToFluro.Fluro.Family;

namespace SongsToFluro
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static string ElvantoAPIKey = "4cwDPuZQO0x91sOz71VOOchajFl7Gg6A ";
        private static string ElvantoSongURI = "https://api.elvanto.com/v1/songs/getAll.json";
        private static string ElvantoSongArrangementURI = "https://api.elvanto.com/v1/songs/arrangements/getAll.json";
        private static string ElvantoSongIndividualArangementURI = "https://api.elvanto.com/v1/songs/arrangements/getInfo.json";
        private static string ElvantoKeysURI = "https://api.elvanto.com/v1/songs/keys/getAll.json";
        private static string ElvantoIndividualKeyURI = "https://api.elvanto.com/v1/songs/keys/getInfo.json";
        private static string ElvantoPeopleURI = "https://api.elvanto.com/v1/people/getAll.json";

        private static string FluroAPIKey = "$2a$10$jjHToxeGm1v.OdbxHy.NqOGm.wKfvaueG0g7pInRsGYy8DWNNutcO";
        private static string FluroSongURI = "https://apiv2.fluro.io/content/song";
        private static string FluroFileUploadURI = "https://api.fluro.io/file/upload";
        private static string FluroChordChartPostURI = "https://apiv2.fluro.io/content/sheetMusic";
        private static string FluroFamilyURI = "https://apiv2.fluro.io/content/family";
        private static string FluroContactURI = "https://apiv2.fluro.io/content/contact";

        private static string FluroCreativeRealm = "5923eaf4319df62ecc6f8005";
        private static string FluroRidgehavenRealm = "599cd5ef983a8a5948613a00";

        static void Main(string[] args)
        {

            JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(() => new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });


            //MigrateSongs();
            MigratePeople();

            Console.ReadLine();
        }

        static void MigratePeople()
        {
            WebClient client = new WebClient
            {
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(ElvantoAPIKey, "")
            };
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            string stringFullOfJson = client.UploadString(ElvantoPeopleURI, "{\"fields\": [\"gender\", \"birthday\", \"marital_status\", \"home_address\", \"home_address2\", \"home_city\", \"home_state\", \"home_postcode\", \"departments\", \"custom_95d1c84c-6196-11e5-9d36-06ba798128be\"] }");

            
            var rootobj = JsonConvert.DeserializeObject<Elvanto.People.RootObject>(stringFullOfJson);
            var groupedFamilyList = rootobj.people.person
                .GroupBy(u => u.family_id)
                .Select(grp => grp.ToList())
                .ToList();

            //TODO need to change the logic of this.  This creates the contacts then the families.  Need to create the family then create the contacts with the family link.

            foreach (var family in groupedFamilyList)
            {
                bool related = true;
                if (family.First().family_id == "")
                {
                    logger.Info("Group None - {0}",  family.First().lastname);
                    related = false;
                }
                else
                {
                    logger.Info("Group {0} - {1}", family.First().family_id, family.First().lastname);
                }
                foreach (var person in family)
                {
                    logger.Info("  {0} - {1} {2}", person.id, person.firstname, person.lastname);

                }
                AddPersonToFluro(family, related);
            }
        }

        static void AddPersonToFluro(List<Elvanto.People.Person> family, bool related)
        {
            Fluro.Realm realm = new Fluro.Realm
            {
                _id = FluroRidgehavenRealm
            };
            Fluro.Family.RootObject newfamily = new Fluro.Family.RootObject
            {
                items = new List<string>(),
                phoneNumbers = new List<string>(),
                emails = new List<string>(),
                address = new Address(),
                _type = "family",
                realms = new List<Realm>()
            };
            newfamily.realms.Add(realm);
            

            
            foreach( Elvanto.People.Person person in family){

                Fluro.Family.Contact contact = new Fluro.Family.Contact
                {
                    data = new Fluro.Family.ContactData(),
                    realms = new List<string>()
                };
                contact.realms.Add(realm._id);
                contact._type = "contact";

                contact.data.channel = "TimothyLuke Elvanto2Fluro";
                contact.data.importId = person.id;
                contact.title = person.firstname.ToLower() + " " + person.lastname.ToLower();
                contact.lastName = person.lastname.ToLower();
                contact.firstName = person.firstname.ToLower();
                contact.gender = person.gender;
                contact.dob = person.birthday;
                contact._type = "contact";
                if (person.archived > 0 )
                {
                    contact.status = "archived";
                } else
                {
                    contact.status = "active";
                }

                newfamily.items.Add(PerformFluroPersonUpload(contact));

                newfamily = UpdateFluroFamily(person, related, newfamily);
            }

            PerformFluroFamilyUpload(newfamily);

        }

        private static Fluro.Family.RootObject UpdateFluroFamily( Elvanto.People.Person person, bool related, Fluro.Family.RootObject family)
        {
            if (family.title == null )
            {
                family.title = person.lastname;
            }
            family.firstLine = family.firstLine + ", " + person.firstname;
            if (family.firstLine.Left(2) == ", ")
            {
                family.firstLine = family.firstLine.Substring(2);
            }
            family.emails.Add(person.email);
            if(family.address.addressLine1 == null)
            {
                family.address.addressLine1 = person.home_address;
                family.address.addressLine2 = person.home_address2;
                family.address.suburb = person.home_city;
                family.address.state = person.home_state;
                if (person.home_postcode != "" )
                { 
                  family.address.postalCode = Convert.ToInt32(person.home_postcode);
                }
                family.address.country = "Australia";
            }

            family.phoneNumbers.Add(person.mobile);
            family.phoneNumbers.Add(person.phone);
           



            if (!related)
            {
                PerformFluroFamilyUpload(family);

                family = new Fluro.Family.RootObject();


                Fluro.Realm realm = new Fluro.Realm
                {
                    _id = FluroRidgehavenRealm
                };
                family.realms = new List<Realm>
                {
                    realm
                };


                family.items = new List<string>();
                family.phoneNumbers = new List<string>();
                family.address = new Address();
                family.emails = new List<string>();

            }
            return family;
        }

        private static void PerformFluroFamilyUpload(Fluro.Family.RootObject family)
        {
            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.Authorization] = "Bearer " + FluroAPIKey;
            client.UseDefaultCredentials = true;
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            string preuploadJson = JsonConvert.SerializeObject(family);
            logger.Info(preuploadJson);
            string stringFullOfJson = client.UploadString(FluroFamilyURI, preuploadJson);

        }

        private static string PerformFluroPersonUpload(Fluro.Family.Contact contact)
        {
            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.Authorization] = "Bearer " + FluroAPIKey;
            client.UseDefaultCredentials = true;
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            string preuploadJson = JsonConvert.SerializeObject(contact);
            logger.Info(preuploadJson);
            string stringFullOfJson = client.UploadString(FluroContactURI, preuploadJson);


            return JsonConvert.DeserializeObject<Fluro.Family.Contact>(stringFullOfJson)._id;

        }

        static void MigrateSongs()
        {



            WebClient client = new WebClient
            {
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(ElvantoAPIKey, "")
            };
            string stringFullOfJson = client.DownloadString(ElvantoSongURI);

            Fluro.Realm realm = new Fluro.Realm
            {
                _id = FluroCreativeRealm
            };




            var rootobj = JsonConvert.DeserializeObject<Elvanto.RootObject>(stringFullOfJson);

            

            foreach (Song song in rootobj.songs.song) {
                logger.Info($"{song.id} {song.title}");
                Fluro.RootObject newsong = new Fluro.RootObject
                {

                    //behold id ad17eee6-3544-11e7-ba01-061a3b9c64af


                    title = song.title,
                    realms = new List<Fluro.Realm>()
                };
                newsong.realms.Add(realm);
                newsong.data = new Fluro.Data
                {
                    artist = song.artist,
                    album = song.album,
                    ccli = song.number
                };

                List<Elvanto.File> elvantofiles = new List<Elvanto.File>();

                Thread.Sleep(2000);
                using (WebClient newclient = new WebClient())
                {
                    newclient.Credentials = new NetworkCredential(ElvantoAPIKey, "");
                    newclient.UseDefaultCredentials = true;
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string arrangementresult = client.UploadString(ElvantoSongArrangementURI, "POST", "{\"song_id\": \"" + song.id + "\",    \"files\": true}");
                    var rootArrangement = JsonConvert.DeserializeObject<Elvanto.Arrangement.RootObject>(arrangementresult);

                    newsong.data.lyrics = new List<object>
                    {
                        rootArrangement.arrangements.arrangement.First().lyrics
                    };



                    try
                    {
                        JObject fileses = (JObject)rootArrangement.arrangements.arrangement.First().files;


                        if (fileses.HasValues)
                        {

                            // Individual call to get arrangement with files
                            foreach(Elvanto.Arrangement.Arrangement arrangement in rootArrangement.arrangements.arrangement)
                            {

                                ProcessIndividualArrangementFiles(arrangement.id, elvantofiles);
                            }

                            foreach (Elvanto.File flz in elvantofiles)
                            {
                                logger.Info($"{flz.title} {flz.content}");
                            }
                        }
                    } catch (InvalidCastException ex)
                    {

                    }
                    





                }

                AddSongToFluro(newsong, elvantofiles);

            }

            Console.ReadLine();
        }

        private static void AddSongToFluro(Fluro.RootObject newsong, List<Elvanto.File> files)
        {
            List<string> chordchartids = new List<string>();
            // add files 
            // first search to see if it already exists
            foreach (Elvanto.File file in files) {
                
                using (WebClient client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    client.Headers[HttpRequestHeader.Authorization] = "Bearer " + FluroAPIKey;
                    string searchstring = FluroChordChartPostURI + "/search/" + Uri.EscapeUriString(file.title).Replace("/"," ");

                    string searchresult = client.DownloadString(searchstring);
                    if (searchresult.Length >= 3)
                    {
                        //found
                        try
                        {
                            var foundobjects = JsonConvert.DeserializeObject<SongsToFluro.Fluro.SheetMusicSearch.Rootobject>(searchresult);
                            foreach (SongsToFluro.Fluro.SheetMusicSearch.Class1 sheet in foundobjects.Property1)
                            {
                                chordchartids.Add(sheet._id);
                            }
                        } catch (Exception ex)
                        {
                            logger.Info("Could not process \n" + searchresult);
                        }
                    } else
                    {
                        
                        // add new sheet
                        //download existing file
                        HttpClient dlclient = new HttpClient();
                        
                        HttpResponseMessage response = new HttpResponseMessage();

                        response = dlclient.GetAsync(file.content).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            //response.Content.ReadAsStringAsync().Result.Replace("\"", string.Empty);
                            //mybytearray = Convert.FromBase64String(result);
                            byte[] downloadedfile = response.Content.ReadAsByteArrayAsync().Result;
                            string filetype = "";
                            filetype = response.Content.Headers.ContentType.ToString();
                            


                            Dictionary<string, object> postParameters = new Dictionary<string, object>();
                            SheetMusic sheet = new SheetMusic
                            {
                                title = file.title.Replace("/", " "),
                                realms = new List<string>()
                            };
                            sheet.realms.Add(FluroCreativeRealm);
                            sheet.definition = "sheetMusic";
                            string jsonsheet = JsonConvert.SerializeObject(sheet);

                            postParameters.Add("json", jsonsheet);
                            postParameters.Add("?returnPopulated", true);
                            postParameters.Add("file", new FormUpload.FileParameter(downloadedfile, FormUpload.GetFileName(file.content), filetype));
                            string userAgent = "Timothy's C# Migrator";
                            HttpWebResponse webResponse = FormUpload.MultipartFormDataPost(FluroFileUploadURI, userAgent, postParameters, FluroAPIKey);

                            StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
                            string fullResponse = responseReader.ReadToEnd();
                            logger.Debug(fullResponse);
                            webResponse.Close();
                            //need to parse the result and add it to the array
                            Fluro.File.RootObject returnedfile = JsonConvert.DeserializeObject<Fluro.File.RootObject>(fullResponse);
                            chordchartids.Add(returnedfile._id);
                        }

                    }
                }
            }

            //Add SheetMusic to Song
            newsong.data.sheetMusic = new List<SheetMusic>();
            foreach (string sheetid in chordchartids)
            {
                SheetMusic sheet = new SheetMusic
                {
                    _id = sheetid
                };
                newsong.data.sheetMusic.Add(sheet);
            }

            //now check for the Song
            using (WebClient client = new WebClient())
            {
                newsong.title = newsong.title.Replace("/", " ");
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + FluroAPIKey;
                string searchstring = FluroSongURI + "/search/" + Uri.EscapeUriString(newsong.title);
                string searchresult = client.DownloadString(searchstring);
                if (searchresult.Length >= 3)
                {
                    //found
                    logger.Info("Song Found skipping.");
                }
                else
                {
                    newsong.realms = new List<Realm>();
                    Realm creative = new Realm
                    {
                        _id = FluroCreativeRealm
                    };
                    newsong.realms.Add(creative);
                    newsong.definition = "song";
                    string upjson = JsonConvert.SerializeObject(newsong);
                    logger.Info(upjson);
                    using (WebClient upclient = new WebClient())
                    {
                        upclient.Headers[HttpRequestHeader.ContentType] = "application/json";
                        upclient.Headers[HttpRequestHeader.Authorization] = "Bearer " + FluroAPIKey;

                        upclient.UploadString(FluroSongURI, "POST", upjson);
                    }
                }
            }

        }



        private static void ProcessIndividualArrangementFiles(string id, List<Elvanto.File> files)
        {
            Thread.Sleep(2000);
            using (WebClient piuclient = new WebClient())
            {
                piuclient.UseDefaultCredentials = true;
                piuclient.Credentials = new NetworkCredential(ElvantoAPIKey, "");
                piuclient.Headers[HttpRequestHeader.ContentType] = "application/json";
                logger.Info($"Getting Arrangement {id}");
                string poststring = "{\"id\": \"" + id + "\",    \"files\": true}";
                string arrangementresult = piuclient.UploadString(ElvantoSongIndividualArangementURI, "POST", poststring);

                var rootArrangement = JsonConvert.DeserializeObject<Elvanto.IndividualArrangement.RootObject>(arrangementresult);
                foreach (Elvanto.IndividualArrangement.Arrangement arr in rootArrangement.arrangement)
                {
                    foreach (Elvanto.File file in arr.files.file)
                    {
                        logger.Info($"Adding File {file.content}");
                        files.Add(file);

                        ProcessKeyFiles(id, files);
                    }

                }

            }
        }

        private static void ProcessKeyFiles(string id, List<Elvanto.File> files)
        {
            Thread.Sleep(2000);
            using (WebClient client = new WebClient())
            {
                
                
                client.UseDefaultCredentials = true;
                client.Credentials = new NetworkCredential(ElvantoAPIKey, "");
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                logger.Info($"Getting Key Files for Arrangement {id}");
                string poststring = "{\"arrangement_id\": \"" + id + "\",    \"files\": true}";
                string keyresult = client.UploadString(ElvantoKeysURI, "POST", poststring);

                var rootkey = JsonConvert.DeserializeObject<Elvanto.Key.RootObject>(keyresult);

                foreach (Elvanto.Key.Key arr in rootkey.keys.key)
                {
                    try
                    {
                        JObject fileses = (JObject)arr.files;


                        if (fileses.HasValues)
                        {
                            ProcessIndividualKeyFiles(arr.id, files);

                            
                        }
                    } catch (InvalidCastException ex)
                    {

                    }

                }

            }
        }

        private static void ProcessIndividualKeyFiles(string id, List<Elvanto.File> files)
        {
            Thread.Sleep(2000);
            using (WebClient client = new WebClient())
            {
                client.UseDefaultCredentials = true;
                client.Credentials = new NetworkCredential(ElvantoAPIKey, "");
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                logger.Info($"Getting Key Files for Arrangement {id}");
                string poststring = "{\"id\": \"" + id + "\",    \"files\": true}";
                string keyresult = client.UploadString(ElvantoIndividualKeyURI, "POST", poststring);

                var rootkey = JsonConvert.DeserializeObject<Elvanto.IndividualKey.RootObject>(keyresult);

                foreach (Elvanto.IndividualKey.Key keys in rootkey.key)
                {
                    foreach (Elvanto.File file in keys.files.file)
                    {
                        logger.Info($"Adding File {file.content}");
                        files.Add(file);
                    }
                }
            }
        }
    }
}
