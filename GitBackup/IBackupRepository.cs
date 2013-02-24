using System.Collections.Generic;

namespace GitBackup
{
    public interface IBackupRepository
    {
        IEnumerable<string> GetHeads();

        void AddHead(string name, string pointer);

        void RemoveHead(string name);

        void UpdateHead(string name, string pointer);

        bool HeadExists(string name);

        IBackup GetBackup(string identifier);

        string ResolveIdentifier(string identifier);

        IBackupPrototype CreateBackup();
    }
}
