namespace Kooboo.Data.Interface
{
    public interface ILoginBlock
    {
        bool IsIpBlocked(string IP);

        void AddIpFailure(string IP);

        void AddUserNameFailure(string userName);

        bool IsUserNameBlocked(string name);

        void AddLoginOk(string username, string ip);
    }
}