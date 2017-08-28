using System;
using System.Collections.Generic;
using System.IO;

using System.Net;
using System.Web;
using System.Collections.Specialized;
using NLog;
using Newtonsoft.Json;
using Elvanto2Fluro.Fluro;
using System.Net.Http;

namespace Elvanto2Fluro
{
    class Util
    {
        private static string ElvantoAPIKey = "4cwDPuZQO0x91sOz71VOOchajFl7Gg6A ";
        private static string FluroAPIKey = "$2a$10$jjHToxeGm1v.OdbxHy.NqOGm.wKfvaueG0g7pInRsGYy8DWNNutcO";
        private static string YoutubeAPIKey = "AIzaSyB-J50AxcA2NLYlSN0zd987J-1DQh1Edks";

        /// <summary>
        /// This method takes the URI, the Method and the JSON string and sends it to UploadToFluroReturnId to perform the upload.  It then grabs the _id and returns it.  Designed to avoid a ton of boilerplate. 
        /// </summary>
        /// <param name="FluroAPIURI"></param>
        /// <param name="Method"></param>
        /// <param name="JsonString"></param>
        /// <returns></returns>
        public static string UploadToFluroReturnId(string FluroAPIURI, string Method, string JsonString)
        {
            string jsonObject = UploadToFluroReturnJson(FluroAPIURI, Method, JsonString);
            string returnId = GetFirstInstance<string>("_id", jsonObject);
            logger.Debug("(Fluro) Obtained ID: {0}", returnId);

            return returnId;
        }

        /// <summary>
        /// This method takes the URI, the Method and the JSON string and returns the Json as returned from Fluro.  Designed to avoid a ton of boilerplate. 
        /// </summary>
        /// <param name="FluroAPIURI"></param>
        /// <param name="Method"></param>
        /// <param name="JsonString"></param>
        /// <returns></returns>
        public static string UploadToFluroReturnJson(string FluroAPIURI, string Method, string JsonString)
        {
            WebClient client = new WebClient
            {
                UseDefaultCredentials = true,

            };
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            client.Headers[HttpRequestHeader.Authorization] = "Bearer " + FluroAPIKey;


            logger.Debug("(Fluro) In JSON:");
            logger.Debug(JsonString);
            string stringFullOfJson = "";
            if (Method == "GET" && JsonString == "")
            {
                stringFullOfJson = client.DownloadString(FluroAPIURI);
            }
            else
            {
                stringFullOfJson = client.UploadString(FluroAPIURI, Method, JsonString);
            }
            
            logger.Debug("(Fluro) Return JSON:");
            logger.Debug(stringFullOfJson);
            return stringFullOfJson;

        }

        /// <summary>
        /// This perofmrs a client.UploadString to Elvanto and returns a JSON object. Designed to avoid a ton of boilerplate. 
        /// </summary>
        /// <param name="ElvantoAPIURI"></param>
        /// <param name="Method"></param>
        /// <param name="JsonString"></param>
        /// <returns></returns>
        public static string UploadToElvantoReturnJson(string ElvantoAPIURI, string Method, string JsonString)
        {
            WebClient client = new WebClient
            {
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(ElvantoAPIKey, "")
            };

            client.Headers[HttpRequestHeader.ContentType] = "application/json";


            logger.Debug("(Elvanto) In JSON:");
            logger.Debug(JsonString);
            string stringFullOfJson = client.UploadString(ElvantoAPIURI, Method, JsonString);
            logger.Debug("(Elvanto) Return JSON:");
            logger.Debug(stringFullOfJson);
            return stringFullOfJson;

        }


        /// <summary>
        /// This returns the first instance of a specific key without having to deserialise the entire Json object.  Useful when you just want the _id for example.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="json"></param>
        /// <returns></returns>
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


        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// This gets teh YourTube Video title from YouTubes API.
        /// The Key is stored in the static YoutubeAPIKey and is obtainable from https://console.developers.google.com/apis/dashboard
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetYoutubeTitle(string url)
        {
            string title = "Unknown Video Title";
            url = url.Replace("youtu.be/", "youtube.com/watch?v=");

            string titleurl = "https://www.googleapis.com/youtube/v3/videos?part=snippet&id=" + GetArgs(url, "v", '?') + "&key=" + YoutubeAPIKey;
            string timeurl = "https://www.googleapis.com/youtube/v3/videos?id=" + GetArgs(url, "v", '?') + "&part=contentDetails&key=" + YoutubeAPIKey;

            HttpWebRequest titlerequest = (HttpWebRequest)WebRequest.Create(titleurl);
            HttpWebResponse titleresponse = (HttpWebResponse)titlerequest.GetResponse();
            Stream titlestream = titleresponse.GetResponseStream();
            StreamReader titlereader = new StreamReader(titlestream);
            string titlejson = titlereader.ReadToEnd();
            Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(titlejson);

            try
            {
                title = (string)jObject["items"][0]["snippet"]["title"];
            }
            catch (ArgumentOutOfRangeException ex)
            {
                logger.Error(ex.Message);
                logger.Error("Unable to get title for {0}", url);

            }
            return title;
        }

        public static string GetArgs(string args, string key, char query)
        {
            int iqs = args.IndexOf(query);
            string querystring = null;

            if (iqs != -1)
            {
                querystring = (iqs < args.Length - 1) ? args.Substring(iqs + 1) : string.Empty;
                NameValueCollection nvcArgs = HttpUtility.ParseQueryString(querystring);
                return nvcArgs[key];
            }
            return string.Empty;
        }

        public static string UploadFiletoFluro(HttpResponseMessage response, Elvanto.File file, string realm, string UploadURI)
        {
            byte[] downloadedfile = response.Content.ReadAsByteArrayAsync().Result;
            string filetype = "";
            filetype = response.Content.Headers.ContentType.ToString();

            Dictionary<string, object> postParameters = new Dictionary<string, object>();
            SheetMusic sheet = new SheetMusic
            {
                title = file.title.Replace("/", " "),
                realms = new List<string>()
            };
            sheet.realms.Add(realm);
            sheet.definition = "sheetMusic";
            string jsonsheet = JsonConvert.SerializeObject(sheet);

            postParameters.Add("json", jsonsheet);
            postParameters.Add("?returnPopulated", true);
            postParameters.Add("file", new FormUpload.FileParameter(downloadedfile, FormUpload.GetFileName(file.content), filetype));
            string userAgent = "Timothy's C# Migrator";
            HttpWebResponse webResponse = FormUpload.MultipartFormDataPost(UploadURI, userAgent, postParameters, FluroAPIKey);

            StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
            string fullResponse = responseReader.ReadToEnd();
            logger.Debug(fullResponse);
            webResponse.Close();

            //need to parse the result and add it to the array
            Fluro.File.RootObject returnedfile = JsonConvert.DeserializeObject<Fluro.File.RootObject>(fullResponse);
            return returnedfile._id;

        }



    }

}
