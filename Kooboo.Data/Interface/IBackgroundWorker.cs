using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Interface
{
    public interface IBackgroundWorker
    { 
        /// <summary>
        /// Interval in seconds. 
        /// </summary>
        int Interval { get;  }

        DateTime LastExecute { get; set; }

        void Execute();
    }
}
