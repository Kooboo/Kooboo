//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB
{
    public interface IObjectStore
    {
        string Name { get; set; }
        string ObjectFolder { get; set; }
        int Count();
        void Close();
        void DelSelf();

        void Flush();

        Database OwnerDatabase { get; set; }

        bool add(object key, object value);

        bool update(object key, object value);

        void delete(object key);

        object get(object key);

        List<object> List(int count = 9999, int skip = 0);

        void RollBack(LogEntry log);

        void RollBack(List<LogEntry> loglist);

        void RollBack(Int64 LastVersionId, bool SelfIncluded = true);

        void RollBackTimeTick(Int64 TimeTick, bool SelfIncluded = true);

        void CheckOut(Int64 VersionId, IObjectStore DestinationStore, bool SelfIncluded = true);

        void CheckOut(List<LogEntry> logs, IObjectStore DestinationStore);

        int getLength(long blockposition);

    }
}
