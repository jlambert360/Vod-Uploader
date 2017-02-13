using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRegisterMelee
{
    class MeleeRegisterFile
    {
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
        
        public static readonly string version = "0.1";
        static string desktoppath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        static string pathA = desktoppath + ("/VodUploader/" + "Path.txt");
        static string VodFolder = File.ReadLines(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/VodUploader/" + "Path.txt").First();
        static string mPath = File.ReadLines(pathA).Skip(3).Take(1).First();
        static string FileRegisterPathM = File.ReadLines(pathA).Skip(6).Take(1).First();

        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = "Melee Vod Uploader Version " + version;
            Console.WriteLine("~~~Melee Vod Uploader by Bird~~~");
            Console.WriteLine("==============================");
            FileSystemWatcher listener;
            listener = new FileSystemWatcher(VodFolder);
            listener.Created += new FileSystemEventHandler(listener_Created);
            listener.EnableRaisingEvents = true;

            while (Console.ReadLine() != "exit") ;

        }

        public static void listener_Created(object sender, FileSystemEventArgs e)
        {

            var directory = new DirectoryInfo(VodFolder);
            var myFile = directory.GetFiles()
             .OrderByDescending(q => q.LastWriteTime)
             .First();

            FileInfo f = new FileInfo(VodFolder + Convert.ToString(myFile));
            long size = f.Length;
            Console.WriteLine
                    (
                        "File Created:\n"
                       + "ChangeType: " + e.ChangeType
                       + "\nName: " + e.Name
                       + "\nFullPath: " + e.FullPath
                    );

            Run();
            AwaitFile();

            Console.Read();
        }
        
        public static void Run()
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = VodFolder;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            watcher.Filter = "*.mp4";
            
            watcher.Changed += new FileSystemEventHandler(OnChanged);

            watcher.EnableRaisingEvents = true;
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            var directory = new DirectoryInfo(VodFolder);
            var myFile = directory.GetFiles()
             .OrderByDescending(q => q.LastWriteTime)
             .First();

            FileStream stream = null;

            try
            {
                stream = myFile.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            FileInfo f = new FileInfo(VodFolder + Convert.ToString(myFile));
            Console.WriteLine("File size is: " + SizeSuffix(f.Length));
            System.Threading.Thread.Sleep(2000);
            
        }

        public static void AwaitFile()
        {

            var directory = new DirectoryInfo(VodFolder);
            var myFile = directory.GetFiles()
             .OrderByDescending(q => q.LastWriteTime)
             .First();

            FileInfo f = new FileInfo(VodFolder + Convert.ToString(myFile));

            var file = new FileInfo(VodFolder + myFile);
            
            while (IsFileLocked(file))
            {
                Console.WriteLine("Vod is being recorded!");
                System.Threading.Thread.Sleep(2000);
            }
            //File is available here
            Console.WriteLine("Vod is done being recorded! \nFinal file size is: " + SizeSuffix(f.Length));
            try
            {
                Process RegisterProcM = new Process();
                RegisterProcM.StartInfo.FileName = FileRegisterPathM;
                RegisterProcM.EnableRaisingEvents = true;
                RegisterProcM.Start();

                Process mProc = new Process();
                mProc.StartInfo.FileName = mPath;
                mProc.EnableRaisingEvents = true;
                mProc.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred!!!: " + ex.Message);
                return;
            }

            System.Threading.Thread.Sleep(5000);
            Environment.Exit(0);
        }

        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
    }
}
