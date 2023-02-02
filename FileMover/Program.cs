/*
 * Saebastion Cole 
 * 2/1/23
 * File Mover: monitors a folder and when a file is added to it moves it back one directory.
 */

using System;
using System.IO;
namespace FileMover
{
    class Program
    {

        static void Main(string[] args)
        {


            Console.WriteLine("Begin monitoring folder...");
            using var watcher = new FileSystemWatcher(@"FolderToMonitor");
            
            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            watcher.Error += OnError;

            watcher.Filter = "*.txt";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Console.WriteLine($"Changed: {e.FullPath}");

        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"Created: {e.FullPath}";
            Console.WriteLine(value);
            Console.WriteLine("Moving : " + e.FullPath);
            string path2 = @"PathToMoveTo";

            try
            {
                //If the file for some reason doesn't exist (it should in this context) create it.
                if (!File.Exists(e.FullPath))
                {
                    Console.WriteLine("File not found.. Creating empty file.");
                    using (FileStream fs = File.Create(e.FullPath)) { }
                }
                Console.WriteLine("PATH: "+ path2 + e.Name);

                //If the move file location doesn't exist create it.
                if (!File.Exists(path2+e.Name))
                {
                    Console.WriteLine("File not found at location " + path2 + e.Name + " creating");
                    using (FileStream fs = File.Create(path2 + e.Name)) { }
                    File.SetAttributes(path2 + e.Name, FileAttributes.Normal);
                    File.Delete(path2 + e.Name);
                }

                //Move the file to the new location.
                File.Move(e.FullPath, path2 + e.Name);
                File.SetAttributes(path2 + e.Name, FileAttributes.Normal);
                Console.WriteLine("{0} was moved to {1}.", e.FullPath, path2+e.Name);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }


        }

        private static void OnDeleted(object sender, FileSystemEventArgs e) =>
            Console.WriteLine($"Deleted: {e.FullPath}");

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Renamed:");
            Console.WriteLine($"    Old: {e.OldFullPath}");
            Console.WriteLine($"    New: {e.FullPath}");
        }

        private static void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private static void PrintException(Exception? ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        
   

            
        }

    }
}