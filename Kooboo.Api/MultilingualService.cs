//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Api.ApiResponse;
using Kooboo.Data.Context;
using Kooboo.Data.Language;

namespace Kooboo.Api
{
    public static class MultilingualService
    {

        public static void EnsureLangText(IResponse response, RenderContext context)
        {
            if (response.Success || response.Messages == null || !response.Messages.Any())
            {
                return;
            }

            string lang = null;
            if (context != null && context.User != null && !string.IsNullOrWhiteSpace(context.User.Language))
            {
                lang = context.User.Language;
            }
            else
            {
                lang = LanguageSetting.SystemLangCode;
            }

            if (lang.ToLower() != "en")
            {
                for (int i = 0; i < response.Messages.Count; i++)
                {
                    var msg = response.Messages[i];
                    if (!string.IsNullOrWhiteSpace(msg))
                    {
                        var value = Kooboo.Data.Language.LanguageProvider.GetValue(msg, context);
                        if (msg != value)
                        {
                            response.Messages[i] = value;
                        }
                    }
                }
            }
        }




    }
}
