//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Interface
{
    public interface ISiteObject
    {
        byte ConstType { get; set; }
         
        DateTime CreationDate { get; set; }

        DateTime LastModified { get; set; }

        Guid Id { get; set; }

        string Name { get; set; } 
    }
}
