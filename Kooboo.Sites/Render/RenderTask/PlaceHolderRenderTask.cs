//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
  
namespace Kooboo.Sites.Render
{ 
    public class PlaceHolderRenderTask : IRenderTask
    {
        public PlaceHolderRenderTask(string PositionName)
        {
            this.PlaceHolderName = PositionName;
            this.NamedPosotion = !string.IsNullOrEmpty(this.PlaceHolderName);
        }
        public string PlaceHolderName { get; set; }
        public bool NamedPosotion { get; set; }

        public bool ClearBefore
        {
            get
            {
                return false; 
            }
        }

        public string Render(RenderContext context)
        {
            string result = null;  
            if (NamedPosotion && context.PlaceholderContents.ContainsKey(this.PlaceHolderName))
            {
                result =  context.PlaceholderContents[this.PlaceHolderName];
                context.PlaceholderContents.Remove(this.PlaceHolderName); 
            }

            if (context.PlaceholderContents.ContainsKey(""))
            {
                result =  context.PlaceholderContents[""];
                context.PlaceholderContents.Remove(""); 
            }
            return result; 
        }

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            string value=null; 
            if (NamedPosotion && context.PlaceholderContents.ContainsKey(this.PlaceHolderName))
            {
                value =  context.PlaceholderContents[this.PlaceHolderName];
                context.PlaceholderContents.Remove(this.PlaceHolderName);
            }

            if (context.PlaceholderContents.ContainsKey(""))
            {
                value =  context.PlaceholderContents[""];
                context.PlaceholderContents.Remove("");
            }
            result.Add(new RenderResult() { Value = value }); 

        }
    }
}
