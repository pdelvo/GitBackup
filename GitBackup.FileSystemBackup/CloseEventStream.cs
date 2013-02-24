using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GitBackup.FileSystemBackup
{
    internal class CloseEventStream : Stream
    {
        protected Stream BaseStream { get; set; }

        public event EventHandler Closed;

        public CloseEventStream(Stream stream)
        {
            BaseStream = stream;
        }

        public override bool CanRead
        {
            get { return BaseStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return BaseStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return BaseStream.CanWrite; }
        }

        public override void Flush()
        {
            BaseStream.Flush ();
        }

        public override long Length
        {
            get { return BaseStream.Length; }
        }

        public override long Position
        {
            get { return BaseStream.Position; }
            set { BaseStream.Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return BaseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            BaseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            BaseStream.Write(buffer, offset, count);
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return BaseStream.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return BaseStream.BeginWrite(buffer, offset, count, callback, state);
        }

        public override bool CanTimeout
        {
            get { return BaseStream.CanTimeout; }
        }

        public override void Close()
        {
            BaseStream.Close();

            if (Closed != null)
                Closed(this, EventArgs.Empty);
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return BaseStream.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return BaseStream.EndRead(asyncResult);
        }
        public override void EndWrite(IAsyncResult asyncResult)
        {
            BaseStream.EndWrite(asyncResult);
        }
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return BaseStream.FlushAsync(cancellationToken);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return BaseStream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override int ReadByte()
        {
            return BaseStream.ReadByte();
        }

        public override int ReadTimeout
        {
            get
            {
                return BaseStream.ReadTimeout;
            }
            set
            {
                BaseStream.ReadTimeout = value;
            }
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
        {
            return BaseStream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override void WriteByte(byte value)
        {
            BaseStream.WriteByte(value);
        }

        public override int WriteTimeout
        {
            get
            {
                return BaseStream.WriteTimeout;
            }
            set
            {
                BaseStream.WriteTimeout = value;
            }
        }
    }
}
