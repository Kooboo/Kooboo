namespace LibSass.Compiler.Options
{
    public class SassImport
    {
        /// <summary>
        /// Input path to read contents from.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Data contents as string.
        /// Note if both Data and Path are set,
        /// Data will be used for contents and
        /// Path will be used in generating source-maps.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Source map to make the compiler merge it in the resultant
        /// source map.
        /// This option comes handy if the data you are trans-compiling
        /// is the compiled result of some other source.
        /// </summary>
        public string Map { get; set; }

        /// <summary>
        /// Error message for compilation to fail with.
        /// This message will be returned in ErrorJson
        /// and ErrorMessage of source SassResult object.
        /// </summary>
        public string Error { get; set; }
    }

    /// <summary>
    /// Prototype for the importer method.
    /// The parameters can be used to resolve
    /// paths or resource to download data from.
    /// </summary>
    /// <param name="currentImport">Import value parsed by the compiler.</param>
    /// <param name="parentImport">
    /// Parent import value retained by the compiler.
    /// If a custom importer altered the previous import value,
    /// or if compiler resolved or normalised the path, it will return the altered value.
    /// </param>
    /// <param name="sassOptions">Options object which used for compilation.</param>
    /// <returns>List of SassImport objects.</returns>
    /// <remarks>
    /// To give a precise example for `currentImport`, it would be `Example/path\\file.scss`
    /// in `@import "Example/path\\\\file.scss"`.
    /// (note the LibSass behaviour of '\\\\' transformed into '\\' is retained).
    /// </remarks>
    public delegate SassImport[] CustomImportDelegate(string currentImport, string parentImport, ISassOptions sassOptions);
}
