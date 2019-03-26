using Abp.Runtime.Session;

namespace JK.Runtime.Session
{
    public interface IJKSession : IAbpSession
    {
        string UserName { get;}
    }
}