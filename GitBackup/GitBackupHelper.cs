using System.Text.RegularExpressions;
namespace GitBackup
{
    public static class GitBackupHelper
    {
        static readonly Regex HeadNameRegex = new Regex("^[a-zA-Z][0-9a-zA-Z_]{2,}$", RegexOptions.Compiled);

        public static bool VerifyHeadName(string name)
        {
            return !string.IsNullOrEmpty(name) && HeadNameRegex.IsMatch(name);
        }
    }
}
