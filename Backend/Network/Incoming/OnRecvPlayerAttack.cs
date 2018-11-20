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
                Entity target = World.Instance.GetEntity(request.target);
                if (target is Sprite)
                {
                    Sprite sprite = (Sprite)target;
                    // player  attack the sprite
                    sprite.OnHit(player, 1);
                    player.OnAttack(sprite);
                }
            }
            else
            {
                player.OnAttack(null);
            }
        }
    }
}
