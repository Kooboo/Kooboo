namespace Kooboo.IndexedDB.WORM.Restore
{
    public class BlockResult
    {
        public bool IsNode { get; set; } // node or value block. 

        public long Position { get; set; }

    }
}
