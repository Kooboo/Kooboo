using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Kooboo.Lib.Helper
{
    public class CurrencyHelper
    {
        public static string GetCurrencySymbol(string ISOCurrencySymbol)
        {

// 1

//United States dollar
//USD(US$)
//87.6 %
//2

//Euro
//EUR(€)
//31.4 %
//3

//Japanese yen
//JPY(¥)
//21.6 %
//4

//Pound sterling
//GBP(£)
//12.8 %
//5

//Australian dollar
//AUD(A$)
//6.9 %
//6

//Canadian dollar
//CAD(C$)
//5.1 %
//7

//Swiss franc
//CHF(Fr)
//4.8 %
//8

//Renminbi
//CNY(元)
//4.0 %




            string symbol = CultureInfo
                .GetCultures(CultureTypes.AllCultures)
                .Where(c => !c.IsNeutralCulture)
                .Select(culture => {
                    try
                    {
                        return new RegionInfo(culture.LCID);
                    }
                    catch
                    {
                        return null;
                    }
                })
                .Where(ri => ri != null && ri.ISOCurrencySymbol == ISOCurrencySymbol)
                .Select(ri => ri.CurrencySymbol)
                .FirstOrDefault();
            if (symbol == null)
                return string.Empty;

            return symbol;
        }
    }
}
