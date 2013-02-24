using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;

namespace GitBackup.FileSystemBackup
{
    internal class ZipBackup : IBackup
    {
        public string FilePath { get; protected set; }

        private HashSet<string> _names = new HashSet<string> (); 
        private Dictionary<string,string> _hashes = new Dictionary<string, string> (); 

        private InfoFile _info;

        protected InfoFile Info
        {
            get { return _info; }
            set { _info = value; }
        }

        protected ZipBackup()
        {
        }

        public ZipBackup(string filePath)
        {
            FilePath = filePath;
            
            var archive = new ZipFile(FilePath);

            foreach (var m in (from ZipEntry x in archive
                        where x.Name.ToLower().StartsWith("data/")
                        let m = "." + x.Name.Substring(4)
                        select m))
            {
                var name = m.Substring(0, m.LastIndexOf("."));
                var hash = m.Substring(m.LastIndexOf(".") + 1);
                _names.Add(name);
                _hashes.Add(name.ToLower(), hash);
            } 

            archive.Close();

            ReadInfo ();
        }

        private void ReadInfo()
        {
            _info = InfoFile.Read(InternalOpenFile("info.xml"));
        }

        public string Name{get { return Path.GetFileNameWithoutExtension(FilePath); }}

        public string Description
        {
            get { return _info.Comment; }
        }

        public string Creator
        {
            get { return _info.Issuer; }
        }

        public System.DateTime CreationDate
        {
            get { return _info.CreationDate; }
        }

        public IBackup ParentBackup
        {
            get
            {
                if (IsFullBackup) return null;
                var parentHash = _info.ParentName;
                return parentHash == null ? null : new ZipBackup(Path.Combine(Path.GetDirectoryName(FilePath), parentHash));
            }

        }

        public IEnumerable<string> GetAddedOrChangedFiles()
        {
            return _names;
        }

        public IEnumerable<string> GetDeletedFiles()
        {
            return _info.DeletedFiles;
        }

        public string GetFileHash(string path)
        {
            return _hashes[path.ToLower ()];
        }

        public Stream OpenFile(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (!path.StartsWith("./"))
                throw new ArgumentOutOfRangeException("Argument 'path' is out of range.",
                    new FormatException("The path must begin with './"));

            var actualPath = "data" + path.Substring(1);

            actualPath += "." + GetFileHash(path);

            return InternalOpenFile(actualPath);
        }

        protected Stream InternalOpenFile(string path)
        {
                var archive = new ZipFile(FilePath);
                var entry = archive.Cast<ZipEntry> ().Single(a=>a.Name == path);

                if (entry == null)
                {
                    throw new FileNotFoundException("File does not exist.");
                }

            var eventStream = new CloseEventStream(archive.GetInputStream(entry));
            eventStream.Closed += (s, e) => archive.Close ();
            return eventStream;
        }

        public void Delete()
        {
            File.Delete(FilePath);
        }

        public void Dispose()
        {
        }


        public long GetFileSize(string path)
        {
            var archive = new ZipFile(FilePath);
            var entry = archive.Cast<ZipEntry>().Where(a=>a.Name.StartsWith("data/")).Single(a => "." + a.Name.Substring(0, a.Name.LastIndexOf(".")).Substring(a.Name.IndexOf("/")) == path);

            if (entry == null)
            {
                throw new FileNotFoundException("File does not exist.");
            }

            return entry.Size;
        }


        public bool IsFullBackup
        {
            get { return _info.IsFullBackup; }
        }

        public IBackup SymbolicFullBackupParent
        {
            get
            {
                if(!IsFullBackup) throw new NotSupportedException("SymbolicFullBackupParent is only supported on full backups");
                var parentHash = _info.ParentName;
                return parentHash == null ? null : new ZipBackup(Path.Combine(Path.GetDirectoryName(FilePath), parentHash));
            }
        }
    }
}
