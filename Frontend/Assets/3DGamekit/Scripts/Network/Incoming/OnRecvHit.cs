using Common;

namespace Gamekit3D.Network
{
    public partial class Incoming
    {
        private void OnRecvHit(IChannel channel, Message message)
        {
            SHit msg = (SHit)message;

            NetworkEntity target = networkEntities[msg.targetId];
            NetworkEntity source = null;
            if (msg.sourceId != 0)
            {
                source = networkEntities[msg.sourceId];
            }

            if (source.behavior == null)
                return;

            ICreatureBehavior srcBehavior = source == null ? null : source.behavior;
            ICreatureBehavior tarBehavior = target.behavior;
            tarBehavior.BeHit(msg.decHP, srcBehavior);

        }
    }
}
