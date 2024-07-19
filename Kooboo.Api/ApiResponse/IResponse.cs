//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;


namespace Kooboo.Api.ApiResponse
{
    public interface IResponse
    {
        object Model { get; set; }

        bool DataChange { get; set; }

        bool Success { get; set; }

        List<string> Messages { get; set; }

        List<FieldError> FieldErrors { get; set; }
    }
}
