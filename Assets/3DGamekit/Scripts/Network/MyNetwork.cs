using UnityEngine;
using UnityEngine.SceneManagement;

namespace Frontend.Network
{

    public class MyNetwork : MonoBehaviour
    {
        public static MyNetwork instance;
        static private Incomming incomming = new Incomming(Client.Instance());
        static private bool connected = false;

        [Tooltip("auto connect to localhost")]
        public bool autoConnect = false;

        [Tooltip("auto connect to localhost")]
        public bool debug = false;

        [Tooltip("address")]
        public string address = "127.0.0.1";

        [Tooltip("port")]
        public short port = 7777;

        void Awake()
        {
            instance = this;
            if (autoConnect)
            {
                Connect(address, port);
            }
            //
            SceneManager.sceneLoaded += RecvSceneLoaded;
        }
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            Client.Instance().RecvMessage();
        }

        // Use this for initialization
        public void Connect(string ip, short port)
        {
            if (!connected)
            { // exactly connect once
                Client.Instance().Connect(ip, port);
                CLogin cLogin = new CLogin();
                cLogin.user = "ybbh";
                cLogin.password = "123456";
                Client.Instance().Send(cLogin);
                connected = true;
            }
        }

        public void Send(Message message)
        {
            Client.Instance().Send(message);
        }
        public void Register(Command command, MessageDelegate @delegate)
        {
            Client.Instance().Register(command, @delegate);
        }

        public void Close()
        {
            Client.Instance().Close();
        }

        private void RecvSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            NetworkEntity[] all = GameObject.FindObjectsOfType<NetworkEntity>();
            foreach (NetworkEntity entity in all)
            {
                GameObject go = entity.gameObject;
                go.SetActive(false);
                incomming.networkObjects[go.name] = go;
            }
            CEnterSceneDone message = new CEnterSceneDone();
            Client.Instance().Send(message);
        }
    }
}
