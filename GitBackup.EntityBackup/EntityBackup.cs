using System.Collections.Generic;
using System.IO;
using GitBackup.EntityBackup.Entities;
using System.Linq;

namespace GitBackup.EntityBackup
{
    public class EntityBackup : IBackup
    {
        private readonly SqlContext _context;
        private readonly Backup _backup;
        private readonly EntityRepository _repository;

        public EntityBackup(EntityRepository repository, int id)
        {
            _context = (_repository = repository).NameOrConnectionString != null ? new SqlContext(repository.NameOrConnectionString) : new SqlContext ();
            _backup = _context.Backups.Single(a => a.BackupId == id);
        }

        public string Name
        {
            get { return _backup.Name; }
        }

        public string Description
        {
            get { return _backup.Description; }
        }

        public string Creator
        {
            get { return _backup.Creator; }
        }

        public System.DateTime CreationDate
        {
            get { return _backup.CreationDate; }
        }

        public IBackup ParentBackup
        {
            get { return _backup.ParentSqlBackup == null ? null : new EntityBackup(_repository, (int)_backup.ParentSqlBackup.BackupId); }
        }

        public IEnumerable<string> GetAddedOrChangedFiles()
        {
            return _backup.Blobs.ToArray ().Select(a => a.Path);
        }

        public IEnumerable<string> GetDeletedFiles()
        {
            return _backup.DeletedFiles.ToArray().Select(a => a.Path);
        }

        public string GetFileHash(string path)
        {
            return _backup.Blobs.Single(a=>a.Path == path).Hash;
        }

        public System.IO.Stream OpenFile(string name)
        {
            return new MemoryStream(_backup.Blobs.Single(a=>a.Path == name).BlobData.Data);
        }

        public void Delete()
        {
            _context.Backups.Remove(_backup);
            _context.SaveChanges ();
        }

        public void Dispose()
        {
            _context.Dispose ();
        }


        public long GetFileSize(string path)
        {
            return _backup.Blobs.Single(a => a.Path == path).Size;
        }


        public bool IsFullBackup
        {
            get { return ParentBackup == null; }
        }

        public IBackup SymbolicFullBackupParent
        {
            get { throw new System.NotSupportedException(); }
        }
    }
}
