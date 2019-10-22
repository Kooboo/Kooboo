//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Mail.Repositories
{
    public interface IRepository<TValue>
    {
        bool Add(TValue value);

        bool Update(TValue value);

        bool Delete(Int32 id);

        TValue Get(Int32 id);

        int Count();

        List<TValue> All(bool useColumnData = false);
    }
}