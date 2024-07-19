//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB
{
    public class BlockFile
    {

        internal string Fullfilename;
        private FileStream _filestream;

        private object _locker = new object();

        public BlockFile(string fullfilename)
        {
            this.Fullfilename = fullfilename;
        }

        public void OpenOrCreate()
        {
            if (!File.Exists(this.Fullfilename))
            {
                // file not exists.first check directory exists or not.
                string dirname = Path.GetDirectoryName(this.Fullfilename);
                if (!System.IO.Directory.Exists(dirname))
                {
                    System.IO.Directory.CreateDirectory(dirname);
                }

                File.WriteAllText(this.Fullfilename, "block content file, do not modify\r\n");
            }
        }

        internal byte[] GetPartial(long position, int offset, int count)
        {
            if (Stream.Length >= position + offset + count)
            {
                byte[] partial = new byte[count];

                lock (_locker)
                {
                    Stream.Position = position + offset;
                    Stream.Read(partial, 0, count);
                    return partial;
                }

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

        /// <summary>
        /// used by very old database file, this format is not used any more since the change of setting file format. 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="KeyColumnOffset"></param>
        /// <returns></returns>
        public byte[] GetContent(long position, int KeyColumnOffset)
        {
            byte[] counterbytes = GetPartial(position, 26, 4);
            int counter = BitConverter.ToInt32(counterbytes, 0);
            return GetPartial(position, 30 + KeyColumnOffset, counter);
        }


        #region  NewAPI

        public long Add(byte[] bytes, int TotalByteLen)
        {
            byte[] header = new byte[10];
            header[0] = 10;
            header[1] = 13;
            System.Buffer.BlockCopy(BitConverter.GetBytes(TotalByteLen), 0, header, 2, 4);

            lock (_locker)
            {
                Int64 currentposition;
                currentposition = Stream.Length;
                Stream.Position = currentposition;
                Stream.Write(header, 0, 10);
                Stream.Write(bytes, 0, TotalByteLen);
                return currentposition;
            }
        }

        public bool UpdatePart(long diskPosition, byte[] parts)
        {
            var header = this.GetPartial(diskPosition, 0, 10);
            if (header == null)
            {
                return false;
            }
            if (header[0] == 10 && header[1] == 13)
            {
                Stream.Position = diskPosition + 10;
                Stream.Write(parts, 0, parts.Length);
                return true;
            }
            return false;
        }

        public byte[] Get(long position)
        {
            //byte[] counterbytes = GetPartial(position, 2, 4);
            //if (counterbytes == null)
            //{
            //    return null;
            //}
            //int counter = BitConverter.ToInt32(counterbytes, 0);
            //if (counter <= 0)
            //{
            //    return null;
            //}
            //return GetPartial(position, 10, counter);
            int counter = GetLength(position);
            return counter > 0 ? GetPartial(position, 10, counter) : null;
        }

        public async Task<byte[]> GetAsync(long position)
        {
            //byte[] counterbytes = GetPartial(position, 2, 4);
            //if (counterbytes == null)
            //{
            //    return null;
            //}
            //int counter = BitConverter.ToInt32(counterbytes, 0);
            var counter = GetLength(position);
            return counter > 0 ? await GetPartialAsync(position, 10, counter) : null;
        }

        public void Delete(long position)
        {
            var header = this.GetPartial(position, 0, 10);
            if (header != null && header[6] == 0)
            {
                lock (_locker)
                {
                    this.Stream.Position = position + 6;
                    this.Stream.WriteByte(1);
                }
            }

        }

        public int GetLength(long position)
        {
            byte[] counterbytes = GetPartial(position, 0, 10);

            if (counterbytes != null && counterbytes[0] == 10 && counterbytes[1] == 13 && counterbytes[6] == 0)
            {
                return BitConverter.ToInt32(counterbytes, 2);
            }

            return 0;
        }

        public byte[] GetAllCols(long position, int ColumnLen)
        {
            var all = GetPartial(position, 0, ColumnLen + 10);
            if (all != null)
            {
                if (all[0] == 10 && all[1] == 13 && all[6] == 0)
                {
                    return all.Skip(10).ToArray();
                }
            }
            return null;
        }

        public async Task<byte[]> GetAllColsAsync(long position, int ColumnLen)
        {
            var all = await GetPartialAsync(position, 0, ColumnLen + 10);
            if (all != null)
            {
                if (all[0] == 10 && all[1] == 13 && all[6] == 0)
                {
                    return all.Skip(10).ToArray();
                }
            }
            return null;
        }

        public byte[] GetCol(long position, int relativePos, int len)
        {
            // because this is used for filter query, for performance reason, did not check deletion byte. 


            if (len > 0)
            {
                return GetPartial(position, relativePos + 10 + 8, len);
            }
            return null;
        }

        public void UpdateCol(long position, int relativeposition, int length, byte[] values)
        {
            lock (_locker)
            {
                this.Stream.Position = position + 10 + relativeposition + 8;
                this.Stream.Write(values, 0, length);
            }

        }

        #endregion
        public void Close()
        {
            if (_filestream != null)
            {
                lock (_locker)
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
            lock (_locker)
            {
                this.Close();
                if (System.IO.File.Exists(this.Fullfilename))
                {
                    System.IO.File.Delete(this.Fullfilename);
                }
            }
        }

        public void Flush()
        {
            if (_filestream != null)
            {

                _filestream.Flush();

            }
        }

        public FileStream Stream
        {
            get
            {

                if (_filestream == null || !_filestream.CanRead)
                {
                    lock (_locker)
                    {
                        if (_filestream == null || !_filestream.CanRead)
                        {
                            this.OpenOrCreate();
                            _filestream = StreamManager.GetFileStream(this.Fullfilename);
                        }
                    }
                }
                return _filestream;
            }
        }

    }
}
