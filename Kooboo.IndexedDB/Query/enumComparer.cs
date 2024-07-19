//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.IndexedDB.Query
{
    /// <summary>
    /// The search comparison type.
    /// StartWith, Contains, only works for string columns. 
    /// The evaluator of compare type will be tested by a MatchEvaluator. 
    /// </summary>
    public enum Comparer
    {
        EqualTo = 0,
        GreaterThan = 1,
        GreaterThanOrEqual = 2,
        LessThan = 3,
        LessThanOrEqual = 4,
        NotEqualTo = 5,
        StartWith = 6,
        Contains = 7,
    }
}
