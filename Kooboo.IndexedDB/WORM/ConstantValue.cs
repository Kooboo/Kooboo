namespace Kooboo.IndexedDB.WORM
{
    public class ConstantValue
    {

        public const byte LeafNode = 1;


        public const byte TreeNode = 2;


        public const byte ItemPointer = 3;   // point to item block content. 

        public const byte NodePointer = 4;   // point to a sub node. 
    }
}
