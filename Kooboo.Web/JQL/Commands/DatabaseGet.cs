//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.JQL.Commands
{
    public class DatabaseGet : ICommand
    {
        public string NameOrId { get; set; }
        public string Table { get; set; }
    }
}
