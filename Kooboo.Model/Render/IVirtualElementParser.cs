using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kooboo.Dom;

namespace Kooboo.Model.Render
{
    /// <summary>
    /// Virtual element who need special render
    /// </summary>
    public interface IVirtualElementParser
    {
        string Name { get; }

        /// <summary>
        /// Attribute priority to decide which one to be parse first in the same tag
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Decide how to build JS
        /// </summary>
        void Parse(Element el, TagParseContext context, Action visitChildren);
    }
}
