//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;


namespace Kooboo.IndexedDB.BTree
{
    /// <summary>
    /// The config value for the tree node leaf.
    /// </summary>
    public class TreeConfig
    {
        /// <summary>
        /// used to create a new config.
        /// </summary>
        /// <param name="keylen"></param>
        /// <param name="unique"></param>
        /// <param name="keyType"></param>
        public TreeConfig(int keylen, bool unique, Type keyType, int MaxCachelevel, IComparer<byte[]> Comparer, IEqualityComparer<byte[]> Equalitycomparer)
        {
            this.ConfigDiskBytes = 100; // default 100, will changed on calculation when needed. 
            this.unique = unique;
            this.keyType = keyType;
            this.KeyLength = (Int16)keylen;

            this.PointerLen = (Int16)NodePointer.GetPointerLength();

            this.RecordLen = (Int16)(1 + this.KeyLength + PointerLen);   // 1 byte for deletation indication. 
            this.comparer = Comparer;
            this.equalitycomparer = Equalitycomparer;

            this.MaxCacheLevel = MaxCachelevel;

            initBuffersize_KeysPerNode();
        }

        public TreeConfig(int keylen, bool unique, Type keyType, int MaxCacheLevel, IComparer<byte[]> Comparer, IEqualityComparer<byte[]> Equalitycomparer, int BPlusLen)
        {
            this.ConfigDiskBytes = 100; // default 100, will changed on calculation when needed. 
            this.unique = unique;
            this.keyType = keyType;
            this.KeyLength = (Int16)keylen;

            this.MaxCacheLevel = MaxCacheLevel;

            this.BPlusLen = (Int16)BPlusLen;

            this.PointerLen = (Int16)NodePointer.GetPointerLength(BPlusLen);

            this.RecordLen = (Int16)(1 + this.KeyLength + PointerLen);   // 1 byte for deletation indication. 
            this.comparer = Comparer;
            this.equalitycomparer = Equalitycomparer;

            initBuffersize_KeysPerNode();
        }

        /// <summary>
        /// This method is only used to load config back. To create a new config, should use another constructor.
        /// </summary>
        public TreeConfig()
        {

        }

        private byte StartByte = 91;
        private byte EndByte = 93;


        public IComparer<byte[]> comparer;

        public IEqualityComparer<byte[]> equalitycomparer;

        public int MaxCacheLevel { get; set; }


        //The total number of bytes this header information required on disk. 
        public Int16 ConfigDiskBytes
        { get; private set; }

        public Type keyType
        { get; private set; }

        /// <summary>
        /// Whether the key must be unique or not. 
        /// </summary>
        public bool unique
        { get; private set; }

        /// <summary>
        /// The max length of keys. Keys that longer than this value will be truncated. 
        /// </summary>
        public Int16 KeyLength
        { get; private set; }

        /// <summary>
        /// The max count before a split
        /// </summary>
        public Int16 SplitCount
        { get; set; }

        /// <summary>
        /// The min count before a merge.
        /// </summary>
        public Int16 MergeCount
        { get; set; }


        /// <summary>
        /// The number of keys in every node. 
        /// </summary>
        public Int16 KeysPerNode
        { get; set; }

        /// <summary>
        ///  the bytes per record (1 + key + pointer) 
        /// 1 + [keylen] + [PinterLen]
        /// </summary>
        public Int16 RecordLen
        { get; set; }

        /// <summary>
        /// Indicator + counter + blockposition
        /// </summary>
        public Int16 PointerLen
        { get; set; }

        /// <summary>
        ///The record tree leaf page size to read from disk per time. this should be multiple of sector file size.
        /// </summary>
        public int NodeDiskSize
        { get; set; }

        public Int16 BPlusLen { get; set; }

        /// <summary>
        /// calculate the bufferedsize and keyspernode. 
        /// The judgement is the waste space, formula for comparison = TotalWastedSpace - (multiplier of szie (i) )*2 
        /// The least wasted method wins.
        /// </summary>
        private void initBuffersize_KeysPerNode()
        {
            // every tree.
            //startbytes, delete indicator, type indicator, count, parent position, long pointer,  
            // [key + pointer8 + 1 + 1] * X number, long pointer, endbytes
            // [1], [1], [8], [X], [1];
            int extrabytes = 2 + 1 + 1 + 2 + 8 + 16 + 16 + 2;

            int maxkeynumber = 999;  // the maijx keys per node.

            int sectorsize = GlobalSettings.SectorSize;
            // The buffered size we define to be 1 - 20 times of sector size. 
            Int16 keynumbers = 0;
            int wastedspace = 99999;  // default a big enough number, every time smaller wasted wins.
            int bufferedsize = 0;

            Int16 tempkeynumber;
            int tempbufferedsize;
            int tempbufferedsize_minus_extrabytes;
            int tempwastedspace;

            int starti = 1;
            if (GlobalSettings.MaxTreeNodeSizeMultiplier >= 4)
            {
                starti = 2;
            }
            if (GlobalSettings.MaxTreeNodeSizeMultiplier >= 10)
            {
                starti = 5;
            }

            /// this test the most space saving, but it actually should test the fastest speed. 
            for (int i = starti; i < GlobalSettings.MaxTreeNodeSizeMultiplier; i++)
            {
                tempbufferedsize = i * sectorsize;
                tempbufferedsize_minus_extrabytes = tempbufferedsize - extrabytes;
                tempkeynumber = (Int16)(tempbufferedsize_minus_extrabytes / this.RecordLen);
                tempwastedspace = tempbufferedsize_minus_extrabytes % this.RecordLen;

                if (tempwastedspace < wastedspace & tempkeynumber > keynumbers & tempkeynumber < maxkeynumber)
                {
                    bufferedsize = tempbufferedsize;
                    keynumbers = tempkeynumber;
                    wastedspace = tempwastedspace;
                }
            }

            // after the loop, the result has been calculated.
            this.KeysPerNode = keynumbers;
            this.NodeDiskSize = bufferedsize;

            this.SplitCount = (Int16)(this.KeysPerNode * GlobalSettings.SplitRatio);
            this.MergeCount = (Int16)(this.KeysPerNode * GlobalSettings.MergeRatio);

        }

