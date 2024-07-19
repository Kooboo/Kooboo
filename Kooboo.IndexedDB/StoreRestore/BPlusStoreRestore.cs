using Kooboo.IndexedDB.BPlusTree;

namespace Kooboo.IndexedDB.StoreRestore
{
    internal class BPlusStoreRestore
    {
    }

    public class BPlusStoreRestore<TKey, TValue> where TValue : IBPlusTreeObject
    {

        public BPlusStore<TKey, TValue> oldStore { get; set; }

        public BPlusStoreRestore(BPlusStore<TKey, TValue> store)
        {
            this.oldStore = store;
        }

        //public BPlusStore<TKey, TValue> RestoreTo(string newStoreName)
        //{
        //    var newstore = new BPlusStore<TKey, TValue>(newStoreName, this.oldStore.PrimaryKey, this.oldStore.KeyLen);


        //    BlockDiskReader<TKey, TValue> diskreader = new BlockDiskReader<TKey, TValue>(this.oldStore);

        //    var blocks = diskreader.GetAllBlocks();
        //    diskreader.Close();

        //    foreach (var block in blocks)
        //    {
        //        var item = this.oldStore.getValue(block);
        //        if (item != null)
        //        {
        //            newstore.add(item);
        //        }
        //    }
        //    this.oldStore.Close();
        //    newstore.Close();
        //    return newstore;
        //}


        public void Close()
        {
            this.oldStore.Close();
        }
    }


}
