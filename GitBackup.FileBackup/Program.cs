﻿using System;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using CLAP;
using GitBackup.FileSystemBackup;
using System.IO;
namespace GitBackup.FileBackup
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            Console.WriteLine("GitBackup by pdelvo");
            Console.WriteLine("GitBackup is a tool to create incremental backups in a git flavour.");
            Console.WriteLine("===================================================================");
            Console.WriteLine ();

            //Try to expand the console window 
            try
            {
                Console.WindowWidth = Console.BufferWidth = 150;
            }
// ReSharper disable EmptyGeneralCatchClause
            catch
// ReSharper restore EmptyGeneralCatchClause
            {
            }

            Trace.Listeners.Add(new ConsoleTraceListener ());

            var instance = new BackupTool ();

            Parser.RunConsole(args, instance);
        }
    }

    public class BackupTool
    {
        [Verb(Description = "Create a new backup", Aliases = "c")]
        public void Create(
            [Description("The root of the directory which should be saved")]
            [Required]
            string workingDirectory,
            [Description("The path of the backup directory")]
            [Required]
            string resultPath,
            [DefaultValue(null)]
            [Description("Defines the issuer of this backup")]
            string issuer,
            [DefaultValue("HEAD")]
            [Description("Defines the branch of this backup")]
            string branch,
            [DefaultValue(null)]
            [Description("Defines the comment of this backup")]
            string comment,
            [DefaultValue(false)]
            [Description("If this is set to true the backup itself is standalone")]
            bool fullBackup)
        {
            var backupRepo = new ZipBackupRepository(resultPath);

            if(!Directory.Exists(workingDirectory))
                throw new DirectoryNotFoundException("Working directory could not be found");

            var backup = backupRepo.GetBackup(branch);

            if (backup == null)
                throw new BackupNotFoundException("Could not find any backup that fits to the given branch.");

            using (backup)
            using (var prototype = backupRepo.CreateBackup ())
            {
                Trace.WriteLine(fullBackup ? "Creating full backup" : "Creating backup", "File Tool");
                prototype.SetCreationDate(DateTime.Now);
                prototype.SetCreator(issuer);
                prototype.SetDescription(comment);
                prototype.SetParentBackup(backup);

                var source = new DirectoryBackupSource(workingDirectory);

                var patch = fullBackup ? Patch.CreateFromSource(source) : Patch.Compare(source, backup.GetChain());

                patch.ApplyPatch(prototype);

                if(fullBackup)
                    prototype.SetFullBackup(true);

                prototype.Save ();

                if (backupRepo.HeadExists(branch))
                    backupRepo.UpdateHead(branch, prototype.Name);
                else
                    Trace.WriteLine("Backup created without having a branch pointing at it.");
            }


        }

        [Verb(Description = "Extracts the current state of a backup", Aliases = "e")]
        public void Extract(
            [Description("The path of the backup directory")]
            [Required]
            string backupPath,
            [Description("The path where the resulting files should be placed in")]
            [Required]
            string resultPath,
            [Description("The Identifier for the backup (can be a branch or hash)")]
            [Required]
            string identifier)
        {
            var backupRepo = new ZipBackupRepository(backupPath);

            var backup = backupRepo.GetBackup(identifier);

            var chain = backup.GetChain();

            resultPath = resultPath.Replace("/", "\\");

            resultPath = resultPath.EndsWith("\\") ? resultPath : resultPath + "\\";

            Directory.CreateDirectory(resultPath);

            foreach (var file in chain.GetFiles())
            {
                using (var stream = chain.OpenFile(file))
                {
                    var resultingPath = resultPath + file.Substring(2);

                    Directory.CreateDirectory(Path.GetDirectoryName(resultingPath));

                    using (var fileStream = File.Create(resultingPath))
                    {
                        stream.CopyTo(fileStream);
                    }
                }
            }
        }

        [Verb(Description = "Pack the current state of a backup", Aliases = "e")]
        public void Pack(
            [Description("The path of the backup directory")]
            [Required]
            string backupPath,
            [Description("The path where the resulting zip file should be placed in")]
            [Required]
            string resultPath,
            [Description("The Identifier for the backup (can be a branch or hash)")]
            [Required]
            string identifier)
        {
            var backupRepo = new ZipBackupRepository(backupPath);

            var backup = backupRepo.GetBackup(identifier);

            var chain = backup.GetChain();

            resultPath = resultPath.Replace("/", "\\");

            Directory.CreateDirectory(Path.GetDirectoryName(resultPath));

            using (var resultFile = ZipFile.Open(resultPath, ZipArchiveMode.Create))
            {
                foreach (var file in chain.GetFiles ())
                {
                    using (var stream = chain.OpenFile(file))
                    {
                        var resultingPath = file.Substring(2);

                        var entry = resultFile.CreateEntry(resultingPath);
                        
                        using (var fileStream = entry.Open ())
                        {
                            stream.CopyTo(fileStream);
                        }
                    }
                }
            }
        }

        [Verb(Description = "Create an empty backup directory", Aliases = "i")]
        public void Init(
            [Description("The path of the backup directory")]
            [Required]
            string backupPath)
        {
            ZipBackupRepository.Init(backupPath);
        }

        [Verb(Description = "Shows a list of taken backups", Aliases = "l")]
        public void List(
            [Description("The path of the backup directory")]
            [Required]
            string backupPath,
            [Description("The identifier for the listing should start from")]
            [DefaultValue("HEAD")]
            string identifier)
        {
            var backupRepo = new ZipBackupRepository(backupPath);

            var backup = backupRepo.GetBackup(identifier);

            var chain = backup.GetChain ();

            foreach (var chainedBackup in chain)
            {
                Console.WriteLine("[{0}] {1}: {2} (Added or changed: {3}, Deleted: {4}, Created by {5})", 
                    chainedBackup.CreationDate, 
                    chainedBackup.Name ?? "(null)", 
                    chainedBackup.Description ?? "(null)", 
                    chainedBackup.GetAddedOrChangedFiles ().Count(), 
                    chainedBackup.GetDeletedFiles ().Count(),
                    chainedBackup.Creator ?? "(null)");
            }
        }

        [Verb(Description = "List all heads", Aliases = "lh")]
        public void ListHeads(
             [Description("The path of the backup directory")]
            [Required]
            string backupPath
            )
        {
            var backupRepo = new ZipBackupRepository(backupPath);

            Console.WriteLine("Heads:");

            foreach (var head in backupRepo.GetHeads())
            {
                Console.WriteLine("{0} -> {1}", head, backupRepo.ResolveIdentifier(head));
            }
        }

        [Verb(Description = "Creates a new head", Aliases = "ch")]
        public void CreateHead(
            [Description("The path of the backup directory")]
            [Required]
            string backupPath,
            [Description("The name of the head")]
            [Required] 
            string name,
            [Description("The identifier the head should point to (can be a backup id or a head name)")]
            [Required] 
            string identifier)
        {
            var backupRepo = new ZipBackupRepository(backupPath);

            if (backupRepo.HeadExists(name))
                throw new InvalidOperationException("The head already exists");

            backupRepo.AddHead(name, backupRepo.ResolveIdentifier(identifier));
        }

        [Verb(Description = "Renames a head", Aliases = "rh")]
        public void RenameHead(
            [Description("The path of the backup directory")]
            [Required]
            string backupPath,
            [Description("The name of the head")]
            [Required] 
            string name,
            [Description("The new name of the head")]
            [Required] 
            string newName)
        {
            var backupRepo = new ZipBackupRepository(backupPath);

            if (!backupRepo.HeadExists(name))
                throw new InvalidOperationException("The head does not exist");

            var value = backupRepo.ResolveIdentifier(name);
            backupRepo.AddHead(newName, value);
            backupRepo.RemoveHead(name);
        }

        [Verb(Description = "Removes a head", Aliases = "dh")]
        public void RemoveHead(
            [Description("The path of the backup directory")]
            [Required]
            string backupPath,
            [Description("The name of the head")]
            [Required] 
            string name)
        {
            var backupRepo = new ZipBackupRepository(backupPath);

            if (!backupRepo.HeadExists(name))
                throw new InvalidOperationException("The head does not exist");
            backupRepo.RemoveHead(name);
        }

        [Empty, Help]
        public void Help(string help)
        {
            // this is an empty handler that prints
            // the automatic help string to the console.
            using (ConsoleHelper.Color(ConsoleColor.Cyan))
            {
                Console.WriteLine("Help");
            }
            Console.WriteLine(help);
        }

    }
}
