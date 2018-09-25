using UnityEngine;
using UnityEngine.SceneManagement;

using Frontend.Network;

public class GameStartUp : MonoBehaviour
{
    public void OnStart()
    {
        // ... TODO get ip && port from configure file
        MyNetwork.instance.Connect("127.0.0.1", 7777);
    }
}
