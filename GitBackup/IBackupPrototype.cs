using System;
using System.IO;

namespace GitBackup
{
    public interface IBackupPrototype : IBackup
    {
        void SetFullBackup(bool isFullBackup);
        void SetDescription(string description);
        void SetCreator(string creator);
        void SetCreationDate(DateTime creationDate);
        void SetParentBackup(IBackup parentBackup);
        void Save();


        void AddFileDeletion(string relativePath);
        Stream CreateFile(string relativePath);
    }
}
