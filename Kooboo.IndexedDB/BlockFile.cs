//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB
{
    public class BlockFile
    {

        internal string _fullfilename;
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

        internal byte[] GetPartial(long position, int offset, int count)
        {
            byte[] partial = new byte[count]; 
            if (Stream.Length >= position + offset + count)
            {
                Stream.Position = position + offset;
                Stream.Read(partial, 0, count);
                return partial;
            }
            return null;
        }

        private async Task<byte[]> GetPartialAsync(long position, int offset, int count)
        {
            byte[] partial = new byte[count];
        
            if (Stream.Length >= position + offset + count)
            {
                Stream.Position = position + offset;
                await Stream.ReadAsync(partial, 0, count);
                return partial;
            }
            return null;
        }

        // keep for upgrade.. not used any more. 
        public byte[] GetContent(long position, int KeyColumnOffset)
        {
            byte[] counterbytes = GetPartial(position, 26, 4);
            int counter = BitConverter.ToInt32(counterbytes, 0);
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

            Int64 currentposition;
            currentposition = Stream.Length;
            Stream.Position = currentposition;
            Stream.Write(header, 0, 10);
            Stream.Write(bytes, 0, TotalByteLen);
            return currentposition;

        }

        public byte[] Get(long position)
        {
            byte[] counterbytes = GetPartial(position, 2, 4);
            if (counterbytes == null)
            {
                return null;
            }
            int counter = BitConverter.ToInt32(counterbytes, 0);
            return GetPartial(position, 10, counter);
        }

        public async Task<byte[]> GetAsync(long position)
        {
            byte[] counterbytes = GetPartial(position, 2, 4);
            if (counterbytes == null)
            {
                return null;
            }
            int counter = BitConverter.ToInt32(counterbytes, 0);
            return await GetPartialAsync(position, 10, counter);
        }


        public int GetLength(long position)
        {
            byte[] counterbytes = GetPartial(position, 2, 4);
            int counter = BitConverter.ToInt32(counterbytes, 0);
            return counter;
        }

        public byte[] Get(long position, int ColumnLen)
        {
            return GetPartial(position, 10, ColumnLen);
        }

        public byte[] GetCol(long position, int relativePos, int len)
        {
            if (len > 0)
            {
                return GetPartial(position, relativePos + 10 + 8, len);
            }
            else
            {
                // TODO: This should not needed.... 
            }
            return null;
        }

        public void UpdateCol(long position, int relativeposition, int length, byte[] values)
        {
            this.Stream.Position = position + 10 + relativeposition + 8;
            this.Stream.Write(values, 0, length);
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

        public void DelSelf()
        {
            lock (_object)
            {
                this.Close();
                if (System.IO.File.Exists(this._fullfilename))
                {
                    System.IO.File.Delete(this._fullfilename);
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
