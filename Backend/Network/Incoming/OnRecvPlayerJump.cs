using Common;
using Backend.Game;

namespace Backend.Network
{
    public partial class Incoming
    {
        private void RecvPlayerJump(IChannel channel, Message message)
        {
            CPlayerJump request = message as CPlayerJump;
            Player player = (Player)World.Instance().GetEntity(request.player);
            SJump response = new SJump();
            response.ID = request.player;
            player.Broadcast(response);
        }
    }
}
