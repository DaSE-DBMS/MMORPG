using System.Xml.Serialization;
using System.IO;
using Backend.Game;
using Backend.AI;
using Common.Data;

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
            server.RegisterClose(RecvConnectionClose);

            // load assets before create game world and receive client connections
            LoadAssets();

            // start the server, block till program exit
            server.Start(ip, port);
        }

        public void Register(Command command, MessageDelegate @delegate)
        {
            server.RegisterMessageRecv(command, @delegate);
        }

        private void RecvConnectionClose(IChannel channel)
        {
            World.Instance().RemoveEntity(((Entity)channel.GetContent()).id);
        }

        private void LoadAssets()
        {
            for (int i = 0; i < 1; i++)
            {// foreach scene assets, TODO
                XmlSerializer serializer = new XmlSerializer(typeof(DSceneAsset));

                StreamReader reader = new StreamReader("E:/Users/ybbh/workspace/MMORPG/Assets/navmesh/Level1.xml");

                DSceneAsset asset = (DSceneAsset)serializer.Deserialize(reader);

                World.Instance().LoadScene(asset);
            }
        }
    }
}
