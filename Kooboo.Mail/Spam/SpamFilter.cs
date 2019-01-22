using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Mail.Spam
{
   public static class SpamFilter
    {
        public static Folder DetermineFolder()
        {
            return new Folder("Inbox"); 
        }
    }
}
