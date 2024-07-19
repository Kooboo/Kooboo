using System;

namespace Kooboo.IndexedDB.WORM
{
    public class Node
    {
        internal int MetaLen { get; set; }
        internal int PointerLen { get; set; }
        internal int NodeLen { get; set; }

        public static int GetNodelen(int pointerLen)
        {
            // return    4 + 8 + 8 + 8 + 8 + NodePointerCount * PointerLen;
            return 36 + WormSetting.NodePointerCount * pointerLen;
        }

        public Node(bool IsLeaf, int NodeLen, int pointerLen, int MetaLen)
        {
            if (IsLeaf)
            {
                this.NodeType = ConstantValue.LeafNode;
            }
            else
            {
                this.NodeType = ConstantValue.TreeNode;
            }
            this.NodeLen = NodeLen;
            this.PointerLen = pointerLen;
            this.MetaLen = MetaLen;
        }


        private byte NodeType { get; set; }

        public bool Isleaf
        {
            get
            {
                return this.NodeType == ConstantValue.LeafNode;
            }
        }

        // 4 bytes for secure mark. 
        // 8 bytes for the next pointer. 
        // 8 bytes for previous pointer.

        public PositionPointer[] Pointer = new PositionPointer[WormSetting.NodePointerCount];

        public long NextNode { get; set; }

        public long PreviousNode { get; set; }

        public long StartId { get; set; }  // every node has a start id so that the database will be more stable in case of errors. can be rescured!

        public int ParentNodePointerIndex { get; set; } = -1;

        public long DiskPosition { get; set; }   // set this when load from disk. 

        public int GetPointerRelativePosition(int pointerIndex)
        {
            return 36 + this.PointerLen * pointerIndex;
        }

        public int GetRelationRelativePosition(EnumNodeRelation relation)
        {

            if (relation == EnumNodeRelation.PreviousNode)
            {
                return 12;
            }
            else if (relation == EnumNodeRelation.NextNode)
            {
                return 4;
            }
            //else if (relation == EnumNodeRelation.ParentNode)
            //{
            //    return 20; 
            //}
            else if (relation == EnumNodeRelation.StartId)
            {
                return 28;
            }

            throw new Exception("not matched exception");
        }

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[this.NodeLen];

            //The first 4 bytes is reserved for system usage. 
            byte[] first4 = new byte[4];
            first4[0] = 1;
            first4[1] = 2;
            first4[2] = this.NodeType;

            first4[3] = 4;

            System.Buffer.BlockCopy(first4, 0, bytes, 0, 4);

            var nextBytes = BitConverter.GetBytes(this.NextNode);
            System.Buffer.BlockCopy(nextBytes, 0, bytes, 4, 8);

            var prevBytes = BitConverter.GetBytes(this.PreviousNode);
            System.Buffer.BlockCopy(prevBytes, 0, bytes, 12, 8);

            // var parentBytes = BitConverter.GetBytes(this.ParentNode);
            //  System.Buffer.BlockCopy(parentBytes, 0, bytes, 20, 8);

            var idBytes = BitConverter.GetBytes(this.StartId);
            System.Buffer.BlockCopy(idBytes, 0, bytes, 28, 8);

            int currentPos = 36;

            for (int i = 0; i < WormSetting.NodePointerCount; i++)
            {
                var item = Pointer[i];
                if (item != null)
                {
                    var itemBytes = item.ToBytes();

                    System.Buffer.BlockCopy(itemBytes, 0, bytes, currentPos, itemBytes.Length);
                }
                currentPos += this.PointerLen;
            }

            return bytes;
        }

        public static Node FromBytes(byte[] bytes, int NodeLen, int PointerLen, int MetaLen)
        {

            if (bytes.Length != NodeLen)
            {
                return null;
            }

            if (bytes[0] == 1 && bytes[1] == 2)
            {
                // nodetype will be reset. 
                var node = new Node(false, NodeLen, PointerLen, MetaLen);

                var next = BitConverter.ToInt64(bytes, 4);
                var prev = BitConverter.ToInt64(bytes, 12);
                //var parent = BitConverter.ToInt64(bytes, 20);
                var id = BitConverter.ToInt64(bytes, 28);

                node.PreviousNode = prev;
                node.NextNode = next;
                node.NodeType = bytes[2];

                if (node.NodeType != ConstantValue.LeafNode && node.NodeType != ConstantValue.TreeNode)
                {
                    return null; // this is an database error. 
                }

                //node.ParentNode = parent;
                node.StartId = id;

                var startPos = 36;

                for (int i = 0; i < WormSetting.NodePointerCount; i++)
                {
                    var offet = startPos + i * PointerLen;

                    byte[] itemBytes = new byte[PointerLen];

                    Buffer.BlockCopy(bytes, offet, itemBytes, 0, PointerLen);

                    var pointer = PositionPointer.FromBytes(itemBytes, MetaLen);

                    node.Pointer[i] = pointer;
                }

                return node;
            }

            return null;

        }

    }
}
