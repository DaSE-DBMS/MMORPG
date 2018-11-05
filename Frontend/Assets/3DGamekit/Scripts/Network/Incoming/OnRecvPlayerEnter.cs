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
            GameStartUp startup = GameObject.FindObjectOfType<GameStartUp>();
            if (network.gameScene)
            {// ignore enter scene message when debug mode
                return;
            }
            //Console.WriteLine("Receive Enter...");
            SPlayerEnter enter = (SPlayerEnter)message;
            startup.LoadScene("Level1");

        }
    }
}