        /// <summary>
        /// Convert the header class to bytes, in order to persist to disk.
        /// Header is always 100 bytes. 
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            byte[] bytearray = new byte[100];
            bytearray[0] = this.StartByte;
            bytearray[99] = this.EndByte;

            byte uniquebyte = 0;

            if (this.unique)
            {
                uniquebyte = 1;
            }

            bytearray[1] = uniquebyte;

            System.Buffer.BlockCopy(BitConverter.GetBytes(this.ConfigDiskBytes), 0, bytearray, 2, 2);

            //two bytes for keylen,
            System.Buffer.BlockCopy(BitConverter.GetBytes(this.KeyLength), 0, bytearray, 4, 2);

            //then the keynumbers. 
            System.Buffer.BlockCopy(BitConverter.GetBytes(this.KeysPerNode), 0, bytearray, 6, 2);

            // recordlen.
            System.Buffer.BlockCopy(BitConverter.GetBytes(this.RecordLen), 0, bytearray, 8, 2);

            // now the buffered size, BufferedSize = int = 4 bytes.
            System.Buffer.BlockCopy(BitConverter.GetBytes(this.NodeDiskSize), 0, bytearray, 10, 4);

            System.Buffer.BlockCopy(BitConverter.GetBytes(this.SplitCount), 0, bytearray, 14, 2);

            System.Buffer.BlockCopy(BitConverter.GetBytes(this.MergeCount), 0, bytearray, 16, 2);

            // The rest is the type information.
            string typeinfo = keyType.ToString();

            byte[] keytypebytes = System.Text.Encoding.ASCII.GetBytes(typeinfo);
            Int16 keytypelen = (Int16)keytypebytes.Length;

            Buffer.BlockCopy(BitConverter.GetBytes(keytypelen), 0, bytearray, 18, 2);

            Buffer.BlockCopy(keytypebytes, 0, bytearray, 20, keytypelen);

            //more added here.
            Buffer.BlockCopy(BitConverter.GetBytes(this.BPlusLen), 0, bytearray, 90, 2);

            return bytearray;

        }

        /// <summary>
        /// convert from bytes to class values. 
        /// </summary>
        /// <param name="diskbytes"></param>
        public static TreeConfig FromBytes(byte[] diskbytes, int MaxCacheLevel, IComparer<byte[]> Comparer, IEqualityComparer<byte[]> Equalitycomparer)
        {
            var newconfig = new TreeConfig();

            newconfig.comparer = Comparer;
            newconfig.equalitycomparer = Equalitycomparer;
            newconfig.MaxCacheLevel = MaxCacheLevel;

            // bytearray[0] = this.StartByte;
            //bytearray[99] = this.EndByte;
            if (diskbytes[0] != newconfig.StartByte || diskbytes[99] != newconfig.EndByte)
            {
                throw new Exception("wrong start or end byte check");
            }
            // now convert them back to value one by one.

            byte uniquebyte = diskbytes[1];
            if (uniquebyte == 1)
            {
                newconfig.unique = true;
            }
            else
            {
                newconfig.unique = false;
            }

            newconfig.ConfigDiskBytes = BitConverter.ToInt16(diskbytes, 2);
            newconfig.KeyLength = BitConverter.ToInt16(diskbytes, 4);
            newconfig.KeysPerNode = BitConverter.ToInt16(diskbytes, 6);
            newconfig.RecordLen = BitConverter.ToInt16(diskbytes, 8);
            newconfig.NodeDiskSize = BitConverter.ToInt32(diskbytes, 10);
            newconfig.SplitCount = BitConverter.ToInt16(diskbytes, 14);
            newconfig.MergeCount = BitConverter.ToInt16(diskbytes, 16);
            newconfig.PointerLen = (Int16)(newconfig.RecordLen - newconfig.KeyLength - 1);

            newconfig.BPlusLen = BitConverter.ToInt16(diskbytes, 90);

            Int16 keytypelen = BitConverter.ToInt16(diskbytes, 18);

            byte[] strinbyte = new byte[keytypelen];

            System.Buffer.BlockCopy(diskbytes, 20, strinbyte, 0, keytypelen);

            string typestring = System.Text.Encoding.ASCII.GetString(strinbyte);

            Type type = Type.GetType(typestring);

            newconfig.keyType = type;
            return newconfig;
        }

    }
}
