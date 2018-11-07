using Common;
using Backend.Game;

namespace Backend.Network
{
    public partial class Incoming
    {
        private void RecvPlayerMove(IChannel channel, Message message)
        {
            CPlayerMove request = (CPlayerMove)message;
            Player player = (Player)World.Instance().GetEntity(request.player);
            player.Position = Entity.V3ToPoint3d(request.pos);
            SPlayerMove response = new SPlayerMove();
            response.ID = request.player;
            response.state = request.state;
            response.pos = request.pos;
            response.rot = request.rot;
            response.move = request.move;
            response.state = request.state;
            player.Broadcast(response);
        }
    }
}
