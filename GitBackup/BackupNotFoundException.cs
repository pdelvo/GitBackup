using System;
using System.Runtime.Serialization;

namespace GitBackup
{
    [Serializable]
    public class BackupNotFoundException : Exception
    {

        public BackupNotFoundException()
        {
        }

        public BackupNotFoundException(string message) : base(message)
        {
        }

        public BackupNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected BackupNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
