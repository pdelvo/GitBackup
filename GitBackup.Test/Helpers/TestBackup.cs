using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace GitBackup.Test.Helpers
{
    public class TestBackup : IBackup
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Creator { get; private set; }
        public DateTime CreationDate { get; private set; }
        public IBackup ParentBackup { get; private set; }
        public bool IsFullBackup { get; private set; }
        public IBackup SymbolicFullBackupParent { get; private set; }
        public bool IsDeleted { get; private set; }
        public bool IsDisposed { get; private set; }

        private string[] _addedOrChangedItems;
        private string[] _deletedItems; 

        public TestBackup(string name, 
            TestBackup parent = null, 
            bool isFullBackup = false,
            string[] addedOrChangedItems = null,
            string[] deletedItems = null)
        {
            Name = name;
            Description = "Description of " + name;
            Creator = "Creator of " + name;
            CreationDate = DateTime.Now;
#pragma warning disable 665
            if (IsFullBackup = isFullBackup)
#pragma warning restore 665
                SymbolicFullBackupParent = parent;
            else
                ParentBackup = parent;

            _addedOrChangedItems = addedOrChangedItems ?? new[]
                                                              {
                                                                  "./test1.txt",
                                                                  "./test2.txt",
                                                                  "./test3.txt",
                                                                  "./test4.txt",
                                                                  "./test5.txt",
                                                                  "./testdir1/test1.txt",
                                                                  "./testdir1/test2.txt",
                                                                  "./testdir1/test3.txt",
                                                                  "./testdir1/test4.txt",
                                                                  "./testdir1/test5.txt",
                                                                  "./testdir2/test1.txt",
                                                                  "./testdir2/test2.txt",
                                                                  "./testdir2/test3.txt",
                                                                  "./testdir2/test4.txt",
                                                                  "./testdir2/test5.txt"
                                                              };
            _deletedItems = deletedItems ?? new[]
                                                              {
                                                                  "./deltest1.txt",
                                                                  "./deltest2.txt",
                                                                  "./deltest3.txt",
                                                                  "./deltest4.txt",
                                                                  "./deltest5.txt",
                                                                  "./deltestdir1/test1.txt",
                                                                  "./deltestdir1/test2.txt",
                                                                  "./deltestdir1/test3.txt",
                                                                  "./deltestdir1/test4.txt",
                                                                  "./deltestdir1/test5.txt",
                                                                  "./deltestdir2/test1.txt",
                                                                  "./deltestdir2/test2.txt",
                                                                  "./deltestdir2/test3.txt",
                                                                  "./deltestdir2/test4.txt",
                                                                  "./deltestdir2/test5.txt"
                                                              };
        }

        public IEnumerable<string> GetAddedOrChangedFiles()
        {
            return _addedOrChangedItems;
        }

        public IEnumerable<string> GetDeletedFiles()
        {
            return _deletedItems;
        }

        public string GetFileHash(string path)
        {
            return "test:" + path;
        }

        public System.IO.Stream OpenFile(string name)
        {
            return new MemoryStream();
        }

        public void Delete()
        {
            IsDeleted = true;
        }

        public long GetFileSize(string path)
        {
            return 0;
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
