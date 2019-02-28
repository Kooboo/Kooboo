//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB.Btree
{
   /// <summary>
   /// each record with the jump table list.
   /// </summary>
    public class JumpRecord
    {
        public JumpRecord()
        {
            //this.pointerBytes = pointerbytes;
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

        private Int64 _buttom;

        public Int64 Buttom
        {
            get
            {
                if (_buttom == default(Int64))
                {
                    if (pointerBytes != null)
                    {
                        _buttom = BitConverter.ToInt64(pointerBytes, 27);
                    }
                }
                return _buttom;
            }
            set
            {
                _buttom = value;
            }

        }

        private Int64 _blockposition;
        public Int64 BlockPosition
        {
            get
            {
                if (_blockposition == default(Int64))
                {
                    if (pointerBytes != null)
                    {
                        _blockposition = BitConverter.ToInt64(pointerBytes, 35);
                    }
                }
                return _blockposition;
            }
            set
            {
                _blockposition = value;
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
            byte[] bytearray = new byte[46];

            bytearray[0] = BtreeIndexDuplicate.startbyteone;
            bytearray[1] = BtreeIndexDuplicate.startbytetwo;
            bytearray[44] = BtreeIndexDuplicate.startbyteone;
            bytearray[45] = BtreeIndexDuplicate.startbytetwo;

            bytearray[2] = (byte)Indicator;
            System.Buffer.BlockCopy(BitConverter.GetBytes(Previous), 0, bytearray, 3, 8);
            System.Buffer.BlockCopy(BitConverter.GetBytes(Next), 0, bytearray, 11, 8);
            System.Buffer.BlockCopy(BitConverter.GetBytes(TOP), 0, bytearray, 19, 8);
            System.Buffer.BlockCopy(BitConverter.GetBytes(Buttom), 0, bytearray, 27, 8);
            System.Buffer.BlockCopy(BitConverter.GetBytes(BlockPosition), 0, bytearray, 35, 8);

            bytearray[43] = level;

            return bytearray;
        }
    }

}
