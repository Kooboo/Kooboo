//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Interface;

namespace Kooboo.Data.Models
{
    public class DashBoardResponseHtml : IDashBoardResponse
    {
        public string Body { get; set; }

        public string Link { get; set; }
    }
}