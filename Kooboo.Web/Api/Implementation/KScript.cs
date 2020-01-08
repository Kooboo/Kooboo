//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Sites.DataSources;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Scripting;
using Kooboo.Web.Frontend.KScriptDefine;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Api.Implementation
{
    public class KScriptApi : IApi
    { 

        private readonly Lazy<string> _defineContent;

        public KScriptApi()
        {
            _defineContent = new Lazy<string>(() => new KScriptToTsDefineConventer().Convent(typeof(KScript.k)), true);
        }

        public string ModelName
        {
            get { return "KScript"; }
        }

        public bool RequireSite
        {
            get { return false; }
        }

        public bool RequireUser
        {
            get { return false; }
        }

        public string GetDefine()
        {
            return _defineContent.Value;
        }
    }
}
