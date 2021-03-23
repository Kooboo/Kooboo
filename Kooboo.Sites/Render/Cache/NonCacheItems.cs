using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Render
{
    //return as {{Guid}}
    public class NonCacheItems
    {
        private static object _locker = new object();

        internal static Dictionary<Guid, Dictionary<Guid, IRenderTask>> SiteNonCacheItems { get; set; }

        static NonCacheItems()
        {
            SiteNonCacheItems = new Dictionary<Guid, Dictionary<Guid, IRenderTask>>();
        }

        internal static Dictionary<Guid, IRenderTask> GetSiteCaches(Guid SiteId)
        {
            if (!SiteNonCacheItems.ContainsKey(SiteId))
            {
                lock (_locker)
                {
                    Dictionary<Guid, IRenderTask> sitecache = new Dictionary<Guid, IRenderTask>();
                    SiteNonCacheItems.Add(SiteId, sitecache);
                }
            }
            return SiteNonCacheItems[SiteId];
        }

        private static string StartMark = "{{k-";
        private static string EndMark = "-}}";

        private static int startlen = StartMark.Length;
        private static int endlen = EndMark.Length;


        public static string Set(Guid SiteId, IRenderTask task)
        {
            string unique = task.GetType().Name.ToString() + task.GetHashCode().ToString();
            var guid = Lib.Security.Hash.ComputeHashGuid(unique);

            var sitecache = GetSiteCaches(SiteId);

            if (!sitecache.ContainsKey(guid))
            {
                sitecache[guid] = task;
            }
            return StartMark + guid.ToString() + EndMark;
        }

        public static IRenderTask Get(Guid SiteId, Guid Id)
        {
            var sitecache = GetSiteCaches(SiteId);
            if (sitecache.ContainsKey(Id))
            {
                return sitecache[Id];
            }
            return null;
        }

        public static Guid PraseGuid(string text)
        {
            text = text.Replace(StartMark, "");
            text = text.Replace(EndMark, "");

            if (System.Guid.TryParse(text, out Guid id))
            {
                return id;
            }
            return default(Guid);
        }

        public static string Render(Guid SiteId, Guid Id, RenderContext Context)
        {
            var task = Get(SiteId, Id);
            if (task != null)
            {
                return task.Render(Context);
            }
            return null;
        }

        public static string Render(string Orginal, RenderContext context)
        {
            string result = string.Empty;
            int currentpos = 0;
            var startindex = Orginal.IndexOf(StartMark); 

            while (startindex > 0)
            {
                var endindex = Orginal.IndexOf(EndMark, startindex);

                if (endindex > -1)
                {
                    result += Orginal.Substring(currentpos, startindex);
                    // render the value.  
                    var midText = Orginal.Substring(startindex, endindex - startindex + endlen);

                    var id = PraseGuid(midText);
                     
                    var renderresult = Render(context.WebSite.Id, id, context);
                    if (renderresult != null)
                    {
                        result += renderresult;
                    } 
                    currentpos = endindex + endlen;
                }

                  startindex = Orginal.IndexOf(StartMark,currentpos);
            }

            if (currentpos > 0)
            {
                result += Orginal.Substring(currentpos);
                return result;
            }
            else
            {
                return Orginal;
            }
        }

    }
}
