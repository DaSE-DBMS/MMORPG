
namespace Gamekit3D.Network
{
    public abstract class IClient
    {
        abstract public void Connect(string ip, short port);

        //abstract public void AsyncConnect(string ip, short port, ChannelDelegate @delegate);

        abstract public void RegisterMessageRecv(Command cmd, MessageDelegate @delegate);
    }
}
