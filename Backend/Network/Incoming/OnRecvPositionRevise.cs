using Common;
using Backend.Game;

namespace Backend.Network
{
    public partial class Incoming
    {
        private void RecvPositionRevise(IChannel channel, Message message)
        {
            CPositionRevise request = (CPositionRevise)message;
            Entity entity = World.Instance().GetEntity(request.entityId);
            entity.Position.X = request.pos.x;
            entity.Position.Y = request.pos.y;
            entity.Position.Z = request.pos.z;
        }
    }
}
