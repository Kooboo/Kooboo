using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kooboo.Sites.Payment
{
    /// <summary>
    /// refer to  https://en.wikipedia.org/wiki/ISO_4217
    /// </summary>
    public class CurrencyCodes
    {
        private static readonly IReadOnlyDictionary<string, CurrencyCode> Codes =
            new ReadOnlyDictionary<string, CurrencyCode>(
                new Dictionary<string, CurrencyCode>(StringComparer.OrdinalIgnoreCase)
                {
                    { "AED", new CurrencyCode("AED", "784", 2) }, //United Arab Emirates dirham
                    { "AFN", new CurrencyCode("AFN", "971", 2) }, //Afghan afghani
                    { "ALL", new CurrencyCode("ALL", "008", 2) }, //Albanian lek
                    { "AMD", new CurrencyCode("AMD", "051", 2) }, //Armenian dram
                    { "ANG", new CurrencyCode("ANG", "532", 2) }, //Netherlands Antillean guilder
                    { "AOA", new CurrencyCode("AOA", "973", 2) }, //Angolan kwanza
                    { "ARS", new CurrencyCode("ARS", "032", 2) }, //Argentine peso
                    { "AUD", new CurrencyCode("AUD", "036", 2) }, //Australian dollar
                    { "AWG", new CurrencyCode("AWG", "533", 2) }, //Aruban florin
                    { "AZN", new CurrencyCode("AZN", "944", 2) }, //Azerbaijani manat
                    { "BAM", new CurrencyCode("BAM", "977", 2) }, //Bosnia and Herzegovina convertible mark
                    { "BBD", new CurrencyCode("BBD", "052", 2) }, //Barbados dollar
                    { "BDT", new CurrencyCode("BDT", "050", 2) }, //Bangladeshi taka
                    { "BGN", new CurrencyCode("BGN", "975", 2) }, //Bulgarian lev
                    { "BHD", new CurrencyCode("BHD", "048", 3) }, //Bahraini dinar
                    { "BIF", new CurrencyCode("BIF", "108", 0) }, //Burundian franc
                    { "BMD", new CurrencyCode("BMD", "060", 2) }, //Bermudian dollar
                    { "BND", new CurrencyCode("BND", "096", 2) }, //Brunei dollar
                    { "BOB", new CurrencyCode("BOB", "068", 2) }, //Boliviano
                    { "BOV", new CurrencyCode("BOV", "984", 2) }, //Bolivian Mvdol (funds code)
                    { "BRL", new CurrencyCode("BRL", "986", 2) }, //Brazilian real
                    { "BSD", new CurrencyCode("BSD", "044", 2) }, //Bahamian dollar
                    { "BTN", new CurrencyCode("BTN", "064", 2) }, //Bhutanese ngultrum
                    { "BWP", new CurrencyCode("BWP", "072", 2) }, //Botswana pula
                    { "BYN", new CurrencyCode("BYN", "933", 2) }, //Belarusian ruble
                    { "BZD", new CurrencyCode("BZD", "084", 2) }, //Belize dollar
                    { "CAD", new CurrencyCode("CAD", "124", 2) }, //Canadian dollar
                    { "CDF", new CurrencyCode("CDF", "976", 2) }, //Congolese franc
                    { "CHE", new CurrencyCode("CHE", "947", 2) }, //WIR Euro (complementary currency)
                    { "CHF", new CurrencyCode("CHF", "756", 2) }, //Swiss franc
                    { "CHW", new CurrencyCode("CHW", "948", 2) }, //WIR Franc (complementary currency)
                    { "CLF", new CurrencyCode("CLF", "990", 4) }, //Unidad de Fomento (funds code)
                    { "CLP", new CurrencyCode("CLP", "152", 0) }, //Chilean peso
                    { "CNY", new CurrencyCode("CNY", "156", 2) }, //Renminbi (Chinese) yuan[7]
                    { "COP", new CurrencyCode("COP", "170", 2) }, //Colombian peso
                    { "COU", new CurrencyCode("COU", "970", 2) }, //Unidad de Valor Real (UVR) (funds code)[8]
                    { "CRC", new CurrencyCode("CRC", "188", 2) }, //Costa Rican colon
                    { "CUC", new CurrencyCode("CUC", "931", 2) }, //Cuban convertible peso
                    { "CUP", new CurrencyCode("CUP", "192", 2) }, //Cuban peso
                    { "CVE", new CurrencyCode("CVE", "132", 2) }, //Cape Verdean escudo
                    { "CZK", new CurrencyCode("CZK", "203", 2) }, //Czech koruna
                    { "DJF", new CurrencyCode("DJF", "262", 0) }, //Djiboutian franc
                    { "DKK", new CurrencyCode("DKK", "208", 2) }, //Danish krone
                    { "DOP", new CurrencyCode("DOP", "214", 2) }, //Dominican peso
                    { "DZD", new CurrencyCode("DZD", "012", 2) }, //Algerian dinar
                    { "EGP", new CurrencyCode("EGP", "818", 2) }, //Egyptian pound
                    { "ERN", new CurrencyCode("ERN", "232", 2) }, //Eritrean nakfa
                    { "ETB", new CurrencyCode("ETB", "230", 2) }, //Ethiopian birr
                    { "EUR", new CurrencyCode("EUR", "978", 2) }, //Euro
                    { "FJD", new CurrencyCode("FJD", "242", 2) }, //Fiji dollar
                    { "FKP", new CurrencyCode("FKP", "238", 2) }, //Falkland Islands pound
                    { "GBP", new CurrencyCode("GBP", "826", 2) }, //Pound sterling
                    { "GEL", new CurrencyCode("GEL", "981", 2) }, //Georgian lari
                    { "GHS", new CurrencyCode("GHS", "936", 2) }, //Ghanaian cedi
                    { "GIP", new CurrencyCode("GIP", "292", 2) }, //Gibraltar pound
                    { "GMD", new CurrencyCode("GMD", "270", 2) }, //Gambian dalasi
                    { "GNF", new CurrencyCode("GNF", "324", 0) }, //Guinean franc
                    { "GTQ", new CurrencyCode("GTQ", "320", 2) }, //Guatemalan quetzal
                    { "GYD", new CurrencyCode("GYD", "328", 2) }, //Guyanese dollar
                    { "HKD", new CurrencyCode("HKD", "344", 2) }, //Hong Kong dollar
                    { "HNL", new CurrencyCode("HNL", "340", 2) }, //Honduran lempira
                    { "HRK", new CurrencyCode("HRK", "191", 2) }, //Croatian kuna
                    { "HTG", new CurrencyCode("HTG", "332", 2) }, //Haitian gourde
                    { "HUF", new CurrencyCode("HUF", "348", 2) }, //Hungarian forint
                    { "IDR", new CurrencyCode("IDR", "360", 2) }, //Indonesian rupiah
                    { "ILS", new CurrencyCode("ILS", "376", 2) }, //Israeli new shekel
                    { "INR", new CurrencyCode("INR", "356", 2) }, //Indian rupee
                    { "IQD", new CurrencyCode("IQD", "368", 3) }, //Iraqi dinar
                    { "IRR", new CurrencyCode("IRR", "364", 2) }, //Iranian rial
                    { "ISK", new CurrencyCode("ISK", "352", 0) }, //Icelandic króna
                    { "JMD", new CurrencyCode("JMD", "388", 2) }, //Jamaican dollar
                    { "JOD", new CurrencyCode("JOD", "400", 3) }, //Jordanian dinar
                    { "JPY", new CurrencyCode("JPY", "392", 0) }, //Japanese yen
                    { "KES", new CurrencyCode("KES", "404", 2) }, //Kenyan shilling
                    { "KGS", new CurrencyCode("KGS", "417", 2) }, //Kyrgyzstani som
                    { "KHR", new CurrencyCode("KHR", "116", 2) }, //Cambodian riel
                    { "KMF", new CurrencyCode("KMF", "174", 0) }, //Comoro franc
                    { "KPW", new CurrencyCode("KPW", "408", 2) }, //North Korean won
                    { "KRW", new CurrencyCode("KRW", "410", 0) }, //South Korean won
                    { "KWD", new CurrencyCode("KWD", "414", 3) }, //Kuwaiti dinar
                    { "KYD", new CurrencyCode("KYD", "136", 2) }, //Cayman Islands dollar
                    { "KZT", new CurrencyCode("KZT", "398", 2) }, //Kazakhstani tenge
                    { "LAK", new CurrencyCode("LAK", "418", 2) }, //Lao kip
                    { "LBP", new CurrencyCode("LBP", "422", 2) }, //Lebanese pound
                    { "LKR", new CurrencyCode("LKR", "144", 2) }, //Sri Lankan rupee
                    { "LRD", new CurrencyCode("LRD", "430", 2) }, //Liberian dollar
                    { "LSL", new CurrencyCode("LSL", "426", 2) }, //Lesotho loti
                    { "LYD", new CurrencyCode("LYD", "434", 3) }, //Libyan dinar
                    { "MAD", new CurrencyCode("MAD", "504", 2) }, //Moroccan dirham
                    { "MDL", new CurrencyCode("MDL", "498", 2) }, //Moldovan leu
                    { "MGA", new CurrencyCode("MGA", "969", 2) }, //Malagasy ariary
                    { "MKD", new CurrencyCode("MKD", "807", 2) }, //Macedonian denar
                    { "MMK", new CurrencyCode("MMK", "104", 2) }, //Myanmar kyat
                    { "MNT", new CurrencyCode("MNT", "496", 2) }, //Mongolian tögrög
                    { "MOP", new CurrencyCode("MOP", "446", 2) }, //Macanese pataca
                    { "MRU", new CurrencyCode("MRU", "929", 2) }, //Mauritanian ouguiya
                    { "MUR", new CurrencyCode("MUR", "480", 2) }, //Mauritian rupee
                    { "MVR", new CurrencyCode("MVR", "462", 2) }, //Maldivian rufiyaa
                    { "MWK", new CurrencyCode("MWK", "454", 2) }, //Malawian kwacha
                    { "MXN", new CurrencyCode("MXN", "484", 2) }, //Mexican peso
                    { "MXV", new CurrencyCode("MXV", "979", 2) }, //Mexican Unidad de Inversion (UDI) (funds code)
                    { "MYR", new CurrencyCode("MYR", "458", 2) }, //Malaysian ringgit
                    { "MZN", new CurrencyCode("MZN", "943", 2) }, //Mozambican metical
                    { "NAD", new CurrencyCode("NAD", "516", 2) }, //Namibian dollar
                    { "NGN", new CurrencyCode("NGN", "566", 2) }, //Nigerian naira
                    { "NIO", new CurrencyCode("NIO", "558", 2) }, //Nicaraguan córdoba
                    { "NOK", new CurrencyCode("NOK", "578", 2) }, //Norwegian krone
                    { "NPR", new CurrencyCode("NPR", "524", 2) }, //Nepalese rupee
                    { "NZD", new CurrencyCode("NZD", "554", 2) }, //New Zealand dollar
                    { "OMR", new CurrencyCode("OMR", "512", 3) }, //Omani rial
                    { "PAB", new CurrencyCode("PAB", "590", 2) }, //Panamanian balboa
                    { "PEN", new CurrencyCode("PEN", "604", 2) }, //Peruvian sol
                    { "PGK", new CurrencyCode("PGK", "598", 2) }, //Papua New Guinean kina
                    { "PHP", new CurrencyCode("PHP", "608", 2) }, //Philippine peso[13]
                    { "PKR", new CurrencyCode("PKR", "586", 2) }, //Pakistani rupee
                    { "PLN", new CurrencyCode("PLN", "985", 2) }, //Polish złoty
                    { "PYG", new CurrencyCode("PYG", "600", 0) }, //Paraguayan guaraní
                    { "QAR", new CurrencyCode("QAR", "634", 2) }, //Qatari riyal
                    { "RON", new CurrencyCode("RON", "946", 2) }, //Romanian leu
                    { "RSD", new CurrencyCode("RSD", "941", 2) }, //Serbian dinar
                    { "RUB", new CurrencyCode("RUB", "643", 2) }, //Russian ruble
                    { "RWF", new CurrencyCode("RWF", "646", 0) }, //Rwandan franc
                    { "SAR", new CurrencyCode("SAR", "682", 2) }, //Saudi riyal
                    { "SBD", new CurrencyCode("SBD", "090", 2) }, //Solomon Islands dollar
                    { "SCR", new CurrencyCode("SCR", "690", 2) }, //Seychelles rupee
                    { "SDG", new CurrencyCode("SDG", "938", 2) }, //Sudanese pound
                    { "SEK", new CurrencyCode("SEK", "752", 2) }, //Swedish krona/kronor
                    { "SGD", new CurrencyCode("SGD", "702", 2) }, //Singapore dollar
                    { "SHP", new CurrencyCode("SHP", "654", 2) }, //Saint Helena pound
                    { "SLL", new CurrencyCode("SLL", "694", 2) }, //Sierra Leonean leone
                    { "SOS", new CurrencyCode("SOS", "706", 2) }, //Somali shilling
                    { "SRD", new CurrencyCode("SRD", "968", 2) }, //Surinamese dollar
                    { "SSP", new CurrencyCode("SSP", "728", 2) }, //South Sudanese pound
                    { "STN", new CurrencyCode("STN", "930", 2) }, //São Tomé and Príncipe dobra
                    { "SVC", new CurrencyCode("SVC", "222", 2) }, //Salvadoran colón
                    { "SYP", new CurrencyCode("SYP", "760", 2) }, //Syrian pound
                    { "SZL", new CurrencyCode("SZL", "748", 2) }, //Swazi lilangeni
                    { "THB", new CurrencyCode("THB", "764", 2) }, //Thai baht
                    { "TJS", new CurrencyCode("TJS", "972", 2) }, //Tajikistani somoni
                    { "TMT", new CurrencyCode("TMT", "934", 2) }, //Turkmenistan manat
                    { "TND", new CurrencyCode("TND", "788", 3) }, //Tunisian dinar
                    { "TOP", new CurrencyCode("TOP", "776", 2) }, //Tongan paʻanga
                    { "TRY", new CurrencyCode("TRY", "949", 2) }, //Turkish lira
                    { "TTD", new CurrencyCode("TTD", "780", 2) }, //Trinidad and Tobago dollar
                    { "TWD", new CurrencyCode("TWD", "901", 2) }, //New Taiwan dollar
                    { "TZS", new CurrencyCode("TZS", "834", 2) }, //Tanzanian shilling
                    { "UAH", new CurrencyCode("UAH", "980", 2) }, //Ukrainian hryvnia
                    { "UGX", new CurrencyCode("UGX", "800", 0) }, //Ugandan shilling
                    { "USD", new CurrencyCode("USD", "840", 2) }, //United States dollar
                    { "USN", new CurrencyCode("USN", "997", 2) }, //United States dollar (next day) (funds code)
                    { "UYI", new CurrencyCode("UYI", "940", 0) }, //Uruguay Peso en Unidades Indexadas (URUIURUI) (funds code)
                    { "UYU", new CurrencyCode("UYU", "858", 2) }, //Uruguayan peso
                    { "UYW", new CurrencyCode("UYW", "927", 4) }, //Unidad previsional[15]
                    { "UZS", new CurrencyCode("UZS", "860", 2) }, //Uzbekistan som
                    { "VES", new CurrencyCode("VES", "928", 2) }, //Venezuelan bolívar soberano[13]
                    { "VND", new CurrencyCode("VND", "704", 0) }, //Vietnamese đồng
                    { "VUV", new CurrencyCode("VUV", "548", 0) }, //Vanuatu vatu
                    { "WST", new CurrencyCode("WST", "882", 2) }, //Samoan tala
                    { "XAF", new CurrencyCode("XAF", "950", 0) }, //CFA franc BEAC
                    // XAG  961  N/A  Silver (one troy ounce)
                    // XAU  959  N/A  Gold (one troy ounce)
                    // XBA  955  N/A  European Composite Unit
                    // XBB  956  N/A  European Monetary Unit (E.M.U.-6) (bond market unit)
                    // XBC  957  N/A  European Unit of Account 9 (E.U.A.-9) (bond market unit)
                    // XBD  958  N/A  European Unit of Account 17 (E.U.A.-17) (bond market unit)
                    { "XCD", new CurrencyCode("XCD", "951", 2) }, //East Caribbean dollar
                    // XDR  960  N/A  Special drawing rights
                    { "XOF", new CurrencyCode("XOF", "952", 0) }, //CFA franc BCEAO
                    // XPD  964  N/A  Palladium (one troy ounce)
                    { "XPF", new CurrencyCode("XPF", "953", 0) }, //CFP franc (franc Pacifique)
                    // XPT  962  N/A  Platinum (one troy ounce)
                    // XSU  994  N/A  SUCRE
                    // XTS  963  N/A  Code reserved for testing
                    // XUA  965  N/A  ADB Unit of Account
                    // XXX  999  N/A  No currency
                    { "YER", new CurrencyCode("YER", "886", 2) }, //Yemeni rial
                    { "ZAR", new CurrencyCode("ZAR", "710", 2) }, //South African rand
                    { "ZMW", new CurrencyCode("ZMW", "967", 2) }, //Zambian kwacha
                    { "ZWL", new CurrencyCode("ZWL", "932", 2) }, //Zimbabwean dollar
                });

        public static CurrencyCode GetCurrencyCode(string alphabeticCode)
        {
            Codes.TryGetValue(alphabeticCode, out var code);
            return code;
        }

        public static CurrencyCode GetCurrencyCodeByNumericCode(string numericCode)
        {
            return Codes.Values.FirstOrDefault(x => x.NumericCode == numericCode);
        }

        /// <param name="alphabeticCode">e.g. CNY, USD, JPY, EUR...</param>
        /// <param name="fallbackNumericCode">if not found, return <paramref name="fallbackNumericCode"/>.</param>
        public static string GetNumericCode(string alphabeticCode, string fallbackNumericCode = null)
        {
            var code = GetCurrencyCode(alphabeticCode);
            return code?.NumericCode ?? fallbackNumericCode;
        }

        /// <param name="numericCode">e.g. 156->CNY, 840->USD, 392->JPY, 978->EUR...</param>
        /// <param name="fallbackAlphabeticCode">if not found, return <paramref name="fallbackAlphabeticCode"/>.</param>
        public static string GetAlphabeticCode(string numericCode, string fallbackAlphabeticCode = null)
        {
            var code = GetCurrencyCodeByNumericCode(numericCode);
            return code?.AlphabeticCode ?? fallbackAlphabeticCode;
        }

        /// <param name="alphabeticCode">e.g. CNY, USD, JPY, EUR...</param>
        /// <param name="fallbackDecimalPlace">if not found, return <paramref name="fallbackDecimalPlace"/>.</param>
        public static int? GetDecimalPlace(string alphabeticCode, int? fallbackDecimalPlace = null)
        {
            var code = GetCurrencyCode(alphabeticCode);
            return code?.DecimalPlace ?? fallbackDecimalPlace;
        }

        /// <param name="numericCode">e.g. 156->CNY, 840->USD, 392->JPY, 978->EUR...</param>
        /// <param name="fallbackDecimalPlace">if not found, return <paramref name="fallbackDecimalPlace"/>.</param>
        public static int? GetDecimalPlaceByNumericCode(string numericCode, int? fallbackDecimalPlace = null)
        {
            var code = GetCurrencyCodeByNumericCode(numericCode);
            return code?.DecimalPlace ?? fallbackDecimalPlace;
        }

        public class CurrencyCode
        {
            public CurrencyCode(string alphabeticCode, string numericCode, int decimalPlace)
            {
                AlphabeticCode = alphabeticCode;
                NumericCode = numericCode;
                DecimalPlace = decimalPlace;
            }

            public string AlphabeticCode { get; set; }

            public string NumericCode { get; set; }

            public int DecimalPlace { get; set; }
        }
    }
}
