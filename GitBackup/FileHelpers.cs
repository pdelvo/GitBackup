using System;
using System.IO;
using System.Linq;

namespace GitBackup
{
    public static class FileHelpers
    {
        public static string MakeRelative(string filePath, string rootPath)
        {
            filePath = filePath.ToLower().Replace('\\', '/');
            rootPath = rootPath.ToLower().Replace('\\', '/');

            if(!filePath.StartsWith(rootPath)) throw new InvalidOperationException("file path is not located in root path");

            var path = filePath.Substring(rootPath.Length);

            return path.StartsWith("/") ? "." + path : "./" + path;
        }
        public static string CalculateHash(string path)
        {
            var inFileInfo = new FileInfo(path);
            using (var inStream = new BufferedStream(new FileStream(inFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), 1200000))
            {
                return CalculateHash (inStream);
            }
        }

        public static string CalculateHash(Stream inStream)
        {
            var provider = new System.Security.Cryptography.MD5CryptoServiceProvider ();
            var hashBytes = provider.ComputeHash(inStream);
            return hashBytes.Aggregate("", (current, inByte) => current + inByte.ToString("X2"));
        }
    }
}
