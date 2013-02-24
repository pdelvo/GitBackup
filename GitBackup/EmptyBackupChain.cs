using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GitBackup
{
    public class EmptyBackupChain : IBackupChain
    {
        public IBackup FirstBackup
        {
            get { return null; }
        }

        public IEnumerable<string> GetFiles()
        {
            return Enumerable.Empty<string> ();
        }

        public string GetFileHash(string path)
        {
            return null;
        }

        public System.IO.Stream OpenFile(string name)
        {
            return null;
        }

        public IEnumerator<IBackup> GetEnumerator()
        {
            return new EmptyBackupChainEnumerator ();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new EmptyBackupChainEnumerator();
        }


        public long GetFileSize(string path)
        {
            throw new FileNotFoundException();
        }
    }

    internal class EmptyBackupChainEnumerator : IEnumerator<IBackup>
    {

        public EmptyBackupChainEnumerator()
        {
        }

        public IBackup Current { get; private set; }

        public void Dispose()
        {
        }

        object IEnumerator.Current
        {
            get { return null; }
        }

        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {
        }
    }
}