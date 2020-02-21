//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Converter
{
  public  interface IConverter
    {
        string Type { get;  }
        /// <summary>
        /// Convert to kooboo components, 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ConvertResult">the json object...</param>
        /// <returns></returns>
        ConvertResponse Convert(RenderContext context, JObject ConvertResult); 
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
