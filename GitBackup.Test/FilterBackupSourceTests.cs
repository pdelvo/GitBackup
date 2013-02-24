using System.IO;
using System.Linq;
using GitBackup.Test.Helpers;
using NUnit.Framework;
using System;
namespace GitBackup.Test
{
    [Category("GitBackup.dll")]
    public class FilterBackupSourceTests
    {
        [Test(
            Description = "Tests if the filter backup source throws an argument exception when the backup source is null"
            )]
        public void TestFilterBackupSourceThrowsOnNullBackuoSource()
        {
            Assert.Throws<ArgumentNullException>(() => new FilterBackupSource(null, a => true));
        }

        [Test(
            Description = "Tests if the filter backup source throws an argument exception when the filter is null"
            )]
        public void TestFilterBackupSourceThrowsOnNullFilter()
        {
            Assert.Throws<ArgumentNullException>(() => new FilterBackupSource(new TestBackupSource(), null));
        }

        [Test(Description = "Tests if the filter backup source works correctly")]
        public void TestFilterBackupSource()
        {
            var source = new TestBackupSource();

            var filteredSource = new FilterBackupSource(source, item => item.RelativePath.StartsWith("./testdir1"));

            foreach (var backupSourceFile in filteredSource.EnumerateFiles())
            {
                Assert.IsTrue(backupSourceFile.RelativePath.StartsWith("./testdir1"));
            }
        }

        [Test(Description = "Tests if the filter backup source works correctly")]
        public void TestFilterBackupDoesNotHandleFilterException()
        {
            var source = new TestBackupSource();

            var filteredSource = new FilterBackupSource(source, item =>
                                                                    {
                                                                        throw new InvalidDataException();
                                                                    });
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<InvalidDataException>(() => filteredSource.EnumerateFiles ().ToArray ());
// ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }
    }
}
