//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Kooboo.Data.Language
{
    public static class LanguageSetting
    {
        static LanguageSetting()
        { 
            ISOTwoLetterCode = initlangcode();

            string[] sep = new string[3];
            sep[0] = ";";
            sep[1] = ",";
            sep[2] = " ";
            //sep[2] = "-";
            //sep[4] = "=";
            LangSep = sep;

            SystemLangCode = AppSettings.CmsLang.ToLower();
             
            LangFiles = getLangFiles();

            CmsLangs = initcmsLangs();

        }

        private static Dictionary<string, string> initlangcode()
        {
            var _dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _dict.Add("aa", "Afaraf");
            _dict.Add("ab", "аҧсуа бызшәа, аҧсшәа");
            _dict.Add("ae", "avesta");
            _dict.Add("af", "Afrikaans");
            _dict.Add("ak", "Akan");
            _dict.Add("am", "አማርኛ");
            _dict.Add("an", "aragonés");
            _dict.Add("ar", "العربية");
            _dict.Add("as", "অসমীয়া");
            _dict.Add("av", "авар мацӀ, магӀарул мацӀ");
            _dict.Add("ay", "aymar aru");
            _dict.Add("az", "azərbaycan dili");
            _dict.Add("ba", "башҡорт теле");
            _dict.Add("be", "беларуская мова");
            _dict.Add("bg", "български език");
            _dict.Add("bh", "भोजपुरी");
            _dict.Add("bi", "Bislama");
            _dict.Add("bm", "bamanankan");
            _dict.Add("bn", "বাংলা");
            _dict.Add("bo", "བོད་ཡིག");
            _dict.Add("br", "brezhoneg");
            _dict.Add("bs", "bosanski jezik");
            _dict.Add("ca", "català");
            _dict.Add("ce", "нохчийн мотт");
            _dict.Add("ch", "Chamoru");
            _dict.Add("co", "corsu, lingua corsa");
            _dict.Add("cr", "ᓀᐦᐃᔭᐍᐏᐣ");
            _dict.Add("cs", "čeština, český jazyk");
            _dict.Add("cu", "ѩзыкъ словѣньскъ");
            _dict.Add("cv", "чӑваш чӗлхи");
            _dict.Add("cy", "Cymraeg");
            _dict.Add("da", "dansk");
            _dict.Add("de", "Deutsch");
            _dict.Add("dv", "ދިވެހި");
            _dict.Add("dz", "རྫོང་ཁ");
            _dict.Add("ee", "Eʋegbe");
            _dict.Add("el", "ελληνικά");
            _dict.Add("en", "English");
            _dict.Add("eo", "Esperanto");
            _dict.Add("es", "Español");
            _dict.Add("et", "eesti, eesti keel");
            _dict.Add("eu", "euskara, euskera");
            _dict.Add("fa", "فارسی");
            _dict.Add("ff", "Fulfulde, Pulaar, Pular");
            _dict.Add("fi", "suomi, suomen kieli");
            _dict.Add("fj", "vosa Vakaviti");
            _dict.Add("fo", "føroyskt");
            _dict.Add("fr", "français, langue française");
            _dict.Add("fy", "Frysk");
            _dict.Add("ga", "Gaeilge");
            _dict.Add("gd", "Gàidhlig");
            _dict.Add("gl", "Galego");
            _dict.Add("gn", "Avañe'ẽ");
            _dict.Add("gu", "ગુજરાતી");
            _dict.Add("gv", "Gaelg, Gailck");
            _dict.Add("ha", "(Hausa) هَوُسَ");
            _dict.Add("he", "עברית");
            _dict.Add("hi", "हिन्दी, हिंदी");
            _dict.Add("ho", "Hiri Motu");
            _dict.Add("hr", "hrvatski jezik");
            _dict.Add("ht", "Kreyòl ayisyen");
            _dict.Add("hu", "magyar");
            _dict.Add("hy", "Հայերեն");
            _dict.Add("hz", "Otjiherero");
            _dict.Add("ia", "Interlingua");
            _dict.Add("id", "Bahasa Indonesia");
            _dict.Add("ie", "Originally called Occidental; then Interlingue after WWII");
            _dict.Add("ig", "Asụsụ Igbo");
            _dict.Add("ii", "ꆈꌠ꒿ Nuosuhxop");
            _dict.Add("ik", "Iñupiaq, Iñupiatun");
            _dict.Add("io", "Ido");
            _dict.Add("is", "Íslenska");
            _dict.Add("it", "Italiano");
            _dict.Add("iu", "ᐃᓄᒃᑎᑐᑦ");
            _dict.Add("ja", "日本語 (にほんご)");
            _dict.Add("jv", "ꦧꦱꦗꦮ, Basa Jawa");
            _dict.Add("ka", "ქართული");
            _dict.Add("kg", "Kikongo");
            _dict.Add("ki", "Gĩkũyũ");
            _dict.Add("kj", "Kuanyama");
            _dict.Add("kk", "қазақ тілі");
            _dict.Add("kl", "kalaallisut, kalaallit oqaasii");
            _dict.Add("km", "ខ្មែរ, ខេមរភាសា, ភាសាខ្មែរ");
            _dict.Add("kn", "ಕನ್ನಡ");
            _dict.Add("ko", "한국어");
            _dict.Add("kr", "Kanuri");
            _dict.Add("ks", "कश्मीरी, كشميري‎");
            _dict.Add("ku", "Kurdî, كوردی‎");
            _dict.Add("kv", "коми кыв");
            _dict.Add("kw", "Kernewek");
            _dict.Add("ky", "Кыргызча, Кыргыз тили");
            _dict.Add("la", "latine, lingua latina");
            _dict.Add("lb", "Lëtzebuergesch");
            _dict.Add("lg", "Luganda");
            _dict.Add("li", "Limburgs");
            _dict.Add("ln", "Lingála");
            _dict.Add("lo", "ພາສາລາວ");
            _dict.Add("lt", "lietuvių kalba");
            _dict.Add("lu", "Kiluba");
            _dict.Add("lv", "Latviešu Valoda");
            _dict.Add("mg", "fiteny malagasy");
            _dict.Add("mh", "Kajin M̧ajeļ");
            _dict.Add("mi", "te reo Māori");
            _dict.Add("mk", "македонски јазик");
            _dict.Add("ml", "മലയാളം");
            _dict.Add("mn", "Монгол хэл");
            _dict.Add("mr", "मराठी");
            _dict.Add("ms", "Bahasa Melayu, بهاس ملايو‎");
            _dict.Add("mt", "Malti");
            _dict.Add("my", "ဗမာစာ");
            _dict.Add("na", "Dorerin Naoero");
            _dict.Add("nb", "Norsk Bokmål");
            _dict.Add("nd", "isiNdebele");
            _dict.Add("ne", "नेपाली");
            _dict.Add("ng", "Owambo");
            _dict.Add("nl", "Nederlands, Vlaams");
            _dict.Add("nn", "Norsk Nynorsk");
            _dict.Add("no", "Norsk");
            _dict.Add("nr", "isiNdebele");
            _dict.Add("nv", "Diné bizaad");
            _dict.Add("ny", "chiCheŵa, chinyanja");
            _dict.Add("oc", "occitan, lenga d'òc");
            _dict.Add("oj", "ᐊᓂᔑᓈᐯᒧᐎᓐ");
            _dict.Add("om", "Afaan Oromoo");
            _dict.Add("or", "ଓଡ଼ିଆ");
            _dict.Add("os", "ирон æвзаг");
            _dict.Add("pa", "ਪੰਜਾਬੀ");
            _dict.Add("pi", "पाऴि");
            _dict.Add("pl", "Język Polski, Polszczyzna");
            _dict.Add("ps", "پښتو");
            _dict.Add("pt", "Português");
            _dict.Add("qu", "Runa Simi, Kichwa");
            _dict.Add("rm", "Rumantsch Grischun");
            _dict.Add("rn", "Ikirundi");
            _dict.Add("ro", "Română");
            _dict.Add("ru", "Русский");
            _dict.Add("rw", "Ikinyarwanda");
            _dict.Add("sa", "संस्कृतम्");
            _dict.Add("sc", "sardu");
            _dict.Add("sd", "सिन्धी, سنڌي، سندھی‎");
            _dict.Add("se", "Davvisámegiella");
            _dict.Add("sg", "yângâ tî sängö");
            _dict.Add("si", "සිංහල");
            _dict.Add("sk", "Slovenčina, Slovenský Jazyk");
            _dict.Add("sl", "Slovenski Jezik, Slovenščina");
            _dict.Add("sm", "gagana fa'a Samoa");
            _dict.Add("sn", "chiShona");
            _dict.Add("so", "Soomaaliga, af Soomaali");
            _dict.Add("sq", "Shqip");
            _dict.Add("sr", "српски језик");
            _dict.Add("ss", "SiSwati");
            _dict.Add("st", "Sesotho");
            _dict.Add("su", "Basa Sunda");
            _dict.Add("sv", "Svenska");
            _dict.Add("sw", "Kiswahili");
            _dict.Add("ta", "தமிழ்");
            _dict.Add("te", "తెలుగు");
            _dict.Add("tg", "тоҷикӣ, toçikī, تاجیکی‎");
            _dict.Add("th", "ไทย");
            _dict.Add("ti", "ትግርኛ");
            _dict.Add("tk", "Türkmen, Түркмен");
            _dict.Add("tl", "Wikang Tagalog");
            _dict.Add("tn", "Setswana");
            _dict.Add("to", "Faka Tonga");
            _dict.Add("tr", "Türkçe");
            _dict.Add("ts", "Xitsonga");
            _dict.Add("tt", "татар теле, tatar tele");
            _dict.Add("tw", "Twi");
            _dict.Add("ty", "Reo Tahiti");
            _dict.Add("ug", "ئۇيغۇرچە‎, Uyghurche");
            _dict.Add("uk", "Українська");
            _dict.Add("ur", "اردو");
            _dict.Add("uz", "Oʻzbek, Ўзбек, أۇزبېك‎");
            _dict.Add("ve", "Tshivenḓa");
            _dict.Add("vi", "Tiếng Việt");
            _dict.Add("vo", "Volapük");
            _dict.Add("wa", "Walon");
            _dict.Add("wo", "Wollof");
            _dict.Add("xh", "isiXhosa");
            _dict.Add("yi", "ייִדיש");
            _dict.Add("yo", "Yorùbá");
            _dict.Add("za", "Saɯ cueŋƅ, Saw cuengh");
            _dict.Add("zh", "中文(Zhōngwén), 汉语, 漢語");
            _dict.Add("zu", "isiZulu");

            return _dict;

        }

        public static string[] LangSep { get; set; }

        private static Dictionary<string, string> getLangFiles()
        {
            Dictionary<string, string> langfile = new Dictionary<string, string>();

            string basedir = AppSettings.RootPath;

            string langpath = System.IO.Path.Combine(AppSettings.RootPath, "Lang");

            if (System.IO.Directory.Exists(langpath))
            {
                var allfiles = System.IO.Directory.GetFiles(langpath, "*.xml");

                if (allfiles != null && allfiles.Count() > 0)
                {
                    foreach (var item in allfiles)
                    {
                        var info = new System.IO.FileInfo(item);
                        string name = info.Name;
                        string key = name.ToLower().Replace(".xml", "");

                        if (ISOTwoLetterCode.ContainsKey(key))
                        {
                            langfile[key] = item;
                        }
                    }
                }
            }
            else
            {
#if Release
            {
             throw new Exception("lang file not found");  
            }
#endif
            }

            return langfile;

        }

        private static object _locker = new object();

        public static Dictionary<string, string> ISOTwoLetterCode
        {
            get; set;
        }

        private static Dictionary<string, List<string>> _regionCulture { get; set; } = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase); 


        public static List<string> GetRegionLangCodes(string regionCode)
        {
            if (!_regionCulture.ContainsKey(regionCode))
            {
                var result = _GetRegionLangcode(regionCode);
                _regionCulture[regionCode] = result; 
            }

            return _regionCulture[regionCode]; 

        }

        private static List<string> _GetRegionLangcode(string regionCode)
        {
            regionCode = regionCode.ToLower();

            List<string> list = new List<string>();
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (CultureInfo culture in cultures)
            {
                RegionInfo region = new RegionInfo(culture.Name);
                if (region != null)
                {
                    if (region.TwoLetterISORegionName.ToLower() == regionCode)
                    {
                        string name = culture.Name;
                        if (name.Length > 2)
                        {
                            name = name.Substring(0, 2);
                        }

                        if (name.Length == 2)
                        {
                            list.Add(name);
                        }
                    }
                }

            }
            return list;


        }


        public static Dictionary<string, string> LangFiles
        {
            get; set;
        }

        public static Dictionary<string, string> CmsLangs
        {
            get; set;
        }

        private static Dictionary<string, string> initcmsLangs()
        {
            var _cmslang = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (LangFiles.Count() > 0)
            {
                foreach (var item in LangFiles)
                {
                    string langkey = item.Key;

                    if (ISOTwoLetterCode.ContainsKey(langkey))
                    {
                        var value = ISOTwoLetterCode[langkey];
                        _cmslang.Add(langkey, value);
                    }
                }
            }

            if (!_cmslang.ContainsKey("en"))
            {
                _cmslang.Add("en", "English");
            }
            return _cmslang;
        }

        public static string SystemLangCode
        {
            get; set;
        }

        public static string GetCmsLangCode(RenderContext context)
        {
            if (context.User != null && !string.IsNullOrEmpty(context.User.Language))
            {
                string lang = context.User.Language;
                if (CmsLangs.ContainsKey(lang))
                {
                    return lang;
                }
            }

            // based on accept langs.. 
            var acceptlangs = context.Request.Headers.Get("Accept-Language");

            if (!string.IsNullOrWhiteSpace(acceptlangs))
            {
                return GetByAcceptLangHeader(acceptlangs);
            }     
              
            return SystemLangCode;
        }

        public static string GetCmsSiteLangCode(RenderContext context, WebSite website)
        {
            if (context.User != null && !string.IsNullOrEmpty(context.User.Language))
            {
                string lang = context.User.Language;
                if (website.Culture.ContainsKey(lang))
                {
                    return lang;
                }
            }
            return website.DefaultCulture; 
        }

        public static string GetByAcceptLangHeader(string AcceptLangHeader)
        {
            if (string.IsNullOrEmpty(AcceptLangHeader))
            {
                return SystemLangCode;
            }

            string[] parts = AcceptLangHeader.Split(LangSep, StringSplitOptions.RemoveEmptyEntries);

            bool HasEn = false;

            foreach (var item in parts)
            {
                if (!string.IsNullOrEmpty(item) && item.Length >= 2)
                {
                    string lang = item.ToLower().Trim();
                    if (lang.Length > 2)
                    {
                        lang = lang.Substring(0, 2);
                    }

                    if (lang.Length != 2)
                    {
                        continue;
                    }

                    if (lang == "en")
                    {
                        HasEn = true;
                        continue;
                    }

                    if (CmsLangs.ContainsKey(lang))
                    {
                        return lang;
                    }
                }
            }

            if (HasEn)
            {
                return "en";
            }

            return SystemLangCode;
        }
    }
}
