namespace dotless.Core.Plugins
{
    using System;
    using System.Collections.Generic;

    public interface IFunctionPlugin : IPlugin
    {
        Dictionary<string, Type> GetFunctions();
    }
}
