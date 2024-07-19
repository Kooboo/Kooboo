namespace Kooboo.Mail.Events
{
    public enum EnumMailEvent
    {
        OnMailIncoming = 1,
        OnMailReceived = 2,
        OnMailBoxNotFound = 3,
        OnMailSending = 4,
        OnMailSent = 5,
        OnMessageRead = 6,
        OnMessageReply = 7,
        OnMessageCreated = 8,
        OnMessageDeleted = 9,
    }
}