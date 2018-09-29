using Kooboo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Automation.Ranking
{
    [Serializable]
    public class RankingRule
    {
        private Guid _id;
        public Guid Id
        {
            set { _id = value; }
            get
            {
                if (_id == default(Guid))
                {
                    _id = IDGenerator.GetRankingRuleId(this.SiteObjectType, this.CompareType.ToString(), this.CompareValue);
                }
                return _id;
            }
        }

        /// <summary>
        /// The type of site object that this rule was created for. 
        /// </summary>
        public byte SiteObjectType { get; set; }

        /// <summary>
        /// The name of notation that will use this facgtor. 
        /// </summary>
        public string NotationName { get; set; }

        public CompareType CompareType { get; set; }


        /// <summary>
        /// The value that is used to compare with the value return from Notation. 
        /// This value type must match the type from notation. 
        /// </summary>
        public object CompareValue { get; set; }

        /// <summary>
        /// The value to add, can be negative.
        /// </summary>
        public int AddedValue { get; set; }
    }

    [Serializable]
    public enum CompareType
    {
        Equal = 0,
        GreaterThan = 1,
        GreaterThanOrEqual = 2,
        LessThan = 3,
        LessThanOrEqual = 4,
        Contains = 5
    }

    //[Serializable]
    //public enum ResultOperator
    //{
    //    PLUS = 0,
    //    MINUS = 1,
    //    MULTIPLE = 2,
    //    DIVIDE = 3
    //}

}
