using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Ensurance
{ 

    public interface IExecutor<T> : IExecutor
    {

    }

    public interface IExecutor
    {
        bool Execute(string JsonModel);
    }




}
