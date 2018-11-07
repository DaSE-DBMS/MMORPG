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
            NetworkEntity target = null;
            if (msg.targetId != 0)
            {
                target = networkEntities[msg.targetId];
            }
            ISpriteBehavior behavior = (ISpriteBehavior)(sprite.behavior);
            Vector3 position = new Vector3(msg.pos.x, msg.pos.y, msg.pos.z);
            switch (msg.state)
            {
                case MoveState.BEGIN:
                    {
                        if (msg.targetId != 0)
                            behavior.BeginChase(position, (ICreatureBehavior)target.behavior);
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
                            behavior.EndChase(position);
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
