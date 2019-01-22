using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB
{
    public interface ISequence
    {
         void Close();

         void DelSelf();

        void Flush(); 
    }
}
