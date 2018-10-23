using Common;

namespace Gamekit3D.Network
{
    public partial class Incoming
    {
        private void OnRecvMove(IChannel channel, Message message)
        {
            SMove msg = (SMove)message;
            NetworkEntity self = networkEntities[msg.ID];
            if (self.creatureBehavior == null)
                return;

            switch (msg.state)
            {
                case MoveState.BEGIN:
                    {
                        self.creatureBehavior.MoveBegin(msg.move, msg.pos, msg.rot);
                        break;
                    }
                case MoveState.STEP:
                    {
                        self.creatureBehavior.MoveStep(msg.move, msg.pos, msg.rot);
                    }
                    break;
                case MoveState.END:
                    {
                        self.creatureBehavior.MoveEnd(msg.move, msg.pos, msg.rot);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
