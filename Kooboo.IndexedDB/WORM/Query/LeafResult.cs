namespace Kooboo.IndexedDB.WORM.Query
{
    public class LeafResult
    {
        public Node Node { get; set; }
        public int PointerIndex { get; set; }

        public long FindKey { get; set; }
    }
}
