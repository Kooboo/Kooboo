//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nager.PublicSuffix
{
    public class WebTldRuleProvider : ITldRuleProvider
    {
        private readonly string _fileUrl;

        public WebTldRuleProvider(string url = "https://publicsuffix.org/list/public_suffix_list.dat")
        {
            this._fileUrl = url;
        }

        public async Task<IEnumerable<TldRule>> BuildAsync()
        {
            var ruleParser = new TldRuleParser();

            var ruleData = await this.LoadFromUrl(this._fileUrl);
            var rules = ruleParser.ParseRules(ruleData);

            return rules;
        }

        public async Task<string> LoadFromUrl(string url)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url))
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
    }
}
