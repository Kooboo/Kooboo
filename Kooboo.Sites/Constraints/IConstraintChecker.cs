//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Constraints
{
  public  interface IConstraintChecker<T> : IMetaProvider<DisplayMetaInfo> where T: ISiteObject
    {
      /// <summary> 
    /// </summary>
    /// <param name="SiteDb"></param>
    /// <param name="SiteObject"></param>
    /// <param name="Language">This is only used by text content language</param>
        void Fix(SiteDb SiteDb, T SiteObject, string Language=null);
      
        /// <summary>
        /// verify and return the error messages. 
        /// </summary>
        /// <param name="SiteDb"></param>
        /// <param name="SiteObjectg"></param>
        /// <param name="Language"></param>
        /// <returns></returns>
        List<ConstraintResponse> Check(SiteDb SiteDb, T SiteObject, string Language = null);

        bool HasFix { get; }

        bool HasCheck { get; }
  
        bool AutoFixOnSave { get;}
    }
}
