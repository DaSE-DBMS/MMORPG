﻿using UnityEngine;
using UnityEngine.SceneManagement;

using Gamekit3D.Network;

public class GameStartUp : MonoBehaviour
{
    public void OnStart()
    {
        // ... TODO get ip && port from configure file
        MyNetwork.Connect("127.0.0.1", 7777);
    }
}
