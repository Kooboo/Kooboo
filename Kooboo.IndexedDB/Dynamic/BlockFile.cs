//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;

namespace Kooboo.IndexedDB.Dynamic
{

    public class BlockFile
    {

        private string _fullfilename;
        private FileStream _filestream;

        private object _object = new object();

        public BlockFile(string fullfilename)
        {
            this._fullfilename = fullfilename;
        }

        public void OpenOrCreate()
        {
            if (!File.Exists(this._fullfilename))
            {
                // file not exists.first check directory exists or not.
                string dirname = Path.GetDirectoryName(this._fullfilename);
                if (!System.IO.Directory.Exists(dirname))
                {
                    System.IO.Directory.CreateDirectory(dirname);
                }

                File.WriteAllText(this._fullfilename, "block content file, do not modify\r\n");
            }
        }

        private byte[] GetPartial(long position, int offset, int count)
        {

            byte[] partial = new byte[count];
            Stream.Position = position + offset;
            Stream.Read(partial, 0, count);
            return partial;

        }

        // keep for upgrade.. not used any more. 
        public byte[] GetContent(long position, int KeyColumnOffset)
        {
            byte[] counterBytes = GetPartial(position, 26, 4);
            int counter = BitConverter.ToInt32(counterBytes, 0);
            return GetPartial(position, 30 + KeyColumnOffset, counter);
        }

        public byte[] GetKey(long position, int ColumnOffset, int KeyLength)
        {
            return GetPartial(position, 30 + ColumnOffset, KeyLength);
        }

        #region  NewAPI

        public long Add(byte[] bytes, int TotalByteLen)
        {
            byte[] header = new byte[10];
            header[0] = 10;
            header[1] = 13;
            System.Buffer.BlockCopy(BitConverter.GetBytes(TotalByteLen), 0, header, 2, 4);

            int tolerance = TotalByteLen * 2;
            System.Buffer.BlockCopy(BitConverter.GetBytes(tolerance), 0, header, 6, 4);

            byte[] total = new byte[tolerance];

            System.Buffer.BlockCopy(bytes, 0, total, 0, TotalByteLen);

            Int64 currentPosition;
            currentPosition = Stream.Length;
            Stream.Position = currentPosition;
            Stream.Write(header, 0, 10);
            Stream.Write(total, 0, tolerance);

            return currentPosition;
        }

        public void UpdateBlock(byte[] bytes, long blockPosition)
        {
            byte[] counter = BitConverter.GetBytes(bytes.Length);

            Stream.Position = blockPosition + 2;

            Stream.Write(counter, 0, 4);

            Stream.Position = blockPosition + 10;
            Stream.Write(bytes, 0, bytes.Length);

        }


        public byte[] Get(long position)
        {
            byte[] counterBytes = GetPartial(position, 2, 4);
            int counter = BitConverter.ToInt32(counterBytes, 0);
            return GetPartial(position, 10, counter);
        }

        public int GetTolerance(long position)
        {
            byte[] counterBytes = GetPartial(position, 6, 4);
            return BitConverter.ToInt32(counterBytes, 0);
        }



        public byte[] Get(long position, int ColumnLen)
        {
            return GetPartial(position, 10, ColumnLen);
        }

        public byte[] GetCol(long position, int relativePos, int len)
        {
            if (relativePos == int.MaxValue)
            {
                throw new Exception("Non supported column");
            }

            if (len > 0 && relativePos >= 0)
            {
                if (len == int.MaxValue)
                {
                    byte[] header = GetPartial(position, relativePos + 10, 8);
                    int counter = BitConverter.ToInt32(header, 4);
                    if (counter > 0)
                    {
                        return GetPartial(position, relativePos + 10 + 8, counter);
                    }
                }
                else
                {
                    return GetPartial(position, relativePos + 10 + 8, len);
                }
            }
            return null;
        }

        public bool UpdateCol(long position, int relativeposition, int length, byte[] values)
        {
            var currentbytes = this.GetCol(position, relativeposition, length);

            var hashone = Helper.KeyHelper.ComputeGuid(currentbytes);

            var hashnew = Helper.KeyHelper.ComputeGuid(values);

            if (hashone == hashnew)
            {
                return false;
            }

            this.Stream.Position = position + 10 + relativeposition + 8;
            this.Stream.Write(values, 0, length);
            return true;
        }

        #endregion
        public void Close()
        {
            if (_filestream != null)
            {
                lock (_object)
                {
                    if (_filestream != null)
                    {
                        _filestream.Close();
                        _filestream = null;
                    }
                }
            }
        }

        public void Flush()
        {
            if (_filestream != null)
            {
                lock (_object)
                {
                    _filestream.Flush();
                }
            }
        }

        public FileStream Stream
        {

            get
            {

                if (_filestream == null || !_filestream.CanRead)
                {
                    lock (_object)
                    {
                        if (_filestream == null || !_filestream.CanRead)
                        {
                            this.OpenOrCreate();
                            _filestream = StreamManager.GetFileStream(this._fullfilename);
                        }
                    }
                }
                return _filestream;
            }
        }

    }

}
