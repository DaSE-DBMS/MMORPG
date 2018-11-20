using Common;

namespace Gamekit3D.Network
{
    public partial class Incoming
    {
        private void OnRecvDie(IChannel channel, Message message)
        {
            SSpriteDie msg = message as SSpriteDie;
            NetworkEntity target = networkEntities[msg.entityId];
            target.behavior.Die();
        }
    }
}
