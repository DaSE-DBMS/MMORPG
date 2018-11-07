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
            entity.Position = Entity.V3ToPoint3d(request.pos);
        }
    }
}
