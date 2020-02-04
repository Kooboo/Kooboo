using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.Promotion
{
    public class PromotionRules : CoreObject
    {
        public string ConditionName { get; set; }

        public string Operator { get; set; }

        /// <summary>
        /// Value stored as string, should be precomputed. rules will be stored in memory. 
        /// </summary>
        public List<string> TargetValue { get; set; }

        public decimal Amount { get; set; } = 0;

        public decimal Percent { get; set; } = 1;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// active based on dates.
        /// </summary>
        public bool ByDate { get; set; }
    }
}
