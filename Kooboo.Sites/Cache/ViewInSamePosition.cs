//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Cache
{
    /// <summary>
    /// This is used to render detail view within the same position as list view
    /// </summary>
    public static class ViewInSamePosition
    {
        private static Dictionary<int, AlternativeView> cachelist = new Dictionary<int, AlternativeView>();

        private static Object _locker = new object();

        public static int GetAlternativeCode(Guid listViewId, Guid destinationViewId)
        {
            int key = listViewId.GetHashCode();

            if (!cachelist.ContainsKey(key))
            {
                AlternativeView record = new AlternativeView
                {
                    ListViewId = listViewId, DestinationViewId = destinationViewId, Id = key
                };
                cachelist[key] = record;
                return key;
            }
            else
            {
                while (cachelist.ContainsKey(key))
                {
                    var result = cachelist[key];
                    if (result.ListViewId == listViewId && result.DestinationViewId == destinationViewId)
                    {
                        return result.Id;
                    }
                    else
                    {
                        key = key.GetHashCode();
                    }
                }

                AlternativeView record = new AlternativeView
                {
                    ListViewId = listViewId, DestinationViewId = destinationViewId, Id = key
                };
                cachelist[key] = record;
                return key;
            }
        }

        public static Guid GetAlternaitiveViewId(int alternativeId, Guid listViewId)
        {
            if (cachelist.ContainsKey(alternativeId))
            {
                var result = cachelist[alternativeId];
                if (result.ListViewId == listViewId)
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