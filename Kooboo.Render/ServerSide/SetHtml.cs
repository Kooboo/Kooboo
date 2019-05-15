//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Render.ObjectSource;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Render.ServerSide
{
    public class SetHtml : IServerTask
    {
        public List<string> paras { get; set; }
        public string name { get { return "setHtml"; } }

        public string Render(CommandDiskSourceProvider sourceProvider, RenderOption option, RenderContext context, string baseRelativeUrl)
        {
            string result= null; 

            if (paras.Count() >1 )
            {
                // set html require two paras, one for the variable name, one for the relative url. 
                string name = paras[0]; 
                var relativeurl = paras[1];

                string text = null; 

                if (!string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(relativeurl))
                {
                    relativeurl = ServerHelper.EnsureRelative(relativeurl, baseRelativeUrl);
                     
                    var response = Kooboo.Render.RenderEngine.RenderHtml(context, option, relativeurl);
                     
                    if (response != null)
                    {
                        if (response.Body != null)
                        {
                            text =  response.Body;
                        }
                        else if (response.BinaryBytes != null)
                        {
                           text = System.Text.Encoding.UTF8.GetString(response.BinaryBytes); 
                        }
                    }
                }

                if (text !=null)
                {
                    text = text.Replace("\"", "\\\"");
                    //html in github will change \r\n to \n
                    text = System.Text.RegularExpressions.Regex.Replace(text, "(?<!\r)\n|\r\n", "\\\r\n");
                }

                result = "var " + name + "=\"" + text + "\"; "; 
            }
            return result;
        }
    }
}




