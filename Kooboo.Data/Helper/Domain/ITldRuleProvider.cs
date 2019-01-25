//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nager.PublicSuffix
{
    public interface ITldRuleProvider
    {
        /// <summary>
        /// Builds the list of TldRules
        /// </summary>
        /// <returns>List of TldRules</returns>

        Task<IEnumerable<TldRule>> BuildAsync();
    }
}
