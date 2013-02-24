using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GitBackup
{
    public class Patch
    {
        protected Patch(IBackupSource source, IBackupChain target)
        {
            Source = source;
            Target = target;
        }

        public IBackupSource Source { get; protected set; }

        public IBackupChain Target { get; protected set; }

        public IEnumerable<IBackupSourceFile> AddedOrChangedFiles { get; protected set; }

        public IEnumerable<string> RemovedFiles { get; protected set; }

        public static Patch Compare(IBackupSource source, IBackupChain target)
        {
            Trace.WriteLine("Comparing source to destination", "Patch");

            var result = new Patch(source, target);

            var sourceFiles = source.EnumerateFiles ().ToArray ();

            result.RemovedFiles = target.GetFiles().Where(file => sourceFiles.All(a => a.RelativePath.ToLower() != file.ToLower()));

            result.AddedOrChangedFiles =
                sourceFiles.Where(a => !target.GetFiles().Select(m => m.ToLower()).Contains(a.RelativePath.ToLower()) || target.GetFileSize(a.RelativePath) != a.GetFileSize() || !HashMatches(target, a)).AsParallel ().ToArray ();
            return result;
        }

        private static bool HashMatches(IBackupChain target, IBackupSourceFile a)
        {
            var sourceHash = a.GetFileHash();
            var targetHash = target.GetFileHash(a.RelativePath);
            return sourceHash == targetHash;
        }

        public static Patch CreateFromSource(IBackupSource source)
        {
            return new Patch(source, new EmptyBackupChain ())
                            {
                                RemovedFiles = new string[0],
                                AddedOrChangedFiles = source.EnumerateFiles ()
                            };
        }
    }
}
