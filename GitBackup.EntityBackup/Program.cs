using System.Linq;
using GitBackup.FileSystemBackup;
using System;
using System.Diagnostics;
using System.IO;
namespace GitBackup.EntityBackup
{
    class Program
    {
        static void Main(string[] args)
        {
            string workingDirectory = @"F:\Documents\Visual Studio 10\Projects\BigIntegerTest";
            string branch = "HEAD";
            string issuer = "pdelvo";
            string comment = "HUHU";

            var backupRepo = new EntityRepository();

            if (!Directory.Exists(workingDirectory))
                throw new DirectoryNotFoundException("Working directory could not be found");

            IBackup backup = null;

            try
            {
                backup = backupRepo.GetBackup(branch);
            }
            catch
            {
            }

            var chain = backup.GetChain ();

            var list = chain.ToList ();

            using (backup)
            using (var prototype = backupRepo.CreateBackup())
            {
                Trace.WriteLine("Creating backup", "File Tool");
                prototype.SetCreationDate(DateTime.Now);
                prototype.SetCreator(issuer);
                prototype.SetDescription(comment);
                prototype.SetParentBackup(backup);

                var source = new DirectoryBackupSource(workingDirectory);

                var patch = Patch.Compare(source, backup.GetChain());

                patch.ApplyPatch(prototype);

                prototype.Save();

                backupRepo.UpdateHead(branch, prototype.Name);
            }
        }
    }
}
