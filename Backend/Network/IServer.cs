
namespace Backend.Network
{
    public abstract class IServer
    {
        abstract public void Start(string ip, short port);

        abstract public void RegisterConnect(ChannelDelegate @delegate);

        abstract public void RegisterClose(ChannelDelegate @delegate);

        abstract public void RegisterMessageRecv(Command cmd, MessageDelegate @delegate);
    }
}


