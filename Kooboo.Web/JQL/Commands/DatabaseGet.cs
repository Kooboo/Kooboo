//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Web.JQL.Commands
{
    public class DatabaseGet : ICommand
    {
        public string NameOrId { get; set; }
        public string Table { get; set; }
    }
}
