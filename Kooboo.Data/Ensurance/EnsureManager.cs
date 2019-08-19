using Kooboo.Data.Ensurance.Model;
using System;
using System.Collections.Generic;

namespace Kooboo.Data.Ensurance
{
    public static class EnsureManager
    {
        private static Log.LogWriter _logging;  
        private static Kooboo.Data.Log.LogWriter LoggingIn {
            get {
                if (_logging== null)
                {
                    lock(_locker)
                    {
                        _logging = new Log.LogWriter("Ensurance_in"); 
                    }
                }
                return _logging; 
            }
            set {
                _logging = value; 
            }
        }

        private static Log.LogWriter _logging_out;
        private static Kooboo.Data.Log.LogWriter LoggingOut
        {
            get
            {
                if (_logging_out == null)
                {
                    lock (_locker)
                    {
                        _logging_out = new Log.LogWriter("Ensurance_out");
                    }
                }
                return _logging_out;
            }
            set
            {
                _logging_out = value;
            }
        }
         

        private static object _locker = new object();
        private static object _exeLocker = new object();

        private static Dictionary<Guid, int> FailTimes = new Dictionary<Guid, int>();
        public static void Add(IQueueTask item)
        {
            EnsureObject obj = new EnsureObject();
            obj.ModelType = item.GetType().FullName;
            obj.Json = Lib.Helper.JsonHelper.Serialize(item);
            GlobalDb.Ensurance.AddOrUpdate(obj);

            // add log
            string log = obj.ModelType + ":\r\n";
            log += obj.Json;
            LoggingIn.Write(log); 
        }

        public static void Execute(List<EnsureObject> queueitems)
        {
            List<Guid> ItemToRemove = new List<Guid>();
            foreach (var item in queueitems)
            {
                try
                {
                    var executor = ExecuterContainer.GetExecutor(item.ModelType);

                    if (executor != null)
                    {
                        if (executor.Execute(item.Json))
                        { 
                            ItemToRemove.Add(item.Id);
                            // The out folder...
                            string log = item.ModelType + ":\r\n";
                            log += item.Json;
                            LoggingOut.Write(log); 
                        }
                        else
                        {
                            if (GetSetFailTimes(item))
                            {
                                ItemToRemove.Add(item.Id);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (GetSetFailTimes(item))
                    {
                        ItemToRemove.Add(item.Id);
                    }

                    Kooboo.Data.Log.Instance.Exception.Write(ex.Message + ex.StackTrace); 

                }
            }

            foreach (var item in ItemToRemove)
            {
                RemoveItem(item);
            }
        }

        public static void RemoveItem(Guid itemid)

        {
            Kooboo.Data.GlobalDb.Ensurance.Delete(itemid);
            FailTimes.Remove(itemid);
        }

        public static bool GetSetFailTimes(EnsureObject item)
        {
            int failedtimes = 0;
            if (FailTimes.ContainsKey(item.Id))
            {
                failedtimes = FailTimes[item.Id];
            }

            failedtimes += 1;

            if (failedtimes > 10)
            {
                // save for backup.
                string folder = Kooboo.Data.AppSettings.RootPath;
                folder = System.IO.Path.Combine(folder, "AppData");
                folder = System.IO.Path.Combine(folder, "EnsureLog");

                string filename = System.IO.Path.Combine(folder, Guid.NewGuid().ToString() + ".txt");

                Lib.Helper.IOHelper.EnsureFileDirectoryExists(filename);

                System.IO.File.WriteAllText(filename, Lib.Helper.JsonHelper.Serialize(item));
                return true;
            }
            else
            {
                FailTimes[item.Id] = failedtimes;
            }
            return false;
        }

        public static List<EnsureObject> AllItems()
        {
            return Kooboo.Data.GlobalDb.Ensurance.All();  
        }

        public static void Execute()
        {
            lock (_locker)
            {
                var all = AllItems();
                Execute(all);
            }
        }
    }
}
