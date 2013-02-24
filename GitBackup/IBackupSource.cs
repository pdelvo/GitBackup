using System.Collections.Generic;

namespace GitBackup
{
    public interface IBackupSource
    {
        IEnumerable<IBackupSourceFile> EnumerateFiles();
    }
}
