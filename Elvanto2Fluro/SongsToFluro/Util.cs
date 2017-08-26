using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Collections.Specialized;
using NLog;

namespace Elvanto2Fluro
{
    class Util
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        //  https://www.youtube.com/watch?v=lX5139rmgGw
        public static string GetYoutubeTitle(string url)
        {
            string title = "Unknown Video Title";
            url = url.Replace("youtu.be/", "youtube.com/watch?v=");
            string apikey = "AIzaSyB-J50AxcA2NLYlSN0zd987J-1DQh1Edks";

            string titleurl = "https://www.googleapis.com/youtube/v3/videos?part=snippet&id=" + GetArgs(url, "v", '?') + "&key=" + apikey;
            string timeurl = "https://www.googleapis.com/youtube/v3/videos?id=" + GetArgs(url, "v", '?') + "&part=contentDetails&key=" + apikey;

            HttpWebRequest titlerequest = (HttpWebRequest)WebRequest.Create(titleurl);
            HttpWebResponse titleresponse = (HttpWebResponse)titlerequest.GetResponse();
            Stream titlestream = titleresponse.GetResponseStream();
            StreamReader titlereader = new StreamReader(titlestream);
            string titlejson = titlereader.ReadToEnd();
            Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(titlejson);

            try
            {
                 title = (string)jObject["items"][0]["snippet"]["title"];
            } catch (ArgumentOutOfRangeException ex)
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
    }

    
}
