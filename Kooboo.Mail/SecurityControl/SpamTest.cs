namespace Kooboo.Mail.SecurityControl
{

    public class SpamTest : ISpamTest
    {
        public static ISpamTest instance { get; set; } = new SpamTest();

        public bool IsSpam(string MessageSource)
        {
            return false;
        }
    }

    public interface ISpamTest
    {
        bool IsSpam(string MessageSource);
    }

}
