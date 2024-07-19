//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Lib.Helper
{
    public static class CurrencyHelper
    {
        private static Dictionary<string, string> CurrencyDic = new Dictionary<string, string>();
        static CurrencyHelper()
        {
            CurrencyDic.Add("USD", "$");//United States dollar
            CurrencyDic.Add("EUR", "€");//Euro
            CurrencyDic.Add("JPY", "JPY¥");//Japanese yen
            CurrencyDic.Add("GBP", "£");//Pound sterling
            CurrencyDic.Add("AUD", "A$");//Australian dollar
            CurrencyDic.Add("CAD", "C$");//Canadian dollar
            CurrencyDic.Add("CHF", "Fr");//Swiss franc
            CurrencyDic.Add("CNY", "¥");//Renminbi
        }
        public static string GetCurrencySymbol(string currency)
        {
            currency = currency.ToUpper();
            if (CurrencyDic.ContainsKey(currency))
            {
                return CurrencyDic[currency];
            }
            throw new Exception(string.Format("currency:{0} is not supported.", currency));

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

        }
    }
}
