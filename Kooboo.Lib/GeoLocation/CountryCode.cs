//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Lib.GeoLocation
{
    public static class CountryCode
    {
        static CountryCode()
        {
            CodeNameList = new Dictionary<string, string>();
            CodeNameList.Add("AF", "Afghanistan");
            CodeNameList.Add("AX", "Aland Islands");
            CodeNameList.Add("AL", "Albania");
            CodeNameList.Add("DZ", "Algeria");
            CodeNameList.Add("AS", "American Samoa");
            CodeNameList.Add("AD", "Andorra");
            CodeNameList.Add("AO", "Angola");
            CodeNameList.Add("AI", "Anguilla");
            CodeNameList.Add("AQ", "Antarctica");
            CodeNameList.Add("AG", "Antigua and Barbuda");
            CodeNameList.Add("AR", "Argentina");
            CodeNameList.Add("AM", "Armenia");
            CodeNameList.Add("AW", "Aruba");
            CodeNameList.Add("AU", "Australia");
            CodeNameList.Add("AT", "Austria");
            CodeNameList.Add("AZ", "Azerbaijan");
            CodeNameList.Add("BS", "Bahamas");
            CodeNameList.Add("BH", "Bahrain");
            CodeNameList.Add("BD", "Bangladesh");
            CodeNameList.Add("BB", "Barbados");
            CodeNameList.Add("BY", "Belarus");
            CodeNameList.Add("BE", "Belgium");
            CodeNameList.Add("BZ", "Belize");
            CodeNameList.Add("BJ", "Benin");
            CodeNameList.Add("BM", "Bermuda");
            CodeNameList.Add("BT", "Bhutan");
            CodeNameList.Add("BO", "Bolivia");
            CodeNameList.Add("BA", "Bosnia and Herzegovina");
            CodeNameList.Add("BW", "Botswana");
            CodeNameList.Add("BV", "Bouvet Island");
            CodeNameList.Add("BR", "Brazil");
            CodeNameList.Add("VG", "British Virgin Islands");
            CodeNameList.Add("IO", "British Indian Ocean Territory");
            CodeNameList.Add("BN", "Brunei Darussalam");
            CodeNameList.Add("BG", "Bulgaria");
            CodeNameList.Add("BF", "Burkina Faso");
            CodeNameList.Add("BI", "Burundi");
            CodeNameList.Add("KH", "Cambodia");
            CodeNameList.Add("CM", "Cameroon");
            CodeNameList.Add("CA", "Canada");
            CodeNameList.Add("CV", "Cape Verde");
            CodeNameList.Add("KY", "Cayman Islands");
            CodeNameList.Add("CF", "Central African Republic");
            CodeNameList.Add("TD", "Chad");
            CodeNameList.Add("CL", "Chile");
            CodeNameList.Add("CN", "China");
            CodeNameList.Add("HK", "Hong Kong, SAR China");
            CodeNameList.Add("MO", "Macao, SAR China");
            CodeNameList.Add("CX", "Christmas Island");
            CodeNameList.Add("CC", "Cocos (Keeling) Islands");
            CodeNameList.Add("CO", "Colombia");
            CodeNameList.Add("KM", "Comoros");
            CodeNameList.Add("CG", "Congo\u00a0(Brazzaville)");
            CodeNameList.Add("CD", "Congo, (Kinshasa)");
            CodeNameList.Add("CK", "Cook Islands");
            CodeNameList.Add("CR", "Costa Rica");
            CodeNameList.Add("CI", "Côte d'Ivoire");
            CodeNameList.Add("HR", "Croatia");
            CodeNameList.Add("CU", "Cuba");
            CodeNameList.Add("CY", "Cyprus");
            CodeNameList.Add("CZ", "Czech Republic");
            CodeNameList.Add("DK", "Denmark");
            CodeNameList.Add("DJ", "Djibouti");
            CodeNameList.Add("DM", "Dominica");
            CodeNameList.Add("DO", "Dominican Republic");
            CodeNameList.Add("EC", "Ecuador");
            CodeNameList.Add("EG", "Egypt");
            CodeNameList.Add("SV", "El Salvador");
            CodeNameList.Add("GQ", "Equatorial Guinea");
            CodeNameList.Add("ER", "Eritrea");
            CodeNameList.Add("EE", "Estonia");
            CodeNameList.Add("ET", "Ethiopia");
            CodeNameList.Add("FK", "Falkland Islands (Malvinas)");
            CodeNameList.Add("FO", "Faroe Islands");
            CodeNameList.Add("FJ", "Fiji");
            CodeNameList.Add("FI", "Finland");
            CodeNameList.Add("FR", "France");
            CodeNameList.Add("GF", "French Guiana");
            CodeNameList.Add("PF", "French Polynesia");
            CodeNameList.Add("TF", "French Southern Territories");
            CodeNameList.Add("GA", "Gabon");
            CodeNameList.Add("GM", "Gambia");
            CodeNameList.Add("GE", "Georgia");
            CodeNameList.Add("DE", "Germany");
            CodeNameList.Add("GH", "Ghana");
            CodeNameList.Add("GI", "Gibraltar");
            CodeNameList.Add("GR", "Greece");
            CodeNameList.Add("GL", "Greenland");
            CodeNameList.Add("GD", "Grenada");
            CodeNameList.Add("GP", "Guadeloupe");
            CodeNameList.Add("GU", "Guam");
            CodeNameList.Add("GT", "Guatemala");
            CodeNameList.Add("GG", "Guernsey");
            CodeNameList.Add("GN", "Guinea");
            CodeNameList.Add("GW", "Guinea-Bissau");
            CodeNameList.Add("GY", "Guyana");
            CodeNameList.Add("HT", "Haiti");
            CodeNameList.Add("HM", "Heard and Mcdonald Islands");
            CodeNameList.Add("VA", "Holy See\u00a0(Vatican City State)");
            CodeNameList.Add("HN", "Honduras");
            CodeNameList.Add("HU", "Hungary");
            CodeNameList.Add("IS", "Iceland");
            CodeNameList.Add("IN", "India");
            CodeNameList.Add("ID", "Indonesia");
            CodeNameList.Add("IR", "Iran, Islamic Republic of");
            CodeNameList.Add("IQ", "Iraq");
            CodeNameList.Add("IE", "Ireland");
            CodeNameList.Add("IM", "Isle of Man");
            CodeNameList.Add("IL", "Israel");
            CodeNameList.Add("IT", "Italy");
            CodeNameList.Add("JM", "Jamaica");
            CodeNameList.Add("JP", "Japan");
            CodeNameList.Add("JE", "Jersey");
            CodeNameList.Add("JO", "Jordan");
            CodeNameList.Add("KZ", "Kazakhstan");
            CodeNameList.Add("KE", "Kenya");
            CodeNameList.Add("KI", "Kiribati");
            CodeNameList.Add("KP", "Korea\u00a0(North)");
            CodeNameList.Add("KR", "Korea\u00a0(South)");
            CodeNameList.Add("KW", "Kuwait");
            CodeNameList.Add("KG", "Kyrgyzstan");
            CodeNameList.Add("LA", "Lao PDR");
            CodeNameList.Add("LV", "Latvia");
            CodeNameList.Add("LB", "Lebanon");
            CodeNameList.Add("LS", "Lesotho");
            CodeNameList.Add("LR", "Liberia");
            CodeNameList.Add("LY", "Libya");
            CodeNameList.Add("LI", "Liechtenstein");
            CodeNameList.Add("LT", "Lithuania");
            CodeNameList.Add("LU", "Luxembourg");
            CodeNameList.Add("MK", "Macedonia, Republic of");
            CodeNameList.Add("MG", "Madagascar");
            CodeNameList.Add("MW", "Malawi");
            CodeNameList.Add("MY", "Malaysia");
            CodeNameList.Add("MV", "Maldives");
            CodeNameList.Add("ML", "Mali");
            CodeNameList.Add("MT", "Malta");
            CodeNameList.Add("MH", "Marshall Islands");
            CodeNameList.Add("MQ", "Martinique");
            CodeNameList.Add("MR", "Mauritania");
            CodeNameList.Add("MU", "Mauritius");
            CodeNameList.Add("YT", "Mayotte");
            CodeNameList.Add("MX", "Mexico");
            CodeNameList.Add("FM", "Micronesia, Federated States of");
            CodeNameList.Add("MD", "Moldova");
            CodeNameList.Add("MC", "Monaco");
            CodeNameList.Add("MN", "Mongolia");
            CodeNameList.Add("ME", "Montenegro");
            CodeNameList.Add("MS", "Montserrat");
            CodeNameList.Add("MA", "Morocco");
            CodeNameList.Add("MZ", "Mozambique");
            CodeNameList.Add("MM", "Myanmar");
            CodeNameList.Add("NA", "Namibia");
            CodeNameList.Add("NR", "Nauru");
            CodeNameList.Add("NP", "Nepal");
            CodeNameList.Add("NL", "Netherlands");
            CodeNameList.Add("AN", "Netherlands Antilles");
            CodeNameList.Add("NC", "New Caledonia");
            CodeNameList.Add("NZ", "New Zealand");
            CodeNameList.Add("NI", "Nicaragua");
            CodeNameList.Add("NE", "Niger");
            CodeNameList.Add("NG", "Nigeria");
            CodeNameList.Add("NU", "Niue");
            CodeNameList.Add("NF", "Norfolk Island");
            CodeNameList.Add("MP", "Northern Mariana Islands");
            CodeNameList.Add("NO", "Norway");
            CodeNameList.Add("OM", "Oman");
            CodeNameList.Add("PK", "Pakistan");
            CodeNameList.Add("PW", "Palau");
            CodeNameList.Add("PS", "Palestinian Territory");
            CodeNameList.Add("PA", "Panama");
            CodeNameList.Add("PG", "Papua New Guinea");
            CodeNameList.Add("PY", "Paraguay");
            CodeNameList.Add("PE", "Peru");
            CodeNameList.Add("PH", "Philippines");
            CodeNameList.Add("PN", "Pitcairn");
            CodeNameList.Add("PL", "Poland");
            CodeNameList.Add("PT", "Portugal");
            CodeNameList.Add("PR", "Puerto Rico");
            CodeNameList.Add("QA", "Qatar");
            CodeNameList.Add("RE", "Réunion");
            CodeNameList.Add("RO", "Romania");
            CodeNameList.Add("RU", "Russian Federation");
            CodeNameList.Add("RW", "Rwanda");
            CodeNameList.Add("BL", "Saint-Barthélemy");
            CodeNameList.Add("SH", "Saint Helena");
            CodeNameList.Add("KN", "Saint Kitts and Nevis");
            CodeNameList.Add("LC", "Saint Lucia");
            CodeNameList.Add("MF", "Saint-Martin (French part)");
            CodeNameList.Add("PM", "Saint Pierre and Miquelon");
            CodeNameList.Add("VC", "Saint Vincent and Grenadines");
            CodeNameList.Add("WS", "Samoa");
            CodeNameList.Add("SM", "San Marino");
            CodeNameList.Add("ST", "Sao Tome and Principe");
            CodeNameList.Add("SA", "Saudi Arabia");
            CodeNameList.Add("SN", "Senegal");
            CodeNameList.Add("RS", "Serbia");
            CodeNameList.Add("SC", "Seychelles");
            CodeNameList.Add("SL", "Sierra Leone");
            CodeNameList.Add("SG", "Singapore");
            CodeNameList.Add("SK", "Slovakia");
            CodeNameList.Add("SI", "Slovenia");
            CodeNameList.Add("SB", "Solomon Islands");
            CodeNameList.Add("SO", "Somalia");
            CodeNameList.Add("ZA", "South Africa");
            CodeNameList.Add("GS", "South Georgia and the South Sandwich Islands");
            CodeNameList.Add("SS", "South Sudan");
            CodeNameList.Add("ES", "Spain");
            CodeNameList.Add("LK", "Sri Lanka");
            CodeNameList.Add("SD", "Sudan");
            CodeNameList.Add("SR", "Suriname");
            CodeNameList.Add("SJ", "Svalbard and Jan Mayen Islands");
            CodeNameList.Add("SZ", "Swaziland");
            CodeNameList.Add("SE", "Sweden");
            CodeNameList.Add("CH", "Switzerland");
            CodeNameList.Add("SY", "Syrian Arab Republic\u00a0(Syria)");
            CodeNameList.Add("TW", "Taiwan, Republic of China");
            CodeNameList.Add("TJ", "Tajikistan");
            CodeNameList.Add("TZ", "Tanzania, United Republic of");
            CodeNameList.Add("TH", "Thailand");
            CodeNameList.Add("TL", "Timor-Leste");
            CodeNameList.Add("TG", "Togo");
            CodeNameList.Add("TK", "Tokelau");
            CodeNameList.Add("TO", "Tonga");
            CodeNameList.Add("TT", "Trinidad and Tobago");
            CodeNameList.Add("TN", "Tunisia");
            CodeNameList.Add("TR", "Turkey");
            CodeNameList.Add("TM", "Turkmenistan");
            CodeNameList.Add("TC", "Turks and Caicos Islands");
            CodeNameList.Add("TV", "Tuvalu");
            CodeNameList.Add("UG", "Uganda");
            CodeNameList.Add("UA", "Ukraine");
            CodeNameList.Add("AE", "United Arab Emirates");
            CodeNameList.Add("GB", "United Kingdom");
            CodeNameList.Add("US", "United States of America");
            CodeNameList.Add("UM", "US Minor Outlying Islands");
            CodeNameList.Add("UY", "Uruguay");
            CodeNameList.Add("UZ", "Uzbekistan");
            CodeNameList.Add("VU", "Vanuatu");
            CodeNameList.Add("VE", "Venezuela\u00a0(Bolivarian Republic)");
            CodeNameList.Add("VN", "Viet Nam");
            CodeNameList.Add("VI", "Virgin Islands, US");
            CodeNameList.Add("WF", "Wallis and Futuna Islands");
            CodeNameList.Add("EH", "Western Sahara");
            CodeNameList.Add("YE", "Yemen");
            CodeNameList.Add("ZM", "Zambia");
            CodeNameList.Add("ZW", "Zimbabwe");

        }

        public static Dictionary<string, string> CodeNameList { get; set; }

        public static string FromShort(short shortcode)
        {
            byte[] bytes = BitConverter.GetBytes(shortcode);
            return Encoding.ASCII.GetString(bytes);
        }

        public static short ToShort(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return 0;
            }
            code = code.ToUpper();
            if (code == "ZZ")
            {
                return 0;
            }
            byte[] bytes = Encoding.ASCII.GetBytes(code);
            return BitConverter.ToInt16(bytes, 0);
        }

        public static string GetCountryName(short code)
        {
            var countrycode = FromShort(code);
            return GetCountryName(countrycode);
        }

        public static string GetCountryName(string code)
        {
            if (CodeNameList.ContainsKey(code))
            {
                return CodeNameList[code];
            }
            return null;
        }


    }
}
