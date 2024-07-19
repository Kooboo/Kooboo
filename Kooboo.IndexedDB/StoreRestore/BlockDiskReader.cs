using System;
using System.Collections.Generic;
using System.IO;

namespace Kooboo.IndexedDB.StoreRestore
{

    public class BlockDiskReader<TKey, TValue>
    {
        public ObjectStore<TKey, TValue> store;

        private string fullFileName { get; set; }

        public BlockDiskReader(ObjectStore<TKey, TValue> dbstore)
        {
            this.store = dbstore;
            this.Length = this.store.BlockFile.Stream.Length;
            this.readIndex = 0;
            this.store.BlockFile.Close();
            this.fullFileName = this.store.BlockFile.Fullfilename;
        }


        private object locker { get; set; } = new object();
        private FileStream _filestream;

        public FileStream Stream
        {
            get
            {
                if (_filestream == null || !_filestream.CanRead)
                {
                    lock (locker)
                    {
                        if (_filestream == null || !_filestream.CanRead)
                        {
                            _filestream = File.Open(fullFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        }
                    }
                }
                return _filestream;
            }
        }

        private long readIndex { get; set; }


        private long Length { get; set; }

        public long ReadNextBlock()
        {
            this.Stream.Position = this.readIndex;

            var nextbyte = this.Stream.ReadByte();

            while (nextbyte != -1)
            {
                if (nextbyte == 10)
                {
                    nextbyte = this.Stream.ReadByte();
                    if (nextbyte == 13)
                    {
                        // Mark current index for possible next read. 
                        this.readIndex = this.Stream.Position;
                        var currentIndex = this.readIndex - 2;

                        // found the block... 
                        var byte3 = this.Stream.ReadByte();
                        var byte4 = this.Stream.ReadByte();
                        var byte5 = this.Stream.ReadByte();
                        var byte6 = this.Stream.ReadByte();
                        var byte7 = this.Stream.ReadByte();


                        byte[] intbytes = new byte[4];
                        intbytes[0] = (byte)byte3;
                        intbytes[1] = (byte)byte4;
                        intbytes[2] = (byte)byte5;
                        intbytes[3] = (byte)byte6;

                        var BlockLen = BitConverter.ToInt32(intbytes, 0);

                        var nextblockPosition = currentIndex + BlockLen + 10; // block contains 10 bytes of header.  
                        if (IsNextBlockStartMark(nextblockPosition))
                        {
                            if (byte7 == 0)
                            {
                                this.readIndex = nextblockPosition;
                                return currentIndex;
                            }
                            else if (byte7 == 1)
                            {
                                this.readIndex = nextblockPosition;
                                return ReadNextBlock();
                            }
                            else
                            {
                                /// this must be an error..
                                this.Stream.Position = this.readIndex;
                            }
                        }
                        else
                        {
                            this.Stream.Position = this.readIndex;
                        }

                    }
                    else
                    {
                        continue;
                    }
                }
                nextbyte = this.Stream.ReadByte();
            }

            return -1;
        }

        public HashSet<long> GetAllBlocks()
        {
            HashSet<long> result = new HashSet<long>();

            var next = this.ReadNextBlock();
            while (next > 0)
            {
                result.Add(next);
                next = this.ReadNextBlock();
            }

            return result;
        }

        private bool IsNextBlockStartMark(long position)
        {
            if (position > this.Length)
            {
                // out of index.
                return false;
            }

            if (position == this.Length)
            {
                return true; // ths is the end... 
            }

            if (position < 10)   // header value > 10 byts
            {
                return false;
            }

            this.Stream.Position = position;
            var byteone = this.Stream.ReadByte();
            var bytetwo = this.Stream.ReadByte();

            if (byteone == 10 && bytetwo == 13)
            {
                return true;  // start mark of next block...
            }

            return false;
        }

        public void Close()
        {
            if (this._filestream != null)
            {
                this._filestream.Close();
            }
        }
    }
}
