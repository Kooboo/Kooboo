using System.Collections.Generic;


namespace Kooboo.IndexedDB.WORM.Restore
{
    public class DiskResult
    {
        public HashSet<long> Nodes { get; set; }

        public HashSet<long> Values { get; set; }
    }
}
