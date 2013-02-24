using System.IO;

namespace GitBackup
{
    public interface IBackupSourceFile
    {
        string RelativePath { get; }
        string GetFileHash();
        Stream Open();
        long GetFileSize();
    }
}
