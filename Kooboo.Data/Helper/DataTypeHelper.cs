//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Helper
{
public    class DataTypeHelper
    {

        public static Type ToClrType(DataTypes dataType)
        {
            switch (dataType)
            {
                case DataTypes.String:
                    return typeof(String);
                case DataTypes.Number:
                    return typeof(Decimal);
                case DataTypes.Decimal:
                    return typeof(Decimal);
                case DataTypes.DateTime:
                    return typeof(DateTime);
                case DataTypes.Bool:
                    return typeof(Boolean);
                case DataTypes.Guid:
                    return typeof(Guid);
                default:
                    return typeof(string);
            }
        }
         
        public static List<Comparer> GetSupportedComparers(DataTypes dataType)
        {
            switch (dataType)
            {
                case DataTypes.String:
                    return new List<Comparer>{ Comparer.EqualTo, Comparer.NotEqualTo, Comparer.StartWith, Comparer.Contains  };
                case DataTypes.Number:
                    return new List<Comparer> {Comparer.LessThan, Comparer.EqualTo, Comparer.NotEqualTo,  Comparer.LessThanOrEqual,   Comparer.GreaterThan, Comparer.GreaterThanOrEqual};
                case DataTypes.Decimal:
                    return new List<Comparer> { Comparer.LessThan, Comparer.EqualTo, Comparer.NotEqualTo, Comparer.LessThanOrEqual, Comparer.GreaterThan, Comparer.GreaterThanOrEqual };
                case DataTypes.DateTime:
                    return new List<Comparer> { Comparer.LessThan,Comparer.GreaterThan };
                case DataTypes.Bool:
                    return new List<Comparer> { Comparer.EqualTo, Comparer.NotEqualTo };
                case DataTypes.Guid:
                    return new List<Comparer> { Comparer.EqualTo, Comparer.NotEqualTo };
                     
                default:
                    // throw new NotSupportedException();
                    return new List<Comparer> { Comparer.EqualTo, Comparer.NotEqualTo};
            }
        }

        public static Dictionary<DataTypes, ComparerModel[]> GetDataTypeCompareModel()
        { 
            Dictionary<DataTypes, ComparerModel[]> result = new Dictionary<DataTypes, ComparerModel[]>();


            foreach (DataTypes dataType in Enum.GetValues(typeof(DataTypes)))
            {
                result.Add(dataType,  DataTypeHelper.GetSupportedComparers(dataType).Select(x => new ComparerModel
                {
                    Name = x.ToString(),
                    Symbol = Data.Helper.ComparerHelper.GetSymbol(x)
                }).ToArray());
            }
            return result;
             
        }


    }
}
