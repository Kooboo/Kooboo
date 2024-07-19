using System;
using System.Collections.Generic;
using System.IO;
using Kooboo.IndexedDB.Helper;

namespace Kooboo.IndexedDB.WORM.Restore
{
    public class DiskReader<T>
    {
        public WormDb<T> db { get; set; }

        private string fullFileName { get; set; }

        private bool OverWriteValueBlock { get; set; }

        public DiskReader(WormDb<T> db)
        {
            this.db = db;
            this.Length = this.db.Stream.Length;
            this.readIndex = 0;
            this.db.Stream.Close();
            this.fullFileName = this.db.FullFileName;
            this.OverWriteValueBlock = this.db.OverWriteValueBLock;
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

        private long NextBlockPosition { get; set; }

        private long Length { get; set; }

        private bool IsEOF => this.readIndex >= this.Length;

        public BlockResult ReadNextBlock()
        {
            this.Stream.Position = this.readIndex;

            var nextByte = this.Stream.ReadByte();

            while (nextByte != -1)
            {
                if (nextByte == 1)
                {
                    nextByte = this.Stream.ReadByte();
                    if (nextByte == 2)
                    {
                        // Mark current index for possible next read. 
                        this.readIndex = this.Stream.Position;
                        var currentIndex = this.readIndex - 2;

                        // found the block... 
                        var byte3 = this.Stream.ReadByte();
                        var byte4 = this.Stream.ReadByte();
                        var byte5 = this.Stream.ReadByte();
                        var byte6 = this.Stream.ReadByte();

                        if (byte4 == 4 && (byte3 == ConstantValue.LeafNode || byte3 == ConstantValue.TreeNode))
                        {
                            // This might be a node...
                            var nextBlockPosition = currentIndex + this.db.NodeLen;
                            if (IsNextBlockStartMark(nextBlockPosition))
                            {
                                this.readIndex = nextBlockPosition;
                                return new BlockResult() { IsNode = true, Position = currentIndex };
                            }
                            else
                            {
                                // check next block change stream position. 
                                this.Stream.Position = this.readIndex;
                            }

                        }
                        else
                        {
                            byte[] intBytes = new byte[4];
                            intBytes[0] = (byte)byte3;
                            intBytes[1] = (byte)byte4;
                            intBytes[2] = (byte)byte5;
                            intBytes[3] = (byte)byte6;

                            var BlockLen = BitConverter.ToInt32(intBytes, 0);

                            // int case overwrite block option open. 
                            if (this.OverWriteValueBlock)
                            {
                                var byte7 = Stream.ReadByte();
                                var byte8 = Stream.ReadByte();
                                var byte9 = Stream.ReadByte();
                                var byte10 = Stream.ReadByte();

                                byte[] ExtraLenBytes = new byte[3];
                                ExtraLenBytes[0] = (byte)byte8;
                                ExtraLenBytes[1] = (byte)byte9;
                                ExtraLenBytes[2] = (byte)byte10;

                                int extraLen = ByteHelper.ThreeBytesToInt(ExtraLenBytes);
                                BlockLen += extraLen;
                            }

                            var nextBlockPosition = currentIndex + BlockLen + 10; // block contains 10 bytes of header.  

                            if (IsNextBlockStartMark(nextBlockPosition))
                            {
                                this.readIndex = nextBlockPosition;
                                return new BlockResult() { IsNode = false, Position = currentIndex };
                            }
                            else
                            {
                                this.Stream.Position = this.readIndex;
                            }

                        }

                    }
                }

                nextByte = this.Stream.ReadByte();
            }

            return null;
        }


        public List<BlockResult> ReadDiskBlock()
        {
            List<BlockResult> result = new List<BlockResult>();

            var next = this.ReadNextBlock();
            while (next != null)
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

            if (position < 100)   // init worm db contains 100 bytes header. 
            {
                return false;
            }

            this.Stream.Position = position;
            var byteOne = this.Stream.ReadByte();
            var byteTwo = this.Stream.ReadByte();

            if (byteOne == 1 && byteTwo == 2)
            {
                return true;  // start mark of next block...
            }

            return false;
        }

    }

}
