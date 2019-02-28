namespace LibSass.Compiler.Options
{
    public interface ISassOptions
    {
        /// <summary>
        /// Data to be compiled. This can be null, if InputPath is set.
        /// </summary>
        string Data { get; set; }

        /// <summary>
        /// Precision for fractional numbers.
        /// </summary>
        int? Precision { get; set; }

        /// <summary>
        /// Output style for the generated CSS code
        /// A value from above SASS_STYLE_* constants.
        /// </summary>
        SassOutputStyle OutputStyle { get; set; }

        /// <summary>
        /// Emit comments in the generated CSS indicating
        /// the corresponding source line.
        /// </summary>
        bool IncludeSourceComments { get; set; }

        /// <summary>
        /// Embed sourceMappingUrl as data URI.
        /// </summary>
        bool EmbedSourceMap { get; set; }

        /// <summary>
        /// Embed include contents in maps.
        /// </summary>
        bool IncludeSourceMapContents { get; set; }

        /// <summary>
        /// Disable sourceMappingUrl in CSS output.
        /// </summary>
        bool OmitSourceMapUrl { get; set; }

        /// <summary>
        /// Treat source_string as sass (as opposed to scss).
        /// </summary>
        bool IsIndentedSyntax { get; set; }

        /// <summary>
        /// The input path is used for source map
        /// generation. It can be used to define
        /// something with string compilation or to
        /// overload the input file path. It is
        /// set to "stdin" for data contexts and
        /// to the input file on file contexts.
        /// </summary>
        string InputPath { get; set; }

        /// <summary>
        /// The output path is used for source map
        /// generation. Libsass will not write to
        /// this file, it is just used to create
        /// information in source-maps etc.
        /// </summary>
        string OutputPath { get; set; }

        /// <summary>
        /// String to be used for indentation.
        /// </summary>
        string Indent { get; set; }

        /// <summary>
        /// String to be used to for line feeds.
        /// </summary>
        string Linefeed { get; set; }

        /// <summary>
        /// Include paths string for disjoint import directories.
        /// When @import path is not found, LibSass will look into
        /// these directories. The directory paths can be specified
        /// as absolute or relative (with respect to process working
        /// directory).
        ///
        /// Colon-separated on Unix.
        /// Semicolon-separated on Windows.
        /// </summary>
        string IncludePath { get; set; }

        /// <summary>
        /// Plugin path string for the location of pluings.
        /// Paths can be absolute or
        /// relative (with respect to process working directory).
        ///
        /// Colon-separated list of paths
        /// Semicolon-separated on Windows
        /// </summary>
        string PluginPath { get; set; }

        /// <summary>
        /// Include paths list for disjoint import directories.
        /// List of lookup paths for disjoint import directories.
        /// When @import path is not found, LibSass will look into
        /// these directories. The directory paths can be specified
        /// as absolute or relative (with respect to process working
        /// directory).
        /// </summary>
        /// <remarks>
        /// This option is same as <see cref="IncludePath"/> (singular)
        /// which requires OS-dependent path separator.
        /// If you prefer LibSass to handle varied path separator, then this
        /// option is for you.
        /// Note: this incurs a little extra-cost as the array is allocated
        /// on both managed and native runtimes.
        /// </remarks>
        string[] IncludePaths { get; set; }

        /// <summary>
        /// Plugin path list for the location of pluings.
        /// </summary>
        /// <remarks>
        /// This option is same as <see cref="PluginPath"/> (singular)
        /// which requires OS-dependent path separator.
        /// If you prefer LibSass to handle varied path separator, then this
        /// option is for you.
        /// Note: this incurs a little extra-cost as the array is allocated
        /// on both managed and native runtimes.
        /// </remarks>
        string[] PluginPaths { get; set; }

        /// <summary>
        /// Path to source map file
        /// Enables source map generation
        /// Used to create sourceMappingUrl.
        /// </summary>
        string SourceMapFile { get; set; }

        /// <summary>
        /// Directly inserted in source maps.
        /// </summary>
        string SourceMapRoot { get; set; }

        /// <summary>
        /// Custom functions that can be called from Sass code.
        /// </summary>
        SassFunctionCollection Functions { get; set; }

        /// <summary>
        /// List of custom importers.
        /// </summary>
        CustomImportDelegate[] Importers { get; set; }

        /// <summary>
        /// List of custom headers (Experimental).
        /// Opposite to custom importers, all custom headers will be
        /// executed in priority order and all imports will be accumulated
        /// (so many custom headers can add various custom mixins or css-code).
        /// </summary>
        CustomImportDelegate[] Headers { get; set; }
    }
}
