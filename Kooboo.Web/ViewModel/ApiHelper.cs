//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Web.ViewModel
{
    public class ApiViewModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

    }

    public class ApiMethodViewModel
    {
        public ApiMethodViewModel()
        {
            this.Parameters = new List<string>();
            this.Name = string.Empty;
            this.UpdateModel = string.Empty;
            this.ResponseModel = string.Empty;
        }
        public string Name { get; set; }

        public List<string> Parameters { get; set; }

        public bool RequireModel { get; set; }

        public string UpdateModel { get; set; }

        public string ResponseModel { get; set; }

    }
}
