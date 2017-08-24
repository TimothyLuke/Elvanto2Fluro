
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
        private static string ElvantoIndividualKeyURI = "https://api.elvanto.com/v1/songs/keys/getInfo.json";

        private static string FluroAPIKey = "$2a$10$jjHToxeGm1v.OdbxHy.NqOGm.wKfvaueG0g7pInRsGYy8DWNNutcO";
        private static string FluroSongURI = "https://apiv2.fluro.io/content/song/";
        private static string FluroChordChartPostURI = "https://apiv2.fluro.io/content/sheetMusic";
        private static string FluroCreativeRealm = "5923eaf4319df62ecc6f8005";

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
            //realm.title = "Creative";
            //realm.bgColor = "#e2a3ff";
            //realm.color = "#7f12b3";
            //realm._type = "realm";



            var rootobj = JsonConvert.DeserializeObject<Elvanto.RootObject>(stringFullOfJson);

            

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

                List<Elvanto.File> elvantofiles = new List<Elvanto.File>();

                Thread.Sleep(2000);
                using (WebClient newclient = new WebClient())
                {
                    newclient.Credentials = new NetworkCredential(ElvantoAPIKey, "");
                    newclient.UseDefaultCredentials = true;
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string arrangementresult = client.UploadString(ElvantoSongArrangementURI, "POST", "{\"song_id\": \"" + song.id + "\",    \"files\": true}");
                    var rootArrangement = JsonConvert.DeserializeObject<Elvanto.Arrangement.RootObject>(arrangementresult);

                    newsong.data.lyrics = new List<object>();
                    newsong.data.lyrics.Add(rootArrangement.arrangements.arrangement.First().lyrics);

                    

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

                AddSongToFluro(newsong, elvantofiles);

            }

            Console.ReadLine();
        }

        private static void AddSongToFluro(Fluro.RootObject newsong, List<Elvanto.File> files)
        {
            // add files 
            // first search to see if it already exists
            foreach (Elvanto.File file in files) {
                List<string> chordchartids = new List<string>();
                using (WebClient client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    client.Headers[HttpRequestHeader.Authorization] = "Bearer " + FluroAPIKey;
                    string searchresult = client.DownloadString(FluroChordChartPostURI + "search/" + Uri.EscapeUriString(file.title));
                    if (searchresult.Length < 3)
                    {
                        //found
                        var foundobjects = JsonConvert.DeserializeObject<SongsToFluro.Fluro.SheetMusicSearch.Rootobject>(searchresult);
                        foreach(SongsToFluro.Fluro.SheetMusicSearch.Class1 sheet in foundobjects.Property1)
                        {
                            chordchartids.Add(sheet._id);
                        }
                    } else
                    {
                        byte[] downloadedfile = new byte[1];
                        // add new sheet
                        //download existing file
                        HttpClient dlclient = new HttpClient();
                        string filetype = "";
                        dlclient.GetAsync(file.content).ContinueWith(
                            (requestTask) =>
                            {
                                // Get HTTP response from completed task.
                                HttpResponseMessage response = requestTask.Result;
                                // Check that response was successful or throw exception
                                response.EnsureSuccessStatusCode();
                                // Read content into buffer
                                response.Content.LoadIntoBufferAsync();
                                // The content can now be read multiple times using any ReadAs* extension method
                                downloadedfile = response.Content.ReadAsAsync<byte[]>().Result;
                                filetype = response.Content.Headers.ContentType.ToString();
                            }
                        );
                        Dictionary<string, object> postParameters = new Dictionary<string, object>();
                        SheetMusic sheet = new SheetMusic();
                        sheet.title = file.title;
                        sheet.realms.Add(FluroCreativeRealm);

                        postParameters.Add("json", JsonConvert.SerializeObject(sheet));
                        postParameters.Add("?returnPopulated", true);
                        postParameters.Add("file", new FormUpload.FileParameter(downloadedfile, FormUpload.GetFileName(file.content), filetype));
                        string userAgent = "Timothy's C# Migrator";
                        HttpWebResponse webResponse = FormUpload.MultipartFormDataPost(FluroChordChartPostURI, userAgent, postParameters);

                        StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
                        string fullResponse = responseReader.ReadToEnd();
                        webResponse.Close();
                        //need to parse the result and add it to the array
                    }
                }
            }

            //now check for the Song
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
                
                client.Credentials = new NetworkCredential(ElvantoAPIKey, "");
                client.UseDefaultCredentials = true;
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
                        files.Add(file);
                    }
                }
            }
        }
    }
}
