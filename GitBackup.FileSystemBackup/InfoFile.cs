using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace GitBackup.FileSystemBackup
{
    [Serializable]
    public class InfoFile
    {
        public HashSet<string> DeletedFiles { get; set; }
        public string ParentName { get; set; }
        public string Comment { get; set; }
        public string Issuer { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsFullBackup { get; set; }

        public static InfoFile Read(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(InfoFile));
            return (InfoFile)serializer.Deserialize(stream);
        }

        public void Save(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(InfoFile));
            serializer.Serialize(stream, this);
        }

    }
}
