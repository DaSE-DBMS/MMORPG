using System.Xml.Serialization;
using System.IO;
using Backend.Game;
using Backend.AI;
using Common.Data;
using System.Diagnostics;

namespace Backend.Network
{
    public class GameServer : IRegister
    {
        private Server server;
        private Incomming incomming;
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
            World.Instance().RemoveEntity(((Entity)channel.GetContent()).entityID);
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
