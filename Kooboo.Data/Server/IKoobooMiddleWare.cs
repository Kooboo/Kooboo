//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using System.Threading.Tasks;

namespace Kooboo.Data.Server
{
  public interface  IKoobooMiddleWare
    { 
         Task Invoke(RenderContext context);   
         IKoobooMiddleWare Next { get; set; }
    }
}
