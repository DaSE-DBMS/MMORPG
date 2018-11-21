using UnityEngine;
using Common;

namespace Gamekit3D.Network
{
    public partial class Incoming
    {
        private void OnRecvPlayerReSpawn(IChannel channel, Message message)
        {
            SPlayerReSpawn msg = message as SPlayerReSpawn;
            NetworkEntity target = networkEntities[msg.entityId];
            Vector3 position = Vector3.zero;
            position.x = msg.position.x;
            position.y = msg.position.y;
            position.z = msg.position.z;
            Quaternion rotation = new Quaternion();
            rotation.x = msg.rotation.x;
            rotation.y = msg.rotation.y;
            rotation.z = msg.rotation.z;
            rotation.w = msg.rotation.w;
            target.gameObject.SetActive(true);
            target.behavior.ReSpawn(msg.HP, position, rotation);
        }
    }
}
