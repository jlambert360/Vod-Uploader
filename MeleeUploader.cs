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
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Melee_Uploader
{
    class MeleeUploader
    {
        public static readonly string version = "0.1";
        public static string FileName;
        static string desktoppath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        static string pathA = desktoppath + ("/VodUploader/" + "Path.txt");
        static string realFile = File.ReadLines((Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/VodUploader/" + "Path.txt")).First();
        static string pathB = Convert.ToString(File.ReadLines(pathA));
        string dubsPath = File.ReadLines(pathA).Skip(4).Take(1).First();
        public static string privacySetting = File.ReadLines(pathA).Skip(1).Take(1).First();
        public static string GameInfo = File.ReadLines((Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/VodUploader/" + "Path.txt")).Last();
        static string descriptionPath = desktoppath + ("/VodUploader/" + "Description.txt");
        static string tagsPath = desktoppath + ("/VodUploader/" + "Tags.txt");

        string descriptionText = File.ReadAllLines(descriptionPath).First();
        string tagsText = File.ReadAllLines(tagsPath).First();

        //public static string VidTitle;
        public static string MELEE_KEYWORDS = "\"Melee\", \"SSBM\", \"Gamecube\"";
        public static string SMASH_KEYWORDS = "\"Smash\", \"Smash Bros\", \"Super Smash Bros\", \"SmashBros\", \"SuperSmashBros\"";

        static readonly string[] SizeSuffixes =
                  { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        static string SizeSuffix(Int64 value)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }

            int i = 0;
            decimal dValue = (decimal)value;
            while (Math.Round(dValue / 1024) >= 1)
            {
                dValue /= 1024;
                i++;
            }

            return string.Format("{0:n1} {1}", dValue, SizeSuffixes[i]);
        }

        [STAThread]
        static void Main(string[] args)
        {
            var directory = new DirectoryInfo(realFile);
            var myFile = directory.GetFiles()
             .OrderByDescending(q => q.LastWriteTime)
             .First();
            Console.Title = "Melee Vod Uploader Version " + version;
            Console.WriteLine("~~~Melee Vod Uploader by Bird~~~");
            Console.WriteLine("==============================");
            //Console.WriteLine("Is the file name " + myFile);
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/VodUploader/" + "Path.txt"))
            {
                Console.WriteLine("Path " + Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\VodUploader\\Path.txt does not exist! Please edit Path.txt to continue!");

                File.Create(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/VodUploader/" + "Path.txt");
                System.Threading.Thread.Sleep(3000);
                File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/VodUploader/" + "Path.txt", $"Please type the path of your vod folder here. EX: C:/Users/YOURNAME/Desktop/VODS/");
                System.Threading.Thread.Sleep(60000);
                Environment.Exit(0);
            }

            FileName = Convert.ToString(myFile);


            try
            {
                new MeleeUploader().Run().Wait();
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
                Console.WriteLine("Melee Uploader: Video title is " + player1[i].InnerXml + " vs. " + player2[i].InnerXml + " " + match[i].InnerXml);
            }


            video.Snippet.Description = descriptionText;

            video.Snippet.Tags = new string[] { SMASH_KEYWORDS + "\t" + tagsText + "\t" + MELEE_KEYWORDS };


            //20 is youtube gaming in U.S.
            video.Snippet.CategoryId = "20"; // See https://developers.google.com/youtube/v3/docs/videoCategories/list
            video.Status = new VideoStatus();

            if (privacySetting == Convert.ToString(1))
            {
                Console.WriteLine("Video uploading as Public");
                video.Status.PrivacyStatus = "public";
            }
            else if (privacySetting == Convert.ToString(2))
            {
                Console.WriteLine("Video uploading as Unlisted");
                video.Status.PrivacyStatus = "unlisted";
            }

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
                    FileInfo f = new FileInfo(realFile + FileName);
                    long filePathA = (f.Length);
                    Console.WriteLine("{0} bytes sent out of {1}.", SizeSuffix(progress.BytesSent), SizeSuffix(filePathA));
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
