using Common;
using UnityEngine;

namespace Gamekit3D.Network
{
    public partial class Incoming
    {
        private void OnRecvSpriteMove(IChannel channel, Message message)
        {
            SSpriteMove msg = (SSpriteMove)message;
            NetworkEntity sprite = networkEntities[msg.ID];
            ISpriteBehavior behavior = (ISpriteBehavior)(sprite.creatureBehavior);
            Vector3 position = new Vector3(msg.pos.x, msg.pos.y, msg.pos.z);
            switch (msg.state)
            {
                case MoveState.BEGIN:
                    {
                        if (msg.targetId != 0)
                            behavior.BeginChase(position, msg.targetId);
                        else
                            behavior.BeginBack(position);
                        break;
                    }
                case MoveState.STEP:
                    {
                        behavior.MoveStep(position);
                        break;
                    }
                case MoveState.END:
                    {
                        if (msg.targetId != 0)
                            behavior.EndChase(position, msg.targetId);
                        else
                            behavior.EndBack(position);
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
