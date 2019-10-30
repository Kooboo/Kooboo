//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Newtonsoft.Json.Linq;

namespace Kooboo.Sites.InlineEditor.Converter
{
    public interface IConverter
    {
        string Type { get; }

        /// <summary>
        /// Convert to kooboo components,
        /// </summary>
        /// <param name="context"></param>
        /// <param name="convertResult">the json object...</param>
        /// <returns></returns>
        ConvertResponse Convert(RenderContext context, JObject convertResult);
    }

    public class ConvertResponse
    {
        public string Tag { get; set; }

        public bool IsSuccess { get; set; } = true;

        public string ComponentNameOrId { get; set; }

        //any other things...
        public string Others { get; set; }

        public string KoobooId { get; set; }
    }
}