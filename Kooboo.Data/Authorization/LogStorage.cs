using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Authorization
{
  public static class LogStorage
    {
        static LogStorage()
        {
            currentDate = DateTime.Now;
            // email setting.
            Kooboo.IndexedDB.Dynamic.Setting setting = new IndexedDB.Dynamic.Setting(); 
            setting.SetPrimaryKey("OrgId", typeof(Guid)); 

            IndexedDB.Dynamic.TableColumn col = new IndexedDB.Dynamic.TableColumn();
            col.Name = "Count";
            col.DataType = typeof(int).FullName; 
            setting.AddColumn(col);

            emailsetting = setting;  

        }

        private static object _locker = new object();

        private static object _dblocker = new object(); 
         
        private static DateTime currentDate { get; set; }


        private static Kooboo.IndexedDB.Dynamic.Setting emailsetting { get; set; }


        private static string Folder()
        {
            return currentDate.Year.ToString() + currentDate.Month.ToString() + currentDate.Day.ToString();  
        }

        private static Kooboo.IndexedDB.Database _db; 
         
        private static Kooboo.IndexedDB.Database getDb(DateTime date)
        {
            var folder =  Folder();
            folder = "global" + "\\logging\\" +  folder; 
            return new Kooboo.IndexedDB.Database(folder);  
        }

        private static Kooboo.IndexedDB.Database DB {

            get
            {
                var now = DateTime.Now; 
                if (now.DayOfYear != currentDate.DayOfYear)
                {
                    lock(_locker)
                    {
                        if (now.DayOfYear != currentDate.DayOfYear)
                        { 
                            if (_db !=null)
                            {
                                _db.Close();
                                _db = null;
                            }

                            currentDate = now;
                            _db = getDb(currentDate); 
                        }
                    }
                }

               if (_db == null)
                {
                    _db = getDb(now); 
                }
                return _db; 
            }
            set { _db = value;  }
        }

         
        public static Kooboo.IndexedDB.Dynamic.Table EmailLog
        {
            get
            { 
                return DB.GetOrCreateTable("EmailLog", emailsetting); 
            }
        }

    }
}
