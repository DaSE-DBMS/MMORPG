using Common;

namespace Gamekit3D.Network
{
    public partial class Incoming
    {
        private void OnRecvUnderAttack(IChannel channel, Message message)
        {
            SUnderAttack msg = (SUnderAttack)message;

            NetworkEntity target = networkEntities[msg.ID];
            NetworkEntity source = null;
            if (msg.sourceID != 0)
            {
                source = networkEntities[msg.sourceID];
            }

            if (source.creatureBehavior == null || target.creatureBehavior == null)
                return;

            target.creatureBehavior.UnderAttack(msg.HP, source.creatureBehavior);

        }
    }
}
