using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using Kooboo.Lib.Helper;

namespace Kooboo.Data.Repository
{
    public class IpCityRepository
    {
        private static string DbPath = System.IO.Path.GetFullPath("./countrystate/");
        public static Database Db = new Database(DbPath);
        public static ObjectStore<int, IPCity> GetStore()
        {
            //    ObjectStoreParameters paras = new ObjectStoreParameters();
            //    paras.AddColumn<IPCity>(i => i.Country);
            //    paras.AddColumn<IPCity>(i => i.State);
            //    paras.AddIndex<IPCity>(i => i.Begin);
            //    paras.AddIndex<IPCity>(i => i.End);
            return Db.GetOrCreateObjectStore<int, IPCity>("ipcity");
        }

        public static ObjectStore<string, int> GetStoreCount()
        {
            return Db.GetOrCreateObjectStore<string, int>("count");
        }
        public static IPCity GetIpCity(string clientIp)
        {
            int intip = IPHelper.ToInt(clientIp);
            var store = GetStore();
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            var total = GetStoreCount().get("count");
            Func<int, IPCity> getData = delegate (int index)
             {
                 return store.get(index);
             };
            //todo: it will search more than one thousand times which is  Worst condition.
            var model = BinarySearch(intip, total, 0, total - 1, getData);

            //index will search more than one million times（can't find better index for search）
            //combine index is not available
            //var model = store.Where(o => o.Begin <= intip && o.End >= intip)
            //    .FirstOrDefault();
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds);
            return model as IPCity;
        }

        public static IPCity BinarySearch(int ip, int total, int lowerBound, int upperBound, Func<int, IPCity> GetData)
        {
            if (upperBound > total - 1)
            {
                upperBound = total - 1;
            }

            if (lowerBound < 0)
            {
                lowerBound = 0;
            }
            if (upperBound - lowerBound <= 5)
            {
                // the last 5, just loop. 
                for (int i = lowerBound; i < upperBound; i++)
                {
                    var model = GetData(i);
                    if (model.Begin <= ip && model.End >= ip)
                    {
                        return model;
                    }
                }
                return default(IPCity);
            }
            else
            {
                int middle = (lowerBound + upperBound) / 2;

                var model = GetData(middle) as IPCity;
                if (ip >= model.Begin)
                {
                    if (ip <= model.End)
                    {
                        return model;
                    }
                    else
                    {
                        return BinarySearch(ip, total, middle, upperBound, GetData);
                    }
                }
                else
                {
                    return BinarySearch(ip, total, lowerBound, middle, GetData);
                }
            }
        }
    }
} 