using System;

namespace Kooboo.Sites.Payment
{
    public class CurrencyDecimalPlaceConverter
    {
        #region Convert decimal to target format

        /// <summary>
        /// e.g. CNY|10.00 => 1000, CNY|10.01 => 1001; JPY|10.00 => 10, JPY|10.50 => 11
        /// </summary>
        public static long ToMinorUnit(string currencyCode, decimal amount)
        {
            var decimalPlace = GetDecimalPlace(currencyCode);
            if (decimalPlace == 0)
            {
                return (long)amount;
            }

            return (long)(amount * (int)Math.Pow(10, decimalPlace));
        }

        /// <summary>
        /// e.g. CNY|10.00 => "1000", CNY|10.01 => "1001"; JPY|10.00 => "10", JPY|10.50 => "11"
        /// </summary>
        public static string ToMinorUnitString(string currencyCode, decimal amount)
        {
            return ToMinorUnit(currencyCode, amount).ToString("#");
        }

        /// <summary>
        /// e.g. CNY|10.004 => 10.00, CNY|10.005 => 10.01, CNY|10 => 10.00; JPY|10.40 => 10, JPY|10.50 => 11
        /// </summary>
        public static decimal ToDecimalPlace(string currencyCode, decimal amount)
        {
            var decimalPlace = GetDecimalPlace(currencyCode);
            return Math.Round(amount * 1.000m, decimalPlace, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// e.g. CNY|10.004 => "10.00", CNY|10.005 => "10.01", CNY|10.0 => "10.00"; JPY|10.40 => "10", JPY|10.50 => "11"
        /// </summary>
        public static string ToDecimalPlaceString(string currencyCode, decimal amount)
        {
            return ToDecimalPlace(currencyCode, amount).ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        #endregion

        #region Convert target format to decimal

        /// <summary>
        /// e.g. CNY|1000 => 10.00,CNY|1001 => 10.01; JPY|1000 => 1000, JPY|1050 => 1050
        /// </summary>
        public static decimal FromMinorUnit(string currencyCode, long minorUnitAmount)
        {
            var decimalPlace = GetDecimalPlace(currencyCode);
            return Math.Round(minorUnitAmount * 1.000m, decimalPlace) / (decimal)Math.Pow(10, decimalPlace);
        }

        /// <summary>
        /// e.g. CNY|1000 => "10.00",CNY|1001 => "10.01"; JPY|1000 => "1000", JPY|1050 => "1050"
        /// </summary>
        public static string FromMinorUnitToDecimalString(string currencyCode, long minorUnitAmount)
        {
            var amount = FromMinorUnit(currencyCode, minorUnitAmount);
            return ToDecimalPlaceString(currencyCode, amount);
        }

        #endregion

        private static int GetDecimalPlace(string currencyCode)
        {
            return CurrencyCodes.GetDecimalPlace(currencyCode, 2).Value;
        }
    }
}
