using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using SysIO = System.IO;

namespace GitBackup.FileSystemBackup
{
    public class ZipBackupRepository : IBackupRepository
    {
        static Random _random = new Random();
        public string Path { get; private set; }

        public ZipBackupRepository(string path)
        {
            if(!Directory.Exists(path))
                throw new DirectoryNotFoundException("Directory could not be found");
            if (!Directory.Exists(SysIO.Path.Combine(path, "data")))
                throw new InvalidOperationException("Directory is not a valid backup location");
            if (!Directory.Exists(SysIO.Path.Combine(path, "heads")))
                throw new InvalidOperationException("Directory is not a valid backup location");
            if (!Directory.Exists(SysIO.Path.Combine(path, "temp")))
                throw new InvalidOperationException("Directory is not a valid backup location");
            Path = path;
        }

        public IEnumerable<string> GetHeads()
        {
            return Directory.EnumerateFiles(SysIO.Path.Combine(Path, "heads")).Select(SysIO.Path.GetFileNameWithoutExtension);
        }

        public void AddHead(string name, string pointer)
        {
            if(HeadExists(name))
                throw new InvalidOperationException("Head already exists");

            File.WriteAllText(SysIO.Path.Combine(Path, "heads", name), pointer);
        }

        public void RemoveHead(string name)
        {
            if (!HeadExists(name))
                throw new InvalidOperationException("Head does not exist");

            File.Delete(SysIO.Path.Combine(Path, "heads", name));
        }

        public void UpdateHead(string name, string pointer)
        {
            if (!HeadExists(name))
                throw new InvalidOperationException("Head does not exist");

            File.WriteAllText(SysIO.Path.Combine(Path, "heads", name), pointer);
        }

        public bool HeadExists(string name)
        {
            return GetHeads ().Any(a => a.ToLower () == name.ToLower ());
        }

        public IBackup GetBackup(string identifier)
        {
            var hash = ResolveIdentifier(identifier);

            if (hash == null) return null;

            return new ZipBackup(SysIO.Path.Combine(Path, "data", hash));
        }

        public IEnumerable<string> GetBackupNames()
        {
            return Directory.EnumerateFiles(SysIO.Path.Combine(Path, "data")).Select(SysIO.Path.GetFileNameWithoutExtension);
        }


        public string ResolveIdentifier(string identifier)
        {
            var pointer =
               GetHeads().Where(x => string.Compare(x, identifier, StringComparison.InvariantCultureIgnoreCase) == 0).Select(a=>File.ReadAllText(SysIO.Path.Combine(Path, "heads", a)));
            var p = pointer.SingleOrDefault();

            if (p != null)
                return ResolveIdentifier(p);

            var hashes = GetBackupNames().Where(x => x.StartsWith(identifier, StringComparison.InvariantCultureIgnoreCase));
            try
            {
                return hashes.SingleOrDefault();
            }
            catch (InvalidOperationException)
            {
                return null; // multiple entries
            }
        }


        public IBackupPrototype CreateBackup()
        {
            var path = SysIO.Path.Combine(Path, "temp");
            Directory.CreateDirectory(path);

            var filePath = SysIO.Path.Combine(path, _random.Next ().ToString (CultureInfo.InvariantCulture));

            return new ZipBackupPrototype(filePath, this);
        }

        public static void Init(string path)
        {
            Directory.CreateDirectory(SysIO.Path.Combine(path, "temp"));
            Directory.CreateDirectory(SysIO.Path.Combine(path, "data"));
            Directory.CreateDirectory(SysIO.Path.Combine(path, "heads"));

            var rep = new ZipBackupRepository(path);

            using (var proto = rep.CreateBackup ())
            {

                proto.SetDescription("Init");

                proto.Save ();

                rep.AddHead("HEAD", proto.Name);
            }
        }
    }
}
