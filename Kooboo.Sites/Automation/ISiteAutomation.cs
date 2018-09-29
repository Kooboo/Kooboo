//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Automation
{
    public interface ISiteAutomation 
    {
     
        object Analyze(WebSite site);

        void Confirm(Dictionary<string, string> FormValues);

        void Apply(Dictionary<string, string> FormVales);
    }
}
