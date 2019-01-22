using System;
using LibSass.Compiler.Context;

namespace LibSass.Types
{
    internal interface ISassExportableType
    {
        /// <summary>
        /// Event handler to invalidate internal ptrs.
        /// </summary>
        void OnInvalidated();

        /// <summary>
        /// Instantiate the type on LibSass heap.
        /// </summary>
        /// <param name="validityEventHandler"></param>
        IntPtr GetInternalTypePtr(InternalPtrValidityEventHandler validityEventHandler, bool dontCache = false);
    }
}
