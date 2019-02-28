//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Definition
{
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

    public class ComparerModel
    {
        public string Name { get; set; }

        public string Symbol { get; set; }
    }


}
