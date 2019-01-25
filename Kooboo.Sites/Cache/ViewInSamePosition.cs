//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Cache
{
    /// <summary>
    /// This is used to render detail view within the same position as list view
    /// </summary>
    public static class ViewInSamePosition
    {
        private static Dictionary<int, AlternativeView> cachelist = new Dictionary<int, AlternativeView>();

        private static Object _locker = new object();

        public static int GetAlternativeCode(Guid ListViewId, Guid DestinationViewId)
        {
            int key = ListViewId.GetHashCode();

            if (!cachelist.ContainsKey(key))
            {
                AlternativeView record = new AlternativeView();
                record.ListViewId = ListViewId;
                record.DestinationViewId = DestinationViewId;
                record.Id = key;
                cachelist[key] = record;
                return key;
            }
            else
            {
                while (cachelist.ContainsKey(key))
                {
                    var result = cachelist[key];
                    if (result.ListViewId == ListViewId && result.DestinationViewId == DestinationViewId)
                    {
                        return result.Id;
                    }
                    else
                    {
                        key = key.GetHashCode();
                    }
                } 
                AlternativeView record = new AlternativeView();
                record.ListViewId = ListViewId;
                record.DestinationViewId = DestinationViewId;
                record.Id = key;
                cachelist[key] = record;
                return key;
            }
        }

        public static Guid GetAlternaitiveViewId(int AlternativeId, Guid ListViewId)
        {
            if (cachelist.ContainsKey(AlternativeId))
            {
                var result = cachelist[AlternativeId]; 
                if (result.ListViewId == ListViewId)
                {
                    return result.DestinationViewId; 
                } 
            }

            return default(Guid); 
        }

        public class AlternativeView
        {
            public int Id { get; set; }

            public Guid ListViewId { get; set; }

            public Guid DestinationViewId { get; set; }

        }
    } 
  
}
