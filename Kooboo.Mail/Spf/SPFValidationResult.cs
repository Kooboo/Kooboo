namespace Kooboo.Mail.Spf
{
    /// <summary>
    /// result of SPF or SenderID validation
    /// </summary>
    public class SPFValidationResult
    {
        public ResultsOfEvaluation Result { get; internal set; }

        public string? Explanation { get; internal set; }
    }
}

