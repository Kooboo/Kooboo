namespace Kooboo.IndexedDB.WORM
{
    public class WritingLeaf
    {
        public WritingLeaf(Node node)
        {
            this.Leaf = node;
        }

        public Node Leaf { get; set; }

        public long DiskPosition { get; set; }

        public int EmptySlotIndex { get; set; }

        public bool IsFull
        {
            get { return EmptySlotIndex == WormSetting.NodePointerCount; }
        }

        public PositionPointer UpdatePointerPosition(long newPosition, long key)
        {
            var pointer = this.Leaf.Pointer[EmptySlotIndex];
            if (pointer == null)
            {
                pointer = new PositionPointer(this.Leaf.MetaLen);
                this.Leaf.Pointer[EmptySlotIndex] = pointer;
            }
            pointer.Position = newPosition;
            pointer.Id = key;

            return pointer;
        }


        public static WritingLeaf FromPath(NodePath path)
        {
            var leaf = new WritingLeaf(path.Leaf);
            leaf.Leaf = path.Leaf;

            var index = -1;

            for (int i = 0; i < WormSetting.NodePointerCount; i++)
            {
                var item = path.Leaf.Pointer[i];
                if (item == null)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                index = WormSetting.NodePointerCount;
            }

            leaf.EmptySlotIndex = index;

            return leaf;

        }
    }
}
