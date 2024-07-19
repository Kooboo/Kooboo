//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Web.JsTest
{
    public class JsTestCommand
    {
        public JsCommand Command { get; set; }

        public bool IsJs { get; set; }

        public string JsPath { get; set; }

        public string Folder { get; set; }

        public string File { get; set; }

        public string Function { get; set; }

        public enum JsCommand
        {
            none = 0,
            view = 1,
            run = 2
        }
    }



}
