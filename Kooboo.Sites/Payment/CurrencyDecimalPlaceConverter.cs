using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Payment
{
    /// <summary>
    /// refer to  https://en.wikipedia.org/wiki/ISO_4217
    /// </summary>
    public class CurrencyDecimalPlaceConverter
    {
        private static readonly Dictionary<string, int> CurrencyCodes
            = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                {"AED", 2}, // UAE Dirham
                {"AFN", 2}, // Afghanistan Afghani
                {"ALL", 2}, // Albanian Lek
                {"AMD", 2}, // Armenian Dram
                {"ANG", 2}, // Netherlands Antillean Guilder
                {"AOA", 2}, // Angolan Kwanza
                {"ARS", 2}, // Argentine Peso
                {"AUD", 2}, // Australian Dollar
                {"AWG", 2}, // Aruban Guilder
                {"AZN", 2}, // Azerbaijanian Manat
                {"BAM", 2}, // Bosnia and Herzegovina Convertible Marks
                {"BBD", 2}, // Barbados Dollar
                {"BDT", 2}, // Bangladesh Taka
                {"BGN", 2}, // Bulgarian Lev
                {"BHD", 3}, // Bahraini Dinar
                {"BIF", 0}, // Burundi Franc
                {"BMD", 2}, // Bermudian Dollar
                {"BND", 2}, // Brunei Dollar
                {"BOB", 2}, // Bolivian Boliviano
                {"BOV", 2}, // Bolivian Mvdol
                {"BRL", 2}, // Brazilian Real
                {"BSD", 2}, // Bahamian Dollar
                {"BTN", 2}, // Bhutan Ngultrum
                {"BWP", 2}, // Botswana Pula
                {"BYR", 0}, // Belarussian Ruble
                {"BZD", 2}, // Belize Dollar
                {"CAD", 2}, // Canadian Dollar
                {"CDF", 2}, // Congolese Franc
                {"CHE", 2}, // WIR Euro
                {"CHF", 2}, // Swiss Franc
                {"CHW", 2}, // WIR Franc
                {"CLF", 4}, // Chilean Unidad de Fomento
                {"CLP", 0}, // Chilean Peso
                {"CNY", 2}, // Chinese Yuan Renminbi
                {"COP", 2}, // Colombian Peso
                {"COU", 2}, // Chilean Unidad de Valor Real
                {"CRC", 2}, // Costa Rican Colon
                {"CUC", 2}, // Peso Convertible
                {"CUP", 2}, // Cuban Peso
                {"CVE", 2}, // Cabo Verde Escudo
                {"CZK", 2}, // Czech Koruna
                {"DJF", 0}, // Djibouti Franc
                {"DKK", 2}, // Danish Krone
                {"DOP", 2}, // Dominican Peso
                {"DZD", 2}, // Algerian Dinar
                {"EGP", 2}, // Egyptian Pound
                {"ERN", 2}, // Eritrean Nafka
                {"ETB", 2}, // Ethiopian Birr
                {"EUR", 2}, // Euro
                {"FJD", 2}, // Fiji Dollar
                {"FKP", 2}, // Falkland Islands Pound
                {"GBP", 2}, // Pound Sterling
                {"GEL", 2}, // Georgian Lari
                {"GHS", 2}, // Ghana Cedi
                {"GIP", 2}, // Gibraltar Pound
                {"GMD", 2}, // Gambian Dalasi
                {"GNF", 0}, // Guinea Franc
                {"GTQ", 2}, // Guatemala Quetzal
                {"GYD", 2}, // Guyana Dollar
                {"HKD", 2}, // Hong Kong Dollar
                {"HNL", 2}, // Honduras Lempira
                {"HRK", 2}, // Croatian Kuna
                {"HTG", 2}, // Haiti Gourde
                {"HUF", 2}, // Hungarian Forint
                {"IDR", 2}, // Indonesian Rupiah
                {"ILS", 2}, // New Israeli Sheqel
                {"INR", 2}, // Indian Rupee
                {"IQD", 3}, // Iraqi Dinar
                {"IRR", 2}, // Iranian Rial
                {"ISK", 0}, // Iceland Krona
                {"JMD", 2}, // Jamaican Dollar
                {"JOD", 3}, // Jordanian Dinar
                {"JPY", 0}, // Japanese Yen
                {"KES", 2}, // Kenyan Shilling
                {"KGS", 2}, // Kyrgyzstan Som
                {"KHR", 2}, // Cambodia Riel
                {"KMF", 0}, // Comoro Franc
                {"KPW", 2}, // North Korean Won
                {"KRW", 0}, // Korean Won
                {"KWD", 3}, // Kuwaiti Dinar
                {"KYD", 2}, // Cayman Islands Dollar
                {"KZT", 2}, // Kazakhstan Tenge
                {"LAK", 2}, // Lao Kip
                {"LBP", 2}, // Lebanese Pound
                {"LKR", 2}, // Sri Lanka Rupee
                {"LRD", 2}, // Liberian Dollar
                {"LSL", 2}, // Lesotho Loti
                {"LYD", 3}, // Libyan Dinar
                {"MAD", 2}, // Moroccan Dirham
                {"MDL", 2}, // Moldovan Leu
                {"MGA", 2}, // Malagasy Ariary
                {"MKD", 2}, // Macedonian Denar
                {"MMK", 2}, // Myanmar Kyat
                {"MNT", 2}, // Mongolian Tugrik
                {"MOP", 2}, // Macau Pataca
                {"MRO", 2}, // Mauritania Ouguiya
                {"MUR", 2}, // Mauritius Rupee
                {"MVR", 2}, // Maldives Rufiyaa
                {"MWK", 2}, // Malawi Kwacha
                {"MXN", 2}, // Mexican Peso
                {"MXV", 2}, // Mexican Unidad de Inversion (UDI)
                {"MYR", 2}, // Malaysian Ringgit
                {"MZN", 2}, // Mozambique Metical
                {"NAD", 2}, // Namibia Dollar
                {"NGN", 2}, // Nigerian Naira
                {"NIO", 2}, // Nicaraguan Cordoba Oro
                {"NOK", 2}, // Norwegian Krone
                {"NPR", 2}, // Nepalese Rupee
                {"NZD", 2}, // New Zealand Dollar
                {"OMR", 3}, // Rial Omani
                {"PAB", 2}, // Panama Balboa
                {"PEN", 2}, // Peruvian Nuevo Sol
                {"PGK", 2}, // Papua New Guinea Kina
                {"PHP", 2}, // Philippine Peso
                {"PKR", 2}, // Pakistan Rupee
                {"PLN", 2}, // Polish Zloty
                {"PYG", 0}, // Paraguayan Guarani
                {"QAR", 2}, // Qatari Rial
                {"RON", 2}, // Romanian Leu
                {"RSD", 2}, // Serbian Dinar
                {"RUB", 2}, // Russian Ruble
                {"RWF", 0}, // Rwanda Franc
                {"SAR", 2}, // Saudi Riyal
                {"SBD", 2}, // Solomon Islands Dollar
                {"SCR", 2}, // Seychelles Rupee
                {"SDG", 2}, // Sudanese Pound
                {"SEK", 2}, // Swedish Krona
                {"SGD", 2}, // Singapore Dollar
                {"SHP", 2}, // Saint Helena Pound
                {"SLL", 2}, // Sierra Leonean Leone
                {"SOS", 2}, // Somali Shilling
                {"SRD", 2}, // Surinam Dollar
                {"SSP", 2}, // South Sudanese Pound
                {"STD", 2}, // S锟給 Tome and Principe Dobra
                {"SVC", 2}, // El Salvador Colon
                {"SYP", 2}, // Syrian Pound
                {"SZL", 2}, // Swaziland Lilangeni
                {"THB", 2}, // Thai Baht
                {"TJS", 2}, // Tajikistani Somoni
                {"TMT", 2}, // Turkmenistan New Manat
                {"TND", 3}, // Tunisian Dinar
                {"TOP", 2}, // Tonga Pa'anga
                {"TRY", 2}, // Turkish Lira
                {"TTD", 2}, // Trinidad and Tobago Dollar
                {"TWD", 2}, // New Taiwan Dollar
                {"TZS", 2}, // Tanzanian Shilling
                {"UAH", 2}, // Ukraine Hryvnia
                {"UGX", 0}, // Uganda Shilling
                {"USD", 2}, // US Dollar
                {"USN", 2}, // US Dollar (Next day)
                {"UYI", 0}, // Uruguay Peso en Unidades Indexadas (URUIURUI)
                {"UYU", 2}, // Peso Uruguayo
                {"UZS", 2}, // Uzbekistan Sum
                {"VEF", 2}, // Venezuelan Bolivar
                {"VND", 0}, // Vietnamese Dong
                {"VUV", 0}, // Vanuatu Vatu
                {"WST", 2}, // Samoa Tala
                {"XAF", 0}, // CFA Franc BEAC
                {"XCD", 2}, // East Caribbean Dollar
                {"XOF", 0}, // CFA Franc BCEAO
                {"XPF", 0}, // CFP Franc
                {"YER", 2}, // Yemeni Rial
                {"ZAR", 2}, // South African Rand
                {"ZMW", 2}, // Zambian Kwacha
                {"ZWL", 2}, // Zimbabwe Dollar
                //XAG	Silver	N.A.
                //XAU	Gold	N.A.
                //XBA	Bond Markets Unit European Composite Unit (EURCO)	N.A.
                //XBB	Bond Markets Unit European Monetary Unit (E.M.U.-6)	N.A.
                //XBC	Bond Markets Unit European Unit of Account 9 (E.U.A.-9)	N.A.
                //XBD	Bond Markets Unit European Unit of Account 17 (E.U.A.-17)	N.A.
                //XDR	SDR (Special Drawing Right)	N.A.
                //XPD	Palladium	N.A.
                //XPT	Platinum	N.A.
                //XSU	Sucre	N.A.
                //XTS	Codes specifically reserved for testing purposes	N.A.
                //XUA	ADB Unit of Account	N.A.
                //XXX	The codes assigned for transactions where no currency is involved	N.A.
            };

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
            return CurrencyCodes.TryGetValue(currencyCode, out var decimalPlaces) ? decimalPlaces : 2;
        }
    }
}
