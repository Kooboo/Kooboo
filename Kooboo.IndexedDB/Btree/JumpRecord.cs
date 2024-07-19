//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.IndexedDB.BTree
{
    /// <summary>
    /// each record with the jump table list.
    /// </summary>
    public class JumpRecord
    {
        public JumpRecord()
        {
            //this.pointerBytes = pointerBytes;
        }

        public Int64 diskLocation;

        private byte _level;

        public byte level
        {
            get
            {
                if (_level == default(byte))
                {
                    if (pointerBytes != null)
                    {
                        _level = pointerBytes[43];
                    }
                }
                return _level;
            }
            set
            {
                _level = value;
            }

        }

        private Int64 _previous;
        public Int64 Previous
        {
            get
            {
                if (_previous == default(Int64))
                {
                    if (pointerBytes != null)
                    {
                        _previous = BitConverter.ToInt64(pointerBytes, 3);
                    }
                }
                return _previous;
            }

            set
            {
                _previous = value;
            }
        }

        private Int64 _next;
        public Int64 Next
        {
            get
            {
                if (_next == default(Int64))
                {
                    if (pointerBytes != null)
                    {
                        _next = BitConverter.ToInt64(pointerBytes, 11);
                    }
                }
                return _next;
            }
            set
            {
                _next = value;
            }

        }

        private Int64 _top;
        public Int64 TOP
        {
            get
            {
                if (_top == default(Int64))
                {
                    if (pointerBytes != null)
                    {
                        _top = BitConverter.ToInt64(pointerBytes, 19);
                    }
                }
                return _top;
            }
            set
            {
                _top = value;
            }

        }

        private Int64 _bottom;

        public Int64 Bottom
        {
            get
            {
                if (_bottom == default(Int64))
                {
                    if (pointerBytes != null)
                    {
                        _bottom = BitConverter.ToInt64(pointerBytes, 27);
                    }
                }
                return _bottom;
            }
            set
            {
                _bottom = value;
            }

        }

        private Int64 _blockPosition;
        public Int64 BlockPosition
        {
            get
            {
                if (_blockPosition == default(Int64))
                {
                    if (pointerBytes != null)
                    {
                        _blockPosition = BitConverter.ToInt64(pointerBytes, 35);
                    }
                }
                return _blockPosition;
            }
            set
            {
                _blockPosition = value;
            }

        }

        private enumSectionType _indicator;
        public enumSectionType Indicator
        {
            get
            {
                if (pointerBytes != null)
                {
                    return (enumSectionType)pointerBytes[2];
                }
                else
                {
                    return _indicator;
                }
            }

            set
            {
                _indicator = value;
            }

        }


        public byte[] pointerBytes;


        public byte[] ToBytes()
        {
            byte[] byteArray = new byte[46];

            byteArray[0] = BTreeIndexDuplicate.startByteOne;
            byteArray[1] = BTreeIndexDuplicate.startByteTwo;
            byteArray[44] = BTreeIndexDuplicate.startByteOne;
            byteArray[45] = BTreeIndexDuplicate.startByteTwo;

            byteArray[2] = (byte)Indicator;
            System.Buffer.BlockCopy(BitConverter.GetBytes(Previous), 0, byteArray, 3, 8);
            System.Buffer.BlockCopy(BitConverter.GetBytes(Next), 0, byteArray, 11, 8);
            System.Buffer.BlockCopy(BitConverter.GetBytes(TOP), 0, byteArray, 19, 8);
            System.Buffer.BlockCopy(BitConverter.GetBytes(Bottom), 0, byteArray, 27, 8);
            System.Buffer.BlockCopy(BitConverter.GetBytes(BlockPosition), 0, byteArray, 35, 8);

            byteArray[43] = level;

            return byteArray;
        }
    }

}
