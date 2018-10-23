using UnityEngine;
using Common;

namespace Gamekit3D.Network
{
    public partial class Incoming
    {
        private void OnRecvTakeItem(IChannel channel, Message message)
        {
            SPlayerTake msg = (SPlayerTake)message;
            NetworkEntity item;
            if (msg.clone)
            {
                GameObject go = CloneGameObject(msg.name, msg.itemID);
                if (go == null)
                    return;
                item = networkEntities[msg.itemID];
            }
            else
            {
                item = networkEntities[msg.itemID];
            }
            thisPlayer.TakeItem(item);
        }
    }
}
