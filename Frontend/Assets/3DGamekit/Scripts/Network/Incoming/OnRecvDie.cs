using Common;

namespace Gamekit3D.Network
{
    public partial class Incoming
    {
        private void OnRecvDie(IChannel channel, Message message)
        {
            SDie msg = message as SDie;
            NetworkEntity target = networkEntities[msg.entityId];
            target.behavior.Die();
        }
    }
}
