using Common;

namespace Gamekit3D.Network
{
    public partial class Incoming
    {
        private void OnRecvAttack(IChannel channel, Message message)
        {
            SAttack msg = message as SAttack;
            NetworkEntity source = networkEntities[msg.ID];
            if (msg.targetID != 0)
            {
                NetworkEntity target = networkEntities[msg.targetID];
                source.behavior.Attack(target.behavior);
            }
            else
            {
                source.behavior.Attack(null);
            }
        }
    }
}
