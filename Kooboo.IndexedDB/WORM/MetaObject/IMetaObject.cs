namespace Kooboo.IndexedDB.WORM.MetaObject
{
    /// <summary>
    /// Meta object allow you to store information on the Node itself instead of block value. 
    /// </summary>
    public interface IMetaObject
    {
        int MetaByteLen { get; }

        long MetaKey { get; set; }   // The key that can be used to retrieve the full content...Set when parse from meta bytes.

        void ParseMetaBytes(byte[] bytes);

        byte[] GetMetaBytes();

        bool SkipValueBlock { get; }  // Where there is a need to store value block or not.
    }
}
