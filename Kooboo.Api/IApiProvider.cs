//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic; 

namespace Kooboo.Api
{  
    public interface IApiProvider
    { 
         Dictionary<string, IApi> List { get; }
   
        //this seems like only for unit test now. 
        void Set(Type apitype);

        IApi Get(string ModelName); 

        string ApiPrefix { get; set;  }
       
    } 
}
