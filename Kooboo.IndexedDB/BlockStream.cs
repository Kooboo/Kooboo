//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;


namespace Kooboo.IndexedDB
{
    /// <summary>
    ///this is for like image store, that can open a new stream to read the image content. 
    /// </summary>
    public class BlockStream : Stream
    {
        private long _start;
        private long _end;
        private long _length;

        private string _blockFilePath;

        private FileStream _stream;

        private FileStream Stream
        {
            get
            {
                if (_stream == null)
                {
                    _stream = StreamManager.GetFileStream(this._blockFilePath);
                }
                return _stream;
            }
        }

        public BlockStream(string blockFilePath, long start, int length)
        {
            _blockFilePath = blockFilePath;
            _start = start;
            _end = _start + length;
            _length = length;

            this.Stream.Position = _start;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override long Position
        {
            get
            {
                return this.Stream.Position - _start;
            }
            set
            {
                this.Stream.Position = value + _start;
            }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            // do nothing. 
        }

        public override long Length
        {
            get { return _length; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (Position == Length)
            {
                return 0;
            }

            if ((count + offset) > this.Length)
            {
                count = Convert.ToInt32(this.Length - offset);
            }

            return this.Stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            offset = offset + _start;

            return this.Stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this.Stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
