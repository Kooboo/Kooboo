//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Linq;

namespace Kooboo.IndexedDB.BTree
{
    /// <summary>
    /// The tree node pointer to sub leaf or node or block
    /// </summary>
    public class NodePointer
    {
        /// <summary>
        /// The object type of pointer destination.
        /// </summary>
        public EnumValues.TypeIndicator Indicator;

        /// <summary>
        /// relative disk position of this key-pointer pair within the leaf/node, this is used to delete a record quickly. This value is set when loading.
        /// </summary>
        public Int16 RelativePosition;

        /// <summary>
        /// The key value that is connected with this pointer.It is set by loading. 
        /// </summary>
        public byte[] KeyToPosition;

        /// <summary>
        ///  The fixed length of a node pointer. point to sub node or leaf or block or duplicate. 
        ///  a pointer fixed at 11 length.
        /// </summary>
        public int Length = 11;

        public static int GetPointerLength(int MetaLen = 0)
        {
            return MetaLen + 11;
        }

        /// <summary>
        /// Pointer to the next position
        /// </summary>
        public long PositionPointer;

        public byte[] ToBytes()
        {
            byte[] bytearray = new byte[this.Length];

            bytearray[0] = (byte)Indicator;

            System.Buffer.BlockCopy(BitConverter.GetBytes(this.RelativePosition), 0, bytearray, 1, 2);

            // 8 bytes for pointer
            Buffer.BlockCopy(BitConverter.GetBytes(this.PositionPointer), 0, bytearray, 3, 8);

            if (this.Length > 11 && this.BPlusBytes != null)
            {
                int appendLen = this.Length - 11;
                if (this.BPlusBytes.Length < appendLen)
                {
                    appendLen = this.BPlusBytes.Length;
                }
                Buffer.BlockCopy(this.BPlusBytes, 0, bytearray, 11, appendLen);
            }

            return bytearray;
        }

        public static NodePointer FromBytes(byte[] bytes)
        {
            NodePointer pointer = new NodePointer();
            pointer.Indicator = (EnumValues.TypeIndicator)bytes[0];
            pointer.RelativePosition = BitConverter.ToInt16(bytes, 1);
            pointer.PositionPointer = BitConverter.ToInt64(bytes, 3);
            return pointer;
        }

        public static NodePointer FromBytes(byte[] bytes, byte[] KeysToPosition)
        {
            var pointer = FromBytes(bytes);
            pointer.KeyToPosition = KeysToPosition;
            return pointer;
        }

        public static NodePointer FromBytes(byte[] bytes, bool IsPreviousPointer)
        {
            var pointer = FromBytes(bytes);
            pointer.IsPreviousPointer = IsPreviousPointer;
            return pointer;
        }

        public static NodePointer Create(EnumValues.TypeIndicator indicator, long diskposition)
        {
            NodePointer pointer = new NodePointer();
            pointer.PositionPointer = diskposition;
            pointer.Indicator = indicator;
            return pointer;
        }


        public static NodePointer FromBPlusBytes(byte[] bytes, byte[] KeysToPosition, int BplusLen)
        {
            var pointer = FromBPlusBytes(bytes, BplusLen);
            pointer.KeyToPosition = KeysToPosition;
            return pointer;
        }

        public static NodePointer FromBPlusBytes(byte[] bytes, bool IsPreviousPointer, int BplusLen)
        {
            var pointer = FromBPlusBytes(bytes, BplusLen);
            pointer.IsPreviousPointer = IsPreviousPointer;
            return pointer;
        }


        public static NodePointer FromBPlusBytes(byte[] bytes, int BPlusLen)
        {
            NodePointer pointer = new NodePointer();
            pointer.Indicator = (EnumValues.TypeIndicator)bytes[0];
            pointer.RelativePosition = BitConverter.ToInt16(bytes, 1);
            pointer.PositionPointer = BitConverter.ToInt64(bytes, 3);
            pointer.BPlusBytes = bytes.Skip(11).ToArray();
            return pointer;
        }

        public static NodePointer CreateBPlus(EnumValues.TypeIndicator indicator, long diskposition, int BplusLen, byte[] BPlusBytes)
        {
            var pointer = new NodePointer();
            pointer.PositionPointer = diskposition;
            pointer.Indicator = indicator;
            pointer.Length = BplusLen + 11;
            pointer.BPlusBytes = BPlusBytes;
            return pointer;
        }

        /// <summary>
        ///  whether this is the left preious pointer of current node or not. 
        /// </summary>
        public bool IsPreviousPointer;

        public byte[] BPlusBytes;

    }
}
