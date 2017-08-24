using Elvanto;
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

namespace SongsToFluro
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static string ElvantoAPIKey = "4cwDPuZQO0x91sOz71VOOchajFl7Gg6A ";
        private static string ElvantoSongURI = "https://api.elvanto.com/v1/songs/getAll.json";
        private static string ElvantoSongArrangementURI = "https://api.elvanto.com/v1/songs/arrangements/getAll.json";
        private static string ElvantoSongIndividualArangementURI = "https://api.elvanto.com/v1/songs/arrangements/getInfo.json";
        private static string ElvantoKeysURI = "https://api.elvanto.com/v1/songs/keys/getArrangement.json"; 

        static void Main(string[] args)
        {

            JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(() => new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });



            WebClient client = new WebClient();
            client.UseDefaultCredentials = true;
            client.Credentials = new NetworkCredential(ElvantoAPIKey, "");
            string stringFullOfJson = client.DownloadString(ElvantoSongURI);

            Fluro.Realm realm = new Fluro.Realm();
            realm._id = "5923eaf4319df62ecc6f8005";
            realm.title = "Creative";
            realm.bgColor = "#e2a3ff";
            realm.color = "#7f12b3";
            realm._type = "realm";



            var rootobj = JsonConvert.DeserializeObject<RootObject>(stringFullOfJson);

            List<Fluro.RootObject> FluroSongs = new List<Fluro.RootObject>();

            foreach (Song song in rootobj.songs.song) {
                logger.Info($"{song.id} {song.title}");
                Fluro.RootObject newsong = new Fluro.RootObject();

                //behold id ad17eee6-3544-11e7-ba01-061a3b9c64af

                //song

                //string arrangements = client.DownloadString("{\"song_id\": \"" + song.id + "\",    \"files\": true}");

                newsong.title = song.title;
                newsong.realms = new List<Fluro.Realm>();
                newsong.realms.Add(realm);
                newsong.data = new Fluro.Data();
                newsong.data.artist = song.artist;
                newsong.data.album = song.album;
                newsong.data.ccli = song.number;

                using (WebClient newclient = new WebClient())
                {
                    newclient.Credentials = new NetworkCredential(ElvantoAPIKey, "");
                    newclient.UseDefaultCredentials = true;
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string arrangementresult = client.UploadString(ElvantoSongArrangementURI, "POST", "{\"song_id\": \"" + song.id + "\",    \"files\": true}");
                    var rootArrangement = JsonConvert.DeserializeObject<Elvanto.Arrangement.RootObject>(arrangementresult);

                    newsong.data.lyrics = new List<object>();
                    newsong.data.lyrics.Add(rootArrangement.arrangements.arrangement.First().lyrics);

                    List<Elvanto.File> elvantofiles = new List<Elvanto.File>();

                    try
                    {
                        JObject fileses = (JObject)rootArrangement.arrangements.arrangement.First().files;


                        if (fileses.HasValues)
                        {

                            // Individual call to get arrangement with files

                            ProcessIndividualArrangementFiles(rootArrangement.arrangements.arrangement.First().id, elvantofiles);
                            foreach (Elvanto.File flz in elvantofiles)
                            {
                                logger.Info($"{flz.title} {flz.content}");
                            }
                        }
                    } catch (InvalidCastException ex)
                    {

                    }
                    





                }

                FluroSongs.Add(newsong);

            }

            Console.ReadLine();
        }

        private static void ProcessIndividualArrangementFiles(string id, List<Elvanto.File> files)
        {
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
                        files.Add(file);

                        ProcessIndividualKeyFiles(id, files);
                    }

                }

            }
        }

        private static void ProcessIndividualKeyFiles(string id, List<File> files)
        {
            using (WebClient piuclient = new WebClient())
            {
                piuclient.UseDefaultCredentials = true;
                piuclient.Credentials = new NetworkCredential(ElvantoAPIKey, "");
                piuclient.Headers[HttpRequestHeader.ContentType] = "application/json";
                logger.Info($"Getting Key Files for Arrangement {id}");
                string poststring = "{\"arrangement_id\": \"" + id + "\",    \"files\": true}";
                string arrangementresult = piuclient.UploadString(ElvantoKeysURI, "POST", poststring);

                var rootArrangement = JsonConvert.DeserializeObject<Elvanto.IndividualArrangement.RootObject>(arrangementresult);
                foreach (Elvanto.IndividualArrangement.Arrangement arr in rootArrangement.arrangement)
                {
                    foreach (Elvanto.File file in arr.files.file)
                    {
                        files.Add(file);

                        ProcessIndividualKeyFiles(id, files);
                    }

                }

            }
        }
    }
}
