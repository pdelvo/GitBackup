using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GitBackup.FileSystemBackup
{
    public class DirectoryBackupSource : IBackupSource
    {
        public string Path { get; set; }

        public DirectoryBackupSource(string path)
        {
            path = path.Replace("/", "\\");
            Path = path.EndsWith("\\") ? path : path + "\\";
        }

        public IEnumerable<IBackupSourceFile> EnumerateFiles()
        {
            foreach (var file in Directory.EnumerateFiles(Path, "*.*", SearchOption.AllDirectories))
            {
                Trace.WriteLine("Found " + file, "Directory Backup Source");
                try
                {
                    using (new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {

                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Could not access file\r\n" + ex);
                    continue;
                }

                yield return new DirectoryBackupFile(file, this);
            }
        }

        private bool CanReadFile(string filename)
        {
            try
            {
                using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    internal class DirectoryBackupFile : IBackupSourceFile
    {
        public string Path { get; set; }
        public DirectoryBackupSource Source { get; set; }

        public DirectoryBackupFile(string path, DirectoryBackupSource source)
        {
            Path = path;
            Source = source;
        }

        public string RelativePath
        {
            get
            {
                var file = new Uri(Path);
                var folder = new Uri(Source.Path);
                return "./" + Uri.UnescapeDataString(folder.MakeRelativeUri(file).ToString());
            }
        }

        public string GetFileHash()
        {
            return FileHelpers.CalculateHash(Path);
        }

        public Stream Open()
        {
            return new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }


        public long GetFileSize()
        {
            return new FileInfo(Path).Length;
        }
    }
}
