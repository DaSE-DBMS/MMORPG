using Common;

namespace Gamekit3D.Network
{
    public partial class Incoming
    {

        private void OnRecvJump(IChannel channel, Message message)
        {
            SJump msg = (SJump)message;
            NetworkEntity self = networkEntities[msg.ID];
            if (self.behavior == null)
                return;

            self.behavior.Jump();
        }

    }
}
