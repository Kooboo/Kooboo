//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using System;
using System.Collections.Generic;

namespace Kooboo.Data.Language
{
    public class LanguageProvider
    {
        private static object _locker = new object();

        private static Dictionary<string, Dictionary<string, string>> langtext = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        public static string GetValue(string key, string langCode)
        {
            var langvalues = GetLangValues(langCode);

            return langvalues.ContainsKey(key) ? langvalues[key] : key;
        }

        public static string GetValue(string key)
        {
            var langcode = LanguageSetting.SystemLangCode;
            return GetValue(key, langcode);
        }

        public static string GetValue(string key, RenderContext context)
        {
            string langcode;
            if (context?.User != null && !string.IsNullOrWhiteSpace(context.User.Language))
            {
                langcode = context.User.Language;
            }
            else
            {
                langcode = LanguageSetting.SystemLangCode;
            }
            return GetValue(key, langcode);
        }

        public static Dictionary<string, string> GetLangValues(string langCode)
        {
            if (langCode == null)
            {
                langCode = LanguageSetting.SystemLangCode;
            }
            if (langCode.Length > 2)
            {
                langCode = langCode.Substring(0, 2);
            }
            if (!langtext.ContainsKey(langCode))
            {
                lock (_locker)
                {
                    if (!langtext.ContainsKey(langCode))
                    {
                        if (LanguageSetting.LangFiles.ContainsKey(langCode))
                        {
                            var file = LanguageSetting.LangFiles[langCode];

                            if (System.IO.File.Exists(file))
                            {
                                string alltext = System.IO.File.ReadAllText(file);

                                var values = Language.MultiLingualHelper.Deserialize(alltext);

                                langtext[langCode] = EscapeQuote(values);
                            }
                        }
                        else
                        {
                            langtext[langCode] = new Dictionary<string, string>();
                        }
                    }
                }
            }

            return langtext[langCode];
        }

        private static Dictionary<string, string> EscapeQuote(Dictionary<string, string> values)
        {
            var newValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in values)
            {
                //change " to \",because " will cause js error.
                newValues[item.Key] = item.Value.Replace("\"", "\\\"");
            }
            return newValues;
        }

        public static void SetValue(string LangCode, Dictionary<string, string> values)
        {
            values = EscapeQuote(values);

            lock (_locker)
            {
                var langcontent = GetLangValues(LangCode);
                if (langcontent.Count == 0)
                {
                    langtext[LangCode] = values;
                }
                else
                {
                    foreach (var item in values)
                    {
                        langcontent[item.Key] = item.Value;
                    }
                }
            }
        }
    }
}