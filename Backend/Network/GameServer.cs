using System;
using Backend.Game;
using Backend.AI;
namespace Backend.Network
{
    public class GameServer : IRegister
    {
        private Server server;
        private Incomming incomming;
        private string ip;
        private short port;

        public GameServer(string ip, short port)
        {
            this.ip = ip;
            this.port = port;
        }

        public void StartUp()
        {
            server = new Server();
            incomming = new Incomming(this);
            // register message callback sink
            PathFinding.Instance().LoadNavMesh();
            server.RegisterClose(RecvConnectionClose);
            World.Instance.Create();
            // start the server, block till program exit
            server.Start(ip, port);
        }

        public void Register(Command command, MessageDelegate @delegate)
        {
            server.RegisterMessageRecv(command, @delegate);
        }

        private void RecvConnectionClose(IChannel channel)
        {
            World.Instance.RemoveEntity(((Entity)channel.GetContent()).id);
        }
    }
}
