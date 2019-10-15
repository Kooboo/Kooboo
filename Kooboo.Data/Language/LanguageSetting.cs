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
            ISOTwoLetterCode = Initlangcode();

            string[] sep = new string[3];
            sep[0] = ";";
            sep[1] = ",";
            sep[2] = " ";
            //sep[2] = "-";
            //sep[4] = "=";
            LangSep = sep;

            SystemLangCode = string.IsNullOrEmpty(AppSettings.CmsLang) ? "en" : AppSettings.CmsLang.ToLower();

            LangFiles = GetLangFiles();

            CmsLangs = InitcmsLangs();
        }

        private static Dictionary<string, string> Initlangcode()
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"aa", "Afaraf"},
                {"ab", "аҧсуа бызшәа, аҧсшәа"},
                {"ae", "avesta"},
                {"af", "Afrikaans"},
                {"ak", "Akan"},
                {"am", "አማርኛ"},
                {"an", "aragonés"},
                {"ar", "العربية"},
                {"as", "অসমীয়া"},
                {"av", "авар мацӀ, магӀарул мацӀ"},
                {"ay", "aymar aru"},
                {"az", "azərbaycan dili"},
                {"ba", "башҡорт теле"},
                {"be", "беларуская мова"},
                {"bg", "български език"},
                {"bh", "भोजपुरी"},
                {"bi", "Bislama"},
                {"bm", "bamanankan"},
                {"bn", "বাংলা"},
                {"bo", "བོད་ཡིག"},
                {"br", "brezhoneg"},
                {"bs", "bosanski jezik"},
                {"ca", "català"},
                {"ce", "нохчийн мотт"},
                {"ch", "Chamoru"},
                {"co", "corsu, lingua corsa"},
                {"cr", "ᓀᐦᐃᔭᐍᐏᐣ"},
                {"cs", "čeština, český jazyk"},
                {"cu", "ѩзыкъ словѣньскъ"},
                {"cv", "чӑваш чӗлхи"},
                {"cy", "Cymraeg"},
                {"da", "dansk"},
                {"de", "Deutsch"},
                {"dv", "ދިވެހި"},
                {"dz", "རྫོང་ཁ"},
                {"ee", "Eʋegbe"},
                {"el", "ελληνικά"},
                {"en", "English"},
                {"eo", "Esperanto"},
                {"es", "Español"},
                {"et", "eesti, eesti keel"},
                {"eu", "euskara, euskera"},
                {"fa", "فارسی"},
                {"ff", "Fulfulde, Pulaar, Pular"},
                {"fi", "suomi, suomen kieli"},
                {"fj", "vosa Vakaviti"},
                {"fo", "føroyskt"},
                {"fr", "français, langue française"},
                {"fy", "Frysk"},
                {"ga", "Gaeilge"},
                {"gd", "Gàidhlig"},
                {"gl", "Galego"},
                {"gn", "Avañe'ẽ"},
                {"gu", "ગુજરાતી"},
                {"gv", "Gaelg, Gailck"},
                {"ha", "(Hausa) هَوُسَ"},
                {"he", "עברית"},
                {"hi", "हिन्दी, हिंदी"},
                {"ho", "Hiri Motu"},
                {"hr", "hrvatski jezik"},
                {"ht", "Kreyòl ayisyen"},
                {"hu", "magyar"},
                {"hy", "Հայերեն"},
                {"hz", "Otjiherero"},
                {"ia", "Interlingua"},
                {"id", "Bahasa Indonesia"},
                {"ie", "Originally called Occidental; then Interlingue after WWII"},
                {"ig", "Asụsụ Igbo"},
                {"ii", "ꆈꌠ꒿ Nuosuhxop"},
                {"ik", "Iñupiaq, Iñupiatun"},
                {"io", "Ido"},
                {"is", "Íslenska"},
                {"it", "Italiano"},
                {"iu", "ᐃᓄᒃᑎᑐᑦ"},
                {"ja", "日本語 (にほんご)"},
                {"jv", "ꦧꦱꦗꦮ, Basa Jawa"},
                {"ka", "ქართული"},
                {"kg", "Kikongo"},
                {"ki", "Gĩkũyũ"},
                {"kj", "Kuanyama"},
                {"kk", "қазақ тілі"},
                {"kl", "kalaallisut, kalaallit oqaasii"},
                {"km", "ខ្មែរ, ខេមរភាសា, ភាសាខ្មែរ"},
                {"kn", "ಕನ್ನಡ"},
                {"ko", "한국어"},
                {"kr", "Kanuri"},
                {"ks", "कश्मीरी, كشميري‎"},
                {"ku", "Kurdî, كوردی‎"},
                {"kv", "коми кыв"},
                {"kw", "Kernewek"},
                {"ky", "Кыргызча, Кыргыз тили"},
                {"la", "latine, lingua latina"},
                {"lb", "Lëtzebuergesch"},
                {"lg", "Luganda"},
                {"li", "Limburgs"},
                {"ln", "Lingála"},
                {"lo", "ພາສາລາວ"},
                {"lt", "lietuvių kalba"},
                {"lu", "Kiluba"},
                {"lv", "Latviešu Valoda"},
                {"mg", "fiteny malagasy"},
                {"mh", "Kajin M̧ajeļ"},
                {"mi", "te reo Māori"},
                {"mk", "македонски јазик"},
                {"ml", "മലയാളം"},
                {"mn", "Монгол хэл"},
                {"mr", "मराठी"},
                {"ms", "Bahasa Melayu, بهاس ملايو‎"},
                {"mt", "Malti"},
                {"my", "ဗမာစာ"},
                {"na", "Dorerin Naoero"},
                {"nb", "Norsk Bokmål"},
                {"nd", "isiNdebele"},
                {"ne", "नेपाली"},
                {"ng", "Owambo"},
                {"nl", "Nederlands, Vlaams"},
                {"nn", "Norsk Nynorsk"},
                {"no", "Norsk"},
                {"nr", "isiNdebele"},
                {"nv", "Diné bizaad"},
                {"ny", "chiCheŵa, chinyanja"},
                {"oc", "occitan, lenga d'òc"},
                {"oj", "ᐊᓂᔑᓈᐯᒧᐎᓐ"},
                {"om", "Afaan Oromoo"},
                {"or", "ଓଡ଼ିଆ"},
                {"os", "ирон æвзаг"},
                {"pa", "ਪੰਜਾਬੀ"},
                {"pi", "पाऴि"},
                {"pl", "Język Polski, Polszczyzna"},
                {"ps", "پښتو"},
                {"pt", "Português"},
                {"qu", "Runa Simi, Kichwa"},
                {"rm", "Rumantsch Grischun"},
                {"rn", "Ikirundi"},
                {"ro", "Română"},
                {"ru", "Русский"},
                {"rw", "Ikinyarwanda"},
                {"sa", "संस्कृतम्"},
                {"sc", "sardu"},
                {"sd", "सिन्धी, سنڌي، سندھی‎"},
                {"se", "Davvisámegiella"},
                {"sg", "yângâ tî sängö"},
                {"si", "සිංහල"},
                {"sk", "Slovenčina, Slovenský Jazyk"},
                {"sl", "Slovenski Jezik, Slovenščina"},
                {"sm", "gagana fa'a Samoa"},
                {"sn", "chiShona"},
                {"so", "Soomaaliga, af Soomaali"},
                {"sq", "Shqip"},
                {"sr", "српски језик"},
                {"ss", "SiSwati"},
                {"st", "Sesotho"},
                {"su", "Basa Sunda"},
                {"sv", "Svenska"},
                {"sw", "Kiswahili"},
                {"ta", "தமிழ்"},
                {"te", "తెలుగు"},
                {"tg", "тоҷикӣ, toçikī, تاجیکی‎"},
                {"th", "ไทย"},
                {"ti", "ትግርኛ"},
                {"tk", "Türkmen, Түркмен"},
                {"tl", "Wikang Tagalog"},
                {"tn", "Setswana"},
                {"to", "Faka Tonga"},
                {"tr", "Türkçe"},
                {"ts", "Xitsonga"},
                {"tt", "татар теле, tatar tele"},
                {"tw", "Twi"},
                {"ty", "Reo Tahiti"},
                {"ug", "ئۇيغۇرچە‎, Uyghurche"},
                {"uk", "Українська"},
                {"ur", "اردو"},
                {"uz", "Oʻzbek, Ўзбек, أۇزبېك‎"},
                {"ve", "Tshivenḓa"},
                {"vi", "Tiếng Việt"},
                {"vo", "Volapük"},
                {"wa", "Walon"},
                {"wo", "Wollof"},
                {"xh", "isiXhosa"},
                {"yi", "ייִדיש"},
                {"yo", "Yorùbá"},
                {"za", "Saɯ cueŋƅ, Saw cuengh"},
                {"zh", "中文(Zhōngwén), 汉语, 漢語"},
                {"zu", "isiZulu"}
            };

            return dict;
        }

        public static string[] LangSep { get; set; }

        private static Dictionary<string, string> GetLangFiles()
        {
            Dictionary<string, string> langfile = new Dictionary<string, string>();

            string basedir = AppSettings.RootPath;

            string langpath = System.IO.Path.Combine(AppSettings.RootPath, "Lang");

            if (System.IO.Directory.Exists(langpath))
            {
                var allfiles = System.IO.Directory.GetFiles(langpath, "*.xml");

                if (allfiles != null && allfiles.Length > 0)
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

        private static Dictionary<string, List<string>> RegionCulture { get; set; } = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        public static List<string> GetRegionLangCodes(string regionCode)
        {
            if (!RegionCulture.ContainsKey(regionCode))
            {
                var result = _GetRegionLangcode(regionCode);
                RegionCulture[regionCode] = result;
            }

            return RegionCulture[regionCode];
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

        private static Dictionary<string, string> InitcmsLangs()
        {
            var cmslang = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (LangFiles.Count() > 0)
            {
                foreach (var item in LangFiles)
                {
                    string langkey = item.Key;

                    if (ISOTwoLetterCode.ContainsKey(langkey))
                    {
                        var value = ISOTwoLetterCode[langkey];
                        cmslang.Add(langkey, value);
                    }
                }
            }

            if (!cmslang.ContainsKey("en"))
            {
                cmslang.Add("en", "English");
            }
            return cmslang;
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

            return !string.IsNullOrWhiteSpace(acceptlangs) ? GetByAcceptLangHeader(acceptlangs) : SystemLangCode;
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

        public static string GetByAcceptLangHeader(string acceptLangHeader)
        {
            if (string.IsNullOrEmpty(acceptLangHeader))
            {
                return SystemLangCode;
            }

            string[] parts = acceptLangHeader.Split(LangSep, StringSplitOptions.RemoveEmptyEntries);

            bool hasEn = false;

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
                        hasEn = true;
                        continue;
                    }

                    if (CmsLangs.ContainsKey(lang))
                    {
                        return lang;
                    }
                }
            }

            return hasEn ? "en" : SystemLangCode;
        }
    }
}