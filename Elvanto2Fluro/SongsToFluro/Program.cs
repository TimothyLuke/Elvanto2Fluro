
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

        
        private static string ElvantoSongURI = "https://api.elvanto.com/v1/songs/getAll.json";
        private static string ElvantoSongArrangementURI = "https://api.elvanto.com/v1/songs/arrangements/getAll.json";
        private static string ElvantoGroupURI = "https://api.elvanto.com/v1/groups/getAll.json";
        private static string ElvantoSongIndividualArangementURI = "https://api.elvanto.com/v1/songs/arrangements/getInfo.json";
        private static string ElvantoKeysURI = "https://api.elvanto.com/v1/songs/keys/getAll.json";
        private static string ElvantoIndividualKeyURI = "https://api.elvanto.com/v1/songs/keys/getInfo.json";
        private static string ElvantoPeopleURI = "https://api.elvanto.com/v1/people/getAll.json";
        private static string ElvantoMemberCategory = "a9802b8c-2e1b-11e2-9039-ef9e4c9f3a46";


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
        private static string FluroNewPeople = "5923a8be2b5ab52ecd519126";

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
            logger.Info("=================================================Starting People=================================================");
            MigratePeople();
            logger.Info("===================================================End People====================================================");
            logger.Info("=================================================Starting Groups=================================================");
            MigrateGroups();
            logger.Info("===================================================End Groups====================================================");

            Console.ReadLine();
        }

        static void MigrateGroups()
        {

            string arrangementresult = Util.UploadToElvantoReturnJson(ElvantoGroupURI, "POST", "{\"fields\":[\"people\"]}");
            GroupRootobject rootArrangement = JsonConvert.DeserializeObject<GroupRootobject>(arrangementresult);
            foreach (Elvanto.GroupCollection.Group group in rootArrangement.groups.group)
            {
                AddGroupToFluro(group);
            }

        }

        private static void AddGroupToFluro(Elvanto.GroupCollection.Group group)
        {
            Team team = new Team
            {
                title = group.name,
                allowProvisional = true,
                data = new TeamData
                {
                    importId = group.id,
                    meeting_address = group.meeting_address,
                    meeting_city = group.meeting_city,
                    meeting_country = group.meeting_country,
                    meeting_day = group.meeting_day,
                    meeting_frequency = group.meeting_frequency,
                    meeting_postcode = group.meeting_postcode,
                    meeting_state = group.meeting_state,
                    meeting_time = group.meeting_time,
                }
            };
            team.realms = new List<Realm>();
            Realm realm = new Realm
            {
                _id = FluroRidgehavenRealm
            };
            team.realms.Add(realm);

            string teamId = Util.UploadToFluroReturnId(FluroTeamURI, "POST", JsonConvert.SerializeObject(team));

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

            if (group.status != "Active")
            {

                if (teamId != null)
                {
                    string jsontoupload = $"{{ \"status\":\"archived\"}}";
                    string stringFullOfJson = Util.UploadToFluroReturnJson(FluroTeamURI + "/" + teamId, "PUT", jsontoupload);
                }

            }

        }


        private static void AddPersonToFluroGroup(string id, string teamId)
        {


            string returnId = Util.UploadToFluroReturnId(FluroOldMemberIdQuery + id, "GET", "");

            string jsontoupload = $"{{ \"_id\":\"{returnId}\"}}";

            Util.UploadToFluroReturnJson(String.Format(FluroTeamJoinURI, teamId), "POST", jsontoupload);
        }



        static void MigratePeople()
        {
            string stringFullOfJson = Util.UploadToElvantoReturnJson(ElvantoPeopleURI, "POST", "{\"fields\": [\"gender\", \"birthday\", \"marital_status\", \"home_address\", \"home_address2\", \"home_city\", \"home_state\", \"home_postcode\", \"departments\", \"custom_95d1c84c-6196-11e5-9d36-06ba798128be\"] }");

            var rootobj = JsonConvert.DeserializeObject<Elvanto.People.ELvantoPeopleRootObject>(stringFullOfJson);
            var groupedFamilyList = rootobj.people.person
                .GroupBy(u => u.family_id)
                .Select(grp => grp.ToList())
                .ToList();


            foreach (var family in groupedFamilyList)
            {

                string familyId = "";
                if (family.First().family_id == "")
                {
                    logger.Info("Group None - {0}", family.First().lastname);

                    familyId = "";
                }
                else
                {
                    logger.Info("Group {0} - {1}", family.First().family_id, family.First().lastname);
                    if (familyId == "")
                    {
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
            if (person.gender != "")
            {
                contact.gender = person.gender;
            }
            else
            {
                // it has to be either male or female.  Search in Fluro for data.manualintervention to see who needs to be updated
                contact.tags.Add(FluroContentErrorTag);
                contact.gender = "unknown";
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
            else
            {
                contact.tags.Add(FluroNewPeople);
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

            }
            else
            {
                contact.emails.Add(person.email);
            }

            contact.data.preferredname = person.preferred_name;
            Util.UploadToFluroReturnJson(FluroContactURI, "POST", JsonConvert.SerializeObject(contact));
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




            if (newfamily.title == null)
            {
                newfamily.title = person.lastname;
            }

            newfamily.emails.Add(person.email);
            if (newfamily.address.addressLine1 == null)
            {
                newfamily.address.addressLine1 = person.home_address;
                newfamily.address.addressLine2 = person.home_address2;
                newfamily.address.suburb = person.home_city;
                newfamily.address.state = person.home_state;
                if (person.home_postcode != "")
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


            return Util.UploadToFluroReturnId(FluroFamilyURI, "POST", JsonConvert.SerializeObject(newfamily));
        }


        static void MigrateSongs()
        {


            string stringFullOfJson = Util.UploadToElvantoReturnJson(ElvantoSongURI, "POST", "{\"files\": true}");

            Fluro.Realm realm = new Fluro.Realm
            {
                _id = FluroCreativeRealm
            };




            SongRootObject rootobj = JsonConvert.DeserializeObject<SongRootObject>(stringFullOfJson);


            foreach (Song song in rootobj.songs.song)
            {
                logger.Info($"{song.id} {song.title}");
                Fluro.RootObject newsong = new Fluro.RootObject
                {
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
                    List<Elvanto.File> files = Util.GetFirstInstance<List<Elvanto.File>>("file", filesstring);
                    foreach (Elvanto.File file in files)
                    {
                        elvantofiles.Add(file);
                    }
                }



                string arrangementresult = Util.UploadToElvantoReturnJson(ElvantoSongArrangementURI, "POST", "{\"song_id\": \"" + song.id + "\",    \"files\": true}");

                var rootArrangement = JsonConvert.DeserializeObject<Elvanto.Arrangement.ArrangementRootObject>(arrangementresult);
                newsong.data.lyrics = new List<object>();
                foreach(Elvanto.Arrangement.Arrangement arrangement in rootArrangement.arrangements.arrangement)
                {
                    newsong.data.lyrics.Add(arrangement.lyrics);
                    if (arrangement.files.ToString().Length > 2)
                    {
                        ProcessIndividualArrangementFiles(arrangement.id, elvantofiles);
                        foreach (Elvanto.File flz in elvantofiles)
                        {
                            logger.Info($"{flz.title} {flz.content}");
                        }
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
                    if (file.content.Left(7) == "<iframe")
                    {
                        videoids.Add(FindVideoFromIFrame(file.content));
                    }
                    else
                    {
                        videoids.Add(AddVideoToFluro(file.content));
                    }

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
                            chordchartids.Add(Util.UploadFiletoFluro(response, file, FluroCreativeRealm, FluroFileUploadURI));
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


            newsong.realms = new List<Realm>();
            Realm creative = new Realm
            {
                _id = FluroCreativeRealm
            };
            newsong.realms.Add(creative);
            newsong.definition = "song";
            Util.UploadToElvantoReturnJson(FluroSongURI, "POST", JsonConvert.SerializeObject(newsong));
            

        }




        /// <summary>
        /// This adds a video to Fluro.  It does a few replaces as we cannot trust the information stored in Elvanto.
        /// </summary>
        /// <param name="videourl"></param>
        /// <returns></returns>
        private static string AddVideoToFluro(string videourl)
        {

            videourl = "https:" + videourl.Replace("\"", "-")
                .Replace("embed/", "watch?v=")
                .Replace("https:https:", "https:")
                .Replace("https:http:", "https:")
                .Replace("http:", "https:");

            Fluro.Video video = new Fluro.Video
            {
                title = Util.GetYoutubeTitle(videourl),
                external = new External
                {
                    youtube = videourl
                },
                realms = new List<Realm>(),
                _type = "video",
                assetType = "youtube"
            };
            Realm realm = new Realm
            {
                _id = FluroCreativeRealm
            };
            video.realms.Add(realm);

            return Util.UploadToFluroReturnId(FluroVideoURI, "POST", JsonConvert.SerializeObject(video));

        }

        private static void ProcessIndividualArrangementFiles(string id, List<Elvanto.File> files)
        {
            
            string poststring = "{\"id\": \"" + id + "\",    \"files\": true}";
            string arrangementresult = Util.UploadToElvantoReturnJson(ElvantoSongIndividualArangementURI, "POST", poststring);

            var rootArrangement = JsonConvert.DeserializeObject<Elvanto.IndividualArrangement.IndividualArrangementRootObject>(arrangementresult);
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

        private static void ProcessKeyFiles(string id, List<Elvanto.File> files)
        {
            string poststring = "{\"arrangement_id\": \"" + id + "\",    \"files\": true}";
            string keyresult = Util.UploadToElvantoReturnJson(ElvantoKeysURI, "POST", poststring);

            Elvanto.Key.KeyRootObject rootkey = JsonConvert.DeserializeObject<Elvanto.Key.KeyRootObject>(keyresult);
            foreach (Elvanto.Key.Key key in rootkey.keys.key)
            {

                if (key.files.ToString().Length > 2)
                {
                    ProcessIndividualKeyFiles(key.id, files);
                }
            }

           
        }

        private static void ProcessIndividualKeyFiles(string id, List<Elvanto.File> files)
        {
            

            string poststring = "{\"id\": \"" + id + "\",    \"files\": true}";
            string keyresult = Util.UploadToElvantoReturnJson(ElvantoIndividualKeyURI, "POST", poststring);

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
        
        /// <summary>
        /// This finds the video string from an iframe.  Elvanto stores iframes as links to Youtube.
        /// </summary>
        /// <param name="htmltag"></param>
        /// <returns></returns>
        public static string FindVideoFromIFrame(string htmltag)
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

    }
}
