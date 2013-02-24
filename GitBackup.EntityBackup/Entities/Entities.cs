using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace GitBackup.EntityBackup.Entities
{
    public class SqlContext :DbContext
    {
        public SqlContext()
        {
            
        }

        public SqlContext(string nameOrConnectionString)
            :base(nameOrConnectionString)
        {
            
        }

        public DbSet<Backup> Backups { get; set; }
        public DbSet<Blob> Blobs { get; set; }
        public DbSet<BlobData> BlobDatas { get; set; }
        public DbSet<DeletedFile> DeletedFiles { get; set; }
        public DbSet<Head> Heads { get; set; }
    }
    public class Head
    {
        public int HeadId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
    }

    public class Backup
    {
        public int BackupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public DateTime CreationDate { get; set; }
        public virtual Backup ParentSqlBackup { get; set; }
        public virtual ICollection<Blob> Blobs { get; set; }
        public virtual ICollection<DeletedFile> DeletedFiles { get; set; }
    }

    public class Blob
    {
        [Key, ForeignKey("BlobData")]
        public int BlobId { get; set; }
        public int BlobDataId { get; set; }
        public string Path { get; set; }
        public string Hash { get; set; }
        public virtual Backup Backup { get; set; }
        public virtual BlobData BlobData { get; set; }
        public long Size { get; set; }
    }

    public class BlobData
    {
        public int BlobDataId { get; set; }
        public int BlobId { get; set; }
        public byte[] Data { get; set; }
        public virtual Blob Blob { get; set; }
    }

    public class DeletedFile
    {
        public int DeletedFileId { get; set; }
        public string Path { get; set; }
        public int BackupId { get; set; }
        public virtual Backup Backup { get; set; }
    }
}