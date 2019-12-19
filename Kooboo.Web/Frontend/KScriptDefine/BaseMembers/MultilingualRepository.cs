using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Frontend.KScriptDefine.BaseMembers
{
    public interface MultilingualRepository
    {
        void Add(MultilingualObject obj);
        MultilingualObject[] All(MultilingualObject obj);
        void Delete(object nameOrId);
        MultilingualObject Get(object nameOrId);
        void Update(MultilingualObject value);
    }
}
