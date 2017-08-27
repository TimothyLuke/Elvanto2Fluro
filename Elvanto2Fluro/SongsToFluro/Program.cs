
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using System.Text;
using System.Threading.Tasks;
using Elvanto2Fluro.Elvanto;
using System.Threading;
using Elvanto2Fluro.Fluro;
using System.Net.Http;
using System.IO;
using Elvanto2Fluro.Fluro.Family;
using System.Text.RegularExpressions;
using Elvanto2Fluro.Elvanto.GroupCollection;

namespace Elvanto2Fluro
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static string ElvantoAPIKey = "4cwDPuZQO0x91sOz71VOOchajFl7Gg6A ";
        private static string ElvantoSongURI = "https://api.elvanto.com/v1/songs/getAll.json";
        private static string ElvantoSongArrangementURI = "https://api.elvanto.com/v1/songs/arrangements/getAll.json";
        private static string ElvantoGroupURI = "https://api.elvanto.com/v1/groups/getAll.json";
        private static string ElvantoSongIndividualArangementURI = "https://api.elvanto.com/v1/songs/arrangements/getInfo.json";
        private static string ElvantoKeysURI = "https://api.elvanto.com/v1/songs/keys/getAll.json";
        private static string ElvantoIndividualKeyURI = "https://api.elvanto.com/v1/songs/keys/getInfo.json";
        private static string ElvantoPeopleURI = "https://api.elvanto.com/v1/people/getAll.json";
        private static string ElvantoMemberCategory = "a9802b8c-2e1b-11e2-9039-ef9e4c9f3a46";
        


        private static string FluroAPIKey = "$2a$10$jjHToxeGm1v.OdbxHy.NqOGm.wKfvaueG0g7pInRsGYy8DWNNutcO";
        private static string FluroSongURI = "https://apiv2.fluro.io/content/song";
        private static string FluroFileUploadURI = "https://api.fluro.io/file/upload";
        private static string FluroChordChartPostURI = "https://apiv2.fluro.io/content/sheetMusic";
        private static string FluroFamilyURI = "https://apiv2.fluro.io/content/family";
        private static string FluroContactURI = "https://apiv2.fluro.io/content/contact";
        private static string FluroVideoURI = "https://apiv2.fluro.io/content/video";
        private static string FluroTeamURI = "https://apiv2.fluro.io/content/team";
        private static string FluroTeamJoinURI = "https://apiv2.fluro.io/teams/{0}/join";
        private static string FluroOldMemberIdQuery = "https://apiv2.fluro.io/content/_query/59a135a9e64e6d71468b8bb2?noCache=true&variables[elvantoId]=";

        private static string FluroContentErrorTag = "59a136abe64e6d71468b90a0";
        private static string FluroVotingMember = "5936300a95402155f8a80346";
        private static string FluroChurchMember = "5936300005bf991296dabfa5";

        private static string FluroCreativeRealm = "5923eaf4319df62ecc6f8005";
        private static string FluroRidgehavenRealm = "599cd5ef983a8a5948613a00";

        static void Main(string[] args)
        {

            JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(() => new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            //logger.Info("==================================================Starting Songs=================================================");
            //MigrateSongs();
            //logger.Info("=====================================================End Songs===================================================");
            //logger.Info("=================================================Starting People=================================================");
            //MigratePeople();
            //logger.Info("===================================================End People====================================================");
            logger.Info("=================================================Starting Groups=================================================");
            MigrateGroups();
            logger.Info("===================================================End Groups====================================================");

            Console.ReadLine();
        }

        static void MigrateGroups()
        {
            // Get Groups
            using (WebClient client = new WebClient())
            {
                client.UseDefaultCredentials = true;
                client.Credentials = new NetworkCredential(ElvantoAPIKey, "");
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                string arrangementresult = client.UploadString(ElvantoGroupURI, "POST", "{\"fields\":[\"people\"]}");
                var rootArrangement = JsonConvert.DeserializeObject<Elvanto.GroupCollection.GroupRootobject>(arrangementresult);
                foreach(Elvanto.GroupCollection.Group group in rootArrangement.groups.group)
                {
                    AddGroupToFluro(group);
                }
            }
        }

        private static void AddGroupToFluro(Elvanto.GroupCollection.Group group)
        {
            Team team = new Team();
            team.title = group.name;
            team.allowProvisional = true;
            team.data = new TeamData();
            team.data.importId = group.id;
            team.data.meeting_address = group.meeting_address;
            team.data.meeting_city = group.meeting_city;
            team.data.meeting_country = group.meeting_country;
            team.data.meeting_day = group.meeting_day;
            team.data.meeting_frequency = group.meeting_frequency;
            team.data.meeting_postcode = group.meeting_postcode;
            team.data.meeting_state = group.meeting_state;
            team.data.meeting_time = group.meeting_time;
            team.realms = new List<Realm>();
            Realm realm = new Realm();
            realm._id = FluroRidgehavenRealm;
            team.realms.Add(realm);

            string teamId = PerformFluroTeamUpload(team);
            string peoplestring = group.people.ToString();
            logger.Debug(peoplestring);
            if (peoplestring.Length > 2)
            {
                Elvanto.People.People people = JsonConvert.DeserializeObject<Elvanto.People.People>(peoplestring);
                foreach (var person in people.person)
                {
                    AddPersonToFluroGroup(person.id, teamId);
                }
            }

            if (group.status != "Active") {

                PerformFluroTeamArchive(teamId);
            }

        }

        private static void PerformFluroTeamArchive(string id)
        {
            if (id != null)
            {


                WebClient client = new WebClient
                {
                    UseDefaultCredentials = true,

                };
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + FluroAPIKey;
                
                string jsontoupload = $"{{ \"status\":\"archived\"}}";

                logger.Debug(jsontoupload);
                string stringFullOfJson = client.UploadString(FluroTeamURI + "/" + id, "PUT", jsontoupload);
            }
        }

        private static void AddPersonToFluroGroup(string id, string teamId)
        {
            WebClient client = new WebClient
            {
                UseDefaultCredentials = true,

            };
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            client.Headers[HttpRequestHeader.Authorization] = "Bearer " + FluroAPIKey;
            
            string stringFullOfJson = client.DownloadString(FluroOldMemberIdQuery + id);
            string returnId = GetFirstInstance<string>("_id", stringFullOfJson);

            PerformFluroTeamJoin(returnId, teamId);
        }

        private static void PerformFluroTeamJoin(string id, string teamId)
        {
            if (id != null)
            {


                WebClient client = new WebClient
                {
                    UseDefaultCredentials = true,

                };
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + FluroAPIKey;
                string jsontoupload = $"{{ \"_id\":\"{id}\"}}";

                logger.Debug(jsontoupload);
                string stringFullOfJson = client.UploadString(String.Format(FluroTeamJoinURI, teamId), jsontoupload);
            }
        }

        private static string PerformFluroTeamUpload(Team team)
        {
            WebClient client = new WebClient
            {
                UseDefaultCredentials = true,
                
            };
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            client.Headers[HttpRequestHeader.Authorization] = "Bearer " + FluroAPIKey;
            string jsontoupload = JsonConvert.SerializeObject(team);
            logger.Debug(jsontoupload);
            string stringFullOfJson = client.UploadString(FluroTeamURI, jsontoupload);

            string returnId = GetFirstInstance<string>("_id", stringFullOfJson);
            return returnId;
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

            
            foreach (var family in groupedFamilyList)
            {

                string familyId = "";
                if (family.First().family_id == "")
                {
                    logger.Info("Group None - {0}",  family.First().lastname);
                    
                    familyId = "";
                }
                else
                {
                    logger.Info("Group {0} - {1}", family.First().family_id, family.First().lastname);
                    if (familyId == "") {
                        familyId = AddFamilyToFluro(family.First());
                    }
                }
                foreach (var person in family)
                {
                    logger.Info("  {0} - {1} {2}", person.id, person.firstname, person.lastname);
                    AddContactToFluro(person, familyId);

                }

                
            }
        }

        static void AddContactToFluro(Elvanto.People.Person person, string familyId)
        {

            Fluro.Family.Contact contact = new Fluro.Family.Contact
            {
                data = new Fluro.Family.ContactData(),
                realms = new List<string>{
                    FluroRidgehavenRealm },
                phoneNumbers = new List<string>(),
                emails = new List<string>(),
                tags = new List<string>()
            };
            
            contact._type = "contact";

            contact.data.channel = "TimothyLuke Elvanto2Fluro";
            contact.data.importId = person.id;
            contact.title = person.firstname.ToLower() + " " + person.lastname.ToLower();
            contact.lastName = person.lastname.ToLower();
            contact.firstName = person.firstname.ToLower();
            if (person.gender != "" ) {
                contact.gender = person.gender;
            } else
            {
                // it has to be either male or female.  Search in Fluro for data.manualintervention to see who needs to be updated
                contact.tags.Add(FluroContentErrorTag);
                contact.gender = "female";
            }
            contact.dob = person.birthday;
            contact._type = "contact";
            if (familyId != "")
            {
                contact.family = familyId;
            }
            if (person.archived > 0)
            {
                contact.status = "archived";
            }
            else
            {
                contact.status = "active";
            }
            if (person.deceased > 0)
            {
                contact.status = "deceased";
                
            }
            if (person.volunteer > 0)
            {
                contact.data.volunteer = true;
            }

            
            if (person.category_id == ElvantoMemberCategory)
            {
                contact.tags.Add(FluroChurchMember);
            }
            if (person.votingMember != null)
            {
                if (person.votingMember.name == "Yes")
                {
                    contact.tags.Add(FluroVotingMember);
                }
            }
            contact.maritalStatus = person.marital_status;
            contact.householdRole = person.family_relationship;
            contact.data.photoURL = person.picture;
            contact.phoneNumbers.Add(person.mobile);
            contact.phoneNumbers.Add(person.phone);
            if (person.email == "" && person.phone == "" && person.mobile == "")
            {
                // needs to be something - Search in Fluro for data.manualintervention to see who needs to be updated
                contact.emails.Add("unknown@dev.null");
                contact.tags.Add(FluroContentErrorTag);

            } else
            {
                contact.emails.Add(person.email);
            }
            
            contact.data.preferredname = person.preferred_name;
            PerformFluroPersonUpload(contact);
        }

        static string AddFamilyToFluro(Elvanto.People.Person person)
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
            

            

            if (newfamily.title == null )
            {
                newfamily.title = person.lastname;
            }
            //newfamily.firstLine = newfamily.firstLine + ", " + person.firstname;
            //if (newfamily.firstLine.Left(2) == ", ")
            //{
            //    newfamily.firstLine = newfamily.firstLine.Substring(2);
            //}
            newfamily.emails.Add(person.email);
            if(newfamily.address.addressLine1 == null)
            {
                newfamily.address.addressLine1 = person.home_address;
                newfamily.address.addressLine2 = person.home_address2;
                newfamily.address.suburb = person.home_city;
                newfamily.address.state = person.home_state;
                if (person.home_postcode != "" )
                {
                    newfamily.address.postalCode = Convert.ToInt32(person.home_postcode);
                }
                newfamily.address.country = "Australia";
            }

            newfamily.phoneNumbers.Add(person.mobile);
            newfamily.phoneNumbers.Add(person.phone);

            if (person.archived > 0)
            {
                newfamily.status = "archived";
            }
            else
            {
                newfamily.status = "active";
            }
            



            return PerformFluroFamilyUpload(newfamily);
        }

        private static string  PerformFluroFamilyUpload(Fluro.Family.RootObject family)
        {
            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.Authorization] = "Bearer " + FluroAPIKey;
            client.UseDefaultCredentials = true;
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            string preuploadJson = JsonConvert.SerializeObject(family);
            logger.Info(preuploadJson);
            string stringFullOfJson = client.UploadString(FluroFamilyURI, preuploadJson);

            string returnId = GetFirstInstance<string>("_id", stringFullOfJson);
            return returnId;
        }

        private static void PerformFluroPersonUpload(Fluro.Family.Contact contact)
        {
            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.Authorization] = "Bearer " + FluroAPIKey;
            client.UseDefaultCredentials = true;
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            string preuploadJson = JsonConvert.SerializeObject(contact);
            logger.Info(preuploadJson);
            string stringFullOfJson = client.UploadString(FluroContactURI, preuploadJson);


            

        }

        static void MigrateSongs()
        {



            WebClient client = new WebClient
            {
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(ElvantoAPIKey, "")
                
            };
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            string stringFullOfJson = client.UploadString(ElvantoSongURI, "{\"files\": true}");

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
                if (song.status == "0")
                {
                    newsong.status = "archived";
                }
                else
                {
                    newsong.status = "active";
                }

                List<Elvanto.File> elvantofiles = new List<Elvanto.File>();

                string filesstring = song.files.ToString();

                if (filesstring.Length >= 3)
                {
                    List<Elvanto.File> files = GetFirstInstance<List<Elvanto.File>>("file", filesstring);
                    foreach (Elvanto.File file in files)
                    {
                        elvantofiles.Add(file);
                    }
                }    
                
                

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
            List<string> videoids = new List<string>();
            // add files 
            // first search to see if it already exists
            foreach (Elvanto.File file in files)
            {

                if (file.type == "Video")
                {
                    if(file.content.Left(7) == "<iframe")
                    {
                        videoids.Add(FindVideoFromIFrame(file.content));
                    } else
                    {
                        videoids.Add(AddVideoToFluro(file.content));
                    }
                    
                }
                else
                { 
                    using (WebClient client = new WebClient())
                    {
                        client.Headers[HttpRequestHeader.ContentType] = "application/json";
                        client.Headers[HttpRequestHeader.Authorization] = "Bearer " + FluroAPIKey;
                        string searchstring = FluroChordChartPostURI + "/search/" + Uri.EscapeUriString(file.title).Replace("/", " ");

                        string searchresult = client.DownloadString(searchstring);
                        if (searchresult.Length >= 3)
                        {
                            //found

                            chordchartids.Add(GetFirstInstance<string>("_id", searchresult));

                        }
                        else
                        {

                            // add new sheet
                            //download existing file
                            HttpClient dlclient = new HttpClient();

                            HttpResponseMessage response = new HttpResponseMessage();
                            if (file.content.Left(7) == "<iframe")
                            {
                                videoids.Add(FindVideoFromIFrame(file.content));
                            }
                            else
                            {
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
            newsong.data.videos = new List<object>();
            foreach (string videoid in videoids)
            {
                newsong.data.videos.Add(videoid);
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

        private static string FindVideoFromIFrame(string htmltag)
        {

            string resultstring = "";
            // This is an iframe
            string re1 = ".*?"; // Non-greedy match on filler
            string re2 = "\".*?\""; // Uninteresting: string
            string re3 = ".*?"; // Non-greedy match on filler
            string re4 = "\".*?\""; // Uninteresting: string
            string re5 = ".*?"; // Non-greedy match on filler
            string re6 = "(\".*?\")";   // Double Quote String 1

            Regex r = new Regex(re1 + re2 + re3 + re4 + re5 + re6, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = r.Match(htmltag);
            if (m.Success)
            {
                String videourl = m.Groups[1].ToString();
                videourl = "https:" + videourl.Replace("\"", "");
                videourl = videourl.Replace("embed/", "watch?v=");
                resultstring = AddVideoToFluro(videourl);
            }
            

            return resultstring;

        }

        private static string AddVideoToFluro(string videourl)
        {

            string resultstring = "";
            videourl = "https:" + videourl.Replace("\"", "");
            videourl = videourl.Replace("embed/", "watch?v=");
            videourl = videourl.Replace("https:https:", "https:");
            videourl = videourl.Replace("https:http:", "https:");
            videourl = videourl.Replace("http:", "https:");

            Fluro.Video video = new Fluro.Video();
            video.title = Util.GetYoutubeTitle(videourl);
            video.external = new External();
            video.external.youtube = videourl;
            video.realms = new List<Realm>();
            Realm realm = new Realm();
            realm._id = FluroCreativeRealm;
            video.realms.Add(realm);
            video._type = "video";
            video.assetType = "youtube";


            string upjson = JsonConvert.SerializeObject(video);
            logger.Info(upjson);
            using (WebClient upclient = new WebClient())
            {
                upclient.Headers[HttpRequestHeader.ContentType] = "application/json";
                upclient.Headers[HttpRequestHeader.Authorization] = "Bearer " + FluroAPIKey;
                logger.Info(upjson);
                string uploadresult = upclient.UploadString(FluroVideoURI, "POST", upjson);

                resultstring = GetFirstInstance<string>("_id", uploadresult);

            }


            return resultstring;

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

        public static T GetFirstInstance<T>(string propertyName, string json)
        {
            using (var stringReader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == JsonToken.PropertyName
                        && (string)jsonReader.Value == propertyName)
                    {
                        jsonReader.Read();

                        var serializer = new JsonSerializer();
                        return serializer.Deserialize<T>(jsonReader);
                    }
                }
                return default(T);
            }
        }
    }
}
