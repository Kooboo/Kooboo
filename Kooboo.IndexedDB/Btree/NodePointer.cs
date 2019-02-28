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
    /// The tree node pointer to sub leaf or node or block
    /// </summary>
    public class NodePointer
    {
        private EnumValues.TypeIndicator _indicator;

        /// <summary>
        /// The object type of pointer destination.
        /// </summary>
        public EnumValues.TypeIndicator Indicator
        {
            get
            {
                if (_indicator == default(EnumValues.TypeIndicator) && _pointerbytes != null)
                {
                    _indicator = (EnumValues.TypeIndicator)_pointerbytes[0];
                }
                return _indicator;
            }
            set
            {
                _indicator = value;
            }
        }

        private Int16 _relativeposition;

        /// <summary>
        /// relative disk position of this key-pointer pair within the leaf/node, this is used to delete a record quickly. 
        /// </summary>
        public Int16 RelativePosition
        {
            get
            {
                if (_relativeposition == default(Int16) && _pointerbytes != null)
                {
                    _relativeposition =BitConverter.ToInt16(_pointerbytes, 1);
                }
                return _relativeposition;
            }
            set
            {
                _relativeposition = value;
            }



        }

        /// <summary>
        /// The key value that is connected with this pointer.
        /// </summary>
        public byte[] KeyToPosition;

        /// <summary>
        ///  The fixed length of a node pointer. point to sub node or leaf or block or duplicate. 
        ///  a pointer fixed at 11 length.
        /// </summary>
        public static Int16 Length = 11;

        private long _positionpointer;

        /// <summary>
        /// Pointer to the next position
        /// </summary>
        public long PositionPointer
        {
            get
            {
                if (_positionpointer == default(long) && _pointerbytes != null)
                {
                    _positionpointer = BitConverter.ToInt64(_pointerbytes, 3);
                }
                return _positionpointer;
            }
            set
            {
                _positionpointer = value;
            }
        }

        /// <summary>
        /// Convert value to bytes. 
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            byte[] bytearray = new byte[NodePointer.Length];

            bytearray[0] = (byte)Indicator;

            System.Buffer.BlockCopy(BitConverter.GetBytes(this.RelativePosition), 0, bytearray, 1, 2);

            // 8 bytes for pointer
            System.Buffer.BlockCopy(BitConverter.GetBytes(this.PositionPointer), 0, bytearray, 3, 8);

            return bytearray;
        }

        private byte[] _pointerbytes;

        public byte[] PointerBytes
        {
            get
            {
                return _pointerbytes;
            }
            set
            {
                _pointerbytes = value;
                //when assign value, reset other values. 
                _positionpointer = default(long);
                _relativeposition = default(Int16);
                _indicator = default(EnumValues.TypeIndicator);
            }
        }

        public byte[] GetBytes()
        {
            if (_pointerbytes != null)
            {
                return _pointerbytes;
            }
            else
            {
                return ToBytes();
            }
        }


        public bool hasPointer
        {
            get
            {
                return (this.PositionPointer != 0);
            }
        }

        private bool _isPreviousPoionter;

        /// <summary>
        ///  whether this is the left preious pointer of current node or not. 
        /// </summary>
        public bool IsFirstPreviousPointer
        {
            get
            {
                if (_isPreviousPoionter)
                {
                    return true;
                }
                else
                {
                    if (KeyToPosition == null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            set
            {
                _isPreviousPoionter = value;
            }

        }

    }
}
