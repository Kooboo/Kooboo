namespace Kooboo.Sites.SMS.ChinaMobile.Models
{
    public class SMSResult
    {
        protected SMSResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        protected SMSResult(string message) : this(false)
        {
            Message = message;
        }

        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public string Message { get; }

        public static SMSResult Success()
        {
            return new SMSResult(true);
        }

        public static SMSResult Failure(string message)
        {
            return new SMSResult(message);
        }
    }
}
