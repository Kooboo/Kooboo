//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Helper;
using Kooboo.Lib.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VirtualFile;

namespace Kooboo.Data.Language
{
    public class LanguageProvider
    {
        private static object _locker = new object();

        private static Dictionary<string, Dictionary<string, string>> langtext = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
        public static string GetValue(string Key, string LangCode)
        {
            var langvalues = GetLangValues(LangCode);

            if (langvalues.ContainsKey(Key))
            {
                return langvalues[Key];
            }
            return Key;
        }

        public static string GetValue(string key)
        {
            var langcode = LanguageSetting.SystemLangCode;
            return GetValue(key, langcode);
        }

        public static string GetValue(string key, RenderContext context)
        {
            string langcode = null;
            if (context != null && context.User != null && !string.IsNullOrWhiteSpace(context.User.Language))
            {
                langcode = context.User.Language;
            }
            else
            {
                langcode = LanguageSetting.SystemLangCode;
            }
            return GetValue(key, langcode);
        }

        public static Dictionary<string, string> GetLangValues(string LangCode)
        {
            if (LangCode == null)
            {
                LangCode = LanguageSetting.SystemLangCode;
            }
            if (LangCode.Length > 2)
            {
                LangCode = LangCode.Substring(0, 2);
            }
            if (!langtext.ContainsKey(LangCode))
            {
                lock (_locker)
                {
                    if (!langtext.ContainsKey(LangCode))
                    {
                        if (LanguageSetting.LangFiles.ContainsKey(LangCode))
                        {
                            var file = LanguageSetting.LangFiles[LangCode];

                            if (System.IO.File.Exists(file))
                            {
                                string alltext = System.IO.File.ReadAllText(file);

                                var values = Language.MultiLingualHelper.Deserialize(alltext);
                                LoadModuleLangPack(values, LangCode);

                                langtext[LangCode] = EscapeQuote(values);
                            }

                        }
                        else
                        {
                            langtext[LangCode] = new Dictionary<string, string>();
                        }
                    }
                }
            }

            return langtext[LangCode];
        }

        private static void LoadModuleLangPack(Dictionary<string, string> values, string langCode)
        {
            foreach (var item in VirtualResources.GetFiles(AppSettings.ModulePath, "*config.json", SearchOption.AllDirectories))
            {
                try
                {
                    var json = VirtualResources.ReadAllText(item);
                    var dic = JsonHelper.DeserializeJObject(json)?["langs"]?[langCode]?.ToObject<Dictionary<string, string>>();
                    if (dic == null) continue;
                    foreach (var kv in dic)
                    {
                        if (!values.ContainsKey(kv.Key) && !string.IsNullOrEmpty(kv.Value)) values[kv.Key] = kv.Value;
                    }
                }
                catch (Exception)
                {
                }
            }
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

        public static void SetValue(string LangCode, Dictionary<string, string> Values)
        {
            Values = EscapeQuote(Values);

            lock (_locker)
            {
                var langcontent = GetLangValues(LangCode);
                if (langcontent.Count() == 0)
                {
                    langtext[LangCode] = Values;
                }
                else
                {
                    foreach (var item in Values)
                    {
                        langcontent[item.Key] = item.Value;
                    }
                }

            }
        }


    }
}
