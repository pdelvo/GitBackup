using System;
using System.Diagnostics;

namespace GitBackup
{
    public static class Extensions
    {
        public static IBackupChain GetChain(this IBackup backup, bool followAllLinks = false)
        {
            if (backup == null)
                return new EmptyBackupChain();
            return new BackupChain(backup, followAllLinks);
        }

        public static void ApplyPatch(this Patch patch, IBackupPrototype prototype)
        {
            if (patch == null) throw new ArgumentNullException("patch");
            if (prototype == null) throw new ArgumentNullException("prototype");

            Trace.WriteLine("Applying Patch", "Patch");

            foreach (var addedOrChangedFile in patch.AddedOrChangedFiles)
            {
                try
                {
                    Trace.WriteLine("Copying " + addedOrChangedFile.RelativePath, "Patch");
                    using (var destinationStream = prototype.CreateFile(addedOrChangedFile.RelativePath))
                    using (var sourceStream = addedOrChangedFile.Open ())
                    {
                        sourceStream.CopyTo(destinationStream);
                    }
                }
                catch(Exception ex)
                {
                    Trace.WriteLine("Could not copy file" + Environment.NewLine + ex, "Patch");
                }
            }
            foreach (var removedFile in patch.RemovedFiles)
            {
                prototype.AddFileDeletion(removedFile);
            }

            Trace.WriteLine("Deletions added", "Patch");
        }

        public static void ApplyPatch(this IBackupPrototype prototype, Patch patch)
        {
            patch.ApplyPatch(prototype);
        }
    }
}
