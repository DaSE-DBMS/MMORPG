using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{

    public enum MsgType
    {
        DAMAGED,
        DEAD,
        RESPAWN,
        //Add your user defined message type after
        BEGIN_CHASE,
        END_CHASE,
        BEGIN_BACK,
        END_BACK,
        MOVE,
        ATTACK,
        HIT
    }

    public interface IMessageReceiver
    {
        void OnReceiveMessage(MsgType type, object sender, object msg);
    }
}
