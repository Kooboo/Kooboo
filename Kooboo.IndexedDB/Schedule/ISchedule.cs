using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB
{
    public interface ISchedule
    {
        void Close();

        void DelSelf();

        string Folder { get; set; }
    }
}
