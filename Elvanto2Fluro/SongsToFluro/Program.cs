using Elvanto;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SongsToFluro
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {


            string ElvantoAPIKey = "4cwDPuZQO0x91sOz71VOOchajFl7Gg6A ";
            string ElvantoSongURI = "https://api.elvanto.com/v1/songs/getAll.json";
            WebClient client = new WebClient();
            client.UseDefaultCredentials = true;
            client.Credentials = new NetworkCredential(ElvantoAPIKey, "");
            string stringFullOfJson = client.DownloadString(ElvantoSongURI);

            
            var rootobj = JsonConvert.DeserializeObject<RootObject>(stringFullOfJson);

            foreach (Song song in rootobj.songs.song) {
                logger.Info($"{song.id}");
                logger.Info($"{song.title}");
            }

            Console.ReadLine();
        }


    }
}
