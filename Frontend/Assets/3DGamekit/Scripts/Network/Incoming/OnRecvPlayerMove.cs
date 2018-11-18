using Common;
using UnityEngine;

namespace Gamekit3D.Network
{
    public partial class Incoming
    {
        private void OnRecvPlayerMove(IChannel channel, Message message)
        {
            SPlayerMove msg = message as SPlayerMove;
            NetworkEntity self = networkEntities[msg.ID];
            IPlayerBehavior behavior = (IPlayerBehavior)(self.behavior);
            Vector2 move = new Vector2(msg.move.x, msg.move.y);
            Vector3 position = new Vector3(msg.pos.x, msg.pos.y, msg.pos.z);
            Quaternion rotation = new Quaternion(msg.rot.x, msg.rot.y, msg.rot.z, msg.rot.w);
            switch (msg.state)
            {
                case MoveState.BEGIN:
                    {
                        behavior.MoveBegin(move, position, rotation);
                        break;
                    }
                case MoveState.STEP:
                    {
                        behavior.MoveStep(move, position, rotation);
                        break;
                    }
                case MoveState.END:
                    {
                        behavior.MoveEnd(move, position, rotation);
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
