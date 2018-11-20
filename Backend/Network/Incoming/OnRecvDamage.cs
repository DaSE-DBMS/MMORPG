using Common;
using Backend.Game;

namespace Backend.Network
{
    public partial class Incoming
    {
        private void OnRecvDamage(IChannel channel, Message message)
        {
            CDamage request = message as CDamage;
            Creature creature = World.Instance.GetEntity(request.entityId) as Creature;
            if (creature == null)
                return;

            creature.OnHit(null, request.decHP);
        }
    }
}
