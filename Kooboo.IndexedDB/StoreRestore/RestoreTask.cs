using System.IO;

namespace Kooboo.IndexedDB.StoreRestore
{
    public class RestoreTask<TKey, TValue>
    {


        public ObjectStore<TKey, TValue> oldStore { get; set; }

        public RestoreTask(ObjectStore<TKey, TValue> store)
        {
            this.oldStore = store;
        }

        public ObjectStore<TKey, TValue> RestoreTo(string newStoreName)
        {
            var newstore = new ObjectStore<TKey, TValue>(newStoreName, this.oldStore.OwnerDatabase, this.oldStore.StoreSetting);

            Serializer.Simple.SimpleConverter<StoreSetting> converter = new Serializer.Simple.SimpleConverter<StoreSetting>();

            var allbytes = converter.ToBytes(this.oldStore.StoreSetting);
            File.WriteAllBytes(newstore.StoreSettingFile, allbytes);

            BlockDiskReader<TKey, TValue> diskreader = new BlockDiskReader<TKey, TValue>(this.oldStore);

            var blocks = diskreader.GetAllBlocks();
            diskreader.Close();

            foreach (var block in blocks)
            {
                var item = this.oldStore.getValue(block);
                if (item != null)
                {
                    newstore.add(item);
                }
            }
            this.oldStore.Close();
            newstore.Close();
            return newstore;
        }


        public void Close()
        {
            this.oldStore.Close();
        }
    }

}
