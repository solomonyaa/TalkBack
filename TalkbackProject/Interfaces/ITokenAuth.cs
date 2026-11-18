namespace Talkback.Server.Interfaces
{
    public interface ITokenAuth
    {
        string Authentication(string username, string password);
    }
}
