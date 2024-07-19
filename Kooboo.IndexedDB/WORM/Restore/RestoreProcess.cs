using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.IndexedDB.WORM.Restore
{
    public class RestoreProcess<T>
    {

        internal HashSet<long> ValueBlock { get; set; } = new HashSet<long>();

        internal HashSet<long> NodeList { get; set; } = new HashSet<long>();

        public WormDb<T> oldDb { get; set; }

        public RestoreProcess(List<BlockResult> blockResult, WormDb<T> db)
        {
            this.oldDb = db;

            var len = blockResult.Count();
            for (int i = 0; i < len; i++)
            {
                var item = blockResult[i];

                if (item.IsNode)
                {
                    this.NodeList.Add(item.Position);
                }
                else
                {
                    this.ValueBlock.Add(item.Position);
                }
            }
        }


        public WormDb<T> RestoreTo(string fullfileName)
        {
            List<PositionPointer> pointers = new List<PositionPointer>();

            var newdb = new WormDb<T>(fullfileName, this.oldDb.keyFieldName, this.oldDb.ValueConverter);

            foreach (var item in NodeList)
            {
                var node = this.oldDb.LoadNode(item);
                if (node != null && node.Isleaf)
                {
                    foreach (var pointer in node.Pointer)
                    {
                        if (pointer != null)
                        {
                            pointers.Add(pointer);

                            if (pointer.Position > 0)
                            {
                                this.ValueBlock.Remove(pointer.Position);
                            }

                        }
                    }
                }
            }

            if (ValueBlock.Any())
            {
                // if there is left at value block.... if there is a key fields..

                foreach (var item in ValueBlock)
                {
                    PositionPointer pointer = new PositionPointer(newdb.MetaByteLen) { Id = -1, Position = item, IsDeleted = false };

                    pointers.Add(pointer);

                }

            }

            if (pointers.Any())
            {
                pointers.Sort(new PointerComparer());

                long ProcessedDisk = 0;  // This is to prevent.

                foreach (var item in pointers)
                {
                    if (item == null || item.Position < ProcessedDisk)
                    {
                        continue;
                    }

                    var obj = this.oldDb.LoadValue(item);

                    if (obj != null)
                    {
                        newdb.Add(obj);

                        if (item.Position > ProcessedDisk)
                        {
                            ProcessedDisk = item.Position;
                        }
                    }

                }
            }

            return newdb;

        }

    }

    public class PointerComparer : IComparer<PositionPointer>
    {
        public int Compare(PositionPointer x, PositionPointer y)
        {
            if (x.Position == y.Position)
            {
                return 0;
            }
            else if (x.Position > y.Position)
            {
                return 1;
            }
            return -1;
        }
    }
}
