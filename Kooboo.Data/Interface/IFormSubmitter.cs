//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Interface
{
  public interface IFormSubmitter
    {
        string Name { get;  }

        bool Submit(RenderContext context, Guid FormId,  Dictionary<string, string> settings);
         
        List<Models.SimpleSetting> Settings(RenderContext context);

        string CustomActionUrl(RenderContext context, Dictionary<string, string> settings); 
    }
}
