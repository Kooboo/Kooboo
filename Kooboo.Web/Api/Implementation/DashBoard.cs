//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Api.Implementation
{
    public class DashBoardApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "dashboard"; 
            }
        }

        public bool RequireSite
        {
            get
            {
                return true; 
            }
        }

        public bool RequireUser
        {
            get
            {
                return true; 
            }
        }

        public List<string> All(ApiCall call)
        {
            return Kooboo.Web.DashBoard.DashBoardManager.Render(call.Context); 
        } 
        public List<Kooboo.Web.ViewModel.DashBoardItemHtml> Items(ApiCall call)
        {
            return Kooboo.Web.DashBoard.DashBoardManager.ItemHtml(call.Context); 
        } 
    }
}
