//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;

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