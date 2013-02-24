using System.Data.Objects.DataClasses;
using System.Linq;
using GitBackup.EntityBackup.Entities;
using System;
using System.Collections.Generic;
using System.IO;

namespace GitBackup.EntityBackup
{
    public class EntityBackupPrototype : IBackupPrototype
    {
        private readonly List<string> _fileDeletions;
        private readonly Dictionary<string, MemoryStream> _files; 
        private static readonly Random _random = new Random();
        private readonly EntityRepository _repository;


        public EntityBackupPrototype(EntityRepository repository)
        {
            _files = new Dictionary<string, MemoryStream> ();
            _fileDeletions = new List<string> ();

            _repository = repository;
        }

        public void SetDescription(string description)
        {
            Description = description;
        }

        public void SetCreator(string creator)
        {
            Creator = creator;
        }

        public void SetCreationDate(DateTime creationDate)
        {
            CreationDate = creationDate;
        }

        public void SetParentBackup(IBackup parentBackup)
        {
            if (parentBackup != null && !(parentBackup is EntityBackup))
                throw new InvalidOperationException("Parent backup must be an entity backup");
            ParentBackup = parentBackup;
        }

        public void Save()
        {
            using (var context = _repository.NameOrConnectionString == null ? new SqlContext() : new SqlContext(_repository.NameOrConnectionString))
            {
                var backup = new Backup
                                 {
                                     Name = Name = GetRandomName (),
                                     CreationDate = CreationDate,
                                     Creator = Creator,
                                     Description = Description,
                                     ParentSqlBackup = ParentBackup == null ? null : context.Backups.Single(a => a.Name == ParentBackup.Name),
                                     Blobs = new EntityCollection<Blob> (),
                                     DeletedFiles = new EntityCollection<DeletedFile> ()
                                 };

                foreach (var file in _files)
                {
                    var hashStream = new MemoryStream(file.Value.ToArray ());
                    var blob = new Blob
                                    {
                                        Backup = backup,
                                        Path = file.Key,
                                        Hash = FileHelpers.CalculateHash(hashStream),
                                    };
                    var blobData = new BlobData
                                       {
                                           Blob = blob,
                                           Data = hashStream.ToArray ()
                                       };
                    blob.BlobData = blobData;
                    backup.Blobs.Add(blob);
                    context.Blobs.Add(blob);
                    context.BlobDatas.Add(blobData);
                }
                foreach (var fileDeletion in _fileDeletions)
                {
                    var deletedFile = new DeletedFile
                                          {
                                              Backup = backup,
                                              Path = fileDeletion
                                          };
                    backup.DeletedFiles.Add(deletedFile);
                    context.DeletedFiles.Add(deletedFile);
                }

                context.Backups.Add(backup);

                context.SaveChanges ();
            }
        }

        private string GetRandomName()
        {
            var bytes = new byte[100];
            _random.NextBytes(bytes);
            return FileHelpers.CalculateHash(new MemoryStream(bytes));
        }

        public void AddFileDeletion(string relativePath)
        {
            _fileDeletions.Add(relativePath);
        }

        public Stream CreateFile(string relativePath)
        {
            var memStream = new MemoryStream ();

            _files.Add(relativePath, memStream);

            return memStream;
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Creator { get; set; }

        public System.DateTime CreationDate { get; set; }

        public IBackup ParentBackup { get; set; }

        public IEnumerable<string> GetAddedOrChangedFiles()
        {
            return _files.Keys;
        }

        public IEnumerable<string> GetDeletedFiles()
        {
            return _fileDeletions;
        }

        public string GetFileHash(string path)
        {
            var stream = new MemoryStream(_files[path].ToArray ());
            return FileHelpers.CalculateHash(stream);
        }

        public Stream OpenFile(string name)
        {
            return new MemoryStream(_files[name].ToArray());
        }

        public void Delete()
        {
            //We don't have to do anything here
        }

        public void Dispose()
        {
            //We don't have to do anything here
        }


        public long GetFileSize(string path)
        {
            throw new NotSupportedException();
        }

        public void SetFullBackup(bool isFullBackup)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not supported (always false)
        /// </summary>
        public bool IsFullBackup
        {
            get { return false; }
        }

        public IBackup SymbolicFullBackupParent
        {
            get { throw new NotSupportedException(); }
        }
    }
}
