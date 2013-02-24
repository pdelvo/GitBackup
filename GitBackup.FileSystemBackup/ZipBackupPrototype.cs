using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;

namespace GitBackup.FileSystemBackup
{
    internal class ZipBackupPrototype : ZipBackup, IBackupPrototype
    {
        private ZipBackupRepository _repository;

        private ZipOutputStream _outputStream;

        static Random _random = new Random();

        public ZipBackupPrototype(string path, ZipBackupRepository repository)
        {
            _repository = repository;

            FilePath = path;

            CreateTemporaryFile ();
        }

        private void CreateTemporaryFile()
        {
            _outputStream = new ZipOutputStream(File.Create(FilePath));
            _outputStream.SetLevel(0);
            Info = new InfoFile
            {
                CreationDate = DateTime.Now,
                DeletedFiles = new HashSet<string>()
            };
        }


        private void SaveInfo()
        {
                if (Info == null)
                {
                    var infoFile = new InfoFile
                                       {
                                           CreationDate = DateTime.Now
                                       };

                    Info = infoFile;
                }

                _outputStream.PutNextEntry(new ZipEntry("info.xml"));
                Info.Save(_outputStream);
            _outputStream.CloseEntry ();
        }

        public void SetDescription(string description)
        {
            Info.Comment = description;
        }

        public void SetCreator(string creator)
        {
            Info.Issuer = creator;
        }

        public void SetCreationDate(DateTime creationDate)
        {
            Info.CreationDate = creationDate;
        }

        public void SetParentBackup(IBackup parentBackup)
        {
            if (!(parentBackup is ZipBackup))
                throw new InvalidOperationException(
                    "Parent backup must be a zip backup too and must be from the same repository");
            if (parentBackup is IBackupPrototype)
                throw new InvalidOperationException("Parent backup must be saved first");

            Info.ParentName = parentBackup.Name;
        }

        public void Save()
        {
            SaveInfo ();

            _outputStream.Finish ();
            _outputStream.Close ();

            var hash = FileHelpers.CalculateHash(FilePath);

            File.Move(FilePath, FilePath = Path.Combine(_repository.Path, "data", hash));
        }

        public void AddFileDeletion(string relativePath)
        {
            Info.DeletedFiles.Add(relativePath);
        }

        /// <summary>
        /// Creates a new backup file
        /// </summary>
        /// <param name="path">The virtual path</param>
        /// <returns></returns>
        public Stream CreateFile(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (!path.StartsWith("./"))
                throw new ArgumentOutOfRangeException("Argument 'path' is out of range.",
                    new FormatException("The path must begin with './"));


            var tempFolderPath = Path.Combine(_repository.Path, "temp");
            Directory.CreateDirectory(tempFolderPath);
            string tempPath;

            do
            {
               tempPath = Path.Combine(tempFolderPath, _random.Next().ToString(CultureInfo.InvariantCulture));
                
            } while (File.Exists(tempPath));

            var stream = new CloseEventStream(new FileStream(tempPath, FileMode.Create, FileAccess.Write));

            stream.Closed += delegate
                                 {
                                     var actualPath = "data" + path.Substring(1);


                                     actualPath += "." + FileHelpers.CalculateHash(tempPath);

                                     var entry = new ZipEntry(actualPath);

                                     _outputStream.PutNextEntry(entry);
                                     using (var sourceStream = File.OpenRead(tempPath))
                                     {
                                         sourceStream.CopyTo(_outputStream);
                                     }
                                     _outputStream.CloseEntry ();

                                     File.Delete(tempPath);
                                 };
            return stream;
        }

        public void SetFullBackup(bool isFullBackup)
        {
            Info.IsFullBackup = isFullBackup;
        }
    }
}
