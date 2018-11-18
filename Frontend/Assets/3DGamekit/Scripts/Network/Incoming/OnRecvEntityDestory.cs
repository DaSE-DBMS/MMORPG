using UnityEngine;
using Common;

namespace Gamekit3D.Network
{
    public partial class Incoming
    {
        private void OnRecvEntityDestory(IChannel channel, Message message)
        {
            SEntityDestroy msg = message as SEntityDestroy;
            GameObject go = gameObjects[msg.entityID];
            GameObject.Destroy(go);
        }
    }
}
