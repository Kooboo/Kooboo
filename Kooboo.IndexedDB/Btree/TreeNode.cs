//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.IndexedDB.Btree
{
    /// <summary>
    /// The nodes within tree, both root node and sub node.
    /// </summary>
    public class TreeNode
    {
        //////////////////////////////////////////////////////////////////////////////////////////////
        // a node contains below info.
        //  /startbyte, delete indicator, type indicator, parent position,  count, Pointers to Previous/Next leaf,  
        // [key + pointer 8 + counter + indicator] * X number, endbyte

        private TreeFile tree;

        public TreeNode(TreeFile tree)
        {
            this.tree = tree;

            KeyArray = new Dictionary<byte[], byte[]>(this.tree.equalitycomparer);

            _freeRelativePos = new HashSet<int>();

            PreviousPointer = new byte[this.tree.config.PointerLen]; 
        }
         
        /// <summary>
        /// To indicate status of this node location.
        /// </summary>
        public EnumValues.DeleteIndicator Deletion;

        public EnumValues.TypeIndicator TypeIndicator;

        /// <summary>
        /// The amount of duplicate keys within this node. 
        /// A node should not have more than 256 keys, otherwise. 
        /// </summary>
      // public byte DuplicateKeys;

        /// <summary>
        /// This value will be available after load one node, but will not be saved into disk.
        /// </summary>
        public Int64 DiskPosition;
        
        public static Int64 NodeCounterPosition = 24;
        
        public static int PreviousPointerPosition = 13;

        public static int DeletionIndicationPosition = 2;
          
        public static int MinKeysMustBeFree = 3;
        public static int StartKeyPosition = 42;

        public static byte startkeyone = 10;
        public static byte startkeytwo = 13;
        
        /// <summary>
        /// free record postion to be used.
        /// </summary>
        private HashSet<int> _freeRelativePos;
        public int GetFreeRelativePosition()
        {
            if (_freeRelativePos.Count > 0)
            {
                int pos = _freeRelativePos.FirstOrDefault();
                _freeRelativePos.Remove(pos);
                return pos;
            }
            else
            {
                int writeposition = _startwriteposition;
                _startwriteposition = _startwriteposition + this.tree.config.RecordLen;

                return writeposition;
            }
        }

        private int _startwriteposition;
        
        /// <summary>
        ///pointer to previous records that =last key.. next pointer contains within the key pointer pairs. 
        /// </summary>
        public byte[] PreviousPointer { get; set; }

       
         /// <summary>
        ///  The key value pair convert into dictionary
        ///  string = key, byte[] =  indicator(leaf or node) + counter + position
        ///  Key len must always = _config.keylength, and value len must always = _config.pointerlen.
        /// </summary>
        public Dictionary<byte[], byte[]> KeyArray { get; set; }
         
        public Int16 Count { get; set; }
        
        public byte[] ToBytes()
        {
            byte[] bytearray = new byte[this.tree.config.NodeDiskSize];

            bytearray[0] =  startkeyone;
            bytearray[1] = startkeytwo;

            bytearray[this.tree.config.NodeDiskSize - 1] = startkeytwo;
            bytearray[this.tree.config.NodeDiskSize - 2] = startkeyone;

            bytearray[2] = (byte)this.Deletion;
            bytearray[3] = (byte)this.TypeIndicator;

           //bytearray[4] = (byte)this.DuplicateKeys;

            // parent position
            // System.Buffer.BlockCopy(BitConverter.GetBytes(this.ParentPosition), 0, bytearray, 5, 8);
            // previous pointer.
            System.Buffer.BlockCopy(this.PreviousPointer, 0, bytearray, 13, 11);
            // The key counts == how many times to read.
            System.Buffer.BlockCopy(BitConverter.GetBytes((Int16)this.KeyArray.Count), 0, bytearray, 24, 2);
           // System.Buffer.BlockCopy(BitConverter.GetBytes(this.Navigator_Previous), 0, bytearray, 26, 8);
           // System.Buffer.BlockCopy(BitConverter.GetBytes(this.Navigator_Next), 0, bytearray, 34, 8);

            // now all the key pairs.
            int startposition = TreeNode.StartKeyPosition;  //after above writing.

            foreach (var item in this.KeyArray)
            {
                bytearray[startposition] = (byte)EnumValues.DeleteIndicator.normal_not_deleted;

                startposition += 1;

                System.Buffer.BlockCopy(item.Key, 0, bytearray, startposition, this.tree.config.KeyLength);

                startposition += this.tree.config.KeyLength; 
           
                System.Buffer.BlockCopy(item.Value, 0, bytearray, startposition, this.tree.config.PointerLen);
                startposition += this.tree.config.PointerLen;
            }

            return bytearray;
        }

        public void ParseBytes(byte[] bytes)
        {
            //if (bytes[0] != 10 || bytes[1] != 13)
            //{
            //    throw new Exception("invalid valid record file");
            //}

            this.Deletion = (EnumValues.DeleteIndicator)bytes[2];

            this.TypeIndicator = (EnumValues.TypeIndicator)bytes[3];

           //this.DuplicateKeys = bytes[4];

           // this.ParentPosition = BitConverter.ToInt64(bytes, 5);

            System.Buffer.BlockCopy(bytes, 13, this.PreviousPointer, 0, 11);

            Int16 count = BitConverter.ToInt16(bytes, 24);
            this.Count = count;

            //byte[] onerecord = new byte[_config.RecordLen];

            int startposition = TreeNode.StartKeyPosition;

            // Now read one record by record till count.
            for (int i = 0; i < count; i++)
            {
                if ((startposition + this.tree.config.RecordLen) >= this.tree.config.NodeDiskSize)
                {
                    break;
                }

                ///To check, this might have an error when startposition out of index. To be checked. 
                EnumValues.DeleteIndicator status = (EnumValues.DeleteIndicator)bytes[startposition];

                if (status == EnumValues.DeleteIndicator.free_deleted)
                {
                    this._freeRelativePos.Add(startposition);
                    i--;  // Skip this record. 
                    startposition = startposition + this.tree.config.RecordLen;
                    continue; //continue for the next loop.
                }

                Int16 relativeStartPosition = (Int16)startposition;

                startposition += 1;

                byte[] keybyte = new byte[this.tree.config.KeyLength];

                System.Buffer.BlockCopy(bytes, startposition, keybyte, 0, this.tree.config.KeyLength);
                
                byte[] pointerbyte = new byte[this.tree.config.PointerLen];

                startposition += this.tree.config.KeyLength;

                System.Buffer.BlockCopy(bytes, startposition, pointerbyte, 0, this.tree.config.PointerLen);

                System.Buffer.BlockCopy(BitConverter.GetBytes(relativeStartPosition), 0, pointerbyte, 1, 2);

                KeyArray.Add(keybyte, pointerbyte);

                startposition = startposition + this.tree.config.PointerLen;


            }

            /// the position that we can start to write new record.
            _startwriteposition = startposition;

        }

        /// <summary>
        /// find the pointer to next node or leaf that should contains the key. 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public NodePointer FindPointer(byte[] key)
        {
            NodePointer nodepointer = new NodePointer();
            byte[] currentkey = key;
            byte[] currentvalue = new byte[NodePointer.Length];
            bool found = false;
            
            foreach (var item in KeyArray)
            {
                if (this.tree.comparer.Compare(key, item.Key) >= 0)
                {
                    if (!found)
                    {
                        found = true;
                        currentkey = item.Key;
                        currentvalue = item.Value;
                    }
                    else
                    {
                        if (this.tree.comparer.Compare(item.Key, currentkey) > 0)
                        {
                            currentkey = item.Key;
                            currentvalue = item.Value;
                        }
                    }

                }
            }

         
            if (found)
            {
                nodepointer.KeyToPosition = currentkey;
                nodepointer.PointerBytes = currentvalue;
            }
            else
            {
                nodepointer.PointerBytes = this.PreviousPointer;
                nodepointer.IsFirstPreviousPointer = true;
            }

            return nodepointer;
        }

    }
}
