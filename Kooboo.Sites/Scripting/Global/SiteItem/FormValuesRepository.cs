using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Data.Interface;

namespace KScript.Sites
{
    public class FormValuesRepository : RepositoryBase
    {
        public FormValuesRepository(IRepository repo, RenderContext context) : base(repo, context)
        {

        }
    }
}
