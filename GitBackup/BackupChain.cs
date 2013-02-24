using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GitBackup
{
    internal class BackupChain : IBackupChain
    {
        public IBackup FirstBackup { get; private set; }
        internal bool FollowAllLinks { get; set; }

        public BackupChain(IBackup firstBackup, bool followAllLinks = false)
        {
            FirstBackup = firstBackup;
            FollowAllLinks = followAllLinks;
        }

        public IEnumerable<string> GetFiles()
        {
            var files = new HashSet<string>();
            var deletedFiles = new HashSet<string>();

            foreach (var backup in this)
            {
                foreach (var addedFile in backup.GetAddedOrChangedFiles ().Where(addedFile => !deletedFiles.Contains(addedFile)))
                {
                    files.Add(addedFile);
                }
                foreach (var deletedFile in backup.GetDeletedFiles ())
                {
                    deletedFiles.Add(deletedFile);
                }
            }
            return files;
        }

        public Stream OpenFile(string name)
        {
            var deletedFiles = new HashSet<string>();

            foreach (var backup in this)
            {
                foreach (var addedFile in backup.GetAddedOrChangedFiles ()
                    .Where(addedFile => !deletedFiles.Contains(addedFile))
                    .Where(addedFile => addedFile.ToLower () == name.ToLower ()))
                {
                    return backup.OpenFile(addedFile);
                }
                foreach (var deletedFile in backup.GetDeletedFiles())
                {
                    deletedFiles.Add(deletedFile);
                }
            }

            throw new FileNotFoundException();
        }

        public IEnumerator<IBackup> GetEnumerator()
        {
            return new BackupChainEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new BackupChainEnumerator(this);
        }

        class BackupChainEnumerator : IEnumerator<IBackup>
        {
            private readonly BackupChain _chain;
            private bool _reset = true;

            public BackupChainEnumerator(BackupChain chain)
            {
                _chain = chain;
                Reset ();
            }

            public IBackup Current { get; private set; }

            public void Dispose()
            {
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                if (_reset)
                {
                    Current = _chain.FirstBackup;
                    _reset = false;
                    return true;
                }
                if ((Current.IsFullBackup && _chain.FollowAllLinks ? Current.SymbolicFullBackupParent : Current.ParentBackup) == null)
                {
                    Current = null;
                    return false;
                }
                Current = Current.IsFullBackup && _chain.FollowAllLinks ? Current.SymbolicFullBackupParent : Current.ParentBackup;
                return true;
            }

            public void Reset()
            {
                Current = null;
                _reset = true;
            }
        }


        public string GetFileHash(string path)
        {
            var deletedFiles = new HashSet<string>();

            foreach (var backup in this)
            {
                foreach (var addedFile in backup.GetAddedOrChangedFiles()
                    .Where(addedFile => !deletedFiles.Contains(addedFile))
                    .Where(addedFile => addedFile.ToLower() == path.ToLower()))
                {
                    return backup.GetFileHash(addedFile);
                }
                foreach (var deletedFile in backup.GetDeletedFiles())
                {
                    deletedFiles.Add(deletedFile);
                }
            }

            throw new FileNotFoundException();
        }


        public long GetFileSize(string path)
        {
            var deletedFiles = new HashSet<string>();

            foreach (var backup in this)
            {
                foreach (var addedFile in backup.GetAddedOrChangedFiles()
                    .Where(addedFile => !deletedFiles.Contains(addedFile))
                    .Where(addedFile => addedFile.ToLower() == path.ToLower()))
                {
                    return backup.GetFileSize(addedFile);
                }
                foreach (var deletedFile in backup.GetDeletedFiles())
                {
                    deletedFiles.Add(deletedFile);
                }
            }

            throw new FileNotFoundException();
        }
    }
}
