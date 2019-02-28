using System;
using System.IO;

namespace LibSass.Compiler
{
    public class SassResult
    {
        public string Output { get; internal set; }
        public int ErrorColumn { get; internal set; }
        public string ErrorFile { get; internal set; }
        public string ErrorJson { get; internal set; }
        public int ErrorLine { get; internal set; }
        public string ErrorMessage { get; internal set; }
        public string ErrorSource { get; internal set; }
        public int ErrorStatus { get; internal set; }
        public string ErrorText { get; internal set; }
        public string SourceMap { get; internal set; }
        public string[] IncludedFiles { get; internal set; }
        // TODO: more stuff; stats, environment (as in actual Ruby Sass like mutable environment)?

        public override string ToString()
        {
            var linefeed = Environment.NewLine;
            var includeFiles = string.Join(Path.PathSeparator.ToString(), IncludedFiles);

            return string.Join($"{linefeed}{linefeed}",
                $"{nameof(Output)}: {Output}",
                $"{nameof(SourceMap)}: {SourceMap}",
                $"{nameof(IncludedFiles)}: {includeFiles}",
                $"{nameof(ErrorMessage)}: {ErrorMessage}",
                $"{nameof(ErrorLine)}: {ErrorLine}",
                $"{nameof(ErrorColumn)}: {ErrorColumn}",
                $"{nameof(ErrorFile)}: {ErrorFile}",
                $"{nameof(ErrorJson)}: {ErrorJson}",
                $"{nameof(ErrorSource)}: {ErrorSource}",
                $"{nameof(ErrorStatus)}: {ErrorStatus}",
                $"{nameof(ErrorText)}: {ErrorText}");
        }
    }
}
