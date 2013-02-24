using System.Linq;
using GitBackup.Test.Helpers;
using NUnit.Framework;
using System;
using System.IO;

namespace GitBackup.Test
{
    [Category("GitBackup.dll")]
    public class BackupChainTest
    {
        [Test(Description = "")]
        public void TestBackupChain()
        {
            var testBackup1 = new TestBackup("Backup 1", addedOrChangedItems: new[] {"./file1.txt"});
            var testBackup2 = new TestBackup("Backup 2", parent: testBackup1, addedOrChangedItems: new[] {"./file2.txt"},
                                             deletedItems: new[] {"./file1.txt"});

            var chain = testBackup2.GetChain ();

            var chain2 = ((TestBackup) null).GetChain ();

            Assert.IsNotNull(chain2);

            Assert.AreEqual(chain.Count(), 2);

            Assert.AreEqual(chain.FirstBackup, testBackup2);

            Assert.Throws<FileNotFoundException>(() => chain.OpenFile("./file1.txt"));
            Assert.Throws<FileNotFoundException>(() => chain.GetFileSize("./file1.txt"));
            Assert.Throws<FileNotFoundException>(() => chain.GetFileHash("./file1.txt"));
            Assert.IsTrue(chain.OpenFile("./file2.txt") is MemoryStream);

            var items = chain.GetFiles ();

            Assert.IsNotNull(items);

            var enumerable = items as string[] ?? items.ToArray ();

            Assert.AreEqual(enumerable.Count(), 1);

            Assert.AreEqual(enumerable.First(), "./file2.txt");
        }
    }
}