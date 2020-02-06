using Kooboo.Sites.Ecommerce.Promotion;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.Models
{
    public class PromotionRule : Kooboo.Sites.Contents.Models.MultipleLanguageObject
    {
        public string ConditionName { get; set; }

        public string Operator { get; set; }

        private List<string> _targetvalues;

        /// <summary>
        /// Value stored as string, should be precomputed. rules will be stored in memory. 
        /// </summary>
        public List<string> TargetValue
        {
            get
            {
                if (_targetvalues == null)
                {
                    _targetvalues = new List<string>();
                }
                return _targetvalues;
            }
            set
            {
                _targetvalues = value;
            }
        }

        public EnumPromotionTarget ForObject { get; set; }

        /// <summary>
        /// Can combined with other rules.
        /// </summary>
        public bool CanCombine { get; set; }

        public decimal Amount { get; set; } = 0;

        public decimal Percent { get; set; } = 0;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// active based on dates.
        /// </summary>
        public bool ByDate { get; set; }
    }
}
