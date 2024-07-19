namespace Kooboo.Mail.SecurityControl
{
    public class SecurityCheckResult
    {

        public bool CanConnect { get; set; }

        public string Error { get; set; } = "unknown error";
    }
}
