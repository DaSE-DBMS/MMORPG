using Common;
using Backend.Game;

namespace Backend.Network
{
    public partial class Incoming
    {
        private void OnRecvPlayerAttack(IChannel channel, Message message)
        {
            CPlayerAttack request = message as CPlayerAttack;
            Player player = (Player)World.Instance.GetEntity(request.player);
            if (request.target != 0)
            {
                Creature target = World.Instance.GetEntity(request.target) as Creature;
                if (target != null)
                {
                    // player attack other creature
                    target.OnHit(player, 1);
                    player.OnAttack(target);
                }
            }
            else
            {
                player.OnAttack(null);
            }
        }
    }
}
