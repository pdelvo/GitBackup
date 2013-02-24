using System.Collections.Generic;
using System.IO;

namespace GitBackup
{
    public interface IBackupChain : IEnumerable<IBackup>
    {
        IBackup FirstBackup { get; }

        IEnumerable<string> GetFiles();

        string GetFileHash(string path);

        Stream OpenFile(string name);

        long GetFileSize(string path);
    }
}
