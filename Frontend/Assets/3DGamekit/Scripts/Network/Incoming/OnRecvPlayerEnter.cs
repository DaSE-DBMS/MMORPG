using Common;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Gamekit3D.Network
{
    public partial class Incoming
    {
        private void OnRecvPlayerEnter(IChannel channel, Message message)
        {
            MyNetwork network = GameObject.FindObjectOfType<MyNetwork>();
            GameStart startup = GameObject.FindObjectOfType<GameStart>();
            if (network.gameScene)
            {// ignore enter scene message when debug mode
                return;
            }
            //Console.WriteLine("Receive Enter...");
            SPlayerEnter msg = (SPlayerEnter)message;
            startup.PlayerEnter(msg.scene);
        }
    }
}
