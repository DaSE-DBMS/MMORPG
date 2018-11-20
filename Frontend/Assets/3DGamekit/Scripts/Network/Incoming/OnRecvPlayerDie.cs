using Common;

namespace Gamekit3D.Network
{
    public partial class Incoming
    {
        private void OnRecvPlayerDie(IChannel channel, Message message)
        {
            SPlayerDie msg = message as SPlayerDie;
            NetworkEntity target = networkEntities[msg.entityId];

            target.behavior.Die();
        }
    }
}
