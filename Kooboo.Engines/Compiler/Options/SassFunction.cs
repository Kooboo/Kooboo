using System.Collections.Generic;
using LibSass.Types;

namespace LibSass.Compiler.Options
{
    public class SassFunction
    {
        /// <summary>
        /// Input path to read contents from.
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// Callback delegate.
        /// </summary>
        public CustomFunctionDelegate CustomFunctionDelegate;
    }

    public class SassFunctionCollection : List<SassFunction>
    {
        public CustomFunctionDelegate this[string signature]
        {
            get { return Find(s => s.Signature == signature).CustomFunctionDelegate; }
            set
            {
                Add(new SassFunction
                {
                    Signature = signature,
                    CustomFunctionDelegate = value
                });
            }
        }
    }

    /// <summary>
    /// Prototype for the Sass function.
    /// </summary>
    /// <param name="sassOptions">Options object which used for compilation.</param>
    /// <param name="sassValues">
    /// List of values passed to the function in Sass code
    /// </param>
    /// <returns>List of SassImport objects.</returns>
    public delegate ISassType CustomFunctionDelegate(ISassOptions sassOptions, string signature, params ISassType[] sassValues);
}
