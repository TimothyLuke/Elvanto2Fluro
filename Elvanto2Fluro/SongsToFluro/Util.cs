using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Collections.Specialized;
namespace Elvanto2Fluro
{
    class Util
    {
        //  https://www.youtube.com/watch?v=lX5139rmgGw
        public static string GetYoutubeTitle(string url)
        {

            string apikey = "AIzaSyB-J50AxcA2NLYlSN0zd987J-1DQh1Edks";

            string titleurl = "https://www.googleapis.com/youtube/v3/videos?part=snippet&id=" + GetArgs(url, "v", '?') + "&key=" + apikey;
            string timeurl = "https://www.googleapis.com/youtube/v3/videos?id=" + GetArgs(url, "v", '?') + "&part=contentDetails&key=" + apikey;

            HttpWebRequest titlerequest = (HttpWebRequest)WebRequest.Create(titleurl);
            HttpWebResponse titleresponse = (HttpWebResponse)titlerequest.GetResponse();
            Stream titlestream = titleresponse.GetResponseStream();
            StreamReader titlereader = new StreamReader(titlestream);
            string titlejson = titlereader.ReadToEnd();
            Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(titlejson);

            string title = (string)jObject["items"][0]["snippet"]["title"];
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
