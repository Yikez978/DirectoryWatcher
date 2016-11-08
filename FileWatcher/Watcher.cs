﻿using System;
using System.IO;
using System.Security.Permissions;
using System.Threading;

public class Watcher
{
    public static void Main()
    {
        Run();
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public static void Run()
    {
        string[] args = System.Environment.GetCommandLineArgs();

        // if a directory is not specified, exit program.
        if (args.Length != 2)
        {
            // Display usage if no argument was given
            Console.WriteLine("usage: brfilesmonitor.exe [directory]");
            return;
        }

        FileSystemWatcher watcher = new FileSystemWatcher();
        watcher.Path = args[1]; // The argument is the path that will be monitored

        // Watch for changes in LastAccess and LastWrite for both folders and files 
        watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
           | NotifyFilters.FileName | NotifyFilters.DirectoryName;
        
        // Watch all files and subfolders.
        watcher.Filter = "*.*";
        watcher.IncludeSubdirectories = true;

        // Binding event handlers.
        watcher.Changed += new FileSystemEventHandler(OnChanged);
        watcher.Created += new FileSystemEventHandler(OnChanged);
        watcher.Deleted += new FileSystemEventHandler(OnChanged);
        watcher.Renamed += new RenamedEventHandler(OnRenamed);

        // Begin watching.
        watcher.EnableRaisingEvents = true;
        
        // Sleep for 1 milisecond.
        while (true) Thread.Sleep(1000);
    }

    // Event handlers.
    private static void OnChanged(object source, FileSystemEventArgs e)
    {
        if (Directory.Exists(e.FullPath)) Console.WriteLine(e.ChangeType + "|directory|" + e.FullPath);
        else
        {
            string fileID = FileID.getFileUniqueSystemID(e.FullPath);
            Console.WriteLine("{0}|{1}|{2}|{3}", e.ChangeType, "file", fileID, e.FullPath);
        }

        return;
    }

    private static void OnRenamed(object source, RenamedEventArgs e)
    {
        string fileOrDirectory = null;
        string fileID = "-1";

        if (Directory.Exists(e.FullPath)) fileOrDirectory = "directory";
        else
        {
            fileOrDirectory = "file";
            fileID = FileID.getFileUniqueSystemID(e.FullPath);
        }

        Console.WriteLine("{0}|{1}|{2}|{3}|{4}", e.ChangeType, fileOrDirectory, e.OldFullPath, e.FullPath, fileID);

        return;
    }
}