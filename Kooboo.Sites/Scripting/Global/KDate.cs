using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Scripting.Global
{
   public  class KDate
    {
        public DateTime UtcNow
        {
            get
            {
                return DateTime.UtcNow; 
            }
        }

        public DateTime Now
        {
            get
            {
                return DateTime.Now.ToLocalTime();
            }
        }
          
        public string Formate(DateTime dateTime, string formate)
        {
            if (string.IsNullOrEmpty(formate))
            {
                formate = "yyyy-MM-dd HH:mm:ss";
            } 
            return dateTime.ToString(formate);
        }

        public string Formate(DateTime dateTime)
        {
            return Formate(dateTime, null);
        }


    }
}
