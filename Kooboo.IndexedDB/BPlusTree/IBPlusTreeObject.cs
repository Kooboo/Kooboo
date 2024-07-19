namespace Kooboo.IndexedDB.BPlusTree
{
    public interface IBPlusTreeObject
    {
        int BPlusTreeLen { get; }

        void SetBPlusBytes(byte[] bytes);

        byte[] GetBPlusBytes();

        bool SkipValueBlock { get; }  // Where there is a need to store value block or not.  
    }
}
