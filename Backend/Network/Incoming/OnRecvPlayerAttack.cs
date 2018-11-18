using Common;
using Backend.Game;

namespace Backend.Network
{
    public partial class Incoming
    {
        private void RecvPlayerAttack(IChannel channel, Message message)
        {
            CPlayerAttack request = message as CPlayerAttack;
            Player player = (Player)World.Instance().GetEntity(request.player);
            if (request.target != 0)
            {
                Entity target = World.Instance().GetEntity(request.target);
                if (target is Sprite)
                {
                    Sprite sprite = (Sprite)target;
                    // player  attack the sprite
                    sprite.BeHit(player);
                    player.Attack(sprite);
                }
            }
            else
            {
                player.Attack(null);
            }
        }
    }
}
