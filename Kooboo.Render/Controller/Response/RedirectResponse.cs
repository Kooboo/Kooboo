//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Render.Response
{
    public class RedirectResponse : ResponseBase
    {
        public string RedirectUrl { get; set; }

        public void Redirect(string url, int statuscode = 302)
        {
            this.RedirectUrl = url;
            this.StatusCode = statuscode;
        }

    }

}
