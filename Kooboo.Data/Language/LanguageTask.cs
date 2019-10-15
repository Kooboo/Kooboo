//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Data.Language
{
    public class LanguageTask
    {
        public string Key { get; set; }

        public string Content { get; set; }

        public LanguageTask(string value, bool iskey)
        {
            if (iskey)
            {
                this.Key = value;
            }
            else
            {
                Content = value;
            }
        }

        public string Render(string langCode)
        {
            return Content ?? LanguageProvider.GetValue(this.Key, langCode);
        }
    }
}