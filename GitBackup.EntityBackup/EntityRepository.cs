using System;
using System.IO;
using GitBackup.EntityBackup.Entities;
using System.Collections.Generic;
using System.Linq;

namespace GitBackup.EntityBackup
{
    public class EntityRepository : IBackupRepository
    {
        private readonly SqlContext _context;

        internal string NameOrConnectionString { get; set; }

        public EntityRepository()
        {
            _context = new SqlContext ();
        }

        public EntityRepository(string nameOrConnectionString)
        {
            _context = new SqlContext(NameOrConnectionString = nameOrConnectionString);
        }

        public IEnumerable<string> GetHeads()
        {
            return _context.Heads.Select(a => a.Name).ToArray ();
        }

        public void AddHead(string name, string pointer)
        {
            var head = new Head {Name = name, Location = pointer};
            _context.Heads.Add(head);

            _context.SaveChanges ();
        }

        public void RemoveHead(string name)
        {
            _context.Heads.Remove(_context.Heads.Single(a => a.Name == name));

            _context.SaveChanges();
        }

        public void UpdateHead(string name, string pointer)
        {
            var head = _context.Heads.Single(a => a.Name == name);
            head.Location = pointer;

            _context.SaveChanges();
        }

        private string GetHeadValue(string name)
        {
            return _context.Heads.Single(a => a.Name == name).Location;
        }

        public bool HeadExists(string name)
        {
            return _context.Heads.Any(a => a.Name == name);
        }

        public IBackup GetBackup(string identifier)
        {
            identifier = ResolveIdentifier(identifier);

            if(identifier == null)
                throw new FileNotFoundException();

            return new EntityBackup(this, _context.Backups.Single(a => a.Name == identifier).BackupId);
        }

        private IEnumerable<string> GetBackupNames()
        {
            return _context.Backups.Select(a => a.Name);
        }

        public string ResolveIdentifier(string identifier)
        {
            var pointer =
               GetHeads().Where(x => string.Compare(x, identifier, StringComparison.InvariantCultureIgnoreCase) == 0).Select(GetHeadValue);
            var p = pointer.SingleOrDefault();

            if (p != null)
                return ResolveIdentifier(p);

            var hashes = from x in GetBackupNames ()
                         where x.StartsWith(identifier, StringComparison.InvariantCultureIgnoreCase)
                         select x;
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
            return new EntityBackupPrototype(this);
        }
    }
}
