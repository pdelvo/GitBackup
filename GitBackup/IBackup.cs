using System;
using System.Collections.Generic;
using System.IO;

namespace GitBackup
{
    public interface IBackup : IDisposable
    {
        string Name { get; }
        string Description { get; }
        string Creator { get; }
        DateTime CreationDate { get; }
        IBackup ParentBackup { get; }
        bool IsFullBackup { get; }
        IBackup SymbolicFullBackupParent { get; }

        IEnumerable<string> GetAddedOrChangedFiles();

        IEnumerable<string> GetDeletedFiles();

        string GetFileHash(string path);

        Stream OpenFile(string name);

        void Delete();

        long GetFileSize(string path);
    }
}
