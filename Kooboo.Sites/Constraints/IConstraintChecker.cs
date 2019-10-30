//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Sites.Repository;
using System.Collections.Generic;

namespace Kooboo.Sites.Constraints
{
    public interface IConstraintChecker<T> : IMetaProvider<DisplayMetaInfo> where T : ISiteObject
    {
        /// <summary>
        /// </summary>
        /// <param name="siteDb"></param>
        /// <param name="siteObject"></param>
        /// <param name="language">This is only used by text content language</param>
        void Fix(SiteDb siteDb, T siteObject, string language = null);

        /// <summary>
        /// verify and return the error messages.
        /// </summary>
        /// <param name="siteDb"></param>
        /// <param name="siteObject"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        List<ConstraintResponse> Check(SiteDb siteDb, T siteObject, string language = null);

        bool HasFix { get; }

        bool HasCheck { get; }

        bool AutoFixOnSave { get; }
    }
}