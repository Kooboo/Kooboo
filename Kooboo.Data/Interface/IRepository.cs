//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using Kooboo.IndexedDB.Query;
using System;
using System.Collections.Generic;


namespace Kooboo.Data.Interface
{
    public interface IRepository<TValue> 
    {
        bool AddOrUpdate(TValue value);

        void Delete(Guid id);

         TValue Get(Guid id, bool getColumnDataOnly = false);

        WhereFilter<Guid, TValue> Query { get; }

        int Count();

        List<TValue> All(bool UseColumnData = false); 

    }

    public interface IRepository
    {
        bool AddOrUpdate(object value, Guid UserId = default(Guid));

        void Delete(Guid id, Guid UserId = default(Guid));
  
        ISiteObject Get(Guid id, bool getColumnDataOnly = false);

        ISiteObject GetByNameOrId(string NameOrId); 

        List<ISiteObject> All(bool UseColumnData=false);

        /// <summary>
        /// Init the repository. 
        /// </summary>
        // void Init();
        ISiteObject GetByLog(LogEntry log); 

        void RollBack(LogEntry log);

        void RollBack(List<LogEntry> loglist); 
         
        /// <summary>
        /// Get the last entry from log, this is used in the case that it is a deletion on last edit... 
        /// </summary>
        /// <param name="ObjectId"></param>
        /// <returns></returns>
        ISiteObject GetLastEntryFromLog(Guid ObjectId); 

        Type ModelType { get; set; }
       
        string StoreName { get; }

        IObjectStore Store { get;  }
         
        void CheckOut(Int64 VersionId, IRepository DestinationRepository, bool SelfIncluded);

        List<UsedByRelation> GetUsedBy(Guid ObjectId);
    }
    
}
