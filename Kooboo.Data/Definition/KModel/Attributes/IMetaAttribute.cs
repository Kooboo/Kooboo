using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Data.Definition.KModel.Attributes
{
    public interface IMetaAttribute
    {
        bool IsHeader { get; }
        string PropertyName { get; }

        string Value();
    }
}
