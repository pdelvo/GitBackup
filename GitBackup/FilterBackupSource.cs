using System;
using System.Collections.Generic;
using System.Linq;

namespace GitBackup
{
    public class FilterBackupSource : IBackupSource
    {
        private readonly Predicate<IBackupSourceFile> _filter;
        private readonly IBackupSource _source;

        public FilterBackupSource(IBackupSource source, Predicate<IBackupSourceFile> filter)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (filter == null) throw new ArgumentNullException("filter");

            _filter = filter;
            _source = source;
        }

        public IEnumerable<IBackupSourceFile> EnumerateFiles()
        {
            return _source.EnumerateFiles ().Where(a=> _filter(a));
        }
    }
}
