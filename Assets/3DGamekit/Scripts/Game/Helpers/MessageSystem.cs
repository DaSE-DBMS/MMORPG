using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{

    public enum DamageMsgType
    {
        DAMAGED,
        DEAD,
        RESPAWN,
        //Add your user defined message type after
    }

    public interface IMessageReceiver
    {
        void OnReceiveMessage(DamageMsgType type, object sender, object msg);
    }
}
