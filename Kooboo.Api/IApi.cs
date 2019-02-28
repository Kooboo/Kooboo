//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Api
{
   public interface IApi
    {  
        string ModelName { get; } 
        bool RequireSite { get; }
        bool RequireUser { get;  }
    }
}
