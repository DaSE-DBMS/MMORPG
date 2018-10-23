using UnityEngine;
using UnityEngine.SceneManagement;
using Common;

namespace Gamekit3D.Network
{
    public class MyNetwork : MonoBehaviour
    {
        static private Incoming incomming = new Incoming(Client.Instance());
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
        static public void Connect(string ip, short port)
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

        static public void Send(Message message)
        {
            Client.Instance().Send(message);
        }
        static public void Register(Command command, MessageDelegate @delegate)
        {
            Client.Instance().Register(command, @delegate);
        }

        static public void Close()
        {
            Client.Instance().Close();
        }

        static private void RecvSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            incomming.InitNetworkEntity();
            CPlayerEnter message = new CPlayerEnter();
            Client.Instance().Send(message);
        }
    }
}
