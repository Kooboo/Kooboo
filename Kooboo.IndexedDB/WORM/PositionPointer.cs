using System;

namespace Kooboo.IndexedDB.WORM
{
    public class PositionPointer
    {
        public static int GetPointerLen(int metaByteLen)
        {
            return metaByteLen + 20;
        }

        private int PointerLen { get; set; }

        private int MetaByteLen { get; set; }

        public PositionPointer(int MetaByteLen)
        {
            this.MetaByteLen = MetaByteLen;
            this.PointerLen = GetPointerLen(this.MetaByteLen);
        }


        public bool IsDeleted { get; set; }

        public long Id { get; set; }  // 8 

        public long Position { get; set; }  //8 

        public byte[] MetaBytes { get; set; }


        public byte[] ToBytes()
        {
            byte[] bytes = new byte[this.PointerLen];

            //The first 3 bytes is reserved for system usage. 
            byte[] first4 = new byte[4];
            first4[0] = 1;
            first4[1] = 2;

            byte IsDelete = (byte)(this.IsDeleted ? 1 : 0);
            first4[3] = IsDelete;

            System.Buffer.BlockCopy(first4, 0, bytes, 0, 4);

            var IdBytes = BitConverter.GetBytes(Id);
            System.Buffer.BlockCopy(IdBytes, 0, bytes, 4, 8);

            var positionBytes = BitConverter.GetBytes(Position);

            System.Buffer.BlockCopy(positionBytes, 0, bytes, 12, 8);

            if (this.MetaByteLen > 0 && this.MetaBytes != null)
            {
                System.Buffer.BlockCopy(this.MetaBytes, 0, bytes, 20, this.MetaByteLen);
            }

            return bytes;
        }

        public static PositionPointer FromBytes(byte[] bytes, int metaLen)
        {
            int pointerLen = GetPointerLen(metaLen);

            if (bytes.Length != pointerLen)
            {
                return null;
            }

            if (bytes[0] == 1 && bytes[1] == 2)
            {
                bool IsDeleted = false;

                if (bytes[3] == 1)
                {
                    IsDeleted = true;
                }

                var id = BitConverter.ToInt64(bytes, 4);

                if (IsDeleted)
                {
                    return new PositionPointer(metaLen) { IsDeleted = true, Id = id };
                }

                var position = BitConverter.ToInt64(bytes, 12);

                if (id >= 0 || position >= 0)
                {
                    var result = new PositionPointer(metaLen)
                    {
                        Id = id,
                        Position = position
                    };

                    if (metaLen > 0)
                    {
                        result.MetaBytes = new byte[metaLen];

                        System.Buffer.BlockCopy(bytes, 20, result.MetaBytes, 0, metaLen);
                    }

                    return result;
                }
            }

            return null;

        }
    }
}
