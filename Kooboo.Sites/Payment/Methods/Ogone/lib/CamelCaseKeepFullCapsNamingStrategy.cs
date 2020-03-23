using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace Kooboo.Sites.Payment.Methods.Ogone.lib
{
    public class CamelCaseKeepFullCapsNamingStrategy : CamelCaseNamingStrategy
    {
        public CamelCaseKeepFullCapsNamingStrategy()
        {
            OverrideSpecifiedNames = true;
            ProcessDictionaryKeys = true;
        }

        protected override string ResolvePropertyName(string name)
        {
            var baseName = base.ResolvePropertyName(name);
            var isAllCaps = true;
            for (int i = 0; i < name.Length; i++)
            {
                if (!char.IsUpper(name[i]))
                {
                    isAllCaps = false;
                    break;
                }
            }
            return isAllCaps ? baseName.ToUpper() : baseName;
        }
    }
}
