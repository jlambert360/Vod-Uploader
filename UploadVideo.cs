using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Linq;
using System.Xml.Linq;
using System.Xml;

namespace VodUploader
{
    /// <summary>
    /// YouTube Data API v3 sample: upload a video.
    /// Relies on the Google APIs Client Library for .NET, v1.7.0 or higher.
    /// See https://code.google.com/p/google-api-dotnet-client/wiki/GettingStarted
    /// </summary>
    internal class UploadVideo
    {
        //string path = File.ReadLines("Path.txt").First();
        string desktoppath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string realFile = File.ReadLines(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/VodUploader/" + "Path.txt").First();
        public static string GameInfo = File.ReadLines(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/VodUploader/" + "Path.txt").Last();
        public static string FileName;
        //public static string VidTitle;
        public static string GameType;
        public static string PM_KEYWORDS = "\"PM\", \"Project M\", \"SSBPM\", \"Wii\"";
        public static string MELEE_KEYWORDS = "\"Melee\", \"SSBM\", \"Gamecube\"";
        public static string SMASH4_KEYWORDS = "\"Smash4\", \"Sm4sh\", \"Smash4WiiU\", \"Smash 4 Wii U\", \"SSB4\", \"Wii U\", \"S4\"";
        public static string SSB64_KEYWORDS = "\"SSB\", \"64\", \"N64\", \"SSB64\", \"Smash64\", \"Smash 64\"";
        public static string SMASH_KEYWORDS = "\"Smash\", \"Smash Bros\", \"Super Smash Bros\", \"SmashBros\", \"SuperSmashBros\"";
        public static string AON_KEYWORDS = "\"AON\", \"AON Gaming\", \"AON Smash\", \"Long Island\", \"Shady Penguin\", \"AON Long Island\", \"AON Gaming Long Island\", \"esports\"";


        [STAThread]
        static void Main(string[] args)
        {
            /* Gets player names and match type from Scoreboard Assistant 
            XmlDocument doc = new XmlDocument();
            doc.Load("D:/Users/Jordan/Downloads/Scoreboard-Assistant-v1.1.5/Scoreboard Assistant/output/versus.xml");
            XmlNodeList player1 = doc.GetElementsByTagName("player1");
            XmlNodeList player2 = doc.GetElementsByTagName("player2");
            XmlNodeList match = doc.GetElementsByTagName("match");
            for (int i = 0; i < player1.Count; i++)
            {
                Console.WriteLine(player1[i].InnerXml + " vs. " + player2[i].InnerXml + " " + match[i].InnerXml);
            }
            */

            Console.WriteLine("~~~AON Vod Uploader by Bird~~~");
            Console.WriteLine("==============================");
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/VodUploader/" + "Path.txt"))
            {
                Console.WriteLine("Path " + Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\VodUploader\\Path.txt does not exist! Please edit Path.txt to continue!");

                //File.Create(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/VodUploader/" + "Path.txt");
                File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/VodUploader/" + "Path.txt", $"Please type the path of your vod folder here. EX: C:/Users/YOURNAME/Desktop/VODS/");
                System.Threading.Thread.Sleep(60000);
                Environment.Exit(0);
            }
            //Asking information about the video
            Console.WriteLine("Enter the FULL name of the file you want to upload. EX: VOD.mp4");
            FileName = Console.ReadLine();
            //Console.WriteLine("Please enter a video title");
            //VidTitle = Console.ReadLine();
            Console.WriteLine("Is the game Melee, Smash 4, or Project M?");
            GameType = Console.ReadLine();

            try
            {
                new UploadVideo().Run().Wait();
            }
            //Prints errors to the console
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        //Connects to YouTube's API
        private async Task Run()
        {
            UserCredential credential;
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    // This OAuth 2.0 access scope allows an application to upload files to the
                    // authenticated user's YouTube channel, but doesn't allow other types of access.
                    new[] { YouTubeService.Scope.YoutubeUpload },
                    "user",
                    CancellationToken.None
                );
            }

            //Don't touch these
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });

            var video = new Video();
            video.Snippet = new VideoSnippet();


            //Loads the output for Scoreboard Assistant and makes a title out of Player1 vs. Player2 and the match type
            XmlDocument doc = new XmlDocument();
            doc.Load(GameInfo);
            //doc.Load("D:/Users/Jordan/Downloads/Scoreboard-Assistant-v1.1.5/Scoreboard Assistant/output/versus.xml");
            XmlNodeList player1 = doc.GetElementsByTagName("player1");
            XmlNodeList player2 = doc.GetElementsByTagName("player2");
            XmlNodeList match = doc.GetElementsByTagName("match");
            for (int i = 0; i < player1.Count; i++)
            {
                video.Snippet.Title = (player1[i].InnerXml + " vs. " + player2[i].InnerXml + " " + match[i].InnerXml);
                Console.WriteLine("Video title is " + player1[i].InnerXml + " vs. " + player2[i].InnerXml + " " + match[i].InnerXml);
            }

            //video.Snippet.Title = VidTitle;

            //Sets the description for the video
            video.Snippet.Description = "AON Gaming is a tournament store. We have all kinds of video game tournaments throughout the entire week. We offer the highest calibur of professional gaming tournaments across all of Long Island!\nSunday - Melee\nTuesday - Training Tuesdays\nThursday - Melee\nFriday - Smash 4\nSaturday - PM";

            //Sets the tags for the video
            if (GameType == "Project M")
            {
                video.Snippet.Tags = new string[] { SMASH_KEYWORDS + "\t" + AON_KEYWORDS + "\t" + PM_KEYWORDS };
            }
            else if (GameType == "Melee")
            {
                video.Snippet.Tags = new string[] { SMASH_KEYWORDS + "\t" + AON_KEYWORDS + "\t" + MELEE_KEYWORDS };
            }
            else if (GameType == "Smash 4")
            {
                video.Snippet.Tags = new string[] { SMASH_KEYWORDS + "\t" + AON_KEYWORDS + "\t" + SMASH4_KEYWORDS };
            }
            else if (GameType == "64")
            {
                video.Snippet.Tags = new string[] { SMASH_KEYWORDS + "\t" + AON_KEYWORDS + "\t" + SSB64_KEYWORDS };
            }
            else
            {
                Console.WriteLine(GameType + " is not a game! Program is closing. Please try again!");
                System.Threading.Thread.Sleep(3000);
                Environment.Exit(0);
            }


            //20 is youtube gaming in U.S.
            video.Snippet.CategoryId = "20"; // See https://developers.google.com/youtube/v3/docs/videoCategories/list
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = "unlisted"; //Should be either "private" or "public" or "unlisted"
            //string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //string path = File.ReadLines("MyFile.txt").First();
            //var filePath = desktoppath+"/Replay/Replay.mp4"; // Replace with path to actual movie file.

            //Finds the location of the video to upload
            var filePath = realFile + FileName;

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

                await videosInsertRequest.UploadAsync();
            }
        }


        //Ignore these! Change nothing here!
        void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    Console.WriteLine("{0} bytes sent.", progress.BytesSent);
                    break;

                case UploadStatus.Failed:
                    Console.WriteLine("An error prevented the upload from completing.\n{0}", progress.Exception);
                    break;
            }
        }

        void videosInsertRequest_ResponseReceived(Video video)
        {
            Console.WriteLine("Video id '{0}' was successfully uploaded.", video.Id);
        }
    }
}
