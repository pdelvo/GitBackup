using NUnit.Framework;
namespace GitBackup.Test
{
    public class HelperTests
    {
        [Test(Description = "Tests if the VerifyHeadName method provide valid results")]
        [Category("GitBackup.dll")]
        public void TestHeadNameVerification()
        {
            Assert.IsFalse(GitBackupHelper.VerifyHeadName(null));
            Assert.IsFalse(GitBackupHelper.VerifyHeadName(""));
            Assert.IsFalse(GitBackupHelper.VerifyHeadName("1abc"));
            Assert.IsFalse(GitBackupHelper.VerifyHeadName("1"));
            Assert.IsFalse(GitBackupHelper.VerifyHeadName("_abc"));
            Assert.IsFalse(GitBackupHelper.VerifyHeadName("(abc)"));


            Assert.IsTrue(GitBackupHelper.VerifyHeadName("anc"));
            Assert.IsTrue(GitBackupHelper.VerifyHeadName("ab3c"));
            Assert.IsTrue(GitBackupHelper.VerifyHeadName("ab3"));
            Assert.IsTrue(GitBackupHelper.VerifyHeadName("a_cde"));
            Assert.IsTrue(GitBackupHelper.VerifyHeadName("abc_"));
            Assert.IsTrue(GitBackupHelper.VerifyHeadName("this_is_a_great_test"));
        }
    }
}
