using Common;
using UnityEngine.SceneManagement;

namespace Gamekit3D.Network
{
    public partial class Incoming
    {
        private void OnRecvPlayerEnter(IChannel channel, Message message)
        {
            if (debug)
            {// ignore enter scene message when debug mode
                return;
            }
            //Console.WriteLine("Receive Enter...");
            SPlayerEnter enter = (SPlayerEnter)message;
            SceneManager.LoadSceneAsync("Level1");
        }
    }
}
