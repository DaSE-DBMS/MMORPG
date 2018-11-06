using System.Xml.Serialization;
using System.IO;
using System.Net.Sockets;
using Backend.Game;
using Common;

namespace Backend.Network
{
    public class GameServer : IRegister
    {
        private Server server;
        private Incoming incomming;
        private string ip;
        private short port;
        private BackendConf conf;

        public GameServer(BackendConf conf)
        {
            this.conf = conf;
            ip = conf.host;
            port = conf.port;
        }

        public void StartUp()
        {
            server = new Server();
            incomming = new Incoming(this);

            // register message callback sink
            server.RegisterClose(OnConnectionClose);

            // load assets before create game world and receive client connections
            LoadAssets();

            // start the server, block till program exit
            server.Start(ip, port);
        }

        public void Register(Command command, MessageDelegate @delegate)
        {
            server.RegisterMessageRecv(command, @delegate);
        }

        private void OnConnectionClose(IChannel channel)
        {
            if (channel.GetContent() != null)
            {
                Entity entity = (Entity)channel.GetContent();
                World.Instance().RemoveEntity(entity.entityId);
            }

            ((Channel)channel).workSocket.Shutdown(SocketShutdown.Both);

        }

        private void LoadAssets()
        {
            foreach (string sceneName in conf.scenes)
            {// foreach scene assets, TODO
                XmlSerializer serializer = new XmlSerializer(typeof(DSceneAsset));
                StreamReader reader = new StreamReader(conf.assestPath + "/" + sceneName);
                DSceneAsset asset = (DSceneAsset)serializer.Deserialize(reader);
                World.Instance().LoadScene(asset);
            }
        }
    }
}
