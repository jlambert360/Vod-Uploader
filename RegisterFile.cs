using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRegister
{
    class RegisterFile
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
        static string desktoppath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        static string pathA = desktoppath + ("/VodUploader/" + "Path.txt");
        static string VodFolder = File.ReadLines(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/VodUploader/" + "Path.txt").First();
        static string UploadPath = File.ReadLines(pathA).Skip(2).Take(1).First();


        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("~~~AON Vod Uploader by Bird~~~");
            Console.WriteLine("==============================");

            //string path = @"D:\levan\FolderListenerTest\ListenedFolder";
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
            //long filePathA = (f.Length);

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
            /*
            do
            {
                //Console.WriteLine(SizeSuffix(f.Length));
                Console.WriteLine(f.Length);
                System.Threading.Thread.Sleep(2000);
            }
            while (size != -1);
            */
            //File.Copy(e.FullPath, @"D:\levan\FolderListenerTest\CopiedFilesFolder\" + e.Name);
            Console.Read();
        }
        
        
        
        public static void Run()
        {
            
            //string[] args = System.Environment.GetCommandLineArgs();

            // If a directory is not specified, exit program.
            //if (args.Length != 2)
            //{
                // Display the proper way to call the program.
                //Console.WriteLine("Usage: Watcher.exe (directory)");
                //return;
            //}

            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = VodFolder;
            //watcher.Path = args[1];
            /* Watch for changes in LastAccess and LastWrite times, and
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Only watch mp4 files.
            watcher.Filter = "*.mp4";

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            //watcher.Created += new FileSystemEventHandler(OnChanged);
            //watcher.Deleted += new FileSystemEventHandler(OnChanged);
            //watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Begin watching.
            watcher.EnableRaisingEvents = true;

            // Wait for the user to quit the program.
            //Console.WriteLine("Press \'q\' to quit the sample.");
            //while (Console.Read() != 'q') ;
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
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                //return Console.WriteLine("");
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            
            FileInfo f = new FileInfo(VodFolder + Convert.ToString(myFile));
            // Specify what is done when a file is changed, created, or deleted.

            //Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType + " " + SizeSuffix(f.Length));

            /*
            long oldSize = f.Length;
            try
            {
                
                System.Threading.Thread.Sleep(1000);
                long sizeFinal = f.Length;

                if (oldSize == sizeFinal)
                {
                    Console.WriteLine("File size was the same!");
                }
            }
            finally
            {
                Console.WriteLine("File size is: " + SizeSuffix(f.Length));
            }
            //oldSize = sizeFinal;
            */
            //Console.WriteLine("File size is: " + SizeSuffix(f.Length));
        }

        public static void AwaitFile()
        {

            var directory = new DirectoryInfo(VodFolder);
            var myFile = directory.GetFiles()
             .OrderByDescending(q => q.LastWriteTime)
             .First();

            FileInfo f = new FileInfo(VodFolder + myFile);
            
            //Your File
            var file = new FileInfo(VodFolder + myFile);

            //While File is not accesable because of writing process
            while (IsFileLocked(file))
            {
                //Console.WriteLine(f.Name);
                //Console.WriteLine("File size is: " + SizeSuffix(f.Length));
                //
                Console.WriteLine("Vod is being recorded!");
                System.Threading.Thread.Sleep(2000);
            }
            Console.WriteLine("Vod is done being recorded!");
            
            //File is available here

            try
            {

                Process firstProc = new Process();
                firstProc.StartInfo.FileName = UploadPath;
                firstProc.EnableRaisingEvents = true;

                firstProc.Start();

                //firstProc.WaitForExit();

                //You may want to perform different actions depending on the exit code.
                //Console.WriteLine("First process exited: " + firstProc.ExitCode);

                //Process secondProc = new Process();
                //secondProc.StartInfo.FileName = "mspaint.exe";
                //secondProc.Start();

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
