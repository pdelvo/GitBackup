using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace GitBackup.Test.Helpers
{
    class TestBackupSource : IBackupSource
    {
        private ReadOnlyCollection<TestBackupSourceFile> _testFiles;

        public TestBackupSource()
        {
            _testFiles = new ReadOnlyCollection<TestBackupSourceFile> 
                (
                new[]
                    {
                        new TestBackupSourceFile("./test1.txt"),
                        new TestBackupSourceFile("./test2.txt"),
                        new TestBackupSourceFile("./test3.txt"),
                        new TestBackupSourceFile("./test4.txt"),
                        new TestBackupSourceFile("./test5.txt"),
                        new TestBackupSourceFile("./testdir1/test1.txt"),
                        new TestBackupSourceFile("./testdir1/test2.txt"),
                        new TestBackupSourceFile("./testdir1/test3.txt"),
                        new TestBackupSourceFile("./testdir1/test4.txt"),
                        new TestBackupSourceFile("./testdir1/test5.txt"),
                        new TestBackupSourceFile("./testdir2/test1.txt"),
                        new TestBackupSourceFile("./testdir2/test2.txt"),
                        new TestBackupSourceFile("./testdir2/test3.txt"),
                        new TestBackupSourceFile("./testdir2/test4.txt"),
                        new TestBackupSourceFile("./testdir2/test5.txt")
                    }.ToList()
                );
        }

        public IEnumerable<IBackupSourceFile> EnumerateFiles()
        {
            return _testFiles;
        }
    }
    class TestBackupSourceFile : IBackupSourceFile
    {
        public TestBackupSourceFile(string path)
        {
            RelativePath = path;
        }

        public string RelativePath { get; private set; }

        public string GetFileHash()
        {
            return "test";
        }

        public Stream Open()
        {
            return new MemoryStream();
        }

        public long GetFileSize()
        {
            return 0;
        }
    }

}
